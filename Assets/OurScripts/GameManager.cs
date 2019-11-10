using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Player player;
    public Player opponent;
    List<ClickableTile> specialTiles = new List<ClickableTile>();

    void Awake() 
    { 
        if (instance)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void SceneSwitchToCombat(Player player1, Player player2)
    {
        this.player = player1;
        this.opponent = player2;
        SceneManager.LoadScene("BaseScene");
    }

    public void SceneSwitchToOverWorld()
    {
        SceneManager.LoadScene("KianWorldMovement");
    }


    //TODO: Keep for now, might be a dud and useless
    public ClickableTile GetClickableTileAtPosition(int x, int z)
    {
        foreach(ClickableTile tile in specialTiles)
        {
            if (tile.tileX == x && tile.tileZ == z)
            {
                return tile;
            }
        }
        return null;
    }

}
