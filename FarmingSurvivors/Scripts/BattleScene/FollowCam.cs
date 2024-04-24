using System.Collections;
using System.Collections.Generic;
using StageMap;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform target;
    Vector3 offset;
    StageType stageType;

    public void Init(Transform target, StageType stageType)
    {
        this.target = target;
        this.stageType = stageType;
        offset = transform.position - target.position;
    }

    private void LateUpdate()
    {
        if(target != null)
        {
            transform.position = target.position + offset;

            switch (stageType)
            {
                case StageType.Normal :
                    break;
                case StageType.Gold :
                {
                    if (transform.position.x < -2.5f)
                    {
                        transform.position = new Vector3 (-2.5f, transform.position.y, transform.position.z);
                    }
                    else if (transform.position.x > 2.5f) 
                    {
                        transform.position = new Vector3 (2.5f, transform.position.y, transform.position.z);
                    }
                }
                    break;
                case StageType.Boss :
                {
                    float x = transform.position.x;
                    float y = transform.position.y;
                    if (transform.position.x <= -4f)
                        x = -4f; 
                    else if (transform.position.x >= 4f)
                        x = 4f; 
                    
                    if (transform.position.y <= -2.5f)
                        y = -2.5f;
                    else if (transform.position.y >= 2.5f)
                        y = 2.5f;

                    transform.position = new Vector3(x, y, transform.position.z);
                }   
                    break;
            }
        }

    }
}
