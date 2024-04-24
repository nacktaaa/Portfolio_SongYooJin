using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathCanvas : MonoBehaviour
{
    public Image deathPanel;
    public TextMeshProUGUI deathLog;

    private void OnEnable()
    {
        int survivalDay = (int)(Managers.Game.totalGameTime / 3600f);
        int survivalHour = (int)((Managers.Game.totalGameTime - (survivalDay * 3600f))/150f);
        deathLog.text = $"당신의 생존시간은 {survivalDay}일 {survivalHour}시간 입니다.\n생존하는 동안 {Managers.Game.killcount}마리의 좀비를 제거했습니다.";
        deathLog.text += $"\n (사망 원인 : {Managers.Game.causeOfDeath})";
    }
    
    public void RetryButton()
    {
        SceneManager.LoadScene(1);
    }

    public void ExitButton()
    {
        SceneManager.LoadScene(0);
    }

}
