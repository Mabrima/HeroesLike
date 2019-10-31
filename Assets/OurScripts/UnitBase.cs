using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UnitBase")]

public class UnitBase : ScriptableObject
{
    public new string name;
    public int baseHealth;
    public int minDamage;
    public int maxDamage;
    public int speed;
    public int initiative;
    public int attack;
    public int defence;
    public Sprite sprite;
    public List<AbilityBase> abilities;
    public bool flyer = false;
    public bool shooter = false;

    private float increasePerAttack = 0.05f;
    private float decreasePerDefence = 0.025f;

    public int CalculateDamage(int otherAttack, int otherDamage)
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

        return (int) Mathf.Max(otherDamage * damageMultiplier, 1);
    }

}
