using UnityEngine;

/// <summary>
/// 프로젝트 공통 Struct 모음.
/// 이벤트 페이로드와 스탯/드랍 데이터는 여기만 수정한다.
/// </summary>
public static class StructData
{
    public struct SpawnContext
    {
        public EnumData.PoolObjectType PoolType;
        public Vector3 Position;
        public Quaternion Rotation;
        public int DataId;

        public static SpawnContext ForMonster(EnumData.MonsterId id, Vector3 position, Quaternion rotation)
        {
            return new SpawnContext
            {
                PoolType = EnumData.PoolObjectType.Monster_Default,
                Position = position,
                Rotation = rotation,
                DataId = (int)id
            };
        }

        public static SpawnContext ForItem(EnumData.ItemId id, Vector3 position, Quaternion rotation)
        {
            return new SpawnContext
            {
                PoolType = EnumData.PoolObjectType.Item_Default,
                Position = position,
                Rotation = rotation,
                DataId = (int)id
            };
        }
    }

    public struct MonsterStat
    {
        public EnumData.MonsterId Id;
        public EnumData.PoolObjectType PoolType;
        public int MaxHp;
        public float MoveSpeed;
    }

    public struct ItemStat
    {
        public EnumData.ItemId Id;
        public EnumData.ItemType Type;
        public EnumData.PoolObjectType PoolType;
        public int Value;
    }

    public struct DropEntry
    {
        public EnumData.ItemId ItemId;
        public float Chance;
        public int MinCount;
        public int MaxCount;
    }

    public struct DropTable
    {
        public EnumData.MonsterId MonsterId;
        public DropEntry[] Entries;
    }

    public struct MonsterDiedPayload
    {
        public EnumData.MonsterId MonsterId;
        public Vector3 Position;
        public InterfaceData.IPoolable Source;
    }

    public struct ItemDroppedPayload
    {
        public EnumData.MonsterId SourceMonsterId;
        public EnumData.ItemId ItemId;
        public Vector3 Position;
    }

    public struct ItemPickedUpPayload
    {
        public EnumData.ItemId ItemId;
        public EnumData.ItemType Type;
        public int Value;
        public GameObject Collector;
        public InterfaceData.IPoolable Source;
    }

    public struct MonsterDefeatedPayload
    {
        public EnumData.MonsterId MonsterId;
        public Vector3 Position;
    }
}
