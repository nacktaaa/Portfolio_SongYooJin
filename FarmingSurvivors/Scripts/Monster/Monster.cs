using DG.Tweening;
using StageMap;
using UnityEngine;

public class Monster : MonoBehaviour
{
#region 변수 선언
    public bool isDead = false;
    public MonsterStatus status;
    public GameObject dropItemPrefab;
    public DropItemType dropType;
    public Transform target;
    public bool isInPattern = false;

    protected Rigidbody2D rb;
    protected SpriteRenderer spr;
    protected CapsuleCollider2D col;
    protected Animator anim;
    protected float curhp;
    protected StageType curStage;
    protected bool isAttack = false;
    protected bool isStopMoving = false;
    protected float tempAttackCool = 0;
    protected bool isSubMonster = false;
    protected Monster mother; 
    protected bool isDamaged = false;

    bool isCollide = false;
    bool isFirstAttack = true;
    float collideCoolTime = 0.5f;
    float collideCoolCount = 0;
    float attackCoolCount = 0;
    float stopMoveCount= 0;
    float damagedCount= 0;
#endregion
    public virtual void Init(MonsterStatus status, Transform target, bool isSubMonster = false)
    {
        this.status = status;
        this.target = target;
        this.isSubMonster = isSubMonster;
        isDead = false;
        isAttack = true;
        isStopMoving = false;
        isCollide = false;
        isDamaged = false;
        isFirstAttack = true;
        curhp = status.hp;
        collideCoolCount = 0;
        attackCoolCount = 0;
        damagedCount = 0;
        stopMoveCount = 0;
        curStage = GameManager.Instance.curStage;

        spr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>();
        spr.sprite = status.sprite;
        anim.runtimeAnimatorController = status.animCtr;   
        tempAttackCool = status.baseCoolTime;

        col.direction = status.direction;
        col.size = this.status.colSize;
        transform.localScale = new Vector3(this.status.monsterSize,this.status.monsterSize,this.status.monsterSize);
    }
    public void SubMonsterInit(MonsterStatus subMonsterStatus, Transform target, Monster mother)
    
    {
        Init(subMonsterStatus, target, true);
        this.mother = mother;
    }
    public virtual void OnDamaged(Vector2 pos, float damage, bool isKnockBack = true, float knockBackDist = 0.5f)
    {
        if (!isDamaged)
        {
            isDamaged = true;
            curhp -= damage;
            
            spr.DOKill();
            spr.DOColor(Color.red, 0.1f).SetInverted().SetLoops(3);

            if (isKnockBack)
            {
                col.enabled = false;
                Vector3 kbPos = transform.position + (transform.position - target.position).normalized * knockBackDist;
                transform.DOKill();
                transform.DOMove(kbPos, 0.3f).SetEase(Ease.OutCirc).OnComplete(() => col.enabled = true);
            }

            if (curhp <= 0)
            {
                if (!isSubMonster)
                {
                    if(GameManager.Instance.battleStage.gameObject.activeSelf)
                    {
                        GameManager.Instance.battleStage.KillCount ++;
                    }
                    DropItem(dropType, pos);
                }
                Invoke("OnDie", 0.3f);
            }
        }
    }
    protected virtual void Update()
    {
        if(GameManager.Instance.isGameOver || GameManager.Instance.isClear || isDead)
            return;

        CheckCoolTime();
        
        if(target != null)
        {
            MoveToTarget();
            TryAttack(); // 공격 범위 안에 들어왔는 지 체크하고 공격
        }
    }
    protected virtual void FlipSprite()
    {
        if(status.flipX)
            spr.flipX = target.position.x < transform.position.x ? false : true;
        else
            spr.flipX = target.position.x < transform.position.x ? true : false;
    }
    protected void CheckCoolTime()
    {
        if(isCollide)
        {
            collideCoolCount += Time.deltaTime;
            if(collideCoolCount >= collideCoolTime)
            {
                isCollide = false;
                collideCoolCount = 0;
            }
        }

        if(isStopMoving)
        {
            stopMoveCount += Time.deltaTime;
            if(stopMoveCount >= 1.2f)
            {
                isStopMoving = false;
                stopMoveCount = 0;
            }
        }

        if(isAttack)
        {
            attackCoolCount += Time.deltaTime;
            if(attackCoolCount >= tempAttackCool)
            {
                isAttack = false;
                attackCoolCount = 0;
            }
        }

        if(isDamaged)
        {
            damagedCount += Time.deltaTime;
            if(damagedCount >= 0.4f)
            {
                isDamaged = false;
                damagedCount = 0;
            }
        }
    }
    protected virtual void OnDie() 
    {
        isDead = true;

        if (status.isAfterDeath)
            Action();
        
        PoolManager.Instance.Push(this.GetComponent<Poolable>());
    }
    protected virtual void DropItem(DropItemType type, Vector2 pos)
    {
        GameObject go = GetDropItem(type);
        go.transform.position = pos;
        int value = type == DropItemType.EXP ? (int)status.exp : 1;
        go.GetComponent<DropItem>().Init(type, value);
    }
    private void LateUpdate()
    {
        if(!isDead)
            FlipSprite();
    }
    private void MoveToTarget()
    {
        if (isInPattern) return;
        if(status.isStop)
        {
            if(isStopMoving)
                return;
        }
        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * status.moveSpeed * Time.deltaTime;
        rb.velocity = Vector2.zero;
    }
    private void TryAttack()
    {
        if (isAttack)
            return;
        if (status.isAfterDeath)
            return;

        // 공격 조건 체크
        switch(status.triggerType)
        {
            case TriggerType.CloseToPlayer :
            {
                if(isFirstAttack)
                {
                    isFirstAttack = false;
                    tempAttackCool = status.attackCoolTime;
                }
                
                float dist = Vector2.Distance(transform.position, target.position);
                if(dist <= status.triggerRange)
                {
                    isAttack = true;
                    isStopMoving = true;
                    Action();
                } 
            }
                break;
            case TriggerType.None :
            default :
                break;
        }
    }
    private void OnCollisionStay2D(Collision2D other)
    {
        if(other.transform.CompareTag("Player") && !isCollide)
        {
            isCollide = true;
            other.transform.GetComponent<PlayerController>().OnDamaged(status.damage);
        }
    }
    private GameObject GetDropItem(DropItemType type)
    {
        GameObject go = null;
        go = PoolManager.Instance.Pop(dropItemPrefab).gameObject;
        
        return go;
    }
    protected virtual void Action() { }

}