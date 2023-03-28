using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Playables;
using UnityEngine;

[System.Serializable]

public class RombaManager : MonoBehaviour
{
    public List<Romba> rombas;
    [SerializeField] public List<RoombaPath> iterations;
    public rombaBehaviour romba;
    private int iterationIndex = 0;
    [SerializeField] public int pathNum;
    public RoombaPath roombaPathPrefab;
    private void Start()
    {
        rombas = new List<Romba>();
    }

    public void AddNewRomba(Romba r, Player player)
    {
        r.romb = Instantiate (romba, r.position, Quaternion.identity);
        r.romb.transform.right = r.direction;
        r.romb.romba = r;
        rombas.Add(r);
        
        StartIteration(player);
    }

    void StartIteration(Player player)
    {
        iterationIndex++;
        player.transform.position = iterations[iterationIndex].transform.position;
        player.target.position = iterations[iterationIndex].destination;

        foreach (Romba r2 in rombas)
        {
            r2.romb.transform.position = r2.position;
            r2.romb.transform.right = r2.direction;
            r2.romb.gameObject.SetActive(true);
        }
        player.OnNewIteration();
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

    private void OnEnable()
    {
        rombaMan = (RombaManager)target;
    }


}

#endif



