using System.Linq.Expressions;
using brokenHeart.Database.DAO;

namespace brokenHeart.Models.DataTransfer.Projection
{
    public class SimpleCharacter
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static Expression<Func<Character, SimpleCharacter>> Map = (character) =>
            new SimpleCharacter() { Id = character.Id, Name = character.Name, };
    }
}
