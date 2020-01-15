using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;

namespace CryptoTransaction
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var encodingProvider = CodePagesEncodingProvider.Instance;
            Encoding.RegisterProvider(encodingProvider);

            using var stream = File.Open(@"C:\Users\joecr\Desktop\transaction_history.xlsx", FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);

            var transactions = new List<Transaction>();
            reader.Read();
            do
            {
                while (reader.Read())
                {
                    if(reader.GetValue(0) == null)
                    {
                        continue;
                    }

                    transactions.Add(new Transaction
                    {
                        Date = reader.GetDateTime(0),
                        Time = reader.GetDateTime(1),
                        Type = reader.GetString(2),
                        Symbol = reader.GetString(3),
                        Specification = reader.GetString(4),
                        AmountUsd = double.Parse(reader.GetValue(7) == null ? "0.0" : reader.GetValue(7).ToString()),
                        AmountBtc = double.Parse(reader.GetValue(10) == null ? "0.0" : reader.GetValue(10).ToString()),
                        AmountEth = double.Parse(reader.GetValue(13) == null ? "0.0" : reader.GetValue(13).ToString()),
                        AmountZec = double.Parse(reader.GetValue(16) == null ? "0.0" : reader.GetValue(16).ToString()),
                        AmountBch = double.Parse(reader.GetValue(19) == null ? "0.0" : reader.GetValue(19).ToString()),
                        AmountLtc = double.Parse(reader.GetValue(22) == null ? "0.0" : reader.GetValue(22).ToString()),
                        FeeUsd = double.Parse(reader.GetValue(8) == null ? "0.0" : reader.GetValue(8).ToString()),
                        FeeBtc = double.Parse(reader.GetValue(11) == null ? "0.0" : reader.GetValue(11).ToString()),
                        FeeEth = double.Parse(reader.GetValue(14) == null ? "0.0" : reader.GetValue(14).ToString()),
                        FeeZec = double.Parse(reader.GetValue(17) == null ? "0.0" : reader.GetValue(17).ToString()),
                        FeeBch = double.Parse(reader.GetValue(20) == null ? "0.0" : reader.GetValue(20).ToString()),
                        FeeLtc = double.Parse(reader.GetValue(23) == null ? "0.0" : reader.GetValue(23).ToString()),
                        BalanceUsd = double.Parse(reader.GetValue(9) == null ? "0.0" : reader.GetValue(9).ToString()),
                        BalanceBtc = double.Parse(reader.GetValue(12) == null ? "0.0" : reader.GetValue(12).ToString()),
                        BalanceEth = double.Parse(reader.GetValue(15) == null ? "0.0" : reader.GetValue(15).ToString()),
                        BalanceZec = double.Parse(reader.GetValue(18) == null ? "0.0" : reader.GetValue(18).ToString()),
                        BalanceBch = double.Parse(reader.GetValue(21) == null ? "0.0" : reader.GetValue(21).ToString()),
                        BalanceLtc = double.Parse(reader.GetValue(24) == null ? "0.0" : reader.GetValue(24).ToString())
                    });
                }
                Console.WriteLine();
            } while (reader.NextResult());


            var cashDeposits = transactions.Where(t => t.Type == "Credit" && (t.Specification.Contains("ACH") || t.Specification.Contains("Wire"))).Sum(t => t.AmountUsd);
            var cashWithdrawals = transactions.Where(t => t.Type == "Debit" && (t.Specification.Contains("ACH") || t.Specification.Contains("Wire"))).Sum(t => t.AmountUsd);
            var netDeposit = cashDeposits + cashWithdrawals;
            Console.WriteLine($"Total Cash Deposits:    {cashDeposits}");
            Console.WriteLine($"Total Cash Withdrawals: {cashWithdrawals}");
            Console.WriteLine($"Net Deposit:            {netDeposit}");

            var btcDeposits = transactions.Where(t => t.Type == "Credit" && t.Specification.Contains("BTC")).Sum(t => t.AmountBtc);
            var btcWithdrawals = transactions.Where(t => t.Type == "Debit" && t.Specification.Contains("BTC")).Sum(t => t.AmountBtc);
            var btcNetDeposit = btcDeposits + btcWithdrawals;

            var ethDeposits = transactions.Where(t => t.Type == "Credit" && t.Specification.Contains("ETH")).Sum(t => t.AmountEth);
            var ethWithdrawals = transactions.Where(t => t.Type == "Debit" && t.Specification.Contains("ETH")).Sum(t => t.AmountEth);
            var ethNetDeposit = ethDeposits + ethWithdrawals;

            var ltcDeposits = transactions.Where(t => t.Type == "Credit" && t.Specification.Contains("LTC")).Sum(t => t.AmountLtc);
            var ltcWithdrawals = transactions.Where(t => t.Type == "Debit" && t.Specification.Contains("LTC")).Sum(t => t.AmountLtc);
            var ltcNetDeposit = ltcDeposits + ltcWithdrawals;

            var bchDeposits = transactions.Where(t => t.Type == "Credit" && t.Specification.Contains("BCH")).Sum(t => t.AmountBch);
            var bchWithdrawals = transactions.Where(t => t.Type == "Debit" && t.Specification.Contains("BCH")).Sum(t => t.AmountBch);
            var bchNetDeposit = bchDeposits + bchWithdrawals;

            var zecDeposits = transactions.Where(t => t.Type == "Credit" && t.Specification.Contains("ZEC")).Sum(t => t.AmountZec);
            var zecWithdrawals = transactions.Where(t => t.Type == "Debit" && t.Specification.Contains("ZEC")).Sum(t => t.AmountZec);
            var zecNetDeposit = zecDeposits + zecWithdrawals;

            Console.WriteLine($"Net BTC Deposit: {btcNetDeposit}");
            Console.WriteLine($"Net ETH Deposit: {ethNetDeposit}");
            Console.WriteLine($"Net ZEC Deposit: {zecNetDeposit}");
            Console.WriteLine($"Net BCH Deposit: {bchNetDeposit}");
            Console.WriteLine($"Net LTC Deposit: {ltcNetDeposit}");
            Console.WriteLine();

            var btcPurchase = transactions.Where(t => t.Type == "Buy" && t.Symbol == "BTCUSD").Sum(t => t.AmountUsd - t.FeeUsd);
            var btcSell = transactions.Where(t => t.Type == "Sell" && t.Symbol == "BTCUSD").Sum(t => t.AmountUsd + t.FeeUsd);

            var ethPurchase = transactions.Where(t => t.Type == "Buy" && t.Symbol == "ETHUSD").Sum(t => t.AmountUsd - t.FeeUsd);
            var ethSell = transactions.Where(t => t.Type == "Sell" && t.Symbol == "ETHUSD").Sum(t => t.AmountUsd + t.FeeUsd);

            var zecPurchase = transactions.Where(t => t.Type == "Buy" && t.Symbol == "ZECUSD").Sum(t => t.AmountUsd - t.FeeUsd);
            var zecSell = transactions.Where(t => t.Type == "Sell" && t.Symbol == "ZECUSD").Sum(t => t.AmountUsd + t.FeeUsd);

            var bchPurchase = transactions.Where(t => t.Type == "Buy" && t.Symbol == "BCHUSD").Sum(t => t.AmountUsd - t.FeeUsd);
            var bchSell = transactions.Where(t => t.Type == "Sell" && t.Symbol == "BCHUSD").Sum(t => t.AmountUsd + t.FeeUsd);

            var ltcPurchase = transactions.Where(t => t.Type == "Buy" && t.Symbol == "LTCUSD").Sum(t => t.AmountUsd - t.FeeUsd);
            var ltcSell = transactions.Where(t => t.Type == "Sell" && t.Symbol == "LTCUSD").Sum(t => t.AmountUsd + t.FeeUsd);

            Console.WriteLine($"BTC Purchases: {btcPurchase}");
            Console.WriteLine($"BTC Sales:     {btcSell}");
            Console.WriteLine();
            Console.WriteLine($"ETH Purchases: {ethPurchase}");
            Console.WriteLine($"ETH Sales:     {ethSell}");
            Console.WriteLine();
            Console.WriteLine($"ZEC Purchases: {zecPurchase}");
            Console.WriteLine($"ZEC Sales:     {zecSell}");
            Console.WriteLine();
            Console.WriteLine($"BCH Purchases: {bchPurchase}");
            Console.WriteLine($"BCH Sales:     {bchSell}");
            Console.WriteLine();
            Console.WriteLine($"LTC Purchases: {ltcPurchase}");
            Console.WriteLine($"LTC Sales:     {ltcSell}");

            Console.WriteLine();
            Console.WriteLine($"Total Transaction Volume: {btcPurchase - btcSell + ethPurchase - ethSell + zecPurchase - zecSell + bchPurchase - bchSell + ltcPurchase - ltcSell}");

            var btcHoldings = transactions.Last().BalanceBtc;
            var ethHoldings = transactions.Last().BalanceEth;
            var zecHoldings = transactions.Last().BalanceZec;
            var bchHoldings = transactions.Last().BalanceBch;
            var ltcHoldings = transactions.Last().BalanceLtc;
            var usdHoldings = transactions.Last().BalanceUsd;

            Console.WriteLine();
            Console.WriteLine($"BTC Current Holdings:   {btcHoldings}");
            Console.WriteLine($"ETH Current Holdings:   {ethHoldings}");
            Console.WriteLine($"ZEC Current Holdings:   {zecHoldings}");
            Console.WriteLine($"BCH Current Holdings:   {bchHoldings}");
            Console.WriteLine($"LTC Current Holdings:   {ltcHoldings}");
            Console.WriteLine($"USD Current Holdings:   {usdHoldings}");

            var balanceUsd = 0.0;
            var balanceBtc = 0.0;
            var balanceEth = 0.0;
            var balanceZec = 0.0;
            var balanceBch = 0.0;
            var balanceLtc = 0.0;


            foreach(var transaction in transactions)
            {
                balanceUsd += transaction.AmountUsd + transaction.FeeUsd;
                balanceBtc += transaction.AmountBtc + transaction.FeeBtc;
                balanceEth += transaction.AmountEth + transaction.FeeEth;
                balanceZec += transaction.AmountZec + transaction.FeeZec;
                balanceBch += transaction.AmountBch + transaction.FeeBch;
                balanceLtc += transaction.AmountLtc + transaction.FeeLtc;
            }

            Console.WriteLine();
            Console.WriteLine($"Final Balance USD: {balanceUsd}");
            Console.WriteLine($"Final Balance BTC: {balanceBtc}");
            Console.WriteLine($"Final Balance ETH: {balanceEth}");
            Console.WriteLine($"Final Balance ZEC: {balanceZec}");
            Console.WriteLine($"Final Balance BCH: {balanceBch}");
            Console.WriteLine($"Final Balance LTC: {balanceLtc}");

            using var client = new HttpClient();
            var uri = "https://production.api.coindesk.com/v2/price/ticker?assets=BCH,BTC,ETH,LTC,ZEC";
            var response = client.GetAsync(uri).GetAwaiter().GetResult();
            var resultStream = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var priceData = JsonSerializer.Deserialize<PriceData>(resultStream);
            

            Console.WriteLine();

            var holdingsValueBtc = priceData.data["BTC"].ohlc.c * balanceBtc;
            var holdingsValueEth = priceData.data["ETH"].ohlc.c * balanceEth;
            var holdingsValueZec = priceData.data["ZEC"].ohlc.c * balanceZec;
            var holdingsValueBch = priceData.data["BCH"].ohlc.c * balanceBch;
            var holdingsValueLtc = priceData.data["LTC"].ohlc.c * balanceLtc;

            var holdingsValue = holdingsValueBch + holdingsValueBtc + holdingsValueEth
                + holdingsValueLtc + holdingsValueZec;

            Console.WriteLine($"Realized Gains: {usdHoldings - netDeposit}");
            Console.WriteLine($"Gains: {usdHoldings + holdingsValue - netDeposit}");

            Console.ReadKey();
        }
    }

    class PriceData
    {
        public int statusCode { get; set; }
        public string message { get; set; }
        public Dictionary<string, CoinInfo> data { get; set; }
    }

    class CoinInfo
    {
        public string iso { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public PriceChange change { get; set; }
        public CandleInfo ohlc { get; set; }
        public double circulatingSupply { get; set; }
        public double marketCap { get; set; }
    }

    class PriceChange
    {
        public double percent { get; set; }
        public double value { get; set; }
    }

    class CandleInfo
    {
        public double o { get; set; }
        public double h { get; set; }
        public double l { get; set; }
        public double c { get; set; }

    }
}
