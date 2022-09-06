using System;
using System.Collections.Generic;
using UnityEngine;

class PlayerCharacter : Character
{
    public PlayerCharacter() : base(100, 5) 
    {
        _rotateSpd = 180;
    }

    //override public void Move()
    override public void Attack() { 
        base.Attack();
    }
    //override public void Die()

    override public void Start()
    {
        base.Start();

        _keyDic = new Dictionary<KeyCode, Action>
        {
            { KeyCode.A, RotateLeft },
            { KeyCode.D, RotateRight },
            { KeyCode.Mouse0, Attack },
            { KeyCode.Mouse1, Accelerate },
        }; 
    }
    override public void Update()
    {
        base.Update();
        
        if (Input.anyKey)
            foreach(var dic in _keyDic) 
                if(Input.GetKey(dic.Key)) dic.Value();
    }

    private void RotateLeft() { transform.Rotate(0, Time.deltaTime * -_rotateSpd, 0); }
    private void RotateRight() { transform.Rotate(0, Time.deltaTime * _rotateSpd, 0); }
    private void Accelerate() { Debug.Log("Accelerate"); }

    private Dictionary<KeyCode, Action> _keyDic;
    private float _rotateSpd;
}
