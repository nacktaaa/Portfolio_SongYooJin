using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 조이스틱으로 받은 인풋을 반환 하는 기능
/// 드래그 관련 인터페이스 사용
/// </summary>
public class JoyStickInput : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image BG;                // 조이스틱 배경 이미지 참조 변수
    public Image stick;             // 조이스틱 스틱 이미지 참조 변수
    Vector2 stickFirstPosition;     // 드래그 시작 시 시작 위치
    Vector2 initPos;                // 조이스틱 기본 위치
    static Vector2 inputVec = Vector2.zero;        // 인풋을 저장할 변수
    float stickRadius;              // 조이스틱 배경 이미지 반지름
    

    private void Start()
    {
        stickRadius = BG.rectTransform.sizeDelta.x / 2;
        initPos = BG.transform.position;
    }

    // 드래그 시작 시 호출되는 함수
    public void OnBeginDrag(PointerEventData eventData)
    {
        BG.transform.position = eventData.position;
        stickFirstPosition = eventData.position;

    }

    // 드래그 중 호출되는 함수
    public void OnDrag(PointerEventData eventData)
    {
        inputVec = (eventData.position - stickFirstPosition).normalized;

        // 스틱 이미지가 배경 이미지를 벗어나지 않도록 처리
        float stickDist = Vector2.Distance(eventData.position, stickFirstPosition);

        if (stickDist < stickRadius)
            stick.transform.position = stickFirstPosition + inputVec * stickDist;
        else
            stick.transform.position = stickFirstPosition + inputVec * stickRadius;
    }

    // 드래그 종료 시 호출되는 함수
    public void OnEndDrag(PointerEventData eventData)
    {
        // 조이스틱을 기본 위치로 되돌림
        BG.transform.position = initPos;
        stick.transform.position = initPos;
        inputVec = Vector2.zero;
    }

    // 플레이어 인풋 정보를 반환하는 함수들
    public static Vector2 GetInputDir()
    {
        return inputVec;
    }
    public static float GetInputX()
    {
        return inputVec.x;
    }
    public static float GetInputY()
    {
        return inputVec.y;
    }

    private void OnDisable()
    {
        inputVec = Vector2.zero;
    }
}
