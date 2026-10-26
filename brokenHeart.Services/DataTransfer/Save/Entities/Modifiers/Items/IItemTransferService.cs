namespace brokenHeart.Services.DataTransfer.Save.Modifiers.Items
{
    public interface IItemTransferService
    {
        public int TransferItem(int id, int characterId, int amount);
    }
}
