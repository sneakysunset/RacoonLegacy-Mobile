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
    [HideInInspector] public Rigidbody2D rb;
    public float speed;
    public UnityEvent OnIterationOver;
    public UnityEvent OnGameOver;
    public Romba romba;
    public bool isActivated = true;
    private RombaManager rMan;
    [HideInInspector] public Animator anim;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        transform.right = (target.position - transform.position).normalized;
        romba = new Romba();
        romba.position = transform.position;
        romba.direction = transform.right;
        rMan = FindObjectOfType<RombaManager>();
        OnIterationOver.AddListener(() => rMan.AddNewRomba(romba, this));
        anim = GetComponentInChildren<Animator>();
    }

    public void OnNewIteration()
    {
        isActivated = true;
        transform.right = (target.position - transform.position).normalized;
        romba = new Romba();
        romba.position = transform.position;
        romba.direction = transform.right;
    }


    void FixedUpdate()
    {
        if (isActivated)
        {
            anim.Play("Walk", 0);
            rb.velocity = transform.right * speed * Time.deltaTime;
            rb.angularVelocity = 0;
        }
        else
        {
            if (rMan.startIteration)
            {
                anim.Play("Run", 0);
            }
            else
            {
                anim.Play("Idle", 0);
            }
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall") || collision.collider.CompareTag("OldWall"))
        {
            transform.right = Vector2.Reflect(transform.right, collision.contacts[0].normal);
            
        }
        else if (collision.collider.CompareTag("Target"))
        {
            OnIterationOver?.Invoke();
        }
        else if (collision.collider.CompareTag("Romba"))
        {
            OnGameOver?.Invoke();
            StartCoroutine(rMan.StartIteration(this, false));
        }
    }
}
