using System;
using JetBrains.Annotations;

namespace Lykke.Frontend.WampHost.Services.Quotes.Mt.Messages
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public class MtQuoteMessage
    {
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string Instrument { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public DateTime Date { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public double Bid { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public double Ask { get; set; }
    }
}
