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
        On.CreatureState.ctor += NewCreatureState;
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
    static void NewCreatureState(On.CreatureState.orig_ctor orig, CreatureState self, AbstractCreature creature)
    {
        orig(self, creature);

        if (creature.creatureTemplate.type != CreatureTemplate.Type.Slugcat && (!ModManager.MSC || creature.creatureTemplate.type != MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.SlugNPC))
        {
            return;
        }

        if (!healthState.TryGetValue(self, out RWState state))
        {
            healthState.Add(self, new RWState());
            healthState.TryGetValue(self, out state);
        }

        RWHealthState.NewRWHealthState(self, state);
    }

    static void CreatureStateLoadFromString(On.CreatureState.orig_LoadFromString orig, CreatureState self, string[] s)
    {
        orig(self, s);

        if (!healthState.TryGetValue(self, out RWState state))
        {
            return;
        }

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
                        UpdateDisease(disease, state, self);
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
                    UpdateDisease(disease, state, self);
                }

                foreach (RWAffliction disease in diseasesToSave)
                {
                    afflictionsToSave[part].Add(disease);
                }
            }

            UpdateInjuries(injuriesToTend, state);

            foreach (var dic in afflictionsToSave)
            {
                dic.Key.afflictions = dic.Value;
            }

            state.updateCapacities = true;
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }

        void UpdateInjuries(List<RWInjury> healList, RWState state)
        {
            int cycleDifference = Mathf.Abs(state.lastCycle - self.creature.world.game.GetStorySession.saveState.cycleNumber);

            float afterCycleTreatmentTime = (afterCycleLength * 6f * cycleDifference) + (cycleLength * 6 * (cycleDifference - 1));

            foreach (RWInjury injury in healList)
            {
                if (!injury.isTended)
                {
                    injury.isTended = true;
                    injury.tendQuality = Mathf.Clamp(RWHealthState.MedicalTendQuality(state) * 0.3f * 0.7f, 0, 0.7f);
                }
            }

            for (int i = 0; i < afterCycleTreatmentTime; i++)
            {
                if (healList.Count <= 0)
                {
                    return;
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

                        afflictionsToSave[injury.part].Add(injury);
                        healList.Remove(injury);
                    }
                }
                else if (injury.damage <= 0)
                {
                    healList.Remove(injury);
                }
            }

            for (int i = 0; i < healList.Count; i++)
            {
                afflictionsToSave[healList[i].part].Add(healList[i]);
            }
        }

        void UpdateDisease(RWDisease disease, RWState state, CreatureState creatureState)
        {
            int cycleDifference = Mathf.Abs(state.lastCycle - self.creature.world.game.GetStorySession.saveState.cycleNumber);

            float afterCycleTreatmentTime = (afterCycleLength * 60f * cycleDifference) + (cycleLength * 60 * (cycleDifference - 1));

            if (disease.timeUntilTreatment <= 0 || !disease.isTended)
            {
                disease.isTended = true;
                disease.tendQuality = Mathf.Clamp(RWHealthState.MedicalTendQuality(state) * 0.3f * 0.7f, 0, 0.7f);
                disease.timeUntilTreatment = cycleLength * disease.treatmentTimes;
            }

            float timeUntilTreatment = disease.timeUntilTreatment * 60f;

            bool willUpdateTend = afterCycleTreatmentTime > timeUntilTreatment;

            float treatmentTime = willUpdateTend ? afterCycleTreatmentTime : timeUntilTreatment;

            bool willSeverityMax;
            bool willImmunityMax;

            float severityMaxTimer;
            float immunityMaxTimer;

            if (!willUpdateTend)
            {
                willSeverityMax = (disease.severity += disease.severityGain / (40 * 60 * cycleLength) * treatmentTime) >= 1;
                willImmunityMax = (disease.immunity += disease.immunityGain * disease.InfectionLuck * RWHealthState.ImmunityGainSpeed(creatureState, state) / (40 * 60 * cycleLength) * treatmentTime) >= 1;

                severityMaxTimer = willSeverityMax ? (disease.severity - 1) / disease.severityGain / (40 * 60 * cycleLength) * treatmentTime : 0;
                immunityMaxTimer = willImmunityMax ? (disease.immunity - 1) / disease.immunityGain * disease.InfectionLuck * RWHealthState.ImmunityGainSpeed(creatureState, state) / (40 * 60 * cycleLength) * treatmentTime : 0;

                if (willSeverityMax && willImmunityMax && severityMaxTimer > immunityMaxTimer)
                {
                    return;
                }

                if (willSeverityMax)
                {
                    return;
                }
                else if (willImmunityMax)
                {
                    treatmentTime -= immunityMaxTimer;

                    disease.severity -= disease.severityLoss / (40 * 60 * cycleLength) * immunityMaxTimer;

                    disease.severity -= disease.severityLoss / (40 * 60 * cycleLength) * treatmentTime;

                    if (disease.severity > 0)
                    {
                        disease.timeUntilTreatment = treatmentTime / 60;

                        diseasesToSave.Add(disease);
                    }

                    return;
                }

                disease.timeUntilTreatment = treatmentTime / 60;

                diseasesToSave.Add(disease);

                return;
            }

            willSeverityMax = (disease.severity += disease.severityGain / (40 * 60 * cycleLength) * timeUntilTreatment) >= 1;
            willImmunityMax = (disease.immunity += disease.immunityGain * disease.InfectionLuck * RWHealthState.ImmunityGainSpeed(creatureState, state) / (40 * 60 * cycleLength) * timeUntilTreatment) >= 1;

            severityMaxTimer = willSeverityMax ? (disease.severity - 1) / disease.severityGain / (40 * 60 * cycleLength) * timeUntilTreatment : 0;
            immunityMaxTimer = willImmunityMax ? (disease.immunity - 1) / disease.immunityGain * disease.InfectionLuck * RWHealthState.ImmunityGainSpeed(creatureState, state) / (40 * 60 * cycleLength) * timeUntilTreatment : 0;

            disease.tendQuality = Mathf.Clamp(RWHealthState.MedicalTendQuality(state) * 0.3f * 0.7f, 0, 0.7f);
            treatmentTime -= timeUntilTreatment;

            willSeverityMax = (disease.severity += disease.severityGain / (40 * 60 * cycleLength) * treatmentTime) >= 1;
            willImmunityMax = (disease.immunity += disease.immunityGain * disease.InfectionLuck * RWHealthState.ImmunityGainSpeed(creatureState, state) / (40 * 60 * cycleLength) * treatmentTime) >= 1;

            severityMaxTimer = willSeverityMax ? (disease.severity - 1) / disease.severityGain / (40 * 60 * cycleLength) * treatmentTime : 0;
            immunityMaxTimer = willImmunityMax ? (disease.immunity - 1) / disease.immunityGain * disease.InfectionLuck * RWHealthState.ImmunityGainSpeed(creatureState, state) / (40 * 60 * cycleLength) * treatmentTime : 0;

            disease.timeUntilTreatment = (cycleLength * disease.treatmentTimes) - (treatmentTime / 60);

            if (willSeverityMax && willImmunityMax && severityMaxTimer > immunityMaxTimer)
            {
                return;
            }

            if (willSeverityMax)
            {
                return;
            }
            else if (willImmunityMax)
            {
                treatmentTime -= immunityMaxTimer;

                disease.severity -= disease.severityLoss / (40 * 60 * cycleLength) * immunityMaxTimer;

                disease.severity -= disease.severityLoss / (40 * 60 * cycleLength) * treatmentTime;

                if (disease.severity > 0)
                {
                    disease.timeUntilTreatment = treatmentTime / 60;

                    diseasesToSave.Add(disease);
                }

                return;
            }

            disease.timeUntilTreatment = treatmentTime / 60;

            diseasesToSave.Add(disease);
        }
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
}