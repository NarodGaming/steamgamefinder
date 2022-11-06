﻿using Narod;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;

namespace SteamGameFinder
{
    public class SteamGameLocator
    {
        private static readonly string steamRegPath = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\Valve\\Steam"; // not compatible with 32-bit

        private bool? steamInstalled = null;
        private string steamInstallPath = null;
        private List<string> steamLibraryList = new List<string>();

        public bool getIsSteamInstalled() // function to return a boolean of whether steam is installed or not
        {
            if (steamInstalled != null) { return (bool)steamInstalled; } // if this information is already stored, let's use that instead
            try // try statement, this could fail due to registry errors, or if the user does not have admin perms
            {
                string steamInstallPath = RegistryHandler.safeGetRegistryKey("InstallPath",steamRegPath); // uses a safe way of getting the registry key
                if(steamInstallPath == null) { steamInstalled = false; return (bool)steamInstalled; } // if the safe registry returner is null, then steam is not installed
                if(Directory.Exists(steamInstallPath) == false) { steamInstalled = false; return (bool)steamInstalled; } // if the folder location in the registry key is not on the system, then steam is not installed
            } catch (ArgumentNullException) { steamInstalled = false; return (bool)steamInstalled; } // unlikely to occur, but could be raised by safe registry returner, will return false as it would mean failed to find reg key
            catch (SecurityException sx) { throw sx; } // security exception, means user needs more perms. will throw this exception back to the program to resolve
            catch (Exception ex) { throw ex; } // any other general exception - this should never occur but good practice to throw other exceptions back to program
            return (bool)steamInstalled; // if other 'guard if statements' are passed, then steam is accepted to be installed
        }

        public string getSteamInstallLocation()
        {
            if(steamInstallPath != null && Directory.Exists(steamInstallPath)) { return steamInstallPath; } // if this information is already stored, let's use that instead
            try // try statement, this could fail due to registry errors, or if the user does not have admin perms
            {
                steamInstallPath = RegistryHandler.safeGetRegistryKey("InstallPath", steamRegPath); // uses a safe way of getting the registry key
                if (steamInstallPath == null) { throw new DirectoryNotFoundException(); } // if the safe registry returner is null, then steam is not installed. throw directory not found exception
                if (Directory.Exists(steamInstallPath) == false) { throw new DirectoryNotFoundException(); } // if the folder location in the registry key is not on the system, then steam is not installed. throw directory not found exception
            }
            catch (ArgumentNullException) { throw new DirectoryNotFoundException(); } // unlikely to occur, but could be raised by safe registry returner, will return false as it would mean failed to find reg key
            catch (SecurityException sx) { throw sx; } // security exception, means user needs more perms. will throw this exception back to the program to resolve
            catch (Exception ex) { throw ex; } // any other general exception - this should never occur but good practice to throw other exceptions back to program
            return steamInstallPath; // if other 'guard if statements' are passed, then steam is accepted to be installed
        }

        public List<String> getSteamLibraryLocations()
        {
            if (steamLibraryList.Count != 0) { return steamLibraryList; } // if this information is already stored, let's use that instead

            StreamReader libraryVDFReader = File.OpenText(steamInstallPath + "\\steamapps\\libraryfolders.vdf");
            string lineReader = libraryVDFReader.ReadLine();
            bool continueRead = true;
            while (continueRead)
            {
                while (lineReader.Contains("path")== false)
                {
                    try
                    {
                        lineReader = libraryVDFReader.ReadLine(); // waiting to read in a line that looks like: "path"      "C:\location\to\library\folder"
                    }
                    catch (Exception) // End of file exception
                    {
                        continueRead = false; // stop reading
                        break; // break this loop
                    }
                }
                string cleanLine = lineReader.Replace("\"path\"", ""); // we then clean this up by removing the path part, leaving us with:         "C:\location\to\library\folder"
                cleanLine = cleanLine.Split('"')[1]; // we then remove the leading spaces and quotes to get: C:\location\to\library\folder"
                cleanLine = cleanLine.Replace("\"", ""); // we then remove the last quote to get: C:\location\to\library\folder

                if (Directory.Exists(cleanLine)) { steamLibraryList.Add(cleanLine); } // if the directory exists on the disk, then add it to the library list
            }
            return steamLibraryList;
        }
    }
}