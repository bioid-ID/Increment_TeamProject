using UnityEngine;

/// <summary>
/// 풀에서 몬스터/아이템을 꺼내고 되돌리는 팩토리.
/// 엔티티는 이 클래스를 몰라도 되고, Facade와 스테이지 스폰 지점만 호출한다.
/// </summary>
public class SpawnFactory : MonoBehaviour, InterfaceData.ISpawnFactory
{
    public static SpawnFactory Instance { get; private set; }

    [SerializeField] ObjectPoolManager poolManager;

    InterfaceData.IObjectPoolService Pool =>
        (InterfaceData.IObjectPoolService)poolManager ?? ObjectPoolManager.Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        if (poolManager == null)
        {
            poolManager = ObjectPoolManager.Instance;
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public Monster SpawnMonster(EnumData.MonsterId monsterId, Vector3 position, Quaternion rotation)
    {
        var context = StructData.SpawnContext.ForMonster(monsterId, position, rotation);
        context.PoolType = ResolveMonsterPoolType(monsterId);
        return Pool.Get<Monster>(context.PoolType, context);
    }

    public Item SpawnItem(EnumData.ItemId itemId, Vector3 position, Quaternion rotation)
    {
        var context = StructData.SpawnContext.ForItem(itemId, position, rotation);
        context.PoolType = ResolveItemPoolType(itemId);
        return Pool.Get<Item>(context.PoolType, context);
    }

    public void Despawn(InterfaceData.IPoolable instance)
    {
        if (instance == null)
        {
            return;
        }

        Pool.Return(instance);
    }

    static EnumData.PoolObjectType ResolveMonsterPoolType(EnumData.MonsterId monsterId)
    {
        if (DataManager.Instance != null)
        {
            return DataManager.Instance.GetMonsterPoolType(monsterId);
        }

        return EnumData.PoolObjectType.Monster_Default;
    }

    static EnumData.PoolObjectType ResolveItemPoolType(EnumData.ItemId itemId)
    {
        if (DataManager.Instance != null)
        {
            return DataManager.Instance.GetItemPoolType(itemId);
        }

        return EnumData.PoolObjectType.Item_Default;
    }
}
