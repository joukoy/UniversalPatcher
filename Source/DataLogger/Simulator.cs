using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using static Helpers;

namespace UniversalPatcher
{
    public class Simulator
    {
        public class Response
        {
            public Response()
            {
                Answers = new string[0];
            }
            public Response(string line)
            {
                try
                {
                    Answers = new string[0];
                    MsgBytes = new string[0];
                    //Remove comments:
                    int cPos = line.IndexOf("/");
                    if (cPos > -1)
                    {
                        line = line.Substring(0, cPos);
                    }
                    cPos = line.IndexOf("#");
                    if (cPos > -1)
                    {
                        line = line.Substring(0, cPos);
                    }
                    string[] parts = line.Split('=');
                    if (parts.Length > 1)
                    {
                        MsgBytes = parts[0].Split(' ');
                        Answers = parts[1].Split(';');
                    }
                }
                catch (Exception ex)
                {
                    var st = new StackTrace(ex, true);
                    // Get the top stack frame
                    var frame = st.GetFrame(st.FrameCount - 1);
                    // Get the line number from the stack frame
                    var errline = frame.GetFileLineNumber();
                    Debug.WriteLine("Error, simulator line " + errline + ": " + ex.Message);
                }
            }
            public string[] MsgBytes { get; set; }
            public string[] Answers { get; set; }

        }
        public List<Response> Responses = new List<Response>();
        public Simulator()
        {

        }
        public Simulator(string ResponseFile)
        {
            try
            {
                Logger("Reading response file: " + ResponseFile);
                string fContent = File.ReadAllText(ResponseFile);
                string[] lines = fContent.Split(new char[] {'\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in lines)
                {
                    Response resp = new Response(line);
                    if (resp.Answers.Length > 0)
                    {
                        Responses.Add(resp);
                    }
                }
                Logger(Responses.Count.ToString() + " responses");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, simulator line " + line + ": " + ex.Message);
            }

        }

        public string[] FindResponse(OBDMessage oMsg)
        {
            try
            {
                for (int r = 0; r < Responses.Count; r++)
                {
                    Response resp = Responses[r];
                    bool match = true;
                    for (int b = 0; b < resp.MsgBytes.Length; b++)
                    {
                        if (oMsg.Length < (b + 1))
                        {
                            match = false;
                            break;
                        }
                        if (resp.MsgBytes[b] == "*")
                        {
                            break;
                        }
                        if (resp.MsgBytes[b] != "X" && resp.MsgBytes[b] != "*" && oMsg[b].ToString("X2") != resp.MsgBytes[b])
                        {
                            match = false;
                            break;
                        }
                    }
                    if (match)
                    {
                        Debug.WriteLine("Simulator Found response, line: " + r.ToString());
                        return resp.Answers;
                    }
                }
                Debug.WriteLine("Simulator find no response");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, simulator line " + line + ": " + ex.Message);
            }
            return new string[0];
        }
    }
}
