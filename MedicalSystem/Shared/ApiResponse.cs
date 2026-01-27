namespace Shared
{
    public record ApiResponse<T>(
        bool Success,
        T? Data,
        string? ErrorMessage)
    {
        public static ApiResponse<T> Ok(T data)
        => new(true, data, null);

        public static ApiResponse<T> Fail(string error)
            => new(false, default, error);
    }
}
