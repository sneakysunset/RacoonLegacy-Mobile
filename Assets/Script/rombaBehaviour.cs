using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class rombaBehaviour : MonoBehaviour
{
    public Romba romba;
    private int index;
    Rigidbody2D rb;
    public float speed = 5;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        rb.velocity = transform.right * speed * Time.deltaTime;
        rb.angularVelocity = 0; 
    }

    /*    private void Update()
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
        }*/
}
