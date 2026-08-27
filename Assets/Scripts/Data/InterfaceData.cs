using UnityEngine;

/// <summary>
/// 프로젝트 공통 인터페이스 모음.
/// 매니저 구현체(이효준/손효림/이동준)와 결합하지 않도록 계약만 여기에 둔다.
/// </summary>
public static class InterfaceData
{
    /// <summary>
    /// ObjectPoolManager가 Spawn/Despawn 시점에 호출하는 풀링 계약.
    /// </summary>
    public interface IPoolable
    {
        EnumData.PoolObjectType PoolType { get; }
        GameObject gameObject { get; }
        Transform transform { get; }

        void OnSpawn(in StructData.SpawnContext context);
        void OnDespawn();
    }

    /// <summary>
    /// ObjectPoolManager(담당: 이효준)가 구현할 풀 서비스 계약.
    /// 몬스터/아이템 코드는 구체 클래스가 아니라 이 인터페이스에만 의존한다.
    /// </summary>
    public interface IObjectPoolService
    {
        T Get<T>(EnumData.PoolObjectType type, in StructData.SpawnContext context)
            where T : Component, IPoolable;

        void Return(IPoolable instance);
    }

    public interface ISpawnFactory
    {
        Monster SpawnMonster(EnumData.MonsterId monsterId, Vector3 position, Quaternion rotation);
        Item SpawnItem(EnumData.ItemId itemId, Vector3 position, Quaternion rotation);
        void Despawn(IPoolable instance);
    }

    /// <summary>
    /// DataManager가 제공할 읽기 전용 데이터 조회 계약.
    /// </summary>
    public interface IGameDataService
    {
        bool TryGetMonsterStat(EnumData.MonsterId id, out StructData.MonsterStat stat);
        bool TryGetItemStat(EnumData.ItemId id, out StructData.ItemStat stat);
        bool TryGetDropTable(EnumData.MonsterId id, out StructData.DropTable table);
        EnumData.PoolObjectType GetMonsterPoolType(EnumData.MonsterId id);
        EnumData.PoolObjectType GetItemPoolType(EnumData.ItemId id);
    }
}
