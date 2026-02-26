using System.Collections.Generic;
using UnityEngine;

using static ShadowOfRimWorldHealth.RimWorldHealth;

namespace ShadowOfRimWorldHealth;

internal class CreatureHooks
{
    public static void Apply()
    {
        On.Creature.Violence += CreatureViolence;
    }

    static void CreatureViolence(On.Creature.orig_Violence orig, Creature self, BodyChunk source, Vector2? directionAndMomentum, BodyChunk hitChunk, PhysicalObject.Appendage.Pos hitAppendage, Creature.DamageType type, float damage, float stunBonus)
    {
        orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);

        if (type == Creature.DamageType.Explosion || self is not Player || hitChunk == null || self.State == null || self.State is not RWPlayerHealthState state)
        {
            return;
        }

        Debug.Log("Bodychunk " + hitChunk.index + " was hit!");

        RWBodyPart focusedBodyPart = null;
        RWBodyPart secondaryFocusedBodyPart = null;

        List<RWBodyPart> list = new();

        for (int i = 0; i < state.bodyParts.Count; i++)
        {
            if (state.bodyParts[i].connectedBodyChunks.Contains(hitChunk.index) && !state.bodyParts[i].isInternal && !IsDestroyed(state.bodyParts[i]) && HitBodyPartCheck(state.bodyParts[i]))
            {
                list.Add(state.bodyParts[i]);

                Debug.Log("Bodychunk " + hitChunk.index + " possible hit Bodypart is = " + state.bodyParts[i].name);
            }
        }

        if (list.Count > 1)
        {
            Debug.Log("More then 1 possible BodyPart!");

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

                Debug.Log("Roll = " + roll + "/" + chance + " for " + list[i].name);

                if (roll <= chance)
                {
                    Debug.Log("Success for " + list[i].name);

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
                        Debug.Log("Adding Subpart of " + focusedBodyPart.name + " with the name " + state.bodyParts[i].name);
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

                    Debug.Log("Roll = " + roll + "/" + chance + " for " + list[i].name);

                    if (roll <= chance)
                    {
                        secondaryFocusedBodyPart = list[i];

                        Debug.Log("Bodypart out all subparts that was hit is " + secondaryFocusedBodyPart.name);
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
                Debug.Log("damage = " + damage);
                if (type == Creature.DamageType.Blunt && secondaryFocusedBodyPart != null)
                {
                    tempDamage = damage * Random.Range(0.8f, 0.9f);
                    Debug.Log("TempDamage = " + tempDamage);
                    extraDamage = damage - tempDamage;
                    Debug.Log("ExtraDamage = " + extraDamage);
                }

                Damage(focusedBodyPart, tempDamage);
            }
            if (secondaryFocusedBodyPart != null)
            {
                if (type == Creature.DamageType.Blunt)
                {
                    damage = (damage * Random.Range(0.2f, 0.35f)) + extraDamage;
                }

                Debug.Log("secondary damage = " + damage);

                Damage(secondaryFocusedBodyPart, damage);
            }
        }

        void Damage(RWBodyPart focusedBodyPart, float damage)
        {
            Debug.Log("Bodypart hit is " + focusedBodyPart.name);

            string attackerName = "";

            if (source != null && source.owner != null)
            {
                attackerName = source.owner.ToString();
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

            state.Damage(damageType, damage, focusedBodyPart, attackerName);
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