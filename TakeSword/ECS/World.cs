using System;
using System.Collections.Generic;
using System.Linq;

namespace TakeSword
{
    public class World
    {
        // Tracks the relationship between types and the component ID/index assigned to that type.
        private Dictionary<Type, int> componentIdsByType = new();
        // Tracks the reason each Type was registered, for more detailed error message.
        private Dictionary<Type, string> registrationReasonByType = new();
        // Tracks which types have been registered as collection membership components
        private HashSet<Type> membershipComponentTypes = new();
        // Stores all data for all components.
        // The index for this list is the component ID.
        private List<IComponentStorage> componentData = new();
        // Stores factory instances used to construct wrapper types of each component. 
        // The index for this list is the component ID.
        private List<IWrapperFactory> writeWrapperFactories = new();
        // The largest number of entities the world can contain before needing a resize.
        private int maxEntityCount = 0;
        // Tracks which entity indexes are available to be reclaimed.
        private DeadIndexList deadIndexes = new();
        // Tracks the generation number for each entity. The index for this list is the entity index.
        private List<int> generationByEntityIndex = new();
        // Stores each game system, in order of execution.
        private List<GameSystem> Systems = new();

        private void ReserveType(Type type, string reason)
        {
            if (registrationReasonByType.TryGetValue(type, out string? conflictingReason))
            {
                throw new ComponentException($"Tried to register {type} as a {reason}, " +
                    $"but was already registred as a {conflictingReason}.");
            }
            else
            {
                registrationReasonByType[type] = reason;
            }
        }

        private ParameterKey ParameterKeyByType(Type type)
        {
            bool isOptional = typeof(IOptional).IsAssignableFrom(type);
            bool isWritable = typeof(IWritable).IsAssignableFrom(type);
            Type targetType;
            if (isWritable || isOptional)
            {
                // Both writable and optional parameters wrap the true component type as the first generic argument
                targetType = type.GetGenericArguments()[0];
            }
            else
            {
                // Anything else is a raw (non-writable, non-optional) parameter, which doesn't have a wrapper.
                targetType = type;
            }
            int componentId = componentIdsByType[targetType];
            return new ParameterKey(componentId, isWritable, isOptional);
        }

        public void InstallSystem<T0, T1>(Action<EntityId, T0, T1> effect)
        {
            // Convert the provided function into a "permissive" one that takes an object[].
            // We cast each argument to the requested type at runtime.
            Action<EntityId, object[]> permissiveAction = (id, args) =>
            {
                effect(
                    id
                    , (T0)args[0]
                    , (T1)args[1]
                );
            };
            // Record the types of the requested arguments as ParameterKeys.
            ParameterKey[] componentIds = new ParameterKey[]
            {
            ParameterKeyByType(typeof(T0))
            ,ParameterKeyByType(typeof(T1))
            };
            // Store the permissive version of the action, along with its parameter type information
            GameSystem system = new GameSystem(permissiveAction, componentIds);
            Systems.Add(system);
        }

        #region Additional IstallSystem implementations
        public void InstallSystem<T0, T1, T2>(Action<EntityId, T0, T1, T2> effect)
        {
            Action<EntityId, object[]> permissiveAction = (id, args) =>
            {
                effect(
                    id
                    , (T0)args[0]
                    , (T1)args[1]
                    , (T2)args[2]
                );
            };
            ParameterKey[] parameterKeys = new ParameterKey[]
            {
            ParameterKeyByType(typeof(T0))
            ,ParameterKeyByType(typeof(T1))
            ,ParameterKeyByType(typeof(T2))
            };
            GameSystem system = new GameSystem(permissiveAction, parameterKeys);
            Systems.Add(system);
        }

