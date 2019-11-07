using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardTile : MonoBehaviour
{
    public List<int> unitAmounts;
    public List<UnitHandler> rewardUnits = new List<UnitHandler>();

    public RectTransform addUnitWindow;
    private Text text;

    private void Start()
    {
        text = addUnitWindow.Find("Text").GetComponent<Text>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            openWindow();
        }
    }

    public void openWindow()
    {
        addUnitWindow.gameObject.SetActive(true);
        text.text = "A group of " + unitAmounts[0].ToString() + ", " + rewardUnits[0].name + "s" + " wants to join your cause!";
    }

    public void AddUnitsToPlayer()
    {

        //Called through YES-Button
    }
}
