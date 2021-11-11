# ARES Software Project – Game Demo

## Description
This project consists in a game demonstration for ARES Company, the objective is for you, the player, to control and armored tank and destroy all targets in the scene by firing at them. The game itself was made with Unity and the user inputs all the commands through an C++ application using the terminal.

## How to Play
Start the Unity Game, then start the C++ application.

The player can move the **tank** by pressing:
- W to move forward
- S to move backward
- A to move left
- D to move right

The player can move the **turret** by pressing:
- I to move the turret upward
- K to move the turret downward
- J to  move the torret toward the left
- L to  move the torret toward the right

The player fires the weapon by pressing **space**.

# Implementation
In this section, we will cover the basics regarding the implementation of both Unity game and C++ program.

Efforts were made in order to avoid hard coded behaviours, create a clean and reusable code which is easy to mantain and to develop an easy to understand code.

## Message exchange
The messages are exchanged between the C++ application and Unity program through the TCP/IP protocol. Inside Unity's source code, a new thread is created with the sole purpose of serving as a TCP server, exchanging messages with the TCP client (C++ application) and notifying other Unity classes. 

TCP/IP was choosen due to its trustworthy handshake connection, but everything could also be done using UDP.

### What messages are exchanged?
All commands inputted by the player (TCP client) are send to Unity listener (TCP server). For each command type sent by the player, there is a specific behaviour in the game. All messages sent **from** the player contains only one character, messages **to** the player may contain more than one character. Possible commands are:
- The player sends the *start the game* command
	* In this case, the game starts and wait for new commands
- The player presses any button
	- A message containing the button is send to the sersver and if the button is a valid movement button, the game will make the tank move or shoot and send back to the player an *ok* message
- The player sends the *end the game* command
	- In this case, Unity finishes the game

### Messages and Game End State
There are three cases that make the game end. First, the player chooses to send the end command, second, Unity detects that there are no more enemies and sends to the player a message saying that the game is over, and third, the timeout for the scene is met and Unity sends to the player that the time is out. For all three cases, the game finishes and the TCP connection is closed.

If the player sends any button and does not receive an *ok* message from the server, the game and C++ application ends and the TCP connection is closed.

## Classes and Files

Since this is a demo project, there are not many files in total, either in Unity and in the C++ project. We will see the main files created and used for both applications.

### C++
#### Logger.h
This header file has the purpose of log everything that happens throughout the communication and operation of the player and server (game). It is simple enough, it just creates a new and unique log file, and keeps appending in that file all commands sent from the player and some responses from the server. The logs contain, for example, the start of the game, how many shots were fired, how many targets were hit and all the movements made by the player. The log file also contains a timestamp column, so we are able to see the exact moment the event ocurred.


#### main.cpp
Creates the socket and attempts to make the TCP connection with the server. If the connetion is done successfully, the player is able to freely input any command. For commands other than *start* and *end* the game, the main function deals with responses containing an *ok* status from the server. If anything other thank *ok* is received, all the programs are aborted.

The main function also calls the Logger header file to log all input done and some messages received.

### Unity
The game scene is fairly simple, it only contains a plane and, once the game starts, the player and enemy tanks. 

#### FollowPlayer.cs
This script makes the camera follows the player. The movement is adjusted with some smoothness

#### GameController.cs
This is the main Game Controller script, having the information about the time left for the game, the number of targets left and the game state (wether is over or not). This script also communicates with TcpServer.cs to know if the targets are hit and to handle the TcpServer, which will further deliver to the player, the reason the game was over. This script also randomly spawns targets in the scene, the number of target spawned is easily changed.

#### PlayerController.cs
All player movements are made here, including moving the tank, turret and firing the cannon. It directly communicates with TcpServer.cs to know if there is a valid command from the user. 

#### ShellController.cs
When a shell is fired from the cannon, this ultra small script sets the shell's lifespan, meaning it will be destroyed after some time.

#### TargerController.cs
Chooses which move pattern the target will have. A target is randomly spawned and instantiated through the GameController.cs and upon instantiation, a random movement is selected, from sinus, circular and horizontal, which are periodically repeated. The hit detection from a fired shell is made here, destroying both shell and target, but communicating with GameController.cs about the hit made.

#### TcpServer.cs
This script sets the TCP server connection and waits for the client connection. Upon connection stablished, it connects diretly to PlayerController.cs to hand over the message received from the TCP client (C++ application, controlled by the user). It also connects with GameController.cs, checking if the game is over.