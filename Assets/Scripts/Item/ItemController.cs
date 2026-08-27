using UnityEngine;

/// <summary>
/// 필드 드랍 아이템 컨트롤러. 인벤토리/몬스터를 직접 참조하지 않는다.
/// 담당: 신현수
/// </summary>
public class ItemController : PooledEntity
{
    [SerializeField] EnumData.ItemId itemId = EnumData.ItemId.Default;
    [SerializeField] int upgradeLevel;
    [SerializeField] int starForce;
    [SerializeField] string collectorTag = "Player";

    readonly ItemStatus _status = new ItemStatus();
    readonly ItemStatusProvider _statusProvider = ItemStatusProvider.Default;

    public EnumData.ItemId ItemId => itemId;
    public ItemStatus Status => _status;

    protected override void OnSpawned(in StructData.SpawnContext context)
    {
        itemId = (EnumData.ItemId)context.DataId;
        var stat = ResolveBaseStat(context.PoolType);
        _statusProvider.ApplyTo(_status, stat, upgradeLevel, starForce);
    }

    protected override void OnDespawned()
    {
        _status.Clear();
    }

    void OnTriggerEnter(Collider other)
    {
        TryPickUp(other.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        TryPickUp(other.gameObject);
    }

    public void TryPickUp(GameObject collector)
    {
        if (!IsSpawned || _status.PickedUp || collector == null)
        {
            return;
        }

        if (!string.IsNullOrEmpty(collectorTag) && !collector.CompareTag(collectorTag))
        {
            return;
        }

        _status.MarkPickedUp();
        GameplayEventHub.RaiseItemPickedUp(new StructData.ItemPickedUpPayload
        {
            ItemId = _status.Id,
            Type = _status.Type,
            Value = _status.EffectiveValue,
            Collector = collector,
            Source = this
        });
    }

    StructData.ItemStat ResolveBaseStat(EnumData.PoolObjectType poolType)
    {
        if (DataManager.Instance != null && DataManager.Instance.TryGetItemStat(itemId, out var stat))
        {
            return stat;
        }

        return new StructData.ItemStat
        {
            Id = itemId,
            Type = itemId == EnumData.ItemId.Weapon ? EnumData.ItemType.Weapon : EnumData.ItemType.Currency,
            PoolType = poolType,
            Value = 1,
            UpgradeStep = 1
        };
    }
}
