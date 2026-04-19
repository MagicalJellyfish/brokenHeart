namespace brokenHeart.Services.DataTransfer.Save.Entities
{
    public interface IUserSimplifiedSaveService
    {
        public string UpdateDefaultCharacterAndReturnName(ulong discordId, int charId);
        public void UpdateDefaultAbility(ulong discordId, string shortcut);
        public void UpdateDefaultTarget(ulong discordId, string targets);
    }
}
