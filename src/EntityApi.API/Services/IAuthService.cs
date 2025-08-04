using System;
using EntityApi.Core.Entities;
using EntityApi.API.Models;

namespace EntityApi.API.Services;

public interface IAuthService
{
    Task<User?> RegisterAsync(RegisterDto request);
    Task<TokenResponseDto> LoginAsync(UserDto request);
    Task<TokenResponseDto> RefreshTokensAsync(RefreshTokenRequestDto request);
}
