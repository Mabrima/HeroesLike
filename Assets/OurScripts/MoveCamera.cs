using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MoveCamera : MonoBehaviour
{
    public int boundary;
    public float cameraMoveSpeed;

    private int screenWidth;
    private int screenHeight;
    private Rect screenArea;


    public float minZoom = 15f;
    public float maxZoom = 90f;

    public float zoomSensitivity = 10f;

    Camera mainCamera;

    Vector3 cameraOffset = new Vector3(0, -5, -10);

    public Player player;
    // Start is called before the first frame update
    void Start()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        screenArea = new Rect(0, 0, screenWidth, screenHeight);
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (screenArea.Contains(Input.mousePosition)) //If our mouse is in our "Screen Area".
        {
            if (Input.mousePosition.x > screenWidth - boundary && transform.position.x < TileMap.INSTANCE.mapSizeX - 1)
            {
                //RIGHT
                transform.position += new Vector3(cameraMoveSpeed, 0, 0) * Time.deltaTime;
            }

            if (Input.mousePosition.x < 0 + boundary && transform.position.x > TileMap.INSTANCE.clickableTiles[0, 0].tileX)
            {
                //LEFT
                transform.position -= new Vector3(cameraMoveSpeed, 0, 0) * Time.deltaTime;
            }

            if (Input.mousePosition.y > screenHeight - boundary && transform.position.y < TileMap.INSTANCE.mapSizeY / 2)
            {
                //UP
                transform.position += new Vector3(0, cameraMoveSpeed, 0) * Time.deltaTime;
            }

            if (Input.mousePosition.y < 0 + boundary && transform.position.y > TileMap.INSTANCE.clickableTiles[0, 0].tileY - (TileMap.INSTANCE.mapSizeY / 4))
            {
                //DOWN
                transform.position -= new Vector3(0, cameraMoveSpeed, 0) * Time.deltaTime;
            }

            float fov = mainCamera.fieldOfView;
            fov -= Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity;
            fov = Mathf.Clamp(fov, minZoom, maxZoom);
            mainCamera.fieldOfView = fov;
        }

        if (Input.GetButton("Jump"))
        {
            transform.position = player.transform.position + cameraOffset;
        }

        
    }
}
