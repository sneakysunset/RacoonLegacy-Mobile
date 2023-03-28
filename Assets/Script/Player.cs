using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.AdaptivePerformance.VisualScripting;
using UnityEngine.Events;



public struct directionValue
{
    public float time;
    public Vector2 direction;
    public Vector2 position;
}

public struct Romba
{
    public Vector2 startPosition;
    public List<directionValue> directions;
    public rombaBehaviour romb;
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
        romba.directions = new List<directionValue>();
        romba.startPosition = transform.position;
        romba.directions.Add(AddDirectionPoint(0, transform.right));
        OnIterationOver.AddListener(() => FindObjectOfType<RombaManager>().AddNewRomba(romba, this));
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
            romba.directions.Add(AddDirectionPoint(timer, transform.right));
        }
        else if (collision.collider.CompareTag("Target"))
        {
            romba.directions.Add(AddDirectionPoint(timer, transform.right));
            OnIterationOver?.Invoke();
        }
        else if (collision.collider.CompareTag("Romba"))
        {
            OnGameOver?.Invoke();
        }
    }

    public directionValue AddDirectionPoint(float time, Vector2 direction)
    {
        var d = new directionValue();
        d.time = timer;
        d.direction = direction;
        d.position = transform.position;
        timer = 0;
        return d;
    }
}
