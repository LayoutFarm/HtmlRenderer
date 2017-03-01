//MIT, 2014-2017, WinterDev 
using System;
using System.IO;
using System.Text;
namespace PixelFarm.Drawing.Fonts
{
    public static class FontPreview
    {
        //TODO: merge this to Typography

        //from http://stackoverflow.com/questions/3633000/net-enumerate-winforms-font-styles
        // https://www.microsoft.com/Typography/OTSpec/name.htm

        public static InstalledFont GetFontDetails(string fontFilePath)
        {
            //TODO: review here
            //merge to the Typography?

            string fontName = string.Empty;
            string fontSubFamily = string.Empty;

            string strRet = string.Empty;

            using (FileStream fs = new FileStream(fontFilePath, FileMode.Open, FileAccess.Read))
            {

                TT_OFFSET_TABLE ttOffsetTable = new TT_OFFSET_TABLE()
                {
                    uMajorVersion = ReadUShort(fs),
                    uMinorVersion = ReadUShort(fs),
                    uNumOfTables = ReadUShort(fs),
                    uSearchRange = ReadUShort(fs),
                    uEntrySelector = ReadUShort(fs),
                    uRangeShift = ReadUShort(fs),
                };

                if (ttOffsetTable.uMajorVersion != 1 || ttOffsetTable.uMinorVersion != 0)
                {
                    return null;
                }


                TT_TABLE_DIRECTORY tblDir = new TT_TABLE_DIRECTORY();
                bool found = false;
                Encoding enc = Encoding.UTF8;
                for (int i = 0; i <= ttOffsetTable.uNumOfTables; i++)
                {
                    tblDir = new TT_TABLE_DIRECTORY();
                    tblDir.Initialize();
                    fs.Read(tblDir.szTag, 0, tblDir.szTag.Length);
                    tblDir.uCheckSum = ReadULong(fs);
                    tblDir.uOffset = ReadULong(fs);
                    tblDir.uLength = ReadULong(fs);


                    string s = enc.GetString(tblDir.szTag);

                    if (s.CompareTo("name") == 0)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found) return null;

                fs.Seek(tblDir.uOffset, SeekOrigin.Begin);

                TT_NAME_TABLE_HEADER ttNTHeader = new TT_NAME_TABLE_HEADER
                {
                    uFSelector = ReadUShort(fs),
                    uNRCount = ReadUShort(fs),
                    uStorageOffset = ReadUShort(fs)
                };

                TT_NAME_RECORD ttRecord = new TT_NAME_RECORD();

                for (int j = 0; j <= ttNTHeader.uNRCount; j++)
                {
                    ttRecord = new TT_NAME_RECORD()
                    {
                        uPlatformID = ReadUShort(fs),
                        uEncodingID = ReadUShort(fs),
                        uLanguageID = ReadUShort(fs),
                        uNameID = ReadUShort(fs),
                        uStringLength = ReadUShort(fs),
                        uStringOffset = ReadUShort(fs)
                    };

                    //if (ttRecord.uNameID > 2)
                    //{

                    //}

                    long nPos = fs.Position;
                    fs.Seek(tblDir.uOffset + ttRecord.uStringOffset + ttNTHeader.uStorageOffset, SeekOrigin.Begin);

                    byte[] buf = new byte[ttRecord.uStringLength];
                    fs.Read(buf, 0, ttRecord.uStringLength);

                    Encoding enc2;
                    if (ttRecord.uEncodingID == 3 || ttRecord.uEncodingID == 1)
                    {
                        enc2 = Encoding.BigEndianUnicode;
                    }
                    else
                    {
                        enc2 = Encoding.UTF8;
                    }
                    strRet = enc2.GetString(buf);
                    switch (ttRecord.uNameID)
                    {
                        case 1:
                            fontName = strRet;
                            break;
                        case 2:
                            fontSubFamily = strRet;
                            break;
                        default:

                            break;
                    }
                    fs.Seek(nPos, SeekOrigin.Begin);
                }


                return new InstalledFont(fontName, fontSubFamily, fontFilePath);
            }
        }

        struct TT_OFFSET_TABLE
        {
            public ushort uMajorVersion;
            public ushort uMinorVersion;
            public ushort uNumOfTables;
            public ushort uSearchRange;
            public ushort uEntrySelector;
            public ushort uRangeShift;
        }

        struct TT_TABLE_DIRECTORY
        {
            public byte[] szTag;
            public UInt32 uCheckSum;
            public UInt32 uOffset;
            public UInt32 uLength;
            public void Initialize()
            {
                szTag = new byte[4];
            }
        }

        struct TT_NAME_TABLE_HEADER
        {
            public ushort uFSelector;
            public ushort uNRCount;
            public ushort uStorageOffset;
        }

        struct TT_NAME_RECORD
        {
            public ushort uPlatformID;
            public ushort uEncodingID;
            public ushort uLanguageID;
            public ushort uNameID;
            public ushort uStringLength;
            public ushort uStringOffset;
        }

        static private UInt16 ReadChar(FileStream fs, int characters)
        {
            string[] s = new string[characters];
            byte[] buf = new byte[Convert.ToByte(s.Length)];

            buf = ReadAndSwap(fs, buf.Length);
            return BitConverter.ToUInt16(buf, 0);
        }

        static private UInt16 ReadByte(FileStream fs)
        {
            byte[] buf = new byte[11];
            buf = ReadAndSwap(fs, buf.Length);
            return BitConverter.ToUInt16(buf, 0);
        }

        static private UInt16 ReadUShort(FileStream fs)
        {
            byte[] buf = new byte[2];
            buf = ReadAndSwap(fs, buf.Length);
            return BitConverter.ToUInt16(buf, 0);
        }

        static private UInt32 ReadULong(FileStream fs)
        {
            byte[] buf = new byte[4];
            buf = ReadAndSwap(fs, buf.Length);
            return BitConverter.ToUInt32(buf, 0);
        }

        static private byte[] ReadAndSwap(FileStream fs, int size)
        {
            byte[] buf = new byte[size];
            fs.Read(buf, 0, buf.Length);
            Array.Reverse(buf);
            return buf;
        }

    }

}
