using System;
using EntityApi.Entities;
using EntityApi.Entities.Models;
using EntityApi.Models;

namespace EntityApi.Services;

public interface IAuthService
{
    Task<User?> RegisterAsync(RegisterDto request);
    Task<TokenResponseDto> LoginAsync(UserDto request);
    Task<TokenResponseDto> RefreshTokensAsync(RefreshTokenRequestDto request);
}
