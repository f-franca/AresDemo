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

int CompareTwoCharArrays(char buffer[], char char2[]){
		size_t len = strlen(buffer)+1;
		char* subject = new char[len]; // allocate for string and ending \0
		strcpy(subject,buffer);
		cout << "comparison " << ( strcmp(subject, char2) == 0  ) << endl;
		return 1;
	}

int main(int argc, char const *argv[])
{
	OpenFile();
	
	bool hasGameStarted = false;
	int sock = 0;
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
	
	Logger("\"Return\" to start the game");
	Logger("\"Escape\" to quit");
	cout << "\"Return\" to start the game\n\"Escape\" to quit" << endl;
	
	while (true){
		int byte_count;
//		char okArray[] = "ok";
		char buffer[56];
		
		system("stty raw"); 
		char oneChar = getchar();
//		int oneCharInt = oneChar;
		if (oneChar == 27) { // escape
			system("stty cooked");
			string quit = "quit";
			Logger("Quitting game");
			const char* toSend = quit.c_str();
			send(sock , toSend , strlen(toSend) , 0 );
			shutdown(sock,2);
			break;
		}
		if (oneChar == 13) { // return
			system("stty cooked");
			
			if(hasGameStarted)
				continue;
				
			string start = "start";
			Logger("Starting game");
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
//			cout << "byte_count: " << byte_count << " sub buffer: " << subbuff << endl;

//			cout << " comparison move" << ( strcmp(subbuff,"ok") == 0 )<< endl;
			if( strcmp(subbuff,"ok") != 0 ){  // message other than OK received
				Logger("Did not received OK, ending program:: " + string(subbuff));
				shutdown(sock,2);
				break;
			}

			byte_count = recv(sock, buffer, sizeof(buffer), 0);
			char subbuff2[byte_count + 1];
			memcpy( subbuff2, &buffer[0], byte_count );
			subbuff2[byte_count] = '\0';
//			cout << "byte_count: " << byte_count << " sub buffer2:[" << subbuff2 << "]"<< endl;
			Logger("Firing weapon:: Shots fired: " + string(subbuff2));

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
//			cout << "byte_count: " << byte_count << " sub buffer: " << subbuff << endl;

			if( strcmp(subbuff,"ok") != 0){  // message other than OK received
				Logger("Did not received OK, ending program:: " + string(subbuff));
				shutdown(sock,2);
				break;
			}

			Logger(toSendStr + " pressed");
//			cout << " comparison move" << ( strcmp(subbuff,"ok") == 0 )<< endl;
//			cout << endl << "char: [" << oneChar << "] = [" << toSend << "] | [" << oneCharInt << "] sent" << endl;

		}

	}
	// TODO number of shots input, time elapsed and # of targets hit
	CloseFile();
	return 0;
}
