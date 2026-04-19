namespace brokenHeart.Models.Rolling
{
    public class RollableCharacter
    {
        public RollableCharacter() { }

        public int Id { get; set; }
        public int Hp { get; set; }
        public int Armor { get; set; }
        public int Evasion { get; set; }
        public int MovementSpeed { get; set; }

        public List<KeyValuePair<string, int>> Stats { get; set; } = new();
        public List<KeyValuePair<string, int>> Counters { get; set; } = new();
        public List<KeyValuePair<string, int>> Variables { get; set; } = new();
    }
}
