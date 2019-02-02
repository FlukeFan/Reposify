using System;

namespace Reposify
{
    [Serializable]
    public abstract class Entity<TId> : IEntity
    {
        public virtual TId Id { get; protected set; }

        object IEntity.Id => Id;
    }
}
