using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thorns : AbilityBase
{
    public override void Initiate()
    {
        timing = AbilityTiming.AfterDefence;
    }

    public override void TriggerAbility(UnitHandler unit)
    {
        unit.GetHit(0, 2, 2, unit.amountOfUnits);
    }
}
