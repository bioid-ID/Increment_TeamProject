using UnityEngine;

/// <summary>
/// StageManager 연동 스텁. 담당: 손효림, 이동준.
/// 스테이지 진행/클리어 판정은 이 이벤트 진입점만 유지한 채 교체하면 된다.
/// </summary>
public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    public event System.Action<StructData.MonsterDefeatedPayload> MonsterDefeated;

    [SerializeField] EnumData.StageId currentStageId = EnumData.StageId.Stage1;
    [SerializeField] int remainingMonsters;

    public EnumData.StageId CurrentStageId => currentStageId;
    public int RemainingMonsters => remainingMonsters;

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

    public void RegisterSpawnedMonster()
    {
        remainingMonsters++;
    }

    public void HandleMonsterDefeated(in StructData.MonsterDefeatedPayload payload)
    {
        remainingMonsters = Mathf.Max(0, remainingMonsters - 1);
        MonsterDefeated?.Invoke(payload);
        // TODO(손효림, 이동준): 웨이브 진행, 클리어 조건, 다음 스테이지 전환
    }
}
