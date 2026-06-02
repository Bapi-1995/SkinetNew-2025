using API.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using API.Extensions;
using Microsoft.AspNetCore.Authorization;


public class AccountController(SignInManager<AppUser> signInManager) : BaseApiController
{
  [HttpPost("register")]
  public async Task<ActionResult> Register(RegisterDto register)
    {
        var user=new AppUser
        {
            FirstName=register.FirstName,
            LastName=register.LastName,
            Email=register.Email,
            UserName=register.Email
        };
        
        var result= await signInManager.UserManager.CreateAsync(user,register.Password);
        if (!result.Succeeded)
        {
            foreach(var error in result.Errors)
            {
                ModelState.AddModelError(error.Code,error.Description);
            }
            return ValidationProblem();
        }
        return Ok();
    } 
    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return NoContent();
    }
    
    [HttpGet("user-info")]
    public async Task<ActionResult> GetUserInfo()
    {
        if(User.Identity.IsAuthenticated==false) return NoContent();
        var user=await signInManager.UserManager.GetUserByEmailWithAddress(User);
        
        //FirstOrDefaultAsync(x=>x.Email==User.FindFirstValue(ClaimTypes.Email));
        if(user==null) return Unauthorized();
        return Ok(new
        {
            user.FirstName,
            user.LastName,
            user.Email,
            Address=user.address?.ToDto()
        });

    }
    [HttpGet("auth-state")]
    public ActionResult GetAuthState()
    {
        return Ok(new {IsAuthenticated=User.Identity?.IsAuthenticated??false});
    }
    [HttpPost("address")]
    public async Task<ActionResult<Address>> CreteOrUpdateAddress(AddressDTO addressDto)
    {
        var user=await signInManager.UserManager.GetUserByEmailWithAddress(User);
        if (user.address == null)
        {
            user.address=addressDto.ToEntity();
        }
        else
        {
            user.address.UpdateFromDto(addressDto);
        }
        var result=await signInManager.UserManager.UpdateAsync(user);
        if(!result.Succeeded) return BadRequest("Problem updaing user address");
        return Ok(user.address.ToDto());
        
    }
}