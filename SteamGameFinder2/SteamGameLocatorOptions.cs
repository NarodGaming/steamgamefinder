using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Narod
{
    public enum LogLevel
    {
        None = 0, // default, don't print anything to the console
        Critical = 1, // print Critical errors to the console
        Error = 2, // print Error/Critical errors to the console
        Warning = 3, // print Warning/Error/Critical errors to the console
        Information = 4 // print Information/Warning/Error/Critical errors to the console
    };

    public enum IndexSettings
    {
        Full, // default, run each function and indexer once automatically, and then only on forced user calls do we re-index
        Partial, // partially index, remember library paths and Steam path, but always re-index for games
        None // no indexing, never remember Steam paths or games and check again everytime
    };

    public class SteamGameLocatorOptions
    {
        public LogLevel LogLevel { get; set; } = LogLevel.None;
        public IndexSettings IndexSettings { get; set; } = IndexSettings.Full;
        public bool SuppressExceptions { get; set; } = false;
    }
}
