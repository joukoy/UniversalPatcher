using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using static Helpers;

namespace UniversalPatcher
{
    public class ObdEmu
    {
        public ObdEmu(byte ID)
        {
            Responses = new List<OBDResponse>();
            ToolID = 0;
            MyID = ID;
        }
        public List<OBDResponse> Responses { get; set; }
        public byte MyID { get; set; }
        public byte ToolID { get; set; }

        public class OBDResponse
        {
            public string Message { get; set; }
            public string Response { get; set; }            
        }

        public static List<OBDResponse> LoadObdResponseFile(string FileName)
        {
            List<OBDResponse> or = new List<OBDResponse>();
            Logger(" (" + Path.GetFileName(FileName) + ") ", false);
            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<OBDResponse>));
            System.IO.StreamReader file = new System.IO.StreamReader(FileName);
            or = (List<OBDResponse>)reader.Deserialize(file);
            file.Close();
            return or;
        }

        public List<string> GetResponses(OBDMessage message)
        {
            List<string> resps = new List<string>();
            if (message.Length < 4)
            {
                Debug.WriteLine("OBDEmu: Short message");
                return resps;
            }

            if (ToolID == 0)                
            {
                if (message[2] != MyID)
                {
                    Debug.WriteLine("OBDEmu: Found tool ID: " + message[2].ToString("X"));
                    ToolID = message[2];
                }
                else
                {
                    Debug.WriteLine("OBDEmu: Message sent from My ID?? " + message[2].ToString("X"));
                    return resps;
                }
            }

            if (message[1] != MyID || message[2] != ToolID) 
            {
                Debug.WriteLine("OBDEmu: Filter message, not for me");
                return resps;   //Filter, not for me
            }

            string msgStr = message.ToString();
            for (int i=0; i<Responses.Count; i++)
            {
                OBDResponse or = Responses[i];
                if (or.Message.Trim() == msgStr.Trim())
                {
                    Debug.WriteLine("OBDEmu: message found from responses file");
                    string[] parts = or.Response.Split(',');
                    resps.AddRange(parts);
                    break;
                }
            }

            if (resps.Count == 0)
            {
                Debug.WriteLine("OBDEmu: generating response");
                if (message[3] == 0x2c)
                {
                    byte[] msg = new byte[6];
                    msg[0] = message[0];
                    msg[1] = ToolID;
                    msg[2] = MyID;
                    msg[3] = (byte)(message[3] + 0x40);
                    msg[4] = message[4];
                    msg[5] = message[5];
                    resps.Add(BitConverter.ToString(msg).Replace("-", " "));
                }
                else
                {
                    byte[] msg = new byte[message.Length];
                    Array.Copy(message.GetBytes(), msg, message.Length);
                    msg[1] = ToolID;
                    msg[2] = MyID;
                    resps.Add(BitConverter.ToString(msg).Replace("-", " "));
                }
            }
            return resps;
        }
    }
}
