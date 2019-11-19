using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FieldType
{
    Empty, Occupied, Obstacle, Reward, Combat
}

/// <summary>
/// Made by Robin Arkblad
/// </summary>
public class FieldHandler : MonoBehaviour
{
    public static int X_SIZE = 12;
    public static int Y_SIZE = 10;
    public static Quaternion ROTATION_TEAM_1 = Quaternion.Euler(0, -90, 0);
    public static Quaternion ROTATION_TEAM_2 = Quaternion.Euler(0, 90, 0);
    public static FieldHandler instance;
    public CombatTile[,] fieldObjects = new CombatTile[X_SIZE, Y_SIZE];
    [SerializeField] GameObject battleTilePrefab;
    List<CombatTile> neighbourList1 = new List<CombatTile>();
    List<CombatTile> neighbourList2 = new List<CombatTile>();

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        InitiateNewField();
    }

    public CombatTile GetTile(int x, int y)
    {
        return fieldObjects[x, y];
    }

    public void PutUnitOnTile(CombatTile tile, UnitHandler unit)
    {
        fieldObjects[tile.position.x, tile.position.y].PutUnitOnTile(unit);
    }

    public void RemoveUnitFromTile(CombatTile tile)
    {
        fieldObjects[tile.position.x, tile.position.y].RemoveUnitFromTile();
    }

    public void GetAvailableMovementTiles(CombatTile tile, int range, bool asFake = false)
    {
        neighbourList1.Clear();
        neighbourList2.Clear();
        neighbourList1.Add(tile);

        for (int i = 0; i < range; i++)
        {
            if (i % 2 == 0) 
            {
                GetNeighbours(neighbourList1, neighbourList2, asFake, tile);
            }
            else
            {
                GetNeighbours(neighbourList2, neighbourList1, asFake, tile);
            }
        }
    }

    private void GetNeighbours(List<CombatTile> useList, List<CombatTile> addList, bool asFake, CombatTile originalTile)
    {
        foreach (CombatTile tile in useList)
        {
            int x = tile.position.x;
            int y = tile.position.y;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    try
                    {
                        if ((i == 0 && j == 0) || (i != 0 && j != 0))
                            continue;

                        CombatTile neighbourTile = fieldObjects[x + i, y + j];
                        if (!asFake)
                        {
                            if (neighbourTile.fieldType == FieldType.Empty && neighbourTile.pfParent == null)
                            {
                                neighbourTile.SetAsSelectable(asFake);
                                neighbourTile.pfParent = tile;
                                addList.Add(neighbourTile);
                            }
                            if (neighbourTile.fieldType == FieldType.Occupied && neighbourTile.pfParent == null && neighbourTile != originalTile)
                            {
                                neighbourTile.pfParent = tile;
                            }
                        }
                        else
                        {
                            if (neighbourTile.fieldType == FieldType.Empty && neighbourTile.perishableParent == null)
                            {
                                neighbourTile.SetAsSelectable(asFake);
                                neighbourTile.perishableParent = tile;
                                addList.Add(neighbourTile);
                            }
                        }
                    }
                    catch
                    {
                        //Do nothing, just out of bounds
                    }
                }
            }
        }
        useList.Clear();
    }


    public void GetAvailableAttackTiles(CombatTile tile, int team)
    {
        int x = tile.position.x;
        int y = tile.position.y;
        Debug.Log("looking for tiles to attack");
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                if (i == 0 && j == 0)
                {
                    j++;
                }
                try
                {
                    if (fieldObjects[x + i, y + j].fieldType == FieldType.Occupied && fieldObjects[x + i, y + j].unitOnTile.team != team)
                    {
                        fieldObjects[x + i, y + j].SetAsSelectable(false);
                    }
                }
                catch
                {
                    //Do nothing, we just went out of array bounds
                }
                try
                {
                    if (fieldObjects[x + i, y - j].fieldType == FieldType.Occupied && fieldObjects[x + i, y - j].unitOnTile.team != team)
                    {
                        fieldObjects[x + i, y - j].SetAsSelectable(false);
                    }
                }
                catch
                {
                    //Do nothing, we just went out of array bounds
                }
                try
                {
                    if (fieldObjects[x - i, y - j].fieldType == FieldType.Occupied && fieldObjects[x - i, y - j].unitOnTile.team != team)
                    {
                        fieldObjects[x - i, y - j].SetAsSelectable(false);
                    }
                }
                catch
                {
                    //Do nothing, we just went out of array bounds
                }
                try
                {
                    if (fieldObjects[x - i, y + j].fieldType == FieldType.Occupied && fieldObjects[x - i, y + j].unitOnTile.team != team)
                    {
                        fieldObjects[x - i, y + j].SetAsSelectable(false);
                    }
                }
                catch
                {
                    //Do nothing, we just went out of array bounds
                }
            }
        }
    }

    public void InitiateNewField()
    {
        int x = 0;
        int y = 0;

        if (fieldObjects[0, 0] != null)
        {
            foreach (CombatTile tile in fieldObjects)
            {
                Destroy(tile.gameObject);
            }
        }

        foreach (CombatTile tile in fieldObjects)
        {
            fieldObjects[x, y] = Instantiate(battleTilePrefab, new Vector3(x, 0, y), Quaternion.identity, transform).GetComponent<CombatTile>();
            fieldObjects[x, y].SetPositionValues(x, y);

            x = (x + 1) % X_SIZE;
            if (x == 0)
                y = (y + 1);
        }
    }

    public void ResetSelectableLocations(bool fake = false)
    {
        foreach (CombatTile tile in fieldObjects)
        {
            tile.SetAsNotSelectable(fake);
            tile.perishableParent = null;
        }
    }

    public void ClearParents()
    {
        foreach (CombatTile tile in fieldObjects)
        {
            tile.pfParent = null;
        }
    }


    //TODO add "find all" when ranged
    public List<UnitHandler> GetEnemyUnitsInRange(CombatTile tile, bool ranged, int team)
    {
        List<UnitHandler> units = new List<UnitHandler>();
        CombatTile tempTile;
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;
                try
                {
                    tempTile = fieldObjects[tile.position.x + x, tile.position.y + y];
                    if (tempTile.fieldType == FieldType.Occupied && tempTile.unitOnTile.team != team)
                    {
                        units.Add(tempTile.unitOnTile);
                    }
                }
                catch
                {
                    //Do nothing, just out of bounds
                }
            }
        }

        return units;
    }

    public void FlyGetAvailableMovementTiles(CombatTile tile, int range, bool asFake = false)
    {
        int x = tile.position.x;
        int y = tile.position.y;
        for (int i = 0; i < range; i++)
        {
            for (int j = 0; j < range - i; j++)
            {
                try
                {
                    if (fieldObjects[x + i, y + j].fieldType == FieldType.Empty)
                        fieldObjects[x + i, y + j].SetAsSelectable(asFake);
                }
                catch
                {
                    //Do nothing, we just went out of array bounds
                }
                try
                {
                    if (fieldObjects[x + i, y - j].fieldType == FieldType.Empty)
                        fieldObjects[x + i, y - j].SetAsSelectable(asFake);
                }
                catch
                {
                    //Do nothing, we just went out of array bounds
                }
                try
                {
                    if (fieldObjects[x - i, y - j].fieldType == FieldType.Empty)
                        fieldObjects[x - i, y - j].SetAsSelectable(asFake);
                }
                catch
                {
                    //Do nothing, we just went out of array bounds
                }
                try
                {
                    if (fieldObjects[x - i, y + j].fieldType == FieldType.Empty)
                        fieldObjects[x - i, y + j].SetAsSelectable(asFake);
                }
                catch
                {
                    //Do nothing, we just went out of array bounds
                }
            }
        }
    }
}
