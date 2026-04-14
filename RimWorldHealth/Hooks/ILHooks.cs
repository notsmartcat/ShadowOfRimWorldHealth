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
        #region AbstractCreature
        IL.AbstractCreature.IsEnteringDen += ILAbstractCreatureIsEnteringDen;
        #endregion

        #region Lungs
        #region AirBreatherCreature
        IL.AirBreatherCreature.Update += ILAirBreatherCreatureUpdate;
        #endregion
        IL.Player.LungUpdate += ILPlayerLungUpdate;
        #endregion

        #region Explosion
        IL.Explosion.Update += ILExplosionUpdate;
        #endregion

        #region Player
        IL.Player.GrabUpdate += ILPlayerGrabUpdate;
        #endregion
    }

    #region AbstractCreature
    static void ILAbstractCreatureIsEnteringDen(ILContext il)
    {
        try
        {
            ILCursor val = new(il);

            if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[2]
            {
                x => x.MatchIsinst(typeof(AbstractCreature)),
                x => x.MatchCallvirt<AbstractCreature>("Die")
            }))
            {
                val.MoveBeforeLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(AbstractCreatureIsEnteringDenFeed);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILAbstractCreatureIsEnteringDen feed!");
            }

            if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[2]
            {
                x => x.MatchLdcI4(200),
                x => x.MatchStfld<AbstractCreature>("remainInDenCounter")
            }))
            {
                val.MoveBeforeLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(AbstractCreatureIsEnteringDenEat);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILAbstractCreatureIsEnteringDen!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }

    public static void AbstractCreatureIsEnteringDenFeed(AbstractCreature self)
    {
        if (self.state == null || !healthState.TryGetValue(self.state, out RWState state))
        {
            return;
        }

        state.hasEaten = true;
    }

    public static void AbstractCreatureIsEnteringDenEat(AbstractCreature self)
    {
        if (self.state == null || !healthState.TryGetValue(self.state, out RWState state))
        {
            return;
        }

        self.remainInDenCounter = Mathf.Max(10, Mathf.RoundToInt(200 * (2 - state.eating)));
    }
    #endregion

    #region Lungs
    #region AirBreatherCreature
    static void ILAirBreatherCreatureUpdate(ILContext il)
    {
        try
        {
            ILCursor val = new(il);

            if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[2]
            {
                x => x.MatchLdfld<AirBreatherCreature>("lungs"),
                x => x.MatchLdcR4(0.033333335f)
            }))
            {
                val.Emit(OpCodes.Ldarg_0);
                val.Emit<Creature>(OpCodes.Call, "get_abstractCreature");
                val.EmitDelegate(LungsGainMultiplier);
                val.Emit(OpCodes.Mul);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILAirBreatherCreatureUpdate!");
            }

            if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[2]
            {
                x => x.MatchLdfld<CreatureTemplate>("lungCapacity"),
                x => x.MatchDiv()
            }))
            {
                val.MoveBeforeLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.Emit<Creature>(OpCodes.Call, "get_abstractCreature");
                val.EmitDelegate(LungsLossMultiplier);
                val.Emit(OpCodes.Mul);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILAirBreatherCreatureUpdate!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    #endregion

    static void ILPlayerLungUpdate(ILContext il)
    {
        try
        {
            ILCursor val = new(il);

            if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[2]
            {
                x => x.MatchLdfld<SlugcatStats>("lungsFac"),
                x => x.MatchMul()
            }))
            {
                val.Emit(OpCodes.Ldarg_0);
                val.Emit<Creature>(OpCodes.Call, "get_abstractCreature");
                val.EmitDelegate(LungsLossMultiplier);
                val.Emit(OpCodes.Mul);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerLungUpdate subtract!");
            }

            if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdcI4(240),
                x => x.MatchConvR4(),
                x => x.MatchDiv()
            }))
            {
                val.Emit(OpCodes.Ldarg_0);
                val.Emit<Creature>(OpCodes.Call, "get_abstractCreature");
                val.EmitDelegate(LungsGainMultiplier);
                val.Emit(OpCodes.Mul);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerLungUpdate add!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }

    public static float LungsGainMultiplier(AbstractCreature self)
    {
        if (self.state == null || !healthState.TryGetValue(self.state, out RWState state))
        {
            return 1;
        }

        return Mathf.Max(0.1f, state.breathing);
    }

    public static float LungsLossMultiplier(AbstractCreature self)
    {
        if (self.state == null || !healthState.TryGetValue(self.state, out RWState state))
        {
            return 1;
        }

        return Mathf.Max(0.1f, 2 - state.breathing);
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
        if (obj.sourceObject != null && obj.sourceObject is Snail || !singleExplosion.TryGetValue(obj, out OneTimeUseData data) || data.creatures.Contains(self) || damage <= 0 || self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return;
        }

        data.creatures.Add(self);

        if (obj.sourceObject != null && obj.sourceObject is not FirecrackerPlant)
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

                RWHealthState.Damage(self.State, state, damageType, tempDamage / amount, focusedBodyPart, attackName, attackerName);
            }
        }
    }
    #endregion

    #region Player
    static void ILPlayerGrabUpdate(ILContext il)
    {
        try
        {
            ILCursor val = new(il);

            if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdcI4(15),
                x => x.MatchStfld<Player>("eatCounter")
            }))
            {
                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(GrabUpdateBite);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerGrabUpdate bite!");
            }

            if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[2]
            {
                x => x.MatchLdfld<Player.InputPackage>("y"),
                x => x.MatchBrtrue(out _)
            }))
            {
                val.Emit(OpCodes.Ldarg_0);
                val.Emit(OpCodes.Ldloc_0);
                val.EmitDelegate(GrabUpdate);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerGrabUpdate!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }

    public static void GrabUpdateBite(Player self)
    {
        if (!healthState.TryGetValue(self.State, out RWState state) || RWHealthState.EatingSpeed(state) >= 1)
        {
            return;
        }

        self.eatCounter = Mathf.RoundToInt(Mathf.Lerp(60, 15, RWHealthState.EatingSpeed(state)));
    }

    public static void GrabUpdate(Player self, bool flag2)
    {
        if (!healthState.TryGetValue(self.State, out RWState state) || RWHealthState.EatingSpeed(state) >= 1 || flag2 || self.eatCounter < 40)
        {
            return;
        }

        if (self.eatCounter < Mathf.RoundToInt(Mathf.Lerp(90, 40, RWHealthState.EatingSpeed(state))))
        {
            self.eatCounter++;
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