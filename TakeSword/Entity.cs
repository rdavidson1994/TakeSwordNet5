using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace TakeSword
{
    public class Entity
    {
        public EntityId Id { get; }
        private World world;


        internal Entity(EntityId entityId, World world)
        {
            this.Id = entityId;
            this.world = world;
        }

        public T? Get<T>() where T : class
        {
            return world.GetComponent<T>(Id);
        }

        public bool Has<T>([NotNullWhen(true)] out T? component)
            where T : class
        {
            component = world.GetComponent<T>(Id);
            return component is not null;
        }

        public bool Has<T>()
            where T : class
        {
            return Has(out T _);
        }

        public void Set<T>(T newValue) where T : class
        {
            world.SetComponent(Id, newValue);
        }

        public void Remove<T>()
            where T : class
        {
            world.RemoveComponent<T>(Id);
        }

        public void Destroy()
        {
            world.DestroyEntity(Id);
        }


        public bool IsMember<T>([NotNullWhen(true)] out Entity? group, [NotNullWhen(true)] out T? membership)
            where T : class
        {
            group = null;
            membership = null;
            var optionalTuple = world.GetMembership<T>(Id);
            if (optionalTuple is null)
            {
                return false;
            }
            group = world.RetrieveEntity(optionalTuple.Item2);
            if (group is null)
            {
                return false;
            }
            membership = optionalTuple.Item1;
            return true;
        }

        public void Enter<T>(Entity group, T membership)
            where T : class
        {
            world.SetMembership<T>(Id, membership, group.Id);
        }

        public void Exit<T>()
        {
            world.RemoveMembership<T>(Id);
        }

        public IEnumerable<Entity> GetMembers<T>()
        {
            return world.GetMembers<T>(Id);
        }
    }
}