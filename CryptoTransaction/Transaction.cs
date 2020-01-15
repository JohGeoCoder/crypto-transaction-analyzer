using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTransaction
{
    public class Transaction
    {
        public DateTime Date { get; set; }
        public DateTime Time { get; set; }
        public string Type { get; set; }
        public string Symbol { get; set; }
        public string Specification { get; set; }
        public double AmountUsd { get; set; }
        public double AmountBtc { get; set; }
        public double AmountEth { get; set; }
        public double AmountZec { get; set; }
        public double AmountBch { get; set; }
        public double AmountLtc { get; set; }
        public double FeeUsd { get; set; }
        public double FeeBtc { get; set; }
        public double FeeEth { get; set; }
        public double FeeZec { get; set; }
        public double FeeBch { get; set; }
        public double FeeLtc { get; set; }
        public double BalanceUsd { get; set; }
        public double BalanceBtc { get; set; }
        public double BalanceEth { get; set; }
        public double BalanceZec { get; set; }
        public double BalanceBch { get; set; }
        public double BalanceLtc { get; set; }

    }
}
