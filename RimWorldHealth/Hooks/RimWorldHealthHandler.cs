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

    public override void BaseLoad() {}

    // save method invoked when you need data to be saved, `handler` is your instantiated saved data handler
    public void Save(RainWorldGame game, string saveSlot, string campaigName)
    {
        Dictionary<string, object> save = [], campaign = [], saveData = [];

        if (!data.TryGetValueWithType(saveSlot, out save))
            save = [];

        if (!save.TryGetValueWithType(campaigName, out campaign))
            campaign = [];

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
                Debug.Log("Saving Failed");
                continue;
            }

            Debug.Log("Saving");

            saveData["LastCycle"] = game.GetStorySession.saveState.cycleNumber.ToString();

            diseasesToSave = new();
            diseasesToTend = new();

            #region WholeBody
            foreach (RWAffliction affliction in state.wholeBodyAfflictions)
            {
                if (affliction.isCharacterSpecific)
                {
                    continue;
                }

                if (affliction is RWDisease disease)
                {
                    diseasesToTend.Add(disease);
                }
            }

            foreach (RWDisease disease in diseasesToTend)
            {
                UpdateDisease(disease, state, playerState);
            }

            state.wholeBodyAfflictions = diseasesToSave;

            if (state.wholeBodyAfflictions.Count > 0)
            {
                saveData["WholeBody"] = SavingandLoadingHooks.GetAllWholeBodyAfflictionValueName(state.wholeBodyAfflictions);
            }
            #endregion

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
                            if (!afflictionsToSave.ContainsKey(injury.part))
                            {
                                afflictionsToSave.Add(injury.part, new());
                            }

                            afflictionsToSave[part].Add(injury);

                            continue;
                        }
                        if (affliction is RWScar scar)
                        {
                            if (scar.isRevealed || scar.isPermanent)
                            {
                                if (!afflictionsToSave.ContainsKey(injury.part))
                                {
                                    afflictionsToSave.Add(injury.part, new());
                                }

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
                    UpdateDisease(disease, state, playerState);
                }

                foreach (RWAffliction disease in diseasesToSave)
                {
                    if (!afflictionsToSave.ContainsKey(part))
                    {
                        afflictionsToSave.Add(part, new());
                    }

                    afflictionsToSave[part].Add(disease);
                }
            }

            UpdateInjuries(injuriesToTend, state);

            foreach (var key in afflictionsToSave)
            {
                key.Key.afflictions = key.Value;
            }

            foreach (RWBodyPart part in state.bodyParts)
            {
                Debug.Log(part + " is being saved");

                if (part.afflictions.Count > 0)
                {
                    Debug.Log(part + " has more then 0 afflictions");
                    saveData[SavingandLoadingHooks.GetBodyPartKeyName(part)] = SavingandLoadingHooks.GetAllAfflictionValueName(part);
                    Debug.Log("Key: " + SavingandLoadingHooks.GetBodyPartKeyName(part) + " Value: " + SavingandLoadingHooks.GetAllAfflictionValueName(part));
                }
            }

            campaign[playerNumber.ToString()] = saveData;
            save[campaigName] = campaign;

            data[saveSlot] = save;
        }

        Write();

        void UpdateInjuries(List<RWInjury> healList, RWState state)
        {
            float afterCycleTreatmentTime = afterCycleLength * 6f;

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

    public void WipeCampaign(string saveSlot, string campaignName)
    {
        Debug.Log("Wiping Save State number: " + campaignName);

        if (!data.ContainsKey(saveSlot) || !data.TryGetValueWithType(saveSlot, out Dictionary<string, object> _))
        {
            return;
        }

        Debug.Log(campaignName + " campaign name exists");

        (data[saveSlot] as Dictionary<string, object>).Remove(campaignName);

        unrecognizedSaveStrings.Clear();

        Write();
    }

    public void WipeSaveSlot(string saveSlot)
    {
        Debug.Log("Wiping Save SLot number: " + saveSlot);

        if (!data.ContainsKey(saveSlot))
        {
            return;
        }

        Debug.Log(saveSlot + " campaign name exists");

        data.Remove(saveSlot);

        unrecognizedSaveStrings.Clear();

        Write();
    }

    public void ClearUnrecognizedSaveStrings()
    {
        unrecognizedSaveStrings.Clear();
    }
}