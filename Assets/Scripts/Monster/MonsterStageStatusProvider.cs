using UnityEngine;

/// <summary>
/// 스테이지별 몬스터 스탯 보정과 팔레트 스왑. native C#.
/// </summary>
public sealed class MonsterStageStatusProvider : InterfaceData.IMonsterStageStatusProvider
{
    public static readonly MonsterStageStatusProvider Default = new MonsterStageStatusProvider();

    static readonly Color[] Palette =
    {
        new Color(0.85f, 0.35f, 0.35f),
        new Color(0.35f, 0.75f, 0.45f),
        new Color(0.35f, 0.45f, 0.90f),
        new Color(0.90f, 0.75f, 0.25f)
    };

    public StructData.MonsterStat ApplyStage(in StructData.MonsterStat baseStat, EnumData.StageId stageId)
    {
        var stage = Mathf.Max(1, (int)stageId);
        var multiplier = 1f + (stage - 1) * 0.25f;

        var scaled = baseStat;
        scaled.MaxHp = Mathf.Max(1, Mathf.RoundToInt(baseStat.MaxHp * multiplier));
        scaled.AttackDamage = Mathf.Max(1, Mathf.RoundToInt(Mathf.Max(1, baseStat.AttackDamage) * multiplier));
        scaled.MoveSpeed = baseStat.MoveSpeed * multiplier;
        return scaled;
    }

    public StructData.PaletteSwap GetPalette(EnumData.StageId stageId)
    {
        var index = Mathf.Max(0, (int)stageId - 1) % Palette.Length;
        return new StructData.PaletteSwap
        {
            Index = index,
            BodyColor = Palette[index]
        };
    }
}
