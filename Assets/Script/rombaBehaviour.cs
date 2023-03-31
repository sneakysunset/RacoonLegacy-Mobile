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
    [HideInInspector] public float timer;
    public bool isActivated = true;
    [HideInInspector] public RombaManager rMan;
    [HideInInspector] public List<path> paths;
    [HideInInspector] public int pathIndex;
    public ParticleSystem pSys;
    MeshRenderer meshR;
    Light lightR;
    bool ascending = true;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        transform.right = paths[pathIndex].pathPoint - paths[pathIndex + 1].pathPoint;
        meshR = GetComponentInChildren<MeshRenderer>();
        lightR = GetComponentInChildren<Light>();
        lightR.color = new Color(Random.Range(.6f, 1), Random.Range(.6f, 1), Random.Range(.6f, 1));

    }

    void FixedUpdate()
    {
        if (ascending)
        {
            if(timer > paths[pathIndex + 1].time)
            {
                if(pathIndex >= paths.Count - 2)
                {
                    ascending = false;
                    timer = 0;
                }
                else
                {
                    pathIndex++;
                    timer = 0;
                    transform.right = paths[pathIndex].pathPoint - paths[pathIndex + 1].pathPoint;
                }
           
            }
        }
        else
        {
            if (timer > paths[pathIndex + 1].time)
            {
                if (pathIndex == 0 )
                {
                    ascending = true;
                    timer = 0;
                }
                else
                {
                    pathIndex--;
                    timer = 0;
                    transform.right = paths[pathIndex + 1].pathPoint - paths[pathIndex].pathPoint;
                }

            }
        }


        if (isActivated)
        {
            if(ascending)
            {
                transform.position = Vector2.Lerp(paths[pathIndex].pathPoint, paths[pathIndex + 1].pathPoint, timer / paths[pathIndex + 1].time);
            }
            else
            {
                transform.position = Vector2.Lerp(paths[pathIndex + 1].pathPoint, paths[pathIndex].pathPoint, timer / paths[pathIndex + 1].time);
            }
            timer += Time.deltaTime;
        }
        else
        {
            rb.velocity = Vector2.zero;
            //rb.angularVelocity = 0;
        }
    }


    public void OnEndIteration()
    {
        StartCoroutine(disableRoomba());
        isActivated = false;
        ascending = true;
        pathIndex = 0;
        timer = 0;
        pSys.Play();
    }

    public void OnStartLoop(Romba r2)
    {
        transform.position = r2.position;
        transform.right = paths[pathIndex].pathPoint - paths[pathIndex + 1].pathPoint;
        meshR.enabled = true;
        lightR.enabled = true;
    }

    IEnumerator disableRoomba()
    {
        yield return new WaitForSeconds(.5f);

        meshR.enabled = false;
        lightR.enabled = false;
        pSys.gameObject.SetActive(true);

    }

    public void OnStartIteration(Romba r2)
    {
        transform.position = r2.position;
        transform.right = paths[pathIndex].pathPoint - paths[pathIndex + 1].pathPoint;
        meshR.enabled = true;
        lightR.enabled = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Romba"))
        {
            transform.right = Vector2.Reflect(transform.right, collision.contacts[0].normal);

        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            //rMan.walls.Remove(other.gameObject);
            //Destroy(other.gameObject);
            //FMODUnity.RuntimeManager.PlayOneShot("event:/Wall/Collider_Wall");
        }
    }
}