        public void InstallSystem<T0, T1, T2, T3>(Action<EntityId, T0, T1, T2, T3> effect)
        {
            Action<EntityId, object[]> permissiveAction = (id, args) =>
            {
                effect(
                    id
                    , (T0)args[0]
                    , (T1)args[1]
                    , (T2)args[2]
                    , (T3)args[3]
                );
            };
            ParameterKey[] parameterKeys = new ParameterKey[]
            {
            ParameterKeyByType(typeof(T0))
            ,ParameterKeyByType(typeof(T1))
            ,ParameterKeyByType(typeof(T2))
            ,ParameterKeyByType(typeof(T3))
            };
            GameSystem system = new GameSystem(permissiveAction, parameterKeys);
            Systems.Add(system);
        }

        public void InstallSystem<T0>(Action<EntityId, T0> effect)
        {
            Action<EntityId, object[]> permissiveAction = (id, args) =>
            {
                effect(
                    id
                    , (T0)args[0]
                );
            };
            ParameterKey[] componentIds = new ParameterKey[]
            {
            ParameterKeyByType(typeof(T0))
            };
            GameSystem system = new(permissiveAction, componentIds);
            Systems.Add(system);
        }
        #endregion

        /// <summary>
        /// Runs the game loop, firing each system once in order of registration.
        /// </summary>
        public void Run()
        {
            foreach (GameSystem system in Systems)
            {
                RunSystem(system);
            }
        }

        private object? GetWrappedComponent(ParameterKey parameterKey, int entityIndex)
        {
            // First, retrieve the raw (possibly null) value found for the desired component
            // on the desired entity. (Because this is internal, we assume the entityIndex is
            // in-range and hasn't been marked dead.)
            object? foundComponent = componentData[parameterKey.componentId][entityIndex];
            // If the parameter is not optional and not writable, it doesn't need wrapping...
            if (!parameterKey.optional && !parameterKey.needsWritable)
            {
                // Just return it as is.
                return foundComponent;
            }
            // Otherwise retrieve a factory to create a wrapper for it.
            IWrapperFactory factory = writeWrapperFactories[parameterKey.componentId];

            // If the parameter is optional. we don't care about null values
            if (parameterKey.optional)
            {
                // If writing is needed, create a writable optional wrapper
                if (parameterKey.needsWritable)
                {
                    return factory.CreateWritableOptional(foundComponent);
                }
                // Otherwise create a normal optional wrapper
                else
                {
                    return factory.CreateOptional(foundComponent);
                }
            }

            // If the component is required and needs writing,
            // create a non-optional Writable wrapper - but only
            // if the wrapped value is non null.
            if (parameterKey.needsWritable && foundComponent != null)
            {
                return factory.CreateWritable(foundComponent);
            }
            // In any other case, return the retrieved value - 
            // which will propogate null if the component wasn't optional.
            else
            {
                return foundComponent;
            }
        }

        private void RunSystem(GameSystem system)
        {
            for (int entityIndex = 0; entityIndex < maxEntityCount; entityIndex++)
            {
                if (deadIndexes.Contains(entityIndex))
                {
                    continue;
                }

                bool missingComponents = false;
                object[] arguments = new object[system.ParameterKeys.Length];
                int argumentIndex = 0;
                foreach (ParameterKey parameterKey in system.ParameterKeys)
                {
                    // For each argument the system needs
                    // Attempt to retrieve the entity's corresponding component from storage.
                    object? foundComponent = GetWrappedComponent(parameterKey, entityIndex);
                    if (foundComponent == null)
                    {
                        // If GetWrappedComponent returned null, the component was
                        // missing from the entity and the parameter was not optional,
                        // so we shouldn't execute the system on this entity.
                        missingComponents = true;
                        break;
                    }
                    else
                    {
                        // Otherwise, record the component in our arguments list.
                        arguments[argumentIndex] = foundComponent;
                        argumentIndex += 1;
                    }
                }

                // If any required components were absent, skip to the next entity.
                if (missingComponents)
                {
                    continue;
                }

                // Otherwise, execute the system's effect on the arguments retrieved for this entity.
                system.Effect(
                    new EntityId(entityIndex, generationByEntityIndex[entityIndex]),
                    arguments
                );

                // Now that the system code has been run, update the entity's components to 
                // reflect changes in each of the system's writable arguments.
                argumentIndex = 0;
                foreach (ParameterKey parameterKey in system.ParameterKeys)
                {
                    // Only consider parameters marked as writable
                    if (arguments[argumentIndex] is IWritable writable)
                    {
                        if (writable.WasDestroyed)
                        {
                            // If the component was marked for destruction, set it to null
                            componentData[parameterKey.componentId][entityIndex]
                                = null;
                        }
                        else
                        {
                            // Otherwise apply the updated value.
                            componentData[parameterKey.componentId][entityIndex]
                                = writable.WrittenValue;
                        }
                    }
                    argumentIndex += 1;
                }
            }
        }

