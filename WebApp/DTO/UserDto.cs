﻿namespace WebApp.DTO
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public IList<string> Roles { get; set; }
    }
}
