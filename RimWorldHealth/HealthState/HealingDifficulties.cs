using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowOfRimWorldHealth
{
    public class RWHealingDifficulty
    {
        public RWHealingDifficulty() 
        {

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

        public string solidTreated = "Set";
    }

    public class RWShredded : RWHealingDifficulty
    {
        public RWShredded() : base()
        {
            name = "Shredded";

            infectionChance = 20;

            destroyed = "Torn off";
            destroyedOut = "Torn out";
        }
    }
    public class RWCrack : RWHealingDifficulty
    {
        public RWCrack() : base()
        {
            name = "Crack";

            pain = 1;

            bleeding = 0;

            infectionChance = 0;

            becomeOldChance = 0;

            destroyed = "Shattered";
            destroyedOut = "Shattered";

            treated = "Set";
            innerTreated = "Set";
        }
    }
}
