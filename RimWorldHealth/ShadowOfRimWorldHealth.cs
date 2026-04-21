using BepInEx;
using BepInEx.Logging;
using Mono.Cecil;
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

        public float cycleLength = 13;

        public int healingRateTics = 600;
        public int healingRate = 600;

        public bool hasEaten = false;

        public int medicalSkill = 0;

        public string violenceAttackOverride = "";
        public string violenceAttackerOverride = "";
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

        if (!healthState.TryGetValue(abstractPlayer.state, out RWState _))
        {
            return;
        }

        healthTab = new HealthTab(hud, abstractPlayer);

        self.hud.AddPart(healthTab);
    }

    void HUDInitSinglePlayerHud(On.HUD.HUD.orig_InitSinglePlayerHud orig, HUD.HUD self, RoomCamera cam)
    {
        orig(self, cam);

        if (!healthState.TryGetValue((self.owner as Creature).State, out RWState _))
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

                        state.updateCapacities = true;
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

                
                for (int i = 0; i < state.bodyParts.Count; i++)
                {
                    if (state.bodyParts[i] is Arm part && !IsDestroyed(part) && part.subName == "Right")
                    {
                        RWHealthState.Damage(self.State, state, new RWPoke(), 999999f, part, "oopsie");

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
            if (buttonHeld == false)
            {
                buttonHeld = true;

                CreatureState healthTabCreatureState = self.State;
                RWState healthTabState = state;

                for (int i = 0; i < self.grasps.Length; i++)
                {
                    if (self.grasps[i] != null && self.grasps[i].grabbedChunk != null && self.grasps[i].grabbedChunk.owner != null && self.grasps[i].grabbedChunk.owner is Creature crit && crit.State != null && healthState.TryGetValue(crit.State, out RWState tempHealthTabState))
                    {
                        healthTabState = tempHealthTabState;
                        healthTabCreatureState = crit.State;
                        break;
                    }
                }

                healthTab.ToggleVisibility(healthTabCreatureState, healthTabState);
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
                if (state.bodyParts[i].name == self.subPartOf && state.bodyParts[i] is not UpperTorso && state.bodyParts[i].afflictions.Count == 1 && state.bodyParts[i].afflictions[0] is RWDestroyed)
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

            bool isSuper = attackerName == "Energy Cell" || attackerName == "Singularity Bomb";

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
                        //Debug.Log("Adding Subpart of " + focusedBodyPart.name + " with the name " + state.bodyParts[i].name);
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

                    //Debug.Log("Roll = " + roll + "/" + chance + " for " + list[i].name);

                    if (roll <= chance)
                    {
                        tempFocusedBodyPart = list[i];

                        //Debug.Log("Bodypart out all subparts that was hit is " + tempFocusedBodyPart.name);
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

                RWHealthState.Damage(self, state, damageType, damage / amount, focusedBodyPart, attackName, attackerName);
            }
        }
    }
    public static void BluntDamage(CreatureState self, RWState state, BodyChunk hitChunk, float damage, string attackName = "", string attackerName = "")
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
            RWHealthState.Damage(self, state, new RWBlunt(), damage, focusedBodyPart, attackName, attackerName);
        }
    }

    public static void CutDamage(CreatureState self, RWState state, BodyChunk hitChunk, float damage, string attackName = "", string attackerName = "")
    {
        //Debug.Log("CutDamage");

        float additionalPartsRoll = UnityEngine.Random.value;
        int additionalParts;

        if (additionalPartsRoll <= 0.3f)
        {
            //Debug.Log("CutDamage hit only 1 bodyPart");
            Damage(GetHitBodyPart(state, hitChunk), damage);
            return;
        }
        else if (additionalPartsRoll <= 0.75f)
        {
            //Debug.Log("CutDamage hit 1 additional bodyPart");
            additionalParts = 1;
        }
        else if (additionalPartsRoll <= 0.95f)
        {
            //Debug.Log("CutDamage hit 2 additional bodyParts");
            additionalParts = 2;
        }
        else
        {
            //Debug.Log("CutDamage hit 3 additional bodyParts");
            additionalParts = 3;
        }

        damage *= (1 + 1.4f) / (1 + additionalParts + 1.4f) * (1 + additionalPartsRoll);

        RWBodyPart focusedBodyPart = GetHitBodyPart(state, hitChunk);
        //Debug.Log("CutDamage focusedBodyPart is " + focusedBodyPart);

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
            RWHealthState.Damage(self, state, new RWCut(), damage, focusedBodyPart, attackName, attackerName);
        }
    }

    public static void ScratchDamage(CreatureState self, RWState state, BodyChunk hitChunk, float damage, string attackName = "", string attackerName = "")
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
            RWHealthState.Damage(self, state, new RWScratch(), damage, focusedBodyPart, attackName, attackerName);
        }
    }

    public static void CentipedeShockDamage(CreatureState self, RWState state, Centipede source, bool underwater, bool shockGiveUp)
    {
        RWBodyPart focusedBodyPart = GetHitBodyPart(state);
        //Debug.Log("CentipedeShockDamage focusedBodyPart is " + focusedBodyPart);

        string attackerName;
        float damage;

        if (source.abstractCreature.creatureTemplate.type == CreatureTemplate.Type.SmallCentipede)
        {
            attackerName = "Baby centipede";
            damage = UnityEngine.Random.Range(0.8f, 1.2f);
        }
        else if (source.abstractCreature.creatureTemplate.type == CreatureTemplate.Type.Centiwing)
        {
            attackerName = "Centiwing";
            damage = UnityEngine.Random.Range(4.8f, 8.2f);
        }
        else if (ModManager.DLCShared && source.abstractCreature.creatureTemplate.type == DLCSharedEnums.CreatureTemplateType.AquaCenti)
        {
            attackerName = "AquaCenti";
            damage = UnityEngine.Random.Range(4.8f, 8.2f);
        }
        else if (source.abstractCreature.creatureTemplate.type == CreatureTemplate.Type.RedCentipede)
        {
            attackerName = "Red centipede";
            damage = UnityEngine.Random.Range(8.8f, 12.2f);
        }
        else
        {
            attackerName = "Centipede";
            damage = UnityEngine.Random.Range(4.8f, 8.2f);
        }

        if (underwater)
        {
            damage *= 1.2f;
        }

        string attackName = attackerName + (source.Submersion > 0f ? " - Underwater shock" : " - Shock");

        RWHealthState.Damage(self, state, new RWElectricBurn(), damage, focusedBodyPart, attackName, attackerName);

        if (shockGiveUp && !self.dead)
        {
            source.Stun(6);
            source.shockGiveUpCounter = Math.Max(source.shockGiveUpCounter, 30);
            source.AI.annoyingCollisions = Math.Min(source.AI.annoyingCollisions * 2, 150);
        }
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

        //Debug.Log(poison);
        
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

        string name = self.ToString();

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
            name = "Slugcat";
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
                name = "Scavenger Elite";
            }
            else if (scavenger.Templar)
            {
                name = "Scavenger Templar";
            }
            else if (scavenger.Disciple)
            {
                name = "Scavenger Templar";
            }
            else
            {
                name = "Scavenger";
            }
        }
        else if (ModManager.MSC && self is SkyWhale)
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

        return name;
    }
}