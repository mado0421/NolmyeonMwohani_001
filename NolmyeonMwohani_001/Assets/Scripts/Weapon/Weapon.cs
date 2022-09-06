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
    // ����ڰ� �ν����Ϳ��� ���ϴ� �κ�
    [SerializeField] private float _rpm = 600; //roundPerMinute
    // ������ ����� ������ Delay �ð�
    protected float _rps;
    private bool _firable;
}

//=============================================================================
// Melee Weapon
//=============================================================================
public abstract class MeleeWeapon : Weapon
{   // ��Ʈ�ڽ��� �Ѱ� ���� ������� �浹 �˻�? ��Ʈ�ڽ��� �Ź� �ѵΰ� �ð����� �˻�?
    // ���ڷ� ����
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

        // �Ѽ����� ������ �� �ִ� �ִ� �Ѿ� ������ ������ ����.
        // (�Ѿ��� ���ӽð� / �߻���� �ɸ��� �ð�) * (�ִ� �߻�ӵ� ���Ӻ��ʽ�) + ���� �� ĭ
        int numBullets = (Mathf.CeilToInt(_bullet.GetComponent<Bullet>()._lifeTime / _rps) * 2) + 1;

        for(int i = 0; i < numBullets; i++)
        {
            GameObject temp = Instantiate(_bullet);
            // ����Ʈ�� ���� ������ ���������� ���� �ν��Ͻ��� �ڽ����� �־� ������ ���̴�.
            temp.transform.SetParent(BulletPool.Instance.transform); 
            temp.SetActive(false);
            _bulletPool.Add(temp);
        }
    }
    override protected void Fire()
    {
        transform.LookAt(GetTargetPosition());    // ������ �ƴ϶� ������ ȸ���ؾ� ��
        InitializeBullet(FindInactiveBullet());
    }

    //=========================================================================
    // �߻��� ������ ���Ѵ�.
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
    // ���� ��Ȱ��ȭ�� �ҷ� ������Ʈ�� ���ϰ�, �ش� �ҷ��� �ʱ�ȭ�Ͽ� ����Ѵ�.
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
// �ƹ��͵� ���ϴ� �� �� ����
public class NoneWeapon : Weapon
{
    override protected void Fire()
    {
    }
}