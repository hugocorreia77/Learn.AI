namespace Learn.Core.Shared.Models.Response
{
    public class BaseContentResponse<T> : BaseResponse where T : class
    {
        public T? Data { get; set; }
    }
}
