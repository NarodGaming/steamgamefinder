# Narod's Steam Game Finder
A C# library compatible back to Windows 7, which finds Steam and Games.

## Summary
This library allows you to check if Steam is installed, the Steam install path, the Steam library paths, and locate game install directories.

## Usage
Code examples in C#
- Download the latest release from the releases tab.
- (Only required if downloaded from releases) Add a reference of the library in your project. (Project -> Add Reference... -> Browse -> Browse...)
- Import in to your program
```c#
using Narod.SteamGameFinder;
```
- Create instance of SteamGameLocator
```c#
SteamGameLocator steamGameLocator = new SteamGameLocator();
```
- Check if Steam is installed
```c#
bool isSteamInstalled = steamGameLocator.getIsSteamInstalled();
```
- Get Steam Install path
```c#
string steamInstallDir = steamGameLocator.getSteamInstallLocation();
```
- Get Steam Library folders
```c#
List<string> paths = steamGameLocator.getSteamLibraryLocations();
```
- Get Steam Game Install Path
```c#
string steamGameInstallDir = steamGameLocator.getGameInfoByFolder("SteamGameFolderName").steamGameLocation
```

## Limitations
You can only search for Steam games by their installed folder name. For example, the game 'Cities: Skylines' is installed to 'Cities_Skylines', for example:
```c#
string citiesSkylinesInstallDir = steamGameLocator.getGameInfoByFolder("Cities_Skylines").steamGameLocation
```
I will look to improve this to allow human readable game names, or Steam app ID's in the future.

## Further Help
I'll release a video at some point detailing how to use this. You can find the 'TestApp' solution in this repository which has an example of how to use the library.

## Helping
If you find any bugs, please report it as an issue.

To-do:
- Implement Steam App ID as a method of finding a game
- Implement human readable game name as a method of finding a game
- Fix issues/bugs
