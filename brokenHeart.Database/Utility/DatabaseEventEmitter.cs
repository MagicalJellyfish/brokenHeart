namespace brokenHeart.Database.Utility
{
    public class DatabaseEventEmitter
    {
        internal void EmitCharacterChange(int charId)
        {
            CharacterChanged?.Invoke(this, charId);
        }

        public event EventHandler<int> CharacterChanged;
    }
}
