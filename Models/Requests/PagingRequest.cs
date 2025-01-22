namespace LimitlessFit.Models.Requests;

[Serializable]
public class PagingRequest
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}