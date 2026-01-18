namespace Api
{
    public record ApiResponse<T>(
        bool Success,
        T? Data,
        string? ErrorMessage
    );
}
