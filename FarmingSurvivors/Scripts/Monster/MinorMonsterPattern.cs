using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Timeline;
using UnityEngine.Events;

public class MinorMonsterPattern 
{
    protected MonsterStatus status;
    protected Transform myPos;
    public MinorMonsterPattern(MonsterStatus status, Transform myPos)
    {
        this.status = status;
        this.myPos = myPos;
    }

    public virtual void Attack(Vector3 attackDir) {}
}

public class Summon : MinorMonsterPattern
{
    GameObject subMonsterPrefab;
    MonsterStatus subMonsterStatus;
    List<GameObject> effects = new List<GameObject>();
    Transform target;
    public Summon(MonsterStatus status, Transform myPos, Transform target) : base(status, myPos)
    {
        this.target = target;
        subMonsterPrefab = DataManager.Instance.minorMonsterPrefab;
        subMonsterStatus = status.monsterTosummon;
        GameObject go = Resources.Load<GameObject>($"Prefabs/Effects/SummonEffect");
        for(int i = 0; i < status.summonAmount; i ++)
        {
            GameObject newEffect = GameObject.Instantiate(go);
            effects.Add(newEffect);
        }    
    }

    public override void Attack(Vector3 attackDir)
    {
        if(subMonsterStatus != null)
        {
            Vector2 summonPos = status.isTargeting ? target.position : myPos.position;

            for(int i = 0; i < status.summonAmount; i++)
            {
                Vector2 dir = new Vector2 (Mathf.Cos(360 * i/status.projectileAmount), Mathf.Sin(360 * i/status.projectileAmount));
                summonPos += dir.normalized * status.summonDist;
                if(status.isRandomPos)
                    summonPos += new Vector2(Random.Range(-3f, 3f), Random.Range(-4f, 4f));
                
                SummonSubMonster(summonPos);
                effects[i].transform.position = summonPos;
                effects[i].GetComponent<Animator>().Play("Summon");
            }

        }
    }
    private void SummonSubMonster(Vector2 summonPos)
    {
        GameObject go = GetSubMonster();
        go.GetComponent<MinorMonster>().SubMonsterInit(subMonsterStatus, target, myPos.GetComponent<Monster>());
        go.SetActive(true);
        go.transform.DOShakePosition(0.5f, 0.2f, 3, 30);
    }

    private GameObject GetSubMonster()
    {
        GameObject go = null;
        go = PoolManager.Instance.Pop(subMonsterPrefab).gameObject;
        return go;
    }
}
public class Jump : MinorMonsterPattern
{
    Transform target;
    GameObject marker;

    public Jump(MonsterStatus status, Transform myPos, Transform target) : base(status, myPos)
    {
        this.target = target;
        if(marker == null)
        {
            GameObject go = Resources.Load<GameObject>("Prefabs/Effects/Marker");
            marker = GameObject.Instantiate(go, myPos);
            marker.SetActive(false);
        }
    }

    public override void Attack(Vector3 attackDir)
    {
        Vector3 dest = 
            status.isTargeting ? target.position : target.position + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0);
        
        marker.transform.SetParent(null);
        marker.transform.position = dest;
        marker.SetActive(true);
        
        var sequence = DOTween.Sequence();
        sequence.Append(marker.GetComponent<SpriteRenderer>().DOFade(0.2f, 0.5f));
        sequence.Append(marker.GetComponent<SpriteRenderer>().DOFade(1f, 0.5f));
        sequence.SetLoops(3, LoopType.Yoyo);

        myPos.GetComponent<Collider2D>().enabled = false;
        myPos.DOJump(dest, status.jumpPower, 1, status.jumpDuration).SetEase(status.curve).OnComplete(() => 
        {
            marker.transform.SetParent(myPos);
            marker.SetActive(false);
            myPos.GetComponent<Collider2D>().enabled = true;
        });
    }
}
public class Melee : MinorMonsterPattern
{
    GameObject effect;
    Animator effectAnim;
    float attackDelayCount = 0;

    public Melee(MonsterStatus status, Transform myPos) : base(status, myPos)
    {
        if(effect == null)
            InitEffect();
    }

    public override void Attack(Vector3 attackDir)
    {
        attackDelayCount += Time.deltaTime;
        if (attackDelayCount < status.attackDelay )
            return;

        effect.transform.DOKill();
        Collider2D hit = Physics2D.OverlapCircle(myPos.position, status.attackDist, 1<<6);

        if(status.isTargeting)
        {
            effect.transform.DOMove(myPos.position + attackDir * status.attackDist, 0f);

            // 이펙트 앵글 전환
            float angle = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;
            effect.transform.rotation = Quaternion.Euler(0, 0, angle);
            
            if(hit != null)
            {
                Vector3 inverV = hit.transform.position - myPos.position;
                float dot = Vector2.Dot(inverV.normalized, attackDir.normalized);
                float degree = Mathf.Acos(dot) * Mathf.Rad2Deg;

                if(degree <= 10)
                {
                    hit.GetComponent<PlayerController>().OnDamaged(status.damage);
                    attackDelayCount = 0;
                }
            }
        }
        else
        {
            effect.transform.DOScale(status.attackDist * 6, 0);

            if(hit != null)
            {
                Debug.Log("PlayerHIT");
                hit.GetComponent<PlayerController>().OnDamaged(status.damage);
                attackDelayCount = 0;
            }
        }

        effectAnim.Play("MeleeEffect");
    }

