namespace brokenHeart.Models.Rolling
{
    public class RollResult
    {
        public RollResult(
            int result,
            string detail,
            bool criticalSuccess = false,
            bool criticalFailure = false
        )
        {
            Result = result;
            Detail = detail;

            CriticalSuccess = criticalSuccess;
            CriticalFailure = criticalFailure;
        }

        public int Result { get; set; }
        public string Detail { get; set; }

        public bool CriticalSuccess { get; set; }
        public bool CriticalFailure { get; set; }
    }
}
