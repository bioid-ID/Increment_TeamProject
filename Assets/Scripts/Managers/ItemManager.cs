using UnityEngine;

/// <summary>
/// ItemManager 연동 스텁. 담당: 손효림, 이동준.
/// 인벤토리/장비 본구현은 이 이벤트 진입점만 유지한 채 교체하면 된다.
/// </summary>
public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    public event System.Action<StructData.ItemDroppedPayload> ItemDropped;
    public event System.Action<StructData.ItemPickedUpPayload> ItemCollected;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void HandleItemDropped(in StructData.ItemDroppedPayload payload)
    {
        ItemDropped?.Invoke(payload);
    }

    public void HandleItemPickedUp(in StructData.ItemPickedUpPayload payload)
    {
        ItemCollected?.Invoke(payload);
        // TODO(손효림, 이동준): 인벤토리 적재, 즉시 사용 아이템 효과 적용
    }
}
