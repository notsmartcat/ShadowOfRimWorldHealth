using static ShadowOfRimWorldHealth.RimWorldHealth;

namespace ShadowOfRimWorldHealth;

internal class WeaponHooks
{
    public static void Apply()
    {
        On.Explosion.ctor += NewExplosion; ;
    }

    private static void NewExplosion(On.Explosion.orig_ctor orig, Explosion self, Room room, PhysicalObject sourceObject, UnityEngine.Vector2 pos, int lifeTime, float rad, float force, float damage, float stun, float deafen, Creature killTagHolder, float killTagHolderDmgFactor, float minStun, float backgroundNoise)
    {
        throw new System.NotImplementedException();
    }

    static void ExplosionUpdate(On.Explosion.orig_Update orig, Explosion self, bool eu)
    {
        orig(self, eu);

        if (!singleUse.TryGetValue(self.sourceObject, out _))
        {
            singleUse.Add(self.sourceObject, new());
        }
    }
}
