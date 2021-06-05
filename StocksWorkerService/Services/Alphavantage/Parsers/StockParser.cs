using StocksWorkerService.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StocksWorkerService.Services.Alphavantage.Parsers
{
    class StockParser
    {
        private Dictionary<string, Action<string, Stock>> fieldsMap = new()
        {
            {"open", (value, stock) => stock.Open = decimal.Parse(value) },
            {"high", (value, stock) => stock.High = decimal.Parse(value) },
            {"low", (value, stock) => stock.Low = decimal.Parse(value) },
            {"close", (value, stock) => stock.Close = decimal.Parse(value) },
            {"volume", (value, stock) => stock.Volume = long.Parse(value) },
        };

        public Stock Parse(string fieldName, string value, Stock stock = null)
        {
            var fieldAdjustedName = fieldName.Split(' ').Last();
            stock ??= new Stock();

            if(fieldsMap.TryGetValue(fieldAdjustedName, out var action))
            {
                action.Invoke(value, stock);
            }
            else
            {
                // log missing field 
            }

            return stock;
        }
    }
}
