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
        #region Saving variables
        public int lastCycle = -1; //This refers to the last cycle the creature was saved so whenever it is loaded the afflictions are properily calculated
        #endregion

        public float maxHealth = 0;
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

        public int healingRateTics = 600;
        public int healingRate = 600;

        public bool hasEaten = false;

        public int medicalSkill = 0;

        public string violenceAttackOverride = "";
        public string violenceAttackerOverride = "";

        #region Treatment
        public float tendTime = 10;
        public float tendTimeMax = 10;
        public readonly float tendTimeBase = 60;
        public Creature tendTarget = null;

        public RWAffliction tendAffliction = null;
        #endregion

        #region Slugcat Specific Variables
        public float poleClimbSpeedFac = 1f;
        public float corridorClimbSpeedFac = 1f;
        public float runspeedFac = 1.27f;
        public float swimForceFac = 1f;
        #endregion
    }

    public class OneTimeUseData
    {
        public List<Creature> creatures = new();
    }

    public class RWWeaponStats
    {
        public string quality = "Normal";

        public float damage = 1;
        public float AP = 0;
    }    

    public static readonly ConditionalWeakTable<CreatureState, RWState> healthState = new();

    public static readonly ConditionalWeakTable<Explosion, OneTimeUseData> singleExplosion = new();

    public static readonly ConditionalWeakTable<AbstractPhysicalObject, RWWeaponStats> weaponstat = new();

    public static readonly MoreSlugcats.SlugNPCAI.BehaviorType SlugTend = new("SlugTend", true);
    public static readonly MoreSlugcats.SlugNPCAI.BehaviorType SlugSelfTend = new("SlugSelfTend", true);

    public static string all = "ShadowOfRWHealth: ";

    private bool init = false;

    public HealthTab healthTab;
    public bool buttonHeld = false;

    public static float cycleLength = 13;
    public static float afterCycleLength = 13;

    internal static new ManualLogSource Logger;

    public static ShadowOfOptions optionsMenuInstance;

    public static RimWorldHealthHandler rimWorldHealthHandler;

    public void OnEnable()
    {
        try
        {
            Logger = base.Logger;

            On.HUD.HUD.InitSinglePlayerHud += HUDInitSinglePlayerHud;
            On.HUD.PlayerSpecificMultiplayerHud.ctor += NewPlayerSpecificMultiplayerHud;

            On.Player.Update += PlayerUpdate;

            On.Player.checkInput += PlayercheckInput;

            rimWorldHealthHandler = new(["notsmartcat"], "rimworldhealth");

            ILHooks.Apply();
            CreatureHooks.Apply();
            SlugcatHooks.Apply();
            WeaponHooks.Apply();
            SavingandLoadingHooks.Apply();

            On.RainWorld.OnModsInit += ModInit;
        }
        catch (Exception e) { Logger.LogError(e); }
    }

    void ModInit(On.RainWorld.orig_OnModsInit orig, RainWorld rainWorld)
    {
        orig(rainWorld);
        try
        {
            if (!init)
            {
                init = true;
            }
            optionsMenuInstance = new(this);
            MachineConnector.SetRegisteredOI("notsmartcat.shadowofrimworldhealth", optionsMenuInstance);
        }
        catch (Exception e) { Logger.LogError(e); }
    }

    void PlayercheckInput(On.Player.orig_checkInput orig, Player self)
    {
        orig(self);

        if (healthTab != null && healthTab.player == self.abstractCreature && healthTab.visible)
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

        if (!healthState.TryGetValue(abstractPlayer.state, out _))
        {
            return;
        }

        healthTab = new HealthTab(hud, abstractPlayer);

        self.hud.AddPart(healthTab);
    }

    void HUDInitSinglePlayerHud(On.HUD.HUD.orig_InitSinglePlayerHud orig, HUD.HUD self, RoomCamera cam)
    {
        orig(self, cam);

        if (!healthState.TryGetValue((self.owner as Creature).State, out _))
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
                            RWHealthState.Damage(self.State, state, new RWBomb(), 0.5f, 0, head, "testtesttesttesttesttest");
                        }
                        else if(j == 1)
                        {
                            RWHealthState.Damage(self.State, state, new RWBomb(), 0.5f, 0, head, "testtesttesttesttesttest testtesttesttesttesttest");
                        }
                        else if (j == 2)
                        {
                            RWHealthState.Damage(self.State, state, new RWBomb(), 0.5f, 0, head, "testtesttesttesttesttest testtesttesttesttesttest testtesttesttesttesttest");
                        }

                        state.updateCapacities = true;
                        break;
                    }
                }
            }
            if (Input.GetKey("m"))
            {
                if (false && state.wholeBodyAfflictions.Count == 0)
                {
                    state.wholeBodyAfflictions.Add(new RWFlu(self.State, null));
                }

                
                for (int i = 0; i < state.bodyParts.Count; i++)
                {
                    if (state.bodyParts[i] is Arm part && !IsDestroyed(part) && part.subName == "Right")
                    {
                        RWHealthState.Damage(self.State, state, new RWPoke(), 999999f, 999, part, "oopsie");

                        break;
                    }
                }

                for (int i = 0; i < state.bodyParts.Count; i++)
                {
                    if (state.bodyParts[i] is Hand part && !IsDestroyed(part) && part.subName == "Left")
                    {
                        RWHealthState.Damage(self.State, state, new RWPoke(), 999999f, 999, part, "oopsie");

                        break;
                    }
                }


                state.updateCapacities = true;
            }
        }

        if (self.Stunned && !self.dead)
        {
            return;
        }

        if (Input.GetKey("h") && healthTab != null)
        {
            if (buttonHeld)
            {
                return;
            }

            buttonHeld = true;

            CreatureState healthTabCreatureState = self.State;
            RWState healthTabState = state;

            foreach (var grasp in self.grasps)
            {
                if (grasp != null && grasp.grabbedChunk != null && grasp.grabbedChunk.owner != null && grasp.grabbedChunk.owner is Creature crit && crit.State != null && healthState.TryGetValue(crit.State, out healthTabState))
                {
                    healthTabCreatureState = crit.State;
                    break;
                }
            }

            healthTab.ToggleVisibility(healthTabCreatureState, healthTabState);
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
                if (IsSubPartName(self, state.bodyParts[i]) && state.bodyParts[i] is not UpperTorso && state.bodyParts[i].afflictions.Count == 1 && state.bodyParts[i].afflictions[0] is RWDestroyed)
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

    #region Custom Damages
    public static void BombDamage(CreatureState self, RWState state, float damage, string attackName = "", string attackerName = "")
    {
        int amount = UnityEngine.Random.Range(1, 5);

        RWBodyPart focusedBodyPart;

        List<RWBodyPart> list = new();

        for (int p = 0; p < amount; p++)
        {
            focusedBodyPart = null;
            list.Clear();

            bool isSuper = attackName == "Energy Cell" || attackName == "Singularity Bomb";

            for (int i = 0; i < state.bodyParts.Count; i++)
            {
                if (state.bodyParts[i].connectedBodyChunks.Count > 0 && !state.bodyParts[i].isInternal && !IsDestroyed(state.bodyParts[i]))
                {
                    list.Add(state.bodyParts[i]);
                }
            }

            if (list.Count > 1)
            {
                float chance = 0;

                for (int i = 0; i < list.Count; i++)
                {
                    chance += list[i].coverage;
                }

                float roll = UnityEngine.Random.Range(0f, chance);

                chance = 0;

                for (int i = 0; i < list.Count; i++)
                {
                    chance += list[i].coverage;

                    if (roll <= chance)
                    {
                        focusedBodyPart = list[i];
                        break;
                    }
                }
            }
            else if (list.Count == 1)
            {
                focusedBodyPart = list[0];
            }

            list.Clear();

            list.Add(focusedBodyPart);

            while (focusedBodyPart != null && focusedBodyPart is not Neck)
            {
                for (int i = 0; i < state.bodyParts.Count; i++)
                {
                    if (!IsDestroyed(state.bodyParts[i]) && state.bodyParts[i].isInternal && IsSubPartName(state.bodyParts[i], list[0]))
                    {
                        list.Add(state.bodyParts[i]);
                    }
                }

                if (list.Count == 0)
                {
                    break;
                }
                else if (list.Count == 1)
                {
                    focusedBodyPart = list[0];
                    break;
                }

                float chance = 0;

                for (int i = 0; i < list.Count; i++)
                {
                    chance += list[i].coverage;
                }

                float roll = UnityEngine.Random.Range(0f, chance);

                chance = 0;

                RWBodyPart tempFocusedBodyPart = null;

                for (int i = 0; i < list.Count; i++)
                {
                    chance += list[i].coverage;

                    if (roll <= chance)
                    {
                        tempFocusedBodyPart = list[i];

                        break;
                    }
                }

                if (tempFocusedBodyPart == null || tempFocusedBodyPart == focusedBodyPart)
                {
                    break;
                }
                else
                {
                    list.Clear();
                    list.Add(tempFocusedBodyPart);
                }
            }

            if (focusedBodyPart != null)
            {
                RWDamageType damageType = isSuper ? new RWSuperBomb() : new RWBomb();

                RWHealthState.Damage(self, state, damageType, damage / amount, 10, focusedBodyPart, attackName, attackerName);
            }
        }
    }
    public static void BluntDamage(CreatureState self, RWState state, BodyChunk hitChunk, float damage, float AP, string attackName = "", string attackerName = "")
    {
        RWBodyPart focusedBodyPart = GetHitBodyPart(state, hitChunk, null, false, true);
        RWBodyPart secondaryFocusedBodyPart = 0.4f < UnityEngine.Random.value ? GetHitBodyPart(state, hitChunk, focusedBodyPart) : null;

        float extraDamage = 0;

        if(damage < 0.8f)
            damage = UnityEngine.Random.Range(0.8f, 1.2f);

        if (focusedBodyPart != null)
        {
            float tempDamage = damage;
            if (secondaryFocusedBodyPart != null)
            {
                tempDamage = damage * UnityEngine.Random.Range(0.8f, 0.9f);
                extraDamage = damage - tempDamage;
            }

            //Debug.Log("Primary Blunt Damage " + tempDamage + " to " + focusedBodyPart);
            Damage(focusedBodyPart, tempDamage);
        }
        if (secondaryFocusedBodyPart != null)
        {
            damage = (damage * UnityEngine.Random.Range(0.2f, 0.35f)) + extraDamage;

            Damage(secondaryFocusedBodyPart, damage);

            while (true)
            {
                focusedBodyPart = GetHitAnotherBodyPart(secondaryFocusedBodyPart);

                if (focusedBodyPart == null || secondaryFocusedBodyPart == null || focusedBodyPart == secondaryFocusedBodyPart)
                {
                    break;
                }

                //Debug.Log("Secondary Blunt Damage " + damage + " to " + focusedBodyPart);
                Damage(focusedBodyPart, damage);

                secondaryFocusedBodyPart = focusedBodyPart;

                RWBodyPart GetHitAnotherBodyPart(RWBodyPart subPartOf)
                {
                    if (subPartOf == null)
                    {
                        return null;
                    }

                    List<RWBodyPart> list = new(1) { subPartOf };
                    RWBodyPart focusedBodyPart = null;

                    for (int i = 0; i < state.bodyParts.Count; i++)
                    {
                        if (IsDestroyed(state.bodyParts[i]) || subPartOf == state.bodyParts[i])
                        {
                            continue;
                        }
                        else if (!IsSubPartName(state.bodyParts[i], subPartOf))
                        {
                            continue;
                        }

                        list.Add(state.bodyParts[i]);
                    }

                    if (list.Count > 1)
                    {
                        float chance = 0;

                        for (int i = 0; i < list.Count; i++)
                        {
                            chance += list[i].coverage;
                        }

                        float roll = UnityEngine.Random.Range(0f, chance);

                        chance = 0;

                        for (int i = 0; i < list.Count; i++)
                        {
                            chance += list[i].coverage;

                            if (roll <= chance)
                            {
                                focusedBodyPart = list[i];
                                break;
                            }
                        }
                    }
                    else if (list.Count == 1)
                    {
                        return null;
                    }

                    return focusedBodyPart == subPartOf ? null : focusedBodyPart;
                }
            }
        }

        void Damage(RWBodyPart focusedBodyPart, float damage)
        {
            RWHealthState.Damage(self, state, new RWBlunt(), damage, AP, focusedBodyPart, attackName, attackerName);
        }
    }

    public static void CutDamage(CreatureState self, RWState state, BodyChunk hitChunk, float damage, float AP, string attackName = "", string attackerName = "")
    {
        float additionalPartsRoll = UnityEngine.Random.value;
        int additionalParts;

        if (additionalPartsRoll <= 0.3f)
        {
            Damage(GetHitBodyPart(state, hitChunk), damage);
            return;
        }
        else if (additionalPartsRoll <= 0.75f)
        {
            additionalParts = 1;
        }
        else if (additionalPartsRoll <= 0.95f)
        {
            additionalParts = 2;
        }
        else
        {
            additionalParts = 3;
        }

        damage *= (1 + 1.4f) / (1 + additionalParts + 1.4f) * (1 + additionalPartsRoll);

        RWBodyPart focusedBodyPart = GetHitBodyPart(state, hitChunk);
        List<RWBodyPart> list = new(1) { focusedBodyPart };

        for (int i = 0; i < state.bodyParts.Count; i++)
        {
            if (IsDestroyed(state.bodyParts[i]) || list.Contains(state.bodyParts[i]))
            {
                continue;
            }
            else if (!IsSubPartName(state.bodyParts[i], focusedBodyPart))
            {
                continue;
            }

            //Debug.Log("CutDamage focusedBodyParts subPart " + state.bodyParts[i] + " was added");
            list.Add(state.bodyParts[i]);
        } //Get all sub parts of the focusedBodypart

        RWBodyPart tempFocusedpart = focusedBodyPart;

        for (int i = 0; i < state.bodyParts.Count; i++)
        {
            if (IsDestroyed(state.bodyParts[i]) || list.Contains(state.bodyParts[i]))
            {
                continue;
            }
            else if (!IsSubPartName(focusedBodyPart, state.bodyParts[i]))
            {
                continue;
            }

            //Debug.Log(focusedBodyPart + " parent is " + state.bodyParts[i]);

            focusedBodyPart = state.bodyParts[i];
            break;
        } //get the focusedBodypart's parent

        if (tempFocusedpart != focusedBodyPart)
        {
            for (int i = 0; i < state.bodyParts.Count; i++)
            {
                if (IsDestroyed(state.bodyParts[i]) || list.Contains(state.bodyParts[i]))
                {
                    continue;
                }
                else if (!IsSubPartName(state.bodyParts[i], focusedBodyPart))
                {
                    continue;
                }

                //Debug.Log("CutDamage focusedBodyParts subPart " + state.bodyParts[i] + " was added");

                list.Add(state.bodyParts[i]);
            } //Get all sub parts of the focusedBodypart's parent

            while (list.Count < 1 + additionalParts)
            {
                tempFocusedpart = focusedBodyPart;

                for (int i = 0; i < state.bodyParts.Count; i++)
                {
                    if (IsDestroyed(state.bodyParts[i]) || list.Contains(state.bodyParts[i]))
                    {
                        continue;
                    }
                    else if (!IsSubPartName(focusedBodyPart, state.bodyParts[i]))
                    {
                        continue;
                    }

                    //Debug.Log(focusedBodyPart + " parent is " + state.bodyParts[i]);

                    focusedBodyPart = state.bodyParts[i];
                    break;
                } //get the focusedBodypart's parent

                if (tempFocusedpart == focusedBodyPart)
                {
                    break;
                }

                for (int i = 0; i < state.bodyParts.Count; i++)
                {
                    if (IsDestroyed(state.bodyParts[i]) || list.Contains(state.bodyParts[i]))
                    {
                        continue;
                    }
                    else if (!IsSubPartName(state.bodyParts[i], focusedBodyPart))
                    {
                        continue;
                    }

                    //Debug.Log("CutDamage focusedBodyParts subPart " + state.bodyParts[i] + " was added");

                    list.Add(state.bodyParts[i]);
                } //Get all sub parts of the focusedBodypart's parent
            }
        }

        for (int i = 0; i < 1 + additionalParts; i++)
        {
            if (list.Count < i - 1 + additionalParts)
            {
                //Debug.Log("CutDamage had less bodyparts stored then hit parts, list count = " + list.Count);
                Damage(list[UnityEngine.Random.Range(0, list.Count)], damage / (1 + additionalParts));
            }
            else
            {
                int listInt = UnityEngine.Random.Range(0, list.Count);

                Damage(list[listInt], damage / (1 + additionalParts));
                //Debug.Log("CutDamage hit part is " + list[listInt]);
                list.RemoveAt(listInt);
            }
        }

        void Damage(RWBodyPart focusedBodyPart, float damage)
        {
            RWHealthState.Damage(self, state, new RWCut(), damage, AP, focusedBodyPart, attackName, attackerName);
        }
    }

    public static void ScratchDamage(CreatureState self, RWState state, BodyChunk hitChunk, float damage, float AP, string attackName = "", string attackerName = "")
    {
        RWBodyPart focusedBodyPart = GetHitBodyPart(state, hitChunk);
        //Debug.Log("ScratchDamage focusedBodyPart is " + focusedBodyPart);

        List<RWBodyPart> list = new(1) { focusedBodyPart };

        for (int i = 0; i < state.bodyParts.Count; i++)
        {
            if (IsDestroyed(state.bodyParts[i]) || list.Contains(state.bodyParts[i]))
            {
                continue;
            }
            else if (!IsSubPartName(state.bodyParts[i], focusedBodyPart))
            {
                continue;
            }

            //Debug.Log("CutDamage focusedBodyParts subPart " + state.bodyParts[i] + " was added");
            list.Add(state.bodyParts[i]);
        } //Get all sub parts of the focusedBodypart

        RWBodyPart tempFocusedpart = focusedBodyPart;

        for (int i = 0; i < state.bodyParts.Count; i++)
        {
            if (IsDestroyed(state.bodyParts[i]) || list.Contains(state.bodyParts[i]))
            {
                continue;
            }
            else if (!IsSubPartName(focusedBodyPart, state.bodyParts[i]))
            {
                continue;
            }

            //Debug.Log(focusedBodyPart + " parent is " + state.bodyParts[i]);

            focusedBodyPart = state.bodyParts[i];
            break;
        } //get the focusedBodypart's parent

        if (tempFocusedpart != focusedBodyPart)
        {
            for (int i = 0; i < state.bodyParts.Count; i++)
            {
                if (IsDestroyed(state.bodyParts[i]) || list.Contains(state.bodyParts[i]))
                {
                    continue;
                }
                else if (!IsSubPartName(state.bodyParts[i], focusedBodyPart))
                {
                    continue;
                }

                //Debug.Log("CutDamage focusedBodyParts subPart " + state.bodyParts[i] + " was added");

                list.Add(state.bodyParts[i]);
            } //Get all sub parts of the focusedBodypart's parent
        }

        int listInt = UnityEngine.Random.Range(0, list.Count);

        Damage(list[listInt], damage * 0.67f);
        //Debug.Log("CutDamage hit part is " + list[listInt]);

        listInt = UnityEngine.Random.Range(0, list.Count);

        Damage(list[listInt], damage * 0.67f);
        //Debug.Log("CutDamage second hit part is " + list[listInt]);

        void Damage(RWBodyPart focusedBodyPart, float damage)
        {
            RWHealthState.Damage(self, state, new RWScratch(), damage, AP, focusedBodyPart, attackName, attackerName);
        }
    }

    public static void CentipedeShockDamage(CreatureState self, RWState state, Centipede source, bool underwater, bool shockGiveUp)
    {
        RWBodyPart focusedBodyPart = GetHitBodyPart(state);
        //Debug.Log("CentipedeShockDamage focusedBodyPart is " + focusedBodyPart);

        string attackerName;
        float damage;
        float AP = 0;

        if (source.abstractCreature.creatureTemplate.type == CreatureTemplate.Type.SmallCentipede)
        {
            attackerName = "Baby centipede";
            damage = 1.2f;
        }
        else if (source.abstractCreature.creatureTemplate.type == CreatureTemplate.Type.Centiwing)
        {
            attackerName = "Centiwing";
            damage = 5f;
        }
        else if (ModManager.DLCShared && source.abstractCreature.creatureTemplate.type == DLCSharedEnums.CreatureTemplateType.AquaCenti)
        {
            attackerName = "AquaCenti";
            damage = 5f;
        }
        else if (source.abstractCreature.creatureTemplate.type == CreatureTemplate.Type.RedCentipede)
        {
            attackerName = "Red centipede";
            damage = 10;
        }
        else
        {
            attackerName = "Centipede";
            damage = 5;
        }

        if (underwater)
        {
            damage *= 1.2f;
        }

        string attackName = attackerName + (source.Submersion > 0f ? " - Underwater shock" : " - Shock");

        RWHealthState.Damage(self, state, new RWElectricalBurn(), damage, AP, focusedBodyPart, attackName, attackerName);

        if (shockGiveUp && !self.dead)
        {
            source.Stun(6);
            source.shockGiveUpCounter = Math.Max(source.shockGiveUpCounter, 30);
            source.AI.annoyingCollisions = Math.Min(source.AI.annoyingCollisions * 2, 150);
        }
    }

    public static void LethalWaterDamage(CreatureState self, RWState state)
    {
        RWBodyPart focusedBodyPart = GetHitBodyPart(state);

        RWDamageType damageType;
        string attackName;

        if (self.creature.Room.realizedRoom.roomSettings.GetEffect(RoomSettings.RoomEffect.Type.LavaSurface) != null)
        {
            damageType = new RWBurn();
            attackName = "Lava";
        }
        else
        {
            damageType = new RWAcidBurn();
            attackName = "Acidic water";
        }

        float damage = 8 + self.creature.realizedCreature.lavaContactCount;

        RWHealthState.Damage(self, state, damageType, damage, 999, focusedBodyPart, attackName);
    }

    public static void LocustDamage(CreatureState self, RWState state)
    {
        RWHealthState.Damage(self, state, new RWBite(), 1, 0, GetHitBodyPart(state), "Locust swarm - Mandibles");
    }

    public static void TerrainImpactDamage(Player self, RWState state, int hitChunk, float speed, bool death)
    {
        RWBodyPart part = GetHitBodyPart(state, self.bodyChunks[hitChunk], null, false, true);

        part ??= GetHitBodyPart(state);

        float damage;

        bool gourm = self.isGourmand;

        if (death)
        {
            if (speed < (gourm ? 90 : 70))
            {
                damage = 8;
            }
            else if (speed < (gourm ? 100 : 80))
            {
                damage = 10;
            }
            else if (speed < (gourm ? 100 : 80))
            {
                damage = 12;
            }
            else
            {
                damage = 14;
            }
        }
        else
        {
            if (speed < (gourm ? 50 : 45))
            {
                damage = 2;
            }
            else if (speed < (gourm ? 60 : 55))
            {
                damage = 4;
            }
            else if (speed < (gourm ? 70 : 75))
            {
                damage = 6;
            }
            else
            {
                damage = 8;
            }
        }

        RWHealthState.Damage(self.State, state, new RWBlunt(), damage, 0, part, "Terrain impact");
    }

    public static void TongueElectricDamage(CreatureState self, RWState state)
    {
        for (int i = 0; i < state.bodyParts.Count; i++)
        {
            if (state.bodyParts[i] is Tongue part && !IsDestroyed(part))
            {
                RWHealthState.Damage(self, state, new RWElectricalBurn(), 8, 999, part, "Zap-Coil");
                break;
            }
        }
    }

    public static void RainDamage(CreatureState self, RWState state, bool death)
    {
        float damage = death ? 8f : 0.5f;

        RWHealthState.Damage(self, state, new RWBlunt(), damage, 999, GetHitBodyPart(state, null, null, false, true), "Rain");
    }
    public static void SandstormDamage(CreatureState self, RWState state, bool death)
    {
        float damage = death ? 8f : 0.5f;

        RWHealthState.Damage(self, state, new RWBlunt(), damage, 999, GetHitBodyPart(state, null, null, false, true), "Sandstorm");
    }

    public static void SpiderDamage(CreatureState self, RWState state)
    {
        RWHealthState.Damage(self, state, new RWBite(), 1, 10, GetHitBodyPart(state), "Coalescipede - Fangs");
    }

    public static void ZapCoilDamage(CreatureState self, RWState state)
    {
        RWHealthState.Damage(self, state, new RWElectricalBurn(), 20, 999, GetHitBodyPart(state), "Zap-Coil");
    }
    public static void BigJellyfishDamage(CreatureState self, RWState state)
    {
        RWHealthState.Damage(self, state, new RWElectricalBurn(), 20, 999, GetHitBodyPart(state), "Big Jellyfish - Electricity");
    }

    public static void ARZapperDamage(CreatureState self, RWState state, bool death)
    {
        float damage = death ? 12f : 8f;

        RWHealthState.Damage(self, state, new RWElectricalBurn(), damage, 999, GetHitBodyPart(state), "Zapper");
    }
    #endregion

    public static RWBodyPart GetHitBodyPart(RWState state, BodyChunk hitChunk = null, RWBodyPart subPartOf = null, bool canHitInternal = false, bool isBlunt = false)
    {
        List<RWBodyPart> list = new();
        RWBodyPart focusedBodyPart = null;

        for (int i = 0; i < state.bodyParts.Count; i++)
        {
            if (IsDestroyed(state.bodyParts[i]))
            {
                continue;
            }
            else if (subPartOf == null)
            {
                if (hitChunk != null && !state.bodyParts[i].connectedBodyChunks.Contains(hitChunk.index) || hitChunk == null && state.bodyParts[i].connectedBodyChunks.Count == 0)
                {
                    continue;
                }
                else if (state.bodyParts[i].isInternal && (isBlunt || !canHitInternal))
                {
                    continue;
                }
                else if (isBlunt && state.bodyParts[i] is Eye)
                {
                    continue;
                }
            }
            else if (subPartOf != null && !IsSubPartName(state.bodyParts[i], subPartOf))
            {
                continue;
            }

            list.Add(state.bodyParts[i]);
        }

        if (list.Count > 1)
        {
            float chance = 0;

            for (int i = 0; i < list.Count; i++)
            {
                chance += list[i].coverage;
            }

            float roll = UnityEngine.Random.Range(0f, chance);

            chance = 0;

            for (int i = 0; i < list.Count; i++)
            {
                chance += list[i].coverage;

                if (roll <= chance)
                {
                    focusedBodyPart = list[i];
                    break;
                }
            }
        }
        else if (list.Count == 1)
        {
            focusedBodyPart = list[0];
        }

        if (focusedBodyPart == null)
        {
            Debug.Log("Error, hit bodypart is null");
            RimWorldHealth.Logger.LogInfo(all + "Error, hit bodypart is null");
        }

        return focusedBodyPart;
    }

    public static float BombDamageMultiplier(bool hasHealthState, bool isSuper)
    {
        if (isSuper)
        {
            return hasHealthState ? 550 : 78.57142857142857f;
        }

        return hasHealthState ? 250 : 35.71428571428571f;
    }

    public static void SetToxicBuildup(RWState state, Creature self)
    {
        RWToxicBuildup toxicBuildup = null;

        for (int i = 0; i < state.wholeBodyAfflictions.Count; i++)
        {
            if (state.wholeBodyAfflictions[i] is RWToxicBuildup tempToxicBuildup)
            {
                toxicBuildup = tempToxicBuildup;
                break;
            }
        }

        float poison = self.injectedPoison / self.Template.instantDeathDamageLimit;

        if (self is InsectoidCreature insectoid && insectoid.poison > 0)
        {
            poison += insectoid.poison;
        }

        if (poison < 0.04f)
        {
            if (toxicBuildup != null)
            {
                state.wholeBodyAfflictions.Remove(toxicBuildup);
                state.updateCapacities = true;
            }
        }
        else
        {
            if (toxicBuildup != null)
            {
                toxicBuildup.tendQuality = Math.Min(1, poison);
            }
            else
            {
                state.wholeBodyAfflictions.Add(new RWToxicBuildup(self.State, null, Math.Min(1, poison)));
                state.updateCapacities = true;
            }
        }
    }

    public static string GetCreatureName(Creature self)
    {
        if (self == null)
        {
            return "";
        }

        string name;

        if (self is BigNeedleWorm)
        {
            name = "Adult Noodlefly";
        }
        else if (self is BigSpider bigSpider)
        {
            if (bigSpider.spitter)
            {
                name = "Spitter Spider";
            }
            else if (bigSpider.mother)
            {
                name = "Mother Spider";
            }
            else
            {
                name = "Big Spider";
            }
        }
        else if (self is DropBug)
        {
            name = "Dropwig";
        }
        else if (self is EggBug)
        {
            name = "Firebug";
        }
        else if (self is JetFish)
        {
            name = "Jetfish";
        }
        else if (self is Vulture vulture)
        {
            if (vulture.IsKing)
            {
                name = "King Vulture";
            }
            else if (vulture.IsMiros)
            {
                name = "Miros Vulture";
            }
            else
            {
                name = "Vulture";
            }
        }
        else if (self is Leech leech)
        {
            if (leech.jungleLeech)
            {
                name = "Jungle Leech";
            }
            else if (leech.seaLeech)
            {
                name = "Sea Leech";
            }
            else
            {
                name = "Leech";
            }
        }
        else if (self is Lizard)
        {
            name = "Lizard";
        } //Will add all non-modded Lizards later
        else if (self is MirosBird)
        {
            name = "Miros Bird";
        }
        else if (self is Player)
        {
            name = ((PlayerState)self.State).isPup ? "Slugpup" : "Slugcat";
        } //Will add all non-modded Slugcats later
        else if (self is SkyWhale)
        {
            name = "Sky Whale";
        }
        else if (self is Scavenger scavenger)
        {
            if (scavenger.King)
            {
                name = "Chieftain Scavenger";
            }
            else if (scavenger.Elite)
            {
                name = "Elite Scavenger";
            }
            else if (scavenger.Templar)
            {
                name = "Scavenger Templar";
            }
            else if (scavenger.Disciple)
            {
                name = "Scavenger Disciple";
            }
            else
            {
                name = "Scavenger";
            }
        }
        else if (ModManager.MSC && self is MoreSlugcats.BigJellyFish)
        {
            name = "Big Jellyfish";
        }
        else if (ModManager.MSC && self is MoreSlugcats.StowawayBug)
        {
            name = "Stowaway";
        }
        else if (ModManager.Watcher && self is Watcher.BoxWorm)
        {
            name = "Box Worm";
        }
        else if (ModManager.Watcher && self is Watcher.DrillCrab)
        {
            name = "Drill Crab";
        }
        else if (ModManager.Watcher && self is Watcher.Frog)
        {
            name = "Frog";
        }
        else if (ModManager.Watcher && self is Watcher.Loach)
        {
            name = "Loach";
        }
        else if (ModManager.Watcher && self is Watcher.Rat)
        {
            name = "Rat";
        }
        else if (ModManager.Watcher && self is Watcher.RippleSpider)
        {
            name = "Ripple Spider";
        }
        else
        {
            name = self.ToString();
            Debug.Log(self + " is a unknown creature");
        }

        return name;
    }

    public static void KarmaFlowerHeal(Creature self, RWState state)
    {
        #region Tier 1 (Missing important BodyParts)
        for (int i = 0; i < state.bodyParts.Count; i++)
        {
            if (state.bodyParts[i] is LowerTorso lowerTorso)
            {
                if (lowerTorso.afflictions.Count == 1 && lowerTorso.afflictions[0] is RWDestroyed)
                {
                    lowerTorso.afflictions.Clear();
                    return;
                }

                break;
            }
        }

        Dictionary<string, List<RWBodyPart>> limbMissingBodyparts = new();

        for (int i = 0; i < state.legSetNames.Count; i++)
        {
            limbMissingBodyparts = state.legSet[state.legSetNames[i]].KarmaFlowerGetMainMissingPart(state.legSetNames[i], limbMissingBodyparts);
        }

        if (limbMissingBodyparts.Count > 0)
        {
            Dictionary<string, RWBodyPart> mainBodyparts = new();

            List<string> mainBodypartNames = new();

            bool containsLeg = false;
            bool containsOneItem = false;
            bool containsBone = false;

            foreach (var dic in limbMissingBodyparts)
            {
                if (dic.Value[0] is Leg leg)
                {
                    if (!containsLeg)
                    {
                        mainBodyparts.Clear();
                        mainBodypartNames.Clear();
                    }

                    containsLeg = true;
                    mainBodyparts.Add(dic.Key, leg);
                    mainBodypartNames.Add(dic.Key);
                }
                else if (!containsLeg)
                {
                    if (dic.Value.Count == 1)
                    {
                        if (!containsOneItem)
                        {
                            mainBodyparts.Clear();
                            mainBodypartNames.Clear();
                        }

                        containsOneItem = true;
                        mainBodyparts.Add(dic.Key, dic.Value[0]);
                        mainBodypartNames.Add(dic.Key);
                    }
                    else if (!containsOneItem)
                    {
                        foreach (RWBodyPart part in dic.Value)
                        {
                            if (part is RWBone)
                            {
                                if (!containsBone)
                                {
                                    mainBodyparts.Clear();
                                    mainBodypartNames.Clear();
                                }

                                containsBone = true;
                                mainBodyparts.Add(dic.Key, part);
                                mainBodypartNames.Add(dic.Key);
                                break;
                            }
                            else if (!containsBone)
                            {
                                mainBodyparts.Add(dic.Key, part);
                                mainBodypartNames.Add(dic.Key);
                                break;
                            }
                        }
                    }
                }
            }

            int randomNumber = UnityEngine.Random.Range(0, mainBodypartNames.Count);

            state.legSet[mainBodypartNames[randomNumber]].KarmaFlowerHeal(state, mainBodyparts[mainBodypartNames[randomNumber]]);

            return;
        }

        for (int i = 0; i < state.armSetNames.Count; i++)
        {
            limbMissingBodyparts = state.armSet[state.armSetNames[i]].KarmaFlowerGetMainMissingPart(state.armSetNames[i], limbMissingBodyparts);
        }

        if (limbMissingBodyparts.Count > 0)
        {
            Dictionary<string, RWBodyPart> mainBodyparts = new();

            List<string> mainBodypartNames = new();

            bool containsShoulder = false;
            bool containsArm = false;
            bool containsOneItem = false;
            bool containsBone = false;

            foreach (var dic in limbMissingBodyparts)
            {
                if (dic.Value[0] is Shoulder || dic.Value[0] is Clavicle)
                {
                    if (!containsShoulder)
                    {
                        mainBodyparts.Clear();
                        mainBodypartNames.Clear();
                    }

                    containsShoulder = true;
                    mainBodyparts.Add(dic.Key, dic.Value[0]);
                    mainBodypartNames.Add(dic.Key);
                }
                else if (!containsShoulder)
                {
                    if (dic.Value[0] is Arm)
                    {
                        if (!containsArm)
                        {
                            mainBodyparts.Clear();
                            mainBodypartNames.Clear();
                        }

                        containsArm = true;
                        mainBodyparts.Add(dic.Key, dic.Value[0]);
                        mainBodypartNames.Add(dic.Key);
                    }
                    else if (!containsArm)
                    {
                        if (dic.Value.Count == 1)
                        {
                            if (!containsOneItem)
                            {
                                mainBodyparts.Clear();
                                mainBodypartNames.Clear();
                            }

                            containsOneItem = true;
                            mainBodyparts.Add(dic.Key, dic.Value[0]);
                            mainBodypartNames.Add(dic.Key);
                        }
                        else if (!containsOneItem)
                        {
                            foreach (RWBodyPart part in dic.Value)
                            {
                                if (part is RWBone)
                                {
                                    if (!containsBone)
                                    {
                                        mainBodyparts.Clear();
                                        mainBodypartNames.Clear();
                                    }

                                    containsBone = true;
                                    mainBodyparts.Add(dic.Key, part);
                                    mainBodypartNames.Add(dic.Key);
                                    break;
                                }
                                else if (!containsBone)
                                {
                                    mainBodyparts.Add(dic.Key, part);
                                    mainBodypartNames.Add(dic.Key);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            int randomNumber = UnityEngine.Random.Range(0, mainBodypartNames.Count);

            state.armSet[mainBodypartNames[randomNumber]].KarmaFlowerHeal(state, mainBodyparts[mainBodypartNames[randomNumber]]);

            return;
        }

        for (int i = 0; i < state.bodyParts.Count; i++)
        {
            if (state.bodyParts[i] is Spine spine)
            {
                if (spine.afflictions.Count == 1 && spine.afflictions[0] is RWDestroyed)
                {
                    spine.afflictions.Clear();
                    return;
                }

                break;
            }
        }
        #endregion

        #region Tier 2 Whole Body Ailments
        if (self.injectedPoison > 0 || self is InsectoidCreature && (self as InsectoidCreature).poison > 0)
        {
            if (self is InsectoidCreature insectoidCreature)
            {
                insectoidCreature.poison = 0;
            }

            self.injectedPoison = 0;

            return;
        }

        RWDisease diseaseToHeal = null;

        foreach (RWAffliction affliction in state.wholeBodyAfflictions)
        {
            if (affliction is RWDisease disease && (diseaseToHeal == null || disease.severity > diseaseToHeal.severity))
            {
                diseaseToHeal = disease;
            }
        }

        if (diseaseToHeal == null)
        {
            foreach (RWBodyPart part in state.bodyParts)
            {
                foreach (RWAffliction affliction in part.afflictions)
                {
                    if (affliction is RWDisease disease && (diseaseToHeal == null || disease.severity > diseaseToHeal.severity))
                    {
                        diseaseToHeal = disease;
                    }
                }
            }
        }

        if (diseaseToHeal != null)
        {
            if (diseaseToHeal.part == null)
            {
                state.wholeBodyAfflictions.Remove(diseaseToHeal);
                diseaseToHeal.state = null;
            }
            else
            {
                state.wholeBodyAfflictions.Remove(diseaseToHeal);
                diseaseToHeal.part = null;
                diseaseToHeal.state = null;
            }

            state.updateCapacities = true;

            return;
        }

        if (state.bloodLoss >= 0.15f)
        {
            state.bloodLoss = 0;

            return;
        }

        if (self.Hypothermia >= 0.04f)
        {
            self.Hypothermia = 0;

            return;
        }

        //Frail (might add at some point)
        #endregion

        //Tier 3 Brain Ailments (if I ever add any)

        #region Tier 4 Scars on the brain
        RWScar scarToHeal = null;

        foreach (RWAffliction affliction in state.consciousnessSource.afflictions)
        {
            if (affliction is RWScar scar && (scar.isRevealed || scar.isPermanent))
            {
                if (scarToHeal == null || scarToHeal != null && scar.scarDamage > scarToHeal.scarDamage)
                {
                    scarToHeal = scar;
                }
            }
        }

        if (scarToHeal != null)
        {
            scarToHeal.part.afflictions.Remove(scarToHeal);

            scarToHeal.part = null;
            scarToHeal.state = null;

            state.updateCapacities = true;

            return;
        }
        #endregion

        #region Tier 5 Missing organs
        List<Lung> lungs = new();
        List<Kidney> kidneys = new();
        List<Eye> eyes = new();
        List<Ear> ears = new();
        List<RWBodyPart> facialParts = new();
        List<RWBodyPart> fingersAndToes = new();

        foreach (RWBodyPart part in state.bodyParts)
        {
            if (part is Lung lung)
            {
                lungs.Add(lung);
            }
            else if (part is Kidney kidney)
            {
                kidneys.Add(kidney);
            }
            else if (part is Eye eye)
            {
                facialParts.Add(eye);
                eyes.Add(eye);
            }
            else if (part is Ear ear)
            {
                facialParts.Add(ear);
                ears.Add(ear);
            }
            else if (part is Nose || part is Jaw || part is Tongue)
            {
                facialParts.Add(part);
            }
            else if (part is Finger || part is Toe)
            {
                fingersAndToes.Add(part);
            }
        }

        foreach (Lung lung in lungs)
        {
            if (lung.afflictions.Count == 1 && lung.afflictions[0] is RWDestroyed)
            {
                lung.afflictions.Clear();

                return;
            }
        }

        foreach (Kidney kidney in kidneys)
        {
            if (kidney.afflictions.Count == 1 && kidney.afflictions[0] is RWDestroyed)
            {
                kidney.afflictions.Clear();

                return;
            }
        }

        foreach (RWBodyPart part in facialParts)
        {
            if (part.afflictions.Count == 1 && part.afflictions[0] is RWDestroyed)
            {
                part.afflictions.Clear();

                return;
            }
        }

        foreach (RWBodyPart part in fingersAndToes)
        {
            if (part.afflictions.Count == 1 && part.afflictions[0] is RWDestroyed)
            {
                part.afflictions.Clear();

                return;
            }
        }
        #endregion

        //Tier 6 Ailments in other parts (currently there are none, this is wher the eyes and ears list would be used)

        //Tier 7 Drug addictions (there might never be any)

        #region Tier 8 Scars on other parts
        foreach (RWBodyPart part in state.bodyParts)
        {
            foreach (RWAffliction affliction in part.afflictions)
            {
                if (affliction is RWScar scar && (scar.isRevealed || scar.isPermanent))
                {
                    if (scarToHeal == null || scarToHeal != null && scar.scarDamage > scarToHeal.scarDamage)
                    {
                        scarToHeal = scar;
                    }
                }
            }
        }

        if (scarToHeal != null)
        {
            scarToHeal.part.afflictions.Remove(scarToHeal);

            scarToHeal.part = null;
            scarToHeal.state = null;

            state.updateCapacities = true;

            return;
        }
        #endregion

        #region Tier 9 Injuries
        RWInjury injuryToHeal = null;

        foreach (RWBodyPart part in state.bodyParts)
        {
            foreach (RWAffliction affliction in part.afflictions)
            {
                if (affliction is RWInjury injury && injury is not RWDestroyed && (affliction is not RWScar || affliction is RWScar scar && !scar.isRevealed && !scar.isPermanent))
                {
                    if (injuryToHeal == null || injuryToHeal != null && injury.damage > injuryToHeal.damage)
                    {
                        injuryToHeal = injury;
                    }
                }
            }
        }

        if (injuryToHeal != null)
        {
            injuryToHeal.part.afflictions.Remove(injuryToHeal);

            injuryToHeal.part = null;
            injuryToHeal.state = null;

            state.updateCapacities = true;

            return;
        }
        #endregion
    }
}