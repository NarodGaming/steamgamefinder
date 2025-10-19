using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Narod
{

    public enum MemorySettings
    {
        Full, // default, run each function and indexer once automatically, and then only on forced user calls do we re-index
        Partial, // partially index, remember library paths and Steam path, but always re-index for games
        None // no indexing, never remember Steam paths or games and check again everytime
    };

    public class SteamGameLocatorOptions
    {
        public MemorySettings MemorySettings { get; set; } = MemorySettings.Full;
        public bool SuppressExceptions { get; set; } = false;
        public bool ShouldIndexLibrary { get; set; } = true;
    }
}
