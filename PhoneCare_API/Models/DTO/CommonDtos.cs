namespace PhoneCare_API.Models.DTO
{
    public class PagedResult<T>
    {
        public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages => TotalItems <= 0 ? 1 : (int)Math.Ceiling(TotalItems / (double)PageSize);
    }

    public class LookupItemDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
