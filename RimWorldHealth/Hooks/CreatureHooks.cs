using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static ShadowOfRimWorldHealth.RimWorldHealth;

namespace ShadowOfRimWorldHealth;

internal class CreatureHooks
{
    public static void Apply()
    {
        On.Creature.Violence += CreatureViolence;
    }

    static void CreatureViolence(On.Creature.orig_Violence orig, Creature self, BodyChunk source, UnityEngine.Vector2? directionAndMomentum, BodyChunk hitChunk, PhysicalObject.Appendage.Pos hitAppendage, Creature.DamageType type, float damage, float stunBonus)
    {
        orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);

        if (self is not Player || hitChunk == null || self.State == null || self.State is not RWPlayerHealthState state)
        {
            return;
        }

        Debug.Log("Bodychunk " + hitChunk.index + " was hit!");

        List<RWBodyPart> list = new();
        List<RWBodyPart> list2 = new();

        for (int i = 0; i < state.bodyParts.Count; i++)
        {
            if (state.bodyParts[i].connectedBodyChunks.Contains(hitChunk.index))
            {
                list.Add(state.bodyParts[i]);

                Debug.Log("possible hit Bodyparts is = " + state.bodyParts[i].name);
            }
        }

        if (list.Count > 1)
        {
            Debug.Log("More then 1 possible Bodychunk!");

            float chance = 0;

            for (int i = 0;i < list.Count;i++)
            {
                chance += list[i].coverage;
            }

            float roll = UnityEngine.Random.Range(0f, chance);

            chance = 0;

            for (int i = 0; i < list.Count; i++)
            {
                chance += list[i].coverage;

                Debug.Log("Roll = " + roll + "/" + chance + " for " + list[i].name);

                if (roll <= chance)
                {
                    Debug.Log("Success for " + list[i].name);

                    list2.Add(list[i]);
                    break;
                }
            }
        }

        if (list2.Count != 0)
        {
            list = new(list2);
            list2.Clear();
        }

        if (list.Count > 0)
        {
            while (true) 
            {
                RWBodyPart focusedBodyPart = list[0];

                for (int i = 0; i < state.bodyParts.Count; i++)
                {
                    if (state.bodyParts[i].subPartOf == list[0].name)
                    {
                        Debug.Log("Adding Subpart of " + list[0].name + " with the name " + state.bodyParts[i].name);
                        list.Add(state.bodyParts[i]);
                    }
                }

                if (list.Count <= 1)
                {
                    break;
                }

                float chance = 0;

                for (int i = 0; i < list.Count; i++)
                {
                    chance += list[i].coverage;
                }

                float roll = UnityEngine.Random.Range(0f, chance);

                chance = 0;

                list2 = new(list);

                for (int i = 0; i < list2.Count; i++)
                {
                    chance += list[i].coverage;

                    Debug.Log("Roll = " + roll + "/" + chance + " for " + list[i].name);

                    if (roll <= chance)
                    {
                        list.Clear();

                        list.Add(list2[i]);

                        Debug.Log("Bodypart out all subparts that was hit is " + list[0].name);
                        break;
                    }
                }

                if (list.Count <= 1 || list[0] == focusedBodyPart)
                {
                    break;
                }
            }

            if (list.Count > 0)
            {
                Debug.Log("Bodypart hit is " + list[0]);

                state.Damage(type.ToString(), damage, list[0]);
            }

        }
    }
}
