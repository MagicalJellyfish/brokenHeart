namespace brokenHeart.Database.DAO.Characters
{
    public class CharacterImage
    {
        public CharacterImage() { }

        public CharacterImage(int id, byte[]? bytes)
        {
            Id = id;
            Bytes = bytes;
        }

        public int Id { get; set; }
        public byte[]? Bytes { get; set; } = new byte[0];
    }
}
