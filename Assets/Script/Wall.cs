using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;
using System.Reflection;

public class Wall : MonoBehaviour
{
    EdgeCollider2D eC;
    LineRenderer lR;
    Vector3 pos1, pos2;
    LeanFinger _finger;
    Vector2[] eCPoints;
    public GameObject wallPrefab;
    private bool walling;
    private RombaManager rMan;
    private void Start()
    {

        eCPoints = new Vector2[2];
        rMan = FindObjectOfType<RombaManager>();
    }

    void OnEnable()
    {
        Lean.Touch.LeanTouch.OnFingerDown += LeanTouch_OnFingerDown;
        Lean.Touch.LeanTouch.OnFingerUp += LeanTouch_OnFingerUp;
    }

    private void LeanTouch_OnFingerUp(LeanFinger finger)
    {
        StartCoroutine(afterPhysics());
        walling = false;
    }

    private void LeanTouch_OnFingerDown(LeanFinger finger)
    {
        if(!walling)
        {
            GameObject wall = Instantiate(wallPrefab, transform.position, Quaternion.identity);
            rMan.walls.Add(wall);
            eC = wall.GetComponent<EdgeCollider2D>();
            lR = wall.GetComponent<LineRenderer>();
            lR.positionCount = 2;
            pos1 = Camera.main.ScreenToWorldPoint(finger.StartScreenPosition);
            pos1.z = 0;
            eCPoints[0] = pos1;
            lR.SetPosition(0, pos1);
        
            _finger = finger;
            walling = true;
        }
    }

    void OnDisable()
    {
        Lean.Touch.LeanTouch.OnFingerDown -= LeanTouch_OnFingerDown;
        Lean.Touch.LeanTouch.OnFingerUp -= LeanTouch_OnFingerUp;
    }
  

    private void FixedUpdate()
    {
        if(_finger != null && walling)
        {
            pos2 = pos2 = Camera.main.ScreenToWorldPoint(_finger.LastScreenPosition);
            pos2.z = 0;
            lR.SetPosition(1, pos2);
            eCPoints[1] = pos2;

        }
    }

    IEnumerator afterPhysics()
    {
        yield return new WaitForFixedUpdate();
        eC.points = eCPoints;
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(pos1, .5f);
        Gizmos.DrawSphere(pos2, .5f);
    }
}
