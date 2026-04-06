using BepInEx;
using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ShadowOfRimWorldHealth;

[BepInPlugin("notsmartcat.shadowofrimworldhealth", "RimWorld styled health", "1.0.0")]

public class RimWorldHealth : BaseUnityPlugin
{
    public class RWState
    {
        public float maxHealth;
        public float bodySizeFactor = 1;

        public List<RWBodyPart> bodyParts = new();

        public List<RWAffliction> wholeBodyAfflictions = new();

        public Brain consciousnessSource = null;

        public readonly List<string> armSetNames = new();
        public readonly Dictionary<string, ArmSet> armSet = new();
        public readonly List<string> legSetNames = new();
        public readonly Dictionary<string, LegSet> legSet = new();

        public float bloodLoss = 0;
        public float bloodLossPerCycle = 0;

        public float pain = 0;

        public bool forceUnconsciousness = false;

        #region Capacities
        public float consciousness = 1;
        public float moving = 1;
        public float manipulation = 1;
        public float talking = 1;
        public float eating = 1;
        public float sight = 1;
        public float hearing = 1;
        public float breathing = 1;
        public float bloodFiltration = 1;
        public float bloodPumping = 1;
        public float digestion = 1;

        public readonly List<RWBodyPart> bloodFiltrationBP = new();
        public readonly List<RWBodyPart> bloodPumpingBP = new();
        public readonly List<RWBodyPart> breathingBP = new();
        public readonly List<RWBodyPart> digestionBP = new();
        public readonly List<RWBodyPart> eatingBP = new();
        public readonly List<RWBodyPart> hearingBP = new();
        public readonly List<RWBodyPart> manipulationBP = new();
        public readonly List<RWBodyPart> movingBP = new();
        public readonly List<RWBodyPart> sightBP = new();
        public readonly List<RWBodyPart> talkingBP = new();

        public readonly List<RWAffliction> capacityAffectingAffliction = new();

        public bool updateCapacities = false;
        #endregion

        public float cycleLength = 13;

        public int healingRateTics = 600;
        public int healingRate = 600;

        public bool hasEaten = false;

        public int medicalSkill = 0;
    }

    public class OneTimeUseData
    {
        public List<Creature> creatures = new();
    }

    public static readonly ConditionalWeakTable<CreatureState, RWState> healthState = new();

    public static readonly ConditionalWeakTable<Explosion, OneTimeUseData> singleExplosion = new();

    public static string all = "ShadowOfRWHealth: ";
    internal static new ManualLogSource Logger;

    public HealthTab healthTab;
    public bool buttonHeld = false;

    public void OnEnable()
    {
        try
        {
            Logger = base.Logger;

            On.HUD.HUD.InitSinglePlayerHud += HUDInitSinglePlayerHud;
            On.HUD.PlayerSpecificMultiplayerHud.ctor += NewPlayerSpecificMultiplayerHud;

            On.Player.Update += PlayerUpdate;

            On.Player.checkInput += PlayercheckInput;

            ILHooks.Apply();
            CreatureHooks.Apply();
            SlugcatHooks.Apply();
            WeaponHooks.Apply();
        }
        catch (Exception e) { Logger.LogError(e); }
    }

    void PlayercheckInput(On.Player.orig_checkInput orig, Player self)
    {
        orig(self);

        if (healthTab != null && healthTab.owner == self.abstractCreature && healthTab.visible)
        {
            healthTab.input = self.input[0];

            self.input[0].x = 0;
            self.input[0].y = 0;
            Player.InputPackage[] input = self.input;
            int num2 = 0;
            input[num2].analogueDir = input[num2].analogueDir * 0f;
            self.input[0].jmp = false;
            self.input[0].thrw = false;
            self.input[0].pckp = false;
            self.input[0].spec = false;
        }
    }

    void NewPlayerSpecificMultiplayerHud(On.HUD.PlayerSpecificMultiplayerHud.orig_ctor orig, HUD.PlayerSpecificMultiplayerHud self, HUD.HUD hud, ArenaGameSession session, AbstractCreature abstractPlayer)
    {
        orig(self, hud, session, abstractPlayer);

        if (!healthState.TryGetValue(abstractPlayer.state, out RWState state))
        {
            return;
        }

        healthTab = new HealthTab(hud, abstractPlayer);

        self.hud.AddPart(healthTab);
    }

