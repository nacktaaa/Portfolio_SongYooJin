using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    float _zoomSpeed = 10f;
    [SerializeField]
    float _originSize = 5f;
    [SerializeField]
    float _minSize = 5f;
    [SerializeField]
    float _maxSize = 5f;
    CinemachineVirtualCamera cam;

    public void Init()
    {
        Managers.Input.MouseEvent_W -= ZoomInOut;
        Managers.Input.MouseEvent_W += ZoomInOut;
        cam = GetComponent<CinemachineVirtualCamera>();
        cam.m_Lens.OrthographicSize = _originSize;
        SetFollow(Managers.Game.player.transform);
    }

    void ZoomInOut(float scroll)
    {
        float newSize = cam.m_Lens.OrthographicSize - (scroll * _zoomSpeed ); 
        newSize = Mathf.Clamp(newSize, _minSize, _maxSize);
        
        cam.m_Lens.OrthographicSize = newSize;
    }

    public void SetFollow(Transform _transform)
    {
        cam.Follow = _transform;
        cam.LookAt = _transform;
    }
    


}
