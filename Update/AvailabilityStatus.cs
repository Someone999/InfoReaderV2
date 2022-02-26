using System;

namespace InfoReader.Update;
[Flags]
public enum AvailabilityStatus
{
    None = 0,
    Files = 1,
    Changelog = 2
}