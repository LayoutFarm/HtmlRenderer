//MIT, 2015, WinterDev
//MIT, 2015-2016, EngineKit, brezza92
#include <vector>
#include <iostream>
#include <string>
#include "espresso.h" 

using namespace v8;


BinaryStreamReader::BinaryStreamReader(const char* stream,int length)
{
	this->length = length;
	this->pos =0;
	this->stream = stream;
}
int BinaryStreamReader:: ReadInt16()
{

	int start= this->pos; 
	//carefully ***
	if(start + 2 > this->length)
	{
		return 0;
	}
	char byte0= this->stream[start];
	start++;
	char byte1= this->stream[start];
	start++;
	//  little endian 
	this->pos= start;
	return byte0 | (byte1 <<8);  		 
}
int BinaryStreamReader:: ReadInt32()
{			
	int start= this->pos;
	//carefully ***
	if(start + 4 > this->length)
	{	
		return 0;
	}
	char byte0= this->stream[start];
	start++;
	char byte1= this->stream[start];
	start++;
	char byte2= this->stream[start];
	start++;
	char byte3= this->stream[start];
	start++;
	//  little endian 
	this->pos= start;
	return byte0 | (byte1 <<8) |(byte2<<16) | (byte3<<24);  
}
std::wstring BinaryStreamReader:: ReadUtf16String()
{
	//carefully ***
	int strLen = this->ReadInt16();
	if(strLen>0)
	{	
		int start= this->pos;
		//-----------------------------------
		if(start + strLen > this->length)
		{	
			return std::wstring();
		}
		//-----------------------------------
		std::wstring outputstr;
		//copy data
		int index=0;
		wchar_t* strBuffer=new wchar_t[strLen+1];
		for(int i=0;i<strLen;++i)
		{
			char byte0= this->stream[start];
			start++;
			char byte1= this->stream[start];
			start++;				
			//outputstr.append( (wchar_t)(byte0 | (byte1 <<8))			    
			strBuffer[i] = (wchar_t)(byte0 | (byte1 <<8));
		}
		this->pos= start;
		strBuffer[strLen]=0;//  null terminate character
		std::wstring output = std::wstring(strBuffer); 
		return output;
	}
	return std::wstring();
} 