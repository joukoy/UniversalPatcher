#include "pch.h"
#include <iostream>
#include <fstream>
#include <sstream>
#include <string>
#include <thread>
#include "windows.h"
#include <queue>
#include "SafeQueue.h"
#include "SafeQueue.cpp"
#include <tchar.h>
#include <atlstr.h>
using namespace std;

#define EXPORT comment(linker, "/EXPORT:" __FUNCTION__ "=" __FUNCDNAME__)

#define crc16  1
#define crc32  2
#define Bytesum  3
#define Wordsum  4
#define Dwordsum  5
#define BoschInv  6
#define Ngc3    7
/*
unsigned int rangestart = 0;
unsigned int rangeend = 0;
unsigned int minrangelen = 1;
unsigned int threads = 1;
unsigned int csbytes = 1;
unsigned int *csaddress;
unsigned int csACount = 0;
unsigned int method = 1;
unsigned int complement = 0;
BOOL msb = true;
BOOL swapbytes = true;
BOOL skipCsAddress = true;
*/
unsigned char* buffer;
HANDLE* handles;
BOOL StopMe;

UINT16 CrcTable16[256];
UINT32 CrcTable32[256];
UINT16 Ngc3Table1[16];
UINT16 Ngc3Table2[16];
UINT16 Ngc3Table3[16];
UINT16 Ngc3Table4[16];

struct StartEnd
{
    UINT32 start;
    UINT32 end;
};
struct Result
{
    UINT32 Start;
    UINT32 End;
    UINT32 CsAddress;
    UINT64 Cheksum;
    int Method;
    BYTE Complement;
    BOOL ByteSwap;
};
struct SearchSettings
{
    UINT32 Start;
    UINT32 End;
    UINT32 MinRangeLen;
    int Method;
    int Complement;
    int CsBytes;
    BOOL MSB;
    BOOL SwapBytes;
    BOOL NoSwapBytes;
    BOOL SkipCsAddress;
    int CsValueCount;
    int Threads;
    UINT64 InitialValue;
    UINT64 Polynomial;
    UINT64 Xor;
    int FilterCount;
    int CsAddressCount;
    UINT32* CsAddresses;
};


SafeQueue<Result> outq;
SafeQueue<StartEnd> workqueue;
SearchSettings *searchsettings;
UINT64* csvalues;
UINT64* filtervalues;

DWORD WINAPI CalcWordSum(_In_  LPVOID lpParameter)
{
    while (!workqueue.empty())
    {
        StartEnd range = workqueue.dequeue();
        //std::cout << id << range.start << "-" << range.end;
        for (UINT32 newStart = range.start;newStart <= range.end;newStart++)
        {
            UINT32 sum = 0;
            for (UINT32 a = newStart;a <= searchsettings->End;a += 2)
            {
                sum += (UINT16)(buffer[a] << 8 | buffer[a + 1]);
                UINT32 cs = (UINT32)(buffer[a + 2] << 24 | buffer[a + 3] << 16 | buffer[a + 4] << 8 | buffer[a + 5]);
                UINT32 csCalc = ~sum + 1;
                if (cs == csCalc && (a- newStart) >= searchsettings->MinRangeLen)
                {
                    BOOL skip = false;
                    for (int f = 0;f < searchsettings->FilterCount;f++)
                    {
                        if (filtervalues[f] == csCalc)
                        {
                            skip = true;
                            break;
                        }
                    }
                    if (!skip)
                    {
                        Result r;
                        r.Start = newStart;
                        r.End = a + 1;
                        r.CsAddress = a + 2;
                        r.Cheksum = cs;
                        r.Method = Wordsum;
                        r.Complement = 2;
                        r.ByteSwap = false;
                        outq.enqueue(r);
                    }
                }
            }
            if (StopMe)
            {
                return 1;
            }
        }
    }
    return 0;
}
void InitCrc16()
{
    UINT16 polynomial = 0xA001;
    UINT16 value;
    UINT16 temp;

    if (searchsettings->Polynomial <= UINT16_MAX)
    {
        polynomial = (UINT16)searchsettings->Polynomial;
    }

    for (UINT16 i = 0; i < 256; ++i)
    {
        value = 0;
        temp = i;
        for (BYTE j = 0; j < 8; ++j)
        {
            if (((value ^ temp) & 0x0001) != 0)
            {
                value = (UINT16)((value >> 1) ^ polynomial);
            }
            else
            {
                value >>= 1;
            }
            temp >>= 1;
        }
        CrcTable16[i] = value;
    }
}

