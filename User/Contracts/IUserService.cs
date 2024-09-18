using Microsoft.AspNetCore.Identity;
using Shared.Features;
using User.DTO;

namespace User.Contracts;

public interface IUserService
{
    Task<(IEnumerable<UserDto> users, MetaData metaData)> GetUsersAsync(UserParameters userParams, bool trackChanges);
    Task<UserDto> GetUserAsync(Guid id, bool trackChanges);
    Task DeleteUserAsync(Guid id, bool trackChanges);
    Task UpdateUserAsync(Guid id, UserForUpdateDto userForUpdateDto, bool trackChanges);
    
    Task<IdentityResult> RegisterUserAsync(UserForRegistrationDto userForRegistrationDto);
    Task<bool> ValidateUser(UserForLoginDto userForLoginDto);
    Task<TokenDto> GenerateToken(bool populateExp);
    Task<TokenDto> RefreshToken(TokenDto tokenDto);
}