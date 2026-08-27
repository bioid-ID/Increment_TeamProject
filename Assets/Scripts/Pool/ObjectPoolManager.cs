using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 오브젝트 풀 서비스.
/// 최종 구현 담당은 이효준. 이 파일은 IObjectPoolService 계약을 맞춘 연동용 기본 구현이다.
/// 이효준 매니저가 합쳐지면 SpawnFactory는 인터페이스만 유지한 채 교체하면 된다.
/// </summary>
public class ObjectPoolManager : MonoBehaviour, InterfaceData.IObjectPoolService
{
    [Serializable]
    public class PoolPrefabEntry
    {
        public EnumData.PoolObjectType Type = EnumData.PoolObjectType.None;
        public GameObject Prefab;
        public int PrewarmCount = 8;
    }

    public static ObjectPoolManager Instance { get; private set; }

    [SerializeField] List<PoolPrefabEntry> entries = new List<PoolPrefabEntry>();
    [SerializeField] Transform inactiveRoot;

    readonly Dictionary<EnumData.PoolObjectType, Queue<InterfaceData.IPoolable>> _pools =
        new Dictionary<EnumData.PoolObjectType, Queue<InterfaceData.IPoolable>>();
    readonly Dictionary<EnumData.PoolObjectType, GameObject> _prefabs =
        new Dictionary<EnumData.PoolObjectType, GameObject>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        if (inactiveRoot == null)
        {
            var root = new GameObject("PooledInactive");
            root.transform.SetParent(transform);
            inactiveRoot = root.transform;
        }

        BuildPools();
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    void BuildPools()
    {
        foreach (var entry in entries)
        {
            if (entry.Type == EnumData.PoolObjectType.None || entry.Prefab == null)
            {
                continue;
            }

            _prefabs[entry.Type] = entry.Prefab;
            if (!_pools.ContainsKey(entry.Type))
            {
                _pools[entry.Type] = new Queue<InterfaceData.IPoolable>();
            }

            for (var i = 0; i < Mathf.Max(0, entry.PrewarmCount); i++)
            {
                _pools[entry.Type].Enqueue(CreateInstance(entry.Type));
            }
        }
    }

    public T Get<T>(EnumData.PoolObjectType type, in StructData.SpawnContext context)
        where T : Component, InterfaceData.IPoolable
    {
        var pooled = GetOrCreate(type);
        if (pooled is T typed)
        {
            typed.OnSpawn(context);
            return typed;
        }

        throw new InvalidOperationException(
            $"[ObjectPoolManager] {type} prefab does not implement {typeof(T).Name}.");
    }

    public void Return(InterfaceData.IPoolable instance)
    {
        if (instance == null)
        {
            return;
        }

        instance.OnDespawn();
        instance.transform.SetParent(inactiveRoot, false);

        if (!_pools.TryGetValue(instance.PoolType, out var queue))
        {
            queue = new Queue<InterfaceData.IPoolable>();
            _pools[instance.PoolType] = queue;
        }

        queue.Enqueue(instance);
    }

    InterfaceData.IPoolable GetOrCreate(EnumData.PoolObjectType type)
    {
        if (_pools.TryGetValue(type, out var queue) && queue.Count > 0)
        {
            var reused = queue.Dequeue();
            reused.transform.SetParent(null, false);
            return reused;
        }

        return CreateInstance(type);
    }

    InterfaceData.IPoolable CreateInstance(EnumData.PoolObjectType type)
    {
        if (!_prefabs.TryGetValue(type, out var prefab) || prefab == null)
        {
            throw new InvalidOperationException(
                $"[ObjectPoolManager] Prefab is not registered for {type}.");
        }

        var go = Instantiate(prefab, inactiveRoot);
        go.name = prefab.name;
        go.SetActive(false);

        if (!go.TryGetComponent<InterfaceData.IPoolable>(out var poolable))
        {
            throw new InvalidOperationException(
                $"[ObjectPoolManager] {prefab.name} must implement IPoolable.");
        }

        return poolable;
    }
}
