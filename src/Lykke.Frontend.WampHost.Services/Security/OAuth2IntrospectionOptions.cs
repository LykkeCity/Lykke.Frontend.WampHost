using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Http;
using IdentityModel;
using IdentityModel.Client;

namespace Lykke.Frontend.WampHost.Services.Security
{
    /// <summary>
    /// Options class for the OAuth 2.0 introspection endpoint authentication handler
    /// </summary>
    public class OAuth2IntrospectionOptions
    {

        /// <summary>
        /// Sets the base-path of the token provider.
        /// If set, the OpenID Connect discovery document will be used to find the introspection endpoint.
        /// </summary>
        public string Authority { get; set; }

        /// <summary>
        /// Sets the URL of the introspection endpoint.
        /// If set, Authority is ignored.
        /// </summary>
        public string IntrospectionEndpoint { get; set; }

        /// <summary>
        /// Specifies the id of the introspection client.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Specifies the shared secret of the introspection client.
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Specifies the token type hint of the introspection client.
        /// </summary>
        public string TokenTypeHint { get; set; } = OidcConstants.TokenTypes.AccessToken;

        /// <summary>
        /// Specifies the policy for the discovery client
        /// </summary>
        public DiscoveryPolicy DiscoveryPolicy { get; set; } = new DiscoveryPolicy();

        /// <summary>
        /// Gets or sets the basic authentication header style (RFC6749 vs RFC2617). Defaults to RFC6749.
        /// </summary>
        /// <value>
        /// The basic authentication header style.
        /// </value>
        public BasicAuthenticationHeaderStyle BasicAuthenticationHeaderStyle { get; set; } = BasicAuthenticationHeaderStyle.Rfc6749;

        /// <summary>
        /// Specifies the timout for contacting the discovery endpoint
        /// </summary>
        public TimeSpan DiscoveryTimeout { get; set; } = TimeSpan.FromSeconds(60);

        /// <summary>
        /// Specifies the HTTP handler for the discovery endpoint
        /// </summary>
        public HttpMessageHandler DiscoveryHttpHandler { get; set; }

        /// <summary>
        /// Specifies the HTTP handler for the introspection endpoint
        /// </summary>
        public HttpMessageHandler IntrospectionHttpHandler { get; set; }


        internal AsyncLazy<IntrospectionClient> IntrospectionClient { get; set; }
        internal ConcurrentDictionary<string, AsyncLazy<IntrospectionResponse>> LazyIntrospections { get; set; }

        /// <summary>
        /// Check that the options are valid. Should throw an exception if things are not ok.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// You must either set Authority or IntrospectionEndpoint
        /// or
        /// You must either set a ClientId or set an introspection HTTP handler
        /// </exception>
        /// <exception cref="ArgumentException">TokenRetriever must be set - TokenRetriever</exception>
        public void Validate()
        {

            if (IsMissing(Authority) && IsMissing(IntrospectionEndpoint))
            {
                throw new InvalidOperationException("You must either set Authority or IntrospectionEndpoint");
            }

            if (IsMissing(ClientId) && IntrospectionHttpHandler == null)
            {
                throw new InvalidOperationException("You must either set a ClientId or set an introspection HTTP handler");
            }
        }

        private static bool IsMissing(string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }
    }
}
