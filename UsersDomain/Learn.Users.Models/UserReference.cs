﻿namespace Learn.Users.Models
{
    public class UserReference
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Name { get; set; }
        public required string Username { get; set; }
    }
}
