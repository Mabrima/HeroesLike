using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UnitBase")]
public class UnitBase : ScriptableObject
{

    public int health;
    public int damage;
    public int speed;
    public int initiative;
    public int attack;
    public int defence;

    private float increasePerAttack = 0.05f;
    private float decreasePerDefence = 0.025f;

    public void CalculateDamage(int otherAttack, int otherDamage)
    {
        float damageMultiplier = 1;
        if (otherAttack > defence)
        {
            damageMultiplier += Mathf.Clamp((otherAttack - defence) * increasePerAttack, 0, 3);
        }

        if (otherAttack < defence)
        {
            damageMultiplier -= Mathf.Clamp((defence - otherAttack) * decreasePerDefence, 0, .7f);
        }

        health -= (int) Mathf.Max(damage * damageMultiplier, 1);
    }

}
