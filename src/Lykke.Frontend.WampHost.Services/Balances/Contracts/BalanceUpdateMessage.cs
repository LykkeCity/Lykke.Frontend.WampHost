﻿using System.ComponentModel;
using Newtonsoft.Json;

namespace Lykke.Frontend.WampHost.Services.Balances.Contracts
{
    public class BalanceUpdateMessage
    {
        [DisplayName("Asset (USD...)")]
        [JsonProperty("a")]
        public string Asset { get; set; }

        [DisplayName("Balance")]
        [JsonProperty("b")]
        public double Balance { get; set; }

        [DisplayName("Reserved")]
        [JsonProperty("r")]
        public double Reserved { get; set; }
    }
}
