using AppCore.Shared.Interfaces;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace AppCore.Shared.Services
{
    public class ResponseHandler : IResponseHandler
    {
        public ResponseParam InitializeResponse(string requestId)
        {
            return new ResponseParam()
            {
                RequestId = requestId,
                Code = ResponseCodes.SUCCESS,
                Message = null,
                Data = null
            };
        }

        public ResponseParam CommitResponse(string requestId, string code, string message, object data = null)
        {
            return new ResponseParam()
            {
                RequestId = requestId,
                Code = code,
                Message = message,
                Data = data == null ? new List<object>() { } : data
            };
        }


        public ResponseParam HandleException(string requestId, string customMessage = null)
        {
            const string ERR_MESSAGE = "An error occoured. Please try again later or contact admin for resolution";
            return new ResponseParam()
            {
                RequestId = requestId,
                Code = ResponseCodes.EXCEPTION,
                Message = string.IsNullOrWhiteSpace(customMessage) ? ERR_MESSAGE : $"An exception error occoured. | {customMessage}",
                Data = new List<object>() { }
            };
        }
    }
}
