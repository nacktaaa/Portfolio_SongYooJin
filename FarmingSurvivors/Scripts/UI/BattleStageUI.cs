using System.Collections;
using System.Collections.Generic;
using StageMap;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleStageUI : MonoBehaviour
{
    public TextMeshProUGUI monsterCountText;

    // 전투 진행 중
    public void SetMonsterCountText(int count)
    {
        if(monsterCountText != null)
            monsterCountText.text = count.ToString();
    }
    
}
