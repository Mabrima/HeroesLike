using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Player player;
    public Player opponent;
    
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
}
