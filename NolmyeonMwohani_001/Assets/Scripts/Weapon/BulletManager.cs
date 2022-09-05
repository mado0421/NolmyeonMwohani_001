using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    [SerializeField] private uint _numBullets = 100;
    public List<GameObject> _bulletPool;

    void Start()
    {
        _bulletPool = new List<GameObject>();
        for(int i = 0; i < _numBullets; i++)
        {
            GameObject temp = new GameObject("Bullet");
            _bulletPool.Add(temp);
            temp.transform.SetParent(transform);
        }
    }
}