void  InitCrc32()
{
    UINT32 poly = 0xedb88320;
    UINT32 temp = 0;

    if (searchsettings->Polynomial <= UINT32_MAX)
    {
        poly = (UINT32)searchsettings->Polynomial;
    }
    for (int i = 0; i < 256; ++i)
    {
        temp = i;
        for (int j = 8; j > 0; --j)
        {
            if ((temp & 1) == 1)
            {
                temp = (UINT32)((temp >> 1) ^ poly);
            }
            else
            {
                temp >>= 1;
            }
        }
        CrcTable32[i] = temp;
    }
}

vector<string> split(string str, string token) {
    vector<string>result;
    while (str.size()) {
        int index = str.find(token);
        if (index != string::npos) {
            result.push_back(str.substr(0, index));
            str = str.substr(index + token.size());
            if (str.size() == 0)result.push_back(str);
        }
        else {
            result.push_back(str);
            str = "";
        }
    }
    return result;
}
string thisDllDirPath()
{
    CStringW thisPath = L"";
    WCHAR path[MAX_PATH];
    HMODULE hm;
    if (GetModuleHandleExW(GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS |
        GET_MODULE_HANDLE_EX_FLAG_UNCHANGED_REFCOUNT,
        (LPWSTR)&thisDllDirPath, &hm))
    {
        GetModuleFileNameW(hm, path, MAX_PATH);
        PathRemoveFileSpecW(path);
        thisPath = CStringW(path);
        if (!thisPath.IsEmpty() &&
            thisPath.GetAt(thisPath.GetLength() - 1) != '\\')
            thisPath += L"\\";
    }
    //	else if (_DEBUG) std::wcout << L"GetModuleHandle Error: " << GetLastError() << std::endl;

    //	if (_DEBUG) std::wcout << L"thisDllDirPath: [" << CStringW::PCXSTR(thisPath) << L"]" << std::endl;
    return { thisPath.GetString(), thisPath.GetString() + thisPath.GetLength() };
}
void InitNgc3()
{
    ifstream myfile(thisDllDirPath() + "XML\\ngc3tables.txt");
    int row = 0;
    string line;
    std::vector<std::string> fileLines;
    const char* hex = "0123456789ABCDEF";
    if (myfile && myfile.is_open())
    {
        if (getline(myfile, line))
        {
            vector<string> valstrs = split(line, ",");
            for (int x = 0;x < 16;x++)
            {
                Ngc3Table1[x] = stoi(valstrs[x], 0, 16);
            }
        }
        if (getline(myfile, line))
        {
            vector<string> valstrs = split(line, ",");
            for (int x = 0;x < 16;x++)
            {
                Ngc3Table2[x] = stoi(valstrs[x], 0, 16);
            }
        }
        if (getline(myfile, line))
        {
            vector<string> valstrs = split(line, ",");
            for (int x = 0;x < 16;x++)
            {
                Ngc3Table3[x] = stoi(valstrs[x], 0, 16);
            }
        }
        if (getline(myfile, line))
        {
            vector<string> valstrs = split(line, ",");
            for (int x = 0;x < 16;x++)
            {
                Ngc3Table4[x] = stoi(valstrs[x], 0, 16);
            }
        }
    }
    myfile.close();
}

