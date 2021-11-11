using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 6f;
    public float turnSpeed = 50f;
    public float turretSpeed = 50f;
    public float launchVelocity = 700f;
    public GameObject projectile;
    
    private GameObject turret;
    private GameObject cannon;
    private GameObject cannonTip;
    private Rigidbody rigidbody;
    private bool hasInput = false;
    private bool hasShot = false;
    private int firedRounds = 0;
    private float timeBetweenShooting = 1f;
    private string messageFromTcpServer;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        turret = GameObject.Find("Player/Turret");
        cannon = GameObject.Find("Player/Turret/CannonObj");
        cannonTip = GameObject.Find("Player/Turret/CannonObj/Cannon Tip");
        this.messageFromTcpServer = "";
    }

    void Update(){
        if (this.hasShot)
            this.timeBetweenShooting -= Time.deltaTime;

        if(this.messageFromTcpServer != ""
        ) {
            if(this.messageFromTcpServer == " "){
                if(!this.hasShot){
                    Debug.Log("Playert attemp to fire");
                    CheckForMovement(this.messageFromTcpServer);
                    this.messageFromTcpServer = "";
                    this.hasShot = true;
                }
                if(this.timeBetweenShooting  <= 0){
                    this.hasShot = false;
                    this.timeBetweenShooting = 1f;
                    return;
                }
            } else
                CheckForMovement(this.messageFromTcpServer);
        }
    }
    
    public void CheckForMovement(string messageFromTcpServer){
        float horizontalMove = 0f;
        float verticalMove = 0f;
        bool turretLeft = false;
        bool turretRight = false;
        bool turretUp = false;
        bool turretDown = false;

        switch(messageFromTcpServer){
            case "w":
                verticalMove = 1f;
                break;
            case "s":
                verticalMove = -1f;
                break;
            case "a":
                horizontalMove = -1f;
                break;
            case "d":
                horizontalMove = 1f;
                break;
            case "j":
                turretLeft = true;
                break;
            case "l":
                turretRight = true;
                break;
            case "i":
                turretUp = true;
                break;
            case "k":
                turretDown = true;
                break;
        }

        if (horizontalMove != 0 || verticalMove != 0) hasInput = true;

        if(turretRight){
        	turret.transform.Rotate(0, Time.deltaTime * turretSpeed, 0);
	    }
        if(turretLeft){
            turret.transform.Rotate(0, Time.deltaTime * -turretSpeed, 0);
	    }
        if(turretUp){
        	cannon.transform.Rotate(Time.deltaTime * -turretSpeed, 0, 0);
			if (cannon.transform.eulerAngles.x < 300 && cannon.transform.eulerAngles.x > 10)        	
				cannon.transform.eulerAngles = new Vector3(
					300,
    				cannon.transform.eulerAngles.y,
    				cannon.transform.eulerAngles.z);
	    }
        if(turretDown){
    		cannon.transform.Rotate(Time.deltaTime * turretSpeed, 0, 0);
			if (cannon.transform.eulerAngles.x < 300 && cannon.transform.eulerAngles.x > 10)        	
				cannon.transform.eulerAngles = new Vector3(
					10,
    				cannon.transform.eulerAngles.y,
    				cannon.transform.eulerAngles.z);
	    }

        if (verticalMove < 0)
            horizontalMove = horizontalMove * (-1);

        // Calculate the Direction to Move based on the tranform of the Player
        Vector3 moveDirectionForward = transform.forward * verticalMove;
        Vector3 direction = (moveDirectionForward).normalized;

        if (hasInput)
        {
            float timeMoving = 0.5f;

                float angularDistance = horizontalMove * turnSpeed * Time.deltaTime;
                Vector3 distance = direction * speed * Time.deltaTime;
                timeMoving -= Time.deltaTime;
                transform.Rotate(Vector3.up, angularDistance);
                rigidbody.MovePosition(transform.position + distance);
        }

        Shoot(messageFromTcpServer);

        hasInput = false;
    }

    public void Shoot(string messageFromTcpServer){
        if (messageFromTcpServer == " ")
        {
 			GameObject shell = Instantiate(projectile, cannonTip.transform.position, cannonTip.transform.rotation);
 			shell.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, launchVelocity * 1000, 0));
            this.firedRounds++;
        }
    }

    public void MessageTcp(string messageReceived){
        this.messageFromTcpServer = messageReceived;
    }

    public int FiredRounds(){
        return this.firedRounds;
    }
}
