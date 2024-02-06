﻿using brokenHeart.Entities.Characters;
using System.Text.Json.Serialization;

namespace brokenHeart.Entities
{
    public class UserSimplified
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

        public virtual ICollection<Character> Characters { get; set; } = new List<Character>();
    }
}
