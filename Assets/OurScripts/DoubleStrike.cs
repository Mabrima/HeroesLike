using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleStrike : AbilityBase
{
    public override void Initiate()
    {
        timing = AbilityTiming.AfterAttack;
    }

    public override void TriggerAbility(UnitHandler unit)
    {
        if (!used)
        {
            used = true;
            CombatManager.instance.CombatAttack(unit);
        }
    }

}