        /// <summary>
        /// Retrieves the value of the component of type <typeparamref name="T"/>
        /// for the given entity. Returns null if the component is not set.
        /// </summary>
        /// <typeparam name="T">The type of component to retrieve.</typeparam>
        /// <param name="entityId">The id of the entity to inspect.</param>
        /// <returns></returns>
        public T? GetComponent<T>(EntityId entityId) where T : class
        {
            return (T?)GetComponent(typeof(T), entityId);
        }

        private object? GetComponent(Type componentType, EntityId entityId)
        {
            int componentId = GetComponentId(componentType);
            if (!EntityIsCurrent(entityId))
            {
                return null;
            }
            // Need checks here - index can fail if they plug in
            // an EntityId from another World
            return componentData[componentId][entityId.index];
        }

        public void RegisterComponent<T>(ComponentStorage storageType) where T : class
        {
            switch (storageType)
            {
                case ComponentStorage.List:
                    RegisterComponent<T>();
                    break;
                case ComponentStorage.Dictionary:
                    RegisterSparseComponent<T>();
                    break;
            }
        }

        public void RegisterComponent<T>() where T : class
        {
            // This list will be full of nulls at first
            ListComponentStorage entries = new ListComponentStorage(maxEntityCount);
            RegisterComponent<T>(entries);
        }

        public void RegisterSparseComponent<T>() where T : class
        {
            DictionaryComponentStorage entries = new DictionaryComponentStorage();
            RegisterComponent<T>(entries);
        }

        internal void RegisterComponent<T>(IComponentStorage entries)
            where T : class
        {
            ReserveType(typeof(T), "component");
            componentData.Add(entries);
            writeWrapperFactories.Add(new WrapperFactory<T>());
            componentIdsByType[typeof(T)] = componentData.Count - 1;
        }

        /// <summary>
        /// Sets the component of type <typeparamref name="T"/> for the
        /// entity identified by <paramref name="entityId"/> to <paramref name="componentValue"/>.
        /// If the component is already present on the entity, its existing value is replaced.
        /// </summary>
        /// <typeparam name="T">The type of component to bet set.</typeparam>
        /// <param name="entityId">The entity to be modified.</param>
        /// <param name="componentValue">The new value of the component.</param>
        public void SetComponent<T>(EntityId entityId, T componentValue)
        {
            int componentId = GetComponentId<T>();
            if (!EntityIsCurrent(entityId))
            {
                throw new ComponentException("Entity has already been destroyed");
            }
            componentData[componentId][entityId.index] = componentValue;
        }

        /// <summary>
        /// Retrieve id (index) of the registered component for this Type.
        /// Throws an exception if no component for the given type is registered.
        /// </summary>
        /// <typeparam name="T">The type whose id will be retrieved</typeparam>
        /// <returns></returns>
        private int GetComponentId<T>()
        {
            return GetComponentId(typeof(T));
        }

        private int GetComponentId(Type type)
        {
            if (!componentIdsByType.TryGetValue(type, out int componentId))
            {
                string errorMessage = $"Type {type} has not been registered as a component.";
                if (registrationReasonByType.TryGetValue(type, out string? reason))
                {
                    errorMessage += $" (It has been registered as a {reason} though.)";
                }
                throw new ComponentException($"No component registered for {type}");
            }
            return componentId;
        }

