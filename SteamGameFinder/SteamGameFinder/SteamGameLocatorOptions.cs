using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Narod
{
    public enum LogLevel
    {
        None, // default
        Critical,
        Error,
        Warning,
        Information
    };

    public enum IndexSettings
    {
        Full, // default
        Partial,
        None
    };

    public class SteamGameLocatorOptions
    {
        public LogLevel LogLevel { get; set; } = LogLevel.None;
        public IndexSettings IndexSettings { get; set; } = IndexSettings.Full;
        public bool SuppressExceptions { get; set; } = false;
    }
}
