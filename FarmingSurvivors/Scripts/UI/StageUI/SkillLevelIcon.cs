using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SkillLevelIcon : MonoBehaviour
{
    public Image[] stars;
    public Sprite redStar;
    public Sprite whiteStar;

    public void SetSkillLevelIcons(int level)
    {
        level = level == 0 ? 1 : level;
        
        for(int i = 0; i < stars.Length; i++)
        {
            if ( i < level)
                stars[i].sprite = redStar;
            else
                stars[i].sprite = whiteStar;
        }        
    }
}
