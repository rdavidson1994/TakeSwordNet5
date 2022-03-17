using System;
using System.Collections.Generic;

namespace TakeSword
{
    public interface IWorld
    {
        EntityId CreateEntityId();
        //EntityId CreateEntityId(params object[] components);
        void DestroyEntity(EntityId entityId);
        bool EntityIsCurrent(EntityId entityId);
        T? GetComponent<T>(EntityId entityId) where T : class;
        IEnumerable<Entity> GetMembers<M>(EntityId entityId);
        Tuple<M, EntityId>? GetMembership<M>(EntityId entityId);
        void InstallSystem<T0, T1, T2, T3>(Action<EntityId, T0, T1, T2, T3> effect);
        void InstallSystem<T0, T1, T2>(Action<EntityId, T0, T1, T2> effect);
        void InstallSystem<T0, T1>(Action<EntityId, T0, T1> effect);
        void InstallSystem<T0>(Action<EntityId, T0> effect);
        void RegisterCollection<TMember>();
        void RegisterComponent<T>() where T : class;
        void RegisterComponent<T>(ComponentStorage storageType) where T : class;
        void RegisterSparseComponent<T>() where T : class;
        void RemoveComponent<T>(EntityId entityId);
        void RemoveMembership<M>(EntityId entityId);
        Entity? RetrieveEntity(EntityId entityId);
        void Run();
        void SetComponent<T>(EntityId entityId, T componentValue);
        void SetMembership<M>(EntityId memberId, M memberData, EntityId destinationCollectionId) where M : class;
        void SetComponentByType(EntityId outputEntity, Type componentType, object component);
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
    }
}