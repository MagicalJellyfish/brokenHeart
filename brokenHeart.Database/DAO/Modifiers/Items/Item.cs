using System.Text.Json.Serialization;

namespace brokenHeart.Database.DAO.Modifiers.Items
{
    public class Item : Modifier
    {
        [JsonConstructor]
        public Item() { }

        public bool Equipped { get; set; } = true;
        public int Amount { get; set; } = 1;
        public string Unit { get; set; } = "";

        public int CharacterId { get; set; }
        public Character? Character { get; set; }
    }
}
