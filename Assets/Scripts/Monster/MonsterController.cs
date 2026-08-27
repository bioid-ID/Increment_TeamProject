using UnityEngine;

/// <summary>
/// 몬스터 컨트롤러. IPoolable 수명과 런타임 상태/AI를 연결한다.
/// 담당: 신현수
/// </summary>
public class MonsterController : PooledEntity, InterfaceData.IDamageable
{
    [SerializeField] EnumData.MonsterId monsterId = EnumData.MonsterId.Default;
    [SerializeField] string targetTag = "Player";

    readonly MonsterStatus _status = new MonsterStatus();
    readonly MonsterAI _ai = new MonsterAI();
    readonly MonsterStageStatusProvider _stageProvider = MonsterStageStatusProvider.Default;

    SpriteRenderer _spriteRenderer;
    Color _baseSpriteColor = Color.white;
    bool _deathNotified;

    public EnumData.MonsterId MonsterId => monsterId;
    public MonsterStatus Status => _status;
    public MonsterAI AI => _ai;
    public bool IsDead => _status.IsDead;

    void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (_spriteRenderer != null)
        {
            _baseSpriteColor = _spriteRenderer.color;
        }
    }

    protected override void OnSpawned(in StructData.SpawnContext context)
    {
        monsterId = (EnumData.MonsterId)context.DataId;
        var stageId = context.StageId == EnumData.StageId.None
            ? (StageManager.Instance != null ? StageManager.Instance.CurrentStageId : EnumData.StageId.Stage1)
            : context.StageId;

        var baseStat = ResolveBaseStat(context.PoolType);
        var stagedStat = _stageProvider.ApplyStage(baseStat, stageId);
        var palette = _stageProvider.GetPalette(stageId);

        _deathNotified = false;
        _status.Reset(monsterId, stageId, stagedStat, palette);
        _ai.Reset();
        ApplyPalette(palette);
    }

    protected override void OnDespawned()
    {
        _deathNotified = false;
        _ai.Reset();
        _status.Clear();
        RestorePalette();
    }

    void Update()
    {
        if (!IsSpawned || IsDead)
        {
            return;
        }

        _ai.Tick(this, Time.deltaTime);
    }

    public void TakeDamage(int amount)
    {
        if (!IsSpawned || IsDead)
        {
            return;
        }

        if (_status.ApplyDamage(amount))
        {
            Die();
        }
    }

    public void Die()
    {
        if (!IsSpawned || _deathNotified)
        {
            return;
        }

        _deathNotified = true;
        if (!_status.IsDead)
        {
            _status.ApplyDamage(_status.CurrentHp);
        }

        _ai.ForceDead();
        _status.SetState(EnumData.MonsterState.Dead);

        GameplayEventHub.RaiseMonsterDied(new StructData.MonsterDiedPayload
        {
            MonsterId = monsterId,
            Position = transform.position,
            Source = this
        });
    }

    public Transform FindTarget()
    {
        if (string.IsNullOrEmpty(targetTag))
        {
            return null;
        }

        GameObject target;
        try
        {
            target = GameObject.FindGameObjectWithTag(targetTag);
        }
        catch (UnityException)
        {
            return null;
        }

        return target != null ? target.transform : null;
    }

    public void MoveTowards(Vector3 worldPosition, float deltaTime)
    {
        var step = _status.Stat.MoveSpeed * deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, worldPosition, step);
    }

    public void PerformAttack(Transform target)
    {
        if (target == null)
        {
            return;
        }

        if (target.TryGetComponent<InterfaceData.IDamageable>(out var damageable))
        {
            damageable.TakeDamage(Mathf.Max(1, _status.Stat.AttackDamage));
        }
    }

    StructData.MonsterStat ResolveBaseStat(EnumData.PoolObjectType poolType)
    {
        if (DataManager.Instance != null && DataManager.Instance.TryGetMonsterStat(monsterId, out var stat))
        {
            return stat;
        }

        return new StructData.MonsterStat
        {
            Id = monsterId,
            PoolType = poolType,
            MaxHp = 10,
            AttackDamage = 1,
            MoveSpeed = 1.5f,
            TraceRange = 6f,
            AttackRange = 1.4f,
            AttackCooldown = 1f
        };
    }

    void ApplyPalette(in StructData.PaletteSwap palette)
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = palette.BodyColor;
        }
    }

    void RestorePalette()
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = _baseSpriteColor;
        }
    }
}
