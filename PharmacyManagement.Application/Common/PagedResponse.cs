namespace PharmacyManagement.Application.Common;

public class PagedResponse<T>
{
    public bool Success { get; set; } = true;
    public int StatusCode { get; set; } = 200;
    public string Message { get; set; } = "Success";
    public List<T> Data { get; set; } = new();
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
    public int TotalRecord { get; set; }

    public static PagedResponse<T> Create(List<T> data, int page, int pageSize, int totalRecord, string message = "Success")
    {
        var totalPages = pageSize > 0 ? (int)Math.Ceiling(totalRecord / (double)pageSize) : 0;
        return new PagedResponse<T>
        {
            Success = true,
            StatusCode = 200,
            Message = message,
            Data = data,
            CurrentPage = page,
            TotalPages = totalPages,
            HasNextPage = page < totalPages,
            HasPreviousPage = page > 1,
            TotalRecord = totalRecord
        };
    }
}
