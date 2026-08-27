using UnityEngine;

/// <summary>
/// 몬스터 의사결정. FSM: Idle, Trace, Attack, Dead.
/// </summary>
public sealed class MonsterAI
{
    public EnumData.MonsterState State { get; private set; } = EnumData.MonsterState.Idle;

    float _attackTimer;

    public void Reset()
    {
        State = EnumData.MonsterState.Idle;
        _attackTimer = 0f;
    }

    public void ForceDead()
    {
        State = EnumData.MonsterState.Dead;
        _attackTimer = 0f;
    }

    public void Tick(MonsterController monster, float deltaTime)
    {
        if (monster == null || monster.Status.IsDead)
        {
            ForceDead();
            return;
        }

        var target = monster.FindTarget();
        var distance = target == null
            ? float.PositiveInfinity
            : Vector3.Distance(monster.transform.position, target.position);
        var stat = monster.Status.Stat;

        switch (State)
        {
            case EnumData.MonsterState.Idle:
                if (target != null && distance <= stat.TraceRange)
                {
                    SetState(EnumData.MonsterState.Trace, monster);
                }
                break;

            case EnumData.MonsterState.Trace:
                if (target == null || distance > stat.TraceRange)
                {
                    SetState(EnumData.MonsterState.Idle, monster);
                    break;
                }

                if (distance <= stat.AttackRange)
                {
                    SetState(EnumData.MonsterState.Attack, monster);
                    break;
                }

                monster.MoveTowards(target.position, deltaTime);
                break;

            case EnumData.MonsterState.Attack:
                if (target == null || distance > stat.AttackRange)
                {
                    SetState(target == null ? EnumData.MonsterState.Idle : EnumData.MonsterState.Trace, monster);
                    break;
                }

                _attackTimer -= deltaTime;
                if (_attackTimer <= 0f)
                {
                    monster.PerformAttack(target);
                    _attackTimer = Mathf.Max(0.1f, stat.AttackCooldown);
                }
                break;

            case EnumData.MonsterState.Dead:
                break;
        }
    }

    void SetState(EnumData.MonsterState next, MonsterController monster)
    {
        if (State == next)
        {
            return;
        }

        State = next;
        monster.Status.SetState(next);
        if (next == EnumData.MonsterState.Attack)
        {
            _attackTimer = 0f;
        }
    }
}
