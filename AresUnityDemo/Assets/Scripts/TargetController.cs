using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    public float speed = 6f;
    public float turnSpeed = 6f;
    
    private GameController gameController;
    private Rigidbody rigidbody;
    private bool hasBeenHit = false;
    private float timer = 0f;
    private string pattern;

    // Start is called before the first frame update
    void Start()
    {
        this.pattern = ChoosePattern();
        rigidbody = GetComponent<Rigidbody>();
        gameController = GameObject.Find("Game Controller").GetComponent<GameController>();
        StartCoroutine(DelayAction(3));
    }
    
    IEnumerator DelayAction(float delayTime)
    {
    yield return new WaitForSeconds(delayTime);
    }

    // Update is called once per frame
    void Update()
    {
        this.timer += Time.deltaTime;
        MoveTarget(this.pattern);
    }

    private string ChoosePattern(){
        string[] patterns = {"horizontal","circular","sinus"};
        int index = Random.Range(0,patterns.Length);
        string pattern = patterns[index];
        return pattern;
    }

    private void MoveTarget(string pattern){
        switch (pattern)
        {
            case "horizontal":
                MoveHorizontal(this.timer, 2, 1);
                break;
            case "circular":
                MoveCircular();
                break;
            case "sinus":
                MoveSinus(this.timer, 5, 1);
                break;
            default:
                MoveHorizontal(this.timer, 2, 1);
                break;
        }
    }

    private void MoveHorizontal(float timer, float speed, float scale){
        Vector3 moveDirectionForward = transform.forward;
        Vector3 direction = (moveDirectionForward).normalized;
        Vector3 distance = direction * this.speed * Time.deltaTime;

        rigidbody.MovePosition(transform.position + distance * oscillate(timer,speed,scale));
    }

    private void MoveCircular(){
        Vector3 moveDirectionForward = transform.forward;
        Vector3 moveDirectionSide = Vector3.zero;
        Vector3 direction = (moveDirectionForward).normalized;
        Vector3 distance = direction * this.speed * Time.deltaTime;

        rigidbody.MovePosition(transform.position + distance);
        transform.Rotate(Vector3.up, Time.deltaTime * turnSpeed);
    }

    private void MoveSinus(float timer, float speed, float scale){
        Vector3 moveDirectionForward = transform.forward;
        Vector3 moveDirectionSide = Vector3.zero;
        Vector3 direction = (moveDirectionForward).normalized;
        Vector3 distance = direction * this.speed * Time.deltaTime;

        rigidbody.MovePosition(transform.position + distance * oscillate(timer, speed/10, scale) );
        transform.Rotate(Vector3.up, Time.deltaTime * turnSpeed * oscillate(timer, speed*1.5f, scale*4) );
    }

    private float oscillate(float timer, float speed, float scale)
    {
        return Mathf.Cos(timer * speed / Mathf.PI) * scale;
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Shell"){
            if(hasBeenHit) return;
            this.hasBeenHit = true;
            this.gameController.SetHitOnTarget();
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
