using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Iteration
{
    public Transform startPos;
    public Transform Target;
}

public class RombaManager : MonoBehaviour
{
    public List<Romba> rombas;
    public List<Iteration> iterations;
    public rombaBehaviour romba;
    private int iterationIndex = 0;
    private void Start()
    {
        rombas = new List<Romba>();
    }

    public void AddNewRomba(Romba r, Player player)
    {
        r.romb = Instantiate (romba, r.startPosition, Quaternion.identity);
        r.romb.transform.right = r.directions[0].direction;
        r.romb.romba = r;
        rombas.Add(r);
        
        StartIteration(player);
    }

    void StartIteration(Player player)
    {
        iterationIndex++;
        foreach(var r in iterations)
        {
            r.Target.gameObject.SetActive(false);
        }
        player.transform.position = iterations[iterationIndex].startPos.position;
        player.target.position = iterations[iterationIndex].Target.position;
        iterations[iterationIndex].Target.gameObject.SetActive(true);

        foreach (Romba r2 in rombas)
        {
            r2.romb.transform.position = r2.startPosition;
            r2.romb.transform.right = r2.directions[0].direction;
            r2.romb.gameObject.SetActive(true);
        }
    }
}
