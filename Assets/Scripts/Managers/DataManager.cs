using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 몬스터/아이템 스탯과 드랍 테이블 조회.
/// DataManager는 팀 공용. 여기서는 Monster/Item이 필요한 읽기 API만 제공한다.
/// </summary>
public class DataManager : MonoBehaviour, InterfaceData.IGameDataService
{
    [Serializable]
    public class MonsterStatEntry
    {
        public EnumData.MonsterId Id = EnumData.MonsterId.Default;
        public EnumData.PoolObjectType PoolType = EnumData.PoolObjectType.Monster_Default;
        public int MaxHp = 10;
        public float MoveSpeed = 1f;
    }

    [Serializable]
    public class ItemStatEntry
    {
        public EnumData.ItemId Id = EnumData.ItemId.Default;
        public EnumData.ItemType Type = EnumData.ItemType.Currency;
        public EnumData.PoolObjectType PoolType = EnumData.PoolObjectType.Item_Default;
        public int Value = 1;
    }

    [Serializable]
    public class DropEntryRecord
    {
        public EnumData.ItemId ItemId = EnumData.ItemId.Default;
        [Range(0f, 1f)] public float Chance = 1f;
        public int MinCount = 1;
        public int MaxCount = 1;
    }

    [Serializable]
    public class DropTableRecord
    {
        public EnumData.MonsterId MonsterId = EnumData.MonsterId.Default;
        public List<DropEntryRecord> Entries = new List<DropEntryRecord>();
    }

    public static DataManager Instance { get; private set; }

    [SerializeField] List<MonsterStatEntry> monsterStats = new List<MonsterStatEntry>
    {
        new MonsterStatEntry()
    };
    [SerializeField] List<ItemStatEntry> itemStats = new List<ItemStatEntry>
    {
        new ItemStatEntry()
    };
    [SerializeField] List<DropTableRecord> dropTables = new List<DropTableRecord>
    {
        new DropTableRecord
        {
            Entries = new List<DropEntryRecord> { new DropEntryRecord() }
        }
    };

    readonly Dictionary<EnumData.MonsterId, StructData.MonsterStat> _monsterMap =
        new Dictionary<EnumData.MonsterId, StructData.MonsterStat>();
    readonly Dictionary<EnumData.ItemId, StructData.ItemStat> _itemMap =
        new Dictionary<EnumData.ItemId, StructData.ItemStat>();
    readonly Dictionary<EnumData.MonsterId, StructData.DropTable> _dropMap =
        new Dictionary<EnumData.MonsterId, StructData.DropTable>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        RebuildMaps();
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void RebuildMaps()
    {
        _monsterMap.Clear();
        _itemMap.Clear();
        _dropMap.Clear();

        foreach (var entry in monsterStats)
        {
            _monsterMap[entry.Id] = new StructData.MonsterStat
            {
                Id = entry.Id,
                PoolType = entry.PoolType,
                MaxHp = entry.MaxHp,
                MoveSpeed = entry.MoveSpeed
            };
        }

        foreach (var entry in itemStats)
        {
            _itemMap[entry.Id] = new StructData.ItemStat
            {
                Id = entry.Id,
                Type = entry.Type,
                PoolType = entry.PoolType,
                Value = entry.Value
            };
        }

        foreach (var table in dropTables)
        {
            var entries = new StructData.DropEntry[table.Entries.Count];
            for (var i = 0; i < table.Entries.Count; i++)
            {
                var source = table.Entries[i];
                entries[i] = new StructData.DropEntry
                {
                    ItemId = source.ItemId,
                    Chance = source.Chance,
                    MinCount = Mathf.Max(0, source.MinCount),
                    MaxCount = Mathf.Max(source.MinCount, source.MaxCount)
                };
            }

            _dropMap[table.MonsterId] = new StructData.DropTable
            {
                MonsterId = table.MonsterId,
                Entries = entries
            };
        }
    }

    public bool TryGetMonsterStat(EnumData.MonsterId id, out StructData.MonsterStat stat)
    {
        return _monsterMap.TryGetValue(id, out stat);
    }

    public bool TryGetItemStat(EnumData.ItemId id, out StructData.ItemStat stat)
    {
        return _itemMap.TryGetValue(id, out stat);
    }

    public bool TryGetDropTable(EnumData.MonsterId id, out StructData.DropTable table)
    {
        return _dropMap.TryGetValue(id, out table);
    }

    public EnumData.PoolObjectType GetMonsterPoolType(EnumData.MonsterId id)
    {
        return TryGetMonsterStat(id, out var stat) ? stat.PoolType : EnumData.PoolObjectType.Monster_Default;
    }

    public EnumData.PoolObjectType GetItemPoolType(EnumData.ItemId id)
    {
        return TryGetItemStat(id, out var stat) ? stat.PoolType : EnumData.PoolObjectType.Item_Default;
    }
}
