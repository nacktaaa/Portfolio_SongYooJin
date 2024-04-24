using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;

public class RunAwayMonster : Monster
{
    protected override void Update()
    {
        if(!isDead)
        {
            CheckCoolTime();
            RunAway();

            float dist = Vector3.Distance(target.position, transform.position);
            if(dist > 15)
            {
                Vector3 dir = target.GetComponent<PlayerController>().GetMoveDir();
                transform.position = new Vector3(Random.Range(-3f, 3f), target.position.y + dir.y * Random.Range(6f, 8f), 0);
            }
        }
    }

    protected override void FlipSprite()
    {
        spr.flipX = target.position.x > transform.position.x ? true : false;
    }

    private void RunAway()
    {
        Vector3 dir = (transform.position - target.position).normalized;
        dir.x = Random.Range(-0.8f, 0.8f);
        if(Mathf.Abs(dir.y) < Mathf.Abs(dir.x))
            dir.y *= 1.2f;
        transform.position += dir.normalized * status.moveSpeed * Time.deltaTime;
    }

    protected override void OnDie()
    {
        isDead = true;
        Poolable poolable = gameObject.GetOrAddComponent<Poolable>();
        PoolManager.Instance.Push(poolable);
        
        if(GameManager.Instance.goldStage.gameObject.activeSelf)
            GameManager.Instance.goldStage.killCount ++;
    }

}
