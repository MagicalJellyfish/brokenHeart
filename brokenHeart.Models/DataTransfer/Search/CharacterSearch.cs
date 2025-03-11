namespace brokenHeart.Models.DataTransfer.Search
{
    public class CharacterSearch
    {
        public int? Id { get; set; } = null;
        public string? OwnedBy { get; set; } = null;
        public string? NotOwnedBy { get; set; } = null;
        public string? Name { get; set; } = null;
        public bool? IsNpc { get; set; } = null;
    }
}
