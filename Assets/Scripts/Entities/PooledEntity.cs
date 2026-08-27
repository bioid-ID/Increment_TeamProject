using UnityEngine;

/// <summary>
/// MonsterController / ItemController 공통 풀링 베이스.
/// Instantiate/Destroy를 쓰지 않고 OnSpawn/OnDespawn에서만 상태를 초기화한다.
/// </summary>
public abstract class PooledEntity : MonoBehaviour, InterfaceData.IPoolable
{
    [SerializeField] EnumData.PoolObjectType poolType = EnumData.PoolObjectType.None;

    public EnumData.PoolObjectType PoolType => poolType;
    public bool IsSpawned { get; private set; }

    public virtual void OnSpawn(in StructData.SpawnContext context)
    {
        poolType = context.PoolType;
        transform.SetPositionAndRotation(context.Position, context.Rotation);
        gameObject.SetActive(true);
        IsSpawned = true;
        OnSpawned(context);
    }

    public virtual void OnDespawn()
    {
        OnDespawned();
        IsSpawned = false;
        gameObject.SetActive(false);
    }

    protected abstract void OnSpawned(in StructData.SpawnContext context);
    protected abstract void OnDespawned();
}
