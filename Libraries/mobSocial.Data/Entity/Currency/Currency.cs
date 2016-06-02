using mobSocial.Core.Data;

namespace mobSocial.Data.Entity.Currency
{
    public class Currency : BaseEntity
    {
        public string CurrencyName { get; set; }

        public string DisplayFormat { get; set; }

        public string DisplayLocale { get; set; }

        public string CurrencyCode { get; set; }
    }

    public class CurrencyMap: BaseEntityConfiguration<Currency> { }
}