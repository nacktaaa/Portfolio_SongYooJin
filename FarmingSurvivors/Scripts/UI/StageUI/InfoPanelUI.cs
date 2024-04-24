using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoPanelUI : MonoBehaviour
{
    public StoreSlot[] slots = new StoreSlot[6];

    public void ShowInfoUI()
    {
        SetSkillSlots(); 
        GameManager.Instance.Pause();
    }
    public void SetSkillSlots()
    {
        foreach(var slot in slots)
        {
            if(slot.isEmpty)
                slot.ClearSlot();
        }
        for(int i = 0; i < GameManager.Instance.player.currentWeapons.Count; i++)
            slots[i].SetSlot(GameManager.Instance.player.currentWeapons[i]);
    }

    public void ResumeButton()
    {
        GameManager.Instance.Resume();
        this.gameObject.SetActive(false);
    }
}
