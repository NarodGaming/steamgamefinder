# Narod's Steam Game Finder
A C# library compatible back to Windows 7 (.NET 3.5 edition) & modern .NET platforms (.NET 8.0 edition), which finds Steam and Games.

## Summary
This library allows you to check if Steam is installed, the Steam install path, the Steam library paths, and locate game install directories.

## Which edition should I use?
If you use the Nuget package manager, the versions are handled automatically for you - no further action required. The two versions behave identically regardless.

For .NET 8.0+ users, use SteamGameFinder2.dll

For .NET 3.5+ users, use SteamGameFinder.dll

## Usage
Code examples in C#
- Download the latest release from the releases tab, or find on Nuget ([Narod.SteamGameFinder](https://www.nuget.org/packages/Narod.SteamGameFinder/))
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
- Get Steam Game Install Path by ID
```c#
string steamGameIDInstallDir = steamGameLocator.getGameInfoByID("0000").steamGameLocation
```
- Get Steam Game Install Path by Name
```c#
string steamGameNameInstallDir = steamGameLocator.getGameInfoByName("Tropico 6").steamGameLocation
```
- Return all installed games
```c#
List<string> allSteamInstalledGames = steamGameLocator.getAllGames()
```

## Further Help
You can find the 'TestApp' solution in this repository which has an example of how to use the library.

## Helping
If you find any bugs, please report it as an issue.

I would also appreciate any extra functionality, feel free to open a PR request.
