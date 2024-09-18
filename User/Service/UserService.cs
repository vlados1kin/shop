using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Entities.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Shared.Features;
using User.Contracts;
using User.DTO;

namespace User.Service;

public class UserService : IUserService
{
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly IRepositoryManager _repository;
    private readonly UserManager<Entities.Models.User> _userManager;
    private Entities.Models.User? _user;

    public UserService(
        IRepositoryManager repository,
        IConfiguration configuration,
        IMapper mapper,
        UserManager<Entities.Models.User> userManager)
    {
        _repository = repository;
        _userManager = userManager;
        _mapper = mapper;
        _configuration = configuration;
    }

    public async Task<(IEnumerable<UserDto> users, MetaData metaData)> GetUsersAsync(UserParameters userParams,
        bool trackChanges)
    {
        var usersWithMetaData = await _repository.User.GetUsersAsync(userParams, trackChanges);
        var usersDto = _mapper.Map<IEnumerable<UserDto>>(usersWithMetaData);
        return (users: usersDto, metaData: usersWithMetaData.MetaData);
    }

    public async Task<UserDto> GetUserAsync(Guid id, bool trackChanges)
    {
        var user = await GetUserAndCheckIfItExists(id, trackChanges);
        var userDto = _mapper.Map<UserDto>(user);
        return userDto;
    }

    public async Task DeleteUserAsync(Guid id, bool trackChanges)
    {
        var user = await GetUserAndCheckIfItExists(id, trackChanges);
        _repository.User.Delete(user);
        await _repository.SaveAsync();
    }

    public async Task UpdateUserAsync(Guid id, UserForUpdateDto userForUpdateDto, bool trackChanges)
    {
        var user = await GetUserAndCheckIfItExists(id, trackChanges);
        _mapper.Map(userForUpdateDto, user);
        await _repository.SaveAsync();
    }

    public async Task<IdentityResult> RegisterUserAsync(UserForRegistrationDto userForRegistrationDto)
    {
        var user = _mapper.Map<Entities.Models.User>(userForRegistrationDto);
        var result = await _userManager.CreateAsync(user, userForRegistrationDto.Password);
        if (result.Succeeded)
            await _userManager.AddToRolesAsync(user, userForRegistrationDto.Roles);
        return result;
    }

    public async Task<bool> ValidateUser(UserForLoginDto userForLoginDto)
    {
        _user = await _userManager.FindByNameAsync(userForLoginDto.UserName);
        var result = (_user != null && await _userManager.CheckPasswordAsync(_user, userForLoginDto.Password));
        return result;
    }

    public async Task<TokenDto> GenerateToken(bool populateExp)
    {
        var signingCredentials = GetSigningCredentials();
        var claims = await GetClaims();

        var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
        var refreshToken = GenerateRefreshToken();
        _user.RefreshToken = refreshToken;
        if (populateExp)
            _user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
        await _userManager.UpdateAsync(_user);
        var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        return new TokenDto { AccessToken = accessToken, RefreshToken = refreshToken };
    }

    public async Task<TokenDto> RefreshToken(TokenDto tokenDto)
    {
        var principal = GetPrincipalFromExpiredToken(tokenDto.AccessToken);
        var user = await _userManager.FindByNameAsync(principal.Identity.Name);
        if (user == null || user.RefreshToken != tokenDto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            throw new RefreshTokenBadRequest();
        _user = user;
        return await GenerateToken(populateExp: false);
    }
    
    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = _configuration["JwtSettings:validKey"];
        var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = secret,
            ValidateLifetime = true,
            ValidIssuer = jwtSettings["validIssuer"],
            ValidAudience = jwtSettings["validAudience"]
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
    
    private SigningCredentials GetSigningCredentials()
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = Encoding.UTF8.GetBytes(jwtSettings["validKey"]);
        var secret = new SymmetricSecurityKey(key);
        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }

    private async Task<List<Claim>> GetClaims()
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, _user.UserName),
            
        };
        var roles = await _userManager.GetRolesAsync(_user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        claims.Add(new(ClaimTypes.NameIdentifier, _user.Id.ToString()));
        return claims;
    }

    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var tokenOptions = new JwtSecurityToken
        (
            issuer: jwtSettings["validIssuer"],
            audience: jwtSettings["validAudience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["expires"])),
            signingCredentials: signingCredentials
        );
        return tokenOptions;
    }

    private async Task<Entities.Models.User> GetUserAndCheckIfItExists(Guid id, bool trackChanges)
    {
        var user = await _repository.User.GetUserAsync(id, trackChanges);
        if (user is null)
            throw new UserNotFoundException(id);
        return user;
    }
}