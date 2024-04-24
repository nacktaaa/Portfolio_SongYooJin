using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public enum ItemType
{
    Food, 
    Weapon, 
    Cure,
    Tool
}
public enum WeaponType
{
    Rifle, Melee
}
public enum CureType
{
    Bleeding, Injury, Sick
}
public class Item
{
    public int idx;                    
    public string name;                 
    
    public ItemType itemType;           
    public float weight;                 
    public string description;
    public string icon;
    public string action;
    public Sprite iconImage;
    public virtual void Action() {}
}

[Serializable] 
public class Food : Item
{
    public float hungerChange;
    public float thirstChange;
    public bool CanEat;
    public bool IsCannedFood;
    public int replaceOnOpen;

    public override void Action()
    {
        if(!IsCannedFood)
        {
            //Debug.Log("Food 아이템 사용");
            Managers.Game.player.stat.Hunger -= hungerChange;
            Managers.Game.player.stat.Thirst -= thirstChange;
            if(!CanEat)
            {
                float rnd = UnityEngine.Random.Range(0f, 100f);
                if(rnd < Managers.Game.player.stat.CurSickRate)
                    Managers.Game.player.stat.SickPoints ++;
            }
            Managers.Sound.PlayEffectSound(Define.EffectSound.Eating);
            Managers.Inven.RemoveItem(this);
        }
    }
}

[Serializable]
public class Weapon : Item
{
    public float atk;
    public float atkSpeed;
    public WeaponType weaponType;
    public string prefab;
    public bool isEquiped = false;

    public Weapon CreateWeapon()
    {
        Weapon newWeapon = new Weapon();
        newWeapon.idx = idx;
        newWeapon.name = name;
        newWeapon.itemType = itemType;
        newWeapon.weight = weight;
        newWeapon.description = description;
        newWeapon.icon = icon;
        newWeapon.action = action;
        newWeapon.iconImage = iconImage;
        newWeapon.atk = atk;
        newWeapon.atkSpeed = atkSpeed;
        newWeapon.weaponType = weaponType;
        newWeapon.prefab = prefab;
        newWeapon.isEquiped = isEquiped;

        return newWeapon;
        
    }

    public override void Action()
    {
        if(isEquiped)
        {
            Managers.Inven.TakeOffItem();
        }
        else
        {
            if(Managers.Inven.equipedItem != null)
                Managers.Inven.TakeOffItem();
            Managers.Inven.EquipItem(this);
        }
    }   
}
[Serializable]
public class Cure : Item
{
    public CureType cureType;
    public float injuryChange;
    public float sickChange;
    public override void Action()
    {
        //Debug.Log("Cure 아이템 사용");
        switch(cureType)
        {
            case CureType.Bleeding :
                Managers.Game.flag.RemoveFlag(Define.FlagType.Bleeding);
                Managers.Sound.PlayEffectSound(Define.EffectSound.Bandage);
                break;
            case CureType.Sick :
                Managers.Game.flag.RemoveFlag(Define.FlagType.Sick);
                Managers.Sound.PlayEffectSound(Define.EffectSound.TakeMedicine);
                break;
            case CureType.Injury :
                Managers.Game.flag.RemoveFlag(Define.FlagType.Injury);
                Managers.Sound.PlayEffectSound(Define.EffectSound.TakeMedicine);
                break;
        }
        
        Managers.Inven.RemoveItem(this);
    }
}
[Serializable]
public class Tools : Item
{
    public override void Action()
    {

    }
}


[Serializable]
public class ItemData 
{
    public List<Food> foods = new List<Food>();
    public List<Weapon> weapons = new List<Weapon>();
    public List<Cure> cures = new List<Cure>();
    public List<Tools> tools = new List<Tools>();

    public void Init()
    {
        foreach (Food food in foods)
        {
            food.iconImage = Resources.Load<Sprite>($"Images/Icons/{food.icon}");
        }  
        foreach (Weapon weapon in weapons)
        {
            weapon.iconImage = Resources.Load<Sprite>($"Images/Icons/{weapon.icon}");
        }
        foreach (Cure cure in cures)
        {
            cure.iconImage = Resources.Load<Sprite>($"Images/Icons/{cure.icon}");
        }
        foreach (Tools tool in tools)
        {
            tool.iconImage = Resources.Load<Sprite>($"Images/Icons/{tool.icon}");
        }
    }

    public Dictionary<int, T> MakeDict<T>(List<T> list) where T : Item
    {
        Dictionary<int, T> dict = new Dictionary<int, T>();
        foreach(T item in list)
        {
            dict.Add(item.idx, item);
        }
        return dict;
    }
    
}
