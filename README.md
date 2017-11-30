# Lykke.Frontend.WampHost

Wamp topics host, which provides real time data for the clients

## Topics

### Candles

* **Name**: candle.\<market\>.\<instrument\>.\<price-type\>.\<interval\>
  * **market**: *{spot|mt}*
  * **instrument**: Asset pair (BTCUSD, EURUSD...)
  * **price-type**: *{bid|ask|mid}*
  * **interval**: *{sec|minute|min5|min15|min30|hour|hour4|hour6|hour12|day|week|month}*
* **Realm**: prices
* **Object difinition**: Each item in the topic is the candle in the json format:
```js
{
  // Asset pair (instrument)
  "a",
  // Price type {Bid|Ask|Mid}
  "p",
  // Interval of the candle {Sec|Minute|Min5|Min15|Min30|Hour|Hour4|Hour6|Hour12|Day|Week|Month}
  "i",
  // Timestamp of the candle opening
  "t",
  // Open price
  "o",
  // Close price
  "c",
  // Highest price
  "h",
  // Lowest price
  "l"
}
```
