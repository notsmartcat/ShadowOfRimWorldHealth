using BepInEx;
using BepInEx.Logging;
using System;
using UnityEngine;

namespace RimWorldHealth;

[BepInPlugin("notsmartcat.shadowofrimworldhealth", "RimWorld styled health", "1.0.0")]

public class RimWorldHealth : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    public ShadowOfRimWorldHealth.HealthTab healthTab;

    public void OnEnable()
    {
        try
        {
            Logger = base.Logger;

            On.HUD.HUD.InitSinglePlayerHud += HUDInitSinglePlayerHud;

            On.Player.Update += PlayerUpdate;
        }
        catch (Exception e) { Logger.LogError(e); }
    }

    private void HUDInitSinglePlayerHud(On.HUD.HUD.orig_InitSinglePlayerHud orig, HUD.HUD self, RoomCamera cam)
    {
        orig(self, cam);

        healthTab = new ShadowOfRimWorldHealth.HealthTab(self, (self.owner as Creature).abstractCreature);

        self.AddPart(healthTab);
    }

    void PlayerUpdate(On.Player.orig_Update orig, Player self, bool eu)
    {
        orig(self, eu);

        if (Input.GetKeyDown("h"))
        {
            if (healthTab == null)
            {
                healthTab = new ShadowOfRimWorldHealth.HealthTab(self.room.game.cameras[0].hud, self.abstractCreature);

                self.room.game.cameras[0].hud.AddPart(healthTab);
            }

            healthTab.visible = !healthTab.visible;

            Debug.Log("HealthTab visibility is = " + healthTab.visible);
        }
    }
}