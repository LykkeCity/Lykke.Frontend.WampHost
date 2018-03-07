using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Services.Balances.Contracts;
using Lykke.Frontend.WampHost.Core.Services.Orderbook;
using Lykke.Frontend.WampHost.Services.Assets.Contracts;
using Lykke.Frontend.WampHost.Services.Candles.Contract;
using Lykke.Frontend.WampHost.Services.Commands;
using Lykke.Frontend.WampHost.Services.Quotes.Contract;
using Lykke.Frontend.WampHost.Services.TradesAnon.Contract;
using Lykke.Job.TradesConverter.Contract;
using MarginTrading.Contract.ClientContracts;

namespace Lykke.Frontend.WampHost.Services.Documentation
{
    public interface IWampTopics
    {
        [DocMe(Name = "candle.{spot|mt}.{instrument}.{bid|ask|mid|trades}.{sec|minute|min5|min15|min30|hour|hour4|hour6|hour12|day|week|month}", Description = "provides candles. realm = 'prices', all parameters in the lower case.")]
        CandleClientMessage CandlesUpdate();

        [UsedImplicitly]
        [DocMe(Name = "quote.{spot|mt}.{instrument}.{bid|ask}", Description = "provides quotes. realm = 'prices', all parameters in the lower case.")]
        QuoteClientMessage Quotes();

        [UsedImplicitly]
        [DocMe(Name = "balances", Description = "user balances updates. realm = 'prices', all parameters in the lower case.")]
        BalanceUpdateMessage BalancesUpdate();

        [UsedImplicitly]
        [DocMe(Name = "orderbook.{spot|mt}.{instrument}.{buy|sell}", Description = "provides orderbooks. realm = 'prices', all parameters in the lower case. Mt is not implemented yet.")]
        OrderbookModel Orderbooks();

        [UsedImplicitly]
        [DocMe(Name = "trades", Description = "provides trades for a specific user. realm = 'prices', all parameters in the lower case.")]
        TradeLogItem Trades();

        [UsedImplicitly]
        [DocMe(Name = "commands.sign", Description = "commands for a phone. PromoteSession - to sign user session by private key.")]
        SignCommand SignCommands();

        [UsedImplicitly]
        [DocMe(Name = "trades.mt", Description = "provides margin trades of all users. realm = 'prices'.")]
        TradeClientContract MtTrades();

        [UsedImplicitly]
        [DocMe(Name = "user-updates.mt", Description = "provides margin trading users and accounts updates. realm = 'prices'.")]
        NotifyResponse MtUserUpdates();

        [UsedImplicitly]
        [DocMe(Name = "trades.spot.{instrument}", Description = "provides trades for a instrument. realm = 'prices', all parameters in the lower case.")]
        TradeAnonClientMessage TradesAnon();

        [UsedImplicitly]
        [DocMe(Name = "assets", Description = "Assets updates. realm = 'prices'")]
        AssetUpdateMessage Assets();

        [UsedImplicitly]
        [DocMe(Name = "assetpairs", Description = "Asset-pairs updates. realm = 'prices'")]
        AssetPairUpdateMessage AssetsPairs();

    }
}
