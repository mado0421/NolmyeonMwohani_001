using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public abstract class Bullet : MonoBehaviour
{
    abstract public void Move();
    virtual public void Die()
    {
        gameObject.SetActive(false);
    }
    private IEnumerator StartLife()
    {
        yield return new WaitForSeconds(_lifeTime);
        Die();
    }
    public void OnEnable()
    {
        StartCoroutine(StartLife());
    }
    public void Update()
    {
        Move();
    }
    public void OnCollisionEnter(Collision collision)
    {
        Character character = collision.gameObject.GetComponent<Character>();
        if (character != null)
        {
            character.Damage(_dmg);
            gameObject.SetActive(false);
        }
    }

    [SerializeField] protected float _dmg = 5;
    [SerializeField] protected float _spd = 20;
    /*[SerializeField]*/ public float _lifeTime = 0.5f;
}
