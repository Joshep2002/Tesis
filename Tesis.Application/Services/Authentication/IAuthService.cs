using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesis.Application.DTOs.Auth;
using Tesis.Application.DTOs.User;
using Tesis.Domain.Entities;
using Tesis.Domain.Models;

namespace Tesis.Application.Services.Authentication
{
    public interface IAuthService
    {
        Task<ActionResult<User?>> RegisterAsync(UserDTO request);
        Task<ActionResult<TokenResponseDto?>> LoginAsync(UserDTO request);
        Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request);
    }
}
