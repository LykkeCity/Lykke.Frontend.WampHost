using System;
using System.Collections.Generic;

namespace Lykke.Frontend.WampHost.Services.Balances.IncomeMessages
{
    public class BalanceUpdateEventModel
    {
        /// <summary> Transaction ID </summary>
        public string Id { get; set; }
        public string Type { get; set; }
        public DateTime Timestamp { get; set; }
        public List<ClientBalanceUpdateModel> Balances { get; set; }
    }
}
