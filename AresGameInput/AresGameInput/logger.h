#include <fstream>
#include <iostream>
#include <string>
#include <chrono>

using namespace std;

ofstream ofs;
std::chrono::steady_clock::time_point begin;// = std::chrono::steady_clock::now();
std::chrono::steady_clock::time_point end;// = std::chrono::steady_clock::now();

void StartTimer(){
	::begin = std::chrono::steady_clock::now();
}

inline string getCurrentDateTime( string s ){
    time_t now = time(0);
    struct tm  tstruct;
    char  buf[80];
    tstruct = *localtime(&now);
    if(s=="now")
        strftime(buf, sizeof(buf), "%Y-%m-%d %X", &tstruct);
    else if(s=="date")
        strftime(buf, sizeof(buf), "%Y-%m-%d", &tstruct);
    return string(buf);
};

void OpenFile(){
	string filePath = "./log_"+getCurrentDateTime("now")+".txt";
    ofs = ofstream(filePath.c_str(), std::ios_base::out | std::ios_base::app );
}

void CloseFile(int shotsFired){
	::end = std::chrono::steady_clock::now();
//	string timeDifference = "" + std::chrono::duration_cast<std::chrono::seconds> (::end - ::begin).count() + "[s]";
	string now = getCurrentDateTime("now");
//	string finalMsg =  " || Shots fired: " + shotsFired + " || Hits on target: " + hitsOnTarget;
    ofs << now << '\t' << "Time elapsed: " << std::chrono::duration_cast<std::chrono::milliseconds> (::end - ::begin).count() << "[ms]";
	ofs << " || Shots fired: " << shotsFired << '\n';
	ofs.close();
}

inline void Logger( string logMsg ){
//    string filePath = "./log_"+getCurrentDateTime("date")+".txt";
    string now = getCurrentDateTime("now");
//    ofstream ofs(filePath.c_str(), std::ios_base::out | std::ios_base::app );
    ofs << now << '\t' << logMsg << '\n';
//    ofs.close();
}