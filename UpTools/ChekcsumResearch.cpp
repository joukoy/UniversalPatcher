#include "pch.h"
#include <iostream>
#include <string>
#include <thread>
#include "windows.h"
#include <queue>
#include "SafeQueue.h"
#include "SafeQueue.cpp"
#define EXPORT comment(linker, "/EXPORT:" __FUNCTION__ "=" __FUNCDNAME__)

#define crc16  1
#define crc32  2
#define Bytesum  3
#define Wordsum  4
#define Dwordsum  5
#define BoschInv  6
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

const UINT16 polynomial = 0xA001;
UINT16 table[256];
UINT32 table32[256];

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
    BOOL SkipCsAddress;
    int Threads;
    UINT64 InitialValue;
    int CsAddressCount;
    UINT32* CsAddresses;
};


SafeQueue<Result> outq;
SafeQueue<StartEnd> workqueue;
SearchSettings *searchsettings;

DWORD WINAPI CalcWordSum(_In_  LPVOID lpParameter)
{
    INT_PTR id = reinterpret_cast<INT_PTR>(lpParameter);
    //std::cout << "Thread: " << id << "\n";
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
                if (cs > 0 && cs == csCalc && (a- newStart) >= searchsettings->MinRangeLen)
                {
                    Result r;
                    r.Start = newStart;
                    r.End = a + 1;
                    r.CsAddress = a + 2;
                    r.Cheksum = cs;
                    outq.enqueue(r);
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
    UINT16 value;
    UINT16 temp;
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
        table[i] = value;
    }
}

void  InitCrc32()
{
    UINT32 poly = 0xedb88320;
    UINT32 temp = 0;
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
        table32[i] = temp;
    }
}


DWORD WINAPI CalcCheckSum(_In_  LPVOID lpParameter)
{
    INT_PTR id = reinterpret_cast<INT_PTR>(lpParameter);
    UINT64 *cs; 
    UINT64 csCalc;
    //UINT32 csAddr = csaddress[0];
    BYTE index;
    UINT32 sum;
    StartEnd range;
    //std::cout << "Thread: " << id << "\n";
    int step = 1;
    if (searchsettings->Method == Wordsum || searchsettings->Method == BoschInv)
        step = 2;
    else if (searchsettings->Method == Dwordsum)
        step = 4;

    if (searchsettings->Method == crc16)
        InitCrc16();
    if (searchsettings->Method == crc32)
        InitCrc32();

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
                        sum = (UINT16)((sum >> 8) ^ table[index]);
                        csCalc = sum;
                        break;
                    case crc32:
                        index = (BYTE)(((sum) & 0xff) ^ buffer[a]);
                        sum = (UINT32)((sum >> 8) ^ table32[index]);
                        csCalc = ~sum;
                        break;
                    default:
                        break;
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
                if (searchsettings->Complement == 1)
                    csCalc = ~sum;
                else if (searchsettings->Complement == 2)
                    csCalc = ~sum + 1;
               
                switch (searchsettings->CsBytes)
                {
                case 1:
                    csCalc = (csCalc & 0xFF);
                    break;
                case 2:
                    if (searchsettings->SwapBytes)
                        csCalc = csCalc && 0xFF << 8 | csCalc & 0xFF00 >> 8;
                    else
                        csCalc = (csCalc & 0xFFFF);
                    break;
                case 4:
                    if (searchsettings->SwapBytes)
                        csCalc = csCalc && (UINT64)0xFF << 24 | csCalc & 0xFF00 << 8 | csCalc && 0xFF0000 >> 8 | csCalc & 0xFF000000 >> 24;
                    else
                        csCalc = (csCalc & 0xFFFFFFFF);
                    break;
                case 8:
                    if (searchsettings->SwapBytes)
                    {
                        csCalc = (csCalc & 0x00000000FFFFFFFF) << 32 | (csCalc & 0xFFFFFFFF00000000) >> 32;
                        csCalc = (csCalc & 0x0000FFFF0000FFFF) << 16 | (csCalc & 0xFFFF0000FFFF0000) >> 16;
                        csCalc = (csCalc & 0x00FF00FF00FF00FF) << 8 | (csCalc & 0xFF00FF00FF00FF00) >> 8;
                    }
                }
                if (searchsettings->CsAddresses[0] == UINT32_MAX)
                {
                    if (cs[0] == csCalc && (a - newStart) >= searchsettings->MinRangeLen)
                    {
                        UINT32 csAddr = a + step;
                        Result result;
                        result.Start = newStart;
                        result.End = a + (step - 1);
                        result.CsAddress = csAddr;
                        result.Cheksum = cs[0];
                        outq.enqueue(result);
                    }

                }
                else
                {
                    if (a >= 0x2ffb)
                    {
                        std::cout << "Test";
                    }
                    for (int c = 0; c < searchsettings->CsAddressCount;c++)
                    {
                        if (cs[c] == csCalc && (a - newStart) >= searchsettings->MinRangeLen)
                        {
                            UINT32 csAddr = searchsettings->CsAddresses[c];
                            Result result;
                            result.Start = newStart;
                            result.End = a + (step - 1);
                            result.CsAddress = csAddr;
                            result.Cheksum = cs[c];
                            outq.enqueue(result);
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

int WINAPI CheckSumSearch(unsigned char* Buffer, SearchSettings *Settings)
{
#pragma EXPORT
     
    buffer = Buffer;
    searchsettings = Settings;
    //memcpy(&searchsettings, &Settings, sizeof(Settings));
    handles = new HANDLE[Settings->Threads];
    
    /*
    rangestart = Settings.Start;
    rangeend = Settings.End;
    minrangelen = Settings.MinRangeLen;
    csbytes = Settings.CsBytes;
    csaddress = Settings.CsAddresses;
    csACount = Settings.CsAddressCount;
    threads = Settings.Threads;
    method = Settings.Method;
    complement = Settings.Complement;
    msb = Settings.MSB;
    swapbytes = Settings.SwapBytes;
    */

    int qSize = 0;
    StopMe = false;

    std::cout << "Range: " << std::hex << searchsettings->Start;
    std::cout << " - " << std::hex << searchsettings->End << "\n";
    UINT32 step = (UINT32)((searchsettings->End - searchsettings->Start) / searchsettings->Threads / 1.5);
    if (step > 100) step = 100;
    UINT32 startMin = searchsettings->Start;
    UINT32 startMax = startMin;
    while (startMax < searchsettings->End)
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
    for (INT_PTR id = 0; id < searchsettings->Threads;id++)
    {
        //reinterpret_cast<LPVOID>(id);
        if (searchsettings->Method == Wordsum && searchsettings->Complement == 2 && searchsettings->CsBytes == 4  && searchsettings->CsAddresses[0] == UINT32_MAX && searchsettings->MSB && !searchsettings->SwapBytes) //Full optimized method for Wordsum,2's complement, MSB
            handles[id] = CreateThread(0, 0, &CalcWordSum, reinterpret_cast<LPVOID>(id), 0, 0);
        else
            handles[id] = CreateThread(0, 0, &CalcCheckSum, reinterpret_cast<LPVOID>(id), 0, 0);
        Sleep(50);
    }
    return qSize;
}

Result WINAPI ChecksumSearchGetResults()
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
    return r;
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
