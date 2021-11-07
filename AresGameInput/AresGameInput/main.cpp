// Client side C/C++ program to demonstrate Socket programming
#include <stdio.h>
#include <sys/socket.h>
#include <arpa/inet.h>
#include <unistd.h>
#include <string.h>
#include <iostream>
#include <sys/types.h>
#include <sys/ioctl.h>


#define KEY_UP 72
#define KEY_DOWN 80
#define KEY_LEFT 75
#define KEY_RIGHT 77
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
	int sock = 0;
//	int valread;
	struct sockaddr_in serv_addr;
//	char *hello = "Hello from client";
//	char buffer[1024] = {0};
	if ((sock = socket(AF_INET, SOCK_STREAM, 0)) < 0)
	{
		printf("\n Socket creation error \n");
		return -1;
	}

	serv_addr.sin_family = AF_INET;
	serv_addr.sin_port = htons(PORT);
	
	// Convert IPv4 and IPv6 addresses from text to binary form
	if(inet_pton(AF_INET, "127.0.0.1", &serv_addr.sin_addr)<=0)
	{
		printf("\nInvalid address/ Address not supported \n");
		return -1;
	}

	if (connect(sock, (struct sockaddr *)&serv_addr, sizeof(serv_addr)) < 0)
	{
		printf("\nConnection Failed \n");
		return -1;
	}
	
	cout << "\"Return\" to start the game\n\"Escape\" to quit" << endl;
	
	int count;
	ioctl(sock, FIONREAD, &count);
	cout << "count " << count << endl;
	
//	send(sock , hello , strlen(hello) , 0 );
//	printf("Hello message sent\n");
//	valread = read( sock , buffer, 1024);
//	printf("%s\n",buffer );


//	char oneChar; // Note: char cannot represent EOF
	while (true){
		int byte_count;
//		char okArray[] = {'o','k','\0'};
		char okArray[] = "ok";
		char buffer[56];
		
//	while ( (oneChar = std::cin.get()) != 27  ) { // EOF  // 27 escape
//		ioctl(sock, FIONREAD, &count);
//		cout << "count " << count << endl;
		// Set terminal to raw mode 
		system("stty raw"); 
		char oneChar = getchar();
		int oneCharInt = oneChar;
		if (oneChar == 27) { // escape
			system("stty cooked");
			string quit = "quit";
			const char* toSend = quit.c_str();
			send(sock , toSend , strlen(toSend) , 0 );
			shutdown(sock,2);
			break;
		}
		if (oneChar == 13) { // return
			system("stty cooked");
			string start = "start";
			const char* toSend = start.c_str();
			send(sock , toSend , strlen(toSend) , 0 );
			continue;			 
		}
		if (oneChar == 32) { // spacebar
			system("stty cooked");
			
//			char buffer[56];//[10] = {0};
			string toSendStr = string(1,(char) oneChar);
			const char* toSend = toSendStr.c_str();
			send(sock , toSend , strlen(toSend) , 0 );
			
			byte_count = recv(sock, buffer, sizeof(buffer), 0);
			char subbuff[byte_count + 1];
			memcpy( subbuff, &buffer[0], byte_count );
			subbuff[byte_count] = '\0';
			cout << "byte_count: " << byte_count << " sub buffer: " << subbuff << endl;
//			char toCompare[byte_count + 1];
//			strcpy(toCompare, subbuff);
			
//			CompareTwoCharArrays(subbuff, okArray);
			
			cout << " comparison now" << ( strcmp(subbuff,"ok") == 0 )<< endl;
			
//			char buffer2[56];
			byte_count = recv(sock, buffer, sizeof(buffer), 0);
			char subbuff2[byte_count + 1];
			memcpy( subbuff2, &buffer[0], byte_count );
			subbuff2[byte_count] = '\0';
			cout << "byte_count: " << byte_count << " sub buffer2:[" << subbuff2 << "]"<< endl;
//			cout << "shots fired: ";
//			for(int i = 0; i < byte_count; i++)
//				cout << buffer[i];
//			cout << endl;

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
			cout << "byte_count: " << byte_count << " sub buffer: " << subbuff << endl;
//			char toCompare[byte_count + 1];
//			strcpy(toCompare, subbuff);
			
//			CompareTwoCharArrays(subbuff, okArray);
			
			cout << " comparison move" << ( strcmp(subbuff,"ok") == 0 )<< endl;
			
//			cout << endl << "char: [" << oneChar << "] = [" << toSend << "] | [" << oneCharInt << "] sent" << endl;

	//		std::cout.put( onechar );
		}

	}
	
	return 0;
}
