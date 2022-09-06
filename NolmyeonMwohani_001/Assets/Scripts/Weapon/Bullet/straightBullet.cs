using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//=============================================================================
// straight Bullet
//=============================================================================
public class straightBullet : Bullet
{
    override public void Move()
    {
        transform.position += transform.forward * Time.deltaTime * _spd;
    }
}