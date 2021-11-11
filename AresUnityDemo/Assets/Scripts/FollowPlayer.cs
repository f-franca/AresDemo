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

    // Update is called once per frame
    void Update()
    {
        if (!this.playerFound){
        	FindPlayer();
			return;
		}
    }

    
    void LateUpdate()
    {
        if(!this.player) return;
        // compute position
        if (offsetPositionSpace == Space.Self)
        {
            Vector3 desiredPosition = player.transform.TransformPoint(offset);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

            transform.position = smoothedPosition;
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
    private void FindPlayer(){
        GameObject playerGO = GameObject.Find("Player");
        if(playerGO){
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
