using System.Collections.Generic;
using gelbi_silly_lib;
using gelbi_silly_lib.Converter;
using UnityEngine;

using static ShadowOfRimWorldHealth.RimWorldHealth;

namespace ShadowOfRimWorldHealth;

public class RimWorldHealthHandler : BaseSavedDataHandler
{
    public RimWorldHealthHandler(string filename) : base(filename) { }

    public RimWorldHealthHandler(string[] nestedFolders, string filename) : base(nestedFolders, filename) { }

    public Dictionary<string, Dictionary<string, string>> unrecognizedSaveStrings = new();

    public override void BaseLoad()
    {
    }

    // save method invoked when you need data to be saved, `handler` is your instantiated saved data handler
    public void Save(RainWorldGame game, string saveSlot, string campaigName)
    {
        Dictionary<string, object> saves = [], save = [], campaign = [], data = [];

        List<RWAffliction> diseasesToSave;
        List<RWDisease> diseasesToTend;

        List<RWInjury> injuriesToTend = new();

        Dictionary<RWBodyPart, List<RWAffliction>> afflictionsToSave = new();

        for (int playerNumber = 0; playerNumber < game.session.Players.Count; playerNumber++)
        {
            CreatureState playerState = game.session.Players[playerNumber].state;

            if (playerState.dead || !healthState.TryGetValue(playerState, out RWState state))
            {
                //Add code that deletes any info from this player

                continue;
            }

            data["LastCycle"] = game.GetStorySession.saveState.cycleNumber.ToString();

            diseasesToSave = new();
            diseasesToTend = new();

            #region WholeBody
            for (int afflictionNumber = 0; afflictionNumber < state.wholeBodyAfflictions.Count; afflictionNumber++)
            {
                RWAffliction affliction = state.wholeBodyAfflictions[afflictionNumber];

                if (affliction.isCharacterSpecific)
                {
                    continue;
                }

                if (affliction is RWDisease disease)
                {
                    diseasesToTend.Add(disease);
                }
            }

            for (int diseasesNumber = 0; diseasesNumber < diseasesToTend.Count; diseasesNumber++)
            {
                UpdateDisease(diseasesToTend[diseasesNumber], state, playerState);
            }

            state.wholeBodyAfflictions = diseasesToSave;

            if (state.wholeBodyAfflictions.Count > 0)
            {
                data["WholeBody"] = CreatureHooks.GetAllWholeBodyAfflictionValueName(state.wholeBodyAfflictions);
            }
            #endregion

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
                    UpdateDisease(diseasesToTend[diseasesNumber], state, playerState);
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

            for (int i = 0; i < state.bodyParts.Count; i++)
            {
                if (state.bodyParts[i].afflictions.Count > 0)
                {
                    data[CreatureHooks.GetBodyPartKeyName(state.bodyParts[i])] = CreatureHooks.GetAllAfflictionValueName(state.bodyParts[i]);
                }
            }

            campaign[playerNumber.ToString()] = data;
            save[campaigName] = campaign;

            rimWorldHealthHandler.data[saveSlot] = save;
        }

        rimWorldHealthHandler.Write();

        void UpdateInjuries(List<RWInjury> healList, RWState state)
        {
            float afterCycleTreatmentTime = afterCycleLength * 6f;

            for (int i = 0; i < healList.Count; i++)
            {
                if (!healList[i].isTended)
                {
                    healList[i].isTended = true;
                    healList[i].tendQuality = Mathf.Clamp(RWHealthState.MedicalTendQuality(state) * 0.3f * 0.7f, 0, 0.7f);

                    Debug.Log("Injury tendQuality " + healList[i].tendQuality);
                }
            }

            for (int i = 0; i < afterCycleTreatmentTime; i++)
            {
                if (healList.Count <= 0)
                {
                    return;
                }

                RWInjury injury = healList[Random.Range(0, healList.Count)];

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

        void UpdateDisease(RWDisease disease, RWState state, CreatureState playerState)
        {
            float afterCycleTreatmentTime = afterCycleLength * 60f;

            if (disease.timeUntilTreatment <= 0 || !disease.isTended)
            {
                disease.isTended = true;
                disease.tendQuality = Mathf.Clamp(RWHealthState.MedicalTendQuality(state) * 0.3f * 0.7f, 0, 0.7f);
                disease.timeUntilTreatment = cycleLength * disease.treatmentTimes;
            }

            Debug.Log("Disease Saving Start");

            Debug.Log("Disease tendQuality " + disease.tendQuality);

            float timeUntilTreatment = disease.timeUntilTreatment * 60f;

            bool willUpdateTend = afterCycleTreatmentTime > timeUntilTreatment;

            Debug.Log("willUpdateTend " + willUpdateTend);

            Debug.Log("afterCycleTreatmentTime " + afterCycleTreatmentTime);

            Debug.Log("timeUntilTreatment " + timeUntilTreatment);

            float treatmentTime = willUpdateTend ? afterCycleTreatmentTime : timeUntilTreatment;

            Debug.Log("treatmentTime " + treatmentTime);

            bool willSeverityMax;
            bool willImmunityMax;

            float severityMaxTimer;
            float immunityMaxTimer;

            if (!willUpdateTend)
            {
                Debug.Log("won't update tend");

                Debug.Log("pre severity " + disease.severity);
                Debug.Log("pre immunity " + disease.immunity);

                willSeverityMax = (disease.severity += disease.severityGain / (40 * 60 * cycleLength) * treatmentTime) >= 1;
                willImmunityMax = (disease.immunity += disease.immunityGain * disease.InfectionLuck * RWHealthState.ImmunityGainSpeed(playerState, state) / (40 * 60 * cycleLength) * treatmentTime) >= 1;

                Debug.Log("post severity " + disease.severity);
                Debug.Log("post immunity " + disease.immunity);

                severityMaxTimer = willSeverityMax ? (disease.severity - 1) / disease.severityGain / (40 * 60 * cycleLength) * treatmentTime : 0;
                immunityMaxTimer = willImmunityMax ? (disease.immunity - 1) / disease.immunityGain * disease.InfectionLuck * RWHealthState.ImmunityGainSpeed(playerState, state) / (40 * 60 * cycleLength) * treatmentTime : 0;

                Debug.Log("severityMaxTimer " + severityMaxTimer);
                Debug.Log("immunityMaxTimer " + immunityMaxTimer);

                if (willSeverityMax && willImmunityMax && severityMaxTimer > immunityMaxTimer)
                {
                    Debug.Log("severity greater then immunity");
                    return;
                }

                if (willSeverityMax)
                {
                    Debug.Log("severity won");
                    return;
                }
                else if (willImmunityMax)
                {
                    Debug.Log("immunity won");
                    treatmentTime -= immunityMaxTimer;

                    Debug.Log("treatmentTime " + treatmentTime);

                    disease.severity -= disease.severityLoss / (40 * 60 * cycleLength) * immunityMaxTimer;

                    Debug.Log("severity " + disease.severity);

                    disease.severity -= disease.severityLoss / (40 * 60 * cycleLength) * treatmentTime;

                    Debug.Log("severity " + disease.severity);

                    if (disease.severity > 0)
                    {
                        disease.timeUntilTreatment = treatmentTime / 60;

                        Debug.Log("timeUntilTreatment " + disease.timeUntilTreatment);

                        diseasesToSave.Add(disease);
                    }

                    return;
                }

                disease.timeUntilTreatment = treatmentTime / 60;

                Debug.Log("timeUntilTreatment " + disease.timeUntilTreatment);

                diseasesToSave.Add(disease);

                return;
            }

            Debug.Log("will update tend");

            Debug.Log("pre severity " + disease.severity);
            Debug.Log("pre immunity " + disease.immunity);

            willSeverityMax = (disease.severity += disease.severityGain / (40 * 60 * cycleLength) * timeUntilTreatment) >= 1;
            willImmunityMax = (disease.immunity += disease.immunityGain * disease.InfectionLuck * RWHealthState.ImmunityGainSpeed(playerState, state) / (40 * 60 * cycleLength) * timeUntilTreatment) >= 1;

            Debug.Log("post severity " + disease.severity);
            Debug.Log("post immunity " + disease.immunity);

            severityMaxTimer = willSeverityMax ? (disease.severity - 1) / disease.severityGain / (40 * 60 * cycleLength) * timeUntilTreatment : 0;
            immunityMaxTimer = willImmunityMax ? (disease.immunity - 1) / disease.immunityGain * disease.InfectionLuck * RWHealthState.ImmunityGainSpeed(playerState, state) / (40 * 60 * cycleLength) * timeUntilTreatment : 0;

            Debug.Log("severityMaxTimer " + severityMaxTimer);
            Debug.Log("immunityMaxTimer " + immunityMaxTimer);

            Debug.Log("pre treatmentTime " + treatmentTime);

            disease.tendQuality = Mathf.Clamp(RWHealthState.MedicalTendQuality(state) * 0.3f * 0.7f, 0, 0.7f);
            treatmentTime -= timeUntilTreatment;

            Debug.Log("Disease tendQuality " + disease.tendQuality);

            Debug.Log("post treatmentTime " + treatmentTime);

            Debug.Log("pre severity " + disease.severity);
            Debug.Log("pre immunity " + disease.immunity);

            willSeverityMax = (disease.severity += disease.severityGain / (40 * 60 * cycleLength) * treatmentTime) >= 1;
            willImmunityMax = (disease.immunity += disease.immunityGain * disease.InfectionLuck * RWHealthState.ImmunityGainSpeed(playerState, state) / (40 * 60 * cycleLength) * treatmentTime) >= 1;

            Debug.Log("post severity " + disease.severity);
            Debug.Log("post immunity " + disease.immunity);

            severityMaxTimer = willSeverityMax ? (disease.severity - 1) / disease.severityGain / (40 * 60 * cycleLength) * treatmentTime : 0;
            immunityMaxTimer = willImmunityMax ? (disease.immunity - 1) / disease.immunityGain * disease.InfectionLuck * RWHealthState.ImmunityGainSpeed(playerState, state) / (40 * 60 * cycleLength) * treatmentTime : 0;

            Debug.Log("severityMaxTimer " + severityMaxTimer);
            Debug.Log("immunityMaxTimer " + immunityMaxTimer);

            disease.timeUntilTreatment = (cycleLength * disease.treatmentTimes) - (treatmentTime / 60);

            if (willSeverityMax && willImmunityMax && severityMaxTimer > immunityMaxTimer)
            {
                Debug.Log("severity greater then immunity");
                return;
            }

            if (willSeverityMax)
            {
                Debug.Log("severity won");
                return;
            }
            else if (willImmunityMax)
            {
                Debug.Log("immunity won");
                treatmentTime -= immunityMaxTimer;

                Debug.Log("treatmentTime " + treatmentTime);

                disease.severity -= disease.severityLoss / (40 * 60 * cycleLength) * immunityMaxTimer;

                Debug.Log("severity " + disease.severity);

                disease.severity -= disease.severityLoss / (40 * 60 * cycleLength) * treatmentTime;

                Debug.Log("severity " + disease.severity);

                if (disease.severity > 0)
                {
                    disease.timeUntilTreatment = treatmentTime / 60;

                    Debug.Log("timeUntilTreatment " + disease.timeUntilTreatment);

                    diseasesToSave.Add(disease);
                }

                return;
            }

            disease.timeUntilTreatment = treatmentTime / 60;

            Debug.Log("timeUntilTreatment " + disease.timeUntilTreatment);

            diseasesToSave.Add(disease);
        }
    }

    public void Load(string currentSave, string currentCampaign)
    {
        Debug.Log("saveSlot: " + currentSave + " campaignName: " + currentCampaign);

        if (!data.TryGetValueWithType(currentSave, out Dictionary<string, object> saveData) || !saveData.TryGetValueWithType(currentCampaign, out Dictionary<string, object> campaignData))
        {
            return;
        }

        foreach (var player in campaignData)
        {
            if (!campaignData.TryGetValueWithType(player.Key, out Dictionary<string, object> saveData2))
            {
                continue;
            }

            if (!unrecognizedSaveStrings.TryGetValue(player.Key, out Dictionary<string, string> unrecognized))
            {
                unrecognizedSaveStrings[player.Key] = unrecognized = [];
            }

            foreach (var campaignData2 in saveData2)
            {
                unrecognized[campaignData2.Key] = campaignData2.Value.ToString();
            }
        }
    }
}