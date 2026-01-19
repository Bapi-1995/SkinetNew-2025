using API.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
   
    public class BuggyController : BaseApiController
    {

    [HttpGet("unauthorized")]
    public ActionResult<string> GetUnauthorized()
    {
        return Unauthorized("You are not authorized to access this");    
    }
    [HttpGet("badrequest")]
    public ActionResult<string> GetBadRequest()
    {
        return BadRequest("Not a good request");    
    }
    [HttpGet("notfound")]
    public ActionResult<string> GeNotFound()
    {
        return NotFound();    
    }
    [HttpGet("internalerror")]
    public ActionResult<string> GetInternalError()
    {
          throw new Exception("This is an internal server error");    
    }
    [HttpPost("validationerror")]
    public ActionResult<string> GetValiddationError(CreateProductDTO  productDto)
    {
          return Ok();    
    }

    }
}
