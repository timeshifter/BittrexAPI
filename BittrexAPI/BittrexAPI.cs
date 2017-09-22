using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BittrexAPI
{
    public static class BittrexAPI
    {
        private static async Task<string> CallAPI(string key, string secret, string method, string parameter)
        {
            string
                urlSign = "",
                url = $"https://bittrex.com/api/v1.1/{method}",
                nonce = DateTime.Now.Ticks.ToString();

            method = method.ToLower().Trim();
            parameter = parameter.Trim();

            WebClient wc = new WebClient()
            {
                Encoding = Encoding.UTF8
            };

            if (parameter != "")
            {
                url += $"?{parameter}";
            }

            if (!method.StartsWith("public"))
            {
                //add appropriate concatenator and private key
                url += (parameter == "" ? "?" : "&") + $"apikey={key}&nonce={nonce}";

                byte[] secretBytes = Encoding.UTF8.GetBytes(secret);
                using (var hmac = new HMACSHA512(secretBytes))
                {
                    //compute hash
                    byte[] hashResult = hmac.ComputeHash(Encoding.UTF8.GetBytes(url));

                    //convert to hex string
                    for (int i = 0; i < hashResult.Length; i++)
                        urlSign += hashResult[i].ToString("X2");
                }
                wc.Headers.Add("apisign", urlSign);
            }

            string result = await wc.DownloadStringTaskAsync(url);
            return result;
        }


        /* PUBLIC */

        public static async Task<APIResult<List<MarketSummaryResult>>> Public_GetMarkets()
        {
            string data = await CallAPI("", "", "public/getmarketsummary", "");
            return JsonConvert.DeserializeObject<APIResult<List<MarketSummaryResult>>>(data);
        }

        public static async Task<APIResult<List<CurrencyResult>>> Public_GetCurrencies()
        {
            string data = await CallAPI("", "", "public/getcurrencies", "");
            return JsonConvert.DeserializeObject<APIResult<List<CurrencyResult>>>(data);
        }

        public static async Task<APIResult<TickerResult>> Public_GetTicker(string market)
        {
            string data = await CallAPI("", "", "public/getticker", $"market={market}");
            return JsonConvert.DeserializeObject<APIResult<TickerResult>>(data);
        }

        public static async Task<APIResult<List<MarketSummaryResult>>> Public_GetMarketSummaries()
        {
            string data = await CallAPI("", "", "public/getmarketsummaries", "");
            return JsonConvert.DeserializeObject<APIResult<List<MarketSummaryResult>>>(data);
        }

        public static async Task<APIResult<List<MarketSummaryResult>>> Public_GetMarketSummary(string market)
        {
            string data = await CallAPI("", "", "public/getmarketsummary", $"market={market}");
            return JsonConvert.DeserializeObject<APIResult<List<MarketSummaryResult>>>(data);
        }

        public static async Task<APIResult<OrderBookResult>> Public_GetOrderBook(string market, string type)
        {
            string data = await CallAPI("", "", "public/getorderbook", $"market={market}&type={type}");
            return JsonConvert.DeserializeObject<APIResult<OrderBookResult>>(data);
        }

        public static async Task<APIResult<List<MarketHistoryResult>>> Public_GetMarketHistory(string market)
        {
            string data = await CallAPI("", "", "public/getmarkethistory", $"market={market}");
            return JsonConvert.DeserializeObject<APIResult<List<MarketHistoryResult>>>(data);
        }


        /* MARKET */

        public static async Task<APIResult<PlaceOrderResult>> Market_BuyLimit(string key, string secret, string market, string quantity, string rate)
        {
            string data = await CallAPI(key, secret, "market/buylimit", $"market={market}&quantity={quantity}&rate={rate}");
            return JsonConvert.DeserializeObject<APIResult<PlaceOrderResult>>(data);
        }

        public static async Task<APIResult<PlaceOrderResult>> Market_SellLimit(string key, string secret, string market, string quantity, string rate)
        {
            string data = await CallAPI(key, secret, "market/selllimit", $"market={market}&quantity={quantity}&rate={rate}");
            return JsonConvert.DeserializeObject<APIResult<PlaceOrderResult>>(data);
        }

        public static async Task<APIResult<string>> Market_Cancel(string key, string secret, string uuid)
        {
            string data = await CallAPI(key, secret, "market/selllimit", $"uuid={uuid}");
            return JsonConvert.DeserializeObject<APIResult<string>>(data);
        }

        public static async Task<APIResult<List<OpenOrderResult>>> Market_GetOpenOrders(string key, string secret, string market)
        {
            string data = await CallAPI(key, secret, "market/getopenorders", $"market={market}");
            return JsonConvert.DeserializeObject<APIResult<List<OpenOrderResult>>>(data);
        }


        /* ACCOUNT */

        public static async Task<APIResult<List<BalanceResult>>> Account_GetBalances(string key, string secret)
        {
            string data = await CallAPI(key, secret, "account/getbalances", "");
            return JsonConvert.DeserializeObject<APIResult<List<BalanceResult>>>(data);
        }

        public static async Task<APIResult<BalanceResult>> Account_GetBalance(string key, string secret, string currency)
        {
            string data = await CallAPI(key, secret, "account/getbalance", $"currency={currency}");
            return JsonConvert.DeserializeObject<APIResult<BalanceResult>>(data);
        }

        public static async Task<APIResult<DepositAddressResult>> Account_GetDepositAddress(string key, string secret, string currency)
        {
            string data = await CallAPI(key, secret, "account/getdepositaddress", $"currency={currency}");
            return JsonConvert.DeserializeObject<APIResult<DepositAddressResult>>(data);
        }

        public static async Task<APIResult<WithdrawResult>> Account_Withdraw(string key, string secret, string currency, decimal quantity, string address, string paymentid)
        {
            string data = await CallAPI(key, secret, "account/withdraw", $"currency={currency}&quantity={quantity}&address={address}&paymentid={paymentid}");
            return JsonConvert.DeserializeObject<APIResult<WithdrawResult>>(data);
        }



    }

    public class APIResult<T>
    {
        public bool success;
        public string message;
        public T result;
    }

    public class MarketResult
    {
        public string
            MarketCurrency,
            BaseCurrency,
            MarketCurrencyLong,
            BaseCurrencyLong,
            MarketName;

        public bool
            IsActive;

        public DateTime
            Created;

        public decimal
            MinTradeSize;
    }

    public class CurrencyResult
    {
        public string
            Currency,
            CurrencyLong,
            CoinType,
            BaseAddress;

        public int
            MinConfirmation;

        public decimal
            TxFee;

        public bool
            IsActive;
    }

    public class TickerResult
    {
        public decimal
            Bid,
            Ask,
            Last;
    }

    public class MarketSummaryResult
    {
        public string
            MarketName,
            BaseVolume,
            TimeStamp,
            OpenBuyOrders,
            OpenSellOrders,
            PrevDay,
            Created,
            DisplayMarketName;

        public decimal
            High,
            Low,
            Bid,
            Volume,
            Ask,
            Last;
    }

    public class OrderBookResult
    {
        public List<OrderBookItem>
            buy,
            sell;
    }

    public class OrderBookItem
    {
        public decimal
            Quantity,
            Rate;
    }

    public class MarketHistoryResult
    {
        public string
            FillType,
            OrderType;

        public int
            Id;

        public DateTime
            TimeStamp;

        public decimal
            Quantity,
            Price,
            Total;
    }

    public class PlaceOrderResult
    {
        public string
            uuid;
    }

    public class OpenOrderResult
    {
        public string
            Uuid,
            OrderUuid,
            Exchange,
            OrderType,
            Condition,
            ConditionTarget;

        public decimal
            Quantity,
            QuantityRemaining,
            Limit,
            CommissionPaid,
            Price,
            PricePerUnit;

        public DateTime
            Opened,
            Closed;

        public bool
            CancelInitiated,
            ImmediateOrCancel,
            IsConditional;
    }

    public class BalanceResult
    {
        public string
            Currency,
            CryptoAddress,
            Uuid;

        public decimal
            Balance,
            Available,
            Pending;

        public bool
            Requested;
    }

    public class DepositAddressResult
    {
        public string
            Currency,
            Address;
    }

    public class WithdrawResult
    {
        public string
            uuid;
    }

    public class OrderResult
    {

    }
}
