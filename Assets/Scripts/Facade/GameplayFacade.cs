using UnityEngine;

/// <summary>
/// 몬스터 사망 → 아이템 드랍 → 풀 회수 → 상위 매니저 통지를 한 곳에서 조율하는 파사드.
/// Monster와 Item을 서로 직접 연결하지 않는다.
/// </summary>
public class GameplayFacade : MonoBehaviour
{
    [SerializeField] SpawnFactory spawnFactory;
    [SerializeField] DataManager dataManager;
    [SerializeField] ItemManager itemManager;
    [SerializeField] StageManager stageManager;

    void OnEnable()
    {
        GameplayEventHub.MonsterDied += HandleMonsterDied;
        GameplayEventHub.ItemPickedUp += HandleItemPickedUp;
    }

    void OnDisable()
    {
        GameplayEventHub.MonsterDied -= HandleMonsterDied;
        GameplayEventHub.ItemPickedUp -= HandleItemPickedUp;
    }

    void HandleMonsterDied(StructData.MonsterDiedPayload payload)
    {
        DropItems(payload);
        NotifyStage(payload);
        Factory.Despawn(payload.Source);
    }

    void HandleItemPickedUp(StructData.ItemPickedUpPayload payload)
    {
        if (itemManager != null)
        {
            itemManager.HandleItemPickedUp(payload);
        }

        Factory.Despawn(payload.Source);
    }

    void DropItems(in StructData.MonsterDiedPayload payload)
    {
        if (Data == null || !Data.TryGetDropTable(payload.MonsterId, out var table) || table.Entries == null)
        {
            return;
        }

        for (var i = 0; i < table.Entries.Length; i++)
        {
            var entry = table.Entries[i];
            if (entry.ItemId == EnumData.ItemId.None || Random.value > Mathf.Clamp01(entry.Chance))
            {
                continue;
            }

            var count = Random.Range(entry.MinCount, entry.MaxCount + 1);
            for (var n = 0; n < count; n++)
            {
                var dropPosition = payload.Position + Random.insideUnitSphere * 0.35f;
                dropPosition.y = payload.Position.y;
                Factory.SpawnItem(entry.ItemId, dropPosition, Quaternion.identity);

                var dropped = new StructData.ItemDroppedPayload
                {
                    SourceMonsterId = payload.MonsterId,
                    ItemId = entry.ItemId,
                    Position = dropPosition
                };

                GameplayEventHub.RaiseItemDropped(dropped);
                if (itemManager != null)
                {
                    itemManager.HandleItemDropped(dropped);
                }
            }
        }
    }

    void NotifyStage(in StructData.MonsterDiedPayload payload)
    {
        var defeated = new StructData.MonsterDefeatedPayload
        {
            MonsterId = payload.MonsterId,
            Position = payload.Position
        };

        GameplayEventHub.RaiseMonsterDefeated(defeated);
        if (stageManager != null)
        {
            stageManager.HandleMonsterDefeated(defeated);
        }
    }

    InterfaceData.ISpawnFactory Factory =>
        spawnFactory != null ? spawnFactory : SpawnFactory.Instance;

    DataManager Data =>
        dataManager != null ? dataManager : DataManager.Instance;
}
