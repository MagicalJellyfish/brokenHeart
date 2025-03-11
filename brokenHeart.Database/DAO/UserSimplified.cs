﻿using System.Text.Json.Serialization;
using brokenHeart.Database.Interfaces;

namespace brokenHeart.Database.DAO
{
    public class UserSimplified : IDao
    {
        [JsonConstructor]
        public UserSimplified() { }

        public UserSimplified(string username, ulong discordId)
        {
            Username = username;
            DiscordId = discordId;
        }

        public int Id { get; set; }
        public string Username { get; set; }
        public ulong DiscordId { get; set; }

        public string? DefaultAbilityString { get; set; }
        public string? DefaultTargetString { get; set; }

        public virtual Character? ActiveCharacter { get; set; }
        public virtual ICollection<Character> Characters { get; set; } = new List<Character>();
    }
}
