#include "pch.h"
#include "Ngc3.h"
class Ngc3
{
    Ngc3()
    {
        GenerateTables();
    }
private:
    UINT16 *Table1;
    UINT16 *Table2;
    UINT16 *Table3;
    UINT16 *Table4;
    void GenerateTables()
    {
        Table1 = new UINT16[]{
            0x0000, 0xE003, 0x4003, 0xA000,
            0x8006, 0x6005, 0xC005, 0x2006,
            0x8009, 0x600A, 0xC00A, 0x2009,
            0x000F, 0xE00C, 0x400C, 0xA00F
        };
        Table2 = new UINT16[]{
            0x0000, 0x8603, 0x8C03, 0x0A00,
            0x9803, 0x1E00, 0x1400, 0x9203,
            0xB003, 0x3600, 0x3C00, 0xBA03,
            0x2800, 0xAE03, 0xA403, 0x2200
        };
        Table3 = new UINT16[]{
            0x0000, 0x8063, 0x80C3, 0x00A0,
            0x8183, 0x01E0, 0x0140, 0x8123,
            0x8303, 0x0360, 0x03C0, 0x83A3,
            0x0280, 0x82E3, 0x8243, 0x0220
        };
        Table4 = new UINT16[]{
            0x0000, 0x8005, 0x800F, 0x000A,
            0x801B, 0x001E, 0x0014, 0x8011,
            0x8033, 0x0036, 0x003C, 0x8039,
            0x0028, 0x802D, 0x8027, 0x0022
        };

    }
public:
    UINT16 CalculateChecksum(BYTE *data, UINT32 start, UINT32 end)
    {
        UINT16 ckSum = 0;
        for (UINT addr = start; addr < end; addr += 2)
        {
            UINT16 v3 = (UINT16)(ckSum ^ ReadUint16(data, addr, true));
            ckSum = (UINT16)(Table1[(v3 >> 12)] ^
                Table2[(v3 & 0xF00) >> 8] ^
                Table3[(v3 & 0xF0) >> 4] ^
                Table4[(v3 & 0x0F)]);
        }
        return ckSum;
    }
};