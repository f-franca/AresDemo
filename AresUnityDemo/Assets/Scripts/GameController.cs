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
    public float timeRemaining = 60;
    //public GameObject completeLevelUI;
    public int numberOfTargets = 3;

    private bool gameover = false;
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

        if (timeRemaining > 0)
        {
            updateTimerText();
        }
        else
        {
            // Debug.Log("GAME OVER (NOTIFY C++)");
            Application.Quit();
            // Invoke("Restart", restartdelay);
        }
    }

    public void StartGame(){
        Debug.Log("Start command received, starting game");
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

    public void EndGame()
    {
        if (gameover == false)
        {
            gameover = true;
            Debug.Log("GAME OVER");
            Invoke("Restart", restartdelay);
        }
    }
    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void updateTimerText()
    {
        timeRemaining -= UnityEngine.Time.deltaTime;
        // timerText.text = ("Timer: " + (int)timeRemaining);
    }

    // void updateFuelText()
    // {
    //     fuelText.text = ("Fuel: " + player.GetFuel());
    // }

    /*
        void Restart()
        {
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    */
    /*
        public void CompleteLevel()
        {
            gameover = true;
            completeLevelUI.SetActive(true);
        }*/
}
