using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Made by Robin Arkblad
/// </summary>
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
                UnitHandler spawnedUnit = Instantiate(unit, transform).GetComponent<UnitHandler>();
                return spawnedUnit;
            }
        }
        Debug.Log("ERROR No matching Unit was found");
        return null;
    }
}
