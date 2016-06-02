using mobSocial.Data.Entity.Currency;

namespace mobSocial.Services.Formatter
{
    public interface IFormatterService
    {
        string FormatCurrency(decimal amount, Currency targetCurrency, bool includeSymbol = true);
    }
}