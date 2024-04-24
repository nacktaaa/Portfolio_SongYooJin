using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;

public class InventoryManager 
{
    // 플레이어의 아이템을 관리하는 매니저
    // 월드 아이템과 플레이어 아이템의 데이터 관리만 담당 
    public Action<List<Item>> PlayerInvenEvent;
    public Action<List<Item>> StorageInvenEvent;

    public List<Item> playerItems = new List<Item>(); // 플레이어 소지 아이템 리스트
    public Storage selectedStorage; // 현재 활성화된 스토리지를 저장
    public Weapon equipedItem;
    public int maxMagazine = 5;
    public int curMagazine = 0;
    int ammoCount = 0;
    bool hasAmmo = false;

    public int AmmoCount
    {
        get { return ammoCount;}
        set
        {
            ammoCount = value;
            if(ammoCount == 0 && hasAmmo == true)
            {
                // 인벤토리에서 탄약 삭제
                RemoveItem(Managers.Data.ToolDict[20]);
                hasAmmo = false;
            }
        }    
    }

    public void SetStorage(Storage _storage)
    {
        selectedStorage = _storage;
    }

    public void ClearStorage()
    {
        selectedStorage = null;
    }

    public void AddItem(Item _item, Define.InvenType _type = Define.InvenType.Player)
    {
        switch(_type)
        {
            case Define.InvenType.Player :
            {
                if(_item.idx == 20)
                {
                    if(!hasAmmo)
                    {
                        playerItems.Add(_item);
                        hasAmmo = true;
                    }
                    ammoCount += 5;
                    
                    if(equipedItem?.weaponType == WeaponType.Rifle)
                        Managers.UI.RefreshMagazineUI();
                }
                else
                {
                    playerItems.Add(_item);
                }
                
                Managers.Game.player.stat.CurWeight += _item.weight;
                Managers.Sound.PlayEffectSound(Define.EffectSound.PutInBag);
                
                if(Managers.UI.IsInvenUIOpen())
                    PlayerInvenEvent?.Invoke(playerItems);
            }
                break;
            case Define.InvenType.World :
            {
                if(selectedStorage != null)
                {
                    if(selectedStorage.curWeight + _item.weight > selectedStorage.maxWeight)
                    {
                        //Debug.Log("용량이 가득 찼다.");
                        return;
                    }
                    selectedStorage.items.Add(_item);
                    selectedStorage.curWeight += _item.weight;
                    if(Managers.UI.IsInvenUIOpen(_type))
                        StorageInvenEvent?.Invoke(selectedStorage.items);
                }
            }
                break;    
        }
    }

    public void SwapBetweenInvens(Slot slotA, Slot slotB)
    {
        if(selectedStorage != null)
        {
            if(slotA.invenType == Define.InvenType.World)
            {
                selectedStorage.items.Remove(slotA.item);
                selectedStorage.items.Add(slotB.item);
                playerItems.Remove(slotB.item);
                playerItems.Add(slotA.item);
                Managers.Sound.PlayEffectSound(Define.EffectSound.PutInBag);
                PlayerInvenEvent.Invoke(playerItems);
                StorageInvenEvent.Invoke(selectedStorage.items);
                
            }
            else
            {
                selectedStorage.items.Remove(slotB.item);
                selectedStorage.items.Add(slotA.item);
                playerItems.Remove(slotA.item);
                playerItems.Add(slotB.item);
                PlayerInvenEvent.Invoke(playerItems);
                StorageInvenEvent.Invoke(selectedStorage.items);
            }
        }
    }

    public void RemoveItem(Item _item, Define.InvenType _type = Define.InvenType.Player)
    {
        switch(_type)
        {
            case Define.InvenType.Player :
            {
                if(playerItems.Contains(_item))
                {
                    playerItems.Remove(_item);
                    Managers.Game.player.stat.CurWeight -= _item.weight;
                    if(Managers.UI.IsInvenUIOpen())
                        PlayerInvenEvent?.Invoke(playerItems);
                }
            }
                break;
            case Define.InvenType.World :
            {
                if(selectedStorage != null && selectedStorage.items.Contains(_item))
                {
                    selectedStorage.items.Remove(_item);
                    selectedStorage.curWeight -= _item.weight;
                    if(Managers.UI.IsInvenUIOpen(_type))
                        StorageInvenEvent?.Invoke(selectedStorage.items);
                }
            }
                break;    
        }
    }

    public void ToPlayerInven(Item _item)
    {
        Managers.Inven.AddItem(_item);
        Managers.Inven.RemoveItem(_item, Define.InvenType.World);
    }
    
    public void EquipItem(Weapon _item)
    {
        equipedItem = _item;
        equipedItem.isEquiped = true;
        Managers.Game.player.MountWeapon(equipedItem);
        if(equipedItem.weaponType == WeaponType.Rifle)
        {
            Managers.Sound.PlayEffectSound(Define.EffectSound.Mount);
            Managers.UI.magazineCanvas.SetActive(true);
            Managers.UI.RefreshMagazineUI();
        }

        PlayerInvenEvent?.Invoke(playerItems);       
    }

    public void TakeOffItem()
    {
        equipedItem.isEquiped = false;
        equipedItem = null;
        Managers.Game.player.UnMountWeapon();
        if(Managers.UI.magazineCanvas.activeSelf)
            Managers.UI.magazineCanvas.SetActive(false);
        
        PlayerInvenEvent?.Invoke(playerItems);   
    }

    public void Clear()
    {
        equipedItem = null;
        selectedStorage = null;
        ammoCount = 0;
        curMagazine = 0;
        playerItems.Clear();
    }
   

}
