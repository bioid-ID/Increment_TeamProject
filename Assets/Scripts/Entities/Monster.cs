using UnityEngine;

/// <summary>
/// 몬스터 엔티티. 아이템/매니저를 직접 참조하지 않는다.
/// 사망 시 GameplayEventHub로만 알리고, 드랍/회수는 Facade가 처리한다.
/// 담당: 신현수
/// </summary>
public class Monster : PooledEntity
{
    [SerializeField] EnumData.MonsterId monsterId = EnumData.MonsterId.Default;

    EnumData.MonsterState _state = EnumData.MonsterState.Despawned;
    StructData.MonsterStat _stat;
    int _currentHp;

    public EnumData.MonsterId MonsterId => monsterId;
    public EnumData.MonsterState State => _state;
    public int CurrentHp => _currentHp;
    public bool IsDead => _state == EnumData.MonsterState.Dead;

    protected override void OnSpawned(in StructData.SpawnContext context)
    {
        monsterId = (EnumData.MonsterId)context.DataId;

        if (DataManager.Instance != null &&
            DataManager.Instance.TryGetMonsterStat(monsterId, out var stat))
        {
            _stat = stat;
        }
        else
        {
            _stat = new StructData.MonsterStat
            {
                Id = monsterId,
                PoolType = context.PoolType,
                MaxHp = 10,
                MoveSpeed = 1f
            };
        }

        _currentHp = Mathf.Max(1, _stat.MaxHp);
        _state = EnumData.MonsterState.Spawned;
    }

    protected override void OnDespawned()
    {
        _currentHp = 0;
        _state = EnumData.MonsterState.Despawned;
    }

    public void TakeDamage(int amount)
    {
        if (!IsSpawned || IsDead || amount <= 0)
        {
            return;
        }

        _currentHp = Mathf.Max(0, _currentHp - amount);
        if (_currentHp == 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (!IsSpawned || IsDead)
        {
            return;
        }

        _state = EnumData.MonsterState.Dead;
        GameplayEventHub.RaiseMonsterDied(new StructData.MonsterDiedPayload
        {
            MonsterId = monsterId,
            Position = transform.position,
            Source = this
        });
    }
}
