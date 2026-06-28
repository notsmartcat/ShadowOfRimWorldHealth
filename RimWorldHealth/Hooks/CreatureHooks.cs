using RWCustom;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

using static ShadowOfRimWorldHealth.RimWorldHealth;

namespace ShadowOfRimWorldHealth;

internal class CreatureHooks
{
    public static void Apply()
    {
        #region Saving and Loading
        #region CreatureState
        On.CreatureState.ctor += NewCreatureState;
        On.CreatureState.LoadFromString += CreatureStateLoadFromString;
        #endregion

        #region SaveState
        On.SaveState.ctor += NewSaveState;

        On.SaveState.AbstractCreatureToStringStoryWorld_AbstractCreature_WorldCoordinate += SaveStateSaveAbstractCreature;

        On.SaveState.BringUpToDate += SaveStateBringUpToDate;

        On.PlayerState.ctor += NewPlayerState;
        #endregion
        #endregion

        #region AirBreatherCreature
        On.AirBreatherCreature.Update += AirBreatherCreatureUpdate;
        #endregion

        #region Creature
        On.Creature.Violence += CreatureViolence;
        #endregion

        #region InsectoidCreature
        On.InsectoidCreature.Update += InsectoidCreatureUpdate;
        #endregion

        #region StaticWorld
        On.StaticWorld.InitStaticWorld += StaticWorldInitStaticWorld;
        #endregion
    }

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

