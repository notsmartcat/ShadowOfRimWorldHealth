using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using UnityEngine;

using static ShadowOfRimWorldHealth.RimWorldHealth;

namespace ShadowOfRimWorldHealth;

internal class SavingandLoadingHooks
{
    public static void Apply()
    {
        #region CreatureState
        On.CreatureState.LoadFromString += CreatureStateLoadFromString;
        #endregion

        #region SaveState
        On.SaveState.ctor += NewSaveState;
        On.SaveState.AbstractCreatureToStringStoryWorld_AbstractCreature_WorldCoordinate += SaveStateSaveAbstractCreature;
        On.SaveState.BringUpToDate += SaveStateBringUpToDate;
        #endregion

        #region PlayerState
        On.PlayerState.ctor += NewPlayerState;
        #endregion

        #region PlayerProgression
        On.PlayerProgression.WipeSaveState += PlayerProgressionWipeSaveState;
        On.PlayerProgression.WipeAll += PlayerProgressionWipeAll;
        #endregion

        #region Menu
        On.Menu.Menu.ctor += NewMenu;
        #endregion
    }

    #region CreatureState
    static void CreatureStateLoadFromString(On.CreatureState.orig_LoadFromString orig, CreatureState self, string[] s)
    {
        orig(self, s);

        if (!healthState.TryGetValue(self, out RWState state))
        {
            return;
        }

        state.timeAbstracted = cycleTick;

        List<RWAffliction> diseasesToSave;
        List<RWDisease> diseasesToTend;

        List<RWInjury> injuriesToTend = new();

        Dictionary<RWBodyPart, List<RWAffliction>> afflictionsToSave = new();

        try
        {
            Dictionary<string, string> savedData = self.unrecognizedSaveStrings;

            if (savedData.ContainsKey("ShadowOfRimWorldLastCycle"))
            {
                state.lastCycle = int.Parse(savedData["ShadowOfRimWorldLastCycle"]);
                savedData.Remove("ShadowOfRimWorldLastCycle");
            }

            if (savedData.ContainsKey("ShadowOfRimWorldBloodLoss"))
            {
                state.bloodLoss = float.Parse(savedData["ShadowOfRimWorldBloodLoss"]); 
                int cycleDifference = Mathf.Abs(state.lastCycle - self.creature.world.game.GetStorySession.saveState.cycleNumber);
                float treatmentTime = (ShadowOfOptions.after_cycle_length.Value * 40f * 60f * cycleDifference / 10) + (CycleLength() * (cycleDifference - 1) / 10);
                state.bloodLoss = state.bloodLoss -= 0.333f / CycleLength() * treatmentTime;
                state.bloodLoss = Mathf.Clamp(state.bloodLoss, 0, 1);
                savedData.Remove("ShadowOfRimWorldBloodLoss");
            }

            diseasesToSave = new();
            diseasesToTend = new();

            #region WholeBody
            if (savedData.TryGetValue("ShadowOfRimWorldWholeBody", out string wholeBodyAfflictions))
            {
                string[] allAfflictions = Regex.Split(wholeBodyAfflictions, ";");

                if (allAfflictions.Length > 0)
                {
                    foreach (string affliction in allAfflictions)
                    {
                        string[] afflictionInfo = Regex.Split(affliction, ":");

                        if (afflictionInfo.Length == 0)
                        {
                            continue;
                        }

                        diseasesToTend.Add(LoadAffliction(afflictionInfo, null, self) as RWDisease);
                    }

                    foreach (RWDisease disease in diseasesToTend)
                    {
                        diseasesToSave = UpdateDisease(disease, state, self, diseasesToSave);
                    }

                    state.wholeBodyAfflictions = diseasesToSave;
                }

                savedData.Remove("ShadowOfRimWorldWholeBody");
            }
            #endregion

            foreach (RWBodyPart part in state.bodyParts)
            {
                string bodyPartName = "ShadowOfRimWorld" + GetBodyPartKeyName(part);

                if (savedData.TryGetValue(bodyPartName, out string bodyPartAfflictions))
                {
                    string[] allAfflictions = Regex.Split(bodyPartAfflictions, ";");

                    if (allAfflictions.Length == 0)
                    {
                        continue;
                    }

                    foreach (string affliction in allAfflictions)
                    {
                        string[] afflictionInfo = Regex.Split(affliction, ":");

                        if (afflictionInfo.Length == 0)
                        {
                            continue;
                        }

                        part.afflictions.Add(LoadAffliction(afflictionInfo, part, self));
                    }

                    savedData.Remove(bodyPartName);
                }
            }

            foreach (RWBodyPart part in state.bodyParts)
            {
                diseasesToSave = new();
                diseasesToTend = new();

                foreach (RWAffliction affliction in part.afflictions)
                {
                    if (affliction.isCharacterSpecific)
                    {
                        continue;
                    }

                    if (affliction is RWInjury injury)
                    {
                        if (affliction is RWDestroyed)
                        {
                            afflictionsToSave[part].Add(injury);

                            continue;
                        }
                        if (affliction is RWScar scar)
                        {
                            if (scar.isRevealed || scar.isPermanent)
                            {
                                afflictionsToSave[part].Add(injury);
                            }
                            else
                            {
                                injuriesToTend.Add(injury);
                            }

                            continue;
                        }

                        injuriesToTend.Add(injury);
                    }
                    else if (affliction is RWDisease disease)
                    {
                        diseasesToTend.Add(disease);
                    }
                }

                foreach (RWDisease disease in diseasesToTend)
                {
                    diseasesToSave = UpdateDisease(disease, state, self, diseasesToSave);
                }

                foreach (RWAffliction disease in diseasesToSave)
                {
                    afflictionsToSave[part].Add(disease);
                }
            }

            afflictionsToSave = UpdateInjuries(injuriesToTend, state, afflictionsToSave, self.creature.world.game.GetStorySession.saveState.cycleNumber);

            foreach (var dic in afflictionsToSave)
            {
                dic.Key.afflictions = dic.Value;
            }

            state.updateCapacities = true;
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    #endregion

    #region SaveState
    static void NewSaveState(On.SaveState.orig_ctor orig, SaveState self, SlugcatStats.Name saveStateNumber, PlayerProgression progression)
    {
        orig(self, saveStateNumber, progression);

        rimWorldHealthHandler.Load(progression.rainWorld.options.saveSlot.ToString(), progression.PlayingAsSlugcat.ToString());
    }

    static void SaveStateBringUpToDate(On.SaveState.orig_BringUpToDate orig, SaveState self, RainWorldGame game)
    {
        orig(self, game);

        rimWorldHealthHandler.Save(game, game.rainWorld.options.saveSlot.ToString(), self.progression.PlayingAsSlugcat.ToString());
    }

    static string SaveStateSaveAbstractCreature(On.SaveState.orig_AbstractCreatureToStringStoryWorld_AbstractCreature_WorldCoordinate orig, AbstractCreature self, WorldCoordinate cc)
    {
        if (self == null || self.state == null || self.state.unrecognizedSaveStrings == null || !healthState.TryGetValue(self.state, out RWState state))
        {
            return orig(self, cc);
        }

        try
        {
            Dictionary<string, string> savedData = self.state.unrecognizedSaveStrings;

            savedData["LastCycle"] = self.world.game.GetStorySession.saveState.cycleNumber.ToString();

            savedData["ShadowOfRimWorldBloodLoss"] = state.bloodLoss.ToString();

            List<RWAffliction> afflictionsToSave = new();

            foreach (RWAffliction affliction in state.wholeBodyAfflictions)
            {
                if (affliction.isCharacterSpecific)
                {
                    continue;
                }

                if (affliction is RWDisease disease)
                {
                    afflictionsToSave.Add(disease);
                }
            }

            if (afflictionsToSave.Count > 0)
            {
                savedData["ShadowOfRimWorldWholeBody"] = GetAllWholeBodyAfflictionValueName(afflictionsToSave);
            }

            foreach (RWBodyPart part in state.bodyParts)
            {
                afflictionsToSave = new();

                if (part.afflictions.Count > 0)
                {
                    foreach (RWAffliction affliction in state.wholeBodyAfflictions)
                    {
                        if (affliction.isCharacterSpecific)
                        {
                            continue;
                        }

                        afflictionsToSave.Add(affliction);
                    }

                    part.afflictions = afflictionsToSave;

                    if (afflictionsToSave.Count > 0)
                    {
                        savedData["ShadowOfRimWorld" + GetBodyPartKeyName(part)] = GetAllAfflictionValueName(part);
                    }
                }
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }

        return orig(self, cc);
    }
    #endregion

    #region PlayerState
    static void NewPlayerState(On.PlayerState.orig_ctor orig, PlayerState self, AbstractCreature crit, int playerNumber, SlugcatStats.Name slugcatCharacter, bool isGhost)
    {
        orig(self, crit, playerNumber, slugcatCharacter, isGhost);

        if (!healthState.TryGetValue(self, out RWState state) || !rimWorldHealthHandler.unrecognizedSaveStrings.ContainsKey(playerNumber.ToString()))
        {
            return;
        }

        Dictionary<string, string> savedData = rimWorldHealthHandler.unrecognizedSaveStrings[playerNumber.ToString()];

        if (savedData.ContainsKey("LastCycle"))
        {
            state.lastCycle = int.Parse(savedData["LastCycle"]);
        }

        if (savedData.ContainsKey("BloodLoss"))
        {
            state.bloodLoss = float.Parse(savedData["BloodLoss"]);
            int cycleDifference = Mathf.Abs(state.lastCycle - self.creature.world.game.GetStorySession.saveState.cycleNumber);
            float treatmentTime = (ShadowOfOptions.after_cycle_length.Value * 40f * 60f * cycleDifference / 10) + (CycleLength() * (cycleDifference - 1) / 10);
            state.bloodLoss = state.bloodLoss -= 0.333f / CycleLength() * treatmentTime;
            state.bloodLoss = Mathf.Clamp(state.bloodLoss, 0, 1);
        }

        if (savedData.TryGetValue("WholeBody", out string wholeBodyAfflictions))
        {
            string[] allAfflictions = Regex.Split(wholeBodyAfflictions, ";");

            if (allAfflictions.Length > 0)
            {
                foreach (string affliction in allAfflictions)
                {
                    string[] afflictionInfo = Regex.Split(affliction, ":");

                    if (afflictionInfo.Length == 0)
                    {
                        continue;
                    }

                    state.wholeBodyAfflictions.Add(LoadAffliction(afflictionInfo, null, self));
                }
            }
        }

        foreach (RWBodyPart part in state.bodyParts)
        {
            string bodyPartName = GetBodyPartKeyName(part);

            if (savedData.TryGetValue(bodyPartName, out string bodyPartAfflictions))
            {
                string[] allAfflictions = Regex.Split(bodyPartAfflictions, ";");

                if (allAfflictions.Length == 0)
                {
                    continue;
                }

                foreach (string affliction in allAfflictions)
                {
                    string[] afflictionInfo = Regex.Split(affliction, ":");

                    if (afflictionInfo.Length == 0)
                    {
                        continue;
                    }

                    part.afflictions.Add(LoadAffliction(afflictionInfo, part, self));
                }
            }
        }

        state.updateCapacities = true;
    }
    #endregion

    #region PlayerProgression
    public static void PlayerProgressionWipeSaveState(On.PlayerProgression.orig_WipeSaveState orig, PlayerProgression self, SlugcatStats.Name saveStateNumber)
    {
        orig(self, saveStateNumber);

        rimWorldHealthHandler.WipeCampaign(self.rainWorld.options.saveSlot.ToString(), saveStateNumber.value);
    }

    public static void PlayerProgressionWipeAll(On.PlayerProgression.orig_WipeAll orig, PlayerProgression self)
    {
        orig(self);

        rimWorldHealthHandler.WipeSaveSlot(self.rainWorld.options.saveSlot.ToString());
    }
    #endregion

    #region Menu
    public static void NewMenu(On.Menu.Menu.orig_ctor orig, Menu.Menu self, ProcessManager manager, ProcessManager.ProcessID ID)
    {
        orig(self, manager, ID);

        rimWorldHealthHandler.ClearUnrecognizedSaveStrings();
    }
    #endregion

    public static string GetBodyPartKeyName(RWBodyPart part)
    {
        return part.subName + part.name;
    }

    public static string GetAllAfflictionValueName(RWBodyPart part)
    {
        string name = "";
        RWAffliction affliction;
        string nameEnding;

        for (int i = 0; i < part.afflictions.Count; i++)
        {
            if (i > 0)
                name += ";";

            affliction = part.afflictions[i];

            if (affliction is RWInjury injury)
            {
                nameEnding = "";

                if (injury is RWScar scar)
                {
                    name += "Scar:";

                    nameEnding = $"{scar.isRevealed}:{scar.isPermanent}:{scar.scarDamage}:{scar.painCategory}";
                }
                else if (injury is RWDestroyed)
                {
                    name += "Destroyed:";
                }
                else
                {
                    name += "Injury:";
                }

                name += $"{affliction.isTended}:{affliction.tendQuality}:{injury.attackName}:{injury.attackerName}:{injury.damage}:{injury.damageType}:{injury.infectionTimer}:{injury.healingDifficulty.name}";

                name += nameEnding;
            }
            else if (affliction is RWDisease disease)
            {
                name = $"{affliction.isTended}:{disease.name}:{disease.severity}:{disease.isImmune}:{disease.immunity}:{disease.timeUntilTreatment}:{disease.totalTendQuality}:{disease.InfectionLuck}";
            }
        }

        return name;
    }

    public static string GetAllWholeBodyAfflictionValueName(List<RWAffliction> wholeBodyAfflictions)
    {
        string name = "";
        RWAffliction affliction;
        string nameEnding;

        for (int i = 0; i < wholeBodyAfflictions.Count; i++)
        {
            if (i > 0)
                name += ";";

            affliction = wholeBodyAfflictions[i];

            if (affliction is RWInjury injury)
            {
                nameEnding = "";

                if (injury is RWScar scar)
                {
                    name += "Scar:";

                    nameEnding = $"{scar.isRevealed}:{scar.isPermanent}:{scar.scarDamage}:{scar.painCategory}";
                }
                else if (injury is RWDestroyed)
                {
                    name += "Destroyed:";
                }
                else
                {
                    name += "Injury:";
                }

                name += $"{affliction.isTended}:{affliction.tendQuality}:{injury.attackName}:{injury.attackerName}:{injury.damage}:{injury.damageType}:{injury.infectionTimer}:{injury.healingDifficulty.name}";

                name += nameEnding;
            }
            else if (affliction is RWDisease disease)
            {
                name = $"{disease.name}:{affliction.isTended}:{disease.severity}:{disease.isImmune}:{disease.immunity}:{disease.timeUntilTreatment}:{disease.totalTendQuality}:{disease.InfectionLuck}";
            }
        }

        return name;
    }

    public static RWAffliction LoadAffliction(string[] afflictionInfo, RWBodyPart part, CreatureState state)
    {
        if (afflictionInfo.Length == 0)
        {
            return null;
        }

        if (afflictionInfo[0] == "Injury" || afflictionInfo[0] == "Scar" || afflictionInfo[0] == "Destroyed")
        {
            RWInjury injury = afflictionInfo[0] switch
            {
                "Scar" => new RWScar(state, part),
                "Destroyed" => new RWDestroyed(state, part),
                _ => new RWInjury(state, part)
            };

            injury.isTended = afflictionInfo[1] == "True";
            injury.tendQuality = float.TryParse(afflictionInfo[2], out float tendQuality) ? tendQuality : 0f;
            injury.attackName = afflictionInfo[3];
            injury.attackerName = afflictionInfo[4];
            injury.damage = float.TryParse(afflictionInfo[5], out float damage) ? damage : 0f;
            injury.damageType = RWDamageType.GetRWDamageType(afflictionInfo[6]);
            injury.infectionTimer = float.TryParse(afflictionInfo[7], out float infectionTimer) ? infectionTimer : 0f;

            injury.healingDifficulty = RWHealingDifficulty.GetRWHealingDifficulty(afflictionInfo[8]);

            if (injury is RWScar scar)
            {
                scar.isRevealed = afflictionInfo[9] == "True";
                scar.isPermanent = afflictionInfo[10] == "True";
                scar.scarDamage = float.TryParse(afflictionInfo[11], out float scarDamage) ? scarDamage : 0f;
                scar.painCategory = afflictionInfo[12];
            }

            return injury;
        }
        else
        {
            RWDisease disease = afflictionInfo[0] switch
            {
                "Flu" => new RWFlu(state, part ?? null),
                "Infection" => new RWInfection(state, part ?? null),
                _ => throw new NotImplementedException()
            };

            disease.isTended = afflictionInfo[1] == "True";

            disease.severity = float.TryParse(afflictionInfo[2], out float severity) ? severity : 0f;
            disease.isImmune = afflictionInfo[3] == "True";
            disease.immunity = float.TryParse(afflictionInfo[4], out float immunity) ? immunity : 0f;
            disease.timeUntilTreatment = float.TryParse(afflictionInfo[5], out float timeUntilTreatment) ? timeUntilTreatment : 0f;
            disease.tendQuality = float.TryParse(afflictionInfo[6], out float tendQuality) ? tendQuality : 0f;
            disease.InfectionLuck = float.TryParse(afflictionInfo[7], out float InfectionLuck) ? InfectionLuck : 0f;

            return disease;
        }
    }

    public static Dictionary<RWBodyPart, List<RWAffliction>> UpdateInjuries(List<RWInjury> healList, RWState state, Dictionary<RWBodyPart, List<RWAffliction>> afflictionsToSave, int cycleNumber = -1)
    {
        float treatmentTime;

        if (cycleNumber == -1)
        {
            treatmentTime = ShadowOfOptions.after_cycle_length.Value * 40f * 60f / 10;
        }
        else
        {
            int cycleDifference = Mathf.Abs(state.lastCycle - cycleNumber);

            treatmentTime = (ShadowOfOptions.after_cycle_length.Value * 40f * 60f * cycleDifference / 10) + (CycleLength() * (cycleDifference - 1) / 10);
        }

        if (cycleTick < CycleLength())
        {
            treatmentTime += CycleLength() - cycleTick;
        }

        foreach (RWInjury injury in healList)
        {
            if (!injury.isTended)
            {
                injury.isTended = true;
                injury.tendQuality = Mathf.Clamp(RWHealthState.MedicalTendQuality(state) * 0.3f * 0.7f, 0, 0.7f);
            }
        }

        for (int i = 0; i < treatmentTime; i++)
        {
            if (healList.Count <= 0)
            {
                return afflictionsToSave;
            }

            RWInjury injury = healList[UnityEngine.Random.Range(0, healList.Count)];

            float healRate = 8;

            if (injury.isTended)
            {
                healRate += 4;

                healRate += Mathf.Round(injury.tendQuality) * 0.08f;
            }

            injury.damage -= healRate * 0.1f;

            if (injury is RWScar scar)
            {
                if (scar.damage <= scar.scarDamage)
                {
                    scar.damage = scar.scarDamage;
                    scar.isTended = true;
                    scar.isBleeding = false;
                    scar.isRevealed = true;

                    if (!afflictionsToSave.ContainsKey(injury.part))
                    {
                        afflictionsToSave.Add(injury.part, new());
                    }

                    afflictionsToSave[injury.part].Add(injury);
                    healList.Remove(injury);
                }
            }
            else if (injury.damage <= 0)
            {
                healList.Remove(injury);
            }
        }

        foreach (RWInjury injury in healList)
        {
            if (!afflictionsToSave.ContainsKey(injury.part))
            {
                afflictionsToSave.Add(injury.part, new());
            }

            afflictionsToSave[injury.part].Add(injury);
        }

        return afflictionsToSave;
    }

    public static List<RWAffliction> UpdateDisease(RWDisease disease, RWState state, CreatureState creatureState, List<RWAffliction> diseasesToSave, bool isPlayer = false, int ticksPassed = -1)
    {
        float afterCycleTreatmentTime = 0;

        if (disease.severity >= 1 && !disease.isImmune)
        {
            diseasesToSave.Add(disease);

            return diseasesToSave;
        }

        if (ticksPassed != -1)
        {
            afterCycleTreatmentTime = ticksPassed;
        }
        else if (isPlayer)
        {
            afterCycleTreatmentTime = ShadowOfOptions.after_cycle_length.Value * 40f * 60f; //multiply to turn into tics
        }
        else
        {
            int cycleDifference = Mathf.Abs(state.lastCycle - creatureState.creature.world.game.GetStorySession.saveState.cycleNumber);

            if (cycleDifference > 0)
            {
                afterCycleTreatmentTime = (ShadowOfOptions.after_cycle_length.Value * 40f * 60f * cycleDifference) + (CycleLength() * (cycleDifference - 1));
            }
        }

        if (cycleTick < CycleLength())
        {
            afterCycleTreatmentTime += CycleLength() - cycleTick;
        }

        if (afterCycleTreatmentTime <= 0)
        {
            diseasesToSave.Add(disease);

            return diseasesToSave;
        }

        if (ticksPassed != -1 && (disease.timeUntilTreatment <= 0 || !disease.isTended))
        {
            disease.isTended = true;
            disease.tendQuality = Mathf.Clamp(RWHealthState.MedicalTendQuality(state) * 0.3f * 0.7f, 0, 0.7f);
            disease.timeUntilTreatment = cycleLength * disease.treatmentTimes;
        }

        //Debug.Log("Disease Saving Start");
        //Debug.Log("Disease tendQuality " + disease.tendQuality);

        float timeUntilTreatment = disease.timeUntilTreatment * 40f * 60f; //multiply to turn into tics

        bool willUpdateTend = afterCycleTreatmentTime > timeUntilTreatment;

        //Debug.Log("willUpdateTend " + willUpdateTend);
        //Debug.Log("afterCycleTreatmentTime " + afterCycleTreatmentTime);
        //Debug.Log("timeUntilTreatment " + timeUntilTreatment);

        float treatmentTime = willUpdateTend ? afterCycleTreatmentTime : timeUntilTreatment;

        //Debug.Log("treatmentTime " + treatmentTime);

        bool willSeverityMax;
        bool willImmunityMax;

        float severityMaxTimer;
        float immunityMaxTimer;

        //Debug.Log("pre severity " + disease.severity);
        //Debug.Log("pre immunity " + disease.immunity);

        if (disease.isTended)
        {
            disease.severity -= Tended();
        }

        willSeverityMax = (disease.severity += WillSeverityMax()) >= 1;
        willImmunityMax = (disease.immunity += WillImmunityMax()) >= 1;

        //Debug.Log("post severity " + disease.severity);
        //Debug.Log("post immunity " + disease.immunity);

        severityMaxTimer = SeverityMaxTimer();
        immunityMaxTimer = ImmunityMaxTimer();

        //Debug.Log("severityMaxTimer " + severityMaxTimer);
        //Debug.Log("immunityMaxTimer " + immunityMaxTimer);

        //Debug.Log("pre treatmentTime " + treatmentTime);

        treatmentTime -= timeUntilTreatment;

        if (!willUpdateTend)
        {
            goto willNotUpdateTend;
        }

        if (ticksPassed != -1)
        {
            //Add code to tend the disease if there are tends left

            disease.isTended = false;
            disease.tendQuality = 0;
        }
        else
        {
            disease.isTended = true;
            disease.tendQuality = Mathf.Clamp(RWHealthState.MedicalTendQuality(state) * 0.3f * 0.7f, 0, 0.7f);
        }

        //Debug.Log("Disease tendQuality " + disease.tendQuality);

        //Debug.Log("post treatmentTime " + treatmentTime);

        //Debug.Log("pre severity " + disease.severity);
        //Debug.Log("pre immunity " + disease.immunity);

        if (disease.isTended)
        {
            disease.severity -= Tended();
        }

        willSeverityMax = (disease.severity += WillSeverityMax()) >= 1;
        willImmunityMax = (disease.immunity += WillImmunityMax()) >= 1;

        //Debug.Log("post severity " + disease.severity);
        //Debug.Log("post immunity " + disease.immunity);

        severityMaxTimer = SeverityMaxTimer();
        immunityMaxTimer = ImmunityMaxTimer();

        //Debug.Log("severityMaxTimer " + severityMaxTimer);
        //Debug.Log("immunityMaxTimer " + immunityMaxTimer);

    willNotUpdateTend:

        //Debug.Log("previous timeUntilTreatment in min " + disease.timeUntilTreatment);

        disease.timeUntilTreatment = (cycleLength * disease.treatmentTimes) - (treatmentTime / 40f / 60f);

        //Debug.Log("timeUntilTreatment base in min " + (cycleLength * disease.treatmentTimes));

        //Debug.Log("timeUntilTreatment treatmentTime " + treatmentTime);
        //Debug.Log("timeUntilTreatment treatmentTime in min " + (treatmentTime / 40 / 60));

        //Debug.Log("new timeUntilTreatment in min " + disease.timeUntilTreatment);

        while (ticksPassed != -1 && disease.timeUntilTreatment <= 0)
        {
            disease.timeUntilTreatment += cycleLength * disease.treatmentTimes;
        }

        //Debug.Log("new new timeUntilTreatment in min " + disease.timeUntilTreatment);

        if (willSeverityMax && !willImmunityMax || willSeverityMax && willImmunityMax && severityMaxTimer > immunityMaxTimer)
        {
            if (ShadowOfOptions.debug_logs.Value)
                Debug.Log(all + creatureState + "'s " + disease + "'s severity won");

            if (ticksPassed != -1)
            {
                RWHealthState.Kill(creatureState);
            }

            return diseasesToSave;
        }
        else if (willImmunityMax)
        {
            //Debug.Log("immunity won");
            treatmentTime -= immunityMaxTimer;

            //Debug.Log("treatmentTime " + treatmentTime);

            disease.severity -= disease.severityLoss / CycleLength() * immunityMaxTimer; //subtract severity by the time immunity was at max

            //Debug.Log("severity " + disease.severity);

            disease.severity -= disease.severityLoss / CycleLength() * treatmentTime; //subtract severity by the time left in the treatment

            //Debug.Log("severity " + disease.severity);

            disease.immunity = 1;
            disease.isImmune = true;

            if (disease.severity > 0)
            {
                diseasesToSave.Add(disease);
            }
            else if (ticksPassed != -1)
            {
                disease.RemoveSelf();
            }

            return diseasesToSave;
        }

        diseasesToSave.Add(disease);

        return diseasesToSave;

        float WillSeverityMax()
        {
            return disease.severityGain / CycleLength() * timeUntilTreatment;
        }
        float WillImmunityMax()
        {
            return disease.immunityGain * disease.InfectionLuck * RWHealthState.ImmunityGainSpeed(creatureState, state, ticksPassed == -1) / CycleLength() * timeUntilTreatment;
        }

        float SeverityMaxTimer()
        {
            return willSeverityMax ? (disease.severity - 1) / disease.severityGain / CycleLength() * treatmentTime : 0;
        }
        float ImmunityMaxTimer()
        {
            return willImmunityMax ? (disease.immunity - 1) / disease.immunityGain * disease.InfectionLuck * RWHealthState.ImmunityGainSpeed(creatureState, state) / CycleLength() * treatmentTime : 0;
        }

        float Tended()
        {
            return disease.treatment * disease.tendQuality / CycleLength() * treatmentTime;
        }
    }
}