    private void InitEffect()
    {
        GameObject go  = Resources.Load<GameObject>($"Prefabs/Effects/{status.effectName}");
        effect = GameObject.Instantiate(go, myPos);
        effectAnim = effect.GetComponent<Animator>();
    }

}
public class FireProjectileLine : MinorMonsterPattern
{
    public FireProjectileLine(MonsterStatus status, Transform myPos) : base(status, myPos)
    {
    }

    public override void Attack(Vector3 attackDir)
    {
        if (status.projectile != null)
        {
            if(status.projectileAmount == 1)
            {
                FireProjectile(attackDir);
                return;
            }
            // 일단 플레이어 방향 기준
            // 방식 1 : 플레이어 방향과 상관없이 둥그렇게 쏜다 360 / projectileAmount
            // 방식 2 : 타게팅일 경우 플레이어 방향을 기준으로 +- 일정각도 에서 발사 
            if(status.isTargeting)
            {
                Vector2 rot = new Vector2(Mathf.Cos(5f), Mathf.Cos(5f));
                for(int i = -status.projectileAmount/2; i <= status.projectileAmount/2; i ++)
                {
                    if(i == status.projectileAmount/2 && status.projectileAmount % 2 == 0)
                        continue;

                    Vector3 addVec = attackDir + (Vector3)rot * i;
                    FireProjectile(addVec.normalized);
                }
            }
            else
            {
                for(int i = 0; i < status.projectileAmount; i ++)
                {
                    attackDir = new Vector2 (Mathf.Cos(Mathf.PI * 2 * i/status.projectileAmount), 
                                        Mathf.Sin(Mathf.PI * 2 * i/status.projectileAmount));
                    FireProjectile(attackDir.normalized);
                }
            }
        }
    }
    private void FireProjectile(Vector3 attackDir)
    {
        GameObject go = GetProjectile();
        Projectile_Monster proj = go.GetComponent<Projectile_Monster>();
        proj.Init(status, attackDir,myPos.position);
        proj.Shoot();
    }    
    protected GameObject GetProjectile()
    {
        var projectile = ProjectileManager.Instance.GetObjectWithName(status.projectile.name);
        if (projectile == null)
        {
            projectile = GameObject.Instantiate(status.projectile, myPos.position, Quaternion.identity);
            ProjectileManager.Instance.GetListWithName(status.projectile.name).Add(projectile);
        }
        projectile.SetActive(true);
        return projectile;
    }
}
public class FireProjectileCurved : MinorMonsterPattern
{
    Transform targetPos;
    public FireProjectileCurved(MonsterStatus status, Transform myPos, Transform targetPos) : base(status, myPos)
    {
        this.targetPos = targetPos;
    }

    public override void Attack(Vector3 attackDir)
    {
        if (status.projectile != null)
        {
            Vector3 destVec = status.isTargeting ?  
                targetPos.position : targetPos.position + new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), 0);
            
            if(status.projectileAmount == 1)
            {
                FireProjectile(destVec);
                return;
            }
            if(status.isTargeting)
            {
                Vector2 rot = new Vector2(Mathf.Cos(2f), Mathf.Cos(2f));
                for(int i = -status.projectileAmount/2; i <= status.projectileAmount/2; i ++)
                {
                    if(i == status.projectileAmount/2 && status.projectileAmount % 2 == 0)
                        continue;

                    Vector3 newDest = destVec + (Vector3)rot/status.projectileAmount * i;
                    FireProjectile(newDest);
                }
            }
            else
            {
                for(int i = 0; i < status.projectileAmount; i ++)
                {
                    Vector3 newDest = myPos.position + new Vector3 (Mathf.Cos(Mathf.PI * 2 * i/status.projectileAmount), Mathf.Sin(Mathf.PI * 2 * i/status.projectileAmount), 0);
                    FireProjectile(newDest);
                }
            }
        }
    }

    private void FireProjectile(Vector3 dest)
    {
        GameObject go = GetProjectile();
        Projectile_Monster proj = go.GetComponent<Projectile_Monster>();
        proj.Init(status, dest, myPos.position);
        proj.Shoot();
    }
    protected GameObject GetProjectile()
    {
        var projectile = ProjectileManager.Instance.GetObjectWithName(status.projectile.name);
        if (projectile == null)
        {
            projectile = GameObject.Instantiate(status.projectile, myPos.position, Quaternion.identity);
            ProjectileManager.Instance.GetListWithName(status.projectile.name).Add(projectile);
        }
        projectile.SetActive(true);
        return projectile;
    }
}