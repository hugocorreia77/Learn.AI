namespace Learn.Core.Shared.Models.Response
{
    public class BaseResponse
    {
        public bool Success { get; set; } = true;
        public List<ResponseMessage> Errors { get; set; } = [];
        public List<ResponseMessage> Warnings { get; set; } = [];
    }
}
