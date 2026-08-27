using UnityEngine;

/// <summary>
/// 필드 드랍 아이템. 인벤토리/몬스터를 직접 참조하지 않는다.
/// 습득 시 GameplayEventHub로만 알리고, 회수는 Facade가 처리한다.
/// 담당: 신현수
/// </summary>
public class Item : PooledEntity
{
    [SerializeField] EnumData.ItemId itemId = EnumData.ItemId.Default;
    [SerializeField] string collectorTag = "Player";

    StructData.ItemStat _stat;
    bool _pickedUp;

    public EnumData.ItemId ItemId => itemId;
    public StructData.ItemStat Stat => _stat;

    protected override void OnSpawned(in StructData.SpawnContext context)
    {
        itemId = (EnumData.ItemId)context.DataId;
        _pickedUp = false;

        if (DataManager.Instance != null &&
            DataManager.Instance.TryGetItemStat(itemId, out var stat))
        {
            _stat = stat;
        }
        else
        {
            _stat = new StructData.ItemStat
            {
                Id = itemId,
                Type = EnumData.ItemType.Currency,
                PoolType = context.PoolType,
                Value = 1
            };
        }
    }

    protected override void OnDespawned()
    {
        _pickedUp = false;
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
        if (!IsSpawned || _pickedUp || collector == null)
        {
            return;
        }

        if (!string.IsNullOrEmpty(collectorTag) && !collector.CompareTag(collectorTag))
        {
            return;
        }

        _pickedUp = true;
        GameplayEventHub.RaiseItemPickedUp(new StructData.ItemPickedUpPayload
        {
            ItemId = _stat.Id,
            Type = _stat.Type,
            Value = _stat.Value,
            Collector = collector,
            Source = this
        });
    }
}
