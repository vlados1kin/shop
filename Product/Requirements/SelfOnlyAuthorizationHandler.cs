using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Product.Requirements;

public class SelfOnlyAuthorizationHandler : AuthorizationHandler<SelfOnlyAuthorizationRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SelfOnlyAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SelfOnlyAuthorizationRequirement requirement)
    {
        var userRole = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        if (userRole != null && userRole.Equals("Admin", StringComparison.InvariantCultureIgnoreCase))
            context.Succeed(requirement);
        
        var userId = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var resourceId = _httpContextAccessor.HttpContext?.Request.RouteValues["id"]?.ToString();

        if (userId != null && userId.Equals(resourceId, StringComparison.InvariantCultureIgnoreCase))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}