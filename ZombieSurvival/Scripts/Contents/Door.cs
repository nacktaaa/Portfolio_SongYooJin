using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Door : MonoBehaviour, IDetectable
{
    public bool isOpen = false;
    public int hp;
    public int maxHp = 100;
    protected Animator anim;
    protected Quaternion originRot;
    bool isVisible = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        originRot = transform.rotation;
        hp = maxHp;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(isVisible)
        {
            if(!isOpen)
            {
                //Debug.Log("문 열기");
                StartCoroutine(DoorOpen());
                isOpen = true;
            }
            else
            {
                //Debug.Log("문 닫기");
                StartCoroutine(DoorClose());
                isOpen = false;
            }
        }
    }

    IEnumerator DoorOpen()
    {
        Managers.Sound.PlayEffectSound(Define.EffectSound.OpenDoor);
        float count = 0;
        while(count < 0.5f)
        {
            count += Time.deltaTime;
            transform.Rotate(0,0,90f*Time.deltaTime*2f);
            yield return null;
        }
    }

    IEnumerator DoorClose()
    {
        Managers.Sound.PlayEffectSound(Define.EffectSound.CloseDoor);
        float count = 0;
        while(count < 0.5f)
        {
            count += Time.deltaTime;
            transform.Rotate(0,0, -90f*Time.deltaTime*2f);
            yield return null;
        }
        transform.rotation = originRot;
    }
    
    public bool OnDamaged()
    {
        bool isOver = false;
        hp -= 5;
        if(hp <= 0)
        {
            this.gameObject.SetActive(false);
            return isOver = true;
        }
        
        return isOver;
    }

    public void OnVisible(bool _on)
    {
        isVisible = _on;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }
}
