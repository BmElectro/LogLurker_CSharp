using System;
using System.IO;
using static System.Net.Mime.MediaTypeNames;

namespace LogLurker
{
    class Program
    {
        public class Config
        {
            public string FirstParameter { get; set; }
            
            public string SearchPath { get; set; }
            public string FilesExtension { get; set; }
            public bool IgnoreLogins { get; set; }
            public bool IgnoreVD { get; set; }
            public string[] FilesToSearch { get; set; }



        }
        static void ParseLine(string line, Config config, StreamWriter sw)
        {
            if (line.ToLower().Contains(config.FirstParameter.ToLower()))
            {
                if (line.Contains(": login (") && config.IgnoreLogins == false)
                {
                    sw.WriteLine(line);
                }
                if (line.Contains("AntiScalperVD") && config.IgnoreVD == false)
                {
                    sw.WriteLine(line);
                }
                if(config.IgnoreLogins == true && config.IgnoreVD == true)
                {
                    if(line.Contains("AntiScalperVD") || line.Contains(": login (")){}
                    else sw.WriteLine(line);
                }





            }
        }
        static void ParseFile(string filePath, Config config)
        {
            var openedFile = File.ReadLines(filePath);
            string pathToSaveOutput = $"All Logs of ({config.FirstParameter}).txt";

            using (StreamWriter sw = File.AppendText(pathToSaveOutput))
            {
                foreach (string line in openedFile)
                {
                    ParseLine(line, config, sw);
                }
            }
        }

        static void ReadConfig(Config config, IEnumerable<string> configFile)
        {
            foreach (string line in configFile)
            {
                if (!line.Contains('#'))
                {
                    if (line.Contains("First_Parameter"))
                    {
                        string[] temp = line.Split(' ', 3);
                        config.FirstParameter = ModifiyForConfig(temp);

                    }
                    if (line.Contains("Search_Path"))
                    {
                        string[] temp = line.Split(' ', 3);
                        string temp2 = ModifiyForConfig(temp);
                        if (temp2.EndsWith("\\"))
                        {
                            config.SearchPath = temp2;
                        }
                        else if(!temp2.EndsWith("\\") && temp2.Length != 0)
                        {
                            config.SearchPath = temp2 + "\\";
                        }
                        

                    }
                    if (line.Contains("Files_Extension"))
                    {
                        string[] temp = line.Split(' ', 3);
                        config.FilesExtension = ModifiyForConfig(temp);

                    }
                    if (line.Contains("Ignore_Login"))
                    {
                        string[] temp = line.Split(' ', 3);
                        string tempString = ModifiyForConfig(temp);
                        if(tempString.ToLower() == "true")
                        {
                            config.IgnoreLogins = true;
                        }else config.IgnoreLogins = false;

                    }
                    if (line.Contains("Ignore_VD"))
                    {
                        string[] temp = line.Split(' ', 3);
                        string tempString = ModifiyForConfig(temp);
                        if (tempString.ToLower() == "true")
                        {
                            config.IgnoreVD = true;
                        }
                        else config.IgnoreVD = false;

                    }
                    if (line.Contains("Search_Files"))
                    {
                        string[] temp = line.Split(' ', 3);
                        string[] tempString = ModifiyForConfig(temp).Split(',');
                        if (tempString[0].Length > 0)
                        {

                            string[] finalArray = new string[tempString.Length];
                            for (int i = 0; i < tempString.Length; i++)
                            {
                                Console.WriteLine(tempString[i]);
                                finalArray[i] = config.SearchPath + tempString[i].Trim(' ', '"');
                            }
                            config.FilesToSearch = finalArray;
                        }
                        else {}


                    }
                }
            }
        }

        static string ModifiyForConfig(string[] temp)
        {
            string tempString = temp[2].Trim(' ', '"');

            return tempString;
        }
        static void LoopAndParse(string[] arr, Config config)
        {
            foreach (string filePath in arr)
            {
                if (File.Exists(filePath))
                {
                    Console.WriteLine("Lurking in file " + filePath);
                    ParseFile(filePath, config);
                }
                else
                {
                    Console.WriteLine("File " + '"'  + filePath + '"' + " does not exist");
                }


            }
        }

        static void Main()
        {
            string configPath = @"config.cfg";
            var configFile = File.ReadLines(configPath);
            var config = new Config();
            ReadConfig(config, configFile);


            string extension = config.FilesExtension.ToLower();

            try
            {
                Console.WriteLine(config.FilesToSearch != null);
                if (config.FilesToSearch != null)
                {

                    LoopAndParse(config.FilesToSearch, config); 

                }
                else if(config.FilesToSearch == null && config.SearchPath == "")
                {
                    Console.WriteLine("Second If");
                    string[] filePaths = Directory.GetFiles(System.AppDomain.CurrentDomain.BaseDirectory, @$"*.{extension}", SearchOption.TopDirectoryOnly);
                    Console.WriteLine("Found " + filePaths.Length + " Files with the extension " + extension);

                    
                }
                else if(config.FilesToSearch == null && config.SearchPath != "")
                {
                    Console.WriteLine("third If");
                    string[] filePaths = Directory.GetFiles(@$"{config.SearchPath}", @$"*.{extension}", SearchOption.TopDirectoryOnly);
                    Console.WriteLine("Found " + filePaths.Length + " Files with the extension " + extension);

                    LoopAndParse(filePaths, config);
                }


            }
            catch
            {
                Console.WriteLine("No Files found");
            }












        }
    }
}
