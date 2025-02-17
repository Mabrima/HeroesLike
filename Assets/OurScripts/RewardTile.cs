﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardTile : MonoBehaviour
{
    public List<int> unitAmounts;
    public List<UnitBase> rewardUnits = new List<UnitBase>();

    public Player player;

    public int tileX;
    public int tileZ;

    public RectTransform addUnitWindow;
    int randomNumber = 999;
    private Text text;

    public bool hasBeenUsed = false;

    public new Light light;

    private void Start()
    {
        text = addUnitWindow.Find("Text").GetComponent<Text>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && hasBeenUsed != true)
        {
            openWindow();
        }
    }

    public void openWindow()
    {
        player.allowedToMove = false;
        addUnitWindow.gameObject.SetActive(true);
        if (randomNumber == 999)
            NewRandom();
        text.text = "A group of " + unitAmounts[randomNumber].ToString() + ", " + rewardUnits[randomNumber].name + "s" + " wants to join your cause!";
        
    }

    public void NewRandom()
    {
        randomNumber = Random.Range(0, rewardUnits.Count);
    }

    public void AddUnitsToPlayer()
    {
        player.allowedToMove = true;
        //Called through YES-Button
        addUnitWindow.gameObject.SetActive(false);
        //TODO: needs to be reworked into a function on the player that checks if the player already has the unit and then adds the unit amounts to that unit. 
        //Or adds a new unit if it's not already existing, or does nothing if the players army is full

        player.AddToPlayerArmy(rewardUnits[randomNumber], unitAmounts[randomNumber]);
        hasBeenUsed = true;
        light.gameObject.SetActive(false);
        tileX = GetComponent<ClickableTile>().tileX;
        tileZ = GetComponent<ClickableTile>().tileZ;
        GameManager.info.rewardTiles.Add(this);
    }
}
