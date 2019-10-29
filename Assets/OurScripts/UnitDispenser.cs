using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDispenser : MonoBehaviour
{
    [SerializeField] List<GameObject> spawnableUnits;

    public static UnitDispenser instance;

    private void Awake()
    {
        if (instance)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    public UnitHandler SpawnUnit(UnitHandler unitToSpawn)
    {
        foreach (GameObject unit in spawnableUnits)
        {
            if(unit.GetComponent<UnitHandler>().unitBase == unitToSpawn.unitBase)
            {
                UnitHandler spawnedUnit = Instantiate(unit).GetComponent<UnitHandler>();
                BringOverInfo(spawnedUnit, unitToSpawn);
                return spawnedUnit;
            }
        }
        Debug.Log("ERROR No matching Unit was found");
        return null;
    }

    void BringOverInfo(UnitHandler to, UnitHandler from)
    {
        to.unitBase = from.unitBase;
        to.team = from.team;
        to.totalHealth = from.totalHealth;
        to.amountOfUnits = from.amountOfUnits;
    }

}
