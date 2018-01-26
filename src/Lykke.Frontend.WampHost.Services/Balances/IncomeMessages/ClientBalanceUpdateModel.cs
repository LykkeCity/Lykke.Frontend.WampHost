namespace Lykke.Frontend.WampHost.Services.Balances.IncomeMessages
{
    public class ClientBalanceUpdateModel
    {
        /// <summary>Client ID</summary>
        public string Id { get; set; }
        public string Asset { get; set; }
        public double OldBalance { get; set; }
        public double NewBalance { get; set; }
        public double? OldReserved { get; set; }
        public double? NewReserved { get; set; }
    }
}
