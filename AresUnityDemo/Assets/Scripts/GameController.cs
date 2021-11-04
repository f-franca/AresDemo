using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    // public PlayerController player;
    public float restartdelay = 1f;
    // public Text timerText;
    // public Text fuelText;
    public float timeRemaining = 60;
    //public GameObject completeLevelUI;
    public int numberOfTargets = 3;
    public GameObject target;

    private bool gameover = false;
    private TcpServer tcpServer;

    // Start is called before the first frame update
    void Start()
    {
        tcpServer = GetComponent<TcpServer>();
        tcpServer.enabled = true;
        float x;
        float y;
        float z;
        y = 0.3f;
        Vector3 pos;       
        
        // transform.position = pos;
        for(int i = 0; i < numberOfTargets; i++){
            x = Random.Range(-18f, 18f);
            z = Random.Range(-18f, 18f);
            pos = new Vector3(x, y, z);
            // Debug.Log($"Random pos {pos}");
            GameObject shell = Instantiate(target, pos, gameObject.transform.rotation);
        }
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

    public bool IsTcpControllerEnabled(){
        return this.tcpServer.enabled;
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
