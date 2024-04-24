using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public enum DropItemType
{
    EXP, Gold
}
public class DropItem : MonoBehaviour
{
    public DropItemType dropItemType = DropItemType.EXP;
    public Sprite expIcon;
    public Sprite goldIcon;
    public float moveSpeed = 12f;
    bool isCollide = false;
    float expValue;
    int goldValue;

    Vector3 initScale = new Vector3 (0.18f,0.18f,0.18f);

    
    public void Init(DropItemType type, int value)
    {
        transform.localScale = initScale;
        isCollide = false;
        switch (type)
        {
            case DropItemType.EXP :
                dropItemType = DropItemType.EXP;
                GetComponent<SpriteRenderer>().sprite = expIcon;
                this.expValue = value;
                break;
            case DropItemType.Gold :
                dropItemType = DropItemType.Gold;
                GetComponent<SpriteRenderer>().sprite = goldIcon;
                transform.localScale *= 0.8f;
                this.goldValue = value;
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            if(isCollide)
                return;

            isCollide = true;    
            Vector3 movedir = (transform.position - other.transform.position).normalized;
            movedir += new Vector3 (0, 0.2f, 0);
            
            transform.DOMove(transform.position + movedir * 0.3f, 0.2f).OnComplete( () => {
                if (this.gameObject.activeInHierarchy)
                    StartCoroutine(FlyCoroutine(other.transform));
            });
        }
    }

    IEnumerator FlyCoroutine(Transform target)
    {
        float dist = 10;
        while(dist > 0.2f)
        {
            Vector3 dir = target.position - transform.position;
            transform.position += dir * moveSpeed * Time.deltaTime;
            dist = Vector3.Distance(target.position, transform.position);
            yield return null;
        }

        DoReward(target);
        Poolable poolable = gameObject.GetOrAddComponent<Poolable>();
        PoolManager.Instance.Push(poolable);
    }

    public void DoReward(Transform target)
    {
        switch(dropItemType)
        {
            case DropItemType.EXP :
                target.GetComponent<PlayerController>().AddEXP(expValue);
                break;
            case DropItemType.Gold :
                GameManager.Instance.goldStage.EarnedGold += goldValue + GameManager.Instance.curFloor;
                break;
        }
        SoundManager.Instance.PlayEffectSound(EffectSound.DropItem);
    }
}
