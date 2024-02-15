﻿using System.Text.Json.Serialization;

namespace brokenHeart.Entities.Characters
{
    public class Bodypart
    {
        [JsonConstructor]
        public Bodypart() { }
        public Bodypart(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<BodypartCondition> BodypartConditions { get; set; } = new List<BodypartCondition>();
    }
}
