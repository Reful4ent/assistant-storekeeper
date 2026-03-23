using System.Collections.Generic;

namespace assistant_storekeeper_backend.DTOS.PagedResultDTOs
{
    public class PagedResultDTO<T>
    {
        public IEnumerable<T> Data { get; set; } = new List<T>();
        public int Total { get; set; }
        public int TotalPages { get; set; }
    }
}