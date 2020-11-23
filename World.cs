using System;
using System.Collections.Generic;

internal interface IWriteWrapperFactory
{
    /// <summary>
    /// Creates a <see cref="Writable{T}"/> containing the provided <paramref name="initialValue"/>
    /// This method will fail with <see cref="InvalidCastException"/> if the runtime type of
    /// <paramref name="initialValue"/> does not match the generic type parameter of the underlying
    /// <see cref="WriteWrapperFactory{T}"/> instance.
    /// </summary>
    /// <param name="initialValue"></param>
    /// <returns></returns>
    public object TrustedCreate(object initialValue);
}
internal class WriteWrapperFactory<T> : IWriteWrapperFactory
    where T : notnull
{
    public Writable<T> Create(T initialValue)
    {
        return new Writable<T>(initialValue);
    }

    public object TrustedCreate(object initialValue)
    {
        return Create((T)initialValue);
    }
}

internal interface IWritable
{
    object GetWrittenValue();
    bool WasDestroyed();
}

public class Writable<T> : IWritable
    where T : notnull
{
    public Writable(T initialValue)
    {
        Value = initialValue;
    }
    public T Value { get; }
    internal bool Destroyed { get; set; } = false;
    internal T? NewValue { get; set; }
    public void Write(T newValue)
    {
        NewValue = newValue;
    }
    public void Destroy()
    {
        Destroyed = true;
    }

    public object GetWrittenValue()
    {
        return NewValue ?? Value;
    }

    public bool WasDestroyed()
    {
        throw new NotImplementedException();
    }
}

public static class WriteableUtil
{
    public static void Destroy<T>(ref Writable<T>? writableT) where T : class
    {
        if (writableT != null)
        {
            writableT.Destroyed = true;
            writableT = null;
        }
    }
}

public interface IComponentStorage
{
    object? this[int index]
    {
        get;
        set;
    }

    public void Expand();
    public void Remove(int index);
}

public class ListComponentStorage : IComponentStorage
{
    private List<object?> innerList;

    public ListComponentStorage(int count)
    {
        this.innerList = new List<object?>(new object?[count]);
    }

    public object? this[int index]
    { 
        get => innerList[index];
        set => innerList[index] = value;
    }

    public void Expand()
    {
        innerList.Add(null);
    }

    public void Remove(int index)
    {
        innerList[index] = null;
    }
}

public class DictionaryComponentStorage : IComponentStorage
{
    private Dictionary<int, object?> innerDictionary;

    public DictionaryComponentStorage(Dictionary<int, object?> innerDictionary)
    {
        this.innerDictionary = innerDictionary;
    }

    public void Expand()
    {
        // Nothing to do - absent keys will already return null.
    }
    public object? this[int index]
    { 
        get => innerDictionary[index];
        set => innerDictionary[index] = value;
    }

    public void Remove(int index)
    {
        innerDictionary.Remove(index);
    }
}

public class IdGameSystem
{
    internal IdGameSystem(Action<EntityId, object[]> effect, ParameterKey[] parameterKeys)
    {
        Effect = effect;
        ParameterKeys = parameterKeys;
    }

    public Action<EntityId, object[]> Effect { get; }
    internal ParameterKey[] ParameterKeys { get; }
}

internal struct ParameterKey
{
    internal int componentId;
    internal bool needsWritable;

    internal ParameterKey(int componentId, bool needsWritable)
    {
        this.componentId = componentId;
        this.needsWritable = needsWritable;
    }
}

public class World
{

    private Dictionary<Type, int> componentIdsByType = new();
    private List<IComponentStorage> componentData = new();
    private List<IWriteWrapperFactory> writeWrapperFactories = new();
    private int entityCount = 0;
    private DeadIndexList deadIndexes = new();
    private List<int> generationByEntityIndex = new();
    private List<IdGameSystem> IdSystems = new();

    private ParameterKey ParameterKeyByType(Type type)
    {
        if (
            type.IsGenericType
            && type.GetGenericTypeDefinition() == typeof(Writable<>))
        {
            Type guardedType = type.GetGenericArguments()[0];
            return new ParameterKey(componentIdsByType[guardedType], true);
        }
        else
        {
            return new ParameterKey(componentIdsByType[type], false);
        }
    }

