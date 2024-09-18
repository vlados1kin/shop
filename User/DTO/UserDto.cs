﻿namespace User.DTO;

public record UserDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Email { get; init; }
    public string PasswordHash { get; init; }
    public string Role { get; init; }
}
