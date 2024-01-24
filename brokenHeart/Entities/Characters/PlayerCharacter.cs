using brokenHeart.DB;
using brokenHeart.Entities.Items;
using brokenHeart.Entities.Traits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Text.Json.Serialization;

namespace brokenHeart.Entities.Characters
{
    public class PlayerCharacter : Character
    {
        [JsonConstructor]
        public PlayerCharacter() { }
        public PlayerCharacter(string name, UserSimplified owner, decimal height, int weight, decimal money, int age, string description,
            List<Item>? inventory = null, List<Trait>? traits = null, string notes = "", string experience = "") 
            : base(name, description, height, weight, money, inventory, traits)
        {
            Owner = owner;
            Age = age;
            Notes = notes;
            Experience = experience;

            ViewPosition = 0;
        }

        public int Age { get; set; }
        public string Notes { get; set; }
        public string Experience { get; set; }

        public byte[]? Image { get; set; }

        public int ViewPosition { get; set; }

        public virtual UserSimplified? Owner { get; set; }

    }
}