    public void InstallSystem<T0,T1>(Action<EntityId,T0,T1> effect)
    {
        Action<EntityId, object[]> permissiveAction = (id, args) =>
        {
            effect(
                id
                , (T0)args[0]
                , (T1)args[1]
            );
        };
        ParameterKey[] componentIds = new ParameterKey[]
        {
            ParameterKeyByType(typeof(T0))
            ,ParameterKeyByType(typeof(T1))
        };
        IdGameSystem system = new IdGameSystem(permissiveAction, componentIds);
        IdSystems.Add(system);
    }

    #region Additional IstallSystem implementations
    public void InstallSystem<T0,T1,T2>(Action<EntityId,T0,T1,T2> effect)
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
        IdGameSystem system = new IdGameSystem(permissiveAction, parameterKeys);
        IdSystems.Add(system);
    }
    #endregion

    /// <summary>
    /// Runs the game loop, firing each system once in order of registration.
    /// </summary>
    public void Run()
    {
        foreach (IdGameSystem system in IdSystems)
        {
            RunSystem(system);
        }
    }

    private EntityId CreateEntityId(int index)
    {
        return new EntityId(index, generationByEntityIndex[index]);
    }

    private object? GetComponentData(ParameterKey parameterKey, int entityIndex)
    {
        object? foundComponent = componentData[parameterKey.componentId][entityIndex];
        if (foundComponent == null)
        {
            return null;
        }
        if (parameterKey.needsWritable)
        {
            IWriteWrapperFactory factory = writeWrapperFactories[parameterKey.componentId];
            return factory.TrustedCreate(foundComponent);
        }
        else
        {
            return foundComponent;
        }
    }

    private void RunSystem(IdGameSystem system)
    {
        for (int entityIndex = 0; entityIndex < entityCount; entityIndex++)
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
                object? foundComponent = GetComponentData(parameterKey, entityIndex);
                if (foundComponent == null)
                {
                    missingComponents = true;
                    break;
                }
                else
                {
                    arguments[argumentIndex] = foundComponent;
                    argumentIndex += 1;
                }
            }
            if (missingComponents)
            {
                continue;
            }
            else
            {
                system.Effect(CreateEntityId(entityIndex), arguments);
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
        int componentId = GetComponentId<T>();
        if (!EntityIsCurrent(entityId))
        {
            return null;
        }
        // Need checks here - index can fail if they plug in
        // an EntityId from another World
        return (T?)componentData[componentId][entityId.index];
    }

    public void RegisterComponent<T>() where T : notnull
    {
        // This list will be full of "value=null, generation=0" at first
        ListComponentStorage entries = new ListComponentStorage(entityCount);
        componentData.Add(entries);
        writeWrapperFactories.Add(new WriteWrapperFactory<T>());
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
            throw new Exception("Entity has already been destroyed");
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
        if (!componentIdsByType.TryGetValue(typeof(T), out int componentId))
        {
            throw new Exception($"No component registered for {typeof(T)}");
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
            throw new Exception("Entity has already been destroyed");
        }
        componentData[componentId].Remove(entityId.index);
    }

    /// <summary>
    /// Create a new entity, with no components set.
    /// </summary>
    /// <returns>The id of the newly created entity.</returns>
    public EntityId CreateEntity()
    {
        // Look for destroyed entities and reclaim their indexes
        if (deadIndexes.ReclaimIndex(out int indexToUsurp))
        {
            foreach (IComponentStorage componentStorage in componentData)
            {
                componentStorage[indexToUsurp] = null;
            }
            return new EntityId(indexToUsurp, generationByEntityIndex[indexToUsurp]);
        }
        else
        {
            foreach (IComponentStorage componentStorage in componentData)
            {
                componentStorage.Expand();
            }
            generationByEntityIndex.Add(0);
            deadIndexes.AddLivingMember();
            entityCount += 1;
            return new EntityId(entityCount-1, 0);
        }
    }

    private bool EntityIsCurrent(EntityId entityId)
    {
        return generationByEntityIndex[entityId.index] == entityId.generation;
    }

    /// <summary>
    /// Destroy an entity, freeing space for the creation of new ones.
    /// NOTE: Any future usage of this entity's id, except for 
    /// <see cref="GetComponent{T}(EntityId)"/>, will result in an exceptions.
    /// </summary>
    /// <param name="entityId">The id of the entity to destroy</param>
    public void DestroyEntity(EntityId entityId)
    {
        if (!EntityIsCurrent(entityId))
        {
            throw new Exception("Entity has already been destroyed");
        }
        generationByEntityIndex[entityId.index] += 1;
        deadIndexes.MarkDead(entityId.index);
    }
}
