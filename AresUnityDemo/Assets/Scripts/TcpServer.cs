using System;
using System.Collections; 
using System.Collections.Generic; 
using System.Net; 
using System.Net.Sockets; 
using System.Text; 
using System.Threading; 
using UnityEngine;  

public class TcpServer : MonoBehaviour {  	
	#region private members 	
	/// <summary> 	
	/// TCPListener to listen for incomming TCP connection 	
	/// requests. 	
	/// </summary> 	
	private TcpListener tcpListener; 
	/// <summary> 
	/// Background thread for TcpServer workload. 	
	/// </summary> 	
	private Thread tcpListenerThread;  	
	/// <summary> 	
	/// Create handle to connected tcp client. 	
	/// </summary> 	
	private TcpClient connectedTcpClient; 
    private PlayerController player;
    private GameController gameController;
	// private bool isEnabled = false;
    private bool playerFound = false;
	private bool startTheGame = false;
	private float delayBeforeEmptyStream = 0.3f;
    private float period;
    private string messageToSend;
    	
	#endregion 	

    void Awake(){
        this.messageToSend = "";
    }
		
	// Use this for initialization
	void Start () { 		
		// Start TcpServer background thread 
        gameController = GetComponent<GameController>();

		this.period = this.delayBeforeEmptyStream;		
		tcpListenerThread = new Thread (new ThreadStart(ListenForIncommingRequests)); 		
		tcpListenerThread.IsBackground = true;
		tcpListenerThread.Start();
	}  	
	
	// Update is called once per frame
	void Update () { 		
		// Debug.Log($"this enabled @ tcpServer {this.enabled}");
		
		// if (Input.GetKeyDown(KeyCode.Space)) {
		// 	SendMessage();         
		// }
		if (this.startTheGame){
			GameControllerStartGame();
			return;
		}

        if (!this.playerFound){
			Debug.Log("Player indeed not found @ tcp");
        	FindPlayer();
			return;
		}
        // Debug.Log($"Player {player}");
        if (this.messageToSend != ""){
            // Debug.Log("client message: " + this.messageToSend);
            switch(this.messageToSend){
                case "w":
                case "s":
                case "a":
                case "d":
                case "j":
                case "l":
                case "i":
                case "k":
                case " ":
                    SendMessageToPlayer(this.messageToSend);
                    break;
            }
        } else{
            SendMessageToPlayer("");
        }
		this.period -= Time.deltaTime;
		// this.isEnabled = this.enabled;
	}

    private void FindPlayer(){
        GameObject playerGO = GameObject.Find("Player");
        if(playerGO){
			Debug.Log("Player found @ tcpServer");
            this.player = playerGO.GetComponent<PlayerController>();
            this.playerFound = true;
        }
    }
	
	/// <summary> 	
	/// Runs in background TcpServerThread; Handles incomming TcpClient requests 	
	/// </summary> 	
	private void ListenForIncommingRequests () { 		
		try { 			
			// Create listener on localhost port 8052. 			
			tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8052);
			tcpListener.Start();              
			Byte[] bytes = new Byte[1024];  
			while (true) { 				
				Debug.Log("Server is listening");    
				using (connectedTcpClient = tcpListener.AcceptTcpClient()) { 					
					Debug.Log("Connection established");    
					// Get a stream object for reading 					
					using (NetworkStream stream = connectedTcpClient.GetStream()) { 						
						int length; 						
						// Read incomming stream into byte arrary.
                        while(true){
							NextMessage:
							if ( stream.DataAvailable ){
								while ((length = stream.Read(bytes, 0, bytes.Length)) != 0) { 	
									var incommingData = new byte[length]; 							
									Array.Copy(bytes, 0, incommingData, 0, length);  							
									// Convert byte array to string message.
									this.messageToSend = Encoding.ASCII.GetString(incommingData);
									// Debug.Log($"recv stream @ tcpServer [{this.messageToSend}]");		
									this.period = this.delayBeforeEmptyStream;
									break;
								}
							}
							// Debug.Log($"data unavailable | period {this.period}");
							if(this.period < 0f) this.messageToSend = "";
							switch(this.messageToSend){
								case "quit":
									CloseTcpConnection(connectedTcpClient, tcpListener);
									Debug.Log("The socket is finally closed");
									goto NextClient;
								case "start":
									Debug.Log("TCP msg is starting the game");
									this.startTheGame = true;
									this.messageToSend = "";			
									goto NextMessage;
							}
                        }
						NextClient:
						this.messageToSend = "";
					} 				
				} 			
			} 		
		} 		
		catch (SocketException socketException) { 			
			Debug.Log("SocketException " + socketException.ToString()); 		
		}     
	}

	private void CloseTcpConnection(TcpClient connectedTcpClient, TcpListener tcpListener){
		try {
			connectedTcpClient.Close();
			Debug.Log("Closing TCP Client");
    	}
		catch(Exception e){
			Debug.Log($"Error when trying to TCP Client: {e.Message}");
		}
	    // try{
        // 	tcpListener.Stop();
        // 	Debug.Log("Closing TCP Listener");
		// }
		// catch(Exception e){
		// 	Debug.Log($"Erro ao fechar TCP Listener: {e.Message}");
		// }
	}

	bool SocketConnected(Socket s) {
		bool part1 = s.Poll(1000, SelectMode.SelectRead);
		bool part2 = (s.Available == 0);
		if (part1 && part2)
			return true;
		else
			return false;
	}

	private void GameControllerStartGame(){
		if(this.gameController){
			this.gameController.StartGame();
			this.startTheGame = false;
		}
		else
			Debug.LogError("gameController not found");
	}

    private void SendMessageToPlayer(string message){
        if (this.player){
            player.MessageTcp(message);
			// this.messageToSend = "";
        }
    }

	/// <summary> 	
	/// Send message to client using socket connection. 	
	/// </summary> 	
	private void SendMessage() { 		
		if (connectedTcpClient == null) {             
			return;         
		}  		
		
		try { 			
			// Get a stream object for writing. 			
			NetworkStream stream = connectedTcpClient.GetStream(); 			
			if (stream.CanWrite) {                 
				string serverMessage = "This is a message from your server."; 			
				// Convert string message to byte array.                 
				byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(serverMessage); 				
				// Write byte array to socketConnection stream.               
				stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);               
				Debug.Log("Server sent his message - should be received by client");           
			}       
		} 		
		catch (SocketException socketException) {             
			Debug.Log("Socket exception: " + socketException);         
		} 	
	} 
}