namespace Lykke.Frontend.WampHost.Services.Balances.Contracts
{
    public class BalanceUpdateModel
    {
        public string Asset { get; set; }
        public double OldBalance { get; set; }
        public double NewBalance { get; set; }
        public double? OldReserved { get; set; }
        public double? NewReserved { get; set; }
    }
}
