using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace ImageDatabase.Source
{
    class Element
    {
        public string Identifier;
        public Dictionary<string, Tag> Tags;
        public Bitmap picture;
        public Element() { Tags = new Dictionary<string, Tag>(); }

        public static Element Read(StreamReader stream)
        {
            Element res = new Element {Identifier = StreamParser.GetString(stream)};
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

        private string GetCacheFileName()
        {
            return Path.GetFileNameWithoutExtension(Identifier);
        }

        public void SavePictureData(string cacheFolder, Bitmap bmp)
        {
            string cacheFile = Path.Combine(cacheFolder, GetCacheFileName());
            Directory.CreateDirectory(cacheFolder);
            File.Create(cacheFile).Close();
            StreamWriter stream = new StreamWriter(cacheFile);
            byte[] imageBytes = new byte[bmp.Width * bmp.Height * 4]; //ARGB. TODO - get rid of encoding dependency
            Tags[Reserved.Width] = new Tag(bmp.Width);
            Tags[Reserved.Height] = new Tag(bmp.Height);
            for (int i = 0; i < bmp.Height; i++)
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    int ind = (i * bmp.Width + j) * 4;
                    imageBytes[ind + 0] = bmp.GetPixel(j, i).A;
                    imageBytes[ind + 1] = bmp.GetPixel(j, i).R;
                    imageBytes[ind + 2] = bmp.GetPixel(j, i).G;
                    imageBytes[ind + 3] = bmp.GetPixel(j, i).B;
                }
            }
            for (int i = 0; i < bmp.Width * bmp.Height * 4; i++)
                stream.Write(Convert.ToChar(imageBytes[i]));
            stream.Close();
        }

        public void GetPictureData(string cacheFolder)
        {
            string cacheFile = Path.Combine(cacheFolder, GetCacheFileName());
            StreamReader stream = new StreamReader(cacheFile);
            string imageBytes = stream.ReadToEnd(); //TODO - read/write async cos of freezes
            int width = GetWidth(), height = GetHeight();
            Bitmap bmp = new Bitmap(width, height);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int ind = (i * width + j) * 4;
                    bmp.SetPixel(j, i, Color.FromArgb(imageBytes[ind], imageBytes[ind + 1], imageBytes[ind + 2], imageBytes[ind + 3]));
                }
            }
            picture = bmp;
        }

        private int GetHeight()
        {
            return Tags[Reserved.Height].IntVal;
        }

        private int GetWidth()
        {
            return Tags[Reserved.Width].IntVal;
        }

        public void UncachePicture()
        {
            //Picture gets unlocked from memory. 
            picture = new Bitmap(0, 0);
        }
    }

    internal struct Reserved
    {
        /// <summary>
        /// Contains reserved keys - image name, width/height, 
        /// </summary>
        public const string Width = ".WIDTH";
        public const string Height = ".HEIGHT";
    }
}
