namespace brokenHeart.Models.DataTransfer.Save
{
    public class CharacterPatch
    {
        public Property TargetProperty { get; set; }
        public string Value { get; set; }

        public enum Property
        {
            Name,
            Experience,
            Notes,
            Description,
            Hp,
            TempHp,
            Age,
            Height,
            Weight,
            IsNPC,
            DefaultShortcut,
            Money,
            C,
        }
    }
}
