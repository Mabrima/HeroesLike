using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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


    private void Start()
    {
        
    }
    public void ShowUnitInformation(GameObject unit)
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

            activeUnit = unit.gameObject;
            originalPosition = unit.transform.position;
            isMoving = false; //Turn bool off so we can click again and see if next unit is same or different again.
            positionSaved = true;
            

            iTween.MoveTo(activeUnit, iTween.Hash("position", targetPosition / 2, "time", speed, "easetype", iTween.EaseType.easeInSine));
            iTween.ScaleAdd(activeUnit, new Vector3(3, 3, 3), scaleSpeed);
            showInfo = true;
            yield break;
        }

        if (showInfo == true) //Move back to position.
        {
            iTween.ScaleTo(activeUnit, new Vector3(1, 1, 1), scaleSpeed);
            iTween.MoveTo(activeUnit, iTween.Hash("position", originalPosition, "time", speed, "easetype", iTween.EaseType.easeInSine));
            showInfo = false;
            positionSaved = false; //Turn bool off when we have moved the unit so we can save the next units position.
            yield return new WaitForSeconds(2f);
            isMoving = false;
        }
        else if (showInfo == false) //Move to middle of screen
        {
            showInfo = true;
            iTween.MoveTo(activeUnit, iTween.Hash("position", targetPosition / 2, "time", speed, "easetype", iTween.EaseType.easeInSine));
            iTween.ScaleAdd(activeUnit, new Vector3(3, 3, 3), scaleSpeed);
            yield return new WaitForSeconds(2f);
            isMoving = false;
        }
        
    }



}
