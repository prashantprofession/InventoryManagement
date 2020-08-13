using System.Text.RegularExpressions;


namespace Gasware.Common
{
    public static class ValidationProvider
    {
        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        public static bool IsValidNumber(string text)
        {
            if ((text.Trim().Length) < 1)
                return false;
            return !_regex.IsMatch(text.Trim());
        }

        public static bool IsValidTaxRate(string text)
        {
            if (text.Trim().Length < 1)
                return false;
            if ((double.Parse(text.Trim()) == 0))
                return false;
            return !_regex.IsMatch(text.Trim());
        }



        public static bool IsValidPinCode(string text)
        {
            if (text.Trim().Length < 1)
                return false;
            if (text.Trim().Length != 6)
                return false;
            return !_regex.IsMatch(text.Trim());
        }

        public static bool IsValidPhoneNumber(string text)
        {
            if (text.Trim().Length < 1)
                return false;
            if (text.Trim().Length != 10)
                return false;
            return !_regex.IsMatch(text.Trim());
        }
    }
}
