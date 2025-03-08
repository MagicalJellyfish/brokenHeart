namespace brokenHeart.Services.Utility
{
    public static class ParseExtensions
    {
        public static int SafeParseInt(this string str)
        {
            if (!int.TryParse(str, out int result))
            {
                result = 0;
            }
            return result;
        }

        public static decimal SafeParseDecimal(this string str)
        {
            if (!decimal.TryParse(str.Replace('.', ','), out decimal result))
            {
                result = 0;
            }
            return result;
        }
    }
}
