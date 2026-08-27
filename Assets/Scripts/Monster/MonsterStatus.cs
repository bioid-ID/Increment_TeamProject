/// <summary>
/// 몬스터 런타임 상태. MonoBehaviour가 아닌 native C# 객체.
/// </summary>
public sealed class MonsterStatus
{
    public EnumData.MonsterId Id { get; private set; }
    public EnumData.StageId StageId { get; private set; }
    public EnumData.MonsterState State { get; private set; }
    public StructData.MonsterStat Stat { get; private set; }
    public StructData.PaletteSwap Palette { get; private set; }
    public int CurrentHp { get; private set; }

    public bool IsDead => State == EnumData.MonsterState.Dead || CurrentHp <= 0;

    public void Reset(EnumData.MonsterId id, EnumData.StageId stageId, in StructData.MonsterStat stat, in StructData.PaletteSwap palette)
    {
        Id = id;
        StageId = stageId;
        Stat = stat;
        Palette = palette;
        CurrentHp = UnityEngine.Mathf.Max(1, stat.MaxHp);
        State = EnumData.MonsterState.Idle;
    }

    public void SetState(EnumData.MonsterState state)
    {
        State = state;
    }

    public bool ApplyDamage(int amount)
    {
        if (IsDead || amount <= 0)
        {
            return false;
        }

        CurrentHp = UnityEngine.Mathf.Max(0, CurrentHp - amount);
        if (CurrentHp == 0)
        {
            State = EnumData.MonsterState.Dead;
            return true;
        }

        return false;
    }

    public void Clear()
    {
        CurrentHp = 0;
        State = EnumData.MonsterState.Idle;
    }
}
