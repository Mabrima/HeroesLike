using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHandler : MonoBehaviour
{
    public UnitBase unitBase;
    // Start is called before the first frame update
    void Start()
    {
    }

    public void Move(int x, int y)
    {
        FieldHandler.instance.RemoveUnitFromTile(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
        transform.position = new Vector3(x, 1, y);
        FieldHandler.instance.PutUnitOnTile(x, y);
        FieldHandler.instance.GetAvailableTiles(x, y, unitBase.speed);
    }

    /*
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FieldHandler.instance.GetAvailableTiles(0, 0, 5);
        }
    } */

}
