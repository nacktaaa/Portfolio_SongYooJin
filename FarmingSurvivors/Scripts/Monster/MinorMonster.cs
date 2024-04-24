using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MinorMonster : Monster
{
    MinorMonsterPattern pattern;
    
    protected override void Update()
    {
        base.Update();

        if(isSubMonster)
        {
            if(mother != null && !mother.gameObject.activeSelf)
                this.gameObject.SetActive(false);
        }

        if(curStage == StageMap.StageType.Boss)
            return;
            
        float dist = Vector3.Distance(target.position, transform.position);
        if(dist > 13f)
        {
            Vector3 randomPos = new Vector3 (UnityEngine.Random.Range(-2.5f, 2.5f), UnityEngine.Random.Range(-2.5f, 2.5f), 0f);
            transform.position = target.position + (Vector3)JoyStickInput.GetInputDir() * 7 + randomPos; 
        }
    }
    protected override void Action()
    {
        if(status.patternType == PatternType.None)
            return;

        if(!String.IsNullOrEmpty(status.animState))
            anim.Play(status.animState);

        RunPattern(status.patternType);
    }
    protected override void OnDie()
    {
        base.OnDie();
    
        if(pattern != null)
            pattern = null;
    }
    
    private void RunPattern(PatternType patternType)
    {
        if(pattern == null && patternType != PatternType.None)
            CreatePattern(patternType);
            
        if(pattern != null)
        {
            Vector2 dir = (target.position - transform.position).normalized;
            if(status.isContinuous)
            {
                StartCoroutine(ContinousAttack(dir));
            }
            else
            {
                pattern.Attack(dir);
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, status.triggerRange);
    }
    private void CreatePattern(PatternType patternType)
    {
        switch(patternType)
        {
            case PatternType.None :
            default :
                break;
            case PatternType.Summon :
                pattern = new Summon(status, transform, target);
                break;
            case PatternType.FireProjectileLine :
                pattern = new FireProjectileLine(status, transform);
                break;
            case PatternType.FireProjectileCurved :
                pattern = new FireProjectileCurved(status, transform, target);
                break;
            case PatternType.Melee :
                pattern = new Melee(status, transform);
                break;
            case PatternType.Jump :
                pattern = new Jump(status, transform, target);
                break;
        }
    }
    IEnumerator ContinousAttack(Vector3 dir)
    {
        for(int i = 0; i < status.continousCount; i ++)
        {
            pattern.Attack(dir);
            yield return new WaitForSeconds(status.continuousInterval);
        }
    }

}
