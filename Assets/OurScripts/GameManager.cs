using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistantInfo
{
    public Player player;
    public Player opponent;
    public Vector3 playerPos;
    public List<RewardTile> rewardTiles = new List<RewardTile>();
    public List<CombatStarter> combatTiles = new List<CombatStarter>();
}

public class GameManager : MonoBehaviour
{
    public static PersistantInfo info = new PersistantInfo();
    public static GameManager instance;


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

    public void SceneSwitchToCombat(Player player1, Player player2, Vector3 position)
    {
        info.player = player1;
        info.opponent = player2;
        info.playerPos = position;
        SceneManager.LoadScene("BaseScene");
    }

    public void SceneSwitchToOverWorld(Player player1)
    {
        info.player = player1;
        SceneManager.LoadScene("KianWorldMovement");
        Invoke("InvokeOnOverWorldLoad", 0.05f);
    }

    private void InvokeOnOverWorldLoad()
    {
        Pathfinding.INSTANCE.UpdteToNewPlayer(info.player, info.playerPos);
        SetInformationOfSpecialTiles();
    }


    //TODO: Keep for now, might be a dud and useless
    public void SetInformationOfSpecialTiles()
    {
        foreach (CombatStarter specialTile in info.combatTiles)
        {
            foreach (ClickableTile tile in TileMap.INSTANCE.clickableTiles)
            {
                if (tile.tileX == specialTile.tileX && tile.tileZ == specialTile.tileZ)
                {
                    tile.GetComponent<CombatStarter>().hasBeenUsed = specialTile.hasBeenUsed;
                    tile.fieldType = FieldType.Empty;
                    tile.GetComponent<CombatStarter>().visualUnit.SetActive(false);
                    break;
                }
            }
        }

        foreach (RewardTile specialTile in info.rewardTiles)
        {
            foreach (ClickableTile tile in TileMap.INSTANCE.clickableTiles)
            {
                if (tile.tileX == specialTile.tileX && tile.tileZ == specialTile.tileZ)
                {
                    tile.GetComponent<RewardTile>().hasBeenUsed = specialTile.hasBeenUsed;
                    tile.fieldType = FieldType.Empty;
                    break;
                }
            }
        }

    }


}
