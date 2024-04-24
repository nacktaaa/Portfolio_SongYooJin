using System;

public class Define 
{
    // 마우스 입력 구분 
    public enum MouseEvent
    {
        RMBDown, RMBPress, RMBUp, RMBClick,
        LMBDown, LMBPress, LMBUp, LMBClick
    }

    // 플레이어의 상태를 구분
    public enum PlayerStateType
    {
        Normal, Attack, Beaten, Death
    }
    public enum PlayerType
    {
        Nary, Susan, Bob
    }

    public enum ZombieGender
    {
        Female, Male
    }

    public enum PoolableType
    {
        Zombie, Bullet
    }

    [Flags]
    public enum FlagType
    {
        None = 0,

        Hunger      = 1 << 0, //배고픔
        Full        = 1 << 1, //배부름
        Thirst      = 1 << 2, //목마름
        Tired       = 1 << 3, //피로함
        Overweight  = 1 << 4, //중량초과
        Bleeding    = 1 << 5, //출혈
        Injury      = 1 << 6, //부상
        Sick        = 1 << 7, //아픔
        Exhausted   = 1 << 8, //지침
        Death       = 1 << 9, //사망
        Zombie      = 1 << 10,//좀비화

        All = int.MaxValue
    }

    public enum InvenType
    {
        Player, World
    }

    public static string GetDateNow()
    {
        return DateTime.Now.ToString(("yyyy-MM-dd HH:mm:ss tt"));
    }

    public enum BGM
    {
        Ambient, Chase1, Chase2, Death, FindZombie, Intro
    }

    public enum EffectSound
    {
        Bandage, Click, CloseBag, CloseDoor, DrinkWater, Eating, FireRifle, Mount, OpenBag, OpenDoor, OpenStorage,
        PutInBag, Reload, TakeMedicine, Ticking
    }

    public enum TriggerSound
    {
        Death, Save, Stab1, Stab2, Stab3, FHeavyBreath, MHeavyBreath, Heartbeat
    }

}
