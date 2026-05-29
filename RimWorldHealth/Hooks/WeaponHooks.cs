using System.Reflection;
using static ShadowOfRimWorldHealth.RimWorldHealth;

namespace ShadowOfRimWorldHealth;

internal class WeaponHooks
{
    public static void Apply()
    {
        On.AbstractPhysicalObject.ctor += NewAbstractPhysicalObject;

        On.Explosion.ctor += NewExplosion;
    }

    static void NewAbstractPhysicalObject(On.AbstractPhysicalObject.orig_ctor orig, AbstractPhysicalObject self, World world, AbstractPhysicalObject.AbstractObjectType type, PhysicalObject realizedObject, WorldCoordinate pos, EntityID ID)
    {
        orig(self, world, type, realizedObject, pos, ID);

        if (weaponstat.TryGetValue(self, out _) || realizedObject == null || realizedObject is not Weapon weapon || weapon is ScavengerBomb || ModManager.MSC && (weapon is MoreSlugcats.SingularityBomb || weapon is MoreSlugcats.Bullet))
        {
            return;
        }

        RWWeaponStats state = weaponstat.GetOrCreateValue(self);

        ApplyWeaponValues(state, weapon);
    }

    public static void ApplyWeaponValues(RWWeaponStats state, Weapon self)
    {
        float rand = UnityEngine.Random.value;
        float qualityMult = 1;

        if (rand < 0.05)
        {
            state.quality = "Awful";
            qualityMult = 0.9f;
        }
        else if (rand < 0.2)
        {
            state.quality = "Poor";
        }
        else if (rand < 0.7)
        {
            state.quality = "Normal";
        }
        else if (rand < 0.85)
        {
            state.quality = "Good";
        }
        else if (rand < 0.95)
        {
            state.quality = "Excellent";
        }
        else if (rand < 0.99)
        {
            state.quality = "Masterwork";
            qualityMult = 1.25f;
        }
        else
        {
            state.quality = "Legendary";
            qualityMult = 1.5f;
        }

        if (self is Boomerang)
        {
            state.damage = 1 * qualityMult;
            state.AP = 0 * qualityMult;
        }
        else if (self is Rock)
        {
            state.damage = 1 * qualityMult;
            state.AP = 0 * qualityMult;
        }
        else if (self is Spear)
        {
            state.damage = 25 * qualityMult;
            state.AP = 10 * qualityMult;
        }
        else if (ModManager.MSC && self is MoreSlugcats.LillyPuck)
        {
            state.damage = 1 * qualityMult;
            state.AP = 0 * qualityMult;
        }
    }

    static void NewExplosion(On.Explosion.orig_ctor orig, Explosion self, Room room, PhysicalObject sourceObject, UnityEngine.Vector2 pos, int lifeTime, float rad, float force, float damage, float stun, float deafen, Creature killTagHolder, float killTagHolderDmgFactor, float minStun, float backgroundNoise)
    {
        orig(self, room, sourceObject, pos, lifeTime, rad, force, damage, stun, deafen, killTagHolder, killTagHolderDmgFactor, minStun, backgroundNoise);

        if (!singleExplosion.TryGetValue(self, out _))
        {
            singleExplosion.Add(self, new());
        }
    }
}