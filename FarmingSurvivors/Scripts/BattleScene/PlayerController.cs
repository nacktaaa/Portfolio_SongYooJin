using UnityEngine;
using UnityEngine.UI;
using System.Linq.Expressions;
using System.Collections.Generic;
using Util;
using System.Collections;
using Equipment;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    Character c;
    public bool isDead = false;
    public PlayerStatus status;
    public Slider slider;
    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer spr;
    Vector2 moveDir;
    bool isInvincibility = false; // 무적인가?
    public Transform Celling, Bottom, LeftSideUp,LeftSide,LeftSideDown, RightSideUp, RightSide, RightSideDown;
    public List<Weapon.Weapon> currentWeapons;
    public List<Transform> respawnPoints = new List<Transform>();
    WaitForSeconds invincibleDelay = new WaitForSeconds(0.25f);
    private readonly int hashIdle = Animator.StringToHash("IdleTree");

    private void Awake()
    {
        moveDir = new Vector2(0, -1);

        Transform[] childs = gameObject.GetComponentsInChildren<Transform>();
        foreach(Transform child in childs)
        {
            if (child.CompareTag("Respawn"))
                respawnPoints.Add(child);
        }

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();
        status = GetComponent<PlayerStatus>();
        status.Init(EquipManager.Instance.inventory.finalMountStats,c);
        slider.value = status.GetHPbarValue();
        var firepoints = transform.Find("FirePoints");
        if (Celling == null) Celling = firepoints.transform.Find("Celling").transform;
        if (LeftSide == null) LeftSide = firepoints.Find("LeftSide").transform;
        if (LeftSideUp == null) LeftSideUp = firepoints.Find("LeftSideUp").transform;
        if (LeftSideDown == null) LeftSideDown = firepoints.Find("LeftSideDown").transform;
        if (RightSide == null) RightSide = firepoints.Find("RightSide").transform;
        if (RightSideUp == null) RightSideUp = firepoints.Find("RightSideUp").transform;
        if (RightSideDown == null) RightSideDown = firepoints.Find("RightSideDown").transform;
        if (Bottom == null) Bottom = firepoints.Find("Bottom").transform;
        currentWeapons = new List<Weapon.Weapon>();
        foreach(var w in GetComponentsInChildren<Weapon.Weapon>()){
            currentWeapons.Add(w);
        }

    }

    private void Update()
    {
        if (!isDead)
            Move();
    }

    private void LateUpdate()
    {
        if (!isDead)
            AnimateMove();
    }

    private void Move()
    {
        if (JoyStickInput.GetInputDir() != Vector2.zero)
        {
            moveDir = new Vector2(JoyStickInput.GetInputX(), JoyStickInput.GetInputY());
            gameObject.transform.position += new Vector3(moveDir.x, moveDir.y,0) * status.MoveSpeed * Time.deltaTime;
            rb.velocity = Vector2.zero;
        }
    }
    
    private void AnimateMove()
    {
        if (JoyStickInput.GetInputDir() == Vector2.zero)
        {
            anim.Play(hashIdle);
            return;
        }

        if (Mathf.Abs(moveDir.x) > Mathf.Abs(moveDir.y))
        {
            if (moveDir.x > 0)
            {
                anim.Play("Right");
                anim.SetFloat("x", 1);
                anim.SetFloat("y", 0);
            }
            else if (moveDir.x < 0)
            {
                anim.Play("Left");
                anim.SetFloat("x", -1);
                anim.SetFloat("y", 0);
            }
        }
        else if (Mathf.Abs(moveDir.x) < Mathf.Abs(moveDir.y))
        {
            if (moveDir.y > 0)
            {
                anim.Play("Up");
                anim.SetFloat("x", 0);
                anim.SetFloat("y", 1);
            }
            else if (moveDir.y < 0)
            {
                anim.Play("Down");
                anim.SetFloat("x", 0);
                anim.SetFloat("y", -1);
            }
        }
    }


    public void OnDamaged(float damage)
    {
        if(isInvincibility)
            return;

        status.HP -= Mathf.Max(1,damage - status.defence);
        SetHPBar();
    }
    public void SetHPBar()
    {
        slider.value = status.GetHPbarValue();
    }

    public void OnDie()
    {
        isDead = true;
        anim.Play("Die");
        SetHPBar();
        GameManager.Instance.GameOver();
    }

    public void AddEXP(float exp)
    {
        status.EXP += exp*status.expMultiplier;
    }

    public Vector2 GetMoveDir()
    {
        return moveDir.normalized;
    }

    public Vector3 GetRespawnPoint()
    {
        
        return GetRandom.PickRandom(respawnPoints).position;
    }

    public Weapon.Weapon GetCurWeapon(int id)
    {
        foreach(var w in currentWeapons)
        {
            if(w.stat.id == id)
                return w;
        }

        return null;
    }

    public void Revival()
    {
        isDead = false;
        status.HP = status.maxHp;
        SetHPBar();
        anim.Play(hashIdle);
        StartCoroutine(InvincibleCotoutine());
    }

    IEnumerator InvincibleCotoutine()
    {
        isInvincibility = true;
        yield return invincibleDelay;
        spr.color = new Color (1,1,1,0.3f);
        yield return invincibleDelay;
        spr.color = Color.white;
        yield return invincibleDelay;
        spr.color = new Color (1,1,1,0.3f);
        yield return invincibleDelay;
        spr.color = Color.white;
        yield return invincibleDelay;
        spr.color = new Color (1,1,1,0.3f);
        yield return invincibleDelay;
        spr.color = Color.white;
        yield return invincibleDelay;
        spr.color = new Color (1,1,1,0.3f);
        yield return invincibleDelay;
        spr.color = Color.white;
        yield return invincibleDelay;
        spr.color = new Color (1,1,1,0.3f);
        yield return invincibleDelay;
        spr.color = Color.white;

        isInvincibility = false;
    }
    
}
