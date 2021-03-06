﻿using JetBrains.Annotations;
using Lykke.Exchange.Api.MarketData.Contract;
using Lykke.Frontend.WampHost.Core.Services.HistoryExport;
using Lykke.Frontend.WampHost.Services.Balances.Contracts;
using Lykke.Frontend.WampHost.Core.Services.Orderbook;
using Lykke.Frontend.WampHost.Core.Services.Orders.OutgoingMessages;
using Lykke.Frontend.WampHost.Services.Assets.Contracts;
using Lykke.Frontend.WampHost.Services.Candles.Contract;
using Lykke.Frontend.WampHost.Services.Quotes.Contract;
using Lykke.Job.TradesConverter.Contract;
using Lykke.Service.IndicesFacade.Contract;
using Lykke.Service.TradesAdapter.Contract;
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
        [DocMe(Name = "trades.mt", Description = "provides margin trades of all users. realm = 'prices'.")]
        TradeClientContract MtTrades();

        [UsedImplicitly]
        [DocMe(Name = "user-updates.mt", Description = "provides margin trading users and accounts updates. realm = 'prices'.")]
        NotifyResponse MtUserUpdates();

        [UsedImplicitly]
        [DocMe(Name = "trades.spot.{instrument}", Description = "provides trades for a instrument. realm = 'prices', all parameters in the lower case.")]
        Trade TradesAnon();

        [UsedImplicitly]
        [DocMe(Name = "assets", Description = "Assets updates. realm = 'prices'")]
        AssetUpdateMessage Assets();

        [UsedImplicitly]
        [DocMe(Name = "assetpairs", Description = "Asset-pairs updates. realm = 'prices'")]
        AssetPairUpdateMessage AssetsPairs();

        [UsedImplicitly]
        [DocMe(Name = "orders.spot", Description = "Orders. realm = 'prices'")]
        Order[] Orders();
        
        [UsedImplicitly]
        [DocMe(Name = "history.export", Description = "Generated csv urls. realm = 'prices'")]
        HistoryExportGeneratedMessage HsistoryExports();
        
        [UsedImplicitly]
        [DocMe(Name = "indices.{AssetId}", Description = "Indices updates. realm = 'prices'")]
        Index Index();
        
        [UsedImplicitly]
        [DocMe(Name = "marketdata", Description = "Market data changes. realm = 'prices'")]
        MarketDataChangedEvent MarketDataChanges();

        [UsedImplicitly]
        [DocMe(Name = "marketdata.{assetPairId}", Description = "Market data changes by asset pair. realm = 'prices'")]
        MarketDataChangedEvent MarketDataAssetPairChanges();
    }
}
