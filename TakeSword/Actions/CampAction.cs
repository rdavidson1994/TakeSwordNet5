using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakeSword
{  
    public record CampAction(Entity Actor) : IGameAction
    {
        public ActionOutcome Execute(bool dryRun = false)
        {
            // Do nothing - placeholder
            return ActionOutcome.Success();
        }
    }
}