        /// <summary>
        /// Remove the component of the given type from the given entity, if it exists.
        /// </summary>
        /// <typeparam name="T">The type of component to remove</typeparam>
        /// <param name="entityId">The entity to remove the component from</param>
        public void RemoveComponent<T>(EntityId entityId)
        {
            int componentId = GetComponentId<T>();
            if (!EntityIsCurrent(entityId))
            {
                throw new ComponentException("Entity has already been destroyed");
            }
            componentData[componentId].Remove(entityId.index);
        }


        public Entity CreateEntity(params object[] components)
        {
            return new Entity(CreateEntityId(components), this);
        }

        /// <summary>
        /// create a new entity, with the requested components set.
        /// </summary>
        /// <param name="components">The list of components to set on the entity.
        /// The runtime type of each argument will be used, not "object".</param>
        /// <returns>The id of the newly created entity.</returns>
        public EntityId CreateEntityId(params object[] components)
        {
            EntityId output;
            // Try to reclaim a vacant index
            if (deadIndexes.ReclaimIndex(out int indexToReclaim))
            {
                foreach (IComponentStorage componentStorage in componentData)
                {
                    // Clear lingering component data at the vacant index
                    componentStorage.Remove(indexToReclaim);
                }
                output = new EntityId(indexToReclaim, generationByEntityIndex[indexToReclaim]);
            }

            // If no vacant index is available, expand all storage by 1 and return the new final index.
            else
            {
                foreach (IComponentStorage componentStorage in componentData)
                {
                    componentStorage.Expand();
                }
                generationByEntityIndex.Add(0);
                deadIndexes.AddLivingMember();
                maxEntityCount += 1;
                output = new EntityId(maxEntityCount - 1, 0);
            }

            // Add the user's requested components.
            foreach (object component in components)
            {
                Type realType = component.GetType();
                int componentId = GetComponentId(realType);
                componentData[componentId][output.index] = component;
            }
            return output;
        }

        /// <summary>
        /// Determines whether the entity identified by <paramref name="entityId"/>
        /// still exists in the world.
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public bool EntityIsCurrent(EntityId entityId)
        {
            return generationByEntityIndex[entityId.index] == entityId.generation;
        }


