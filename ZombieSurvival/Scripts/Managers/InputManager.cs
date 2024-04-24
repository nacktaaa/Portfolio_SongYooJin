using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InputManager 
{
    public Action<Define.MouseEvent> MouseEvent_R;
    public Action<Define.MouseEvent> MouseEvent_L;

    public Action<float> MouseEvent_W;


    [SerializeField]
    private string hAxisName = "Horizontal";
    [SerializeField]
    private string vAxisName = "Vertical";
    [SerializeField]
    private string splashName = "Splash";
    [SerializeField]
    private string crouchName = "Crouch";
    [SerializeField]
    private string inventoryName = "Inventory";
    [SerializeField]
    private string reloadName = "Reload";
    [SerializeField]
    private string mouseScrollName = "Mouse ScrollWheel";
    

    public float Hinput {get; private set;}
    public float Vinput {get; private set;}
    public bool Splash {get; private set;} = false;
    public bool Crouch {get; private set;} = false;
    public float Scroll {get; private set;}
    public bool Inven {get; private set;} = false;
    public bool Reload {get; private set;} = false;


    // 마우스 오른쪽 버튼 감지를 위한 변수
    bool bRMBpressed = false;  // 눌렸나?
    float bRMBpressedTime = 0; // 눌린 시간 저장


    // 마우스 왼쪽 버튼 감지를 위한 변수
    bool bLMBpressed = false;  // 눌렸나?
    float bLMBpressedTime = 0; // 눌린 시간 저장

    public void OnUpdate()
    {
        if(Managers.Game.isSleep)
            return;

        if(Managers.Game.isGameOver)
            return;

        // 키보드 입력 감지
        if(!Managers.Game.isPause)
        {
            Hinput = Input.GetAxis(hAxisName);
            Vinput = Input.GetAxis(vAxisName);
            Splash = Input.GetButton(splashName);
            Crouch = Input.GetButtonDown(crouchName);
            Inven = Input.GetButtonDown(inventoryName);
            Reload = Input.GetButtonDown(reloadName);
        }

        // 포인터가 UI 위에 있는 경우 true 를, 아니라면 false를 반환하는 IsPointerOverGameObject()
        if(EventSystem.current.IsPointerOverGameObject())
            return; 

        // 마우스 휠 입력 감지, 이벤트 호출
        if(MouseEvent_W != null)
        {
            Scroll = Input.GetAxis(mouseScrollName);
            if(Scroll != 0)
                MouseEvent_W.Invoke(Scroll);            
        }

        // 마우스 오른쪽 버튼 입력 감지, 이벤트 호출
        if(MouseEvent_R != null)
        {
            // 마우스 단발클릭 (처음 누르기 시작 / 짧게 누름 / 떼기)
            // 마우스 누르고 있기 (처음 누르기 시작 / 누르는 중 / 떼기)
            // 처음 누르고 누르는 중의 시간을 재서 그 시간이 짧으면 클릭, 아니면 누르고 있는 것으로 판단
            // 결과 적으로 
            // '클릭' 시 : RBMDown 호출 -> RMBClick호출
            // '누르고 있을' 시 :  RBMDown 호출 -> RMBPress 호출(계속) -> RMBUp 호출

            if(Input.GetMouseButton(1))
            {
                if(!bRMBpressed) //처음 눌렸다면
                {
                    MouseEvent_R.Invoke(Define.MouseEvent.RMBDown); // 처음 누르는 순간 1번 호출
                    bRMBpressed = true;
                    bRMBpressedTime = Time.time;
                }
                else
                {
                    if(Time.time - bRMBpressedTime < 0.1f ) // 클릭하는 경우 Press 호출 안하기 위함 
                        return;
                    else
                    {
                        MouseEvent_R.Invoke(Define.MouseEvent.RMBPress); // 눌린 상태에서 계속 호출
                    }
                        
                }
            }
            else if (Input.GetMouseButtonUp(1))
            {
                if(bRMBpressed)
                {
                    if( Time.time - bRMBpressedTime < 0.1f )
                        MouseEvent_R.Invoke(Define.MouseEvent.RMBClick); // 클릭인 경우 1번 호출
                    else
                        MouseEvent_R.Invoke(Define.MouseEvent.RMBUp);    // 프레스 상태에서 뗏을 경우 1번  호출
                    bRMBpressed = false;
                    bRMBpressedTime = 0;
                }
            }

        }

        // 마우스 왼쪽 버튼 입력 감지, 이벤트 호출
        if(MouseEvent_L != null)
        {
            // 오른쪽 버튼과 같음
            // '클릭' 시 : LBMDown 호출 -> LMBClick호출
            // '누르고 있을' 시 : LBMDown 호출 -> LMBPress 호출(계속) -> LMBUp 호출

            if(Input.GetMouseButton(0))
            {
                if(!bLMBpressed) //처음 눌렸다면
                {
                    MouseEvent_L.Invoke(Define.MouseEvent.LMBDown); // 처음 누르는 순간 1번 호출
                    bLMBpressed = true;
                    bLMBpressedTime = Time.time;
                }
                else
                {
                    if(Time.time - bLMBpressedTime < 0.1f ) // 클릭하는 경우 Press 호출 안하기 위함 
                        return;
                    else
                    {
                        MouseEvent_L.Invoke(Define.MouseEvent.LMBPress); // 눌린 상태에서 계속 호출
                    }
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if(bLMBpressed)
                {
                    if( Time.time - bLMBpressedTime < 0.1f )
                        MouseEvent_L.Invoke(Define.MouseEvent.LMBClick); // 클릭인 경우 1번 호출
                    else
                        MouseEvent_L.Invoke(Define.MouseEvent.LMBUp);    // 프레스 상태에서 뗏을 경우 1번  호출
                    bLMBpressed = false;
                    bLMBpressedTime = 0;
                }
            }
        }

    }

}
