using BepInEx;
using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ShadowOfRimWorldHealth;

[BepInPlugin("notsmartcat.shadowofrimworldhealth", "RimWorld styled health", "1.0.0")]

public class RimWorldHealth : BaseUnityPlugin
{
    public class OneTimeUseData
    {
        public List<Creature> creatures = new();
    }

    public static readonly ConditionalWeakTable<PhysicalObject, OneTimeUseData> singleUse = new();

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
            WeaponHooks.Apply();
        }
        catch (Exception e) { Logger.LogError(e); }
    }

    void PlayercheckInput(On.Player.orig_checkInput orig, Player self)
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
    }

    void HUDInitSinglePlayerHud(On.HUD.HUD.orig_InitSinglePlayerHud orig, HUD.HUD self, RoomCamera cam)
    {
        orig(self, cam);

        healthTab = new HealthTab(self, (self.owner as Creature).abstractCreature);

        self.AddPart(healthTab);
    }

    void PlayerUpdate(On.Player.orig_Update orig, Player self, bool eu)
    {
        orig(self, eu);

        if (self.State is RWPlayerHealthState state)
        {
            state.Update();
        }

        if (self.Stunned && !self.dead)
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
        return self.afflictions.Count != 0 && self.afflictions[0] is RWDestroyed;
    }

    public static bool IsSubPartDestroyed(RWPlayerHealthState state, RWBodyPart self)
    {
        if (self.afflictions.Count == 1 && self.afflictions[0] is RWDestroyed && self.subPartOf != "")
        {
            for (int i = 0; i < state.bodyParts.Count; i++)
            {
                if (state.bodyParts[i].name == self.subPartOf && state.bodyParts[i] is not UpperTorso && state.bodyParts[i] is not LowerTorso && state.bodyParts[i].afflictions.Count == 1 && state.bodyParts[i].afflictions[0] is RWDestroyed)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static bool IsSubPartName(RWBodyPart self, RWBodyPart other)
    {
        if (self.parentSubName)
        {
            return (self.subName + self.subPartOf) == (other.subName + other.name);
        }
        else
        {
            return self.subPartOf == other.name;
        }
    }
}