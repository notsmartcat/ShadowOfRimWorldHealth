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
        if (obj.sourceObject != null && obj.sourceObject is Snail || !singleExplosion.TryGetValue(obj, out OneTimeUseData data) || data.creatures.Contains(self) || damage <= 0 || self.State == null || self.State is not RWPlayerHealthState state)
        {
            return;
        }

        data.creatures.Add(self);

        if(obj.sourceObject != null && obj.sourceObject is not FirecrackerPlant)
            damage *= 200;

        int amount = UnityEngine.Random.Range(1, 5);

        RWBodyPart focusedBodyPart;

        List<RWBodyPart> list = new();

        for (int p = 0; p < amount; p++)
        {
            focusedBodyPart = null;
            list.Clear();

            for (int i = 0; i < state.bodyParts.Count; i++)
            {
                if (state.bodyParts[i].connectedBodyChunks.Count > 0 && !state.bodyParts[i].isInternal && !IsDestroyed(state.bodyParts[i]))
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

                float roll = UnityEngine.Random.Range(0f, chance);

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
            else if (list.Count == 1)
            {
                focusedBodyPart = list[0];
            }

            list.Clear();

            list.Add(focusedBodyPart);

            while (focusedBodyPart != null && focusedBodyPart is not Neck)
            {
                for (int i = 0; i < state.bodyParts.Count; i++)
                {
                    if (!IsDestroyed(state.bodyParts[i]) && state.bodyParts[i].isInternal && IsSubPartName(state.bodyParts[i], list[0]))
                    {
                        Debug.Log("Adding Subpart of " + focusedBodyPart.name + " with the name " + state.bodyParts[i].name);
                        list.Add(state.bodyParts[i]);
                    }
                }

                if (list.Count == 0)
                {
                    break;
                }
                else if (list.Count == 1)
                {
                    focusedBodyPart = list[0];
                    break;
                }

                float chance = 0;

                for (int i = 0; i < list.Count; i++)
                {
                    chance += list[i].coverage;
                }

                float roll = UnityEngine.Random.Range(0f, chance);

                chance = 0;

                RWBodyPart tempFocusedBodyPart = null;

                for (int i = 0; i < list.Count; i++)
                {
                    chance += list[i].coverage;

                    Debug.Log("Roll = " + roll + "/" + chance + " for " + list[i].name);

                    if (roll <= chance)
                    {
                        tempFocusedBodyPart = list[i];

                        Debug.Log("Bodypart out all subparts that was hit is " + tempFocusedBodyPart.name);
                        break;
                    }
                }

                if (tempFocusedBodyPart == null || tempFocusedBodyPart == focusedBodyPart)
                {
                    break;
                }
                else
                {
                    list.Clear();
                    list.Add(tempFocusedBodyPart);
                }
            }

            if (focusedBodyPart != null)
            {
                PhysicalObject sourceObj = obj.sourceObject ?? null;

                string attackName = obj.ToString();
                string attackerName = "";

                bool super = false;

                if (sourceObj == null)
                {
                    attackName = "Explosion";
                }
                else if (sourceObj is EggBug)
                {
                    attackName = "FireBug - Explosion";
                }
                else if (sourceObj is ExplosiveSpear)
                {
                    attackName = "Explosive spear";
                }
                else if (sourceObj is FirecrackerPlant)
                {
                    attackName = "Firecracker";
                }
                else if (sourceObj is ScavengerBomb)
                {
                    attackName = "Bomb";
                }
                else if (sourceObj is Player)
                {
                    attackName = "Slugcat - Explosion";
                }
                else if (sourceObj is Vulture)
                {
                    attackName = "Miros Vulture - Laser explosion";
                }
                else if (ModManager.MSC && sourceObj is MoreSlugcats.EnergyCell)
                {
                    attackName = "Energy cell";
                    super = true;
                }
                else if (ModManager.MSC && sourceObj is MoreSlugcats.FireEgg)
                {
                    attackName = "Fire egg";
                }
                else if (ModManager.MSC && sourceObj is MoreSlugcats.SingularityBomb)
                {
                    attackName = "Singularity bomb";
                    super = true;
                }
                else if (ModManager.MSC && sourceObj is Oracle)
                {
                    attackName = "Oracle";
                }

                if (obj.killTagHolder != null)
                {
                    attackerName = obj.killTagHolder.ToString();
                }

                RWDamageType damageType;

                damageType = super ? new RWSuperBomb() : new RWBomb();

                float tempDamage = super ? damage * 800 : damage;

                state.Damage(damageType, tempDamage / amount, focusedBodyPart, attackName, attackerName);
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