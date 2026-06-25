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

        for (int playerNumber = 0; playerNumber < game.session.Players.Count; playerNumber++)
        {
            if (!healthState.TryGetValue(game.session.Players[playerNumber].state, out RWState state))
            {
                continue;
            }

            data["LastCycle"] = game.GetStorySession.saveState.cycleNumber.ToString();

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