using static ShadowOfRimWorldHealth.RimWorldHealth;

namespace ShadowOfRimWorldHealth;

internal class WeaponHooks
{
    public static void Apply()
    {
        On.Explosion.ctor += NewExplosion;
    }

    private static void NewExplosion(On.Explosion.orig_ctor orig, Explosion self, Room room, PhysicalObject sourceObject, UnityEngine.Vector2 pos, int lifeTime, float rad, float force, float damage, float stun, float deafen, Creature killTagHolder, float killTagHolderDmgFactor, float minStun, float backgroundNoise)
    {
        orig(self, room, sourceObject, pos, lifeTime, rad, force, damage, stun, deafen, killTagHolder, killTagHolderDmgFactor, minStun, backgroundNoise);

        if (!singleUse.TryGetValue(self.sourceObject, out _))
        {
            singleUse.Add(self.sourceObject, new());
        }
    }
}
