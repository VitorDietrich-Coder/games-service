using Abp.Domain.Values;
using Games.Microservice.Domain.Core.Exceptions;
using System.Collections.Generic;

namespace Games.Microservice.Domain.ValueObjects
{
    /// <summary>
    /// Represents a monetary value with a specific currency.
    /// </summary>
    public class CurrencyAmount : ValueObject
    {
        public decimal Value { get; }
        public string Currency { get; }

        private static readonly HashSet<string> ValidCurrencies = new HashSet<string>
        {
            "USD", // US Dollar
            "EUR", // Euro
            "BRL", // Brazilian Real
            "JPY", // Japanese Yen
            "GBP", // British Pound
            "AUD", // Australian Dollar
            "CAD", // Canadian Dollar
            "CHF", // Swiss Franc
            "CNY", // Chinese Yuan
            "SEK", // Swedish Krona
            "NZD"  // New Zealand Dollar
        };

        public CurrencyAmount(decimal value, string currency = "BRL")
        {
            if (value < 0)
                throw new BusinessRulesException("The monetary amount must be greater than or equal to zero.");

            if (string.IsNullOrWhiteSpace(currency))
                throw new BusinessRulesException("Currency code cannot be null or empty.");

            var normalizedCurrency = currency.ToUpperInvariant();

            if (!IsValidCurrency(normalizedCurrency))
                throw new BusinessRulesException($"Invalid currency: '{normalizedCurrency}'. Supported currencies are: {string.Join(", ", ValidCurrencies)}.");

            Value = value;
            Currency = normalizedCurrency;
        }

        public static bool IsValidCurrency(string currency)
        {
            return ValidCurrencies.Contains(currency);
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
            yield return Currency;
        }
    }
}
