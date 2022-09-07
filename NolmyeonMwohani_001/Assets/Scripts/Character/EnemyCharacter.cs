using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract class EnemyCharacter : Character
{
    public EnemyCharacter(float hp, float spd) : base(hp, spd) { }

    override public void Move()
    {
        transform.LookAt(_player.transform);
        base.Move();
    }
    //override public void Die()
    //override public void Attack()

    override public void Start()
    {
        base.Start();
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerCharacter = _player.GetComponent<PlayerCharacter>();
    }
    override public void Update()
    {
        base.Update();
    }
    public Character target
    {
        get { return _playerCharacter; }
    }

    private Character _playerCharacter;
    private GameObject _player;
}