DWORD WINAPI CalcCheckSum(_In_  LPVOID lpParameter)
{
    UINT64 sum;
    UINT64 csCalc;
    UINT64* cs; //Checksum array
    BYTE index;
    StartEnd range;
    int step = 1;
    if (searchsettings->Method == Wordsum || searchsettings->Method == BoschInv || searchsettings->Method == Ngc3)
        step = 2;
    else if (searchsettings->Method == Dwordsum)
        step = 4;


    if (searchsettings->CsValueCount > 0)
    {
        cs = csvalues;
    }
    else
    {
        cs = new UINT64[searchsettings->CsAddressCount];
        for (int c = 0; c < searchsettings->CsAddressCount;c++)
        {
            UINT32 csAddr = searchsettings->CsAddresses[c];
            if (csAddr < UINT32_MAX)
            {
                switch (searchsettings->CsBytes)
                {
                case 1:
                    cs[c] = (BYTE)buffer[csAddr];
                    break;
                case 2:
                    if (searchsettings->MSB)
                        cs[c] = (UINT16)(buffer[csAddr] << 8 | buffer[csAddr + 1]);
                    else
                        cs[c] = (UINT16)(buffer[csAddr + 1] << 8 | buffer[csAddr]);
                    break;
                case 4:
                    if (searchsettings->MSB)
                        cs[c] = (UINT32)(buffer[csAddr] << 24 | buffer[csAddr + 1] << 16 | buffer[csAddr + 2] << 8 | buffer[csAddr + 3]);
                    else
                        cs[c] = (UINT32)(buffer[csAddr + 3] << 24 | buffer[csAddr + 2] << 16 | buffer[csAddr + 1] << 8 | buffer[csAddr]);
                    break;
                case 8:
                    if (searchsettings->MSB)
                        cs[c] = (UINT64)((UINT64)buffer[csAddr] << 56 | (UINT64)buffer[csAddr + 1] << 48 | (UINT64)buffer[csAddr + 2] << 40 | (UINT64)buffer[csAddr + 3] << 32 | (UINT64)buffer[csAddr + 4] << 24 | (UINT64)buffer[csAddr + 5] << 16 | (UINT64)buffer[csAddr + 6] << 8 | buffer[csAddr + 7]);
                    else
                        cs[c] = (UINT64)((UINT64)buffer[csAddr + 7] << 56 | (UINT64)buffer[csAddr + 6] << 48 | (UINT64)buffer[csAddr + 5] << 40 | (UINT64)buffer[csAddr + 4] << 32 | (UINT64)buffer[csAddr + 3] << 24 | (UINT64)buffer[csAddr + 2] << 16 | (UINT64)buffer[csAddr + 1] << 8 | buffer[csAddr]);
                    break;
                }
            }
            else
            {
                cs[c] = 0;  //Make compiler happy
            }
        }
    }
    while (!workqueue.empty())
    {
        range = workqueue.dequeue();
        //std::cout << id << range.start << "-" << range.end;

        for (UINT32 newStart = range.start;newStart <= range.end;newStart++)
        {
            sum = searchsettings->InitialValue;
            if (searchsettings->Method == crc32)
                sum = 0xffffffff;
            for (UINT32 a = newStart;a <= searchsettings->End;a += step)
            {
                if (searchsettings->SkipCsAddress)
                {
                    for (int c = 0; c < searchsettings->CsAddressCount;c++)
                    {
                        UINT32 csAddr = searchsettings->CsAddresses[c];
                        if (a >= csAddr && a <= (csAddr + searchsettings->CsBytes))
                        {
                            //Skip Checksum address
                            continue;
                        }
                    }
                }
                switch (searchsettings->Method)
                {
                    case Bytesum:
                        sum += buffer[a];
                        csCalc = sum;
                        break;
                    case Wordsum:
                        if (searchsettings->MSB)
                            sum += (UINT16)(buffer[a] << 8 | buffer[a + 1]);
                        else
                            sum += (UINT16)(buffer[a+1] << 8 | buffer[a]);
                        csCalc = sum;
                        break;
                    case Dwordsum:
                        if (searchsettings->MSB)
                            sum += (UINT32)(buffer[a] << 24 | buffer[a + 1] << 16 | buffer[a + 2] << 8 | buffer[a + 3]);
                        else
                            sum += (UINT32)(buffer[a+3] << 24 | buffer[a + 2] << 16 | buffer[a + 1] << 8 | buffer[a]);
                        csCalc = sum;
                        break;
                    case BoschInv:
                        if (searchsettings->MSB)
                            sum += (UINT16)(buffer[a] << 8 | buffer[a + 1]);
                        else
                            sum += (UINT16)(buffer[a + 1] << 8 | buffer[a]);
                        csCalc = sum;
                        for (int c = 0; c < searchsettings->CsAddressCount;c++)
                        {
                            UINT32 csAddr = searchsettings->CsAddresses[c];
                            if (csAddr >= newStart && csAddr <= (a + 8))
                            {
                                csCalc = 0x1FFFE + csCalc;
                            }
                        }
                        if (searchsettings->MSB)
                        {
                            csCalc = (csCalc << 32) + (0xFFFFFFFF - csCalc);
                        }
                        else
                        {
                            csCalc = ((0xFFFFFFFF - csCalc) << 32) + csCalc;
                        }
                        break;
                    case crc16:
                        index = (BYTE)(sum ^ buffer[a]);
                        sum = (UINT16)((sum >> 8) ^ CrcTable16[index]);
                        csCalc = sum;
                        break;
                    case crc32:
                        index = (BYTE)(((sum) & 0xff) ^ buffer[a]);
                        sum = (UINT32)((sum >> 8) ^ CrcTable32[index]);
                        csCalc = ~sum;
                        break;
                    case Ngc3:
                        UINT16 v3;
                        if (searchsettings->MSB)
                            v3 = (UINT16)(buffer[a] << 8 | buffer[a + 1]);
                        else
                            v3 = (UINT16)(buffer[a + 1] << 8 | buffer[a]);
                        v3 = sum ^ v3;
                        sum = (UINT16)(Ngc3Table1[(v3 >> 12)] ^
                            Ngc3Table2[(v3 & 0xF00) >> 8] ^
                            Ngc3Table3[(v3 & 0xF0) >> 4] ^
                            Ngc3Table4[(v3 & 0x0F)]);
                        csCalc = sum;
                        break;
                    default:
                        break;
                }
                if ((a - newStart) < searchsettings->MinRangeLen)
                {
                    continue;
                }
                if (searchsettings->CsAddresses[0] == UINT32_MAX)
                {
                    UINT32 csAddr = a + step;
                    switch (searchsettings->CsBytes)
                    {
                    case 1:
                        cs[0] = (BYTE)buffer[csAddr];
                        break;
                    case 2:
                        if (searchsettings->MSB)
                            cs[0] = (UINT16)(buffer[csAddr] << 8 | buffer[csAddr + 1]);
                        else
                            cs[0] = (UINT16)(buffer[csAddr + 1] << 8 | buffer[csAddr]);
                        break;
                    case 4:
                        if (searchsettings->MSB)
                            cs[0] = (UINT32)(buffer[csAddr] << 24 | buffer[csAddr + 1] << 16 | buffer[csAddr + 2] << 8 | buffer[csAddr + 3]);
                        else
                            cs[0] = (UINT32)(buffer[csAddr + 3] << 24 | buffer[csAddr + 2] << 16 | buffer[csAddr + 1] << 8 | buffer[csAddr]);
                        break;
                    case 8:
                        if (searchsettings->MSB)
                            cs[0] = (UINT64)((UINT64)buffer[csAddr] << 56 | (UINT64)buffer[csAddr + 1] << 48 | (UINT64)buffer[csAddr + 2] << 40 | (UINT64)buffer[csAddr + 3] << 32 | (UINT64)buffer[csAddr + 4] << 24 | (UINT64)buffer[csAddr + 5] << 16 | (UINT64)buffer[csAddr + 6] << 8 | buffer[csAddr + 7]);
                        else
                            cs[0] = (UINT64)((UINT64)buffer[csAddr + 7] << 56 | (UINT64)buffer[csAddr + 6] << 48 | (UINT64)buffer[csAddr + 5] << 40 | (UINT64)buffer[csAddr + 4] << 32 | (UINT64)buffer[csAddr + 3] << 24 | (UINT64)buffer[csAddr + 2] << 16 | (UINT64)buffer[csAddr + 1] << 8 | buffer[csAddr]);
                        break;
                    }
                }

                for (int swb = 0; swb <= 1; swb++)
                {
                    BOOL swapBytes;
                    if (swb == 0) //Don't swap bytes
                    {
                        if (!searchsettings->NoSwapBytes)
                            continue;
                        swapBytes = false;
                    }
                    else //swb ==1 (Swap bytes)
                    {
                        if (!searchsettings->SwapBytes)
                            continue;
                        swapBytes = true;
                    }
                    for (int comp = 0; comp <= 2; comp++)
                    {
                        if (comp == 0 && (searchsettings->Complement & 4) != 4)
                            continue;   //Not selected, skip
                        if (comp == 1 && (searchsettings->Complement & 1) != 1)
                            continue;
                        if (comp == 2 && (searchsettings->Complement & 2) != 2)
                            continue;
                        if (comp == 1)
                            csCalc = ~sum;
                        else if (comp == 2)
                            csCalc = ~sum + 1;

                        csCalc = csCalc ^ searchsettings->Xor;

                        switch (searchsettings->CsBytes)
                        {
                        case 1:
                            csCalc = (csCalc & 0xFF);
                            break;
                        case 2:
                            if (swapBytes)
                                csCalc = csCalc && 0xFF << 8 | csCalc & 0xFF00 >> 8;
                            else
                                csCalc = (csCalc & 0xFFFF);
                            break;
                        case 4:
                            if (swapBytes)
                                csCalc = csCalc && (UINT64)0xFF << 24 | csCalc & 0xFF00 << 8 | csCalc && 0xFF0000 >> 8 | csCalc & 0xFF000000 >> 24;
                            else
                                csCalc = (csCalc & 0xFFFFFFFF);
                            break;
                        case 8:
                            if (swapBytes)
                            {
                                csCalc = (csCalc & 0x00000000FFFFFFFF) << 32 | (csCalc & 0xFFFFFFFF00000000) >> 32;
                                csCalc = (csCalc & 0x0000FFFF0000FFFF) << 16 | (csCalc & 0xFFFF0000FFFF0000) >> 16;
                                csCalc = (csCalc & 0x00FF00FF00FF00FF) << 8 | (csCalc & 0xFF00FF00FF00FF00) >> 8;
                            }
                        }
                        BOOL skip = false;
                        for (int f = 0;f < searchsettings->FilterCount;f++)
                        {
                            if (filtervalues[f] == csCalc)
                            {
                                skip = true;
                                break;
                            }
                        }
                        if (skip)
                        {
                            continue;
                        }
                        if (searchsettings->CsAddresses[0] == UINT32_MAX)
                        {
                            if (cs[0] == csCalc)
                            {
                                UINT32 csAddr = a + step;
                                Result result;
                                result.Start = newStart;
                                result.End = a + (step - 1);
                                result.CsAddress = csAddr;
                                result.Cheksum = cs[0];
                                result.Method = searchsettings->Method;
                                result.Complement = comp;
                                result.ByteSwap = swapBytes;
                                outq.enqueue(result);
                            }

                        }
                        else
                        {
                            for (int c = 0; c < searchsettings->CsAddressCount;c++)
                            {
                                if (cs[c] == csCalc)
                                {
                                    UINT32 csAddr = searchsettings->CsAddresses[c];
                                    Result result;
                                    result.Start = newStart;
                                    result.End = a + (step - 1);
                                    result.CsAddress = csAddr;
                                    result.Cheksum = cs[c];
                                    result.Method = searchsettings->Method;
                                    result.Complement = comp;
                                    result.ByteSwap = swapBytes;
                                    outq.enqueue(result);
                                }
                            }
                        }
                    }
                }
            }
            if (StopMe)
            {
                return 1;
            }
        }
    }
    return 0;
}

