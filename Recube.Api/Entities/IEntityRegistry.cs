using System;

namespace Recube.Api.Entities
{
    public interface IEntityRegistry
    {
        IEntity RegisterEntity(Func<int, IEntity> func);
        IEntity? DeregisterEntity(int id);
        IEntity? GetEntityById(int id);
    }
}