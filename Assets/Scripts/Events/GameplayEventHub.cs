using System;

/// <summary>
/// 모듈 간 결합도를 낮추기 위한 전역 이벤트 허브.
/// Monster/Item은 매니저를 직접 호출하지 않고 여기로만 신호를 보낸다.
/// </summary>
public static class GameplayEventHub
{
    public static event Action<StructData.MonsterDiedPayload> MonsterDied;
    public static event Action<StructData.ItemDroppedPayload> ItemDropped;
    public static event Action<StructData.ItemPickedUpPayload> ItemPickedUp;
    public static event Action<StructData.MonsterDefeatedPayload> MonsterDefeated;

    public static void RaiseMonsterDied(in StructData.MonsterDiedPayload payload)
    {
        MonsterDied?.Invoke(payload);
    }

    public static void RaiseItemDropped(in StructData.ItemDroppedPayload payload)
    {
        ItemDropped?.Invoke(payload);
    }

    public static void RaiseItemPickedUp(in StructData.ItemPickedUpPayload payload)
    {
        ItemPickedUp?.Invoke(payload);
    }

    public static void RaiseMonsterDefeated(in StructData.MonsterDefeatedPayload payload)
    {
        MonsterDefeated?.Invoke(payload);
    }

    public static void ClearAll()
    {
        MonsterDied = null;
        ItemDropped = null;
        ItemPickedUp = null;
        MonsterDefeated = null;
    }
}
