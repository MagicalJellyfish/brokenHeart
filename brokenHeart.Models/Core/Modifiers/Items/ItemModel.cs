namespace brokenHeart.Models.Core.Modifiers.Items
{
    public class ItemModel : ModiferModel
    {
        public bool Equipped { get; set; }
        public int Amount { get; set; }
        public string Unit { get; set; }
    }
}
