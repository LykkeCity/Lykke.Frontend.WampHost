using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace Lykke.Frontend.WampHost.Services.Security
{
    internal class PostConfigureOAuth2IntrospectionOptions 
    {
        public void PostConfigure(OAuth2IntrospectionOptions options)
        {
            options.Validate();

            options.IntrospectionClient = new AsyncLazy<IntrospectionClient>(() => InitializeIntrospectionClient(options));
            options.LazyIntrospections = new ConcurrentDictionary<string, AsyncLazy<IntrospectionResponse>>();
        }

        private static async Task<string> GetIntrospectionEndpointFromDiscoveryDocument(OAuth2IntrospectionOptions options)
        {
            DiscoveryClient client;

            if (options.DiscoveryHttpHandler != null)
            {
                client = new DiscoveryClient(options.Authority, options.DiscoveryHttpHandler);
            }
            else
            {
                client = new DiscoveryClient(options.Authority);
            }

            client.Timeout = options.DiscoveryTimeout;
            client.Policy = options.DiscoveryPolicy ?? new DiscoveryPolicy();
            
            var discoveryResponse = await client.GetAsync().ConfigureAwait(false);
            if (discoveryResponse.IsError)
            {
                if (discoveryResponse.ErrorType == ResponseErrorType.Http)
                {
                    throw new InvalidOperationException($"Discovery endpoint {client.Url} is unavailable: {discoveryResponse.Error}");
                }
                if (discoveryResponse.ErrorType == ResponseErrorType.PolicyViolation)
                {
                    throw new InvalidOperationException($"Policy error while contacting the discovery endpoint {client.Url}: {discoveryResponse.Error}");
                }
                if (discoveryResponse.ErrorType == ResponseErrorType.Exception)
                {
                    throw new InvalidOperationException($"Error parsing discovery document from {client.Url}: {discoveryResponse.Error}");
                }
            }

            return discoveryResponse.IntrospectionEndpoint;
        }

        private static async Task<IntrospectionClient> InitializeIntrospectionClient(OAuth2IntrospectionOptions options)
        {
            string endpoint;

            if (IsPresent(options.IntrospectionEndpoint))
            {
                endpoint = options.IntrospectionEndpoint;
            }
            else
            {
                endpoint = await GetIntrospectionEndpointFromDiscoveryDocument(options).ConfigureAwait(false);
                options.IntrospectionEndpoint = endpoint;
            }

            IntrospectionClient client;
            if (options.IntrospectionHttpHandler != null)
            {
                client = new IntrospectionClient(
                    endpoint,
                    headerStyle: options.BasicAuthenticationHeaderStyle,
                    innerHttpMessageHandler: options.IntrospectionHttpHandler);
            }
            else
            {
                client = new IntrospectionClient(endpoint);
            }

            client.Timeout = options.DiscoveryTimeout;
            return client;
        }

        private static bool IsPresent(string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }
    }
}
