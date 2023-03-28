using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class RoombaPath : MonoBehaviour
{
    [HideInInspector] public Vector3 destination;
    [HideInInspector] public Vector3 originalPos;

    [HideInInspector] public Vector3 prevPos;
}
#if UNITY_EDITOR
[CustomEditor(typeof(RoombaPath))]
public class RoombaPathEditor : Editor
{
    RoombaPath roombaPath;

    private void OnSceneGUI()
    {
        if (!Application.isPlaying)
        {
            Draw();
            EditorUtility.SetDirty(roombaPath);
        }
    }

    void Draw()
    {
        MoveHandlesAlongObject();
        Handles.color = Color.black;
        Handles.DrawLine(roombaPath.destination, roombaPath.transform.position, 5f) ;

        Handles.color = Color.red;
        Vector3 newPosA = Handles.FreeMoveHandle(roombaPath.destination, Quaternion.identity, .5f, Vector3.zero, Handles.CylinderHandleCap);
        if (roombaPath.destination != newPosA)
        {
            Undo.RecordObject(roombaPath, "MovePoint");
            MovePoint(true, newPosA, roombaPath.transform.position);
        }
    }

    public void MovePoint(bool pA, Vector3 pos, Vector3 center)
    {
        var a = roombaPath.destination;
        roombaPath.destination = new Vector3(pos.x, pos.y, 0);
    }

    private void OnEnable()
    {
        roombaPath = (RoombaPath)target;
    }

    void MoveHandlesAlongObject()
    {
        if (roombaPath.prevPos != roombaPath.transform.position && !Application.isPlaying)
        {
            Vector3 movement = roombaPath.transform.position - roombaPath.prevPos;
            roombaPath.destination += movement;
            roombaPath.prevPos = roombaPath.transform.position;
        }
    }
}

#endif
