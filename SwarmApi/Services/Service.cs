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

        protected IActionResult Created(string location, object value)
        {
            return new CreatedResult(location, value);
        }

        protected IActionResult NotFound()
        {
            var result = new ContentResult();
            result.StatusCode = 404;
            return result;
        }

        protected IActionResult BadRequest(string message)
        {
            var result = new ContentResult();
            result.StatusCode = 400;
            result.Content = message;
            return result;
        }

        protected IActionResult NoContent()
        {
            var result = new ContentResult();
            result.StatusCode = 204;
            return result;
        }

        protected IActionResult Ok()
        {
            var result = new ContentResult();
            result.StatusCode = 200;
            return result;
        }
    }
}