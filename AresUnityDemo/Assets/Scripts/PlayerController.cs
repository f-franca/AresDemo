using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //public CharacterController characterController;
    public float speed = 6f;
    public float turnSpeed = 50f;
    public float turretSpeed = 50f;
    public float launchVelocity = 700f;
    public GameObject projectile;
    
    private GameObject turret;
    private GameObject cannon;
    private GameObject cannonTip;
    private new Rigidbody rigidbody;
    private bool hasInput = false;
    private bool hasShot = false;
    private int fuel = 50;
    private int totalFuelConsumed = 0;
    private float timeBetweenShooting = 1f;
    // public float movementPeriod;
    // private float timeMoving;
    private string messageFromTcpServer;

    // Start is called before the first frame update
    void Start()
    {
        // body = GameObject.Find("Player/Body/");
        rigidbody = GetComponent<Rigidbody>();
        // characterController = gameObject.GetComponentInChildren<CharacterController>();
        turret = GameObject.Find("Player/Turret");
        cannon = GameObject.Find("Player/Turret/CannonObj");
        cannonTip = GameObject.Find("Player/Turret/CannonObj/Cannon Tip");
        this.messageFromTcpServer = "";
        // this.previousMessage = "";
        // this.timeMoving = this.movementPeriod;
    }

    void Update(){
        if (this.hasShot)
            this.timeBetweenShooting -= Time.deltaTime;

        if(this.messageFromTcpServer != ""
            // (this.previousMessage != this.messageFromTcpServer)
        ) {
            if(this.messageFromTcpServer == " "){
                if(!this.hasShot){
                    CheckForMovement(this.messageFromTcpServer);
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
        //     // while(this.timeMoving >= 0){
        //     if (this.timeMoving >= 0){
        //         // Debug.Log($"update player {this.timeMoving}");
        //         this.timeMoving -= Time.deltaTime;
        //         CheckForMovement(this.messageFromTcpServer);
        //     } else {

        //         CheckForMovement(this.messageFromTcpServer);
        //         this.previousMessage = messageFromTcpServer;
        //         this.messageFromTcpServer = "";
        //         // this.timeMoving = this.movementPeriod;
        //     }
        // } 
        // else
        //     if(this.messageFromTcpServer != "" &&
        //             (this.previousMessage == this.messageFromTcpServer)
        //         ) {
        //             CheckForMovement(this.messageFromTcpServer);
        //             this.timeMoving = this.movementPeriod;
        //             this.previousMessage = "";
        //     }
    }
    
    public void CheckForMovement(string messageFromTcpServer){
        // float horizontalMove = Input.GetAxis("Horizontal");
        // float verticalMove = Input.GetAxis("Vertical");
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

        // if (Input.GetKey("l")){
        if(turretRight){
        	turret.transform.Rotate(0, Time.deltaTime * turretSpeed, 0);//, Space.World);
	    }
        // if (Input.GetKey("j")){
        if(turretLeft){
            turret.transform.Rotate(0, Time.deltaTime * -turretSpeed, 0);//, Space.World);
	    }
        // if (Input.GetKey("i")){
        if(turretUp){
        	cannon.transform.Rotate(Time.deltaTime * -turretSpeed, 0, 0);//, Space.World);
			if (cannon.transform.eulerAngles.x < 300 && cannon.transform.eulerAngles.x > 10)        	
				cannon.transform.eulerAngles = new Vector3(
					300,
    				cannon.transform.eulerAngles.y,
    				cannon.transform.eulerAngles.z);
	    }
        // if (Input.GetKey("k")){
        if(turretDown){
    		cannon.transform.Rotate(Time.deltaTime * turretSpeed, 0, 0);//, Space.World);
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

            // while(timeMoving >= 0){
                float angularDistance = horizontalMove * turnSpeed * Time.deltaTime;
                Vector3 distance = direction * speed * Time.deltaTime;
                timeMoving -= Time.deltaTime;
                transform.Rotate(Vector3.up, angularDistance);
                rigidbody.MovePosition(transform.position + distance);
            // }
        }

        Shoot(messageFromTcpServer);

        hasInput = false;
    }

    private void Shoot(string messageFromTcpServer){
        if (messageFromTcpServer == " ")
        // if (Input.GetKeyDown("space"))
        {
 			GameObject shell = Instantiate(projectile, cannonTip.transform.position, cannonTip.transform.rotation);
 			shell.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, launchVelocity * 1000, 0));
        }
    }

    public void MessageTcp(string messageReceived){
        this.messageFromTcpServer = messageReceived;
    }

    public void ConsumeFuel(int amount)
    {
        this.fuel -= amount;
        this.totalFuelConsumed += amount;
        //Debug.Log($"Fuel consumed: {amount}, total fuel left: {this.fuel}");
    }

    public int GetFuel()
    {
        return this.fuel;
    }

    public int GetTotalCost()
    {
        return this.totalFuelConsumed;
    }
}
