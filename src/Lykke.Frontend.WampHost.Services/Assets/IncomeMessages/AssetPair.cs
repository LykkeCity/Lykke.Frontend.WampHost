namespace Lykke.Frontend.WampHost.Services.Assets.IncomeMessages
{
    public class AssetPair
    {
        public int Accuracy { get; set; }

        public string BaseAssetId { get; set; }

        public string Id { get; set; }

        public int InvertedAccuracy { get; set; }

        public bool IsDisabled { get; set; }

        public string Name { get; set; }

        public string QuotingAssetId { get; set; }

        public string Source { get; set; }

        public string Source2 { get; set; }

        /// <summary>
        /// Minimum volume of Limit or Market orders
        /// </summary>
        public double MinVolume { get; set; }

        /// <summary>
        /// Minimum volume of Limit or Market orders for inverted pair
        /// </summary>
        public double MinInvertedVolume { get; set; }
    }
}