int WINAPI CheckSumSearch(unsigned char* Buffer, SearchSettings *Settings, UINT64 *CsValues, UINT64* FilterValues)
{
#pragma EXPORT
     
    buffer = Buffer;
    searchsettings = Settings;
    csvalues = CsValues;
    filtervalues = FilterValues;
    handles = new HANDLE[Settings->Threads];
    
    int qSize = 0;
    StopMe = false;
    if (searchsettings->Method == crc16)
        InitCrc16();
    if (searchsettings->Method == crc32)
        InitCrc32();
    if (searchsettings->Method == Ngc3)
        InitNgc3();

    std::cout << "Range: " << std::hex << searchsettings->Start;
    std::cout << " - " << std::hex << searchsettings->End << "\n";
    UINT32 step = (UINT32)((searchsettings->End - searchsettings->Start) / searchsettings->Threads / 1.5);
    if (step == 0) step = 1;
    if (step > 100) step = 100;
    UINT32 startMin = searchsettings->Start;
    UINT32 startMax = startMin;
    while (startMax < (searchsettings->End - searchsettings->MinRangeLen))
    {
        startMax = startMin + step;
        if ((searchsettings->End - startMax) < step)
        {
            startMax = searchsettings->End;
        }
        StartEnd range;
        range.start = startMin;
        range.end = startMax;
        workqueue.enqueue(range);
        startMin = startMax + 1;
    }
    qSize = workqueue.size();
    for (int id = 0; id < searchsettings->Threads;id++)
    {
        if (searchsettings->Method == Wordsum && searchsettings->Complement == 2 && searchsettings->CsBytes == 4  && 
            searchsettings->CsAddresses[0] == UINT32_MAX && searchsettings->MSB && !searchsettings->SwapBytes && searchsettings->Xor == 0) //Full optimized method for Wordsum,2's complement, MSB
            handles[id] = CreateThread(0, 0, &CalcWordSum, 0, 0, 0);
        else
            handles[id] = CreateThread(0, 0, &CalcCheckSum, 0, 0, 0);
        Sleep(50);
    }
    return qSize;
}

void WINAPI ChecksumSearchGetResults(Result *result)
{
#pragma EXPORT
    Result r;
    if (outq.empty())
    {
        r.Start = UINT32_MAX;
    }
    else
    {
        r = outq.dequeue();
        //outq.pop();
    }
    memcpy(result, &r, sizeof(r));
}

BOOL WINAPI ChecksumSearchIsRunning()
{
#pragma EXPORT
    BOOL active = false;
    DWORD dwExitCode;
    for (INT_PTR id = 0; id < searchsettings->Threads;id++)
    {
        GetExitCodeThread(handles[id], &dwExitCode);
        if (dwExitCode == STILL_ACTIVE)
        {
            active = true;
            break;
        }
    }
    return active;
}
int WINAPI ChecksumSearchGetQueueSize()
{
#pragma EXPORT
    return workqueue.size();
}

void WINAPI ChecksumSearchStop()
{
#pragma EXPORT
    StopMe = true;
}
