namespace Lykke.Frontend.WampHost.Core.Orders.Contract
{
    public class Fee
    {
        public FeeInstruction Instruction { get; set; }

        public FeeTransfer Transfer { get; set; }
    }
}