        /// <summary>
        /// Verify that the entity referenced by the given <paramref name="entityId"/>
        /// still exists, and if so, return it as an <see cref="Entity"/> instance.
        /// Otherwise, returns null.
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public Entity? RetrieveEntity(EntityId entityId)
        {
            if (EntityIsCurrent(entityId))
            {
                return new Entity(entityId, this);
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Destroy an entity, freeing space for the creation of new ones.
        /// NOTE: Any future usage of this entity's id, except for 
        /// <see cref="GetComponent{T}(EntityId)"/>, will result in an exception.
        /// </summary>
        /// <param name="entityId">The id of the entity to destroy</param>
        public void DestroyEntity(EntityId entityId)
        {
            CheckEntityIsCurrent(entityId);
            generationByEntityIndex[entityId.index] += 1;
            deadIndexes.MarkDead(entityId.index);
        }

        private void CheckEntityIsCurrent(EntityId entityId)
        {
            if (!EntityIsCurrent(entityId))
            {
                throw new ComponentException("Entity has already been destroyed");
            }
        }

        public void RegisterCollection<TMember>()
        {
            ReserveType(typeof(TMember), "membership component");
            membershipComponentTypes.Add(typeof(TMember));
            RegisterComponent<MembershipComponent<TMember>>();
            RegisterComponent<CollectionComponent<TMember>>();
        }

        public IEnumerable<Entity> GetMembers<M>(EntityId entityId)
        {
            CollectionComponent<M>? found = GetComponent<CollectionComponent<M>>(entityId);
            if (found == null)
            {
                return Enumerable.Empty<Entity>();
            }
            else
            {
                return found.EnumerateMembers(this);
            }
        }

        public Tuple<M, EntityId>? GetMembership<M>(EntityId entityId)
        {
            MembershipComponent<M>? membershipComponent = GetComponent<MembershipComponent<M>>(entityId);
            if (membershipComponent is null)
            {
                return null;
            }

            if (!EntityIsCurrent(membershipComponent.Collection))
            {
                return null;
            }
            return Tuple.Create(membershipComponent.MembershipData, membershipComponent.Collection);
        }

        public void RemoveMembership<M>(EntityId entityId)
        {
            // Retrieve our current member data, and save it for later
            MembershipComponent<M>? previousMemberData = GetComponent<MembershipComponent<M>>(entityId);

            // If we don't have any, there's nothing to do.
            if (previousMemberData is null)
            {
                return;
            }

            // Otherwise, remove membership data from ourselves
            RemoveComponent<MembershipComponent<M>>(entityId);

            // Then, retrieve the data for the previous collection.
            EntityId previousCollectionId = previousMemberData.Collection;
            CollectionComponent<M>? previousCollectionComponent = GetComponent<CollectionComponent<M>>(previousCollectionId);

            // If it's missing (probably because the entity has been trashed), no action is needed
            if (previousCollectionComponent is null)
            {
                return;
            }

            // Otherwise, remove this member from it.
            previousCollectionComponent.Members.Remove(entityId);
        }

        public void SetMembership<M>(EntityId memberId, M memberData, EntityId destinationCollectionId)
            where M : class
        {
            // Ensure that the type M actually is a registered membership component.
            // Throw an exception otherwise.
            if (!membershipComponentTypes.Contains(typeof(M)))
            {
                throw new ComponentException($"Type {typeof(M)} is not a membership component.");
            }

            // Verify that the entity id's given are both alive.
            CheckEntityIsCurrent(memberId);
            CheckEntityIsCurrent(destinationCollectionId);

            // Retrieve existing membership data from the member entity, if any.
            MembershipComponent<M>? previousMemberData = GetComponent<MembershipComponent<M>>(memberId);

            // Set the new member data for the member entity.
            SetComponent(memberId, new MembershipComponent<M>(memberData, destinationCollectionId));

            // If the collection entity is the same as the previous one, no other changes are needed
            if (destinationCollectionId.Equals(previousMemberData?.Collection))
            {
                return;
            }

            // Otherwise, add the member to the destination collection - creating a new collection component if needed.
            CollectionComponent<M>? destinationCollectionData
                = GetComponent<CollectionComponent<M>>(destinationCollectionId);
            if (destinationCollectionData is null)
            {
                CollectionComponent<M> newCollectionData = new();
                SetComponent(destinationCollectionId, newCollectionData);
                destinationCollectionData = newCollectionData;
            }
            destinationCollectionData.Members.Add(memberId);

            // If the new member entity had no previous collection, we are all done.
            if (previousMemberData is null)
            {
                return;
            }

            // Otherwise, retrieve the data for the previous collection.
            EntityId previousCollectionId = previousMemberData.Collection;
            CollectionComponent<M>? previousCollectionComponent = GetComponent<CollectionComponent<M>>(previousCollectionId);

            // If it's missing (probably because the entity has been trashed), no action is needed
            if (previousCollectionComponent is null)
            {
                return;
            }

            // Otherwise, remove this member from it.
            previousCollectionComponent.Members.Remove(memberId);
        }

        private class CollectionComponent<T>
        {
            public List<EntityId> Members { get; } = new();

            public IEnumerable<Entity> EnumerateMembers(World world)
            {
                Members.RemoveAll(e => !world.EntityIsCurrent(e));
                return Members.Select(e => new Entity(e, world));
            }
        }

        private record MembershipComponent<T>(T MembershipData, EntityId Collection);
    }
}