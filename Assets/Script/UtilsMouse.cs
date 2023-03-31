using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilsMouse
{
    public static Vector3 GetMousePosition(Vector2 touchPos, LayerMask layer)
    {
        Ray ray = Camera.main.ScreenPointToRay(touchPos);
        if(Physics.Raycast(ray, out RaycastHit hit,Mathf.Infinity, layer))
        {
            Vector3 pos = new Vector3(hit.point.x, hit.point.y, 0);
            return pos;
        }
        else return Vector3.zero;
    }
}
