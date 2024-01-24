using System.Text.Json.Serialization;

namespace brokenHeart.Entities.Combat
{
    public class CombatCharacterData
    {
        [JsonConstructor]
        public CombatCharacterData() { }
        public CombatCharacterData(Character character, int initRoll, string shortcut)
        {
            Character = character;
            InitRoll = initRoll;
            Shortcut = shortcut;
        }

        public int Id { get; set; }
        public int InitRoll { get; set; }
        public string Shortcut { get; set; }

        public int CharacterId { get; set; }
        public virtual Character Character { get; set; }
    }
}
