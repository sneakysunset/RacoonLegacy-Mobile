using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class rombaBehaviour : MonoBehaviour
{
    public Romba romba;
    private int index;
    [HideInInspector] public Rigidbody2D rb;
    public float speed = 5;
    public bool isActivated = true;
    [HideInInspector] public RombaManager rMan;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if(isActivated)
        {
            rb.velocity = transform.right * speed * Time.deltaTime;
            rb.angularVelocity = 0; 
        }
        else
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
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
