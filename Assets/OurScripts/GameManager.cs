using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    Player player;
    Player opponent;
    
    void Awake() 
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

    public void SceneSwitch(Player player1, Player player2)
    {
        this.player = player1;
        this.opponent = player2;
        SceneManager.LoadScene("BaseScene");
        Invoke("StartCombat", .5f);
    }

    private void StartCombat()
    {
        CombatManager.instance.StartCombat(player, opponent);
    }

}
