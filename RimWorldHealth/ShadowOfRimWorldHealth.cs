using BepInEx;
using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using RWCustom;

namespace ShadowOfRimWorldHealth;

[BepInPlugin("notsmartcat.shadowofrimworldhealth", "RimWorld styled health", "1.0.0")]

public class RimWorldHealth : BaseUnityPlugin
{
    public class RimState
    {
        #region Saving variables
        public int lastCycle = -1; //This refers to the last cycle the creature was saved so whenever it is loaded the afflictions are properily calculated
        #endregion

        public float maxHealth = 0; //Everytime a body part is created it's max health is added to this. Currently unused. Forgot what the purpase was but I will see if it is necessary later.
        public float bodySizeFactor = 1; //Used to multiply blood loss. Currently not implemented properly.
        public float healthScale = 1; //Reffer to https://rimworldwiki.com/wiki/Property:Health_Scale when possible. (lizards would have this be 1.2 due to the Alligator, however I will need to scale it as currently the slugcat is scaled as if it was a human, and the 1.2 is based on the human being 1. It will also need to be scaled based on the actual lizard scale)

        public List<RimBodyPart> bodyParts = new(); //The entire list of all body parts. They are created when the RWHealthState is created.

        public List<RimAffliction> wholeBodyAfflictions = new(); //List of all affliction that do not belong to a single body part but are more general. All Informational Afflictions are whole body.

        public Brain consciousnessSource = null; //Used to directly multiply consciousness my it's own efficiency, generally the Brain.

        public readonly List<string> armSetNames = new();
        public readonly Dictionary<string, ArmSet> armSet = new();
        public readonly List<string> legSetNames = new();
        public readonly Dictionary<string, LegSet> legSet = new();

        public float bloodLoss = 0; //Ranges from 0 to 1 where 1 equals death.
        public float bloodLossPerCycle = 0; //How much blood loss will increase by in the span of a cycle (13 minutes by default). If it is 100 it will take 13 minutes for the bloodloss to reach 1.

        public float pain = 0; //Ranges from 0 to 1, afflictions can increase this. If it is higher then painShockThreshold the creature will go into pain shock. 
        public float painShockThreshold = 0.8f;

        public Dictionary<string, List<float>> clothingArmour = new(); //Currently unimplemented.

        public int consciousState = 0; //0 = Conscious, 1 = Incapacitated, 2 = Pain Shock, 3 = Unconscious. in order of severity so more severe states will override

        public bool onFire = false; //If true the creature will take 2 burn damage to a random non-internal limb every second.
        public float fireSize = 0; //Ranges from 0 to 1.25 (2.5 if the creature is unconscious or dead), the fire size does not influence how much burn damage is dealt or how often it is dealt. When fire is put out is decreases by 0.1 every 10 ticks aka 0.25 seconds.
        public int fireRate = 40; //Decreased by 1 each tick and returned to 40 when it reaches 0. Used to count for when the burn damage is meant to be dealt.
        public int onFireMentalBreak = 0; //Decreased by 1 each tick, when above 0 non-player creatures will enter a special AI state where they will run around and not be able to tend, attack or flee.

        public int timeAbstracted = 0; //Increased by 1 each tick when the creature is abstracted (meaning there is no actual creature spawned). When the creature is no longer abstracted (or the cycle ends) the number is used to tend the creature and progress any diseases it has.

        #region Capacities
        public float consciousness = 1; //Multiplies manipulation, moving, eating and talking capacities by iself.
        public float moving = 1; //Along with Manipulation it multiplies movement speed by either a portion or it's entire amount depending on how leg-centric the movement is.
        public float manipulation = 1; //Along with Moving it multiplies movement speed by either a portion or it's entire amount depending on how arm-centric the movement is. Multiplies Eating Speed by 30% of itself. Multiplies Medical Operation Speed, Medical Surgery Success Chance, Medical Tend Quality and Medical Tend Speed by itself. 
        public float talking = 1; //Currently unused.
        public float eating = 1; //Multiplies Eating Speed by 95% of itself. Multiplies the amount of time creatures spend in their den after taking prey by 40% of itself.
        public float sight = 1; //Multiplies Medical Operation Speed and Medical Tend Quality by 7% of itself. Multiplies Medical Surgery Success Chance by 40% of itself. Multiplies Medical Tend Speed by 80% of itself.
        public float hearing = 1; //Currently unused.
        public float breathing = 1; //Multiplies Consciousness by 20% of itself. Multiplies Lungs Gain Multiplier and Lungs Loss Multiplier by itself.
        public float bloodFiltration = 1; //Multiplies Consciousness by 10% of itself. Multiplies Immunity Gain Speed by 50% of itself.
        public float bloodPumping = 1; //Multiplies Consciousness by 20% of itself.
        public float digestion = 1; //Multiplies the amount of time creatures spend in their den after taking prey by 60% of itself.

        //Collections of all body parts that modify each capacity. Used when setting the capacities so I do not have to go through each individual body part each time even if it doesn't modify the capacity.
        public readonly List<RimBodyPart> bloodFiltrationBP = new();
        public readonly List<RimBodyPart> bloodPumpingBP = new();
        public readonly List<RimBodyPart> breathingBP = new();
        public readonly List<RimBodyPart> digestionBP = new();
        public readonly List<RimBodyPart> eatingBP = new();
        public readonly List<RimBodyPart> hearingBP = new();
        public readonly List<RimBodyPart> manipulationBP = new();
        public readonly List<RimBodyPart> movingBP = new();
        public readonly List<RimBodyPart> sightBP = new();
        public readonly List<RimBodyPart> talkingBP = new();

        public readonly List<RimAffliction> capacityAffectingAffliction = new();

        public bool updateCapacities = false; //Set to true whenever anything happens that could cause capacities to be differet, prevents updating capacities when they would be the same.
        #endregion

        public List<int> missingBodyChunks = new(); //The body chunk index number is added to this if all body parts connected to the body chunk are destroyed. Body chunks inclided here will not be hittable by weapons.

        public bool hasEaten = false; //Set to true if the creature eats anything (some creatures cannot eat so this will need to be considered). Used to multiply Immunity Gain Speed by 0.9 if false. If the slugcat is Malnourished the multiplication is 0.9 if this is true and 0.7 if false.

        public string violenceAttackOverride = ""; //Set by certain attacks just before the Violence is called, overrides the Attack Name.
        public string violenceAttackerOverride = ""; //Set by certain attacks just before the Violence is called, overrides the Attacker Name.

        public bool wasLastBehaviourTend = false; //Used by AI to continue tending if they tended previousily.

        #region Medical
        public int medicalSkill = 0; //Ranges from 0 to 20, affects Medical Operation Speed, Medical Surgery Success Chance, Medical Tend Quality and Medical Tend Speed. Currently there is no way to increase it.

        public int healingRateTics = 400; //1 second = 40 ticks, healing should happen every 10 seconds
        public int healingRate = 400;

        public float tendTime = 10;
        public float tendTimeMax = 10;
        public readonly int tendTimeBase = 60; //1 second = 40 ticks, tending will take 1.5 seconds (couldn't find an official RimWorld base tend time)
        public Creature tendTarget = null;

        public RimTendableAffliction tendAffliction = null;
        public RimOperation tendOperation = null;

        public List<RimOperation> operationsList = new();

        public int allowedMedicine = 0; //0 is no Medicine, 1 is Herbal Medicine, 2 is Medicine and 3 is Glitterworld Medicine
        public List<int> storedMedicine = new(); //When medicine is picked up it will be stored in this list so the player doesn't have to carry it all in hands.
        #endregion

        #region Slugcat Specific Variables
        public float swimForceFac = 1f;
        #endregion

        #region Visual Data
        public List<List<float>> visualBleedAmount = new();
        public List<float> visualDisease = new(2)
        {
            0, 0
        };
        public RimFireSmoke visualFire = null;
        public LightSource lightSource = null;
        #endregion
    }

    public class OneTimeUseData
    {
        public Dictionary<CreatureState, float> hitCreatures = new();
    }

    public class RimWeaponStats
    {
        public string quality = "Normal";

        public float damage = 1;
        public float AP = 0;

        #region Weapons that can stick to creatures (Spear and DartMaggot)
        public bool wasDeflected = false;
        public bool destroyedPart = false;
        #endregion
    }

    public static readonly ConditionalWeakTable<CreatureState, RimState> healthState = new();

    public static readonly ConditionalWeakTable<Explosion, OneTimeUseData> singleExplosion = new();