    static void NewPlayerState(On.PlayerState.orig_ctor orig, PlayerState self, AbstractCreature crit, int playerNumber, SlugcatStats.Name slugcatCharacter, bool isGhost)
    {
        orig(self, crit, playerNumber, slugcatCharacter, isGhost);

        Debug.Log("playerState load pre for " + crit);

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
                Debug.Log("existing affliction found for WholeBody");

                for (int i = 0; i < allAfflictions.Length; i++)
                {
                    Debug.Log(allAfflictions[i]);

                    string[] afflictionInfo = Regex.Split(allAfflictions[i], ":");

                    if (afflictionInfo.Length == 0)
                    {
                        continue;
                    }

                    Debug.Log(afflictionInfo[0]);

                    state.wholeBodyAfflictions.Add(LoadAffliction(afflictionInfo, null, self));
                }
            }
        }

        for (int bodyPartNumber = 0; bodyPartNumber < state.bodyParts.Count; bodyPartNumber++)
        {
            RWBodyPart part = state.bodyParts[bodyPartNumber];
            string bodyPartName = GetBodyPartKeyName(part);

            Debug.Log("checking for existing afflictions for " + bodyPartName);

            if (savedData.TryGetValue(bodyPartName, out string bodyPartAfflictions))
            {
                string[] allAfflictions = Regex.Split(bodyPartAfflictions, ";");

                if (allAfflictions.Length == 0)
                {
                    continue;
                }

                Debug.Log("existing affliction found for " + bodyPartName);

                for (int i = 0; i < allAfflictions.Length; i++)
                {
                    Debug.Log(allAfflictions[i]);

                    string[] afflictionInfo = Regex.Split(allAfflictions[i], ":");

                    if (afflictionInfo.Length == 0)
                    {
                        continue;
                    }

                    Debug.Log(afflictionInfo[0]);

                    part.afflictions.Add(LoadAffliction(afflictionInfo, part, self));
                }
            }
        }

        state.updateCapacities = true;

        Debug.Log(all + crit + " lastCycle = " + state.lastCycle);
    }
    #region Saving and Loading
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
                    Debug.Log("existing affliction found for WholeBody");

                    for (int i = 0; i < allAfflictions.Length; i++)
                    {
                        Debug.Log(allAfflictions[i]);

                        string[] afflictionInfo = Regex.Split(allAfflictions[i], ":");

                        if (afflictionInfo.Length == 0)
                        {
                            continue;
                        }

                        diseasesToTend.Add(LoadAffliction(afflictionInfo, null, self) as RWDisease);
                    }

                    for (int diseasesNumber = 0; diseasesNumber < diseasesToTend.Count; diseasesNumber++)
                    {
                        UpdateDisease(diseasesToTend[diseasesNumber], state, self);
                    }

                    state.wholeBodyAfflictions = diseasesToSave;
                }

                savedData.Remove("ShadowOfRimWorldWholeBody");
            }
            #endregion

            for (int bodyPartNumber = 0; bodyPartNumber < state.bodyParts.Count; bodyPartNumber++)
            {
                RWBodyPart part = state.bodyParts[bodyPartNumber];
                string bodyPartName = "ShadowOfRimWorld" + GetBodyPartKeyName(part);

                Debug.Log("checking for existing afflictions for " + bodyPartName);

                if (savedData.TryGetValue(bodyPartName, out string bodyPartAfflictions))
                {
                    string[] allAfflictions = Regex.Split(bodyPartAfflictions, ";");

                    if (allAfflictions.Length == 0)
                    {
                        continue;
                    }

                    Debug.Log("existing affliction found for " + bodyPartName);

                    for (int i = 0; i < allAfflictions.Length; i++)
                    {
                        Debug.Log(allAfflictions[i]);

                        string[] afflictionInfo = Regex.Split(allAfflictions[i], ":");

                        if (afflictionInfo.Length == 0)
                        {
                            continue;
                        }

                        Debug.Log(afflictionInfo[0]);

                        part.afflictions.Add(LoadAffliction(afflictionInfo, part, self));
                    }

                    savedData.Remove(bodyPartName);
                }
            }

            for (int bodyPartNumber = 0; bodyPartNumber < state.bodyParts.Count; bodyPartNumber++)
            {
                RWBodyPart part = state.bodyParts[bodyPartNumber];

                diseasesToSave = new();
                diseasesToTend = new();

                for (int afflictionNumber = 0; afflictionNumber < part.afflictions.Count; afflictionNumber++)
                {
                    RWAffliction affliction = part.afflictions[afflictionNumber];

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

                for (int diseasesNumber = 0; diseasesNumber < diseasesToTend.Count; diseasesNumber++)
                {
                    UpdateDisease(diseasesToTend[diseasesNumber], state, self);
                }

                for (int diseasesNumber = 0; diseasesNumber < diseasesToSave.Count; diseasesNumber++)
                {
                    afflictionsToSave[part].Add(diseasesToSave[diseasesNumber]);
                }
            }

            UpdateInjuries(injuriesToTend, state);

            foreach (var key in afflictionsToSave)
            {
                key.Key.afflictions = key.Value;
            }

            state.updateCapacities = true;
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }

        void UpdateInjuries(List<RWInjury> healList, RWState state)
        {
            int cycleDifference = Mathf.Abs(state.lastCycle - self.creature.world.game.GetStorySession.saveState.cycleNumber);

            float afterCycleTreatmentTime = (afterCycleLength * 6f * cycleDifference) + (cycleLength * 6 * (cycleDifference - 1));

            for (int i = 0; i < healList.Count; i++)
            {
                if (!healList[i].isTended)
                {
                    healList[i].isTended = true;
                    healList[i].tendQuality = Mathf.Clamp(RWHealthState.MedicalTendQuality(state) * 0.3f * 0.7f, 0, 0.7f);
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

            for (int afflictionNumber = 0; afflictionNumber < state.wholeBodyAfflictions.Count; afflictionNumber++)
            {
                RWAffliction affliction = state.wholeBodyAfflictions[afflictionNumber];

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



            for (int i = 0; i < state.bodyParts.Count; i++)
            {
                afflictionsToSave = new();

                if (state.bodyParts[i].afflictions.Count > 0)
                {
                    for (int afflictionNumber = 0; afflictionNumber < state.wholeBodyAfflictions.Count; afflictionNumber++)
                    {
                        RWAffliction affliction = state.bodyParts[i].afflictions[afflictionNumber];

                        if (affliction.isCharacterSpecific)
                        {
                            continue;
                        }

                        afflictionsToSave.Add(affliction);
                    }

                    state.bodyParts[i].afflictions = afflictionsToSave;

                    if (afflictionsToSave.Count > 0)
                    {
                        savedData["ShadowOfRimWorld" + GetBodyPartKeyName(state.bodyParts[i])] = GetAllAfflictionValueName(state.bodyParts[i]);
                    }
                }
            }

            //Debug.Log("saving data for " + self);
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }

        return orig(self, cc);
    }

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



            Debug.Log("created new affliction " + injury + " for " + part);

            return injury;
        }
        else
        {
            Debug.Log("Disease name = " + afflictionInfo[0]);

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
    #endregion

    #region AirBreatherCreature
    static void AirBreatherCreatureUpdate(On.AirBreatherCreature.orig_Update orig, AirBreatherCreature self, bool eu)
    {
        orig(self, eu);

        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state) || (self is Lizard lizard && lizard.Swimmer))
        {
            return;
        }

        RWAirInLungs airInLungs = null;

        for (int i = 0; i < state.wholeBodyAfflictions.Count; i++)
        {
            if (state.wholeBodyAfflictions[i] is RWAirInLungs tempAirInLungs)
            {
                airInLungs = tempAirInLungs;
                break;
            }
        }

        if (self.lungs >= 1)
        {
            if (airInLungs != null)
            {
                state.wholeBodyAfflictions.Remove(airInLungs);
            }
        }
        else
        {
            if (airInLungs != null)
            {
                airInLungs.tendQuality = Mathf.InverseLerp(-0.5f, 1, self.lungs);
            }
            else
            {
                state.wholeBodyAfflictions.Add(new RWAirInLungs(self.State, null, Mathf.InverseLerp(-0.5f, 1, self.lungs)));
            }
        }
    }
    #endregion

    #region Creature
    static void CreatureViolence(On.Creature.orig_Violence orig, Creature self, BodyChunk source, Vector2? directionAndMomentum, BodyChunk hitChunk, PhysicalObject.Appendage.Pos hitAppendage, Creature.DamageType type, float damage, float stunBonus)
    {
        if (hitChunk == null || damage <= 0 || self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
            return;
        }

        #region Regular Violence Code
        if (source != null && source.owner is Creature)
        {
            self.SetKillTag((source.owner as Creature).abstractCreature);
        }
        if (directionAndMomentum != null)
        {
            if (hitChunk != null)
            {
                hitChunk.vel += Vector2.ClampMagnitude(directionAndMomentum.Value / hitChunk.mass, 10f);
            }
            else if (hitAppendage != null && self is PhysicalObject.IHaveAppendages)
            {
                (self as PhysicalObject.IHaveAppendages).ApplyForceOnAppendage(hitAppendage, directionAndMomentum.Value);
            }
        }

        float stunFactor = (damage * 30f + stunBonus) / self.Template.baseStunResistance;

        if (type.Index != -1)
        {
            if (self.Template.damageRestistances[type.Index, 1] > 0f)
            {
                stunFactor /= self.Template.damageRestistances[type.Index, 1];
            }
        }

        self.stunDamageType = type;
        self.Stun((int)stunFactor);
        self.stunDamageType = Creature.DamageType.None;
        #endregion

        string attackName;
        string attackerName = "";

        float AP = 0;

        PhysicalObject attacker = source != null && source.owner != null ? source.owner : null;

        if (attacker == null)
        {
            if (type == Creature.DamageType.Electric)
            {
                attackName = "Electricity";

                Override();

                RWHealthState.Damage(self.State, state, new RWElectricalBurn(), damage, AP, GetHitBodyPart(state, hitChunk), attackName, attackerName);
            }
            else if (type == Creature.DamageType.Explosion)
            {
                attackName = "Explosion";

                Override();

                BombDamage(self.State, state, damage * BombDamageMultiplier(self.State is HealthState, false), attackName);
            }
            else if (type == Creature.DamageType.Stab)
            {
                attackName = "Stab";

                Override();

                RWHealthState.Damage(self.State, state, new RWStab(), damage, AP, GetHitBodyPart(state, hitChunk), attackName, attackerName);
            }
            else if (type == Creature.DamageType.Blunt)
            {
                attackName = "Blunt";

                Override();

                BluntDamage(self.State, state, hitChunk, damage, AP, attackName, attackerName);

                return;
            }
            else if (type == Creature.DamageType.Bite)
            {
                attackName = "Bite";

                Override();

                RWHealthState.Damage(self.State, state, new RWBite(), damage, AP, GetHitBodyPart(state, hitChunk), attackName, attackerName);
            }
            else if (type == Creature.DamageType.Water)
            {
                attackName = "Water";

                Override();

                BluntDamage(self.State, state, hitChunk, damage, AP, attackName, attackerName);
            }
            else
            {
                attackName = "None";

                Override();

                BluntDamage(self.State, state, hitChunk, damage, AP, attackName, attackerName);
            }
        }
        else if (attacker is not Creature)
        {
            if (attacker is Boomerang boomerang)
            {
                if (boomerang.thrownBy != null)
                {
                    attackerName = GetCreatureName(boomerang.thrownBy);
                }
                attackName = "Boomerang";

                Override();

                if (weaponstat.TryGetValue(boomerang.abstractPhysicalObject, out RWWeaponStats weaponState))
                {
                    damage = weaponState.damage;
                }
                else
                {
                    damage = 1;
                }

                BluntDamage(self.State, state, hitChunk, damage, AP, attackName, attackerName);
            }
            else if (attacker is DartMaggot dartMaggot)
            {
                if (dartMaggot.shotBy != null)
                {
                    attackerName = GetCreatureName(dartMaggot.shotBy);
                }
                attackName = "Dart Maggot";

                Override();

                damage = 0.5f;

                RWHealthState.Damage(self.State, state, new RWStab(), damage, AP, GetHitBodyPart(state, hitChunk), attackName, attackerName);
            }
            else if (attacker is JellyFish jellyFish)
            {
                if (jellyFish.thrownBy != null)
                {
                    attackerName = GetCreatureName(jellyFish.thrownBy);
                }
                attackName = "Jellyfish";

                Override();

                damage = 1.5f;

                RWHealthState.Damage(self.State, state, new RWElectricalBurn(), damage, AP, GetHitBodyPart(state, hitChunk), attackName, attackerName);
            }
            else if (attacker is Pomegranate pomegranate)
            {
                if (pomegranate.killTagHolder != null)
                {
                    attackerName = GetCreatureName(pomegranate.killTagHolder);
                }
                attackName = "Pomegranate";

                Override();

                damage = 25f;

                BluntDamage(self.State, state, hitChunk, damage, AP, attackName, attackerName);
            }
            else if (attacker is Rock rock)
            {
                if (rock.thrownBy != null)
                {
                    attackerName = GetCreatureName(rock.thrownBy);
                }
                attackName = "Rock";

                Override();

                if (weaponstat.TryGetValue(rock.abstractPhysicalObject, out RWWeaponStats weaponState))
                {
                    damage = weaponState.damage;
                }
                else
                {
                    damage = 1;
                }

                BluntDamage(self.State, state, hitChunk, damage, AP, attackName, attackerName);
            }
            else if (attacker is ScavengerBomb bomb)
            {
                if (bomb.thrownBy != null)
                {
                    attackerName = GetCreatureName(bomb.thrownBy);
                }
                attackName = "Bomb";

                Override();

                BombDamage(self.State, state, damage * BombDamageMultiplier(self.State is HealthState, false), attackName, attackerName);
            }
            else if (attacker is Spear spear)
            {
                if (spear.thrownBy != null)
                {
                    attackerName = GetCreatureName(spear.thrownBy);
                }

                Debug.Log("Spear");

                if (type != Creature.DamageType.Explosion)
                {
                    if (weaponstat.TryGetValue(spear.abstractPhysicalObject, out RWWeaponStats weaponState))
                    {
                        damage = weaponState.damage;
                        AP = weaponState.AP;
                    }
                    else
                    {
                        damage = 8.3f; //1/3 of the RimWorld pila damage due to the spear not having a long cooldown like the pila does
                    }

                    damage *= spear.spearDamageBonus; //if thrown by the non-exhausted Gourmand the damage will match the RimWorld damage
                }

                if (spear.bugSpear)
                {
                    attackName = "Fire Spear";

                    Override();

                    RWHealthState.Damage(self.State, state, new RWStab(), damage, AP, GetHitBodyPart(state, hitChunk, null, true), attackName, attackerName);

                    RWHealthState.Damage(self.State, state, new RWBurn(), 5, AP, GetHitBodyPart(state, hitChunk), attackName, attackerName);

                    return;
                }
                else if (spear is ExplosiveSpear)
                {
                    attackName = "Explosive Spear";

                    Override();

                    Debug.Log("Explosive Spear");

                    if (type == Creature.DamageType.Explosion)
                    {
                        BombDamage(self.State, state, damage * BombDamageMultiplier(self.State is HealthState, false), attackName, attackerName);
                    }
                    else
                    {
                        RWHealthState.Damage(self.State, state, new RWStab(), damage, AP, GetHitBodyPart(state, hitChunk, null, true), attackName, attackerName);
                    }

                    return;
                }
                else if (ModManager.MSC && spear is MoreSlugcats.ElectricSpear)
                {
                    attackName = "Electric Spear";

                    Override();

                    if (type == Creature.DamageType.Electric)
                    {
                        RWHealthState.Damage(self.State, state, new RWElectricalBurn(), 5, AP, GetHitBodyPart(state, hitChunk), attackName, attackerName);
                    }
                    else
                    {
                        RWHealthState.Damage(self.State, state, new RWStab(), damage, AP, GetHitBodyPart(state, hitChunk, null, true), attackName, attackerName);
                    }

                    return;
                }

                attackName = "Spear";

                Override();

                RWHealthState.Damage(self.State, state, new RWStab(), damage, AP, GetHitBodyPart(state, hitChunk, null, true), attackName, attackerName);
            }
            else if (ModManager.MSC && attacker is MoreSlugcats.Bullet bullet)
            {
                if (bullet.thrownBy != null)
                {
                    attackerName = GetCreatureName(bullet.thrownBy);
                }

                damage = 18;

                AP = 27;

                if (bullet.abstractBullet.bulletType == JokeRifle.AbstractRifle.AmmoType.Rock)
                {
                    damage = 5;
                    attackName = "JokeRifle - Rock";
                }
                else if (bullet.abstractBullet.bulletType == JokeRifle.AbstractRifle.AmmoType.Light)
                {
                    damage = 1;
                    attackName = "JokeRifle - Light";
                }
                else if (bullet.abstractBullet.bulletType == JokeRifle.AbstractRifle.AmmoType.Ash)
                {
                    damage = 0.5f;
                    attackName = "JokeRifle - Ash";
                }
                else if (bullet.abstractBullet.bulletType == JokeRifle.AbstractRifle.AmmoType.Void)
                {
                    attackName = "JokeRifle - Void";
                }
                else if (bullet.abstractBullet.bulletType == JokeRifle.AbstractRifle.AmmoType.Fruit)
                {
                    damage = 2;
                    attackName = "JokeRifle - Fruit";
                }
                else
                {
                    attackName = "Bullet";
                }

                Override();

                RWHealthState.Damage(self.State, state, new RWBullet(), damage, AP, GetHitBodyPart(state, hitChunk, null, true), attackName, attackerName);
            }
            else if (ModManager.MSC && attacker is MoreSlugcats.LillyPuck lillyPuck)
            {
                if (lillyPuck.thrownBy != null)
                {
                    attackerName = GetCreatureName(lillyPuck.thrownBy);
                }
                attackName = "Lilypuck";

                Override();

                if (weaponstat.TryGetValue(lillyPuck.abstractPhysicalObject, out RWWeaponStats weaponState))
                {
                    damage = weaponState.damage;
                }
                else
                {
                    damage = 0.8f;
                }

                BluntDamage(self.State, state, hitChunk, damage, AP, attackName, attackerName);
            }
            else
            {
                attackName = attacker.ToString();

                Override();

                BluntDamage(self.State, state, hitChunk, damage, AP, attackName, attackerName);
            }
        }
        else if (attacker is BigNeedleWorm)
        {
            attackerName = GetCreatureName((Creature)attacker);
            attackName = attackerName + " - Needle";

            Override();

            RWHealthState.Damage(self.State, state, new RWStab(), damage, AP, GetHitBodyPart(state, hitChunk, null, true), attackName, attackerName);
        }
        else if (attacker is BigSpider)
        {
            attackerName = GetCreatureName((Creature)attacker);

            attackName = attackerName + " - Fangs";

            Override();

            RWHealthState.Damage(self.State, state, new RWBite(), damage, AP, GetHitBodyPart(state, hitChunk, null, true), attackName, attackerName);
        }
        else if (attacker is DropBug)
        {
            attackerName = GetCreatureName((Creature)attacker);

            attackName = attackerName + " - Mandibles";

            Override();

            RWHealthState.Damage(self.State, state, new RWBite(), damage, AP, GetHitBodyPart(state, hitChunk, null, true), attackName, attackerName);
        }
        else if (attacker is EggBug)
        {
            attackerName = GetCreatureName((Creature)attacker);

            attackName = attackerName + " - Spine Spikes";

            Override();

            RWHealthState.Damage(self.State, state, new RWStab(), damage, AP, GetHitBodyPart(state, hitChunk, null, true), attackName, attackerName);
        }
        else if (attacker is JetFish)
        {
            attackerName = GetCreatureName((Creature)attacker);

            attackName = attackerName + " - Head";

            Override();

            BluntDamage(self.State, state, hitChunk, damage, AP, attackName, attackerName);
        }
        else if (attacker is Leech)
        {
            attackerName = GetCreatureName((Creature)attacker);

            attackName = attackerName + " - Teeth";

            Override();

            RWHealthState.Damage(self.State, state, new RWBite(), damage, AP, GetHitBodyPart(state, hitChunk), attackName, attackerName);
        }
        else if (attacker is Lizard lizard)
        {
            attackerName = GetCreatureName((Creature)attacker);

            if (type == Creature.DamageType.Explosion && damage == 1.5f || type == Creature.DamageType.Electric && damage == 0.1f)
            {
                attackName = attackerName + " - Blizzard Laser";

                Override();

                RWHealthState.Damage(self.State, state, new RWFrostbite(), damage, AP, GetHitBodyPart(state, hitChunk), attackName, attackerName);
            }
            else
            {
                attackName = attackerName + " - Teeth";

                Override();

                damage = Custom.LerpMap(lizard.lizardParams.maxMusclePower, 0, 16, 4, 22);

                Debug.Log(damage);

                RWHealthState.Damage(self.State, state, new RWBite(), damage, AP, GetHitBodyPart(state, hitChunk, null, true), attackName, attackerName);
            }
        }
        else if (attacker is MirosBird)
        {
            attackerName = GetCreatureName((Creature)attacker);

            attackName = attackerName + " - Beak";

            Override();

            RWHealthState.Damage(self.State, state, new RWBite(), damage, AP, GetHitBodyPart(state, hitChunk, null, true), attackName, attackerName);
        }
        else if (attacker is Player)
        {
            attackerName = GetCreatureName((Creature)attacker);

            if (type == Creature.DamageType.Blunt)
            {
                if (damage == 1)
                {
                    attackName = attackerName + " - Roll";
                }
                else
                {
                    attackName = attackerName + " - Slam";
                }

                Override();

                BluntDamage(self.State, state, hitChunk, damage, AP, attackName, attackerName);
            }
            else
            {
                attackName = attackerName + " - Teeth";

                Override();

                RWHealthState.Damage(self.State, state, new RWBite(), damage, AP, GetHitBodyPart(state, hitChunk, null, true), attackName, attackerName);
            }

        }
        else if (attacker is SkyWhale)
        {
            attackerName = GetCreatureName((Creature)attacker);

            attackName = attackerName + " - Head";

            Override();

            BluntDamage(self.State, state, hitChunk, damage, AP, attackName, attackerName);
        }
        else if (attacker is Vulture)
        {
            attackerName = GetCreatureName((Creature)attacker);

            attackName = attackerName + " - Beak";

            Override();

            RWHealthState.Damage(self.State, state, new RWBite(), damage, AP, GetHitBodyPart(state, hitChunk, null, true), attackName, attackerName);
        }
        else if (ModManager.MSC && attacker is MoreSlugcats.StowawayBug)
        {
            attackerName = GetCreatureName((Creature)attacker);

            attackName = attackerName + " - Tendril";

            Override();

            RWHealthState.Damage(self.State, state, new RWStab(), damage, AP, GetHitBodyPart(state, hitChunk, null, true), attackName, attackerName);
        }
        else if (ModManager.Watcher && attacker is Watcher.BoxWorm)
        {
            attackerName = GetCreatureName((Creature)attacker);

            attackName = attackerName + " - Steam";

            Override();

            RWHealthState.Damage(self.State, state, new RWBurn(), damage, AP, GetHitBodyPart(state, hitChunk), attackName, attackerName);
        }
        else if (ModManager.Watcher && attacker is Watcher.DrillCrab)
        {
            attackerName = GetCreatureName((Creature)attacker);

            attackName = attackerName + " - Drill";

            Override();

            CutDamage(self.State, state, hitChunk, damage, AP, attackName, attackerName);
        }
        else if (ModManager.Watcher && attacker is Watcher.Frog)
        {
            attackerName = GetCreatureName((Creature)attacker);

            if (type == Creature.DamageType.Bite)
            {
                attackName = attackerName + " - Tendril";

                Override();

                RWHealthState.Damage(self.State, state, new RWStab(), AP, damage, GetHitBodyPart(state, hitChunk), attackName, attackerName);
            }
            else
            {
                attackName = attackerName + " - Body";

                Override();

                BluntDamage(self.State, state, hitChunk, damage, AP, attackName, attackerName);
            }
        }
        else if (ModManager.Watcher && attacker is Watcher.Loach)
        {
            attackerName = GetCreatureName((Creature)attacker);

            attackName = attackerName + " - Body";

            Override();

            BluntDamage(self.State, state, hitChunk, damage, AP, attackName, attackerName);
        }
        else if (ModManager.Watcher && attacker is Watcher.Rat)
        {
            attackerName = GetCreatureName((Creature)attacker);

            attackName = attackerName + " - Body";

            Override();

            BluntDamage(self.State, state, hitChunk, damage, AP, attackName, attackerName);
        }
        else if (ModManager.Watcher && attacker is Watcher.RippleSpider)
        {
            attackerName = GetCreatureName((Creature)attacker);

            attackName = attackerName + " - Body";

            Override();

            BluntDamage(self.State, state, hitChunk, damage, AP, attackName, attackerName);
        }
        else
        {
            attackerName = GetCreatureName((Creature)attacker);

            attackName = attackerName + " - Unknown";

            Override();

            BluntDamage(self.State, state, hitChunk, damage, AP, attackName, attackerName);
        }

        void Override()
        {
            if (state.violenceAttackOverride != "")
            {
                attackName = state.violenceAttackOverride;
                state.violenceAttackOverride = "";
            }
            if (state.violenceAttackerOverride != "")
            {
                attackerName = state.violenceAttackerOverride;
                state.violenceAttackerOverride = "";
            }
        }
    }
    #endregion

    #region InsectoidCreature
    static void InsectoidCreatureUpdate(On.InsectoidCreature.orig_Update orig, InsectoidCreature self, bool eu)
    {
        orig(self, eu);

        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return;
        }

        SetToxicBuildup(state, self);
    }
    #endregion

    #region StaticWorld
    static void StaticWorldInitStaticWorld(On.StaticWorld.orig_InitStaticWorld orig)
    {
        orig();

        for (int i = 0; i < StaticWorld.creatureTemplates.Length; i++)
        {
            if (StaticWorld.creatureTemplates[i].IsLizard)
            {
                (StaticWorld.creatureTemplates[i].breedParameters as LizardBreedParams).biteDamageChance = 1;
            }
        }
    }
    #endregion
}