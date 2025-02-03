namespace LimitlessFit.Models.Requests;

public record UserSearchRequest(
    string? SearchTerm,
    int PageNumber,
    int PageSize
) : PagingRequest(PageNumber, PageSize);