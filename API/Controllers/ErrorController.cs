using API.Errors;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("errors/{code}")]
    // Fix error from Video 54 : Failed to load API definition
    [ApiExplorerSettings(IgnoreApi=true)] 
    public class ErrorController: BaseApiController
    {
        public IActionResult Error(int code)
        {
             return new ObjectResult(new ApiResponse(code));
        }
    }
}