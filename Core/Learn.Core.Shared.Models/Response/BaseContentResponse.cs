namespace Learn.Core.Shared.Models.Response
{
    public class BaseContentResponse<T> : BaseResponse 
    {
        public T? Data { get; set; }
    }
}
