using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using DG.Tweening;

public class SpawnPointSpin : MonoBehaviour
{
    void Start()
    {
        transform.DORotate(new Vector3(0, 0, 360), 1, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
        // z축으로 360도 회전하는 동작을 1초 동안 수행하고, 이를 무한히 반복
        /*Sequence seq = DOTween.Sequence(); // Sequence 생성
        seq.Append(transform.DOScale(0.5f, 2)) // localscale을 0.5로 바꾸는 동작을 2초 동안 수행
           .Append(transform.DOScale(1, 2)) // localscale을 1로 바꾸는 동작을 2초 동안 수행
           .SetLoops(-1); // 위의 두 동작을 연속적으로 수행하는 것을 무한번 반복
        */
    }

    // Update is called once per frame
    void Update()
    {
    }
}
