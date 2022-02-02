using System;
using System.IO;
using System.Collections;

namespace PlexRenamer
{
    class Renamer
    {
        public static void Main()
        {
            Console.WriteLine("Plex Renamer\n");
            Console.Write("Enter a directory: ");

            string path = Console.ReadLine();

            if (File.Exists(path))
            {
                // For a file
                ProcessFile(path);
            }
            else if (Directory.Exists(path))
            {
                // For a directory
                ProcessDirectory(path);
            }
            else
            {
                // Invalid
                Console.WriteLine("{0} is not a valid file or directory...", path);
            }

            Console.ReadLine();
        }

        // Process the directory, recurse into child directories or send files to process
        public static void ProcessDirectory(string targetDirectory)
        {
            Console.WriteLine("Found directory: {0}", targetDirectory);

            // Get the list of files and process them
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
            {
                ProcessFile(fileName);
            }

            // Recurse into subdirectories
            string[] subDirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subDirectory in subDirectoryEntries)
            {
                ProcessDirectory(subDirectory);
            }
        }

        // Process the file to determine if it needs to be renamed
        public static void ProcessFile(string filePath)
        {
            // Setup
            string[] pathArr = Path.GetDirectoryName(filePath).Split('\\');
            string seasonStr = pathArr[pathArr.Length - 1];
            string showStr = pathArr[pathArr.Length - 2];
            string[] seasonArr = seasonStr.Split(' ');

            if (showStr != "Plex" && showStr != "TV Shows")
            {
                // Output given data
                Console.WriteLine("\nFound File. Parsing...");
                Console.WriteLine("Show: {0}", showStr);
                Console.WriteLine("Season: {0}", seasonArr[1]);
                Console.WriteLine("Current Episode Title: {0}", Path.GetFileName(filePath));

                // Determine if the file already matches with what we want
                string[] episodeSplit = Path.GetFileName(filePath).Split(' ');
                string[] episodeNumSplitArr = episodeSplit[episodeSplit.Length - 1].Split('E');
                string[] seasonNumSplitArr = episodeNumSplitArr[0].Split('S');

                // Edge case protection
                if (seasonNumSplitArr.Length >= 2 && episodeNumSplitArr.Length >= 2)
                {
                    string testEpiTitle = showStr + " S" + seasonNumSplitArr[1] + "E" + episodeNumSplitArr[1];
                    string testEpiTitleHyphen = showStr + " - S" + seasonNumSplitArr[1] + "E" + episodeNumSplitArr[1];
                    bool invalidTitle = (Path.GetFileName(filePath) != testEpiTitle) && (Path.GetFileName(filePath) != testEpiTitleHyphen);

                    if (invalidTitle)
                    {
                        RenameFile(filePath, seasonStr, showStr);
                    }
                    else
                    {
                        Console.WriteLine("File name is already correct. Skipping...\n");
                    }
                }
                else
                {
                    RenameFile(filePath, seasonStr, showStr);
                }
            }
        }

        public static void RenameFile(string filePath, string seasonStr, string showStr)
        {
            // Double check
            Console.Write("Change this title? (y/n): ");
            string changeEpiFlagStr = Console.ReadLine();
            bool changeEpiFlag = changeEpiFlagStr == "y";

            if (changeEpiFlag)
            {
                Console.Write("What episode #? : ");

                // Setup the correct format for episode and season numbers
                string givenEpisodeNum = Console.ReadLine();
                string[] seasonArr = seasonStr.Split(' ');

                int seasonNumInt = Int32.Parse(seasonArr[1]);
                string seasonNumStr = "";
                if (seasonNumInt < 10)
                {
                    seasonNumStr = "0" + seasonNumInt;
                }
                else
                {
                    seasonNumStr = "" + seasonNumInt;
                }

                int episodeNumInt = Int32.Parse(givenEpisodeNum);
                string episodeNumStr = "";

                if (episodeNumInt < 10)
                {
                    episodeNumStr = "0" + episodeNumInt;
                }
                else
                {
                    episodeNumStr = "" + episodeNumInt;
                }

                // Final check
                string potentialEpiName = showStr + " - S" + seasonNumStr + "E" + episodeNumStr + Path.GetExtension(filePath);
                Console.WriteLine("Potential Episode Title: {0}", potentialEpiName);
                Console.Write("Is this correct? (y/n): ");
                string correctFlagStr = Console.ReadLine();
                bool correctFlag = correctFlagStr == "y";

                // Rename the file
                if (correctFlag)
                {
                    string episodePath = Path.GetDirectoryName(filePath) + "\\" + potentialEpiName;
                    System.IO.File.Move(filePath, episodePath);
                }
                else
                {
                    // Manual renaming
                    Console.WriteLine("Type in the expected file name, without the extension: ");
                    string correctedFileName = Console.ReadLine();
                    string episodePath = Path.GetDirectoryName(filePath) + "\\" + correctedFileName + Path.GetExtension(filePath);
                    System.IO.File.Move(filePath, episodePath);
                }
            }
            else
            {
                Console.WriteLine("Skipping...\n");
            }
        }
    }
}
