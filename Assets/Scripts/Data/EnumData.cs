/// <summary>
/// 프로젝트 공통 Enum 모음.
/// 새 Enum은 이 파일에만 추가하고, 엔티티/매니저 파일에 인라인으로 선언하지 않는다.
/// </summary>
public static class EnumData
{
    public enum PoolObjectType
    {
        None = 0,
        Monster_Default = 1,
        Item_Default = 100
    }

    public enum MonsterId
    {
        None = 0,
        Default = 1
    }

    public enum MonsterState
    {
        Idle = 0,
        Spawned = 1,
        Dead = 2,
        Despawned = 3
    }

    public enum ItemId
    {
        None = 0,
        Default = 1
    }

    public enum ItemType
    {
        None = 0,
        Consumable = 1,
        Equipment = 2,
        Currency = 3
    }
}
