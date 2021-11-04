// Client side C/C++ program to demonstrate Socket programming
#include <stdio.h>
#include <sys/socket.h>
#include <arpa/inet.h>
#include <unistd.h>
#include <string.h>
#include <iostream>

#define KEY_UP 72
#define KEY_DOWN 80
#define KEY_LEFT 75
#define KEY_RIGHT 77
#define PORT 8052

using namespace std;

int main(int argc, char const *argv[])
{
	int sock = 0;// valread;
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
	
	std::cout << "sock: " << sock << std::endl;
	
//	send(sock , hello , strlen(hello) , 0 );
//	printf("Hello message sent\n");
//	valread = read( sock , buffer, 1024);
//	printf("%s\n",buffer );


//	char oneChar; // Note: char cannot represent EOF
	while (true){
//	while ( (oneChar = std::cin.get()) != 27  ) { // EOF  // 27 escape
		// Set terminal to raw mode 
		system("stty raw"); 
		char oneChar = getchar();
		int oneCharInt = oneChar;
		if (oneChar == 27) {
			system("stty cooked");
			string quit = "quit";
			const char* toSend = quit.c_str();
			send(sock , toSend , strlen(toSend) , 0 );
			shutdown(sock,2);
			break;
		}
		if (oneChar == 13) {
			string start = "start";
			const char* toSend = start.c_str();
			send(sock , toSend , strlen(toSend) , 0 );
			continue;
		}
		// Reset terminal to normal "cooked" mode 
		system("stty cooked");
		
		string toSendStr = string(1,(char) oneChar);
		const char* toSend = toSendStr.c_str();
		send(sock , toSend , strlen(toSend) , 0 );
//		cout << endl << "char: [" << oneChar << "] = [" << toSend << "] | [" << oneCharInt << "] sent" << endl;

//		std::cout.put( onechar );
	}
  

	return 0;
}
