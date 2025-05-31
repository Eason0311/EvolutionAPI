﻿using prjtestAPI.Services.Interfaces;

namespace prjtestAPI.Services
{
    public class PasswordHasherService : IPasswordHasher
    {
        public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);

        public bool Verify(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
