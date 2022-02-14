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
            // Todo - make sure you're outside
            if (dryRun)
            {
                return ActionOutcome.Success();
            }
            
            return ActionOutcome.Success();
        }
    }
}
