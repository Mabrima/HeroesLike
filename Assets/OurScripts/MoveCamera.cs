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

    public float dragSpeed = 0.5f;
    private Vector3 dragOrigin;

    public float minZoom = 15f;
    public float maxZoom = 90f;

    public float zoomSensitivity = 10f;

    Camera mainCamera;

    Vector3 cameraOffset = new Vector3(0, 10, -7.5f);

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

            if (Input.mousePosition.y > screenHeight - boundary && transform.position.z < TileMap.INSTANCE.mapSizeZ / 2)
            {
                //UP
                transform.position += new Vector3(0, 0, cameraMoveSpeed) * Time.deltaTime;
            }

            if (Input.mousePosition.y < 0 + boundary && transform.position.z > TileMap.INSTANCE.clickableTiles[0, 0].tileZ - (TileMap.INSTANCE.mapSizeZ / 4))
            {
                //DOWN
                transform.position -= new Vector3(0, 0, cameraMoveSpeed) * Time.deltaTime;
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

        if (Input.GetMouseButtonDown(2))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(2)) return;

        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        Vector3 move = new Vector3(-pos.x * dragSpeed, 0, -pos.y * dragSpeed);

        transform.Translate(move, Space.World);
    
    }
}
