using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatStarter : MonoBehaviour
{
    public Player ArmyOnTile;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCombat(other.GetComponent<Player>(), ArmyOnTile);
        }
    }


    public void StartCombat(Player player1, Player player2)
    {
        GameManager.instance.SceneSwitchToCombat(player1, player2);
    }
}
