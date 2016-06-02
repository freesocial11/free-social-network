using System;

namespace mobSocial.Data.Extensions
{
    public static class TypeConversionExtensions
    {
        public static bool GetBoolean(this object value, bool throwExceptionOnError = true)
        {
            var result = false;
            if (Boolean.TryParse(value.ToString(), out result))
                return result;

            if (throwExceptionOnError)
                throw new FormatException();

            return false;
        }

        public static int GetInteger(this object value, bool throwExceptionOnError = true)
        {
            var result = 0;
            if (int.TryParse(value.ToString(), out result))
                return result;
            if (throwExceptionOnError)
                throw new FormatException();
            return 0;
        }

        public static decimal GetDecimal(this object value, bool throwExceptionOnError = true)
        {

            decimal result = 0;
            if (decimal.TryParse(value.ToString(), out result))
                return result;
            if (throwExceptionOnError)
                throw new FormatException();
            return 0;
        }

    }
}