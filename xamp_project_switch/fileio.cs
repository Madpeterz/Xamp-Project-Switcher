using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;


namespace xamp_project_switch
{
    public class file_read_write
    {
        string filename = "";
        string folder1 = "config";
        string folder2 = "";
        string folder3 = "";
        string map_chars = "=";
        string newline_chars = "[NL]";
        public file_read_write()
        {
            folder_exists("", folder1);
        }
        public List<FileInfo> matching_files(string partfilename = "", string filetype = ".log")
        {
            partfilename = partfilename.ToLower();
            List<FileInfo> myfiles = new DirectoryInfo(getpath("")).GetFiles("*" + filetype + "").OrderBy(f => f.CreationTime).ToList();
            List<FileInfo> cleaned = new List<FileInfo>();
            foreach (FileInfo F in myfiles)
            {
                if (F.Name.ToLower().Contains(partfilename))
                {
                    cleaned.Add(F);
                }
            }
            myfiles = cleaned;
            return myfiles;
        }
        public bool folder_exists(string in_folder, string folder_name)
        {
            if (in_folder != "")
            {
                in_folder = "" + in_folder + "/";
            }
            if (folder_name != "")
            {
                string path = "" + in_folder + "" + folder_name + "";
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                return true;
            }
            return false;
        }
        private string getpath(string tofile)
        {
            string path = "";
            string addon = "";
            if (folder1 != "")
            {
                path = "" + path + "" + folder1 + "";
                addon = "/";
                if (folder2 != "")
                {
                    path = "" + path + "" + addon + "" + folder2 + "";
                    if (folder3 != "")
                    {
                        path = "" + path + "" + addon + "" + folder3 + "";
                    }
                }
            }
            path = "" + path + "" + addon + "" + tofile + "";
            return path;
        }
        public void set_filename(string setfilename)
        {
            filename = setfilename.ToLower();
        }
        public void set_folder3(string foldername)
        {
            folder3 = foldername;
            if (folder2 != "")
            {
                folder_exists("" + folder1 + "/" + folder2 + "", folder3);
            }
        }
        public void set_folder2(string foldername)
        {
            folder2 = foldername;
            folder_exists(folder1, folder2);
        }
        public void set_folder1(string foldername)
        {
            folder1 = foldername;
            folder_exists(folder1, folder2);
        }
        public string[] read_file()
        {
            if (System.IO.File.Exists(getpath(filename)))
            {
                return System.IO.File.ReadAllLines(getpath(filename));
            }
            else
            {
                return new string[1] { "unable to read file" };
            }
        }
        public string read_file_formated()
        {
            if (System.IO.File.Exists(getpath(filename)))
            {
                string returntext = "";
                string addon = "";
                string[] results = System.IO.File.ReadAllLines(getpath(filename));
                foreach (string R in results)
                {
                    returntext = "" + returntext + "" + addon + "" + R + "";
                    addon = System.Environment.NewLine;
                }
                return returntext;
            }
            else
            {
                return null;
            }
        }
        public void add_to_file(string line, int trys = 0)
        {
            try
            {
                System.IO.StreamWriter w = System.IO.File.AppendText(getpath(filename));
                w.Write("" + line + "\r\n");
                w.Close();
            }
            catch
            {
                if (trys < 10)
                {
                    System.Threading.Thread.Sleep(1000);
                    add_to_file(line, trys++);
                }
            }
        }
        public void save_file(String[] lines)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(getpath(filename));
            foreach (String A in lines)
            {
                file.WriteLine(A);
            }
            file.Close();
        }
        public Dictionary<string, string> read_mapped_file()
        {
            Dictionary<string, string> returnmap = new Dictionary<string, string>();
            if (System.IO.File.Exists(getpath(filename)))
            {
                String[] maplines = System.IO.File.ReadAllLines(getpath(filename));
                foreach (String line in maplines)
                {
                    String[] parts = line.Split(new string[] { map_chars }, StringSplitOptions.None);
                    if (parts.Length == 2)
                    {
                        returnmap.Add(parts[0], parts[1].Replace(newline_chars, Environment.NewLine));
                    }
                }
            }
            return returnmap;
        }
        public bool save_mapped_file(Dictionary<string, string> input)
        {
            try
            {
                System.IO.StreamWriter file = new System.IO.StreamWriter(getpath(filename));
                foreach (KeyValuePair<string, string> pair in input)
                {
                    string line = "" + pair.Key.ToString() + "" + map_chars + "" + pair.Value.Replace(Environment.NewLine, newline_chars) + "";
                    file.WriteLine(line);
                }
                file.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}