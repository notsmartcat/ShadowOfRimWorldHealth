using System.Collections.Generic;
using UnityEngine;

using static ShadowOfRimWorldHealth.RimWorldHealth;

namespace ShadowOfRimWorldHealth;

internal class CreatureHooks
{
    public static void Apply()
    {
        #region Saving and Loading
        #region CreatureState
        On.CreatureState.ctor += NewCreatureState;
        On.CreatureState.LoadFromString += CreatureStateLoadFromString;
        #endregion

        #region SaveState
        On.SaveState.AbstractCreatureToStringStoryWorld_AbstractCreature_WorldCoordinate += SaveStateSaveAbstractCreature;
        #endregion
        #endregion

        On.Creature.Violence += CreatureViolence;
    }

    #region Saving and Loading
    #region CreatureState
    static void NewCreatureState(On.CreatureState.orig_ctor orig, CreatureState self, AbstractCreature creature)
    {
        orig(self, creature);

        if (creature.creatureTemplate.type != CreatureTemplate.Type.Slugcat)
        {
            return;
        }

        if (!healthState.TryGetValue(self, out RWState state))
        {
            healthState.Add(self, new RWState());
            healthState.TryGetValue(self, out state);
        }

        RWHealthState.NewRWHealthState(self, state);
    }
    static void CreatureStateLoadFromString(On.CreatureState.orig_LoadFromString orig, CreatureState self, string[] s)
    {
        orig(self, s);

        if (!healthState.TryGetValue(self, out RWState _))
        {
            return;
        }
    }
    #endregion

    static string SaveStateSaveAbstractCreature(On.SaveState.orig_AbstractCreatureToStringStoryWorld_AbstractCreature_WorldCoordinate orig, AbstractCreature critter, WorldCoordinate pos)
    {
        return orig(critter, pos);
    }
    #endregion

    static void CreatureViolence(On.Creature.orig_Violence orig, Creature self, BodyChunk source, Vector2? directionAndMomentum, BodyChunk hitChunk, PhysicalObject.Appendage.Pos hitAppendage, Creature.DamageType type, float damage, float stunBonus)
    {
        if (type == Creature.DamageType.Explosion || hitChunk == null || self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
            return;
        }

        if (type == Creature.DamageType.Blunt)
        {
            PhysicalObject tempSource = null;

            if (source != null && source.owner != null)
            {
                tempSource = source.owner;
            }

            BluntDamage(self.State, state, hitChunk, damage, tempSource);

            return;
        }

        RWBodyPart focusedBodyPart = GetHitBodyPart(state, hitChunk, null, false);

        if (focusedBodyPart != null)
        {
            float tempDamage = damage;

            Damage(focusedBodyPart, tempDamage);
        }

        orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);

        void Damage(RWBodyPart focusedBodyPart, float damage)
        {
            string attackName = "";

            if (source != null && source.owner != null)
            {
                attackName = source.owner.ToString();
            }

            RWHealthState.Damage(self.State, state, new(), damage, focusedBodyPart, attackName);
        }
    } //This is a backup hook in case a attack causes Violence, all attacks will be hooked into where Violence is called so the right DamageType can be used, this will mean that Violence is never actually meant to be called
}