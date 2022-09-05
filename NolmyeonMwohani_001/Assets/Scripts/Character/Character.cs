using UnityEngine;

abstract class Character : MonoBehaviour
{
    public Character(float hp, float spd)
    {
        _hp = hp;
        _spd = spd;
    }

    virtual public void Move()
    {
        transform.position += transform.forward * Time.deltaTime;
    }
    virtual public void Die() { }
    virtual public void Attack() { _weapon.Attack(); }
    public void Damage(float dmg) { _hp -= dmg; }
    public bool IsDead { get { return _hp >= 0; } }

    virtual public void Start()
    {
        _weapon = GetComponent<Weapon>();
    }
    virtual public void Update()
    {
        Move();
    }

    [SerializeField] private float _hp;
    [SerializeField] private float _spd;
    protected Weapon _weapon;
}
