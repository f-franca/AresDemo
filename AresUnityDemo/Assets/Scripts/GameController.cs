using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject target;
    public GameObject player;
    // public PlayerController player;
    public float restartdelay = 1f;
    // public Text timerText;
    // public Text fuelText;
    public float gameTime = 60;
    //public GameObject completeLevelUI;
    public int numberOfTargets = 3;

    private bool gameHasStarted = false;
    private TcpServer tcpServer;

    // Start is called before the first frame update
    void Start()
    {
        tcpServer = GetComponent<TcpServer>();
        tcpServer.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Debug.Log("Quit");
            Application.Quit();
        }

        if(this.gameHasStarted)
            if (gameTime > 0)
            {
                UpdateTimer();
                if(!CheckForTargets())
                    EndGame("No more targets");
            }
            else
            {
                EndGame("Time is up");
                // Debug.Log("GAME OVER (NOTIFY C++)");
            }
    }

    public void StartGame(){
        Debug.Log("Start command received, starting game");
        this.gameHasStarted = true;
        Vector3 posPlayer = new Vector3(0f, 0.5f, -40f);
        this.player = Instantiate(this.player, posPlayer, gameObject.transform.rotation);
        this.player.name = "Player";

        float x;
        float y;
        float z;
        y = 0.3f;
        Vector3 posTarget;       
        
        // transform.position = pos;
        for(int i = 0; i < numberOfTargets; i++){
            x = Random.Range(-18f, 18f);
            z = Random.Range(-18f, 18f);
            posTarget = new Vector3(x, y, z);
            // Debug.Log($"Random pos {pos}");
            GameObject targetGameObject = Instantiate(target, posTarget, gameObject.transform.rotation);
            targetGameObject.name = "Target";
        }

    }

    public void EndGame(string reason)
    {
        Debug.Log($"GAME OVER, {reason}");
        Invoke("Restart", restartdelay);
    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void UpdateTimer()
    {
        gameTime -= UnityEngine.Time.deltaTime;
        // timerText.text = ("Timer: " + (int)gameTime);
    }

    private bool CheckForTargets(){
        if(GameObject.Find("Target"))
            return true;
        return false;
    }

    // void updateFuelText()
    // {
    //     fuelText.text = ("Fuel: " + player.GetFuel());
    // }

    /*
        public void CompleteLevel()
        {
            gameover = true;
            completeLevelUI.SetActive(true);
        }*/
}
