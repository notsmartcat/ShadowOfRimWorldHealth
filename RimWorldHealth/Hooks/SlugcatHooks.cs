using RWCustom;
using UnityEngine;

using static ShadowOfRimWorldHealth.RimWorldHealth;

namespace ShadowOfRimWorldHealth;

internal class SlugcatHooks
{
    public static void Apply()
    {
        On.Player.Update += PlayerUpdate;
        On.Player.GraphicsModuleUpdated += PlayerGraphicsModuleUpdated;

        On.PlayerGraphics.DrawSprites += PlayerGraphicsDrawSprites;
    }

    static void PlayerGraphicsDrawSprites(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        orig(self, sLeaser, rCam, timeStacker, camPos);

        if (!healthState.TryGetValue(self.player.State, out RWState state) && !ArmCheck(state) && JawCheck(state) && self.player.grasps[0] != null)
        {
            sLeaser.sprites[5].isVisible = false;
        }
    }

    static void PlayerUpdate(On.Player.orig_Update orig, Player self, bool eu)
    {
        if (!healthState.TryGetValue(self.State, out RWState state))
        {
            orig(self, eu);
            return;
        }

        if (!LegCheck(state))
        {
            self.standing = false;

            if (self.animation == Player.AnimationIndex.StandOnBeam)
            {
                self.animation = Player.AnimationIndex.ClimbOnBeam;
            }
        }

        orig(self, eu);

        if (!ArmCheck(state))
        {
            if (!JawCheck(state))
            {
                self.grasps[0]?.Release();
            }

            self.grasps[1]?.Release();
        }
        else
        {
            for (int i = 0; i < state.armSetNames.Count; i++)
            {
                if (state.armSet[state.armSetNames[i]].efficiency <= 0)
                {
                    self.grasps[i]?.Release();
                }
            }
        }
    }

    static void PlayerGraphicsModuleUpdated(On.Player.orig_GraphicsModuleUpdated orig, Player self, bool actuallyViewed, bool eu)
    {
        orig(self, actuallyViewed, eu);

        if (!healthState.TryGetValue(self.State, out RWState state) || self.grasps[0] == null || ArmCheck(state) || !JawCheck(state))
        {
            return;
        }

        Vector2 headPos = self.bodyChunks[0].pos;
        if (self.graphicsModule != null)
        {
            headPos = (self.graphicsModule as PlayerGraphics).head.pos - Custom.DirVec(self.bodyChunks[1].pos, headPos) * 4f + (self.graphicsModule as PlayerGraphics).lookDirection * 4f;
        }

        if (!self.HeavyCarry(self.grasps[0].grabbed) && actuallyViewed)
        {
            self.grasps[0].grabbed.firstChunk.vel = self.bodyChunks[0].vel;
            self.grasps[0].grabbed.firstChunk.MoveFromOutsideMyUpdate(eu, headPos);

            if (self.grasps[0].grabbed is Weapon weapon)
            {
                weapon.setRotation = new Vector2?(Custom.PerpendicularVector(Custom.DirVec(self.bodyChunks[1].pos, headPos) * -1f));
                weapon.rotationSpeed = 0f;
                weapon.ChangeOverlap(true);
            }
        }
        else
        {
            if (!self.HeavyCarry(self.grasps[0].grabbed))
            {
                self.grasps[0].grabbed.firstChunk.pos = headPos;
                self.grasps[0].grabbed.firstChunk.vel = self.mainBodyChunk.vel;
            }
        }
    }

    static bool JawCheck(RWState state)
    {
        for (int i = 0; i < state.bodyParts.Count; i++)
        {
            if (state.bodyParts[i] is Jaw jaw)
            {
                return jaw.efficiency > 0;
            }
        }

        return false;
    }
}