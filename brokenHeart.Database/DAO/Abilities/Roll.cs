using System.Text.Json.Serialization;
using brokenHeart.Database.DAO.Abilities.Abilities;
using brokenHeart.Database.Interfaces;

namespace brokenHeart.Database.DAO.Abilities
{
    public class Roll : IDao
    {
        [JsonConstructor]
        public Roll() { }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Instruction { get; set; }

        public int? AbilityId { get; set; }
        public Ability? Ability { get; set; }

        public int? AbilityTemplateId { get; set; }
        public AbilityTemplate? AbilityTemplate { get; set; }
    }
}
