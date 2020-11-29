using Domain.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppCore.Shared.Interfaces
{
    public interface IResponseHandler
    {
        ResponseParam InitializeResponse(string requestId);
        ResponseParam CommitResponse(string requestId, string code, string message, object data = null);
        ResponseParam HandleException(string requestId, string customMessage = null);
    }
}
