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

            bool dead = playerState.dead;

            if ((dead && ShadowOfOptions.coop_dead_saving.Value == "Clear All Afflictions") || !healthState.TryGetValue(playerState, out RWState state))
            {
                if (campaign.ContainsKey(playerNumber.ToString()))
                {
                    campaign.Remove(playerNumber.ToString());
                    save[campaigName] = campaign;

                    data[saveSlot] = save;
                }

                Debug.Log(all + "Saving Failed for player number " + playerNumber);

                continue;
            }

            saveData["LastCycle"] = game.GetStorySession.saveState.cycleNumber.ToString();

            diseasesToSave = new();
            diseasesToTend = new();

            #region WholeBody
            foreach (RWAffliction affliction in state.wholeBodyAfflictions)
            {
                if (affliction.isCharacterSpecific || dead)
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
                if (dead)
                {
                    continue;
                }

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

                bool partAfflictionDeleted = false;

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
                            if (dead && RWHealthState.IsPartNecessary(part))
                            {
                                partAfflictionDeleted = true;

                                continue;
                            }

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
                        if (dead)
                        {
                            continue;
                        }

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
                if (part.afflictions.Count > 0)
                {
                    saveData[SavingandLoadingHooks.GetBodyPartKeyName(part)] = SavingandLoadingHooks.GetAllAfflictionValueName(part);
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
        if (!data.ContainsKey(saveSlot) || !data.TryGetValueWithType(saveSlot, out Dictionary<string, object> _))
        {
            return;
        }

        (data[saveSlot] as Dictionary<string, object>).Remove(campaignName);

        unrecognizedSaveStrings.Clear();

        Write();
    }

    public void WipeSaveSlot(string saveSlot)
    {
        if (!data.ContainsKey(saveSlot))
        {
            return;
        }

        data.Remove(saveSlot);

        unrecognizedSaveStrings.Clear();

        Write();
    }

    public void ClearUnrecognizedSaveStrings()
    {
        unrecognizedSaveStrings.Clear();
    }
}