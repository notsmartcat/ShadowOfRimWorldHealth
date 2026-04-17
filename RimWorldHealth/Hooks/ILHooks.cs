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

        #region BigSpider
        IL.BigSpider.Act += ILBigSpiderAct;

        IL.BigSpider.Update += ILBigSpiderUpdate;
        #endregion

        #region Centipede
        IL.Centipede.BitByPlayer += ILCentipedeBitByPlayer;
        IL.Centipede.Shock += ILCentipedeShock;
        IL.Centipede.Update += ILCentipedeUpdate;
        #endregion

        #region Cicada
        IL.Cicada.Update += ILCicadaUpdate;
        #endregion

        #region Creature
        IL.Creature.HypothermiaUpdate += ILCreatureHypothermiaUpdate;
        IL.Creature.Update += ILCreatureUpdate;
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

    #region BigSpider
    static void ILBigSpiderAct(ILContext il)
    {
        try
        {
            ILCursor val = new(il);

            if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[1]
            {
                x => x.MatchCallvirt<Creature>("Die")
            }))
            {
                val.MoveBeforeLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(BigSpiderAct);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILBigSpiderAct!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    public static void BigSpiderAct(BigSpider self)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return;
        }

        RWBodyPart mainPart = null;

        for (int i = 0; i < state.bodyParts.Count; i++)
        {
            if (state.bodyParts[i] is UpperTorso)
            {
                mainPart = state.bodyParts[i];
                break;
            }
        }

        if (mainPart == null)
        {
            return;
        }

        RWHealthState.Damage(self.State, state, new RWCut(), mainPart.maxHealth, mainPart, "Big Spider - Spew Babies");
    }

    static void ILBigSpiderUpdate(ILContext il)
    {
        try
        {
            ILCursor val = new(il);
            ILLabel target = null;

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[5]
            {
                x => x.MatchLdarg(0),
                x => x.MatchCall<BigSpider>("get_State"),
                x => x.MatchCallvirt<HealthState>("get_health"),
                x => x.MatchLdcR4(0.0f),
                x => x.MatchBgeUn(out target)
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(CreatureHasState);
                val.Emit(OpCodes.Brfalse, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILBigSpiderAct!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    #endregion

    #region Centipede
    static void ILCentipedeBitByPlayer(ILContext il)
    {
        try
        {
            ILCursor val = new(il);
            ILLabel target = null;

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<UpdatableAndDeletable>("room"),
                x => x.MatchLdarg(0)
            }))
            {
                val.MoveAfterLabels();

                target = val.MarkLabel();
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILCentipedeBitByPlayer target!");
            }

            if (val.TryGotoPrev(MoveType.Before, new Func<Instruction, bool>[2]
            {
                x => x.MatchLdarg(0),
                x => x.MatchCallvirt<Creature>("Die"),
            }))
            {
                val.Emit(OpCodes.Ldarg_0);
                val.Emit(OpCodes.Isinst, typeof(Creature));
                val.EmitDelegate(CreatureHasState);
                val.Emit(OpCodes.Brfalse, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILCentipedeBitByPlayer!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }

    static void ILCentipedeShock(ILContext il)
    {
        try
        {
            ILCursor val = new(il);
            ILLabel target = null;

            #region AquaPyro
            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdarg(1),
                x => x.MatchIsinst<Creature>(),
                x => x.MatchBrfalse(out target)
            })) {}
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILCentipedeShock AquaPyro target!");
            }

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdarg(1),
                x => x.MatchIsinst<Player>(),
                x => x.MatchCallvirt<Player>("PyroDeath"),
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.Emit(OpCodes.Ldarg_1);
                val.Emit(OpCodes.Isinst, typeof(Creature));
                val.EmitDelegate(CentipedeShockAquaPyroDeath);
                val.Emit(OpCodes.Brfalse, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILCentipedeShock AquaPyro!");
            }
            #endregion

            #region Aqua
            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[2]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<UpdatableAndDeletable>("room")
            })) 
            {
                val = val.MoveAfterLabels();

                target = val.MarkLabel();
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILCentipedeShock Aqua target!");
            }

            if (val.TryGotoPrev(MoveType.Before, new Func<Instruction, bool>[4]
            {
                x => x.MatchLdarg(1),
                x => x.MatchIsinst<Creature>(),
                x => x.MatchLdarg(0),
                x => x.MatchCall<Creature>("get_mainBodyChunk"),
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.Emit(OpCodes.Ldarg_1);
                val.Emit(OpCodes.Isinst, typeof(Creature));
                val.EmitDelegate(CentipedeShockAqua);
                val.Emit(OpCodes.Brfalse, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILCentipedeShock Aqua!");
            }
            #endregion

            #region Small
            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[4]
            {
                x => x.MatchLdarg(1),
                x => x.MatchIsinst<Creature>(),
                x => x.MatchLdcI4(120),
                x => x.MatchCallvirt<Creature>("Stun"),
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.Emit(OpCodes.Ldarg_1);
                val.Emit(OpCodes.Isinst, typeof(Creature));
                val.EmitDelegate(CentipedeShockSmall);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILCentipedeShock Small!");
            }
            #endregion

            #region Pyro
            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[1]
            {
                x => x.MatchBr(out target),
            })) { }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILCentipedeShock Pyro target!");
            }

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdarg(1),
                x => x.MatchIsinst<Player>(),
                x => x.MatchCallvirt<Player>("PyroDeath"),
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.Emit(OpCodes.Ldarg_1);
                val.Emit(OpCodes.Isinst, typeof(Creature));
                val.EmitDelegate(CentipedeShockPyroDeath);
                val.Emit(OpCodes.Brfalse, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILCentipedeShock Pyro!");
            }
            #endregion

            #region Mass
            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdarg(1),
                x => x.MatchIsinst<Creature>(),
                x => x.MatchCallvirt<Creature>("Die"),
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.Emit(OpCodes.Ldarg_1);
                val.Emit(OpCodes.Isinst, typeof(Creature));
                val.EmitDelegate(CentipedeShock);
                val.Emit(OpCodes.Brfalse, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILCentipedeShock Mass!");
            }
            #endregion

            #region LessMass
            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[4]
            {
                x => x.MatchLdarg(1),
                x => x.MatchIsinst<Creature>(),
                x => x.MatchLdarg(1),
                x => x.MatchCallvirt<PhysicalObject>("get_TotalMass"),
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.Emit(OpCodes.Ldarg_1);
                val.Emit(OpCodes.Isinst, typeof(Creature));
                val.EmitDelegate(CentipedeShockLessMass);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILCentipedeShock LessMass!");
            }
            #endregion
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    public static bool CentipedeShockAquaPyroDeath(Centipede self, Creature shockObj)
    {
        if (shockObj.State == null || !healthState.TryGetValue(shockObj.State, out RWState state))
        {
            return true;
        }

        CentipedeShockDamage(shockObj.State, state, self, shockObj.Submersion > 0f);

        float stun = 200;

        if (!shockObj.dead)
        {
            stun = StunMath(stun, shockObj, Creature.DamageType.Electric);
        }
        else
        {
            ((Player)shockObj).PyroDeath();
        }

        Debug.Log(stun + " stun");

        shockObj.Stun((int)stun);
        self.room.AddObject(new CreatureSpasmer(shockObj, true, shockObj.stun));
        shockObj.LoseAllGrasps();
        if (!shockObj.dead)
        {
            self.Stun(6);
            self.shockGiveUpCounter = Math.Max(self.shockGiveUpCounter, 30);
            self.AI.annoyingCollisions = Math.Min(self.AI.annoyingCollisions / 2, 150);
        }

        return false;
    }
    public static bool CentipedeShockAqua(Centipede self, Creature shockObj)
    {
        if (shockObj.State == null || !healthState.TryGetValue(shockObj.State, out RWState state))
        {
            return true;
        }

        CentipedeShockDamage(shockObj.State, state, self, shockObj.Submersion > 0f);

        float stun = 200;

        if (!shockObj.dead)
        {
            stun = StunMath(stun, shockObj, Creature.DamageType.Electric);
        }

        Debug.Log(stun + " stun");

        shockObj.Stun((int)stun);
        if (!shockObj.dead)
        {
            self.Stun(6);
            self.shockGiveUpCounter = Math.Max(self.shockGiveUpCounter, 30);
            self.AI.annoyingCollisions = Math.Min(self.AI.annoyingCollisions / 2, 150);
        }

        return false;
    }
    public static void CentipedeShockSmall(Centipede self, Creature shockObj)
    {
        if (shockObj.State == null || !healthState.TryGetValue(shockObj.State, out RWState state))
        {
            return;
        }

        CentipedeShockDamage(shockObj.State, state, self, shockObj.Submersion > 0f);
    }
    public static bool CentipedeShockPyroDeath(Centipede self, Creature shockObj)
    {
        if (shockObj.State == null || !healthState.TryGetValue(shockObj.State, out RWState state))
        {
            return true;
        }

        CentipedeShockDamage(shockObj.State, state, self, shockObj.Submersion > 0f);

        float stun = (int)Mathf.Lerp(70f, 120f, self.size);

        if (!shockObj.dead)
        {
            stun = StunMath(stun, shockObj, Creature.DamageType.Electric);
        }
        else
        {
            ((Player)shockObj).PyroDeath();
        }

        Debug.Log(stun + " stun");

        shockObj.Stun((int)stun);
        self.room.AddObject(new CreatureSpasmer(shockObj, true, shockObj.stun));
        shockObj.LoseAllGrasps();
        if (!shockObj.dead)
        {
            self.Stun(6);
            self.shockGiveUpCounter = Math.Max(self.shockGiveUpCounter, 30);
            self.AI.annoyingCollisions = Math.Min(self.AI.annoyingCollisions / 2, 150);
        }

        return false;
    }
    public static bool CentipedeShock(Centipede self, Creature shockObj)
    {
        if (shockObj.State == null || !healthState.TryGetValue(shockObj.State, out RWState state))
        {
            return true;
        }

        CentipedeShockDamage(shockObj.State, state, self, shockObj.Submersion > 0f);

        float stun = (int)Mathf.Lerp(70f, 120f, self.size);

        if (!shockObj.dead)
        {
            stun = StunMath(stun, shockObj, Creature.DamageType.Electric);
        }

        Debug.Log(stun + " stun");

        shockObj.Stun((int)stun);
        self.room.AddObject(new CreatureSpasmer(shockObj, true, shockObj.stun));
        shockObj.LoseAllGrasps();
        if (!shockObj.dead)
        {
            self.Stun(6);
            self.shockGiveUpCounter = Math.Max(self.shockGiveUpCounter, 30);
            self.AI.annoyingCollisions = Math.Min(self.AI.annoyingCollisions / 2, 150);
        }

        return false;
    }
    public static void CentipedeShockLessMass(Centipede self, Creature shockObj)
    {
        if (shockObj.State == null || !healthState.TryGetValue(shockObj.State, out RWState state))
        {
            return;
        }

        CentipedeShockDamage(shockObj.State, state, self, shockObj.Submersion > 0f);

        return;
    }

    static void ILCentipedeUpdate(ILContext il)
    {
        try
        {
            ILCursor val = new(il);
            ILLabel target = null;

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[6]
            {
                x => x.MatchLdarg(0),
                x => x.MatchCall<Creature>("get_State"),
                x => x.MatchIsinst(typeof(HealthState)),
                x => x.MatchCallvirt<HealthState>("get_health"),
                x => x.MatchLdcR4(0.0f),
                x => x.MatchBgtUn(out target),
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(CreatureHasState);
                val.Emit(OpCodes.Brfalse_S, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILCentipedeShock AquaPyro!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    #endregion

    #region Cicada
    static void ILCicadaUpdate(ILContext il)
    {
        try
        {
            ILCursor val = new(il);
            ILLabel target = null;

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[6]
            {
                x => x.MatchLdarg(0),
                x => x.MatchCall<Creature>("get_State"),
                x => x.MatchIsinst(typeof(HealthState)),
                x => x.MatchCallvirt<HealthState>("get_health"),
                x => x.MatchLdcR4(0.0f),
                x => x.MatchBgtUn(out target),
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(CreatureHasState);
                val.Emit(OpCodes.Brfalse_S, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILCicadaUpdate!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    #endregion

    #region Creature
    static void ILCreatureHypothermiaUpdate(ILContext il)
    {
        try
        {
            ILCursor val = new(il);
            ILLabel target = null;

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[4]
            {
                x => x.MatchLdarg(0),
                x => x.MatchCall<Creature>("get_Hypothermia"),
                x => x.MatchLdcR4(1f),
                x => x.MatchBltUn(out target),
            })) {}
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILCreatureHypothermiaUpdate target!");
            }

            if (val.TryGotoPrev(MoveType.Before, new Func<Instruction, bool>[4]
            {
                x => x.MatchLdarg(0),
                x => x.MatchCall<Creature>("get_Hypothermia"),
                x => x.MatchLdcR4(0.8f),
                x => x.MatchBltUn(out target),
            }))
            {
                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(CreatureHypothermiaUpdate);

                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(CreatureHasState);
                val.Emit(OpCodes.Brfalse_S, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILCreatureHypothermiaUpdate!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    public static void CreatureHypothermiaUpdate(Creature self)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return;
        }

        RWHypothermia hypothermia = null;

        for (int i = 0; i < state.wholeBodyAfflictions.Count; i++)
        {
            if (state.wholeBodyAfflictions[i] is RWHypothermia tempHypothermia)
            {
                hypothermia = tempHypothermia;
                break;
            }
        }

        if (self.Hypothermia < 0.04f)
        {
            if (hypothermia != null)
            {
                state.wholeBodyAfflictions.Remove(hypothermia);
                state.updateCapacities = true;
            }
        }
        else
        {
            if (hypothermia != null)
            {
                hypothermia.tendQuality = Math.Min(1, self.Hypothermia);
            }
            else
            {
                state.wholeBodyAfflictions.Add(new RWHypothermia(self.State, null, Math.Min(1, self.Hypothermia)));
                state.updateCapacities = true;
            }
        }
    }

    static void ILCreatureUpdate(ILContext il)
    {
        try
        {
            ILCursor val = new(il);
            ILLabel target = null;

            #region LethalWater
            if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[6]
            {
                x => x.MatchLdarg(0),
                x => x.MatchIsinst(typeof(Player)),
                x => x.MatchLdfld<Player>("pyroJumpCounter"),
                x => x.MatchLdsfld<MoreSlugcats.MoreSlugcats>("cfgArtificerExplosionCapacity"),
                x => x.MatchCallvirt(typeof(Configurable<int>).GetProperty("Value").GetGetMethod()), //:monkcurious:
                x => x.MatchBlt(out target)
            })) 
            {
                val.MoveBeforeLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(CreatureHasState);
                val.Emit(OpCodes.Brfalse, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILCreatureUpdate LethalArti!");
            }

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdarg(0),
                x => x.MatchIsinst(typeof(Player)),
                x => x.MatchCallvirt<Creature>("Die")
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(CreatureHasState);
                val.Emit(OpCodes.Brfalse_S, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILCreatureUpdate LethalPlayer!");
            }

            if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdarg(0),
                x => x.MatchCall<Creature>("get_dead"),
                x => x.MatchBrtrue(out _),
            }))
            {
                val.MoveBeforeLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(CreatureHasState);
                val.Emit(OpCodes.Brfalse_S, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILCreatureUpdate Violence!");
            }

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[2]
            {
                x => x.MatchLdarg(0),
                x => x.MatchCallvirt<Creature>("Die")
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(CreatureHasState);
                val.Emit(OpCodes.Brfalse_S, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILCreatureUpdate Lethal!");
            }
            #endregion

            if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[6]
            {
                x => x.MatchLdloc(10),
                x => x.MatchLdcR4(1),
                x => x.MatchBltUn(out target),
                x => x.MatchLdarg(0),
                x => x.MatchCall<Creature>("get_dead"),
                x => x.MatchBrtrue(out _)
            }))
            {
                val.MoveBeforeLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(CreatureHasState);
                val.Emit(OpCodes.Brfalse_S, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILCreatureUpdate Poison!");
            }

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdarg(0),
                x => x.MatchCall<Creature>("get_dead"),
                x => x.MatchBrtrue(out target)
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(CreatureHasState);
                val.Emit(OpCodes.Brfalse_S, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILCreatureUpdate Bleed!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    #endregion

    #region Explosion
    static void ILExplosionUpdate(ILContext il)
    {
        try
        {
            ILCursor val = new(il);
            ILLabel target = null;

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[4]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<Explosion>("minStun"),
                x => x.MatchLdcR4(0.0f),
                x => x.MatchBleUn(out _)
            }))
            {
                val.MoveAfterLabels();

                target = val.MarkLabel();
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILExplosionUpdate target!");
            }

            if (val.TryGotoPrev(MoveType.Before, new Func<Instruction, bool>[9]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<UpdatableAndDeletable>("room"),
                x => x.MatchLdfld<Room>("physicalObjects"),
                x => x.MatchLdloc(2),
                x => x.MatchLdelemRef(),
                x => x.MatchLdloc(3),
                x => x.MatchCallvirt(typeof(List<PhysicalObject>).GetMethod("get_Item")),
                x => x.MatchIsinst(typeof(Creature)),
                x => x.MatchLdnull()
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
                val.Emit(OpCodes.Brfalse, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILExplosionUpdate!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    public static bool ExplosionUpdate(Explosion obj, Creature self, float damage)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state) || !singleExplosion.TryGetValue(obj, out OneTimeUseData data) || data.creatures.Contains(self) || damage <= 0)
        {
            return true;
        }

        data.creatures.Add(self);

        bool isSuper = ModManager.MSC && obj.sourceObject != null && (obj.sourceObject is MoreSlugcats.EnergyCell || obj.sourceObject is MoreSlugcats.SingularityBomb);

        damage *= BombDamageMultiplier(true, isSuper);

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

        return false;
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

    public static bool CreatureHasState(Creature self)
    {
        return self.State == null || !healthState.TryGetValue(self.State, out _);
    }

    public static int StunMath(float stun, Creature self, Creature.DamageType type)
    {
        Debug.Log("Pre-Stun " + stun);

        stun = (2 * 30f + stun) / self.Template.baseStunResistance;

        if (self.State is HealthState)
        {
            stun *= 1.5f;
        }
        if (type.Index != -1 && self.Template.damageRestistances[type.Index, 1] > 0f)
        {
            stun /= self.Template.damageRestistances[type.Index, 1];
        }

        Debug.Log("Post-Stun " + stun);

        return (int)stun;
    }
}
/*
class ILHooks
{
	public static bool ShadowOfLizardUpdate(Creature self)
    {
        return true;
    }
	public static void ShadowOfLizard(Creature self)
    {
        return true;
    }
}
*/