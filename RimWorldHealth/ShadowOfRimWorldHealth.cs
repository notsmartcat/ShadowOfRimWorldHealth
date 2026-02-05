using BepInEx;
using BepInEx.Logging;
using System;
using UnityEngine;

namespace ShadowOfRimWorldHealth;

[BepInPlugin("notsmartcat.shadowofrimworldhealth", "RimWorld styled health", "1.0.0")]

public class RimWorldHealth : BaseUnityPlugin
{
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
        }
        catch (Exception e) { Logger.LogError(e); }
    }

    private void PlayercheckInput(On.Player.orig_checkInput orig, Player self)
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

        healthTab = new HealthTab(hud, abstractPlayer);

        self.hud.AddPart(healthTab);

        Debug.Log("HealthTab for arena Created");
    }

    void HUDInitSinglePlayerHud(On.HUD.HUD.orig_InitSinglePlayerHud orig, HUD.HUD self, RoomCamera cam)
    {
        orig(self, cam);

        healthTab = new HealthTab(self, (self.owner as Creature).abstractCreature);

        self.AddPart(healthTab);

        Debug.Log("HealthTab Created");
    }

    void PlayerUpdate(On.Player.orig_Update orig, Player self, bool eu)
    {
        orig(self, eu);

        if (self.State is not RWPlayerHealthState state)
        {
            return;
        }

        state.Update();

        if (!self.Consious)
        {
            return;
        }

        if (Input.GetKey("h") && healthTab != null)
        {
            if (buttonHeld == false)
            {
                buttonHeld = true;
                healthTab.ToggleVisibility(self.State as RWPlayerHealthState);
            }
        }
        else
        {
            buttonHeld = false;
        }
    }

    public static bool IsDestroyed(RWBodyPart self)
    {
        return self.injuries.Count != 0 && self.injuries[0] is Destroyed;
    }
}