namespace LimitlessFit.Models.Requests;

public record PagingRequest(
    int PageNumber,
    int PageSize
);