using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CharacterSelectUI : UIPanel
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descripText;
    public string PyotrText;
    public string AlexText;
    public string IshaText;
    public void SetDescription(Character character)
    {
        switch (character)
        {
            case Character.Pyotr :
                nameText.text = "표트르";
                descripText.text = PyotrText.Replace("\\n", "\n");
                break;
            case Character.Alex :
                nameText.text = "알렉스";
                descripText.text = AlexText.Replace("\\n", "\n");
                break;
            case Character.Isha :
                nameText.text = "이샤";
                descripText.text = IshaText.Replace("\\n", "\n");
                break;
            default :
                nameText.text = "";
                descripText.text = "";
                break;
        }
    }
}
