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

    // ����ڰ� �ν����Ϳ��� ���ϴ� �κ�
    [SerializeField] private float _rpm = 60; //roundPerMinute

    // ������ ����� Delay �ð�
    private float _rps;
    private bool _firable;
}

//=============================================================================
// Melee Weapon
//=============================================================================
public abstract class MeleeWeapon : Weapon
{   // ��Ʈ�ڽ��� �Ѱ� ���� ������� �浹 �˻�? ��Ʈ�ڽ��� �Ź� �ѵΰ� �ð����� �˻�? ���ڷ� ����?

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
