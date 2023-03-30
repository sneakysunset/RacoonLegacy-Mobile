using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenBounds : MonoBehaviour
{
    public Transform bottomLeft;
    public Transform topRight;

    private void Start()
    {
        SetUpBoundaries();
    }

    private void SetUpBoundaries()
    {
        Vector3 point = new Vector3();

        //topright
        point = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        point.z = 0;
        topRight.position = point;

        //bottomleft
        point = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        point.z = 0;
        bottomLeft.position = point;
    }
}
