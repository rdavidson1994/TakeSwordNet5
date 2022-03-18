using System;
using System.Collections.Generic;

namespace TakeSword
{
    public interface IWorld
    {
        EntityId CreateEntityId();
        void DestroyEntity(EntityId entityId);
        bool EntityIsCurrent(EntityId entityId);
        T? GetComponent<T>(EntityId entityId) where T : class;
        Tuple<M, EntityId>? GetMembership<M>(EntityId entityId);
        void InstallSystem<T0, T1, T2, T3>(Action<EntityId, T0, T1, T2, T3> effect);
        void InstallSystem<T0, T1, T2>(Action<EntityId, T0, T1, T2> effect);
        void InstallSystem<T0, T1>(Action<EntityId, T0, T1> effect);
        void InstallSystem<T0>(Action<EntityId, T0> effect);
        void RegisterCollection<TMember>()
        {
            // By default, do nothing. Some implementations may not need registration.
        }
        void RegisterComponent<T>(ComponentStorage storageType) where T : class
        {
            // By default, do nothing. Some implementations may not need registration.
        }
        void RemoveComponent<T>(EntityId entityId);
        void RemoveMembership<M>(EntityId entityId);
        Entity? RetrieveEntity(EntityId entityId);
        void Run();
        void SetMembership<M>(EntityId memberId, M memberData, EntityId destinationCollectionId) where M : class;
        void SetComponentByType(EntityId outputEntity, Type componentType, object component);
        IEnumerable<EntityId> GetMemberIds<M>(EntityId entityId);
    }

    public static class WorldExtensions
    {
        /// <summary>
        /// create a new entity, with the requested components set.
        /// </summary>
        /// <param name="components">The list of components to set on the entity.
        /// The runtime type of each argument will be used, not "object".</param>
        /// <returns>The id of the newly created entity.</returns>
        public static EntityId CreateEntityId(this IWorld world, params object[] components)
        {
            // Create a new entity 
            EntityId outputEntity = world.CreateEntityId();
            // Add the user's requested components.
            foreach (object component in components)
            {
                Type componentType = component.GetType();
                world.SetComponentByType(outputEntity, componentType, component);
            }
            return outputEntity;
        }

        /// <summary>
        /// Sets the component of type <typeparamref name="T"/> for the
        /// entity identified by <paramref name="entityId"/> to <paramref name="componentValue"/>.
        /// If the component is already present on the entity, its existing value is replaced.
        /// </summary>
        /// <typeparam name="T">The type of component to bet set.</typeparam>
        /// <param name="entityId">The entity to be modified.</param>
        /// <param name="componentValue">The new value of the component.</param>
        public static void SetComponent<T>(this IWorld world, EntityId entityId, T componentValue)
        {
            Type componentType = typeof(T);
            if (!world.EntityIsCurrent(entityId))
            {
                throw new ComponentException("Entity has already been destroyed");
            }
            if (componentValue is null)
            {
                throw new ComponentException("Attempted to set null value for component");
            }
            world.SetComponentByType(entityId, componentType, componentValue);

        }

        public static IEnumerable<Entity> GetMembers<M>(this IWorld world, EntityId entityId)
        {
            foreach (EntityId memberId in world.GetMemberIds<M>(entityId))
            {
                yield return new Entity(memberId, world);
            }
        }

        public static void RegisterComponent<T>(this IWorld world) where T : class
        {
            world.RegisterComponent<T>(ComponentStorage.List);
        }


        public static void RegisterSparseComponent<T>(this IWorld world) where T : class
        {
            world.RegisterComponent<T>(ComponentStorage.Dictionary);
        }

        public static Entity CreateEntity(this IWorld world, params object[] components)
        {
            return new Entity(world.CreateEntityId(components), world);
        }
    }
}