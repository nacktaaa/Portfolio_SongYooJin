using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Linq;

public class SkillSelectSlot : MonoBehaviour
{
    public Image skillIcon;
    public TextMeshProUGUI skillDescription, skillName;
    public Weapon.Weapon skill;
    public GameObject star, starParent;
    public List<ToggleImage> stars;

    public void Init(){
        
        stars = GetComponentsInChildren<ToggleImage>().ToList();
        SetSkillSlot();
        SetStars();
    }
    public void Init(Weapon.Weapon weapon)
    {
        if (weapon == null) gameObject.SetActive(false);
        skill = weapon;
        Init();
        
    }
    public void SetSkillNull()
    {
        this.skill = null;
        skillDescription.text = "골드 획득";
        
    }
    public void SetSkillSlot()
    {
        skillIcon.sprite = skill.stat.icon;
        skillDescription.text = skill.getLevelUpText();
        skillName.text = skill.stat.weaponName;
    }
    public void SetStars(){
        int maxLevel = skill.stat.maxLevel, starCount = stars.Count;
        if (starCount < maxLevel){

            for (int i = 0; i <maxLevel - starCount; i++){
                var go = Instantiate(star,starParent.transform);
                stars.Add(go.GetComponent<ToggleImage>());
            }
            starCount = stars.Count;
        }
        for (int i = 0; i < skill.stat.currentLevel + 1; i++)
        {
            stars[i].gameObject.SetActive(true);
            stars[i].isActive = true;
        }
        for (int i = skill.stat.currentLevel + 1; i < maxLevel; i++)
        {
            stars[i].gameObject.SetActive(true);
            stars[i].isActive = false;
        }
        for (int i = maxLevel; i < starCount; i++){
            stars[i].gameObject.SetActive(false);
        }
    }
    public void SelectSkill()
    {
        LevelUpManager.Instance.SkillUp(skill);
        
    }
}
