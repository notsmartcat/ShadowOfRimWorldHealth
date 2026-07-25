using UnityEngine;

using static ShadowOfRimWorldHealth.RimWorldHealth;

namespace ShadowOfRimWorldHealth;

internal class HudHooks
{
    public static void Apply()
    {
        On.HUD.HUD.InitSafariHud += HUDInitSafariHud;
        On.HUD.HUD.InitSinglePlayerHud += HUDInitSinglePlayerHud;

        On.HUD.PlayerSpecificMultiplayerHud.ctor += NewPlayerSpecificMultiplayerHud;
    }

    private static void HUDInitSafariHud(On.HUD.HUD.orig_InitSafariHud orig, HUD.HUD self, RoomCamera cam)
    {
        orig(self, cam);

        Debug.Log("HUDInitSafariHud");

        healthTab = new HealthTab(self, null);

        self.AddPart(healthTab);
    }
    static void HUDInitSinglePlayerHud(On.HUD.HUD.orig_InitSinglePlayerHud orig, HUD.HUD self, RoomCamera cam)
    {
        orig(self, cam);

        Debug.Log("HUDInitSinglePlayerHud");

        if (!healthState.TryGetValue((self.owner as Creature).State, out _))
        {
            return;
        }

        healthTab = new HealthTab(self, (self.owner as Creature).abstractCreature);

        self.AddPart(healthTab);
    }

    static void NewPlayerSpecificMultiplayerHud(On.HUD.PlayerSpecificMultiplayerHud.orig_ctor orig, HUD.PlayerSpecificMultiplayerHud self, HUD.HUD hud, ArenaGameSession session, AbstractCreature abstractPlayer)
    {
        orig(self, hud, session, abstractPlayer);

         Debug.Log("NewPlayerSpecificMultiplayerHud");

        if (!healthState.TryGetValue(abstractPlayer.state, out _))
        {
            return;
        }

        healthTab = new HealthTab(hud, abstractPlayer);

        self.hud.AddPart(healthTab);
    }
}