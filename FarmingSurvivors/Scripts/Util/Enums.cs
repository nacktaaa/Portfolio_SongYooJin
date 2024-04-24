using UnityEngine.Purchasing;
public enum Character { Pyotr, Isha, Alex }

namespace Equipment
{
    public enum Parts
    {
        MainWeapon, SubWeapon, Armor, Pants, Helmet, Shoes
    }

    public enum Grade
    {
        SEED, GRASS, FLOWER, FRUIT
    }

    public enum StatType
    {
        Hp, AttackDamage, Defence, Cooltime, MoveSpeed, Strength, Intelligence, LifeSpan, ProjectileSpeed, ProjectileAmount, ProjectileArea, Penetration, ExpPlus
    }
    public enum SortingType
    {
        None, Grade, Id
    }
    public enum Ordering{
        Descending, Ascending
    }
}




namespace StageMap
{
    public enum StageType
    //노드들의 타입. 여기 고치면 MapPlayerTracker 80번째 줄 부근 수정해주세요
    //Assets/Scriptable Objects/StageMap/Nodes/DefaultConfigNodes도 수정해주세요
    //혹시 내부 순서 바꿨다면 위 경로 들어가서 파일 하나하나 타입 다 바꿔줘야 되니까 새로운거 추가하실거면 밑에다가!
    {
        Normal,     // 일반 방
        Scarecrow,      // 허수아비 방
        Rest,       // 휴식 장소
        Gold,           // 재화 방
        Store,          // 상점
        Dungeon,        // 던전 스테이지
        Story,      // 캐릭터 고유컨셉 방
        Boss,           // 보스
        Fishing,        // 낚시
        Shooting        // 탄막 슈팅
    }
}