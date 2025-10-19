using Narod;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;

namespace Narod
{
    namespace SteamGameFinder
    {
        public class SteamGameLocator
        {
            private readonly SteamGameLocatorOptions _options;

            private static string steamRegPath = "";

            private bool? steamInstalled = null;
            private string steamInstallPath = null;
            private List<string> steamLibraryList = new List<string>();
            private List<GameStruct> steamGameList = new List<GameStruct>();

            private bool hasIndexed = false;

            public SteamGameLocator(SteamGameLocatorOptions options = null)
            {
                // check if system is 32-bit or 64-bit, and set the registry path accordingly, Environ,ent.Is64BitOperatingSystem is unavailable in .NET 3.5
                if (IntPtr.Size == 8 || Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE") == "AMD64")
                {
                    steamRegPath = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\Valve\\Steam"; // 64-bit registry path
                }
                else
                {
                    steamRegPath = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Valve\\Steam"; // 32-bit registry path
                }

                _options = options ?? new SteamGameLocatorOptions();
            }

            /// <summary>
            /// A struct holding properties on games
            /// </summary>
            public struct GameStruct
            {
                public string steamGameID;
                public string steamGameName;
                public string steamGameLocation;
            }

            /// <summary>
            /// Checks if Steam is installed on the system by querying the Windows registry, and returns a boolean value.
            /// </summary>
            /// <returns>
            /// A <see cref="bool"/> indicating whether Steam is installed on the system.
            /// </returns>
            /// <remarks>This method only checks once, and then returns a cached result on subsequent runs.</remarks>
            /// <exception cref="SecurityException">Thrown if unsufficient permissions to check Steam install.</exception>
            public bool getIsSteamInstalled() // function to return a boolean of whether steam is installed or not
            {
                if (_options.MemorySettings == MemorySettings.None) { steamInstalled = null; } // if user wishes to not index this, wipe the previous result at the start so we have a clean slate
                if (steamInstalled != null) { return (bool)steamInstalled; } // if this information is already stored, let's use that instead
                try // try statement, this could fail due to registry errors, or if the user does not have admin perms
                {
                    string steamInstallPath = RegistryHandler.safeGetRegistryKey("InstallPath", steamRegPath); // uses a safe way of getting the registry key
                    if (steamInstallPath == null) { steamInstalled = false; return (bool)steamInstalled; } // if the safe registry returner is null, then steam is not installed
                    if (Directory.Exists(steamInstallPath) == false) { steamInstalled = false; return (bool)steamInstalled; } // if the folder location in the registry key is not on the system, then steam is not installed
                }
                catch (ArgumentNullException) { steamInstalled = false; return (bool)steamInstalled; } // unlikely to occur, but could be raised by safe registry returner, will return false as it would mean failed to find reg key
                catch (Exception) { throw; } // any other general exception - this should never occur but good practice to throw other exceptions back to program
                steamInstalled = true;
                return (bool)steamInstalled; // if other 'guard if statements' are passed, then steam is accepted to be installed
            }

            /// <summary>
            /// Checks the registry for the Steam install location and returns it.
            /// </summary>
            /// <returns>
            /// A <see cref="string"/> of the full file path of where Steam is installed.
            /// </returns>
            /// <remarks>This method only indexes the Steam install location once, and then returns a cached copy on subsequent runs.</remarks>
            /// <exception cref="DirectoryNotFoundException">Thrown if Steam is not installed.</exception>
            /// <exception cref="SecurityException">Thrown if unsufficient permissions to check Steam install path.</exception>
            public string getSteamInstallLocation()
            {
                if (_options.MemorySettings == MemorySettings.None) { steamInstalled = null; steamInstallPath = null; } // if user wishes to not index this, wipe the previous result at the start so we have a clean slate
                if (steamInstalled == false) { if (_options.SuppressExceptions) { return null; } else { throw new DirectoryNotFoundException(); } } // if already checked if steam is installed and it isn't
                if (steamInstallPath != null && Directory.Exists(steamInstallPath)) { return steamInstallPath; } // if this information is already stored, let's use that instead
                try // try statement, this could fail due to registry errors, or if the user does not have admin perms
                {
                    steamInstallPath = RegistryHandler.safeGetRegistryKey("InstallPath", steamRegPath); // uses a safe way of getting the registry key
                    if (steamInstallPath == null) { if (_options.SuppressExceptions) { return null; } else { throw new DirectoryNotFoundException(); } } // if the safe registry returner is null, then steam is not installed. throw directory not found exception
                    if (Directory.Exists(steamInstallPath) == false) { if (_options.SuppressExceptions) { return null; } else { throw new DirectoryNotFoundException(); } } // if the folder location in the registry key is not on the system, then steam is not installed. throw directory not found exception
                }
                catch (ArgumentNullException) { if (_options.SuppressExceptions) { return null; } else { throw new DirectoryNotFoundException("Steam is not installed."); } }
                catch (Exception) { throw; } // any other general exception - this should never occur but good practice to throw other exceptions back to program
                return steamInstallPath; // if other 'guard if statements' are passed, then steam is accepted to be installed
            }

            /// <summary>
            /// Indexes all Steam library locations, and returns a list of their full file paths.
            /// </summary>
            /// <returns>
            /// A <see cref="List{T}"/> of <see cref="string"/> containing the full file paths of all Steam library locations.
            /// </returns>
            /// <remarks>This method only indexes the library locations once, and then returns a cached copy on subsequent runs.</remarks>
            public List<String> getSteamLibraryLocations()
            {
                if (_options.MemorySettings == MemorySettings.None) { steamInstalled = null; steamInstallPath = null; steamLibraryList.Clear(); } // if user wishes to not index this, wipe the previous result at the start so we have a clean slate
                if (steamLibraryList.Count != 0) { return steamLibraryList; } // if this information is already stored, let's use that instead

                if (steamInstallPath == null) { getSteamInstallLocation(); } // if the steam install path has not already been fetched, fetch it
                if (steamInstallPath == null) { return new List<string>(); } // can only occur if suppress exceptions is turned on, we need to check if it's still false and just provide an empty list back

                StreamReader libraryVDFReader = File.OpenText(steamInstallPath + "\\steamapps\\libraryfolders.vdf");
                string lineReader = libraryVDFReader.ReadLine();
                bool continueRead = true;
                while (continueRead)
                {
                    while (lineReader.Contains("path") == false)
                    {
                        try
                        {
                            lineReader = libraryVDFReader.ReadLine(); // waiting to read in a line that looks like: "path"      "C:\location\to\library\folder"
                            if (lineReader == null) { break; }
                        }
                        catch (Exception) // End of file exception
                        {
                            continueRead = false; // stop reading
                            break; // break this loop
                        }
                    }
                    if (lineReader == null) { break; }
                    string cleanLine = lineReader.Replace("\"path\"", ""); // we then clean this up by removing the path part, leaving us with:         "C:\location\to\library\folder"
                    cleanLine = cleanLine.Split('"')[1]; // we then remove the leading spaces and quotes to get: C:\location\to\library\folder"
                    cleanLine = cleanLine.Replace("\"", ""); // we then remove the last quote to get: C:\location\to\library\folder

                    lineReader = libraryVDFReader.ReadLine(); // prevents it from getting stuck on the same library folder

                    if (Directory.Exists(cleanLine)) { steamLibraryList.Add(cleanLine); } // if the directory exists on the disk, then add it to the library list
                }
                return steamLibraryList;
            }

            /// <summary>
            /// Indexes Steam games by scanning Steam library folders and extracting game information.
            /// </summary>
            /// <remarks>This method clears the current list of indexed games and scans all configured
            /// Steam library folders to identify installed games. It reads the appmanifest files in each library folder
            /// to retrieve the game's name, ID, and installation directory. The indexed game information is stored in a
            /// list.</remarks>
            public void indexSteamGames()
            {
                if (!_options.ShouldIndexLibrary) { return; } // prevent this from being run if we aren't allowing indexing
                steamGameList.Clear();
                getSteamLibraryLocations(); // ensure we have the library locations before indexing games
                if (steamLibraryList.Count == 0) { if (_options.SuppressExceptions) { return; } else { throw new InvalidOperationException("Cannot index library with no libraries present."); } }

                foreach (string libraryFolder in steamLibraryList)
                {
                    List<string> gameFiles = Directory.GetFiles(libraryFolder + "\\steamapps", "appmanifest_*.acf").ToList(); // get all the appmanifest files in the steamapps folder
                    foreach (string gameFile in gameFiles)
                    {
                        GameStruct gameInfo = new GameStruct();
                        gameInfo.steamGameID = Path.GetFileNameWithoutExtension(gameFile).Replace("appmanifest_", ""); // get the ID from the file name
                        StreamReader gameManifestReader = File.OpenText(gameFile);
                        string lineReader = gameManifestReader.ReadLine();
                        while (lineReader != null)
                        {
                            if (lineReader.Contains("name")) // looks like: "name"		"Game Name"
                            {
                                string cleanLine = lineReader.Replace("\"name\"", ""); // we then clean this up by removing the name part, leaving us with:         "Game Name"
                                cleanLine = cleanLine.Split('"')[1]; // we then remove the leading spaces and quotes to get: Game Name
                                gameInfo.steamGameName = cleanLine; // set the name
                            }
                            if (lineReader.Contains("installdir")) // looks like: "installdir"		"Game Folder Name"
                            {
                                string cleanLine = lineReader.Replace("\"installdir\"", ""); // we then clean this up by removing the installdir part, leaving us with:         "Game Folder Name"
                                cleanLine = cleanLine.Split('"')[1]; // we then remove the leading spaces and quotes to get: Game Folder Name
                                gameInfo.steamGameLocation = libraryFolder + "\\steamapps\\common\\" + cleanLine; // set the location
                            }
                            if (gameInfo.steamGameName != null && gameInfo.steamGameLocation != null) { break; } // if we have both the name and location, we can stop reading the file
                            lineReader = gameManifestReader.ReadLine(); // read next line
                        }
                        steamGameList.Add(gameInfo); // add the game to our list
                    }
                }
                if (_options.MemorySettings == MemorySettings.Full) { hasIndexed = true; } // if we are allowing indexing, we'll set this to true, otherwise we won't set it, so other functions will think no indexing has happened and run it again
            }

            /// <summary>
            /// Returns the install path, name & ID of a game, by its Steam install folder.
            /// </summary>
            /// <remarks>If the game list has not been indexed yet, this method with automatically
            /// index the games before searching for this game. Subsequent calls will check the already indexed
            /// list.</remarks>
            /// <param name="gameName">The name of the folder Steam installs the game to.</param>
            /// <returns>A single <see cref="GameStruct"/> object of the game.</returns>
            /// <exception cref="DirectoryNotFoundException">Thrown if the game is not installed.</exception>
            public GameStruct getGameInfoByFolder(string gameName)
            {
                if (_options.ShouldIndexLibrary) { return getGameInfoByFolder_index(gameName); } else { return getGameInfoByFolder_noIndex(gameName); }
            }

            private GameStruct getGameInfoByFolder_noIndex(string gameName)
            {
                getSteamLibraryLocations(); // ensure we have the library locations before looking for games
                if (steamLibraryList.Count == 0) { if (_options.SuppressExceptions) { return new GameStruct(); } else { throw new InvalidOperationException("Cannot search for games in no libraries."); } }

                GameStruct gameInfo = new GameStruct();
                gameInfo.steamGameName = gameName;
                gameInfo.steamGameID = "0"; // not used

                foreach (string libraryFolder in steamLibraryList)
                {
                    string checkFolder = libraryFolder + "\\steamapps\\common\\" + gameName;

                    if (Directory.Exists(checkFolder)) { gameInfo.steamGameLocation = checkFolder; break; }
                }

                if (gameInfo.steamGameLocation != null) { return gameInfo; }

                if (_options.SuppressExceptions) { return new GameStruct(); } else { throw new DirectoryNotFoundException("Game not found in Steam library. Please ensure the game is installed and try again."); } // if we reach here, then the game was not found, so throw an exception
            }

            private GameStruct getGameInfoByFolder_index(string gameName)
            {
                if (!hasIndexed || _options.MemorySettings != MemorySettings.Full) { indexSteamGames(); } // if the game list is empty, index the games first

                foreach (GameStruct steamGame in steamGameList)
                {
                    if (steamGame.steamGameLocation.EndsWith(gameName)) { return steamGame; } // if game is already stored in our list, just return that instead
                }

                if (_options.SuppressExceptions) { return new GameStruct(); } else { throw new DirectoryNotFoundException("Game not found in Steam library. Please ensure the game is installed and try again."); } // if we reach here, then the game was not found in our list, so throw an exception
            }

            /// <summary>
            /// Returns the install path, name & ID of a game, by its Steam ID.
            /// </summary>
            /// <remarks>If the game list has not been indexed yet, this method with automatically
            /// index the games before searching for this game. Subsequent calls will check the already indexed
            /// list.</remarks>
            /// <param name="gameID">The Steam ID of the game you want to look for.</param>
            /// <returns>A single <see cref="GameStruct"/> object of the game.</returns>
            /// <exception cref="FileNotFoundException">Thrown if the game is not installed.</exception>
            public GameStruct getGameInfoByID(string gameID)
            {
                if (_options.ShouldIndexLibrary) { return getGameInfoByID_index(gameID); } else { return getGameInfoByID_noindex(gameID); }
            }

            private GameStruct getGameInfoByID_noindex(string gameID)
            {
                string gameInstallDir = null;
                string gameName = null;
                try
                {
                    gameInstallDir = RegistryHandler.safeGetRegistryKey("InstallLocation", $"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Steam App {gameID}");
                    gameName = RegistryHandler.safeGetRegistryKey("DisplayName", $"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Steam App {gameID}");
                }
                catch (ArgumentNullException) { if (_options.SuppressExceptions) { return new GameStruct(); } else { throw new DirectoryNotFoundException("Game not found in Steam Library. Please ensure the game is installed and try again."); } }
                if (gameInstallDir == null) { if (_options.SuppressExceptions) { return new GameStruct(); } else { throw new DirectoryNotFoundException("Game not found in Steam Library. Please ensure the game is installed and try again. Sometimes Steam does not correctly update the registry, use a different search or enable indexing to resolve these cases."); } }

                return new GameStruct() { steamGameID = gameID, steamGameLocation = gameInstallDir, steamGameName = gameName ?? "" };
            }

            private GameStruct getGameInfoByID_index(string gameID)
            {
                if (!hasIndexed || _options.MemorySettings != MemorySettings.Full) { indexSteamGames(); } // if the game list is empty, index the games first

                foreach (GameStruct steamGame in steamGameList)
                {
                    if (steamGame.steamGameID == gameID) { return steamGame; } // if game is already stored in our list, just return that instead
                }

                if (_options.SuppressExceptions) { return new GameStruct(); } else { throw new DirectoryNotFoundException("Game not found in Steam library. Please ensure the game is installed and try again."); } // if we reach here, then the game was not found in our list, so throw an exception
            }

            /// <summary>
            /// Returns the install path, name & ID of a game, by its Steam name.
            /// </summary>
            /// <remarks>If the game list has not been indexed yet, this method with automatically
            /// index the games before searching for this game. Subsequent calls will check the already indexed
            /// list.</remarks>
            /// <param name="gameName">The Steam game name of what you want to look for.</param>
            /// <returns>A single <see cref="GameStruct"/> object of the game.</returns>
            /// <exception cref="DirectoryNotFoundException">Thrown if game is not installed.</exception>
            public GameStruct getGameInfoByName(string gameName)
            {
                if (!_options.ShouldIndexLibrary) { throw new InvalidOperationException("Unable to locate by game name when indexing is disabled. Either enable indexing, or search by folder name / Steam App ID."); } // this isn't possible if you have disabled indexing, so an exception will be thrown
                if (!hasIndexed || _options.MemorySettings != MemorySettings.Full) { indexSteamGames(); } // if the game list is empty, index the games first
                foreach (GameStruct steamGame in steamGameList)
                {
                    if (steamGame.steamGameName == gameName) { return steamGame; } // if game is already stored in our list, just return that instead
                }
                if (_options.SuppressExceptions) { return new GameStruct(); } else { throw new DirectoryNotFoundException("Game not found in Steam library. Please ensure the game is installed and try again."); } // if we reach here, then the game was not found in our list, so throw an exception
            }

            /// <summary>
            /// Retrieves a list of all indexed Steam games.
            /// </summary>
            /// <remarks>If the game list has not been indexed yet, this method will automatically
            /// index the games before returning the list. Subsequent calls will return the already indexed
            /// list.</remarks>
            /// <returns>A <see cref="List{T}"/> of <see cref="GameStruct"/> objects representing the indexed games. If no games are available,
            /// the <see cref="List{T}"/> will be empty.</returns>
            public List<GameStruct> getAllGames()
            {
                if (!_options.ShouldIndexLibrary) { throw new InvalidOperationException("Unable to return all games when indexing is disabled. Enable indexing to use this function."); } // this isn't possible if you have disabled indexing, so an exception will be thrown
                if (!hasIndexed || _options.MemorySettings != MemorySettings.Full) { indexSteamGames(); } // if the game list is empty, index the games first
                return steamGameList; // return the list of games
            }
        }
    }
}