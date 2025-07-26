using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Narod.SteamGameFinder;

namespace TestApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SteamGameLocator steamGameLocator = new SteamGameLocator();
            while (true)
            {
                Console.WriteLine("Test utility for Narod.SteamGameFinder");
                Console.WriteLine("Test suite will now commence...");
                System.Threading.Thread.Sleep(1000);
                Console.WriteLine("Locating Steam install location.");
                Console.WriteLine("Is Steam Installed: " + steamGameLocator.getIsSteamInstalled());
                System.Threading.Thread.Sleep(1000);
                Console.WriteLine("Fetching Steam Install path");
                Console.WriteLine("Steam install path: " + steamGameLocator.getSteamInstallLocation());
                System.Threading.Thread.Sleep(1000);
                Console.WriteLine("Fetching Steam library locations");
                List<string> paths = steamGameLocator.getSteamLibraryLocations();
                foreach (string path in paths)
                {
                    Console.WriteLine("Library: " + path);
                }
                System.Threading.Thread.Sleep(1000);
                Console.WriteLine("Checking if Fallout 3 GOTY is installed");
                Console.WriteLine("Fallout 3 GOTY install path: " + steamGameLocator.getGameInfoByFolder("Fallout 3 goty").steamGameLocation);
                Console.WriteLine("Checking if The Binding of Isaac: Rebirth is installed");
                Console.WriteLine("The Binding of Isaac: Rebirth install path: " + steamGameLocator.getGameInfoByID("250900").steamGameLocation);
                Console.WriteLine("Tests complete.");
                System.Threading.Thread.Sleep(10000);
            }
        }
    }
}
