using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISafeAreaHandler : MonoBehaviour
{
    RectTransform panel;

    private void Start()
    {
        panel = GetComponent<RectTransform>();
        SetAnchorToSafeArea();
    }

    private void SetAnchorToSafeArea()
    {
        Rect area = Screen.safeArea;

        // 화면 전체의 픽셀 사이즈
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);

        // 사용되는 스크린 비율에 맞게 앵커를 설정
        panel.anchorMin = area.position / screenSize;
        panel.anchorMax = (area.position + area.size) / screenSize;
    }
}
