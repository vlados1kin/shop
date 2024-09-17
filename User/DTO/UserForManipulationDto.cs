using System.ComponentModel.DataAnnotations;

namespace User.DTO;

public abstract record UserForManipulationDto {
    public string? Name { get; init; }
    public string? UserName { get; init; }
    [EmailAddress]
    public string? Email { get; init; }
    public string? Password { get; init; }
    public ICollection<string>? Roles { get; init; }
}