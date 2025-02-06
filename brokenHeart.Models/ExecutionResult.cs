using System.Net;

namespace brokenHeart.Models
{
    public class ExecutionResult
    {
        public bool Succeeded { get; set; } = true;
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
        public string Message { get; set; }
    }

    public class ExecutionResult<T> : ExecutionResult
    {
        public T Value { get; set; }
    }
}
