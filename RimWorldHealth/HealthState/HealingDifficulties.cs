using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowOfRimWorldHealth
{
    public class RWHealingDifficulty
    {
        public RWHealingDifficulty(bool destroyed = false) 
        {
            if (destroyed)
            {
                return;
            }
        }

        public string name = "Misc";

        public float pain = 1.25f;

        public float scarPain = 0.625f;

        public string oldLabel = "Scar";

        public bool combines = true;

        public int bleeding = 6;

        public int infectionChance = 0;

        public int becomeOldChance = 1;

        public bool naturallyHeals = true;

        public string destroyed = "Destroyed";

        public string destroyedOut = "Destroyed";

        public string treated = "Bandaged";

        public string innerTreated = "Sutured";

        public string soldiTreated = "Set";
    }
}
