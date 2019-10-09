﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    ClickableTile startTile;
    ClickableTile destinationTile;
    List<ClickableTile> open;
    List<ClickableTile> closed;
    List<ClickableTile> allTiles;
    ClickableTile currentTile;
    List<ClickableTile> currentTileNeighbours;
    List<ClickableTile> tilePath;

    public Player player;

    public Stack<ClickableTile> playerPath;

    const int ONE_G_STEP = 10;

    [Range(0f, 1f)]
    public float waitTime;

    TileMap map;

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

    private void CalculateH(ClickableTile tile, ClickableTile goal)
    {
        int calculatedH = 0;

        int xDis = Mathf.Abs(goal.tileX - tile.tileX);
        int yDis = Mathf.Abs(goal.tileY - tile.tileY);

        calculatedH = (xDis + yDis) * ONE_G_STEP;
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

    public void PathfindingSlow(ClickableTile start, ClickableTile goal)
    {
        

        if (initialRun)
        {
            allTiles.AddRange(TileMap.INSTANCE.GetAllTiles());
            initialRun = false;
        }

        open.Clear();
        closed.Clear();

        foreach (ClickableTile tile in allTiles)
        {
            tile.sphereRend.enabled = false; //Does not work weirdly. 
            tile.ResetPathfindingValues();
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
                do
                {
                    current.sphereRend.enabled = true;
                    playerPath.Push(current);
                    current = current.tileData.pfParent;
                }
                while (current.tileData.pfParent != start);
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

                if (neighbour.isWalkable == false)
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

    ClickableTile chosenTile;


    public bool ChoseNextTile(ClickableTile tile)
    {
        if (chosenTile != null && chosenTile == tile)
        {
            Debug.Log("move player");
            player.StartMovePlayer(player.isMoving);
            return true;
        }
        chosenTile = tile;
        return false;
    }

}
