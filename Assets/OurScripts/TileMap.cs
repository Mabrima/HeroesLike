using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TileMap : MonoBehaviour
{

    public GameObject player;

    public TileType[] tileTypes;

    public int[,] tiles;
    public ClickableTile[,] clickableTiles;

    int mapSizeX = 10;
    int mapSizeY = 10;

    public static TileMap INSTANCE;

    private void Awake()
    {
        if (INSTANCE == null)
        {
            INSTANCE = this;
        }
        else
        {
            if (INSTANCE != this)
            {
                Destroy(this.gameObject);
            }
        }
    }

    void Start()
    {
        clickableTiles = new ClickableTile[mapSizeX, mapSizeY];

        GenerateMapData();
        GenerateMapVisual();
    }

    void GenerateMapData()
    {
        tiles = new int[mapSizeX, mapSizeY];

        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeX; y++)
            {
                tiles[x, y] = 0;
            }
        }

        tiles[1, 0] = 2;
        tiles[1, 1] = 2;
        tiles[1, 2] = 2;
        tiles[1, 3] = 2;
        tiles[1, 4] = 2;
        tiles[1, 5] = 2;
        tiles[5, 1] = 2;
        tiles[5, 2] = 2;
        tiles[5, 3] = 2;
        tiles[5, 4] = 2;
        tiles[5, 5] = 2;
        tiles[5, 6] = 2;
    }
    
    void GenerateMapVisual()
    {
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeX; y++)
            {
                TileType tt = tileTypes[tiles[x, y]];

                ClickableTile go = Instantiate(tt.tileVisualPrefab, new Vector3(x, y, 0), Quaternion.identity).GetComponent<ClickableTile>();
                go.tileX = x;
                go.tileY = y;
                go.map = this;
                go.isWalkable = tt.CheckIfWalkable();

                clickableTiles[x, y] = go;

                
            }
        }
    }
   
    public List<ClickableTile> GetAllTiles()
    {
        List<ClickableTile> all = new List<ClickableTile>();

        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeX; y++)
            {
                all.Add(clickableTiles[x, y]);
            }
        }

        return all;
    }

    public List<ClickableTile> GetNeighbouringTiles(ClickableTile a_Tile)
    {
        List<ClickableTile> neighbouringTiles = new List<ClickableTile>();
        int xCheck;
        int yCheck;

        //right side
        xCheck = a_Tile.tileX + 1;
        yCheck = a_Tile.tileY;
        if (xCheck >= 0 && xCheck < mapSizeX)
        {
            if (yCheck >= 0 && yCheck < mapSizeY)
            {
                neighbouringTiles.Add(clickableTiles[xCheck, yCheck]);
            }
        }

        //left side
        xCheck = a_Tile.tileX - 1;
        yCheck = a_Tile.tileY;
        if (xCheck >= 0 && xCheck < mapSizeX)
        {
            if (yCheck >= 0 && yCheck < mapSizeY)
            {
                neighbouringTiles.Add(clickableTiles[xCheck, yCheck]);
            }
        }

        //Top side
        xCheck = a_Tile.tileX;
        yCheck = a_Tile.tileY + 1;
        if (xCheck >= 0 && xCheck < mapSizeX)
        {
            if (yCheck >= 0 && yCheck < mapSizeY)
            {
                neighbouringTiles.Add(clickableTiles[xCheck, yCheck]);
            }
        }

        //Bottom side
        xCheck = a_Tile.tileX;
        yCheck = a_Tile.tileY - 1;
        if (xCheck >= 0 && xCheck < mapSizeX)
        {
            if (yCheck >= 0 && yCheck < mapSizeY)
            {
                neighbouringTiles.Add(clickableTiles[xCheck, yCheck]);
            }
        }

        return neighbouringTiles;
    }

    public Vector3 TileCoordToWorldCoord(int x, int y)
    {
        return new Vector3(x, y, player.transform.position.z);
    }

    public void MovePlayerTo(int x, int y, ClickableTile tile)
    {
        //player.transform.position = TileCoordToWorldCoord(x,y);
        Pathfinding.INSTANCE.PathfindingSlow(clickableTiles[0,0], tile);
    }
}
