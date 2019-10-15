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

    public override void OnMouseUp()
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

    public override void OnMouseEnter()
    {
        rend.material = mouseOverMaterial;
    }

    public override void OnMouseExit()
    {
        rend.material = defaultMaterial;
    }


}
