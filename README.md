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
  // Market {Spot|Mt}
  "m", 
  // Asset pair (instrument)
  "a",
  // Price type {Bid|Ask|Mid}
  "p",
  // Interval of the candle {Sec|Minute|Min5|Min15|Min30|Hour|Hour4|Hour6|Hour12|Day|Week|Month}
  "i",
  // Timestamp of the candle opening (ISO 8601 UTC)
  "t",
  // Open price
  "o",
  // Close price
  "c",
  // Highest price
  "h",
  // Lowest price
  "l",
  // Trading volume
  "v"
}
```

### Quotes

* **Name**: quote.\<market\>.\<instrument\>.\<price-type\>
  * **market**: *{spot|mt}*
  * **instrument**: Asset pair (BTCUSD, EURUSD...)
  * **price-type**: *{bid|ask}*
* **Realm**: prices
* **Object difinition**: Each item in the topic is the quote in the json format:
```js
{
  // Market {Spot|Mt}
  "m",
  // Asset pair (instrument)
  "a",
  // Price type {Bid|Ask}
  "pt",
  // Timestamp of the quote opening (ISO 8601 UTC)
  "t",
  // price
  "p"
}
```