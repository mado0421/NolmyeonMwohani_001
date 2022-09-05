using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class EnemyCharacter_Pawn : EnemyCharacter
{
    public EnemyCharacter_Pawn() : base(20, 5.5f) { }

    //override public void Move()
    //override public void Die()
    override public void Attack() { Debug.Log("attack"); }

    override public void Update()
    {
        base.Update();
    }
}
