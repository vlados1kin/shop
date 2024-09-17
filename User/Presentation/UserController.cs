using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Shared.Features;
using Shared.Filters;
using User.Contracts;
using User.DTO;

namespace User.Presentation;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IServiceManager _service;
    public UserController(IServiceManager service) => _service = service;

    [HttpGet]
    [HttpHead]
    public async Task<IActionResult> GetUsers([FromQuery] UserParameters userParameters)
    {
        var pagedResult = await _service.UserService.GetUsersAsync(userParameters, trackChanges: false);
        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));
        return Ok(pagedResult.users);
    }

    [HttpGet("{id:guid}", Name = "GetUserById")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var user = await _service.UserService.GetUserAsync(id, false);
        return Ok(user);
    }

    [HttpPost]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> RegisterUser([FromBody] UserForRegistrationDto userForRegistrationDto)
    {
        var result = await _service.UserService.RegisterUserAsync(userForRegistrationDto);
        if (result.Succeeded) 
            return StatusCode(201);
        foreach (var error in result.Errors)
            ModelState.TryAddModelError(error.Code, error.Description);
        return BadRequest(ModelState);
    }

    [HttpPost("login")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> Authenticate([FromBody] UserForLoginDto userForLoginDto)
    {
        if (!await _service.UserService.ValidateUser(userForLoginDto))
            return Unauthorized();
        return Ok(new
        {
            Token = await _service.UserService.GenerateToken(populateExp: true)
        });
    }
    
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody]TokenDto tokenDto)
    {
        var tokenDtoToReturn = await _service.UserService.RefreshToken(tokenDto);
        return Ok(tokenDtoToReturn);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser([FromRoute] Guid id)
    {
        await _service.UserService.DeleteUserAsync(id, trackChanges: true);
        return NoContent();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateUser([FromRoute] Guid id, [FromBody] UserForUpdateDto userForUpdateDto)
    {
        await _service.UserService.UpdateUserAsync(id, userForUpdateDto, trackChanges: true);
        return NoContent();
    }
}