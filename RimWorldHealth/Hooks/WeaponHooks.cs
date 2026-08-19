using RWCustom;
using UnityEngine;

using static ShadowOfRimWorldHealth.RimWorldHealth;

namespace ShadowOfRimWorldHealth;

internal class WeaponHooks
{
    public static void Apply()
    {
        On.AbstractPhysicalObject.ctor += NewAbstractPhysicalObject;

        On.Explosion.ctor += NewExplosion;

        On.Spear.ChangeMode += SpearChangeMode;
        On.Spear.LodgeInCreature_CollisionResult_bool_bool += SpearLodgeInCreature;
    }

    static void NewAbstractPhysicalObject(On.AbstractPhysicalObject.orig_ctor orig, AbstractPhysicalObject self, World world, AbstractPhysicalObject.AbstractObjectType type, PhysicalObject realizedObject, WorldCoordinate pos, EntityID ID)
    {
        orig(self, world, type, realizedObject, pos, ID);

        if (weaponstat.TryGetValue(self, out _) || !TypeCheck())
        {
            return;
        }

        RWWeaponStats state = weaponstat.GetOrCreateValue(self);

        ApplyWeaponValues();

        bool TypeCheck()
        {
            return type == AbstractPhysicalObject.AbstractObjectType.Spear ||
                type == AbstractPhysicalObject.AbstractObjectType.Rock ||
                (ModManager.DLCShared && type == DLCSharedEnums.AbstractObjectType.LillyPuck) ||
                (ModManager.Watcher && type == Watcher.WatcherEnums.AbstractObjectType.Boomerang);
        }

        void ApplyWeaponValues()
        {
            float rand = Random.value;
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

            if (type == AbstractPhysicalObject.AbstractObjectType.Spear)
            {
                state.damage = 25f * qualityMult; //Taken from RimWOrld's Pila, might make this slower due to the speed at which it can be thrown
                state.AP = 0.1f * qualityMult;
            }
            else if (type == AbstractPhysicalObject.AbstractObjectType.Rock)
            {
                state.damage = 12f * qualityMult;
                state.AP = 0.05f;
            }
            else if (ModManager.DLCShared && type == DLCSharedEnums.AbstractObjectType.LillyPuck)
            {
                state.damage = 4f * qualityMult;
                state.AP = 0;
            }
            else if (ModManager.Watcher && type == Watcher.WatcherEnums.AbstractObjectType.Boomerang)
            {
                state.damage = 6f * qualityMult;
                state.AP = 0;
            }
        }
    }

    static void NewExplosion(On.Explosion.orig_ctor orig, Explosion self, Room room, PhysicalObject sourceObject, Vector2 pos, int lifeTime, float rad, float force, float damage, float stun, float deafen, Creature killTagHolder, float killTagHolderDmgFactor, float minStun, float backgroundNoise)
    {
        orig(self, room, sourceObject, pos, lifeTime, rad, force, damage, stun, deafen, killTagHolder, killTagHolderDmgFactor, minStun, backgroundNoise);

        if (!singleExplosion.TryGetValue(self, out _))
        {
            singleExplosion.Add(self, new());
        }
    }

    static void SpearChangeMode(On.Spear.orig_ChangeMode orig, Spear self, Weapon.Mode newMode)
    {
        orig(self, newMode);

        if (!weaponstat.TryGetValue(self.abstractPhysicalObject, out RWWeaponStats weaponState))
        {
            return;
        }

        weaponState.wasDeflected = false;
        weaponState.destroyedPart = false;
    }
    static void SpearLodgeInCreature(On.Spear.orig_LodgeInCreature_CollisionResult_bool_bool orig, Spear self, SharedPhysics.CollisionResult result, bool eu, bool isJellyFish)
    {
        if (!weaponstat.TryGetValue(self.abstractPhysicalObject, out RWWeaponStats weaponState) || (!weaponState.wasDeflected && !weaponState.destroyedPart))
        {
            orig(self, result, eu, isJellyFish);
            return;
        }

        self.ChangeMode(Weapon.Mode.Free);
        self.stuckInObject = null;

        if (weaponState.wasDeflected)
        {
            self.room.AddObject(new Spark(result.chunk.pos + Custom.DegToVec(Random.value * 360f) * (5f * Random.value), self.firstChunk.vel * -0.1f + Custom.DegToVec(Random.value * 360f) * (Mathf.Lerp(0.2f, 0.4f, Random.value) * self.firstChunk.vel.magnitude), new Color(1f, 1f, 1f), null, 10, 170));
            self.room.PlaySound(SoundID.Spear_Bounce_Off_Creauture_Shell, self.firstChunk);
            self.vibrate = 20;
            self.firstChunk.vel = self.firstChunk.vel * -0.5f + Custom.DegToVec(Random.value * 360f) * (Mathf.Lerp(0.1f, 0.4f, Random.value) * self.firstChunk.vel.magnitude);
            self.SetRandomSpin();
        }
    }
}