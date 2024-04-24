using System.Collections;
using UnityEngine;

public class MountedWeapon : MonoBehaviour
{
    public Collider col;
    public Weapon stat;
    public Transform firePos;
    MeshRenderer muzzleRenderer;
    Light flashLight;

    public void Init(Weapon weapon)
    {
        stat = weapon;
        col = GetComponent<CapsuleCollider>();
        col.enabled = false;
        if(stat.weaponType == WeaponType.Rifle)
        {
            muzzleRenderer = firePos.GetComponentInChildren<MeshRenderer>();
            flashLight = firePos.GetComponentInChildren<Light>();
            muzzleRenderer.enabled = false;
            flashLight.enabled = false;
        }
    }

    Coroutine reloadCoroutine;
    private void Update()
    {
        if(Managers.Input.Reload)
        {
            if(Managers.Inven.equipedItem.weaponType == WeaponType.Rifle)
                reloadCoroutine = StartCoroutine(Reload());
        }
    }

    // 근접무기에 달린 콜라이더로 인해 콜백 됨
    private void OnTriggerEnter(Collider other)
    {
        // 공격처리
        ZombieBT zombie = other.GetComponent<ZombieBT>();
        if(zombie != null)
        {
            //Debug.Log($"현재 공격력 : {Managers.Game.player.stat.GetDamage()}");
            Vector3 hitPos = other.ClosestPoint(col.bounds.center);
            Quaternion normal = Quaternion.LookRotation(transform.position - hitPos);
            zombie.OnDamaged(Managers.Game.player.stat.GetDamage());
            zombie.ShowBloodEffect(hitPos, normal);
        }
    }

    // 총기로 공격 시 호출하는 함수 
    public void FireRifle(Vector3 aimPos)
    {
        if(stat.weaponType == WeaponType.Rifle)
        {
            if(Managers.Inven.curMagazine > 0)
            {
                Managers.Sound.PlayEffectSound(Define.EffectSound.FireRifle);
                Vector3 bulletDir = (aimPos - firePos.position);
                bulletDir.y = 0;
                Bullet bullet = Managers.Pool.Pop(Define.PoolableType.Bullet).GetComponent<Bullet>();

                bullet.transform.position = firePos.position;              
                bullet.FlyBullet(bulletDir.normalized);
                StopCoroutine(ShowMuzzleEffect());
                StopCoroutine(OnMuzzleLight());
                StartCoroutine(ShowMuzzleEffect());
                StartCoroutine(OnMuzzleLight());

                RaycastHit hit;
                if(Physics.Raycast(firePos.position, bulletDir.normalized, out hit, 20f, 1 << 8))
                {
                    Quaternion rot = Quaternion.LookRotation(hit.normal);
                    hit.collider.GetComponent<ZombieBT>()?.OnDamaged(Managers.Game.player.stat.GetDamage());
                    hit.collider.GetComponent<ZombieBT>()?.ShowBloodEffect(hit.point, rot);
                }
                Managers.Inven.curMagazine --;
                Managers.UI.RefreshMagazineUI();
            }
        }
    }

    IEnumerator Reload()
    {
        if(Managers.Inven.AmmoCount > 0)
        {
            Managers.Sound.PlayEffectSound(Define.EffectSound.Reload);
            int count = Managers.Inven.AmmoCount;
            for(int i = 0; i < count; i++)
            {
                if(Managers.Inven.curMagazine == Managers.Inven.maxMagazine)
                    break;
                
                Managers.Inven.curMagazine ++;
                Managers.Inven.AmmoCount --;
                Managers.UI.RefreshMagazineUI();

                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    IEnumerator ShowMuzzleEffect()
    {
        SetRandomMuzzle();
        muzzleRenderer.enabled = true;
        yield return new WaitForSeconds(0.05f);

        SetRandomMuzzle();
        yield return new WaitForSeconds(0.05f);

        SetRandomMuzzle();
        yield return new WaitForSeconds(0.05f);
        muzzleRenderer.enabled = false;
    }

    float tempIntensity =0;
    IEnumerator OnMuzzleLight()
    {
        flashLight.enabled = true;
        flashLight.intensity = tempIntensity;

        while(!Mathf.Approximately(tempIntensity, 1.5f))
        {
            tempIntensity = Mathf.Lerp(tempIntensity, 1.5f, 1f);
            yield return null;
        } 

        while(!Mathf.Approximately(tempIntensity, 0))
        {
            flashLight.intensity = tempIntensity;
            tempIntensity = Mathf.Lerp(tempIntensity, 0, 1);
            yield return null;
        }
        tempIntensity = 0;
        flashLight.enabled = false;
    }
    void SetRandomMuzzle()
    {
        Vector2 offset = new Vector2(Random.Range(0,2), Random.Range(0,2))* 0.33f;
        muzzleRenderer.material.mainTextureOffset = offset;
        float angle = Random.Range(0, 360);
        muzzleRenderer.transform.localRotation = Quaternion.Euler(0,0,angle);
        float scale = Random.Range(0.4f, 0.8f);
        muzzleRenderer.transform.localScale = Vector3.one * scale;
    }

}