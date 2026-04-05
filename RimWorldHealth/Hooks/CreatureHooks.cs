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
        orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);

        if (type == Creature.DamageType.Explosion || self is not Player || hitChunk == null || self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return;
        }

        RWBodyPart focusedBodyPart = null;
        RWBodyPart secondaryFocusedBodyPart = null;

        List<RWBodyPart> list = new();

        for (int i = 0; i < state.bodyParts.Count; i++)
        {
            if (state.bodyParts[i].connectedBodyChunks.Contains(hitChunk.index) && !state.bodyParts[i].isInternal && !IsDestroyed(state.bodyParts[i]) && HitBodyPartCheck(state.bodyParts[i]))
            {
                list.Add(state.bodyParts[i]);
            }
        }

        if (list.Count > 1)
        {
            float chance = 0;

            for (int i = 0; i < list.Count; i++)
            {
                chance += list[i].coverage;
            }

            float roll = Random.Range(0f, chance);

            chance = 0;

            for (int i = 0; i < list.Count; i++)
            {
                chance += list[i].coverage;

                if (roll <= chance)
                {
                    focusedBodyPart = list[i];
                    break;
                }
            }
        }

        list = new();

        if (focusedBodyPart != null)
        {
            if (type == Creature.DamageType.Blunt && 0.4f < Random.value)
            {
                for (int i = 0; i < state.bodyParts.Count; i++)
                {
                    if (!IsDestroyed(state.bodyParts[i]) && state.bodyParts[i].isInternal && IsSubPartName(state.bodyParts[i], focusedBodyPart))
                    {
                        list.Add(state.bodyParts[i]);
                    }
                }

                if (list.Count <= 1)
                {
                    goto skip1;
                }

                float chance = 0;

                for (int i = 0; i < list.Count; i++)
                {
                    chance += list[i].coverage;
                }

                float roll = Random.Range(0f, chance);

                chance = 0;

                for (int i = 0; i < list.Count; i++)
                {
                    chance += list[i].coverage;

                    if (roll <= chance)
                    {
                        secondaryFocusedBodyPart = list[i];

                        break;
                    }
                }
            }

        skip1:

            float extraDamage = 0;

            if (type == Creature.DamageType.Blunt)
            {
                damage = Random.Range(0.8f, 1.2f);
            }

            if (focusedBodyPart != null)
            {
                float tempDamage = damage;
                if (type == Creature.DamageType.Blunt && secondaryFocusedBodyPart != null)
                {
                    tempDamage = damage * Random.Range(0.8f, 0.9f);
                    extraDamage = damage - tempDamage;
                }

                Damage(focusedBodyPart, tempDamage);
            }
            if (secondaryFocusedBodyPart != null)
            {
                if (type == Creature.DamageType.Blunt)
                {
                    damage = (damage * Random.Range(0.2f, 0.35f)) + extraDamage;
                }

                Damage(secondaryFocusedBodyPart, damage);
            }
        }

        void Damage(RWBodyPart focusedBodyPart, float damage)
        {
            string attackName = "";

            if (source != null && source.owner != null)
            {
                attackName = source.owner.ToString();
            }

            RWDamageType damageType;

            if (type == Creature.DamageType.Blunt)
            {
                damageType = new RWBlunt();
            }
            else
            {
                damageType = new RWDamageType();
            }

            RWHealthState.Damage(self.State, state, damageType, damage, focusedBodyPart, attackName);
        }

        bool HitBodyPartCheck(RWBodyPart part)
        {
            if (type == Creature.DamageType.Blunt)
            {
                return !part.isInternal && part is not Eye;
            }

            return true;
        }
    } // this whole hook might need to go in the future, all attacks will be hooked into where Violence is called so the right DamageType can be used
}