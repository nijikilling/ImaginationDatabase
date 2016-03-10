using System;
using System.Collections.Generic;
using System.IO;

namespace ImageDatabase.Source
{
    class StreamParser
    {
        public static string GetString(StreamReader stream)
        {
            if (stream.EndOfStream)
                return "";
            int ch = stream.Read();    
            List <char> builder = new List<char>();
            while (!stream.EndOfStream && ch != ':' && ch != '?' && ch != '#') {
                builder.Add(Convert.ToChar(ch));
                ch = stream.Read();
            }
            return new string(builder.ToArray());
        }

        public static int GetInt(StreamReader stream)
        {
            return Convert.ToInt32(GetString(stream));
        }

        public static bool GetBool(StreamReader stream)
        {
            return Convert.ToBoolean(GetString(stream));
        }
    }
}
