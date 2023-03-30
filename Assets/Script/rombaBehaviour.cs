using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[System.Serializable]
public struct path
{
    public float time;
    public Vector2 pathPoint;
}

public class rombaBehaviour : MonoBehaviour
{
    public Romba romba;
    [HideInInspector] public Rigidbody2D rb;
    float timer;
    public bool isActivated = true;
    [HideInInspector] public RombaManager rMan;
    [HideInInspector] public List<path> paths;
    [HideInInspector] public int pathIndex;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if(timer > paths[pathIndex + 1].time)
        {
            if(pathIndex >= paths.Count - 2)
            {
                isActivated = false;
                timer = 0;
                pathIndex = 0;
            }
            else
            {
                pathIndex++;
                timer = 0;
            }
           
        }
        if(isActivated)
        {
            timer += Time.deltaTime;
            transform.position = Vector2.Lerp(paths[pathIndex].pathPoint, paths[pathIndex + 1].pathPoint, timer / paths[pathIndex + 1].time);
        }
        else
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall") || collision.collider.CompareTag("Romba"))
        {
            transform.right = Vector2.Reflect(transform.right, collision.contacts[0].normal);

        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("OldWall"))
        {
            rMan.walls.Remove(this.gameObject);
            Destroy(other.gameObject);
        }
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
