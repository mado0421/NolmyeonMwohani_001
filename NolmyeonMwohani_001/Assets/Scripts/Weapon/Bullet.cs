using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bullet : MonoBehaviour
{
    abstract public void Move();
    public void Update()
    {
        Move();
    }
    public void OnCollisionEnter(Collision collision)
    {
        Character character = collision.gameObject.GetComponent<Character>();
        character.Damage(_dmg);
        gameObject.SetActive(false);
    }

    [SerializeField] protected float _dmg;
    [SerializeField] protected float _spd;
    [SerializeField] protected float _range;
}
