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
    }

    public static class WorldExtensions
    {

    }
}