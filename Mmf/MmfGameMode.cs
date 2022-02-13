using System;

namespace InfoReader.Mmf; 
[Flags]
public enum MmfGameMode
{
    Unknown = -1,
    Osu = 1,
    Taiko = 2,
    Catch = 4,
    Mania = 8
}