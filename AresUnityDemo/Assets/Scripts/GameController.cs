using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject target;
    public GameObject player;
    public float restartdelay = 1f;
    public float gameTime = 60;
    public int numberOfTargets = 3;

    private bool hasGameStarted = false;
    private bool gameOver = false;
    private int hitsOnTarget = 0;
    private string gameOverReason;
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

        if(this.hasGameStarted)
            if (gameTime > 0)
            {
                UpdateTimer();
                if(!CheckForTargets())
                    EndGame("No more targets");
            }
            else
            {
                EndGame("Time is up");
            }
    }

    public void StartGame(){
        if(hasGameStarted) return;

        Debug.Log("Start command received, starting game");
        this.hasGameStarted = true;
        Vector3 posPlayer = new Vector3(0f, 0.5f, -40f);
        this.player = Instantiate(this.player, posPlayer, gameObject.transform.rotation);
        this.player.name = "Player";

        float x;
        float y;
        float z;
        y = 0.3f;
        Vector3 posTarget;       
        
        for(int i = 0; i < numberOfTargets; i++){
            x = Random.Range(-18f, 18f);
            z = Random.Range(-18f, 18f);
            posTarget = new Vector3(x, y, z);
            GameObject targetGameObject = Instantiate(target, posTarget, gameObject.transform.rotation);
            targetGameObject.name = "Target";
        }

    }

    public void EndGame(string reason)
    {
        if(this.gameOver) return;
        Debug.Log($"GAME OVER, {reason}");
        this.gameOver = true;
        this.gameOverReason = "GAME OVER, " + reason;
        Invoke("Restart", restartdelay);
    }

    public string GameOverReason(){
        return this.gameOverReason;
    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void UpdateTimer()
    {
        gameTime -= UnityEngine.Time.deltaTime;
    }

    private bool CheckForTargets(){
        if(GameObject.Find("Target"))
            return true;
        return false;
    }

    public bool IsGameOver(){
        return this.gameOver;
    }

    public void SetHitOnTarget(){
        this.hitsOnTarget++;
    }

    public int GetHitsOnTarget(){
        return this.hitsOnTarget;
    }
}
