using Learn.Core.Shared.Models.Response;

namespace Learn.Core.Shared.Extensions
{
    public static class BaseResponseExtensions
    {
        #region public static BaseResponse SetSucceeded(this BaseResponse response)
        public static BaseResponse SetSucceeded(this BaseResponse response)
        {
            response.Success = true;
            return response;
        }
        #endregion

        #region public static BaseResponse SetFailed(this BaseResponse response)
        public static BaseResponse SetFailed(this BaseResponse response)
        {
            response.Success = false;
            return response;
        }
        #endregion

        #region public static BaseResponse AddError(this BaseResponse response, string errorMessage)
        public static BaseResponse AddError(this BaseResponse response, string errorMessage)
        {
            response.Errors ??= [];
            response.Errors.Add(new ResponseMessage
            {
                Message = errorMessage
            });
            return response;
        }
        #endregion

        #region public static BaseResponse AddWarning(this BaseResponse response, string errorMessage)
        public static BaseResponse AddWarning(this BaseResponse response, string errorMessage)
        {
            response.Warnings ??= [];
            response.Warnings.Add(new ResponseMessage
            {
                Message = errorMessage
            });
            return response;
        }
        #endregion

        #region public static BaseContentResponse<T> SetSucceeded<T>(this BaseContentResponse<T> response)
        public static BaseContentResponse<T> SetSucceeded<T>(this BaseContentResponse<T> response)
            where T : class
        {
            response.Success = true;
            return response;
        }
        #endregion

        #region public static BaseContentResponse<T> SetFailed<T>(this BaseContentResponse<T> response)
        public static BaseContentResponse<T> SetFailed<T>(this BaseContentResponse<T> response)
            where T : class
        {
            response.Success = false;
            return response;
        }
        #endregion

        #region public static BaseContentResponse<T> AddError<T>(this BaseContentResponse<T> response, string errorMessage)
        public static BaseContentResponse<T> AddError<T>(this BaseContentResponse<T> response, string errorMessage)
            where T : class
        {
            response.Errors ??= [];
            response.Errors.Add(new ResponseMessage
            {
                Message = errorMessage
            });
            return response;
        }
        #endregion

        #region public static BaseContentResponse<T> AddWarning<T>(this BaseContentResponse<T> response, string errorMessage)
        public static BaseContentResponse<T> AddWarning<T>(this BaseContentResponse<T> response, string errorMessage)
            where T : class
        {
            response.Warnings ??= [];
            response.Warnings.Add(new ResponseMessage
            {
                Message = errorMessage
            });
            return response;
        }
        #endregion
    }
}
