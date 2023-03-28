using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.AdaptivePerformance.VisualScripting;
using UnityEngine.Events;

public struct Romba
{
    public rombaBehaviour romb;
    public Vector2 direction;
    public Vector2 position;
    public float speed;
}

public class Player : MonoBehaviour
{
    public Transform target;
    private Rigidbody2D rb;
    public float speed;
    public UnityEvent OnIterationOver;
    public UnityEvent OnGameOver;
    public Romba romba;
    private float timer = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        transform.right = (target.position - transform.position).normalized;
        romba = new Romba();
        romba.position = transform.position;
        romba.direction = transform.right;
        OnIterationOver.AddListener(() => FindObjectOfType<RombaManager>().AddNewRomba(romba, this));
    }
    
    public void OnNewIteration()
    {
        transform.right = (target.position - transform.position).normalized;
        romba = new Romba();
        romba.position = transform.position;
        romba.direction = transform.right;
    }

    private void Update()
    {
        timer += Time.deltaTime;
    }


    void FixedUpdate()
    {
        rb.velocity = transform.right * speed * Time.deltaTime ;
        rb.angularVelocity = 0;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            transform.right = Vector2.Reflect(transform.right, collision.contacts[0].normal);
            romba.direction = transform.right;
        }
        else if (collision.collider.CompareTag("Target"))
        {
            OnIterationOver?.Invoke();
        }
        else if (collision.collider.CompareTag("Romba"))
        {
            OnGameOver?.Invoke();
        }
    }
}
