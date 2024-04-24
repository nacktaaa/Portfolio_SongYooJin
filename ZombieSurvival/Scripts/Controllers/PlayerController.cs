using System;
using System.Collections;
using System.Collections.Generic;
using FOW;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    FSM fsm;
    public PlayerStat stat;
    public Define.PlayerType playerType;
    public FogOfWarRevealer3D revealer;
    Animator anim;
    AudioSource audioSource;
    CapsuleCollider playerCol;
    GameObject aimObj = null;
    Camera cam;

    bool isCrouch = false;     // 웅크렸는가?
    [SerializeField]
    bool isAim = false;        // 조준 중 인가?
    public bool isRunningAttack = false;     // 공격 중 인가?
    public bool stopAttck = true;     // 공격을 멈추는가? 
    public bool isRunningBeaten = false;     // 물렸는가?
    public bool isEquip = false;

    // 조준 상태 관련 변수
    public GameObject AimObject;
    public GameObject BulletObject;    
    public GameObject RHandContainer;
    public MountedWeapon mountedWeapon;
    public Transform firePos;

    //Animator 파라미터의 해시값 추출 
    private readonly int hashMove = Animator.StringToHash("Move");
    private readonly int hashCrouch = Animator.StringToHash("Crouch");
    private readonly int hashAttack_normal = Animator.StringToHash("Attack_Normal");
    private readonly int hashAttack_Gun = Animator.StringToHash("Attack_Gun");
    private readonly int hashYelling = Animator.StringToHash("Yelling");
    private readonly int hashAim = Animator.StringToHash("Aim");
    private readonly int hashDie = Animator.StringToHash("Die");

    
    int moveMask = 1<<7 | 1<<3 | 1<<8 | 1<<11 | 1<<12; // Layer7 "Storage", Layer3 "Obstacle", Layer8 "Zombie", Layer11 "Wall", Layer12 "Door"
    int aimMask = 1<<6; // Layer6 Floor
    int detectMask = 1<<7 | 1 <<12; 

    WaitForSeconds attackDelay = new WaitForSeconds(2);

    private void OnEnable()
    {
        Managers.Input.MouseEvent_R -= OnMouseEvent_R;
        Managers.Input.MouseEvent_R += OnMouseEvent_R;

        Managers.Input.MouseEvent_L -= OnMouseEvent_L;
        Managers.Input.MouseEvent_L += OnMouseEvent_L;
    }    
    
    private void OnDisable()
    {
        Managers.Input.MouseEvent_R -= OnMouseEvent_R;
        Managers.Input.MouseEvent_L -= OnMouseEvent_L;
    }
    
    public void Init()
    {
        Init_FSM();
        cam = Camera.main;
        stat = GetComponent<PlayerStat>();
        revealer = GetComponentInChildren<FogOfWarRevealer3D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        playerCol = GetComponent<CapsuleCollider>();
        RHandContainer.GetComponent<SphereCollider>().enabled = false;
        revealer.ViewAngle = stat.GetViewAngle();
        AimObject = Resources.Load<GameObject>("Prefabs/AimObject");
        aimObj = Instantiate(AimObject);
        aimObj.SetActive(false);
    }

    public void Init_FSM()
    {
        fsm = new FSM();
        fsm.Add((int)Define.PlayerStateType.Normal, new PlayerState(this, Define.PlayerStateType.Normal));
        fsm.Add((int)Define.PlayerStateType.Attack, new PlayerState(this, Define.PlayerStateType.Attack));
        fsm.Add((int)Define.PlayerStateType.Beaten, new PlayerState(this, Define.PlayerStateType.Beaten));
        fsm.Add((int)Define.PlayerStateType.Death, new PlayerState(this, Define.PlayerStateType.Death));

        Init_FSM_Normal();
        Init_FSM_Attack();
        Init_FSM_Beaten(); 
        Init_FSM_Death();
        fsm.SetCurState((int)Define.PlayerStateType.Normal);
    }

    void Init_FSM_Normal()
    {
        PlayerState state = (PlayerState)fsm.GetState((int)Define.PlayerStateType.Normal);

        state.OnEnterDelegate = () => 
        {
            //Debug.Log("OnENTER - Normal");
        };
        state.OnExitDelegate = () =>
        {
            //Debug.Log("OnExit - Normal");
            anim.SetFloat(hashMove, 0);
        };
        state.OnUpdateDelegate = () =>
        {
            if(!isRunningAttack || !isRunningBeaten)
            {
                Move();
                TryCrouch();
                DetectObject();
            }
        };
    }

    void Init_FSM_Attack()
    {
        PlayerState state = (PlayerState)fsm.GetState((int)Define.PlayerStateType.Attack);

        state.OnEnterDelegate = () => 
        {
            //Debug.Log("OnENTER - Attack");
            if(!isRunningAttack)
                StartCoroutine(AttackCoroutine());
            
        };
        state.OnExitDelegate = () =>
        {
            //Debug.Log("OnExit - Attack");
            
        };
        state.OnUpdateDelegate = () =>
        {
                
        };
    }

    void Init_FSM_Beaten()
    {
        PlayerState state = (PlayerState)fsm.GetState((int)Define.PlayerStateType.Beaten);

        state.OnEnterDelegate = () => 
        {
            //Debug.Log("OnENTER - Beaten");
            anim.SetBool(hashYelling, true);

            if(Managers.Game.isSleep) // 잠자는 중이라면 깨어나는 처리
            {
                Time.timeScale = 1;
                Managers.Sound.PlayTriggerMusic(Define.TriggerSound.Stab3);
                Managers.UI.CloseSleepCanvas();
                Managers.Game.isSleep = false;
            }

            if(isAim) // 조준 중이라면 조준상태 해제
            {
                isAim = false; 
                isCrouch = false; 
                anim.SetBool(hashAim, false);
                aimObj.SetActive(false);
            }

            if(isRunningBeaten)
                StartCoroutine(BeatenCoroutine());

        };
        state.OnExitDelegate = () =>
        {
            //Debug.Log("OnExit - Beaten");
            anim.SetBool(hashYelling, false);
        };
        state.OnUpdateDelegate = () =>
        {
                
        };        
        
    }

    void Init_FSM_Death()
    {
        PlayerState state = (PlayerState)fsm.GetState((int)Define.PlayerStateType.Death);

        state.OnEnterDelegate = () => 
        {
            //Debug.Log("OnENTER - Death");
            Managers.Game.isGameOver = true;
            anim.SetTrigger(hashDie);
            transform.gameObject.layer = LayerMask.NameToLayer("DeadBody");
            transform.AddComponent<DeadBody>();
            StopAllCoroutines();
            StartCoroutine(GameOver());
        };
        state.OnExitDelegate = () =>
        {
            //Debug.Log("OnExit - Death");
        };
        state.OnUpdateDelegate = () =>
        {
            
        };        
    }

    public void SetState(Define.PlayerStateType type)
    {
        fsm.SetCurState((int)type);
    }

    public Define.PlayerStateType GetCurState()
    {
        return fsm.GetCurStatetype();
    }
    private void OnMouseEvent_L(Define.MouseEvent @event)
    {
        if(isAim && @event == Define.MouseEvent.LMBDown)
            fsm.SetCurState((int)Define.PlayerStateType.Attack);

        if (fsm.GetCurState() == fsm.GetState((int)Define.PlayerStateType.Attack) 
        && @event == Define.MouseEvent.LMBPress)
            stopAttck = false;

        if(!stopAttck && @event == Define.MouseEvent.LMBUp)
            stopAttck = true;        
    }

    private void OnMouseEvent_R(Define.MouseEvent @event) 
    {
        if(@event != Define.MouseEvent.RMBClick)
            Aiming(@event);
    }

    private void Update()
    {
        if(Managers.Game.isGameOver)
            return;
        
        if(Managers.Game.isPause)
        {
            anim.SetFloat(hashMove, 0);
            return;
        }

        fsm.Update();

    }

    IEnumerator BeatenCoroutine()
    {
        isRunningBeaten = true;
        stat.HP -= 5;
        int rand = UnityEngine.Random.Range(0, 100);
        if(rand < stat.CurInjuryRate)
            stat.InjuryPoints++;

        int infectinonRand = UnityEngine.Random.Range(0, 100);
        if(infectinonRand < stat.infectionRate)
            stat.isInfected = true;
            
        float randRate = UnityEngine.Random.Range(0f, 100f);

        if(!Managers.Game.flag.HasFlag(Define.FlagType.Bleeding))
        {
            if(randRate < stat.bleedingRate)
                Managers.Game.flag.AddFlag(Define.FlagType.Bleeding);
        }
        else
        {
            if(Managers.Game.flag.GetFlagStep(Define.FlagType.Bleeding) == 1 && randRate < stat.bleedingRate)
                Managers.Game.flag.ChangeFlagStep(Define.FlagType.Bleeding, isPlus: true);
            else if(Managers.Game.flag.GetFlagStep(Define.FlagType.Bleeding) == 2 && randRate < stat.bleedingRate)
                Managers.Game.durTime_Bleeding[1] += 300f;
        } 

        yield return new WaitForSeconds(1.2f);

        isRunningBeaten = false;
        fsm.SetCurState((int)Define.PlayerStateType.Normal);
    }

    IEnumerator AttackCoroutine()
    {
        isRunningAttack = true;
        if(!isEquip) // 무기가 없는 경우
        {
            anim.SetTrigger(hashAttack_normal);
            RHandContainer.GetComponent<SphereCollider>().enabled = true;
            yield return new WaitForSeconds(0.5f);
            RHandContainer.GetComponent<SphereCollider>().enabled = false;
            yield return new WaitForSeconds(0.8f);
        }
        else
        {
            switch (mountedWeapon.stat.weaponType)
            {
                case WeaponType.Melee :
                {
                    anim.SetTrigger(hashAttack_normal);
                    mountedWeapon.col.enabled = true;
                    yield return new WaitForSeconds(0.5f);
                    mountedWeapon.col.enabled = false;
                    yield return new WaitForSeconds(0.8f);
                }
                break;
                case WeaponType.Rifle :
                {
                    do
                    {
                        mountedWeapon.FireRifle(aimObj.transform.position);
                        anim.SetTrigger(hashAttack_Gun);
                        yield return new WaitForSeconds(1.2f);

                    }while(!stopAttck);
                }
                break;
            }
        }

        isRunningAttack = false;
        fsm.SetCurState((int)Define.PlayerStateType.Normal);
    }

    private void Move()
    {
        Vector3 dir = new Vector3(Managers.Input.Hinput, 0, Managers.Input.Vinput);
        dir = dir.normalized;

        if (dir != Vector3.zero && !isAim)
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), stat.GetRotateSpeed() * Time.deltaTime); 
        
        if(Physics.Raycast(transform.position + Vector3.up * 0.25f , dir, 0.8f, moveMask))
            return;
        
        transform.position += dir * stat.GetMoveSpeed(isCrouch, isAim) * Time.deltaTime;
        float animMove = dir.magnitude * stat.GetMoveSpeed()/stat.GetMaxSpeed();
        anim.SetFloat(hashMove, animMove, 0.25f, Time.deltaTime);
    }

    private void TryCrouch()
    {
        if(Managers.Input.Crouch && !isAim) // 조준 중이 아니고 C가 눌렸으면
        {
            if(isCrouch) // 이미 크라우치 중이라면, 다시 일어서기
            {
                isCrouch = false;
                anim.SetBool(hashCrouch, false);
                
                playerCol.center = new Vector3(0, 0.85f, 0); //원래 위치로 
                playerCol.height = 1.7f;

            }
            else // 웅크리기 
            {
                isCrouch = true;
                anim.SetBool(hashCrouch, true);

                playerCol.center = new Vector3(0, 0.76f, 0);
                playerCol.height = 1.5f;
            }
            
        }

    }
    
    private void Aiming(Define.MouseEvent @event) // 마우스 우클릭 하고 있는 방향으로 조준(시선 이동, 조준점 생성)
    {
        switch (@event)
        {
            case Define.MouseEvent.RMBDown :
            case Define.MouseEvent.RMBClick :
                return;
            case Define.MouseEvent.RMBPress: // 누르는 중
            {
                // 클릭 지점에서 출발하는 레이 생성
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if(Physics.Raycast(ray, out hit, 100f, aimMask))
                {
                    if(!isAim) // 조준 중이 아닐 경우(조준의 시작)
                    {
                        isAim = true;
                        anim.SetBool(hashCrouch, false);
                        anim.SetBool(hashAim, true);
                        aimObj.SetActive(true); // 조준점 생성
                        cam.GetComponent<PhysicsRaycaster>().enabled = false; // 우클릭 중 상호작용 방지
                    }
                    // 조준점 위치 설정
                    Vector3 hitPoint = new Vector3(hit.point.x, 0, hit.point.z); 
                    aimObj.transform.position = hitPoint;

                    // 마우스 위치 바라보도록 회전 
                    Vector3 dir = hitPoint - transform.position;
                    Quaternion rot = Quaternion.LookRotation(dir.normalized);
                    transform.rotation = Quaternion.Lerp(transform.rotation, rot, stat.GetRotateSpeed() * Time.deltaTime);
                }
            }
                break;
            case Define.MouseEvent.RMBUp : // 버튼 업 시 조준 해제
            {
                if(isAim)
                {
                    isAim = false; 
                    isCrouch = false; 
                    anim.SetBool(hashAim, false);
                    aimObj.SetActive(false);
                    cam.GetComponent<PhysicsRaycaster>().enabled = true;
                }
            }
                break;
        }
    }

    void DetectObject()
    {
        // 인풋에 변화가 있다면
        if(Managers.Input.Hinput != 0 && Managers.Input.Vinput != 0)
        {
            // OverlapSphere 로 감지 범위 내 오브젝트 배열로 가져오기 (detectable 레이어 만) 
            Collider[] detectables = Physics.OverlapSphere(transform.position, stat.DetectRange, detectMask);
            
            // 오브젝트 배열 순회
            foreach (Collider obj in detectables)
            {
                // 오브젝트까지의 방향 벡터 구하기
                Vector3 dir = obj.transform.position - transform.position; 
                dir = dir.normalized;

                IDetectable detectable = obj.GetComponent<IDetectable>();
                if(detectable == null)
                    continue;

                // 방향 벡터로 내적 구하기
                float dot = Vector3.Dot(transform.forward, dir);
                if (dot > 0 ) // 오브젝트가 플레이어의 전방에 있다면,
                {
                    detectable.OnVisible(true); // 보이는 상태로 전환
                }
                else // 후방에 있다면 
                {
                    detectable.OnVisible(false); // 안보이는 상태로 전환
                    Managers.UI.HideInvenUI(Define.InvenType.World); //UI 감추기
                }
            }

            if (detectables.Length == 0) // 오브젝트가 감지 되지 않는다면
            {
                Managers.UI.HideInvenUI(Define.InvenType.World); //UI 감추기
            }
        }    
    }

    // 플레이어오브젝트의 오른손 컨테이너에 무기를 장착하는 함수
    public void MountWeapon(Weapon _weapon)
    {
        if(isEquip)
            UnMountWeapon();
        
        for (int i = 0; i < RHandContainer.transform.childCount; i++)
        {
            if(RHandContainer.transform.GetChild(i).name == _weapon.prefab)
            {
                RHandContainer.transform.GetChild(i).gameObject.SetActive(true);
                GameObject go = RHandContainer.transform.GetChild(i).gameObject;
                mountedWeapon = go.GetComponent<MountedWeapon>();
                mountedWeapon.Init(_weapon);
                if (mountedWeapon.stat.weaponType == WeaponType.Rifle)
                    anim.SetLayerWeight(1, 1);
                
                isEquip = true;
            }
        }
    }

    // 현재 장착된 무기가 있다면 해제하는 함수
    public void UnMountWeapon()
    {
        for (int i = 0; i < RHandContainer.transform.childCount; i++)
        {
            if(RHandContainer.transform.GetChild(i).gameObject.activeSelf)
            {
                MountedWeapon tempWeapon = RHandContainer.transform.GetChild(i).GetComponent<MountedWeapon>();
                if (tempWeapon.stat.weaponType == WeaponType.Rifle)
                    anim.SetLayerWeight(1, 0);

                tempWeapon.gameObject.SetActive(false);
                mountedWeapon = null;
                isEquip = false;
            }
        }
    }

    public void ChangeViewAngle()
    {
        revealer.ViewAngle = stat.GetViewAngle();
    }

    IEnumerator GameOver()
    {
        Managers.Sound.PlayTriggerMusic(Define.TriggerSound.Death);

        float count = 0;
        while (count < 0.8f)
        {
            count += Time.deltaTime;
            Managers.UI.ShowDeathCanvas(count);
            yield return null;
        }

        Managers.Sound.PlayBGM(Define.BGM.Death);
        yield return new WaitForSeconds(3f);

        // 죽은 원인이 좀비감염이라면 좀비화 하기 
        if(Managers.Game.causeOfDeath == "Zombie")
        {
            // 본체 비활성화 
            playerCol.enabled = false;
            this.gameObject.SetActive(false);
            // 본체의 좀비버전 생성
            GameObject go = null;
            switch(playerType)
            {
                case Define.PlayerType.Nary :
                    go = Instantiate(Resources.Load<GameObject>("Prefabs/ZombieNary"));
                    break;
                case Define.PlayerType.Susan :
                    go = Instantiate(Resources.Load<GameObject>("Prefabs/ZombieSusan"));
                    break;
                case Define.PlayerType.Bob :
                    go = Instantiate(Resources.Load<GameObject>("Prefabs/ZombieBob"));
                    break;
            }

            if(go != null)
            {
                go.transform.position = transform.position;
                go.transform.rotation = transform.rotation;
                Managers.Game.virtualCam.SetFollow(go.transform);
                go.GetComponent<Animator>().Play("Z_death_A", 0, 1);
                go.GetComponent<Animator>().SetTrigger("WakeUp");
                yield return new WaitForSeconds(2f);
            }
        }
    }

    public void OnDamaged()
    {
        if(!Managers.Game.isGameOver)
            fsm.SetCurState((int)Define.PlayerStateType.Beaten);
    }

    // 걷기, 뛰기 애니메이션 이벤트에 사용 
    public void FootStepSoundPlay()
    {
        if(!audioSource.isPlaying)
        {
            switch(playerType)
            {
                case Define.PlayerType.Nary :
                    audioSource.PlayOneShot(Managers.Sound.playerSoundsDict["FootStepNary"], 0.05f);
                    break;
                case Define.PlayerType.Susan :
                    audioSource.PlayOneShot(Managers.Sound.playerSoundsDict["FootStepSusan"], 0.05f);
                    break;
                case Define.PlayerType.Bob :
                    audioSource.PlayOneShot(Managers.Sound.playerSoundsDict["FootStepBob"], 0.05f);
                    break;
            }
        }
    }
}