    public static readonly ConditionalWeakTable<AbstractPhysicalObject, RimWeaponStats> weaponstat = new();

    public static readonly MoreSlugcats.SlugNPCAI.BehaviorType SlugTend = new("SlugTend", true);
    public static readonly MoreSlugcats.SlugNPCAI.BehaviorType SlugSelfTend = new("SlugSelfTend", true);
    public static readonly MoreSlugcats.SlugNPCAI.BehaviorType SlugOnFire = new("SlugOnFire", true);

    public static string all = "ShadowOfRWHealth: ";

    private bool init = false;

    public static List<HealthTab> healthTabs = new(4) { null, null, null, null};
    public List<bool> buttonHeld = new(4) { false, false, false, false };
    public bool tempButtonHeld = false;

    public static bool singleplayerHud = true;

    public static List<AbstractCreature> updatableCreatures = new();

    public static int cycleTick = 0;

    public static float cycleLength = 13;

    internal static new ManualLogSource Logger;

    public static ShadowOfOptions optionsMenuInstance;

    public static RimWorldHealthHandler rimWorldHealthHandler;

    public static RimTendableAffliction burningAffliction = new(null, null); //generic affliction not connected to anything and used to make tending target burn as the act of burning is not a actual affliction

    public static List<CreatureTemplate> customTemplates = new();

    public void OnEnable()
    {
        try
        {
            Logger = base.Logger;

            On.Player.Update += PlayerUpdate;

            On.Player.checkInput += PlayercheckInput;

            On.RainCycle.ctor += NewRainCycle;
            On.RainCycle.Update += RainCycleUpdate;

            On.UpdatableAndDeletable.Destroy += UpdatableAndDeletableDestroy;

            rimWorldHealthHandler = new(["notsmartcat"], "rimworldhealth");

            ILHooks.Apply();
            CreatureHooks.Apply();
            SlugcatHooks.Apply();
            WeaponHooks.Apply();
            SavingandLoadingHooks.Apply();
            HudHooks.Apply();

            On.RainWorld.OnModsInit += ModInit;
        }
        catch (Exception e) { Logger.LogError(e); }
    }

    void UpdatableAndDeletableDestroy(On.UpdatableAndDeletable.orig_Destroy orig, UpdatableAndDeletable self)
    {
        if (self is Player player && player.State != null)
        {
            if (player.State is PlayerState state && (!ModManager.MSC || player.State is not MoreSlugcats.PlayerNPCState))
            {
                healthTabs[state.playerNumber].visible = false;
            }
        }

        orig(self);
    }

