using UnityEngine;

/// <summary>
/// 프로젝트 공통 인터페이스 모음.
/// 매니저 구현체(이효준/손효림/이동준)와 결합하지 않도록 계약만 여기에 둔다.
/// </summary>
public static class InterfaceData
{
    public interface IPoolable
    {
        EnumData.PoolObjectType PoolType { get; }
        GameObject gameObject { get; }
        Transform transform { get; }

        void OnSpawn(in StructData.SpawnContext context);
        void OnDespawn();
    }

    public interface IObjectPoolService
    {
        T Get<T>(EnumData.PoolObjectType type, in StructData.SpawnContext context)
            where T : Component, IPoolable;

        void Return(IPoolable instance);
    }

    public interface ISpawnFactory
    {
        MonsterController SpawnMonster(EnumData.MonsterId monsterId, Vector3 position, Quaternion rotation);
        Item SpawnItem(EnumData.ItemId itemId, Vector3 position, Quaternion rotation);
        void Despawn(IPoolable instance);
    }

    public interface IGameDataService
    {
        bool TryGetMonsterStat(EnumData.MonsterId id, out StructData.MonsterStat stat);
        bool TryGetItemStat(EnumData.ItemId id, out StructData.ItemStat stat);
        bool TryGetDropTable(EnumData.MonsterId id, out StructData.DropTable table);
        EnumData.PoolObjectType GetMonsterPoolType(EnumData.MonsterId id);
        EnumData.PoolObjectType GetItemPoolType(EnumData.ItemId id);
    }

    public interface IDamageable
    {
        void TakeDamage(int amount);
    }

    public interface IMonsterStageStatusProvider
    {
        StructData.MonsterStat ApplyStage(in StructData.MonsterStat baseStat, EnumData.StageId stageId);
        StructData.PaletteSwap GetPalette(EnumData.StageId stageId);
    }
}
