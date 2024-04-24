using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Outline))]
public class SaveResource : MonoBehaviour, IDetectable
{
    public bool isVisible = true;
    public GameObject saveCanvas;
    Button saveButton;
    Outline _outline;


    private void Awake()
    {
        _outline = GetComponent<Outline>();
        _outline.enabled = false;
        saveButton = GetComponentInChildren<Button>();
        saveButton.gameObject.SetActive(false);
        saveButton.onClick.AddListener(SaveButtonClick);
        saveCanvas.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 마시기 메뉴 팝업
        if(isVisible)
        {
            Vector3 dir = transform.position - Camera.main.transform.position;
            saveButton.transform.rotation = Quaternion.LookRotation(dir);
            Managers.UI.curActionButton = saveButton.gameObject;
            saveButton.gameObject.SetActive(true);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(isVisible)
            _outline.enabled = true;
   }

    public void OnPointerExit(PointerEventData eventData)
    {
        _outline.enabled = false;
    }
    public void OnVisible(bool _on)
    {
        isVisible = _on;
    }

    public void SaveButtonClick()
    {
        StartCoroutine(SaveGame());
    }
    
    IEnumerator SaveGame()
    {
        Managers.Sound.PlayTriggerMusic(Define.TriggerSound.Save);
        Managers.Data.SaveData(Managers.Game.player.name, Define.GetDateNow());
        saveCanvas.SetActive(true);
        yield return new WaitForSeconds(3f);
        saveCanvas.SetActive(false);
    }

}
