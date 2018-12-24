using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Core.Services.Clients;
using Lykke.Service.ClientAccount.Client;
using Lykke.Service.ClientAccount.Client.AutorestClient.Models;

namespace Lykke.Frontend.WampHost.Services.Clients
{
    public class ClientToWalletMapper : IClientToWalletMapper
    {
        private readonly IClientAccountClient _clientAccountClient;
        private readonly ConcurrentDictionary<string, string> _clientIdToTradingWalletIdMap = new ConcurrentDictionary<string, string>();
        private readonly ConcurrentDictionary<string, string> _tradingWalletIdToClientIdMap = new ConcurrentDictionary<string, string>();

        public ClientToWalletMapper([NotNull] IClientAccountClient clientAccountClient)
        {
            _clientAccountClient = clientAccountClient ?? throw new ArgumentNullException(nameof(clientAccountClient));
        }

        public async Task<(string, string)> GetClientIdAndWalletIdAsync(string id)
        {
            if (_clientIdToTradingWalletIdMap.TryGetValue(id, out var walletId))
                return (id, walletId);

            if (_tradingWalletIdToClientIdMap.TryGetValue(id, out var clientId))
                return (clientId, id);

            clientId = await _clientAccountClient.GetClientByWalletAsync(id);
            if (string.IsNullOrWhiteSpace(clientId))
                throw new InvalidOperationException($"Client not found by wallet {id}");

            if (clientId == id)
            {
                var wallets = await _clientAccountClient.GetClientWalletsByTypeAsync(clientId, WalletType.Trading);
                walletId = wallets?.FirstOrDefault(i => !string.IsNullOrWhiteSpace(i?.Id))?.Id;
                if (string.IsNullOrWhiteSpace(walletId))
                    throw new InvalidOperationException($"Trading wallet not found for client {clientId}");
            }
            else
            {
                walletId = id;
            }
            _clientIdToTradingWalletIdMap.TryAdd(clientId, walletId);
            _tradingWalletIdToClientIdMap.TryAdd(walletId, clientId);

            return (clientId, walletId);
        }
    }
}
