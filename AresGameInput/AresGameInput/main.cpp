// Client side C/C++ program to demonstrate Socket programming
#include <stdio.h>
#include <sys/socket.h>
#include <arpa/inet.h>
#include <unistd.h>
#include <string.h>
#include <iostream>
#include "logger.h"

#define PORT 8052

using namespace std;

string ButtonMap(string input){
	string output;
	
	if ( input ==  "w" )
		output = "Move Forward";
	else if ( input ==  "s" )
		output = "Move Backward";
	else if ( input ==  "a" )
		output = "Move Left";
	else if ( input ==  "d" )
		output = "Move Right";
	else if ( input ==  "i" )
		output = "Move Turret Up";
	else if ( input ==  "k" )
		output = "Move Turret Down";
	else if ( input ==  "j" )
		output = "Move Turret Left";
	else if ( input ==  "l" )
		output = "Move Turret Right";
	else
		output = input;
	
	return output;
}

void SimpleInstructions(){
	cout << "WASD to move the tank\nIJKL to move the turret\nSpace to shoot" << endl;
	cout << "\"Return\" to start the game\n\"Escape\" to quit" << endl;
}

int main(int argc, char const *argv[])
{
	OpenFile();
	
	bool hasGameStarted = false;
	int sock = 0;
	int shotsFired = 0;
	struct sockaddr_in serv_addr;
	
	if ((sock = socket(AF_INET, SOCK_STREAM, 0)) < 0)
	{
		Logger("\n Socket creation error \n");
		printf("\n Socket creation error \n");
		return -1;
	}

	serv_addr.sin_family = AF_INET;
	serv_addr.sin_port = htons(PORT);
	
	// Convert IPv4 and IPv6 addresses from text to binary form
	if(inet_pton(AF_INET, "127.0.0.1", &serv_addr.sin_addr)<=0)
	{
		Logger("\nInvalid address/ Address not supported \n");
		printf("\nInvalid address/ Address not supported \n");
		return -1;
	}

	if (connect(sock, (struct sockaddr *)&serv_addr, sizeof(serv_addr)) < 0)
	{
		Logger("\nConnection Failed \n");
		printf("\nConnection Failed \n");
		return -1;
	}
	
	SimpleInstructions();
	
	while (true){
		int byte_count;
		char buffer[56];
		
		system("stty raw"); 
		char oneChar = getchar();
		if (oneChar == 27) { // escape
			system("stty cooked");
			string quit = "quit";
			Logger("Quitting game");
			const char* toSend = quit.c_str();
			send(sock , toSend , strlen(toSend) , 0 );
			
			byte_count = recv(sock, buffer, sizeof(buffer), 0);
			char subbuff[byte_count + 1];
			memcpy( subbuff, &buffer[0], byte_count );
			subbuff[byte_count] = '\0';
			Logger( string(subbuff) );
			shutdown(sock,2);
			break;
		}
		if (oneChar == 13) { // return
			system("stty cooked");
			
			if(hasGameStarted)
				continue;
			
			StartTimer();
			string start = "start";
			Logger("Starting the game");
			const char* toSend = start.c_str();
			send(sock , toSend , strlen(toSend) , 0 );
			hasGameStarted = true;
			continue;			 
		}
		if (oneChar == 32) { // spacebar
			system("stty cooked");
			
			string toSendStr = string(1,(char) oneChar);
			const char* toSend = toSendStr.c_str();
			send(sock , toSend , strlen(toSend) , 0 );
			
			byte_count = recv(sock, buffer, sizeof(buffer), 0);
			char subbuff[byte_count + 1];
			memcpy( subbuff, &buffer[0], byte_count );
			subbuff[byte_count] = '\0';

			if( strcmp(subbuff,"ok") != 0 ){  // message other than OK received
				Logger("Did not received OK, ending program:: " + string(subbuff));
				shutdown(sock,2);
				break;
			}

			byte_count = recv(sock, buffer, sizeof(buffer), 0);
			char subbuff2[byte_count + 1];
			memcpy( subbuff2, &buffer[0], byte_count );
			subbuff2[byte_count] = '\0';
			Logger("Firing weapon:: Shots fired: " + string(subbuff2));
			shotsFired = atoi(subbuff2);
			
			continue;
		}
		else {
			// Reset terminal to normal "cooked" mode 
			system("stty cooked");
			
			string toSendStr = string(1,(char) oneChar);
			const char* toSend = toSendStr.c_str();
			send(sock , toSend , strlen(toSend) , 0 );
			
			byte_count = recv(sock, buffer, sizeof(buffer), 0);
			char subbuff[byte_count + 1];
			memcpy( subbuff, &buffer[0], byte_count );
			subbuff[byte_count] = '\0';

			if( strcmp(subbuff,"ok") != 0){  // message other than OK received
				Logger("Did not received OK, ending program:: " + string(subbuff));
				shutdown(sock,2);
				break;
			}
			
			
			Logger( ButtonMap(toSendStr) + " pressed");
		}

	}
	CloseFile(shotsFired);
	return 0;
}
