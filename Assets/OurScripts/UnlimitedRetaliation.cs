using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlimitedRetaliation : AbilityBase
{

    public override void Initiate()
    {
        timing = AbilityTiming.AfterDefence;
    }

    public override bool CheckIfApplicable(UnitHandler unit)
    {
        return !unit.canRetaliate;
    }

    public override void TriggerAbility(UnitHandler unit)
    {
        unit.canRetaliate = true;
    }
}
