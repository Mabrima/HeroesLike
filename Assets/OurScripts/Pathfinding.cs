﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    List<ClickableTile> open;
    List<ClickableTile> closed;
    public List<ClickableTile> allTiles;
    public Stack<ClickableTile> playerPath;
    ClickableTile chosenTile;

    public Player player;

    const int ONE_G_STEP = 10;

    public static Pathfinding INSTANCE;

    private void Awake()
    {
        if (INSTANCE == null)
        {
            INSTANCE = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        open = new List<ClickableTile>();
        closed = new List<ClickableTile>();
        allTiles = new List<ClickableTile>();
        playerPath = new Stack<ClickableTile>();
    }

    public void UpdteToNewPlayer(Player newPlayer, Vector3 position)
    {
        player.transform.position = position;
        player.units = newPlayer.units;
    }

    private void CalculateH(ClickableTile tile, ClickableTile goal)
    {
        int calculatedH = 0;

        int xDis = Mathf.Abs(goal.tileX - tile.tileX);
        int zDis = Mathf.Abs(goal.tileZ - tile.tileZ);

        calculatedH = (xDis + zDis) * ONE_G_STEP;
    }

    private int CalculateGCost(ClickableTile calculatedTile, ClickableTile parentTile)
    {
        return ONE_G_STEP;
    }

    private ClickableTile FindLowestFCostTile()
    {
        if (open.Count > 0)
        {
            ClickableTile smallestFTile = open[0];
            int smallestF = smallestFTile.tileData.CalculateAndGetF();
            foreach (ClickableTile tile in open)
            {
                if (tile.tileData.CalculateAndGetF() < smallestF)
                {
                    smallestF = tile.tileData.fCost;
                    smallestFTile = tile;
                }
            }
            return smallestFTile;
        }

        else
        {
            Debug.Log("Open list was empty");
            return null;
        }
    }

    private void SetHForAll(ClickableTile theGoal)
    {
        foreach (ClickableTile tile in allTiles)
        {
            CalculateH(tile, theGoal);
        }
    }

    bool initialRun = true;

    public void PathFinding(ClickableTile start, ClickableTile goal)
    {
        if (initialRun)
        {
            Debug.Log("initial run");
            allTiles.AddRange(TileMap.INSTANCE.GetAllTiles());
            initialRun = false;
        }

        //Clear all lists/stacks when we find a path
        open.Clear();
        closed.Clear();
        playerPath.Clear();


        foreach (ClickableTile tile in allTiles) //Go through all tiles and turn spheres off and set start material and reset pf-values.
        {
            tile.ResetPathfindingValues();

            tile.sphereRend.enabled = false;
            tile.sphereRend.material = tile.sphereStartMat;
        }

        SetHForAll(goal);

        open.Add(start);

        do
        {
            ClickableTile current = FindLowestFCostTile();

            open.Remove(current);
            closed.Add(current);

            if (current == goal)
            {
                playerPath.Clear();

                current.sphereRend.material = current.sphereEndMat; //Change material of End sphere.
                while (current.tileData.pfParent != start)
                {
                    playerPath.Push(current);

                    current.sphereRend.enabled = true;
                    current = current.tileData.pfParent;
                }
                playerPath.Push(current);

                current.sphereRend.enabled = true;
                return;
            }


            foreach (ClickableTile neighbour in current.map.GetNeighbouringTiles(current))
            {
                if (closed.Contains(neighbour))
                {
                    Debug.Log("Fail closed");
                    continue;
                }

                if (neighbour.fieldType == FieldType.Obstacle)
                {
                    Debug.Log("Not walkable");
                    continue;
                }

                if (open.Contains(neighbour) == false)
                {
                    open.Add(neighbour);
                    neighbour.tileData.pfParent = current;
                    neighbour.tileData.gCost = current.tileData.gCost + CalculateGCost(neighbour, current);
                }

                else
                {
                    if (current.tileData.gCost + ONE_G_STEP < neighbour.tileData.gCost)
                    {
                        neighbour.tileData.pfParent = current;
                        neighbour.tileData.gCost = current.tileData.gCost + CalculateGCost(neighbour, current);
                    }
                }
            }


        } while (open.Count > 0);
        Debug.Log("No Path Found");

    }

    public bool ChoseNextTile(ClickableTile tile)
    {
        if (chosenTile != null && chosenTile == tile)
        {
            player.StartMovePlayer(player.isMoving);
            return true;
        }

        chosenTile = tile;
        return false;
    }

}
