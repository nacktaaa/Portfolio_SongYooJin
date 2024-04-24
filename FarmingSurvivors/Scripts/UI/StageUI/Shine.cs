using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shine : MonoBehaviour
{
    public Animator bling;

    public void Bling()
    {
        this.GetComponent<Image>().color = new Color(1,1,1,0);
        bling.Play("shineEffect2");
    }
}
