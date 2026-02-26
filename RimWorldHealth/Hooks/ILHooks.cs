using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static ShadowOfRimWorldHealth.RimWorldHealth;

namespace ShadowOfRimWorldHealth;

internal class ILHooks
{
    public static void Apply()
    {
        IL.ArenaGameSession.SpawnPlayers += ILArenaGameSessionSpawnPlayers;

        IL.Explosion.Update += ILExplosionUpdate;
    }

    #region ArenaGameSesson
    static void ILArenaGameSessionSpawnPlayers(ILContext il)
    {
        try
        {
            ILCursor val = new(il);
            ILLabel target = null;

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[2]
            {
                x => x.MatchLdfld<ArenaGameSession>("chMeta"),
                x => x.MatchBrfalse(out _)
            }))
            {
                if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[1]
                {
                x => x.MatchBr(out target)
                })){}
                else
                {
                    RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILArenaGameSessionSpawnPlayers target!");
                }
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILArenaGameSessionSpawnPlayers chMeta!");
            }

            if (val.TryGotoPrev(MoveType.Before, new Func<Instruction, bool>[6]
            {
                x => x.MatchLdloc(11),
                x => x.MatchLdloc(11),
                x => x.MatchLdloc(0),
                x => x.MatchLdloc(8),
                x => x.MatchCallvirt("System.Collections.Generic.List`1<ArenaSitting/ArenaPlayer>", "get_Item"),
                x => x.MatchLdfld<ArenaSitting.ArenaPlayer>("playerNumber")
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.Emit(OpCodes.Ldloc, 11);
                val.Emit(OpCodes.Ldloc_0);
                val.Emit(OpCodes.Ldloc, 8);
                val.EmitDelegate(ArenaGameSessionSpawnPlayers);
                val.Emit(OpCodes.Brtrue_S, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILArenaGameSessionSpawnPlayers!");
            }

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[1]
            {
                x => x.MatchBr(out _)
            })) { }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILArenaGameSessionSpawnPlayers skip!");
            }

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[6]
            {
                x => x.MatchLdloc(11),
                x => x.MatchLdloc(11),
                x => x.MatchLdloc(0),
                x => x.MatchLdloc(8),
                x => x.MatchCallvirt("System.Collections.Generic.List`1<ArenaSitting/ArenaPlayer>", "get_Item"),
                x => x.MatchLdfld<ArenaSitting.ArenaPlayer>("playerNumber")
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.Emit(OpCodes.Ldloc, 11);
                val.Emit(OpCodes.Ldloc_0);
                val.Emit(OpCodes.Ldloc, 8);
                val.EmitDelegate(ArenaGameSessionSpawnPlayers2);
                val.Emit(OpCodes.Brtrue_S, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILArenaGameSessionSpawnPlayers!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }

    public static bool ArenaGameSessionSpawnPlayers(ArenaGameSession self, AbstractCreature abstractCreature, List<ArenaSitting.ArenaPlayer> list, int l)
    {
        abstractCreature.state = new RWPlayerHealthState(abstractCreature, list[l].playerNumber, self.characterStats_Mplayer[0].name, false);

        return true;
    }
    public static bool ArenaGameSessionSpawnPlayers2(ArenaGameSession self, AbstractCreature abstractCreature, List<ArenaSitting.ArenaPlayer> list, int l)
    {
        abstractCreature.state = new RWPlayerHealthState(abstractCreature, list[l].playerNumber, new SlugcatStats.Name(ExtEnum<SlugcatStats.Name>.values.GetEntry(list[l].playerNumber), false), false);

        return true;
    }
    #endregion

    #region Explosion
    static void ILExplosionUpdate(ILContext il)
    {
        try
        {
            ILCursor val = new(il);

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[2]
            {
                x => x.MatchIsinst(typeof(Creature)),
                x => x.MatchLdnull()
            }))
            {
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILExplosionUpdate skip2!");
            }

            if (val.TryGotoPrev(MoveType.Before, new Func<Instruction, bool>[2]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<UpdatableAndDeletable>("room")
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.Emit(OpCodes.Ldarg_0);
                val.Emit<UpdatableAndDeletable>(OpCodes.Ldfld, "room");
                val.Emit<Room>(OpCodes.Ldfld, "physicalObjects");
                val.Emit(OpCodes.Ldloc_2);
                val.Emit(OpCodes.Ldelem_Ref);
                val.Emit(OpCodes.Ldloc_3);
                val.Emit(OpCodes.Callvirt, typeof(List<PhysicalObject>).GetMethod("get_Item", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
                val.Emit(OpCodes.Isinst, typeof(Creature));
                val.Emit(OpCodes.Ldloc, 4);
                val.Emit(OpCodes.Ldloc, 12);
                val.Emit(OpCodes.Mul);
                val.Emit(OpCodes.Ldarg_0);
                val.Emit<Explosion>(OpCodes.Ldfld, "lifeTime");
                val.Emit(OpCodes.Conv_R4);
                val.Emit(OpCodes.Div);
                val.EmitDelegate(ExplosionUpdate);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILExplosionUpdate!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }

    public static void ExplosionUpdate(Explosion obj, Creature self, float damage)
    {
        if (obj.sourceObject == null || !singleUse.TryGetValue(obj.sourceObject, out OneTimeUseData data) || data.creatures.Contains(self) || damage <= 0 || self.State == null || self.State is not RWPlayerHealthState state)
        {
            return;
        }

        data.creatures.Add(self);

        Debug.Log("Pre " + damage);
        damage *= 200;
        Debug.Log("Post " + damage);

        int amount = UnityEngine.Random.Range(1, 5);

        for (int p = 0; p < amount; p++)
        {
            RWBodyPart focusedBodyPart = null;

            List<RWBodyPart> list = new();

            for (int i = 0; i < state.bodyParts.Count; i++)
            {
                if (!IsDestroyed(state.bodyParts[i]))
                {
                    list.Add(state.bodyParts[i]);

                    //Debug.Log("Explosion possible hit Bodypart is = " + state.bodyParts[i].name);
                }
            } //Add all non-Destroyed BodyParts because explosions can hit any BodyPart

            if (list.Count > 1)
            {
                Debug.Log("Explosion More then 1 possible BodyPart!");

                float chance = 0;

                for (int i = 0; i < list.Count; i++)
                {
                    chance += list[i].coverage;
                }

                float roll = UnityEngine.Random.Range(0f, chance);

                chance = 0;

                for (int i = 0; i < list.Count; i++)
                {
                    chance += list[i].coverage;

                    Debug.Log("Explosion Roll = " + roll + "/" + chance + " for " + list[i].name);

                    if (roll <= chance)
                    {
                        Debug.Log("Explosion Success for " + list[i].name);

                        focusedBodyPart = list[i];
                        break;
                    }
                }
            }

            if (focusedBodyPart != null)
            {
                Debug.Log("Explosion Bodypart hit is " + list[0].name);

                PhysicalObject sourceObj = obj.sourceObject;

                string attackerName = obj.ToString();

                if (sourceObj is ScavengerBomb)
                {
                    attackerName = "Scavenger bomb";
                }
                else if (sourceObj is ExplosiveSpear)
                {
                    attackerName = "Explosive spear";
                }

                if (obj.killTagHolder != null)
                {
                    attackerName = obj.killTagHolder + " - " + attackerName;
                }

                RWDamageType damageType;

                damageType = new RWBomb();

                state.Damage(damageType, damage / amount, focusedBodyPart, attackerName);
            }
        }
    }
    #endregion
}
/*
class ILHooks
{
	public static bool ShadowOfLizardUpdate(Lizard self)
    {
        return true;
    }
}
*/