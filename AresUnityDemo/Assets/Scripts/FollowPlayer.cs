using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public float speed = 5f;
    public float smoothSpeed = 1f;
    public  Vector3 offset = new Vector3(0, 8, -11);
    public bool lookAt = true;

    private bool playerFound = false;
    private GameObject player;
    private Space offsetPositionSpace = Space.Self;


    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(WaitAfterStart(3)); //
    }

    // Update is called once per frame
    void Update()
    {
        // TryToFindPlayer();
        if (!this.playerFound){
			// Debug.Log("Player indeed not found @ camera");
        	FindPlayer();
			return;
		}
        /*
        //transform.position = player.transform.position + offset;
        //transform.rotation = Quaternion.Lerp(transform.rotation, player.transform.rotation, Time.deltaTime * speed);
        
        Vector3 desiredPosition = player.transform.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
        transform.LookAt(player.transform);
        */
    }

    
    void LateUpdate()
    {
        if(!this.player) return;
        // compute position
        if (offsetPositionSpace == Space.Self)
        {
            Vector3 desiredPosition = player.transform.TransformPoint(offset);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

            //transform.position = player.transform.TransformPoint(offset);
            transform.position = smoothedPosition;
            //transform.position = desiredPosition;
        }
        else
        {
            transform.position = player.transform.position + offset;
        }

        // compute rotation
        if (lookAt)
        {
            transform.LookAt(player.transform);
        }
        else
        {
            transform.rotation = player.transform.rotation;
        }
    }

    // private void TryToFindPlayer(){
    //     if(!this.player){
    //         try{
    //             // GameObject
    //             this.player = GameObject.Find("Player/Turret/CannonObj");
    //             Debug.Log($"Player found @ camera {player}");
    //         } catch(Exception e){
    //             Debug.Log($"Player not found @ camera {e}");
    //         }
    //     }
    // }

    private void FindPlayer(){
        GameObject playerGO = GameObject.Find("Player");
        if(playerGO){
			// Debug.Log("Player found @ camera");
            // this.player = playerGO.GetComponent<PlayerController>();
            this.player = GameObject.Find("Player/Turret/CannonObj");
            this.playerFound = true;
        }
    }

     IEnumerator WaitAfterStart(int seconds)
    {
        //Print the time of when the function is first called.
        Debug.Log("Started Coroutine at timestamp : " + Time.time);

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(seconds);

        //After we have waited 5 seconds print the time again.
        Debug.Log("Finished Coroutine at timestamp : " + Time.time);
    }
}
