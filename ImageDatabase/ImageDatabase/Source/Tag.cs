using System;
using static System.String;

namespace ImageDatabase.Source
{
    class Tag
    {
        public enum TagType { String, Int, Bool };
        public TagType Type;
        public string StringVal; //always contains value!
        public int IntVal;
        public bool BoolVal;
        public Tag(int intVal)
        {
            Type = TagType.Int;
            IntVal = intVal;
            StringVal = intVal.ToString();
        }

        public Tag(bool val)
        {
            Type = TagType.Bool;
            BoolVal = val;
            StringVal = val.ToString();
        }

        public Tag(string val)
        {
            Type = TagType.String;
            StringVal = val;
        }

        public static bool operator <(Tag t1, Tag t2)
        {
            if (t1.Type != t2.Type)
                return t1.Type < t2.Type;
            if (t1.Type == TagType.Bool)
                return t1.BoolVal.CompareTo(t2.BoolVal) < 0;
            if (t1.Type == TagType.Int)
                return t1.IntVal < t2.IntVal;
            return Compare(t1.StringVal, t2.StringVal, StringComparison.Ordinal) < 0;
        }

        public static bool operator >(Tag t1, Tag t2)
        {
            return t2 < t1;
        }
    }
}
