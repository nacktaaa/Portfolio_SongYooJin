using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour, IPoolable
{
    public float force = 150f;
    Rigidbody rb;

    public Define.PoolableType Type { get; set; } = Define.PoolableType.Bullet;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void FlyBullet(Vector3 _dir)
    {
        rb.AddForce(_dir * force);
    }

    private void OnTriggerEnter(Collider other) 
    {
        //Debug.Log($"TriggerEnter {other.name}");
        Managers.Pool.Push(this.gameObject);
    }

}
