using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace ImageDatabase.Source
{
    class Database
    {
        public string Filepath, Name, CacheFolder;
        public int PartCount;
        public List<Element> Data;
        
        public Database(string fullpath)
        {
            fullpath = Path.GetFullPath(fullpath); // In a way
            Filepath = fullpath;
            // ReSharper disable once AssignNullToNotNullAttribute
            CacheFolder = Path.Combine(Path.GetDirectoryName(fullpath), '.' + Path.GetFileNameWithoutExtension(fullpath));
            Data = new List<Element>();

            Directory.CreateDirectory(CacheFolder);
        }

        public Database()
        {
            Data = new List<Element>();
        }

        public static Database Load(string path)
        {
            StreamReader stream = new StreamReader(path);
            Database res = new Database();
            res.Filepath = path;
            res.CacheFolder = Path.Combine(Path.GetDirectoryName(path), '.' + Path.GetFileNameWithoutExtension(path));
            res.Name = StreamParser.GetString(stream);
            res.PartCount = StreamParser.GetInt(stream);
            int size = StreamParser.GetInt(stream);
            for (int i = 0; i < size; i++)
            {
                res.Data.Add(Element.Read(stream));
            }
            stream.Close();
            return res;
        }
        public void Save()
        {
            StreamWriter stream = new StreamWriter(Filepath);
            stream.Write(Name + '#');
            stream.Write(PartCount.ToString() + '#');
            stream.Write(Data.Count.ToString() + '#');
            for (int i = 0; i < Data.Count; i++)
            {
                Data[i].SaveConfig(stream);
            }
            stream.Close();
        }
        public void AddPicture(string path)
        {
            Bitmap image = new Bitmap(path);
            Element elem = new Element {Identifier = Path.GetFileName(path)};
            elem.Tags.Add("NAME", new Tag(elem.Identifier));
            Data.Add(elem);
            elem.SavePictureData(CacheFolder, image);
        }
    }
}
