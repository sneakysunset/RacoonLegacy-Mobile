using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class RacoonFootPrints : MonoBehaviour
{
    public Transform[] printPos;
    public GameObject footPrint;
    private float timer;
    public float printFrequence;
    private int footIndex;
    Player player;
    [HideInInspector] public List<GameObject> footList;
    private void Start()
    {
        player = GetComponent<Player>();
        timer = printFrequence;
        footList = new List<GameObject>();
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if(timer <= 0 && player.isActivated)
        {
            timer = printFrequence;
            Transform t = Instantiate(footPrint, printPos[footIndex].position, transform.rotation).transform;
            footList.Add(t.gameObject);
            t.Rotate(0, 0, -90);
            footIndex++;
            footIndex %= 4;
        }
    }
}
