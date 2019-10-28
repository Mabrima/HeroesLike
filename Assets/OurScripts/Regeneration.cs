using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Regeneration : AbilityBase
{
    public override void Initiate()
    {
        timing = AbilityTiming.StartOfTurn;
    }

    public override void TriggerAbility(UnitHandler unit)
    {
        unit.currentUnitHealth = unit.unitBase.baseHealth;
    }
}
