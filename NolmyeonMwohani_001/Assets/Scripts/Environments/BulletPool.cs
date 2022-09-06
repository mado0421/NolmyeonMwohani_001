using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    private static BulletPool instance = null;
    // Start is called before the first frame update
    void Awake()
    {
        if(null == instance)
        {
            instance = this;
        }
    }

    public static BulletPool Instance
    {
        get
        {
            if (null == instance) return null;
            return instance;
        }
    }
}
