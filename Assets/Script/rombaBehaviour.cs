using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class rombaBehaviour : MonoBehaviour
{
    public Romba romba;
    private int index;
    Rigidbody rb;
    private float timer;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        timer = 0;
        foreach (var direction in romba.directions)
        { print(direction.time + " " + direction.position); }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if(index < romba.directions.Count - 1)
        {
            transform.position = Vector2.Lerp(romba.directions[index].position, romba.directions[index + 1].position, timer /romba.directions[index + 1].time);
            if(timer >= romba.directions[index + 1].time)
            {
                index++;
                timer = 0;
            }
        }
    }
}
