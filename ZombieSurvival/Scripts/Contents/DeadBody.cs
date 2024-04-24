using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadBody : MonoBehaviour
{
    public int hp = 50;

    public bool OnDamaged()
    {
        bool isOver = false;
        hp -= 5;
        if(hp <= 0)
        {
            Destroy(this.gameObject);
            return isOver = true;
        }
        
        return isOver;
    }
    
}
