namespace FutureCoreBackend.DTO
{
    public class ResultDTO
    {
        public int Statescode { get; set; }
        public dynamic Data { get; set; }
        public string Message { get; set; }
        public int Count { get; set; } = 0;
    }
}
