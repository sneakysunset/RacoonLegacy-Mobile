using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.AdaptivePerformance.VisualScripting;
using UnityEngine.Events;
using Lean.Touch;
using Lofelt.NiceVibrations;

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
    [HideInInspector] public List<path> paths;
    float timer;
    [HideInInspector] public Collider2D racoonCol;
    public float minTimerSound, maxTimerSound;
    private float timerSound;
    bool startGame;
    private RacoonFootPrints rFP;
    public UnityEvent StartGameEvent;
    public float levelTimer = 40;
    float maxTimer;
    [HideInInspector] public int iterationNumber = 0;
    void Start()
    {
        maxTimer = levelTimer;
        paths = new List<path>();
        rb = GetComponent<Rigidbody2D>();
        transform.right = (target.position - transform.position).normalized;
        romba = new Romba();
        romba.position = transform.position;
        romba.direction = transform.right;
        rMan = FindObjectOfType<RombaManager>();
        OnIterationOver.AddListener(() => rMan.AddNewRomba(romba, this));
        anim = GetComponentInChildren<Animator>();
        var p = new path();
        p.pathPoint = (Vector2)transform.position;
        p.time = 0;
        paths.Add(p);
        racoonCol = GetComponent<Collider2D>();
        timerSound = Random.Range(minTimerSound, maxTimerSound);
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/Level_Start");
        rFP = GetComponent<RacoonFootPrints>();
    }

    private void OnEnable()
    {
        LeanTouch.OnFingerDown += LeanTouch_OnFingerDown;
    }

    private void LeanTouch_OnFingerDown(LeanFinger obj)
    {
        if (!startGame)
        {
            startGame = true;
            isActivated = true;
            rMan.startIteration = true;
            StartGameEvent?.Invoke();
        }
    }

    private void OnDisable()
    {
        LeanTouch.OnFingerDown -= LeanTouch_OnFingerDown;
    }

    public void OnNewIteration(bool newIteration)
    {
        levelTimer = maxTimer;
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/Level_Start");
        isActivated = true;
        transform.right = (target.position - transform.position).normalized;
        romba = new Romba();
        romba.position = transform.position;
        romba.direction = transform.right;
        timer = 0;
        paths = new List<path>();
        var p = new path();
        p.pathPoint = (Vector2)transform.position;
        p.time = 0;
        paths.Add(p);
        timerSound = Random.Range(minTimerSound, maxTimerSound);
        
        if(!newIteration)
        {
            int listLenght = rFP.footList.Count - 1;
            for (int i = listLenght; i >= 0; i--)
            {
                Destroy(rFP.footList[i]);
            }
            rFP.footList.Clear();
        }
        else
        {
            rFP.footList.Clear();
            iterationNumber++;
        }
    }


    void FixedUpdate()
    {
        timer += Time.deltaTime;
        if (isActivated)
        {
            levelTimer -= Time.deltaTime;
            if(levelTimer < 0)
            {
                OnGameOver?.Invoke();
                StartCoroutine(rMan.StartIteration(this, false));
                FMODUnity.RuntimeManager.PlayOneShot("event:/Entity/Racoon_Dead");

                FMODUnity.RuntimeManager.PlayOneShot("event:/UI/Level_Lose");
                Vibrator.Vibrate(600);
            }
            timerSound -= Time.deltaTime;
            anim.Play("Walk", 0);
            rb.velocity = transform.right * speed * Time.deltaTime;
            rb.angularVelocity = 0;
            if(timerSound <= 0)
            {
              FMODUnity.RuntimeManager.PlayOneShot("event:/Entity/Racoon_Sad");
              timerSound = Random.Range(minTimerSound, maxTimerSound); ;
            }
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
            var p = new path();
            p.pathPoint = transform.position;
            p.time = timer;
            paths.Add(p);
            timer = 0;
            FMODUnity.RuntimeManager.PlayOneShot("event:/Wall/Bounce_Wall");
            FMODUnity.RuntimeManager.PlayOneShot("event:/Entity/Racoon_Hit");
            Vibrator.Vibrate();
            
        }
        else if (collision.collider.CompareTag("Target"))
        {
            var p = new path();
            p.pathPoint = transform.position;
            p.time = timer;
            paths.Add(p);
            timer = 0;
            OnIterationOver?.Invoke();
            FMODUnity.RuntimeManager.PlayOneShot("event:/UI/Level_Victory");
            FMODUnity.RuntimeManager.PlayOneShot("event:/Entity/Racoon_Happy");
            Vibrator.Vibrate(500);
        }
        else if (collision.collider.CompareTag("Romba"))
        {
            OnGameOver?.Invoke();
            StartCoroutine(rMan.StartIteration(this, false));
            FMODUnity.RuntimeManager.PlayOneShot("event:/Entity/Racoon_Dead");
            FMODUnity.RuntimeManager.PlayOneShot("event:/Entity/Entity_Racoon_Collide");

            FMODUnity.RuntimeManager.PlayOneShot("event:/UI/Level_Lose");
            Vibrator.Vibrate(600);
        }
    }
}
