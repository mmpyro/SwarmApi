using Microsoft.AspNetCore.Mvc;

namespace SwarmApi.Services
{
    public abstract class Service
    {
        protected IActionResult InternalServerError(string errorMessage)
        {
            var result = new ContentResult();
            result.StatusCode = 500;
            result.Content = errorMessage;
            return result;
        }

        protected IActionResult Json(object data, int? statusCode = 200)
        {
            var result = new JsonResult(data);
            result.StatusCode = statusCode;
            return result;
        }
    }
}