using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatStarter : MonoBehaviour
{
    public Player ArmyOnTile;
    public int tileX;
    public int tileZ;
    public bool hasBeenUsed = false;
    public GameObject visualUnit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && hasBeenUsed != true)
        {
            StartCombat(other.GetComponent<Player>(), ArmyOnTile);
        }
    }


    public void StartCombat(Player player1, Player player2)
    {
        tileX = GetComponent<ClickableTile>().tileX;
        tileZ = GetComponent<ClickableTile>().tileZ;
        hasBeenUsed = true;
        GameManager.info.combatTiles.Add(this);
        GameManager.instance.SceneSwitchToCombat(player1, player2, player1.transform.position);
    }
}
