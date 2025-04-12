using Tradof.ResponseHandler.Models;

namespace Tradof.ResponseHandler.Consts
{
    public class CommonErrorCodes : IErrorCodes
    {
        public static readonly CommonErrorCodes Null = new("NULL", CommonErrorCode.Null);
        public static readonly CommonErrorCodes Forbidden = new("FORBIDDEN", CommonErrorCode.Forbidden);
        public static readonly CommonErrorCodes Unauthorized = new("UNAUTHORIZED", CommonErrorCode.Unauthorized);
        public static readonly CommonErrorCodes OperationFailed = new("OPERATION_FAILED", CommonErrorCode.OperationFailed);
        public static readonly CommonErrorCodes InvalidInput = new("INVALID_INPUT", CommonErrorCode.InvalidInput);
        public static readonly CommonErrorCodes FailedToSaveData = new("FAILED_TO_SAVE_DATA", CommonErrorCode.FailedToSaveData);
        public static readonly CommonErrorCodes ServerError = new("SERVER_ERROR", CommonErrorCode.ServerError);
        public static readonly CommonErrorCodes FailedToSendCode = new("FAILED_TO_SEND_CODE", CommonErrorCode.FailedToSendCode);
        public static readonly CommonErrorCodes InvalidEmailOrPassword = new("INVALID_EMAIL_OR_PASSWORD", CommonErrorCode.InvalidEmailOrPassword);
        public static readonly CommonErrorCodes EmailNotConfirmed = new("EMAIL_NOT_CONFIRMED", CommonErrorCode.EmailNotConfirmed);
        public static readonly CommonErrorCodes InvalidRefreshToken = new("INVALID_REFRESH_TOKEN", CommonErrorCode.InvalidRefreshToken);
        public static readonly CommonErrorCodes ResourceNotFound = new("RESOURCE_NOT_FOUND", CommonErrorCode.ResourceNotFound);
        public static readonly CommonErrorCodes NotAuthorized = new("NOT_AUTHORIZED", CommonErrorCode.NotAuthorized);
        public static readonly CommonErrorCodes NotFound = new("NOT_FOUND", CommonErrorCode.NotFound);
        public static readonly CommonErrorCodes FailedToRetrieveData = new("Failed_To_Retrieve_Data", CommonErrorCode.FailedToRetrieveData);
        public static readonly CommonErrorCodes FailedToUpdateData = new("Failed_To_Update_Data", CommonErrorCode.FailedToUpdateData);

        private CommonErrorCodes(string value, CommonErrorCode code)
        {
            Value = value;
            Code = (int)code;
        }

        public string Value { get; init; }
        public int Code { get; init; }
    }

    public enum CommonErrorCode
    {
        Null = 0,
        Forbidden = 1,
        Unauthorized = 2,
        OperationFailed = 3,
        InvalidInput = 4,
        FailedToSaveData = 5,
        ServerError = 6,
        FailedToSendCode = 7,
        InvalidEmailOrPassword = 8,
        EmailNotConfirmed = 9,
        InvalidRefreshToken = 10,
        ResourceNotFound = 11,
        NotAuthorized = 12,
        NotFound = 13,
        FailedToRetrieveData = 14,
        FailedToUpdateData = 15
    }
}