using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityTiming
{
    DuringAttack, AfterAttack, PreDefence, DuringDefence, AfterDefence, StartOfTurn
}


/// <summary>
/// WIP, never got finished so it does nothing.
/// </summary>
public abstract class AbilityBase : MonoBehaviour
{
    [HideInInspector] public AbilityTiming timing;
    public bool used = false;

    public abstract void Initiate();

    public abstract void TriggerAbility(UnitHandler unit);

    public virtual bool CheckIfApplicable(AbilityTiming time)
    {
        return time == timing;
    }

}