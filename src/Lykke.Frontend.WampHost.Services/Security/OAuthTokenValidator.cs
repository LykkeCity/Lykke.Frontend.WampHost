using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using IdentityModel.Client;
using JetBrains.Annotations;

namespace Lykke.Frontend.WampHost.Services.Security
{
    public class OAuthTokenValidator : IOAuthTokenValidator
    {
        private readonly OAuth2IntrospectionOptions _options;
        private readonly ILog _log;
        private const string SubjectClaimType = "sub";


        public OAuthTokenValidator([NotNull] OAuth2IntrospectionOptions options, ILog log)
        {
            var optionsConfigurator = new PostConfigureOAuth2IntrospectionOptions();
            _options = options;
            optionsConfigurator.PostConfigure(_options);
            _log = log.CreateComponentScope(nameof(OAuthTokenValidator));
        }

        public async Task<string> GetClientId(string token)
        {
            // Use a LazyAsync to ensure only one thread is requesting introspection for a token - the rest will wait for the result
            var lazyIntrospection = _options.LazyIntrospections.GetOrAdd(token, CreateLazyIntrospection);

            try
            {
                var response = await lazyIntrospection.Value.ConfigureAwait(false);

                if (response.IsError)
                {
                    _log.WriteWarning(nameof(GetClientId), null, "Error returned from introspection endpoint: " + response.Error, null);
                    return null;
                }

                if (response.IsActive)
                {
                    return response.Claims.FirstOrDefault(c => c.Type == SubjectClaimType)?.Value;
                }
                else
                {
                    return null;
                }
            }
            finally
            {
                // If caching is on and it succeeded, the claims are now in the cache.
                // If caching is off and it succeeded, the claims will be discarded.
                // Either way, we want to remove the temporary store of claims for this token because it is only intended for de-duping fetch requests
                _options.LazyIntrospections.TryRemove(token, out _);
            }
        }

        private AsyncLazy<IntrospectionResponse> CreateLazyIntrospection(string token)
        {
            return new AsyncLazy<IntrospectionResponse>(() => LoadClaimsForToken(token));
        }

        private async Task<IntrospectionResponse> LoadClaimsForToken(string token)
        {
            var introspectionClient = await _options.IntrospectionClient.Value.ConfigureAwait(false);

            return await introspectionClient.SendAsync(new IntrospectionRequest
            {
                Token = token,
                TokenTypeHint = _options.TokenTypeHint,
                ClientId = _options.ClientId,
                ClientSecret = _options.ClientSecret
            }).ConfigureAwait(false);
        }
    }

}