    void HUDInitSinglePlayerHud(On.HUD.HUD.orig_InitSinglePlayerHud orig, HUD.HUD self, RoomCamera cam)
    {
        orig(self, cam);

        if (!healthState.TryGetValue((self.owner as Creature).State, out RWState state))
        {
            return;
        }

        healthTab = new HealthTab(self, (self.owner as Creature).abstractCreature);

        self.AddPart(healthTab);
    }

    void PlayerUpdate(On.Player.orig_Update orig, Player self, bool eu)
    {
        orig(self, eu);

        if (healthState.TryGetValue(self.State, out RWState state))
        {
            RWHealthState.Update(self.State, state);

            if (Input.GetKey("n"))
            {
                //state.bloodLoss = 0.5f;

                for (int i = 0; i < state.bodyParts.Count; i++)
                {
                    if (state.bodyParts[i] is Head head)
                    {
                        int j = UnityEngine.Random.Range(0, 3);

                        if (j == 0)
                        {
                            RWHealthState.Damage(self.State, state, new RWBomb(), 0.5f, head, "testtesttesttesttesttest");
                        }
                        else if(j == 1)
                        {
                            RWHealthState.Damage(self.State, state, new RWBomb(), 0.5f, head, "testtesttesttesttesttest testtesttesttesttesttest");
                        }
                        else if (j == 2)
                        {
                            RWHealthState.Damage(self.State, state, new RWBomb(), 0.5f, head, "testtesttesttesttesttest testtesttesttesttesttest testtesttesttesttesttest");
                        }

                        break;
                    }
                }
            }
            if (Input.GetKey("m"))
            {
                if (state.wholeBodyAfflictions.Count == 0)
                {
                    state.wholeBodyAfflictions.Add(new RWFlu(self.State, null));
                }

                /*
                for (int i = 0; i < state.bodyParts.Count; i++)
                {
                    if (state.bodyParts[i] is Leg part)
                    {
                        state.Damage(new RWPoke(), 999999f, part, "opsie");

                        break;
                    }
                }
                */
            }
        }

        if (self.Stunned && !self.dead)
        {
            return;
        }

        if (Input.GetKey("h") && healthTab != null)
        {
            if (buttonHeld == false)
            {
                buttonHeld = true;
                healthTab.ToggleVisibility(self.State, state);
            }
        }
        else
        {
            buttonHeld = false;
        }
    }

    public static bool IsDestroyed(RWBodyPart self)
    {
        return self.afflictions.Count != 0 && self.afflictions[0] is RWDestroyed;
    }

    public static bool IsSubPartDestroyed(RWState state, RWBodyPart self)
    {
        if (self.afflictions.Count == 1 && self.afflictions[0] is RWDestroyed && self.subPartOf != "")
        {
            for (int i = 0; i < state.bodyParts.Count; i++)
            {
                if (state.bodyParts[i].name == self.subPartOf && state.bodyParts[i] is not UpperTorso && state.bodyParts[i] is not LowerTorso && state.bodyParts[i].afflictions.Count == 1 && state.bodyParts[i].afflictions[0] is RWDestroyed)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static bool IsSubPartName(RWBodyPart self, RWBodyPart other)
    {
        if (self.parentSubName)
        {
            return (self.subName + self.subPartOf) == (other.subName + other.name);
        }
        else
        {
            return self.subPartOf == other.name;
        }
    }

    public static bool LegCheck(RWState state)
    {
        if (state.moving < 0.5f)
        {
            return false;
        }

        int workingLegs = 0;
        int maxWorkingLegs = state.legSetNames.Count;

        for (int i = 0; i < state.legSetNames.Count; i++)
        {
            if (state.legSet[state.legSetNames[i]].efficiency > 0)
            {
                //Debug.Log(state.legSetNames[i] + " Leg has " + state.legSet[state.legSetNames[i]].efficiency + " efficiency");
                workingLegs++;
            }
        }

        return workingLegs >= maxWorkingLegs / 2;
    }

    public static bool ArmCheck(RWState state)
    {
        int workingArms = 0;

        for (int i = 0; i < state.armSetNames.Count; i++)
        {
            if (state.armSet[state.armSetNames[i]].efficiency > 0)
            {
                //Debug.Log(state.armSetNames[i] + " Arm has " + state.armSet[state.armSetNames[i]].efficiency + " efficiency");
                workingArms++;
            }
        }

        return workingArms > 0;
    }
}