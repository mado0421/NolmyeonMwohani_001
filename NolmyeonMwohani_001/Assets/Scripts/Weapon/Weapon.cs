using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

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

    //=========================================================================
    // 사용자가 인스펙터에서 정하는 부분
    [SerializeField] private float _rpm = 600; //roundPerMinute
    // 실제로 사용할 숨겨진 Delay 시간
    protected float _rps;
    private bool _firable;
}

//=============================================================================
// Melee Weapon
//=============================================================================
public abstract class MeleeWeapon : Weapon
{   // 히트박스를 켜고 끄는 방식으로 충돌 검사? 히트박스는 매번 켜두고 시간마다 검사?
    // 후자로 하자
    override protected void Fire()
    {
    }
}

//=============================================================================
// Range Weapon
//=============================================================================
public abstract class RangeWeapon : Weapon
{
    override public void Start()
    {
        base.Start();

        // 한순간에 존재할 수 있는 최대 총알 개수는 다음과 같다.
        // (총알의 지속시간 / 발사까지 걸리는 시간) * (최대 발사속도 가속보너스) + 여유 한 칸
        int numBullets = (Mathf.CeilToInt(_bullet.GetComponent<Bullet>()._lifeTime / _rps) * 2) + 1;

        for(int i = 0; i < numBullets; i++)
        {
            GameObject temp = Instantiate(_bullet);
            // 리스트는 여기 있지만 계층구조는 전역 인스턴스의 자식으로 넣어 관리할 것이다.
            temp.transform.SetParent(BulletPool.Instance.transform); 
            temp.SetActive(false);
            _bulletPool.Add(temp);
        }
    }
    override protected void Fire()
    {
        transform.LookAt(GetTargetPosition());    // 머즐이 아니라 웨폰이 회전해야 함
        InitializeBullet(FindInactiveBullet());
    }

    //=========================================================================
    // 발사할 방향을 구한다.
    //=========================================================================
    private Vector3 GetTargetPosition()
    {
        Vector3 result = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(result);
        RaycastHit hit;
        LayerMask layerMask = LayerMask.GetMask("Ground");
        if (Physics.Raycast(ray, out hit, layerMask))
        {
            result = hit.point;
        }
        else result = transform.position;

        return result;
    }

    //=========================================================================
    // 현재 비활성화된 불렛 오브젝트를 구하고, 해당 불렛을 초기화하여 사용한다.
    //=========================================================================
    private GameObject FindInactiveBullet()
    {
        foreach (GameObject temp in _bulletPool)
            if (!temp.activeSelf) return temp;
        return null;
    }
    private void InitializeBullet(GameObject obj)
    {
        if (obj == null) return;
        obj.transform.position = _muzzle.position;
        obj.transform.rotation = _muzzle.rotation;
        obj.SetActive(true);    
    }

    [SerializeField] private Transform _muzzle;
    [SerializeField] private GameObject _bullet;
    private List<GameObject> _bulletPool = new List<GameObject>();
}

//=============================================================================
// None
//=============================================================================
// 아무것도 안하는 빈 손 역할
public class NoneWeapon : Weapon
{
    override protected void Fire()
    {
    }
}