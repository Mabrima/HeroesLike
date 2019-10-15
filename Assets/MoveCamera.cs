using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MoveCamera : MonoBehaviour
{
    public int boundary = 25;
    public float speed;

    private int screenWidth;
    private int screenHeight;

    Vector3 cameraOffset = new Vector3(0, -5, -10);

    public Player player;
    // Start is called before the first frame update
    void Start()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.mousePosition.x > screenWidth - boundary && transform.position.x < TileMap.INSTANCE.mapSizeX-1)
        {
           transform.position += new Vector3(speed, 0, 0) * Time.deltaTime;
        }

        if (Input.mousePosition.x < 0 + boundary && transform.position.x > TileMap.INSTANCE.clickableTiles[0, 0].tileX)
        {
            transform.position -= new Vector3(speed, 0, 0) * Time.deltaTime;
        }

        if (Input.mousePosition.y > screenHeight - boundary && transform.position.y < TileMap.INSTANCE.mapSizeY / 2)
        {
            transform.position += new Vector3(0, speed, 0) * Time.deltaTime;
        }

        if (Input.mousePosition.y < 0 + boundary && transform.position.y > TileMap.INSTANCE.clickableTiles[0, 0].tileY - (TileMap.INSTANCE.mapSizeY / 4))
        {
            transform.position -= new Vector3(0, speed, 0) * Time.deltaTime;
        }

        if (Input.GetButton("Jump"))
        {
            transform.position = player.transform.position + cameraOffset;
        }
    }
}
