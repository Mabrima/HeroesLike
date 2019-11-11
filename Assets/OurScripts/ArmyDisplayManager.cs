using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArmyDisplayManager : MonoBehaviour
{
    Vector3 targetPosition = new Vector3(Screen.width, Screen.height, 0);
    Vector3 originalPosition;

    public float speed;
    public float scaleSpeed;

    bool showInfo = false;
    bool isMoving;
    bool positionSaved = false;
    GameObject activeUnit;

    public Player player;

    public List<GameObject> unitHolders = new List<GameObject>(); 



    public void ShowUnits()
    {
        for (int i = 0; i < player.units.Count; i++)
        {
            foreach (GameObject unitHolder in unitHolders)
            {
                if (unitHolder.GetComponent<EmptyOrFull>().empty == true)
                {
                    unitHolder.GetComponent<EmptyOrFull>().empty = false;
                    unitHolder.transform.Find("Image").GetComponent<Image>().sprite = player.units[i].unitBase.sprite;
                    unitHolder.transform.Find("Amount").GetComponent<Text>().text = player.units[i].amountOfUnits.ToString();
                    Transform unitChild = unitHolder.transform.Find("Stats").GetComponent<Transform>();
                    SetStats(unitChild, i);
                    break;
                }
                
            }
        }
        
    }

    public void UpdateArmy() //Set all holders bool to true so we update them next time we open window.
    {
        foreach (GameObject unitHolder in unitHolders)
        {
            unitHolder.GetComponent<EmptyOrFull>().empty = true;
        }
    }


    public void SetStats(Transform unitChild, int unit)
    {
        unitChild.Find("Name").GetComponent<Text>().text = player.units[unit].unitBase.name;
        unitChild.Find("Health").GetComponent<Text>().text = "Health: " + player.units[unit].unitBase.baseHealth.ToString();
        unitChild.Find("Damage").GetComponent<Text>().text = "Damage: " + player.units[unit].unitBase.minDamage + "-" + player.units[unit].unitBase.maxDamage;
        unitChild.Find("Speed").GetComponent<Text>().text = "Speed: " + player.units[unit].unitBase.speed;
        unitChild.Find("Initiative").GetComponent<Text>().text = "Initiative: " + player.units[unit].unitBase.initiative;
        unitChild.Find("Attack").GetComponent<Text>().text = "Attack: " + player.units[unit].unitBase.attack;
        unitChild.Find("Defence").GetComponent<Text>().text = "Defence: " + player.units[unit].unitBase.defence;
    }

    public void ShowUnitInformation(GameObject unit) //Runs through event trigger in inspector.
    {
        if (isMoving == false)
        {
            isMoving = true;
            StartCoroutine(MoveDisplay(unit));
        }
        
    }

    public 

    IEnumerator MoveDisplay(GameObject unit)
    {
        if (positionSaved == false) //Save the original position the first time its clicked.
        {
            activeUnit = unit.gameObject; //Link activeUnit variable to the gameobject we currently are showing.
            originalPosition = activeUnit.transform.position;
            positionSaved = true; //Change bool so we dont save other units position if we click on them.
        }

        if (activeUnit != unit.gameObject) //Stop courotine if we didnt click on same unit again. 
        {
            iTween.ScaleTo(activeUnit, new Vector3(1, 1, 1), scaleSpeed);
            iTween.MoveTo(activeUnit, iTween.Hash("position", originalPosition, "time", speed, "easetype", iTween.EaseType.easeInSine));
            activeUnit.transform.Find("Stats").gameObject.SetActive(false);

            activeUnit = unit.gameObject;
            originalPosition = unit.transform.position;
            isMoving = false; //Turn bool off so we can click again and see if next unit is same or different again.
            positionSaved = true;
            

            iTween.MoveTo(activeUnit, iTween.Hash("position", targetPosition / 2, "time", speed, "easetype", iTween.EaseType.easeInSine));
            iTween.ScaleAdd(activeUnit, new Vector3(3, 3, 3), scaleSpeed);
            activeUnit.transform.Find("Stats").gameObject.SetActive(true);
            showInfo = true;
            yield break;
        }

        if (showInfo == true) //Move back to position.
        {
            unit.transform.Find("Stats").gameObject.SetActive(false);
            iTween.ScaleTo(activeUnit, new Vector3(1, 1, 1), scaleSpeed);
            iTween.MoveTo(activeUnit, iTween.Hash("position", originalPosition, "time", speed, "easetype", iTween.EaseType.easeInSine));
            showInfo = false;
            positionSaved = false; //Turn bool off when we have moved the unit so we can save the next units position.
            yield return new WaitForSeconds(1f);
            isMoving = false;
        }
        else if (showInfo == false) //Move to middle of screen
        {

            unit.transform.Find("Stats").gameObject.SetActive(true);
            showInfo = true;
            iTween.MoveTo(activeUnit, iTween.Hash("position", targetPosition / 2, "time", speed, "easetype", iTween.EaseType.easeInSine));
            iTween.ScaleAdd(activeUnit, new Vector3(3, 3, 3), scaleSpeed);
            yield return new WaitForSeconds(1f);
            isMoving = false;
        }
        
    }



}
