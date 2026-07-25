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
                diseasesToSave = SavingandLoadingHooks.UpdateDisease(disease, state, playerState, diseasesToSave, true);
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
                    diseasesToSave = SavingandLoadingHooks.UpdateDisease(disease, state, playerState, diseasesToSave, true);
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

            afflictionsToSave = SavingandLoadingHooks.UpdateInjuries(injuriesToTend, state, afflictionsToSave);

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