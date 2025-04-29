using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Tesis.Domain.Entities
{
    public class User
    {
        [Key] public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
