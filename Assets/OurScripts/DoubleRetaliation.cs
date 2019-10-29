using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleRetaliation : AbilityBase
{
    public override void Initiate()
    {
        timing = AbilityTiming.AfterDefence;
    }

    public override void TriggerAbility(UnitHandler unit)
    {
        if (!used)
        {
            used = true;
            unit.canRetaliate = true;
        }
    }
}
