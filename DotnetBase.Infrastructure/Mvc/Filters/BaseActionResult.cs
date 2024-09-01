using Microsoft.AspNetCore.Mvc;
using DotnetBase.Infrastructure.Common.Models;
using System.Threading.Tasks;

namespace DotnetBase.Infrastructure.Mvc.Filters
{
    public class BaseActionResult : IActionResult
    {
        private readonly ResponseModel _responseModel;

        public BaseActionResult(ResponseModel responseModel)
        {
            _responseModel = responseModel;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            ObjectResult objectResult;
            switch (_responseModel.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    objectResult = new ObjectResult(_responseModel.Data != null ? _responseModel.Data : _responseModel.Message)
                    {
                        StatusCode = (int)_responseModel.StatusCode
                    };
                    break;
                case System.Net.HttpStatusCode.NotFound:
                    objectResult = new ObjectResult(_responseModel.Message)
                    {
                        StatusCode = (int)_responseModel.StatusCode
                    };
                    break;
                default:
                    objectResult = new ObjectResult(_responseModel.Message)
                    {
                        StatusCode = (int)_responseModel.StatusCode
                    };
                    break;
            }
            await objectResult.ExecuteResultAsync(context);
        }
    }
}
