using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngine.Engine;

namespace SuperTownDefense
{
    public class PausedState : EntityState
    {
        public PausedState(SuperTownGame game) : base(game, "pause")
        {
            
        }
    }
}
