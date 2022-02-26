using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using osuTools.Game.Modes;

namespace InfoReader.Mmf
{
    public interface IModeMmf : IMmf
    {
        MmfGameMode EnabledMode { get; set; }
    }
}
