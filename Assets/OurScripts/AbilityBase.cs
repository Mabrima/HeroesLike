﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityTiming
{
    DuringAttack, AfterAttack, PreDefence, DuringDefence, AfterDefence, StartOfTurn
}

public abstract class AbilityBase : MonoBehaviour
{
    public AbilityTiming timing;
    public bool used = false;

    public abstract void Initiate();

    public abstract void TriggerAbility(UnitHandler unit);

    public virtual bool CheckIfApplicable(AbilityTiming time)
    {
        return time == timing;
    }

}