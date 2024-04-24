using System;
using UnityEngine;
using UnityEngine.UI;

public class SaveFilebutton : MonoBehaviour
{
    public string fileName;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if(String.IsNullOrEmpty(fileName))
            return;

        transform.root.GetComponent<MenuManager>().popup.SetActive(true);
        Managers.saveDataName = fileName;
    }
}
