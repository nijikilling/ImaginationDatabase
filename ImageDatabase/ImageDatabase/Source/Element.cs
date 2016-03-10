using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace ImageDatabase.Source
{
    class Element
    {
        public string Identifier;
        public Dictionary<string, Tag> Tags;
        public Element() { Tags = new Dictionary<string, Tag>(); }

        public static Element Read(StreamReader stream)
        {
            Element res = new Element();
            res.Identifier = StreamParser.GetString(stream);
            int tagCount = StreamParser.GetInt(stream);
            for (int i = 0; i < tagCount; i++)
            {
                string tagName = StreamParser.GetString(stream);
                int tagType = StreamParser.GetInt(stream);
                switch (tagType)
                {
                    case 0: {
                        res.Tags.Add(tagName, new Tag(StreamParser.GetString(stream)));
                        break;
                    }
                    case 1:
                    {
                        res.Tags.Add(tagName, new Tag(StreamParser.GetInt(stream)));
                        break;
                    }
                    case 2:
                    {
                        res.Tags.Add(tagName, new Tag(StreamParser.GetBool(stream)));
                        break;
                    }
                }
            }
            return res;
        }

        public void SaveConfig(StreamWriter stream)
        {
            stream.Write(Identifier + '#');
            stream.Write(Tags.Count.ToString() + '#');
            foreach(KeyValuePair<string, Tag> p in Tags)
            {
                stream.Write(p.Key + ':');
                stream.Write(((int)(p.Value.Type)).ToString() + ':');
                stream.Write(p.Value.StringVal + '#');
            }
        }

        public string GetCacheFileName()
        {
            return Path.GetFileNameWithoutExtension(Identifier);
        }

        public void SavePictureData(string folder, Bitmap bmp)
        {
            string cacheFile = Path.Combine(folder, GetCacheFileName());
            StreamWriter stream = new StreamWriter(cacheFile);
            byte[] imageBytes = new byte[bmp.Width * bmp.Height * 4]; //ARGB. TODO - get rid of encoding dependency
            for (int i = 0; i < bmp.Height; i++)
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    int ind = (i*bmp.Width + j)*4;
                    imageBytes[i * bmp.Width + j] = bmp.GetPixel(j, i).A;
                    imageBytes[i * bmp.Width + j] = bmp.GetPixel(j, i).A;
                    imageBytes[i * bmp.Width + j] = bmp.GetPixel(j, i).A;
                    imageBytes[i * bmp.Width + j] = bmp.GetPixel(j, i).A;
                }
            }
        }

    }
}
