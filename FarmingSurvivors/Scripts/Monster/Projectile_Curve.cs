using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Projectile_Curve : Projectile_Monster
{
    float timeCount = 0;
    public override void Shoot()
    {
        timeCount = 0;
        GetComponent<Collider2D>().enabled = false;
        
        transform.DOJump(dir, 1f, 1, 2f).SetEase(Ease.OutSine).OnUpdate(() => 
        {
            timeCount += Time.deltaTime;
            if(timeCount > 1f)
                GetComponent<Collider2D>().enabled = true;
        }).OnComplete(()=> DestroyThis());
    }

}
