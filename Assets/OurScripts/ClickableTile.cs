using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableTile : PathfindingTile
{

    private void Start()
    {
        base.Initiate();
        defaultMaterial = rend.material;
        player = FindObjectOfType<Player>();
        sphereRend = transform.Find("Sphere").GetComponent<MeshRenderer>();

    }

    private void OnMouseUp()
    {
        Debug.Log("On Click: " + player.isMoving);
        if (player.isMoving == true)
        {
            Debug.Log("hej");
            return;
        }

        if (!Pathfinding.INSTANCE.ChoseNextTile(this))
        {
            map.FindPath(tileX, tileY, this);
        }
           
    }

    private void OnMouseEnter()
    {
        rend.material = mouseOverMaterial;
    }

    private void OnMouseExit()
    {
        rend.material = defaultMaterial;
    }


}
