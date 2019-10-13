using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    List<PathfindingTile> open;
    List<PathfindingTile> closed;
    List<PathfindingTile> allTiles;
    public Stack<PathfindingTile> playerPath;
    PathfindingTile chosenTile;

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
        open = new List<PathfindingTile>();
        closed = new List<PathfindingTile>();
        allTiles = new List<PathfindingTile>();
        playerPath = new Stack<PathfindingTile>();
    }

    private void CalculateH(PathfindingTile tile, PathfindingTile goal)
    {
        int calculatedH = 0;

        int xDis = Mathf.Abs(goal.tileX - tile.tileX);
        int yDis = Mathf.Abs(goal.tileY - tile.tileY);

        calculatedH = (xDis + yDis) * ONE_G_STEP;
    }

    private int CalculateGCost(PathfindingTile calculatedTile, PathfindingTile parentTile)
    {
        return ONE_G_STEP;
    }

    private PathfindingTile FindLowestFCostTile()
    {
        if (open.Count > 0)
        {
            PathfindingTile smallestFTile = open[0];
            int smallestF = smallestFTile.tileData.CalculateAndGetF();
            foreach (PathfindingTile tile in open)
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

    private void SetHForAll(PathfindingTile theGoal)
    {
        foreach (PathfindingTile tile in allTiles)
        {
            CalculateH(tile, theGoal);
        }
    }

    bool initialRun = true;

    public void PathFinding(PathfindingTile start, PathfindingTile goal)
    {
        if (initialRun)
        {
            allTiles.AddRange(TileMap.INSTANCE.GetAllTiles());
            initialRun = false;
        }

        //Clear all lists/stacks when we find a path
        open.Clear();
        closed.Clear();
        playerPath.Clear();


        foreach (PathfindingTile tile in allTiles) //Go through all tiles and turn spheres off and set start material and reset pf-values.
        {
            tile.ResetPathfindingValues();

            tile.sphereRend.enabled = false;   
            tile.sphereRend.material = tile.sphereStartMat;
        }

        SetHForAll(goal);

        open.Add(start);

        do
        {
            PathfindingTile current = FindLowestFCostTile();

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


            foreach (PathfindingTile neighbour in current.map.GetNeighbouringTiles(current))
            {
                if (closed.Contains(neighbour))
                {
                    Debug.Log("Fail closed");
                    continue;
                }

                if (neighbour.fieldType != FieldType.Empty)
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

    public bool ChoseNextTile(PathfindingTile tile)
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
