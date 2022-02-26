using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using osuTools.Game;

namespace InfoReader.Mmf
{
    public interface IStatusMmf : IMmf
    {
        OsuGameStatus EnabledStatus { get; set; }
    }
}
