using brokenHeart.DB;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace brokenHeart.Services.Endpoints
{
    public interface IEndpointEntityService
    {
        public dynamic GetEntityPrepare(dynamic requestEntity);

        public void PatchEntity(
            BrokenDbContext context,
            Type entityType,
            dynamic requestEntity,
            List<Operation> operations
        );

        public dynamic PostEntity(BrokenDbContext context, Type entityType, dynamic requestEntity);
    }
}