    void NewRainCycle(On.RainCycle.orig_ctor orig, RainCycle self, World world, float minutes)
    {
        orig(self, world, minutes);

        cycleTick = 0;
    }
    void RainCycleUpdate(On.RainCycle.orig_Update orig, RainCycle self)
    {
        orig(self);

        cycleTick++;

        int divider = ShadowOfOptions.update_abstracted.Value switch
        {
            0 => -1,
            1 => 200,
            2 => 400,
            3 => 1200,
            4 => 2400,
            5 => 4800,
            _ => -1,
        };

        if (ShadowOfOptions.update_abstracted.Value != 0 && cycleTick % divider == 0)
        {
            List<AbstractCreature> tempUpdatableCreatures = updatableCreatures;

            foreach (AbstractCreature creature in tempUpdatableCreatures)
            {
                if (creature is null || !healthState.TryGetValue(creature.state, out RimState state))
                {
                    updatableCreatures.Remove(creature);
                    continue;
                }

                if (!UpdateAfflictions(creature.state, state, divider))
                {
                    updatableCreatures.Remove(creature);
                }
            }
        }

        //Debug.Log("RainCycle tick = " + cycleTick);
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

        int playerNumber = ((PlayerState)self.State).playerNumber;

        if (healthTabs.Count >= playerNumber + 1 && healthTabs[playerNumber] != null && healthTabs[playerNumber].player == self.abstractCreature && healthTabs[playerNumber].visible)
        {
            healthTabs[playerNumber].input = self.input[0];

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

    void PlayerUpdate(On.Player.orig_Update orig, Player self, bool eu)
    {
        orig(self, eu);

        if (!healthState.TryGetValue(self.State, out RimState state))
        {
            return;
        }

        if (ShadowOfOptions.debug_keys.Value)
        {
            if (Input.GetKey("n"))
            {
                //state.bloodLoss = 0.5f;

                if (tempButtonHeld)
                {
                    return;
                }

                tempButtonHeld = true;

                for (int i = 0; i < state.bodyParts.Count; i++)
                {
                    if (true)
                    {
                        if (state.bodyParts[i] is Tail)
                        {
                            int j = UnityEngine.Random.Range(0, 3);

                            if (j == 0)
                            {
                                RimHealthState.Damage(self.State, state, new RimCut(), 0.5f, 0, state.bodyParts[i], "testtesttesttesttesttest");
                            }
                            else if (j == 1)
                            {
                                RimHealthState.Damage(self.State, state, new RimCut(), 0.5f, 0, state.bodyParts[i], "testtesttesttesttesttest testtesttesttesttesttest");
                            }
                            else if (j == 2)
                            {
                                RimHealthState.Damage(self.State, state, new RimCut(), 0.5f, 0, state.bodyParts[i], "testtesttesttesttesttest testtesttesttesttesttest testtesttesttesttesttest");
                            }

                            state.updateCapacities = true;
                        } 
                    }
                    else
                    {
                        int k = UnityEngine.Random.Range(0, 3);

                        if (k == 0)
                        {
                            RimHealthState.Damage(self.State, state, new RimCut(), 0.5f, 0, state.bodyParts[i], "testtesttesttesttesttest");
                        }
                        else if (k == 1)
                        {
                            RimHealthState.Damage(self.State, state, new RimCut(), 0.5f, 0, state.bodyParts[i], "testtesttesttesttesttest testtesttesttesttesttest");
                        }
                        else if (k == 2)
                        {
                            RimHealthState.Damage(self.State, state, new RimCut(), 0.5f, 0, state.bodyParts[i], "testtesttesttesttesttest testtesttesttesttesttest testtesttesttesttesttest");
                        }
                    }

                    for (int k = 0; k < state.bodyParts.Count; k++)
                    {
                        if (state.bodyParts[k] is Leg part && !IsDestroyed(part))
                        {
                            RimHealthState.Damage(self.State, state, new RimPoke(), 999999f, 999, part, "oopsie");

                            break;
                        }
                    }
                }
            }
            else
            {
                tempButtonHeld = false;
            }
            if (Input.GetKey("m"))
            {
                //RWHealthState.Burn(state);

                for (int i = 0; i < state.bodyParts.Count; i++)
                {
                    if (state.bodyParts[i] is Arm part && !IsDestroyed(part) && part.subName == "Right")
                    {
                        RimHealthState.Damage(self.State, state, new RimPoke(), 999999f, 999, part, "oopsie");

                        break;
                    }
                }

                return;

                for (int i = 0; i < state.bodyParts.Count; i++)
                {
                    if (state.bodyParts[i] is Jaw part && !IsDestroyed(part))
                    {
                        RimHealthState.Damage(self.State, state, new RimPoke(), 999999f, 999, part, "oopsie");

                        break;
                    }
                }

                return;

                for (int i = 0; i < state.bodyParts.Count; i++)
                {
                    if (state.bodyParts[i] is Leg part && !IsDestroyed(part))
                    {
                        RimHealthState.Damage(self.State, state, new RimPoke(), 999999f, 999, part, "oopsie");
                    }
                }

                state.updateCapacities = true;

                return;

                if (state.wholeBodyAfflictions.Count == 0)
                {
                    state.wholeBodyAfflictions.Add(new RimFlu(self.State, null));
                }

                state.bloodLoss = 0.2f;

                self.injectedPoison = 0.2f;

                self.Hypothermia = 0.06f;

                self.airInLungs = 0.5f;

                state.updateCapacities = true;

                return;



                for (int i = 0; i < state.bodyParts.Count; i++)
                {
                    if (state.bodyParts[i] is Hand part && !IsDestroyed(part) && part.subName == "Left")
                    {
                        RimHealthState.Damage(self.State, state, new RimPoke(), 999999f, 999, part, "oopsie");

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

        int playerNumber = ((PlayerState)self.State).playerNumber;

        KeyCode keyCode = playerNumber switch
        {
            0 => ShadowOfOptions.player_1_key.Value,
            1 => ShadowOfOptions.player_2_key.Value,
            2 => ShadowOfOptions.player_3_key.Value,
            3 => ShadowOfOptions.player_4_key.Value,
            _ => throw new NotImplementedException()
        };

        if (Input.GetKey(keyCode) && healthTabs.Count >= playerNumber + 1 && healthTabs[playerNumber] != null)
        {
            if (buttonHeld[playerNumber] || !self.dead && self.Stunned)
            {
                return;
            }

            buttonHeld[playerNumber] = true;

            CreatureState healthTabCreatureState = self.State;
            RimState healthTabState = state;

            foreach (var grasp in self.grasps)
            {
                if (grasp != null && grasp.grabbedChunk != null && grasp.grabbedChunk.owner != null && grasp.grabbedChunk.owner is Creature crit && crit.State != null && healthState.TryGetValue(crit.State, out healthTabState))
                {
                    healthTabCreatureState = crit.State;
                    break;
                }
            }

            healthTabs[playerNumber].ToggleVisibility(healthTabCreatureState, healthTabState);
        }
        else
        {
            buttonHeld[playerNumber] = false;
        }
    }

    public static bool IsDestroyed(RimBodyPart self)
    {
        return self.afflictions.Count != 0 && self.afflictions[0] is RimDestroyed;
    }

    public static bool IsSubPartDestroyed(RimState state, RimBodyPart self)
    {
        if (IsDestroyed(self) && self.subPartOf != "")
        {
            foreach (RimBodyPart part in state.bodyParts)
            {
                if (IsSubPartOf(self, part))
                {
                    return part is not UpperTorso && (IsDestroyed(part) || part.isBionic);
                }
            }
        }

        return false;
    } //Checks is "self" is a subpart of anything and if it is destroyed. If "self" is destroyed but isn't a subpart or it's parent isn't destroyed this returns false

    public static bool IsSubPartOf(RimBodyPart self, RimBodyPart other)
    {
        if (self.parentSubName)
        {
            return (self.subName + self.subPartOf) == (other.subName + other.name);
        }
        else
        {
            return self.subPartOf == other.name;
        }
    } //Checks if "self" is a sub-part of "other". Meaning that "self" is under "other"

    public static bool LegCheck(RimState state)
    {
        if (state.consciousState != 0)
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

    public static int WorkingArms(RimState state)
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

        return workingArms;
    }

    #region Custom Damages
    public static void BombDamage(CreatureState self, RimState state, float damage, string attackName = "", string attackerName = "")
    {
        int amount = UnityEngine.Random.Range(1, 5);

        RimBodyPart focusedBodyPart;

        List<RimBodyPart> list = new();

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
                    if (!IsDestroyed(state.bodyParts[i]) && state.bodyParts[i].isInternal && IsSubPartOf(state.bodyParts[i], list[0]))
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

                RimBodyPart tempFocusedBodyPart = null;

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
                RimDamageType damageType = isSuper ? new RimSuperBomb() : new RimBomb();

                RimHealthState.Damage(self, state, damageType, damage / amount, 0.1f, focusedBodyPart, attackName, attackerName);
            }
        }
    }
    public static void BluntDamage(CreatureState self, RimState state, BodyChunk hitChunk, float damage, float AP, string attackName = "", string attackerName = "")
    {
        RimBodyPart focusedBodyPart = GetHitBodyPart(state, hitChunk, null, false, true);
        RimBodyPart secondaryFocusedBodyPart = 0.4f < UnityEngine.Random.value ? GetHitBodyPart(state, hitChunk, focusedBodyPart) : null;

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

                RimBodyPart GetHitAnotherBodyPart(RimBodyPart subPartOf)
                {
                    if (subPartOf == null)
                    {
                        return null;
                    }

                    List<RimBodyPart> list = new(1) { subPartOf };
                    RimBodyPart focusedBodyPart = null;

                    for (int i = 0; i < state.bodyParts.Count; i++)
                    {
                        if (IsDestroyed(state.bodyParts[i]) || subPartOf == state.bodyParts[i] || !IsSubPartOf(state.bodyParts[i], subPartOf) || state.bodyParts[i] is Eye)
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

        void Damage(RimBodyPart focusedBodyPart, float damage)
        {
            RimHealthState.Damage(self, state, new RimBlunt(), damage, AP, focusedBodyPart, attackName, attackerName);
        }
    }

    public static void CutDamage(CreatureState self, RimState state, BodyChunk hitChunk, float damage, float AP, string attackName = "", string attackerName = "")
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

        RimBodyPart focusedBodyPart = GetHitBodyPart(state, hitChunk);
        List<RimBodyPart> list = new(1) { focusedBodyPart };

        for (int i = 0; i < state.bodyParts.Count; i++)
        {
            if (IsDestroyed(state.bodyParts[i]) || list.Contains(state.bodyParts[i]))
            {
                continue;
            }
            else if (!IsSubPartOf(state.bodyParts[i], focusedBodyPart))
            {
                continue;
            }

            //Debug.Log("CutDamage focusedBodyParts subPart " + state.bodyParts[i] + " was added");
            list.Add(state.bodyParts[i]);
        } //Get all sub parts of the focusedBodypart

        RimBodyPart tempFocusedpart = focusedBodyPart;

        for (int i = 0; i < state.bodyParts.Count; i++)
        {
            if (IsDestroyed(state.bodyParts[i]) || list.Contains(state.bodyParts[i]))
            {
                continue;
            }
            else if (!IsSubPartOf(focusedBodyPart, state.bodyParts[i]))
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
                else if (!IsSubPartOf(state.bodyParts[i], focusedBodyPart))
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
                    else if (!IsSubPartOf(focusedBodyPart, state.bodyParts[i]))
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
                    else if (!IsSubPartOf(state.bodyParts[i], focusedBodyPart))
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

        void Damage(RimBodyPart focusedBodyPart, float damage)
        {
            RimHealthState.Damage(self, state, new RimCut(), damage, AP, focusedBodyPart, attackName, attackerName);
        }
    }

    public static void ScratchDamage(CreatureState self, RimState state, BodyChunk hitChunk, float damage, float AP, string attackName = "", string attackerName = "")
    {
        RimBodyPart focusedBodyPart = GetHitBodyPart(state, hitChunk);
        //Debug.Log("ScratchDamage focusedBodyPart is " + focusedBodyPart);

        List<RimBodyPart> list = new(1) { focusedBodyPart };

        for (int i = 0; i < state.bodyParts.Count; i++)
        {
            if (IsDestroyed(state.bodyParts[i]) || list.Contains(state.bodyParts[i]))
            {
                continue;
            }
            else if (!IsSubPartOf(state.bodyParts[i], focusedBodyPart))
            {
                continue;
            }

            //Debug.Log("CutDamage focusedBodyParts subPart " + state.bodyParts[i] + " was added");
            list.Add(state.bodyParts[i]);
        } //Get all sub parts of the focusedBodypart

        RimBodyPart tempFocusedpart = focusedBodyPart;

        for (int i = 0; i < state.bodyParts.Count; i++)
        {
            if (IsDestroyed(state.bodyParts[i]) || list.Contains(state.bodyParts[i]))
            {
                continue;
            }
            else if (!IsSubPartOf(focusedBodyPart, state.bodyParts[i]))
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
                else if (!IsSubPartOf(state.bodyParts[i], focusedBodyPart))
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

        void Damage(RimBodyPart focusedBodyPart, float damage)
        {
            RimHealthState.Damage(self, state, new RimScratch(), damage, AP, focusedBodyPart, attackName, attackerName);
        }
    }

    public static void CentipedeShockDamage(CreatureState self, RimState state, Centipede source, bool underwater, bool shockGiveUp)
    {
        RimBodyPart focusedBodyPart = GetHitBodyPart(state);
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

        RimHealthState.Damage(self, state, new RimElectricalBurn(), damage, AP, focusedBodyPart, attackName, attackerName);

        if (shockGiveUp && !self.dead)
        {
            source.Stun(6);
            source.shockGiveUpCounter = Math.Max(source.shockGiveUpCounter, 30);
            source.AI.annoyingCollisions = Math.Min(source.AI.annoyingCollisions * 2, 150);
        }
    }

    public static void LethalWaterDamage(CreatureState self, RimState state)
    {
        RimBodyPart focusedBodyPart = GetHitBodyPart(state);

        RimDamageType damageType;
        string attackName;

        if (self.creature.Room.realizedRoom.roomSettings.GetEffect(RoomSettings.RoomEffect.Type.LavaSurface) != null)
        {
            damageType = new RimFlame();
            attackName = "Lava";
        }
        else
        {
            damageType = new RimAcidBurn();
            attackName = "Acidic water";
        }

        float damage = 8 + self.creature.realizedCreature.lavaContactCount;

        RimHealthState.Damage(self, state, damageType, damage, 999, focusedBodyPart, attackName);
    }

    public static void LocustDamage(CreatureState self, RimState state)
    {
        RimHealthState.Damage(self, state, new RimBite(), 1, 0, GetHitBodyPart(state), "Locust swarm - Mandibles");
    } //make this add onto an already existing not-tended damage to reduce the amount of afflictions

    public static void TerrainImpactDamage(Player self, RimState state, int hitChunk, float speed, bool death)
    {
        RimBodyPart part = GetHitBodyPart(state, self.bodyChunks[hitChunk], null, false, true);

        part ??= GetHitBodyPart(state, null, null, false, true);

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

        RimHealthState.Damage(self.State, state, new RimBlunt(), damage, 0, part, "Terrain impact");
    }

    public static void TongueElectricDamage(CreatureState self, RimState state)
    {
        for (int i = 0; i < state.bodyParts.Count; i++)
        {
            if (state.bodyParts[i] is Tongue part && !IsDestroyed(part))
            {
                RimHealthState.Damage(self, state, new RimElectricalBurn(), 8, 999, part, "Zap-Coil");
                break;
            }
        }
    }

    public static void RainDamage(CreatureState self, RimState state, bool death)
    {
        float damage = death ? 8f : 0.5f;

        RimHealthState.Damage(self, state, new RimBlunt(), damage, 999, GetHitBodyPart(state, null, null, false, true), "Rain");
    } //if not death make this add onto an already existing not-tended damage to reduce the amount of afflictions
    public static void SandstormDamage(CreatureState self, RimState state, bool death)
    {
        float damage = death ? 8f : 0.5f;

        RimHealthState.Damage(self, state, new RimBlunt(), damage, 999, GetHitBodyPart(state, null, null, false, true), "Sandstorm");
    } //if not death make this add onto an already existing not-tended damage to reduce the amount of afflictions

    public static void SpiderDamage(CreatureState self, RimState state)
    {
        RimHealthState.Damage(self, state, new RimBite(), 1, 10, GetHitBodyPart(state), "Coalescipede - Fangs");
    } //make this add onto an already existing not-tended damage to reduce the amount of afflictions

    public static void ZapCoilDamage(CreatureState self, RimState state)
    {
        RimHealthState.Damage(self, state, new RimElectricalBurn(), 20, 999, GetHitBodyPart(state), "Zap-Coil");
    }
    public static void BigJellyfishDamage(CreatureState self, RimState state)
    {
        RimHealthState.Damage(self, state, new RimElectricalBurn(), 20, 999, GetHitBodyPart(state), "Big Jellyfish - Electricity");
    }

    public static void ARZapperDamage(CreatureState self, RimState state, bool death)
    {
        float damage = death ? 12f : 8f;

        RimHealthState.Damage(self, state, new RimElectricalBurn(), damage, 999, GetHitBodyPart(state), "Zapper");
    }
    #endregion

    public static RimBodyPart GetHitBodyPart(RimState state, BodyChunk hitChunk = null, RimBodyPart subPartOf = null, bool canHitInternal = false, bool isBlunt = false)
    {
        List<RimBodyPart> list = new();
        RimBodyPart focusedBodyPart = null;

        for (int i = 0; i < state.bodyParts.Count; i++)
        {
            if (IsDestroyed(state.bodyParts[i]) || (state.bodyParts[i].deathEffect == "" || state.bodyParts[i] is UpperTorso) && state.bodyParts[i].health <= 0)
            {
                continue;
            }
            else if (subPartOf == null)
            {
                if (hitChunk != null && !state.bodyParts[i].connectedBodyChunks.Contains(hitChunk.index) || hitChunk == null && state.bodyParts[i].connectedBodyChunks.Count == 0 || 
                    state.bodyParts[i].isInternal && (isBlunt || !canHitInternal) || 
                    isBlunt && state.bodyParts[i] is Eye)
                {
                    continue;
                }
            }
            else if (subPartOf != null && !IsSubPartOf(state.bodyParts[i], subPartOf))
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
            Debug.Log(all + "Error, hit bodypart is null");
            RimWorldHealth.Logger.LogInfo(all + "Error, hit bodypart is null");
        }

        return focusedBodyPart;
    }

    public static float BombDamageMultiplier(bool isSuper)
    {
        return isSuper ? 55f : 27.5f;
    }

    public static void SetToxicBuildup(RimState state, Creature self)
    {
        RimToxicBuildup toxicBuildup = null;

        for (int i = 0; i < state.wholeBodyAfflictions.Count; i++)
        {
            if (state.wholeBodyAfflictions[i] is RimToxicBuildup tempToxicBuildup)
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
                toxicBuildup.amount = Math.Min(1, poison);
            }
            else
            {
                state.wholeBodyAfflictions.Add(new RimToxicBuildup(self.State, null, Math.Min(1, poison)));
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

        switch (self)
        {
            case BigNeedleWorm:
                name = "Adult Noodlefly";
                break;
            case BigSpider bigSpider:
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
                break;
            case DropBug:
                name = "Dropwig";
                break;
            case EggBug:
                name = "Firebug";
                break;
            case JetFish:
                name = "Jetfish";
                break;
            case Vulture vulture:
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
                break;
            case Leech leech:
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
                break;
            case Lizard:
                name = "Lizard";
                break; //Will add all non-modded Lizards later
            case MirosBird:
                name = "Miros Bird";
                break;
            case Player:
                name = ((PlayerState)self.State).isPup ? "Slugpup" : "Slugcat";

                if (!singleplayerHud && (!ModManager.MSC || self.State is not MoreSlugcats.PlayerNPCState))
                {
                    name = "Player " + (((PlayerState)self.State).playerNumber + 1);
                }
                break;
            case SkyWhale:
                name = "Sky Whale";
                break;
            case Scavenger scavenger:
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
                break;
            case MoreSlugcats.BigJellyFish:
                name = "Big Jellyfish";
                break;
            case MoreSlugcats.StowawayBug:
                name = "Stowaway";
                break;
            case Watcher.BoxWorm:
                name = "Box Worm";
                break;
            case Watcher.DrillCrab:
                name = "Drill Crab";
                break;
            case Watcher.Frog:
                name = "Frog";
                break;
            case Watcher.Loach:
                name = "Loach";
                break;
            case Watcher.Rat:
                name = "Rat";
                break;
            case Watcher.RippleSpider:
                name = "Ripple Spider";
                break;
            default:
                name = self.ToString();
                Debug.Log(all + self + " is a unknown creature");
                Logger.LogError(all + self + " is a unknown creature");
                break;
        }

        return name;
    }

    public static void KarmaFlowerHeal(Creature self, RimState state)
    {
        #region Tier 1 (Missing important BodyParts)
        for (int i = 0; i < state.bodyParts.Count; i++)
        {
            if (state.bodyParts[i] is LowerTorso lowerTorso)
            {
                if (lowerTorso.afflictions.Count == 1 && lowerTorso.afflictions[0] is RimDestroyed)
                {
                    lowerTorso.afflictions.Clear();
                    return;
                }

                break;
            }
        }

        Dictionary<string, List<RimBodyPart>> limbMissingBodyparts = new();

        for (int i = 0; i < state.legSetNames.Count; i++)
        {
            limbMissingBodyparts = state.legSet[state.legSetNames[i]].KarmaFlowerGetMainMissingPart(state.legSetNames[i], limbMissingBodyparts);
        }

        if (limbMissingBodyparts.Count > 0)
        {
            Dictionary<string, RimBodyPart> mainBodyparts = new();

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
                        foreach (RimBodyPart part in dic.Value)
                        {
                            if (part is RimBone)
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
            Dictionary<string, RimBodyPart> mainBodyparts = new();

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
                            foreach (RimBodyPart part in dic.Value)
                            {
                                if (part is RimBone)
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
                if (spine.afflictions.Count == 1 && spine.afflictions[0] is RimDestroyed)
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

        RimDisease diseaseToHeal = null;

        foreach (RimAffliction affliction in state.wholeBodyAfflictions)
        {
            if (affliction is RimDisease disease && (diseaseToHeal == null || disease.severity > diseaseToHeal.severity))
            {
                diseaseToHeal = disease;
            }
        }

        if (diseaseToHeal == null)
        {
            foreach (RimBodyPart part in state.bodyParts)
            {
                foreach (RimAffliction affliction in part.afflictions)
                {
                    if (affliction is RimDisease disease && (diseaseToHeal == null || disease.severity > diseaseToHeal.severity))
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
        RimScar scarToHeal = null;

        foreach (RimAffliction affliction in state.consciousnessSource.afflictions)
        {
            if (affliction is RimScar scar && (scar.isRevealed || scar.isPermanent))
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
        List<RimBodyPart> facialParts = new();
        List<RimBodyPart> fingersAndToes = new();

        foreach (RimBodyPart part in state.bodyParts)
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
            if (lung.afflictions.Count == 1 && lung.afflictions[0] is RimDestroyed)
            {
                lung.afflictions.Clear();

                return;
            }
        }

        foreach (Kidney kidney in kidneys)
        {
            if (kidney.afflictions.Count == 1 && kidney.afflictions[0] is RimDestroyed)
            {
                kidney.afflictions.Clear();

                return;
            }
        }

        foreach (RimBodyPart part in facialParts)
        {
            if (part.afflictions.Count == 1 && part.afflictions[0] is RimDestroyed)
            {
                part.afflictions.Clear();

                return;
            }
        }

        foreach (RimBodyPart part in fingersAndToes)
        {
            if (part.afflictions.Count == 1 && part.afflictions[0] is RimDestroyed)
            {
                part.afflictions.Clear();

                return;
            }
        }
        #endregion

        //Tier 6 Ailments in other parts (currently there are none, this is where the eyes and ears list would be used)

        //Tier 7 Drug addictions (there might never be any)

        #region Tier 8 Scars on other parts
        foreach (RimBodyPart part in state.bodyParts)
        {
            foreach (RimAffliction affliction in part.afflictions)
            {
                if (affliction is RimScar scar && (scar.isRevealed || scar.isPermanent))
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
        RimInjury injuryToHeal = null;

        foreach (RimBodyPart part in state.bodyParts)
        {
            foreach (RimAffliction affliction in part.afflictions)
            {
                if (affliction is RimInjury injury && injury is not RimDestroyed && (affliction is not RimScar || affliction is RimScar scar && !scar.isRevealed && !scar.isPermanent))
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

    public static bool UpdateAfflictions(CreatureState self, RimState state, int ticksPassed)
    {
        state.timeAbstracted = cycleTick;

        bool keepUpdating = false;

        List<RimDisease> diseasesToTend = new();
        List<RimInjury> injuriesToTend = new();

        try
        {
            if (state.onFire)
            {
                int firePatTimesMax = ticksPassed / 10;
                int firePatTimes = 0;

                if (!self.dead)
                {
                    while (state.fireRate <= 0 && firePatTimes < firePatTimesMax)
                    {
                        state.fireSize -= 0.1f * (ticksPassed / 10);
                        firePatTimes++;
                    }

                    if (state.fireSize <= 0)
                    {
                        state.onFire = false;
                        state.fireSize = 0;
                    }
                }

                state.fireRate -= state.onFire ? ticksPassed : firePatTimes * 10;

                while (state.fireRate <= 0)
                {
                    state.fireRate += 40;

                    RimHealthState.Damage(self, state, new RimBurn(), 2, 0, GetHitBodyPart(state));
                }

                ticksPassed -= firePatTimes * 10;
            }

            #region Tend
            if (!self.dead && (state.consciousState == 0 || (ShadowOfOptions.downed_tend.Value == "Everyone" && state.consciousState != 3)))
            {
                state.tendTime -= ticksPassed;

                int timesTended = 0;

                while (state.tendTime < 0)
                {
                    Debug.Log("tend time was " + state.tendTime);
                    state.tendTime += Mathf.RoundToInt(state.tendTimeBase / RimHealthState.MedicalTendSpeed(state));
                    Debug.Log("new tend time " + state.tendTime);
                    timesTended++;
                }

                Debug.Log("tending times " + timesTended);

                for (int i = 0; i < timesTended; i++)
                {
                    RimInjury bleeding = null;
                    RimDisease diseaseAffliction = null;
                    RimInjury untendedAffliction = null;

                    foreach (RimBodyPart part in state.bodyParts)
                    {
                        foreach (RimAffliction baseAffliction in part.afflictions)
                        {
                            if (baseAffliction is not RimTendableAffliction affliction)
                            {
                                continue;
                            }

                            if (!affliction.isTended)
                            {
                                if (affliction is RimInjury injury)
                                {
                                    if (injury.isBleeding)
                                    {
                                        if (bleeding != null)
                                        {
                                            if (injury.healingDifficulty.bleeding * injury.damage > bleeding.healingDifficulty.bleeding * bleeding.damage)
                                            {
                                                bleeding = injury;
                                            }
                                        }
                                        else bleeding ??= injury;
                                    }
                                    else
                                    {
                                        if (untendedAffliction != null)
                                        {
                                            if (injury.damage > untendedAffliction.damage)
                                            {
                                                untendedAffliction = injury;
                                            }
                                        }
                                        else untendedAffliction ??= injury;

                                    }
                                }
                                else if (affliction is RimDisease disease && disease.timeUntilTreatment <= 0)
                                {
                                    if (diseaseAffliction != null)
                                    {
                                        if (disease.severity > diseaseAffliction.severity)
                                        {
                                            diseaseAffliction = disease;
                                        }
                                    }
                                    else diseaseAffliction ??= disease;
                                }
                                else
                                {
                                    Debug.Log(all + affliction + " does not belong to any tendable check");
                                    Logger.LogError(all + affliction + " does not belong to any tendable check");
                                }
                            }
                            else if (affliction is RimDisease disease && disease.timeUntilTreatment <= 0)
                            {
                                if (diseaseAffliction != null)
                                {
                                    if (disease.severity > diseaseAffliction.severity)
                                    {
                                        diseaseAffliction = disease;
                                    }
                                }
                                else diseaseAffliction ??= disease;
                            }
                        }
                    }

                    if (bleeding == null)
                    {
                        foreach (RimAffliction affliction in state.wholeBodyAfflictions)
                        {
                            if (affliction is not RimDisease disease || disease.timeUntilTreatment > 0)
                            {
                                continue;
                            }

                            if (diseaseAffliction != null)
                            {
                                if (disease.severity > diseaseAffliction.severity)
                                {
                                    diseaseAffliction = disease;
                                }
                            }
                            else diseaseAffliction ??= disease;
                        }
                    }

                    if (bleeding != null)
                    {
                        keepUpdating = true;
                        tend(bleeding);
                    }
                    else if (diseaseAffliction != null)
                    {
                        keepUpdating = true;
                        tend(diseaseAffliction);
                    }
                    else if (untendedAffliction != null)
                    {
                        keepUpdating = true;
                        tend(untendedAffliction);
                    }
                    else
                    {
                        keepUpdating = false;
                        timesTended = 0;
                        break;
                    }

                    void tend(RimTendableAffliction tendAffliction)
                    {
                        tendAffliction.isTended = true;

                        RimHealthState.TendAffliction(state, self, state.tendAffliction.state);

                        if (tendAffliction is RimInjury injury)
                        {
                            injury.isBleeding = false;

                            if (injury is RimDestroyed destroyed)
                            {
                                destroyed.isFresh = false;
                            }
                        }
                        else if (tendAffliction is RimDisease disease)
                        {
                            disease.timeUntilTreatment = cycleLength * disease.treatmentTimes;
                            disease.totalTendQuality += disease.tendQuality;
                        }
                    }
                }
            }
            #endregion

            state.consciousState = 0;

            #region WholeBody
            foreach (RimAffliction affliction in state.wholeBodyAfflictions)
            {
                if (affliction is RimDisease disease)
                {
                    keepUpdating = true;
                    diseasesToTend.Add(disease);
                }
            }
            #endregion

            foreach (RimBodyPart part in state.bodyParts)
            {
                foreach (RimAffliction baseAffliction in part.afflictions)
                {
                    if (baseAffliction is not RimTendableAffliction affliction || affliction.isCharacterSpecific)
                    {
                        continue;
                    }

                    if (affliction is RimInjury injury)
                    {
                        if (affliction is RimDestroyed || affliction is RimScar scar && (scar.isRevealed || scar.isPermanent))
                        {
                            continue;
                        }

                        injuriesToTend.Add(injury);
                    }
                    else if (affliction is RimDisease disease)
                    {
                        diseasesToTend.Add(disease);
                    }
                }
            }

            if (injuriesToTend.Count > 0)
            {
                keepUpdating = true;
            }

            UpdateInjuries(injuriesToTend, state);

            state.bloodLossPerCycle = 0;

            foreach (RimBodyPart part in state.bodyParts)
            {
                if (IsSubPartDestroyed(state, part))
                {
                    continue;
                }

                List<RimAffliction> afflictionList = new(part.afflictions);
                foreach (RimAffliction baseAffliction in afflictionList)
                {
                    if (baseAffliction is not RimTendableAffliction affliction)
                    {
                        continue;
                    }

                    if (affliction is RimDestroyed destroyed)
                    {
                        part.health = 0;
                        part.efficiency = 0;

                        if (!destroyed.isTended)
                        {
                            state.bloodLossPerCycle += destroyed.isBleeding ? destroyed.healingDifficulty.bleeding * destroyed.part.maxHealth * 2 * state.bodySizeFactor * RimHealthState.BloodLossMultiplier(part) : 0;

                            destroyed.pain = destroyed.part.maxHealth * 2 * destroyed.healingDifficulty.pain / state.bodySizeFactor / 100;
                        }

                        break;
                    }

                    if (self.dead || affliction is not RimInjury injury)
                    {
                        continue;
                    }

                    if (injury is RimScar scar && scar.isRevealed)
                    {
                        if (scar.painCategory == "painful")
                        {
                            scar.pain = scar.scarDamage * 1.5f * injury.healingDifficulty.scarPain / state.bodySizeFactor / 100;
                        }
                        else if (scar.painCategory == "aching")
                        {
                            scar.pain = scar.scarDamage * injury.healingDifficulty.scarPain / state.bodySizeFactor / 100;
                        }
                        else if (scar.painCategory == "itchy")
                        {
                            scar.pain = scar.scarDamage * 0.5f * injury.healingDifficulty.scarPain / state.bodySizeFactor / 100;
                        }
                    }
                    else
                    {
                        injury.pain = injury.damage * injury.healingDifficulty.pain / state.bodySizeFactor / 100;
                    }

                    state.pain += affliction.pain;

                    float bloodLoss = injury.isBleeding && !injury.isTended ? injury.healingDifficulty.bleeding * injury.damage * state.bodySizeFactor * RimHealthState.BloodLossMultiplier(part) : 0;

                    foreach (int bodyChunk in injury.part.connectedBodyChunks)
                    {
                        state.visualBleedAmount[bodyChunk][0] += bloodLoss / injury.part.connectedBodyChunks.Count;
                    }

                    state.bloodLossPerCycle += bloodLoss;
                }
            }

            if (state.bloodLossPerCycle > 0)
            {
                state.bloodLoss += state.bloodLossPerCycle / 100 / CycleLength() * (ticksPassed / 40);
            }

            state.bloodLoss = Mathf.Clamp(state.bloodLoss, 0, 1);

            if (state.bloodLoss == 1)
            {
                RimHealthState.Kill(self);

                return false;
            }
            else if (state.bloodLoss > 0)
            {
                keepUpdating = true;
            }

            foreach (RimDisease disease in diseasesToTend)
            {
                SavingandLoadingHooks.UpdateDisease(disease, state, self, new(), false, ticksPassed);
            }

            RimHealthState.UpdateCapacities(state, self);

            state.updateCapacities = true;
        }
        catch (Exception e) { Logger.LogError(e); }

        return keepUpdating;

        void UpdateInjuries(List<RimInjury> healList, RimState state)
        {
            state.healingRate -= ticksPassed;
            int treatmentTime = 0;

            while (treatmentTime < 0)
            {
                state.healingRate += state.healingRateTics;

                treatmentTime++;
            }

            for (int i = 0; i < treatmentTime; i++)
            {
                if (healList.Count <= 0)
                {
                    return;
                }

                RimInjury injury = healList[UnityEngine.Random.Range(0, healList.Count)];

                float healRate = 8;

                if (injury.isTended)
                {
                    healRate += 4;

                    healRate += Mathf.Round(injury.tendQuality) * 0.08f;
                }

                injury.damage -= healRate * 0.1f;

                if (injury is RimScar scar)
                {
                    if (scar.damage <= scar.scarDamage)
                    {
                        scar.damage = scar.scarDamage;
                        scar.isTended = true;
                        scar.isBleeding = false;
                        scar.isRevealed = true;

                        healList.Remove(injury);
                    }
                }
                else if (injury.damage <= 0)
                {
                    healList.Remove(injury);

                    injury.RemoveSelf();
                }
            }
        }
        
        /*
        void UpdateCapacities()
        {
            state.capacityAffectingAffliction.Clear();

            float painFactor = 1f;

            float maxConsciousness = -1;

            if (!self.dead)
            {
                state.bloodLossPerCycle = 0;
                state.pain = 0;
                state.consciousness = 0;
                state.moving = 0;
                state.manipulation = 0;
                state.sight = 0;
                state.hearing = 0;
                state.breathing = 0;
                state.bloodFiltration = 0;
                state.bloodPumping = 0;

                foreach (List<float> floats in state.visualBleedAmount)
                {
                    floats[0] = 0;
                }
            }

            List<RWAffliction> afflictionList;

            float totalDamage = 0;

            foreach (RWBodyPart part in state.bodyParts)
            {
                part.health = part.maxHealth;
                if (part.afflictions.Count == 0)
                {
                    goto line1;
                }
                else if (IsSubPartDestroyed(state, part))
                {
                    part.health = 0;
                    part.efficiency = 0;

                    goto line1;
                }

                afflictionList = new(part.afflictions);
                foreach (RWAffliction baseAffliction in afflictionList)
                {
                    if (baseAffliction is not RWTendableAffliction affliction)
                    {
                        continue;
                    }

                    if (affliction is RWDestroyed destroyed)
                    {
                        part.health = 0;
                        part.efficiency = 0;

                        if (!destroyed.isTended)
                        {
                            state.bloodLossPerCycle += destroyed.isBleeding ? destroyed.healingDifficulty.bleeding * destroyed.part.maxHealth * 2 * state.bodySizeFactor * RWHealthState.BloodLossMultiplier(part) : 0;

                            destroyed.pain = destroyed.part.maxHealth * 2 * destroyed.healingDifficulty.pain / state.bodySizeFactor / 100;
                        }

                        break;
                    }

                    if (affliction is not RWInjury injury)
                    {
                        if (!self.dead && affliction is RWDisease disease)
                        {
                            Disease(disease);
                        }

                        continue;
                    }

                    part.health -= injury.damage;

                    totalDamage += injury.damage;

                    if (self.dead)
                    {
                        continue;
                    }

                    if (injury is RWScar scar && scar.isRevealed)
                    {
                        if (scar.painCategory == "painful")
                        {
                            scar.pain = scar.scarDamage * 1.5f * injury.healingDifficulty.scarPain / state.bodySizeFactor / 100;
                        }
                        else if (scar.painCategory == "aching")
                        {
                            scar.pain = scar.scarDamage * injury.healingDifficulty.scarPain / state.bodySizeFactor / 100;
                        }
                        else if (scar.painCategory == "itchy")
                        {
                            scar.pain = scar.scarDamage * 0.5f * injury.healingDifficulty.scarPain / state.bodySizeFactor / 100;
                        }
                    }
                    else
                    {
                        injury.pain = injury.damage * injury.healingDifficulty.pain / state.bodySizeFactor / 100;
                    }

                    state.pain += affliction.pain;

                    float bloodLoss = injury.isBleeding && !injury.isTended ? injury.healingDifficulty.bleeding * injury.damage * state.bodySizeFactor * RWHealthState.BloodLossMultiplier(part) : 0;

                    foreach (int bodyChunk in injury.part.connectedBodyChunks)
                    {
                        state.visualBleedAmount[bodyChunk][0] += bloodLoss / injury.part.connectedBodyChunks.Count;
                    }

                    state.bloodLossPerCycle += bloodLoss;
                }

            line1:

                part.efficiency = Mathf.Max(0, part.health / part.maxHealth);

                if (part.health > 0 && part.health < 1 || part.deathEffect == "" && part.health < 1)
                {
                    part.health = 1;
                    part.efficiency = 0;
                }
                else if (part is UpperTorso && part.health <= 0)
                {
                    part.health = 0;
                    part.efficiency = 0;
                }
            }

            if (totalDamage > 150 * state.healthScale)
            {
                RWHealthState.Kill(self);

                if (state.onFire)
                {
                    RWHealthState.DestroyBurn(self);
                }
                return;
            }

            if (self is HealthState healthState)
            {
                healthState.health = state.consciousnessSource.efficiency;
            }
            else if (self is PlayerState playerState)
            {
                playerState.permanentDamageTracking = Mathf.Max(0, 1 - state.consciousnessSource.efficiency);
            }

            afflictionList = new(state.wholeBodyAfflictions);

            foreach (RWAffliction affliction in afflictionList)
            {
                if (affliction is RWDisease disease)
                {
                    Disease(disease);
                }
                else if (affliction is RWAilment informational)
                {
                    Ailment(informational);
                }
            }

            state.pain = Mathf.Clamp(state.pain, 0, 1);

            if (state.pain > state.painShockThreshold && state.consciousState < 2)
            {
                state.consciousState = 2;
            }

            if (state.bloodFiltrationBP.Count > 0)
            {
                float baseEfficiency = 1;
                float offsets = state.bloodFiltration;
                float postFactors = 1;

                foreach (RWBodyPart part in state.bloodFiltrationBP)
                {
                    baseEfficiency *= 1 - part.capacityImportance[part.capacity.IndexOf("Blood Filtration")] * (1 - part.efficiency);
                }

                state.bloodFiltration = Mathf.Max(0, (baseEfficiency + offsets) * postFactors);
            }
            else
            {
                state.bloodFiltration = 0;
            }

            if (state.bloodPumpingBP.Count > 0)
            {
                float baseEfficiency = 1;
                float offsets = state.bloodPumping;
                float postFactors = 1;

                foreach (RWBodyPart part in state.bloodPumpingBP)
                {
                    baseEfficiency *= 1 - part.capacityImportance[part.capacity.IndexOf("Blood Pumping")] * (1 - part.efficiency);
                }

                state.bloodPumping = Mathf.Max(0, (baseEfficiency + offsets) * postFactors);
            }
            else
            {
                state.bloodPumping = 0;
            }

            if (state.breathingBP.Count > 0)
            {
                float baseEfficiency = 1;
                float offsets = state.breathing;
                float postFactors = 1;

                foreach (RWBodyPart part in state.breathingBP)
                {
                    baseEfficiency *= 1 - part.capacityImportance[part.capacity.IndexOf("Breathing")] * (1 - part.efficiency);
                }

                state.breathing = Mathf.Max(0, (baseEfficiency + offsets) * postFactors);
            }
            else
            {
                state.breathing = 0;
            }

            float consciounessOffset = state.consciousness;

            state.consciousness = ((state.consciousnessSource == null ? 1 : state.consciousnessSource.efficiency) * (1 - Mathf.Clamp((state.pain - 0.1f) * 4 / 9, 0, 0.4f)) * Mathf.Min(1, 1 - 0.2f * (1 - state.bloodPumping)) * Mathf.Min(1, 1 - 0.2f * (1 - state.breathing)) * Mathf.Min(1, 1 - 0.1f * (1 - state.bloodFiltration))) + consciounessOffset;

            if (state.bloodLoss >= 0.6f)
            {
                state.consciousness -= 0.4f;

                maxConsciousness = 0.1f;
            }
            else if (state.bloodLoss >= 0.45f)
            {
                state.consciousness -= 0.4f;
            }
            else if (state.bloodLoss >= 0.3f)
            {
                state.consciousness -= 0.2f;
            }
            else if (state.bloodLoss >= 0.15f)
            {
                state.consciousness -= 0.1f;
            }

            if (maxConsciousness != -1)
            {
                state.consciousness = Mathf.Min(state.consciousness, maxConsciousness);
            }

            state.consciousness = Mathf.Max(state.consciousness, 0);

            if (state.consciousness <= 0 || state.bloodFiltration <= 0 || state.bloodPumping <= 0 || state.breathing <= 0)
            {
                RWHealthState.Kill(self);
                return;
            }
            else if (state.consciousness < 0.3f)
            {
                state.consciousState = 3;
            }

            if (state.hearingBP.Count > 0)
            {
                float baseEfficiency = 0;
                float offsets = state.hearing;
                float postFactors = 1;

                if (state.hearingBP.Count == 1)
                {
                    baseEfficiency = state.hearingBP[0].efficiency;

                    state.hearing = (baseEfficiency + offsets) * postFactors;
                }
                else
                {
                    float bestEfficiency = 0;

                    foreach (RWBodyPart part in state.hearingBP)
                    {
                        baseEfficiency += part.efficiency / (state.hearingBP.Count * 2);

                        if (part.efficiency > bestEfficiency)
                        {
                            bestEfficiency = part.efficiency;
                        }
                    }

                    baseEfficiency += bestEfficiency / 2;

                    state.hearing = Mathf.Max(0, (baseEfficiency + offsets) * postFactors);
                }
            }
            else
            {
                state.hearing = 0;
            }

            if ((state.armSetNames.Count + state.manipulationBP.Count) > 0)
            {
                float baseEfficiency = 0;
                float offsets = state.manipulation;
                float postFactors = 1;
                float otherEfficiency = 1;

                foreach (RWBodyPart part in state.manipulationBP)
                {
                    otherEfficiency *= 1 - part.capacityImportance[part.capacity.IndexOf("Manipulation")] * (1 - part.efficiency);
                }

                foreach (string setName in state.armSetNames)
                {
                    baseEfficiency += state.armSet[setName].Efficiency(state, offsets / state.armSetNames.Count, postFactors, otherEfficiency) / state.armSetNames.Count;
                }

                state.manipulation = Mathf.Max(0, baseEfficiency);
            }
            else
            {
                state.manipulation = 0;
            }

            if ((state.legSetNames.Count + state.movingBP.Count) > 0)
            {
                float baseEfficiency = 0;
                float offsets = state.moving;
                float postFactors = 1;
                float otherEfficiency = 1;

                foreach (RWBodyPart part in state.movingBP)
                {
                    otherEfficiency *= 1 - part.capacityImportance[part.capacity.IndexOf("Moving")] * (1 - part.efficiency);
                }

                foreach (string setName in state.legSetNames)
                {
                    baseEfficiency += state.legSet[setName].Efficiency(state, offsets / state.legSetNames.Count, postFactors, otherEfficiency) / state.legSetNames.Count;
                }

                state.moving = Mathf.Max(0, baseEfficiency);
            }
            else
            {
                state.moving = 0;
            }

            if (state.moving <= 0.15f && state.consciousState < 1)
            {
                state.consciousState = 1;
            }

            if (state.sightBP.Count > 0)
            {
                float baseEfficiency = 0;
                float offsets = state.sight;
                float postFactors = 1;

                if (state.sightBP.Count == 1)
                {
                    baseEfficiency = state.sightBP[0].efficiency;

                    state.sight = (baseEfficiency + offsets) * postFactors;
                }
                else
                {
                    float bestEfficiency = 0;

                    foreach (RWBodyPart part in state.sightBP)
                    {
                        baseEfficiency += part.efficiency / (state.sightBP.Count * 2);

                        if (part.efficiency > bestEfficiency)
                        {
                            bestEfficiency = part.efficiency;
                        }
                    }

                    baseEfficiency += bestEfficiency / 2;

                    state.sight = Mathf.Max(0, (baseEfficiency + offsets) * postFactors);
                }
            }
            else
            {
                state.sight = 0;
            }

            void Disease(RWDisease disease)
            {
                if (disease is RWFlu)
                {
                    state.capacityAffectingAffliction.Add(disease);

                    switch (disease.severity)
                    {
                        case <= 0.665f:
                            state.consciousness -= 0.05f;
                            state.manipulation -= 0.05f;
                            state.breathing -= 0.1f;
                            break;
                        case <= 0.832f:
                            state.consciousness -= 0.1f;
                            state.manipulation -= 0.1f;
                            state.breathing -= 0.15f;
                            break;
                        default:
                            state.pain += 0.05f;

                            state.consciousness -= 0.15f;
                            state.manipulation -= 0.2f;
                            state.breathing -= 0.2f;
                            break;
                    }
                }
                else if (disease is RWInfection)
                {
                    switch (disease.severity)
                    {
                        case <= 0.32f:
                            state.pain += 0.05f;
                            break;
                        case <= 0.77f:
                            state.pain += 0.08f;
                            break;
                        case <= 0.86f:
                            state.pain += 0.12f;

                            state.consciousness -= 0.5f;

                            state.capacityAffectingAffliction.Add(disease);
                            break;
                        default:
                            maxConsciousness = 0.1f;

                            state.pain += 0.85f;

                            state.breathing -= 0.5f;

                            state.capacityAffectingAffliction.Add(disease);
                            break;
                    }
                }
            }

            void Ailment(RWAilment ailment)
            {
                if (ailment is RWHypothermia)
                {
                    state.capacityAffectingAffliction.Add(ailment);

                    switch (ailment.amount)
                    {
                        case <= 0.2f:
                            state.consciousness -= 0.05f;
                            state.manipulation -= 0.08f;
                            break;
                        case <= 0.35f:
                            state.consciousness -= 0.1f;
                            state.manipulation -= 0.2f;
                            state.moving -= 0.1f;
                            break;
                        case <= 0.62f:
                            state.consciousness -= 0.2f;
                            state.manipulation -= 0.5f;
                            state.moving -= 0.3f;

                            state.pain += 0.15f;
                            break;
                        default:
                            maxConsciousness = 0.1f;

                            state.pain += 0.3f;
                            break;
                    }
                }
                else if (ailment is RWToxicBuildup)
                {
                    state.capacityAffectingAffliction.Add(ailment);

                    switch (ailment.amount)
                    {
                        case <= 0.2f:
                            state.consciousness -= 0.05f;
                            break;
                        case <= 0.4f:
                            state.consciousness -= 0.1f;
                            break;
                        case <= 0.6f:
                            state.consciousness -= 0.15f;
                            break;
                        case <= 0.8f:
                            state.consciousness -= 0.25f;
                            break;
                        default:
                            maxConsciousness = 0.1f;

                            state.consciousness -= 0.25f;
                            break;
                    }
                }
                else if (ailment is RWAnesthetic)
                {
                    state.capacityAffectingAffliction.Add(ailment);

                    switch (ailment.amount)
                    {
                        case <= 0.6f:
                            painFactor *= 0.95f;

                            maxConsciousness = 0.9f;

                            state.manipulation -= 0.1f;
                            state.moving -= 0.05f;
                            break;
                        case <= 0.8f:
                            painFactor *= 0.8f;

                            maxConsciousness = 0.7f;

                            state.manipulation -= 0.2f;
                            state.moving -= 0.2f;
                            state.talking -= 0.2f;
                            state.digestion -= 0.2f;
                            state.sight -= 0.15f;
                            break;
                        default:
                            painFactor *= 0f;

                            maxConsciousness = 0.01f;
                            break;
                    }
                }
            }
        }
        */
    }

    public static float CycleLength()
    {
        return 40 * 60 * cycleLength;
    }
}

public class RimBloodDrip(Vector2 pos, Vector2 vel, bool waterColor) : WaterDrip(pos, vel, waterColor)
{
    public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        colors = new Color[]
        {
            new Color(1f, 0f, 0f),
            new Color(1f, 0f, 0f),
            new Color(1f, 0f, 0f)
        };
    }
}

public class RimDiseaseCloud : SporeCloud
{
    public RimDiseaseCloud(Vector2 pos, Vector2 vel, Color color, float size, AbstractCreature killTag, int checkInsectsDelay, InsectCoordinator smallInsects, int rippleLayer) : base(pos, vel, color, size, killTag, checkInsectsDelay, smallInsects, rippleLayer)
    {
        rad = Mathf.Lerp(0.2f, 0.6f, UnityEngine.Random.value) * size;
        lifeTime = UnityEngine.Random.value * 20f + 25f;
        this.pos = pos;
        getToPos = pos + vel;
    }
}

public class RimFireSmoke(Room room, RimWorldHealth.RimState state, Creature creature) : Smoke.FireSmoke(room)
{
    public override SmokeSystemParticle CreateParticle()
    {
        return new RimFireSmokeParticle();
    }

    public void RimEmitSmoke()
    {
        if (creature == null || state == null || !state.onFire)
        {
            return;
        }

        Vector2 pos = creature.mainBodyChunk.pos + new Vector2(UnityEngine.Random.Range(-creature.mainBodyChunk.rad, creature.mainBodyChunk.rad), UnityEngine.Random.Range(-creature.mainBodyChunk.rad, creature.mainBodyChunk.rad));

        if (AddParticle(pos, Custom.RNV(), Mathf.Lerp(10f, 40f, UnityEngine.Random.value)) is RimFireSmokeParticle fireSmokeParticle)
        {
            fireSmokeParticle.effectColor = Color.yellow;
            fireSmokeParticle.colorFadeTime = 25;
            fireSmokeParticle.state = state;
            fireSmokeParticle.rad = Mathf.Lerp(Custom.LerpMap(state.fireSize, 0, 2.5f, 16f, 42f), Custom.LerpMap(state.fireSize, 0, 2.5f, 34f, 62f), UnityEngine.Random.value);
        }
    }

    public class RimFireSmokeParticle : FireSmokeParticle
    {
        public override void Reset(Smoke.SmokeSystem newOwner, Vector2 pos, Vector2 vel, float lifeTime)
        {
            base.Reset(newOwner, pos, vel, lifeTime);
            col = 0f;
            lastCol = 0f;
            rad = Mathf.Lerp(28f, 46f, UnityEngine.Random.value);
            moveDir = 0;
        }

        public override void Update(bool eu)
        {
            base.Update(eu);
            if (resting)
            {
                return;
            }

            moveDir = 0;

            vel *= Custom.LerpMap(state.fireSize, 0, 2.5f, 0.5f, 1f);

            //vel *= 0.7f + 0.3f / Mathf.Pow(vel.magnitude, 0.5f);
            //moveDir += Mathf.Lerp(-0.05f, 0.05f, UnityEngine.Random.value) * 10f;
            //vel += Custom.DegToVec(0) * (0.6f * Mathf.Lerp(vel.magnitude, 1f, 0.6f));
            if (room.PointSubmerged(pos))
            {
                pos.y = room.FloatWaterLevel(pos);
            }
            lastCol = col;
            col += 1f;
            if (room.GetTile(pos).Solid && !room.GetTile(lastPos).Solid)
            {
                IntVector2? intVector = SharedPhysics.RayTraceTilesForTerrainReturnFirstSolid(room, room.GetTilePosition(lastPos), room.GetTilePosition(pos));
                FloatRect floatRect = Custom.RectCollision(pos, lastPos, room.TileRect(intVector.Value).Grow(2f));
                pos = floatRect.GetCorner(FloatRect.CornerLabel.D);
                if (floatRect.GetCorner(FloatRect.CornerLabel.B).x < 0f)
                {
                    vel.x = Mathf.Abs(vel.x);
                    return;
                }
                if (floatRect.GetCorner(FloatRect.CornerLabel.B).x > 0f)
                {
                    vel.x = -Mathf.Abs(vel.x);
                    return;
                }
                if (floatRect.GetCorner(FloatRect.CornerLabel.B).y < 0f)
                {
                    vel.y = Mathf.Abs(vel.y);
                    return;
                }
                if (floatRect.GetCorner(FloatRect.CornerLabel.B).y > 0f)
                {
                    vel.y = -Mathf.Abs(vel.y);
                }
            }
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
            if (resting)
            {
                return;
            }
            float currentLife = Mathf.Lerp(lastLife, life, timeStacker);
            float t = Mathf.InverseLerp((float)colorFadeTime, 0.5f, Mathf.Lerp(lastCol, col, timeStacker));

            if (currentLife < 0.3f)
            {
                sLeaser.sprites[0].color = colorB;
                sLeaser.sprites[1].color = colorB;
            }
            else
            {
                sLeaser.sprites[0].color = Color.Lerp(colorA, effectColor, t);
                sLeaser.sprites[1].color = Color.Lerp(colorA, effectColor, t);
            }

            sLeaser.sprites[0].alpha = Mathf.Pow(currentLife, 0.25f) * (1f - stretched);
            sLeaser.sprites[1].alpha = 0.3f + Mathf.Pow(Mathf.Sin(currentLife * 3.1415927f), 0.7f) * 0.65f * (1f - stretched);
        }

        public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            colorA = Color.Lerp(Color.red, palette.fogColor, 0.2f);
            colorB = Color.Lerp(palette.blackColor, palette.fogColor, 0.2f);
        }

        private Color colorB;

        public RimWorldHealth.RimState state;
    }
}