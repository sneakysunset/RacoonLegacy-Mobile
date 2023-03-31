using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Lean.Touch;
using UnityEngine.Events;
using Unity.VisualScripting;

public class RombaManager : MonoBehaviour
{
    public List<Romba> rombas;
    public List<GameObject> walls;
    [SerializeField] public List<RoombaPath> iterations;
    public rombaBehaviour romba;
    private int iterationIndex = 0;
    [SerializeField] public int pathNum;
    public RoombaPath roombaPathPrefab;
    public float resetPlayerMovementSpeed;
    /*[HideInInspector]*/ public bool startIteration;
    public UnityEvent iterationPause;
    public UnityEvent iterationStart;
    public UnityEvent iterationGameOver;
    private void Start()
    {
        rombas = new List<Romba>();
        for (int i = 0; i < iterations.Count; i++)
        {
            RoombaPath temp = iterations[i];
            int randomIndex = Random.Range(i, iterations.Count);
            iterations[i] = iterations[randomIndex];
            iterations[randomIndex] = temp;
        }
    }

    public void AddNewRomba(Romba r, Player player)
    {
        r.romb = Instantiate (romba, r.position, Quaternion.identity);
        r.romb.transform.right = r.direction;
        r.romb.romba = r;
        r.romb.rMan = this;
        r.romb.rb = r.romb.transform.GetComponent<Rigidbody2D>();
        rombas.Add(r);
        r.romb.paths = player.paths ;
        

       
        StartCoroutine(StartIteration(player, true));
    }

    public IEnumerator StartIteration(Player player, bool newIteration)
    {
        if (newIteration)
        {
            iterationIndex++;
        }
        
        foreach (Romba r2 in rombas)
        {
            r2.romb.OnEndIteration();
        }

        player.target.transform.position = iterations[iterationIndex].destination;
        player.target.gameObject.SetActive(false);
        player.transform.right = iterations[iterationIndex].transform.position - player.transform.position;
        player.isActivated = false;
        player.racoonCol.isTrigger = true;
        for (int i = walls.Count - 1; i >= 0; i--)
        {
            Destroy(walls[i].gameObject);
            walls.RemoveAt(i);
        }

        FMODUnity.RuntimeManager.PlayOneShot("event:/Entity/Racoon_Noise");
        while (player.transform.position != iterations[iterationIndex].transform.position)
        {
            player.transform.position = Vector3.MoveTowards(player.transform.position, iterations[iterationIndex].transform.position, resetPlayerMovementSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitUntil(() => player.transform.position == iterations[iterationIndex].transform.position);
        iterationPause?.Invoke();
        if (!newIteration) iterationGameOver?.Invoke();
        player.transform.position = iterations[iterationIndex].transform.position;
        player.target.gameObject.SetActive(true);

        foreach (Romba r2 in rombas)
        {
            r2.romb.OnStartIteration(r2);

        }
        startIteration = false;
        player.transform.right = iterations[iterationIndex].destination - player.transform.position;
        yield return new WaitUntil(() => startIteration == true);
        yield return new WaitForSeconds(.2f);
        iterationStart?.Invoke();
        foreach (Romba r2 in rombas)
        {
            r2.romb.isActivated = true;
            r2.romb.OnStartLoop(r2);
        }
        player.racoonCol.isTrigger = false;
        walls.Clear();

        player.OnNewIteration(newIteration);
    }

    private void OnEnable()
    {
        Lean.Touch.LeanTouch.OnFingerDown += LeanTouch_OnFingerDown; ;
    }

    private void LeanTouch_OnFingerDown(LeanFinger obj)
    {
        if (!startIteration)
        {
            startIteration = true;
        }
    }

    private void OnDisable()
    {
        Lean.Touch.LeanTouch.OnFingerDown -= LeanTouch_OnFingerDown; ;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(RombaManager))]
public class RombaManagerEditor : Editor
{
    RombaManager rombaMan;
    private void OnSceneGUI()
    {
        if (!Application.isPlaying)
        {
            Draw();
            EditorUtility.SetDirty(rombaMan);
        }
    }

    void Draw()
    {
        if(rombaMan.iterations.Count != rombaMan.pathNum)
        {
            CreatePaths();
        }

        if(rombaMan.iterations.Count > 0)
        {
            foreach(var path in rombaMan.iterations)
            {
                UpdatePaths(path);
            }
        }
    }

    void CreatePaths()
    {
        if (rombaMan.pathNum > rombaMan.iterations.Count && rombaMan.pathNum >= 0)
        {
            while(rombaMan.pathNum > rombaMan.iterations.Count)
            {

                RoombaPath rP = PrefabUtility.InstantiatePrefab(rombaMan.roombaPathPrefab, rombaMan.transform) as RoombaPath;
                rombaMan.iterations.Add(rP);
            }
        }
        else if(rombaMan.pathNum < rombaMan.iterations.Count)
        {
            while(rombaMan.pathNum < rombaMan.iterations.Count)
            {
                RoombaPath rP = rombaMan.iterations[rombaMan.iterations.Count - 1];
                rombaMan.iterations.RemoveAt(rombaMan.iterations.Count - 1);

                DestroyImmediate(rP.gameObject);
            }
        }
    }



    void UpdatePaths(RoombaPath roombaPath)
    {
        MoveHandlesAlongObject(roombaPath);
        Handles.color = Color.black;
        Handles.DrawLine(roombaPath.destination, roombaPath.transform.position, 5f);

        Handles.color = Color.red;
        Vector3 newPosA = Handles.FreeMoveHandle(roombaPath.destination, Quaternion.identity, .5f, Vector3.zero, Handles.CylinderHandleCap);
        if (roombaPath.destination != newPosA)
        {
            Undo.RecordObject(roombaPath, "MovePoint");
            MovePoint(roombaPath, true, newPosA, roombaPath.transform.position);
        }
    }

    public void MovePoint(RoombaPath roombaPath, bool pA, Vector3 pos, Vector3 center)
    {
        var a = roombaPath.destination;
        roombaPath.destination = new Vector3(pos.x, pos.y, 0);
    }

    private void OnEnable()
    {
        rombaMan = (RombaManager)target;
    }

    void MoveHandlesAlongObject(RoombaPath roombaPath)
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



