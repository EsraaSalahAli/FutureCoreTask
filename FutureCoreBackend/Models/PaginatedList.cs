namespace FutureCoreBackend.Models
{
    public class PaginatedList<T>
    {
        public List<T> Items { get; set; }
        public int PageIndex { get; set; }
        public int PageCount { get; set; }
        public int Count { get; set; }
    }
}
