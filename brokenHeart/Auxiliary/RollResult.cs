namespace brokenHeart.Auxiliary
{
    public class RollResult
    {
        public RollResult(int result, string detail)
        {
            Result = result;
            Detail = detail;
        }

        public int Result { get; set; }
        public string Detail { get; set; }
    }
}
