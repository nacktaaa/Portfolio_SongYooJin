using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Monster : MonoBehaviour
{
    protected MonsterStatus status;
    protected Rigidbody2D rb;
    protected Vector3 dir;

    float lifeSpanCount = 0;

    public virtual void Init(MonsterStatus status, Vector3 dir, Vector3 from)
    {
        transform.position = from;
        TrailRenderer trail = gameObject.GetComponent<TrailRenderer>();
        if (trail != null)
        {
            trail.Clear();
        }
        ParticleSystem particleSystem = gameObject.GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            particleSystem.Clear();
        }
        lifeSpanCount = 0;
        this.status = status;
        this.dir = dir;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        lifeSpanCount += Time.deltaTime;
        if (lifeSpanCount >= status.lifeSpan)
            DestroyThis();
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        other.GetComponent<PlayerController>().OnDamaged(status.damage);
        //Destroy(this.gameObject);
        DestroyThis();
    }

    protected void DestroyThis()
    {
        this.gameObject.SetActive(false);
    }

    public virtual void Shoot()
    {
        rb.velocity = dir * status.projectileSpeed;
    }
}
