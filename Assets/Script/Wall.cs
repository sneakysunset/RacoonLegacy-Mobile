using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;
using System.Reflection;

public class Wall : MonoBehaviour
{
    EdgeCollider2D eC;
    LineRenderer lR;
    WallBehavious wB;
    Vector3 pos1, pos2;
    LeanFinger _finger;
    Vector2[] eCPoints;
    public GameObject wallPrefab;
    private bool walling;
    private RombaManager rMan;
    private WaitForFixedUpdate waiter;
    public float minDistanceToCreateWall = 1;
    public LayerMask playerLayer;
    public LayerMask touchLayer;
    private void Start()
    {
        waiter = new WaitForFixedUpdate();
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
        finger = null;
        walling = false;
        if (Vector2.Distance(eCPoints[0], eCPoints[1]) < minDistanceToCreateWall)
        {
            rMan.walls.Remove(lR.gameObject);
            Destroy(lR.gameObject);
            return;
        }
        StartCoroutine(afterPhysics());
    }

    private void LeanTouch_OnFingerDown(LeanFinger finger)
    {
        if (!walling)
        {
            GameObject wall = Instantiate(wallPrefab, transform.position, Quaternion.identity);
            rMan.walls.Add(wall);
            eC = wall.GetComponent<EdgeCollider2D>();
            lR = wall.GetComponent<LineRenderer>();
            wB = wall.GetComponent<WallBehavious>();
            lR.enabled = true;
            lR.positionCount = 2;
            pos1 = UtilsMouse.GetMousePosition(finger.StartScreenPosition, touchLayer);
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
        if (_finger != null && walling)
        {
            pos2 = UtilsMouse.GetMousePosition(_finger.ScreenPosition, touchLayer);
            pos1.z = 0;
            if (lR)
            {
                lR.SetPosition(1, pos2);
                eCPoints[1] = pos2;
            }


        }
    }

    IEnumerator afterPhysics()
    {
        yield return waiter;
        while (CheckIfRacoonOverlap())
        {
            yield return new WaitForEndOfFrame();
        }

        eC.points = eCPoints;
        eC.enabled = true;
    }

    private bool CheckIfRacoonOverlap()
    {
        if (Physics2D.CircleCast(eCPoints[0], eC.edgeRadius, eCPoints[1] - eCPoints[0], Vector2.Distance(eCPoints[0], eCPoints[1]), playerLayer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
