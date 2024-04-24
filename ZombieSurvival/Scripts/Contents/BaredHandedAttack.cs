using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaredHandedAttack : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // 공격처리
        ZombieBT zombie = other.GetComponent<ZombieBT>();
        if(zombie != null)
        {
            //Debug.Log("좀비가 맞았다");
            zombie.OnDamaged(Managers.Game.player.stat.GetDamage());
        }
    }
}
