# Narod's Steam Game Finder
A C# library compatible back to Windows 7 (.NET 3.5 edition) & modern .NET platforms (.NET 8.0 edition), which finds Steam and Games.

## Summary
This library allows you to check if Steam is installed, the Steam install path, the Steam library paths, and locate game install directories.

## Which edition should I use?
You should really use the .NET 8.0 version (SteamGameFinder2) unless you require legacy support with pre-Windows 10 operating systems.

The .NET 8.0 version will also work much better on macOS & Linux platforms, as well as being more secure and performant on Windows 10 & 11 systems.

With that said, the two versions operate identically with the same feature set and very similar codebase. They are also both maintained at present.

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

## Limitations
If you use `getGameInfoByFolder`, you can only search for Steam games by their installed folder name. For example, the game 'Cities: Skylines' is installed to 'Cities_Skylines':
```c#
string citiesSkylinesInstallDir = steamGameLocator.getGameInfoByFolder("Cities_Skylines").steamGameLocation
```
You can use Steam Game IDs to make this easier to locate. Steam Game name searching may be added later, but it's much slower than the currently added operations.

## Further Help
You can find the 'TestApp' solution in this repository which has an example of how to use the library.

## Helping
If you find any bugs, please report it as an issue.

I would also appreciate any extra functionality, feel free to open a PR request.