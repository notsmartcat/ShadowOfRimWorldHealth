using static ShadowOfRimWorldHealth.RimWorldHealth;

namespace ShadowOfRimWorldHealth;

internal class HudHooks
{
    public static void Apply()
    {
        On.HUD.HUD.ClearAllSprites += HUDClearAllSprites;

        On.HUD.HUD.InitSafariHud += HUDInitSafariHud;
        On.HUD.HUD.InitSinglePlayerHud += HUDInitSinglePlayerHud;

        On.HUD.PlayerSpecificMultiplayerHud.ctor += NewPlayerSpecificMultiplayerHud;
    }

    static void HUDClearAllSprites(On.HUD.HUD.orig_ClearAllSprites orig, HUD.HUD self)
    {
        orig(self);

        singleplayerHud = true;

        for (int i = 0; i < healthTabs.Count; i++)
        {
            healthTabs[i] = null;
        }
    }

    static void HUDInitSafariHud(On.HUD.HUD.orig_InitSafariHud orig, HUD.HUD self, RoomCamera cam)
    {
        orig(self, cam);

        singleplayerHud = true;

        healthTabs[0] = new HealthTab(self, null);

        self.AddPart(healthTabs[0]);
    }
    static void HUDInitSinglePlayerHud(On.HUD.HUD.orig_InitSinglePlayerHud orig, HUD.HUD self, RoomCamera cam)
    {
        orig(self, cam);

        if (!healthState.TryGetValue((self.owner as Creature).State, out _))
        {
            return;
        }

        if (ModManager.CoopAvailable && cam.room.game.session.Players.Count > 1)
        {
            singleplayerHud = false;

            for (int k = 0; k < cam.room.game.session.Players.Count; k++)
            {
                healthTabs[k] = new HealthTab(self, cam.room.game.session.Players[k]);

                self.AddPart(healthTabs[k]);
            }

            return;
        }

        singleplayerHud = true;

        healthTabs[0] = new HealthTab(self, (self.owner as Creature).abstractCreature);

        self.AddPart(healthTabs[0]);
    }

    static void NewPlayerSpecificMultiplayerHud(On.HUD.PlayerSpecificMultiplayerHud.orig_ctor orig, HUD.PlayerSpecificMultiplayerHud self, HUD.HUD hud, ArenaGameSession session, AbstractCreature abstractPlayer)
    {
        if (session.Players.Count > 1)
        {
            singleplayerHud = false;
        }

        orig(self, hud, session, abstractPlayer);

        if (!healthState.TryGetValue(abstractPlayer.state, out _))
        {
            return;
        }

        HealthTab healthTab = new(hud, abstractPlayer);

        int playerNumber = ((PlayerState)abstractPlayer.state).playerNumber;

        healthTabs[playerNumber] = healthTab;

        self.hud.AddPart(healthTab);
    }
}