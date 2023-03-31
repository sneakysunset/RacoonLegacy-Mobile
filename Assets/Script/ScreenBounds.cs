using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenBounds : MonoBehaviour
{
    public Transform bottomLeft;
    public Transform topRight;
    public LayerMask touchLayer;
    private void Start()
    {
        SetUpBoundaries();
    }

    private void SetUpBoundaries()
    {
        Vector3 point = new Vector3();

        //topright
        point = UtilsMouse.GetMousePosition(new Vector2(Screen.width, Screen.height), touchLayer);
        topRight.position = point;

        //bottomleft
        point = UtilsMouse.GetMousePosition(new Vector2(0, 0), touchLayer);
        bottomLeft.position = point;
    }
}
