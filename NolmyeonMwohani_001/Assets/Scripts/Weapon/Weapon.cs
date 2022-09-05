using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    virtual public void Start()
    {
        _rps = (1 /_rpm) * 60;
        _firable = true;
    }

    public void Attack() 
    {
        if (_firable)
        {
            // Pull bullet object or Check Collisions for melee.
            Fire();
            Debug.Log("Fire");
            _firable = false;
            StartCoroutine(Delay());
        }
    }
    abstract protected void Fire();

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(_rps);
        _firable = true;
    }

    // 사용자가 인스펙터에서 정하는 부분
    [SerializeField] private float _rpm = 60; //roundPerMinute

    // 실제로 사용할 Delay 시간
    private float _rps;
    private bool _firable;
}

//=============================================================================
// Melee Weapon
//=============================================================================
public abstract class MeleeWeapon : Weapon
{   // 히트박스를 켜고 끄는 방식으로 충돌 검사? 히트박스는 매번 켜두고 시간마다 검사? 후자로 하자?

}

//=============================================================================
// Range Weapon
//=============================================================================
public abstract class RangeWeapon : Weapon
{
    override public void Start()
    {
        base.Start();
        _bulletManager = GameObject.Find("BulletPool").GetComponent<BulletManager>();
    }
    override protected void Fire()
    {
        foreach(GameObject obj in _bulletManager._bulletPool)
        {
            if (!obj.activeSelf)
            {
                InitializeBullet(obj);
                return;
            }
        }
    }
    private void InitializeBullet(GameObject obj)
    {
        obj = _bullet;
        if (_muzzle)
        {
            obj.transform.position = _muzzle.position;
            obj.transform.rotation = _muzzle.rotation;
        }
        else
        {
            obj.transform.position = transform.position;
            obj.transform.rotation = transform.rotation;
        }
        obj.SetActive(true);
    }

    [SerializeField] private Transform _muzzle;
    [SerializeField] private GameObject _bullet;
    [SerializeField] private BulletManager _bulletManager;
}
