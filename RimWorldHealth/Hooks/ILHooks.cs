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
        #region BaseGame
        #region AbstractCreature
        IL.AbstractCreature.IsEnteringDen += ILAbstractCreatureIsEnteringDen;
        #endregion

        #region AirBreatherCreature
        IL.AirBreatherCreature.Update += ILAirBreatherCreatureUpdate;
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

        #region DropBug
        IL.DropBug.Update += ILDropBugUpdate;
        #endregion

        #region EggBug
        IL.EggBug.Update += ILEggBugUpdate;
        #endregion

        #region Explosion
        IL.Explosion.Update += ILExplosionUpdate;
        #endregion

        #region FlareBomb
        IL.FlareBomb.Update += ILFlareBombUpdate;
        #endregion

        #region Fly
        IL.Fly.BitByPlayer += ILFlyBitByPlayer;
        IL.Fly.Update += ILFlyUpdate;
        #endregion

        #region Hazer
        IL.Hazer.Update += ILHazerUpdate;
        #endregion

        #region InsectoidCreature
        IL.InsectoidCreature.Update += ILInsectoidCreatureUpdate;
        #endregion

        #region LanternMouse
        IL.LanternMouse.Update += ILLanternMouseUpdate;
        #endregion

        #region Leech
        IL.Leech.Attached += ILLeechAttached;
        #endregion

        #region LocustSwarm
        IL.LocustSystem.Swarm.Update += ILLocustSwarmUpdate;
        #endregion

        #region Player
        IL.Player.GrabUpdate += ILPlayerGrabUpdate;
        IL.Player.LungUpdate += ILPlayerLungUpdate;
        IL.Player.TerrainImpact += ILPlayerTerrainImpact;
        IL.Player.Tongue.Update += ILPlayerTongueUpdate;
        #endregion

        #region RoomRain
        IL.RoomRain.ThrowAroundObjects += ILRoomRainThrowAroundObjects;
        #endregion

        #region Scavenger
        IL.Scavenger.Violence += ILScavengerViolence;
        #endregion

        #region SmallNeedleWorm
        IL.SmallNeedleWorm.BitByPlayer += ILSmallNeedleWormBitByPlayer;
        IL.SmallNeedleWorm.Scream += ILSmallNeedleWormScream;
        IL.SmallNeedleWorm.Update += ILSmallNeedleWormUpdate;
        #endregion

        #region Spear
        IL.Spear.HitSomething += ILSpearHitSomething;
        #endregion

        #region Spider
        IL.Spider.Attached += ILSpiderAttached;
        #endregion

        #region SporeCloud
        IL.SporeCloud.Update += ILSporeCloudUpdate;
        #endregion

        #region OracleBehavior
        IL.SSOracleBehavior.ThrowOutBehavior.Update += ILThrowOutBehaviorUpdate;
        #endregion

        #region TubeWorm
        IL.TubeWorm.Update += ILTubeWormUpdate;
        IL.TubeWorm.Tongue.Update += ILTubeWormTongueUpdate;
        #endregion

        #region VultureGrub
        IL.VultureGrub.Update += ILVultureGrubUpdate;
        #endregion

        #region Weapon
        IL.Weapon.HitSomethingWithoutStopping += ILWeaponHitSomethingWithoutStopping;
        #endregion

        #region ZapCoil
        IL.ZapCoil.Update += ILZapCoilUpdate;
        #endregion
        #endregion

        #region MoreSlugcats
        #region BigJellyFish
        IL.MoreSlugcats.BigJellyFish.Collide += ILBigJellyFishCollide;
        #endregion

        #region EnergyCell
        IL.MoreSlugcats.EnergyCell.Explode += ILEnergyCellExplode;
        #endregion

        #region LillyPuck
        IL.MoreSlugcats.LillyPuck.HitSomething += ILLillyPuckHitSomething;
        #endregion

        #region SingularityBomb
        IL.MoreSlugcats.SingularityBomb.Explode += ILSingularityBombExplode;
        #endregion
        #endregion

        #region Watcher
        #region ARZapper
        IL.Watcher.ARZapper.ZapperContact += ILARZapperZapperContact;
        #endregion

        #region Barnacle
        IL.Watcher.Barnacle.BitByPlayer += ILBarnacleBitByPlayer;
        #endregion

        #region BigMoth
        IL.Watcher.BigMoth.Update += ILBigMothUpdate;
        #endregion

        #region Frog
        IL.Watcher.Frog.BitByPlayer += ILFrogBitByPlayer;
        #endregion

        #region Rat
        IL.Watcher.Rat.BitByPlayer += ILRatBitByPlayer;
        #endregion

        #region Sandstorm
        IL.Watcher.Sandstorm.AffectObjects += ILSandstormAffectObjects;
        #endregion

        #region Tardigrade
        IL.Watcher.Tardigrade.BitByPlayer += ILTardigradeBitByPlayer;
        #endregion
        #endregion
    }

    #region BaseGame
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

        CentipedeShockDamage(shockObj.State, state, self, shockObj.Submersion > 0f, true);

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

        return false;
    }
    public static bool CentipedeShockAqua(Centipede self, Creature shockObj)
    {
        if (shockObj.State == null || !healthState.TryGetValue(shockObj.State, out RWState state))
        {
            return true;
        }

        CentipedeShockDamage(shockObj.State, state, self, shockObj.Submersion > 0f, true);

        float stun = 200;

        if (!shockObj.dead)
        {
            stun = StunMath(stun, shockObj, Creature.DamageType.Electric);
        }

        Debug.Log(stun + " stun");

        shockObj.Stun((int)stun);

        return false;
    }
    public static void CentipedeShockSmall(Centipede self, Creature shockObj)
    {
        if (shockObj.State == null || !healthState.TryGetValue(shockObj.State, out RWState state))
        {
            return;
        }

        CentipedeShockDamage(shockObj.State, state, self, shockObj.Submersion > 0f, false);
    }
    public static bool CentipedeShockPyroDeath(Centipede self, Creature shockObj)
    {
        if (shockObj.State == null || !healthState.TryGetValue(shockObj.State, out RWState state))
        {
            return true;
        }

        CentipedeShockDamage(shockObj.State, state, self, shockObj.Submersion > 0f, true);

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

        return false;
    }
    public static bool CentipedeShock(Centipede self, Creature shockObj)
    {
        if (shockObj.State == null || !healthState.TryGetValue(shockObj.State, out RWState state))
        {
            return true;
        }

        CentipedeShockDamage(shockObj.State, state, self, shockObj.Submersion > 0f, true);

        float stun = (int)Mathf.Lerp(70f, 120f, self.size);

        if (!shockObj.dead)
        {
            stun = StunMath(stun, shockObj, Creature.DamageType.Electric);
        }

        Debug.Log(stun + " stun");

        shockObj.Stun((int)stun);
        self.room.AddObject(new CreatureSpasmer(shockObj, true, shockObj.stun));
        shockObj.LoseAllGrasps();

        return false;
    }
    public static void CentipedeShockLessMass(Centipede self, Creature shockObj)
    {
        if (shockObj.State == null || !healthState.TryGetValue(shockObj.State, out RWState state))
        {
            return;
        }

        CentipedeShockDamage(shockObj.State, state, self, shockObj.Submersion > 0f, false);

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
                val.EmitDelegate(CreatureUpdateLethalWater);
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
                val.EmitDelegate(CreatureUpdateLethalWater);
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
                val.EmitDelegate(CreatureUpdateLethalWater);
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
                val.EmitDelegate(CreatureUpdateLethalWater);
                val.Emit(OpCodes.Brfalse_S, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILCreatureUpdate Lethal!");
            }
            #endregion

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[6]
            {
                x => x.MatchLdloc(10),
                x => x.MatchLdcR4(1),
                x => x.MatchBltUn(out target),
                x => x.MatchLdarg(0),
                x => x.MatchCall<Creature>("get_dead"),
                x => x.MatchBrtrue(out _)
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(CreatureUpdatePoison);
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
    public static bool CreatureUpdateLethalWater(Creature self)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return true;
        }

        RWBodyPart focusedBodyPart = GetHitBodyPart(state);
        Debug.Log("CentipedeShockDamage focusedBodyPart is " + focusedBodyPart);

        RWHealthState.Damage(self.State, state, new RWAcidBurn(), UnityEngine.Random.Range(8.2f, 18.8f), focusedBodyPart, "Acidic water");

        self.firstChunk.vel += Vector2.ClampMagnitude(new Vector2(0f, 5f) / self.firstChunk.mass, 10f);

        self.Stun(StunMath(0.1f, self, Creature.DamageType.Explosion));

        return false;
    }
    public static bool CreatureUpdatePoison(Creature self)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return true;
        }

        SetToxicBuildup(state, self);

        return false;
    }
    #endregion

    #region DropBug
    static void ILDropBugUpdate(ILContext il)
    {
        try
        {
            ILCursor val = new(il);
            ILLabel target = null;

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[5]
            {
                x => x.MatchLdarg(0),
                x => x.MatchCall<DropBug>("get_State"),
                x => x.MatchCallvirt<HealthState>("get_health"),
                x => x.MatchLdcR4(0.0f),
                x => x.MatchBgeUn(out target),
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(CreatureHasState);
                val.Emit(OpCodes.Brfalse_S, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILDropBugUpdate Bleed!");
            }

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[5]
            {
                x => x.MatchLdarg(0),
                x => x.MatchCall<DropBug>("get_State"),
                x => x.MatchCallvirt<HealthState>("get_health"),
                x => x.MatchLdcR4(0.0f),
                x => x.MatchBleUn(out target),
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(CreatureHasState);
                val.Emit(OpCodes.Brfalse_S, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILDropBugUpdate Heal!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    #endregion

    #region EggBug
    static void ILEggBugUpdate(ILContext il)
    {
        try
        {
            ILCursor val = new(il);
            ILLabel target = null;

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdarg(0),
                x => x.MatchCall<Creature>("get_dead"),
                x => x.MatchBrtrue(out target),
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(CreatureHasState);
                val.Emit(OpCodes.Brfalse, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILEggBugUpdate Bleed!");
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

    #region FlareBomb
    static void ILFlareBombUpdate(ILContext il)
    {
        try
        {
            ILCursor val = new(il);
            ILLabel target = null;

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[1]
            {
                x => x.MatchCallvirt<Creature>("Stun"),
            })) {}
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILFlareBombUpdate skip!");
            }

            if (val.TryGotoPrev(MoveType.Before, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<UpdatableAndDeletable>("room"),
                x => x.MatchCallvirt<Room>("get_abstractRoom"),
            }))
            {
                val.MoveAfterLabels();

                target = val.MarkLabel();
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILFlareBombUpdate target!");
            }

            if (val.TryGotoPrev(MoveType.Before, new Func<Instruction, bool>[1]
            {
                x => x.MatchStfld<InsectoidCreature>("poison"),
            })) { }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILFlareBombUpdate skip2!");
            }

            if (val.TryGotoPrev(MoveType.Before, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<UpdatableAndDeletable>("room"),
                x => x.MatchCallvirt<Room>("get_abstractRoom"),
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.Emit<UpdatableAndDeletable>(OpCodes.Ldfld, "room");
                val.Emit<Room>(OpCodes.Callvirt, "get_abstractRoom");
                val.Emit<AbstractRoom>(OpCodes.Ldfld, "creatures");
                val.Emit(OpCodes.Ldloc_0);
                val.Emit(OpCodes.Callvirt, typeof(List<AbstractCreature>).GetMethod("get_Item"));
                val.Emit<AbstractCreature>(OpCodes.Callvirt, "get_realizedCreature");
                val.EmitDelegate(FlareBombUpdate);
                val.Emit(OpCodes.Brfalse_S, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILFlareBombUpdate!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    public static bool FlareBombUpdate(Creature self)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out _))
        {
            return true;
        }

        (self as BigSpider).poison = UnityEngine.Random.value + 0.5f;

        return false;
    }
    #endregion

    #region Fly
    static void ILFlyBitByPlayer(ILContext il)
    {
        try
        {
            ILCursor val = new(il);
            ILLabel target = null;

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdarg(0),
                x => x.MatchCall<Creature>("get_dead"),
                x => x.MatchBrtrue(out target),
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(CreatureHasState);
                val.Emit(OpCodes.Brfalse, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILFlyBitByPlayer!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }

    static void ILFlyUpdate(ILContext il)
    {
        try
        {
            ILCursor val = new(il);
            ILLabel target = null;

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[4]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<Fly>("drown"),
                x => x.MatchLdcR4(1),
                x => x.MatchBneUn(out _),
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(FlyUpdateDrown);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILFlyUpdate Drown!");
            }

            if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[1]
            {
                x => x.MatchBle(out target)
            }))
            {
                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(CreatureHasState);
                val.Emit(OpCodes.Brfalse, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILFlyUpdate Crush!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    public static void FlyUpdateDrown(Creature self)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return;
        }

        RWAirInLungs airInLungs = null;
        float lungs = 1 - ((Fly)self).drown;

        for (int i = 0; i < state.wholeBodyAfflictions.Count; i++)
        {
            if (state.wholeBodyAfflictions[i] is RWAirInLungs tempAirInLungs)
            {
                airInLungs = tempAirInLungs;
                break;
            }
        }

        if (lungs >= 1)
        {
            if (airInLungs != null)
            {
                state.wholeBodyAfflictions.Remove(airInLungs);
            }
        }
        else
        {
            if (airInLungs != null)
            {
                airInLungs.tendQuality = lungs;
            }
            else
            {
                state.wholeBodyAfflictions.Add(new RWAirInLungs(self.State, null, lungs));
            }
        }
    }
    #endregion

    #region Hazer
    static void ILHazerUpdate(ILContext il)
    {
        try
        {
            ILCursor val = new(il);
            ILLabel target = null;

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[2]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdflda<Hazer>("sprayStuckPos")
            }))
            {
                val.MoveAfterLabels();

                target = val.MarkLabel();
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILFlareBombUpdate target!");
            }

            if (val.TryGotoPrev(MoveType.Before, new Func<Instruction, bool>[2]
            {
                x => x.MatchLdarg(0),
                x => x.MatchCallvirt<Creature>("Die")
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(CreatureHasState);
                val.Emit(OpCodes.Brfalse, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILFlyBitByPlayer!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    #endregion

    #region InsectoidCreature
    static void ILInsectoidCreatureUpdate(ILContext il)
    {
        try
        {
            ILCursor val = new(il);
            ILLabel target = null;

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[4]
            {
                x => x.MatchLdarg(0),
                x => x.MatchCall<Creature>("get_State"),
                x => x.MatchIsinst(typeof(HealthState)),
                x => x.MatchBrfalse(out target),
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(CreatureHasState);
                val.Emit(OpCodes.Brfalse_S, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILInsectoidCreatureUpdate Damage!");
            }

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[4]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<InsectoidCreature>("poison"),
                x => x.MatchLdcR4(1),
                x => x.MatchBltUn(out target),
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(InsectoidCreatureUpdatePoison);
                val.Emit(OpCodes.Brfalse_S, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILInsectoidCreatureUpdate Poison!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    public static bool InsectoidCreatureUpdatePoison(Creature self)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return true;
        }

        RWAirInLungs airInLungs = null;
        float lungs = 1 - ((Fly)self).drown;

        for (int i = 0; i < state.wholeBodyAfflictions.Count; i++)
        {
            if (state.wholeBodyAfflictions[i] is RWAirInLungs tempAirInLungs)
            {
                airInLungs = tempAirInLungs;
                break;
            }
        }

        if (lungs >= 1)
        {
            if (airInLungs != null)
            {
                state.wholeBodyAfflictions.Remove(airInLungs);
            }
        }
        else
        {
            if (airInLungs != null)
            {
                airInLungs.tendQuality = lungs;
            }
            else
            {
                state.wholeBodyAfflictions.Add(new RWAirInLungs(self.State, null, lungs));
            }
        }

        return false;
    }
    #endregion

    #region LanternMouse
    static void ILLanternMouseUpdate(ILContext il)
    {
        try
        {
            ILCursor val = new(il);
            ILLabel target = null;

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdarg(0),
                x => x.MatchCall<Creature>("get_dead"),
                x => x.MatchBrtrue(out target),
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(CreatureHasState);
                val.Emit(OpCodes.Brfalse_S, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILLanternMouseUpdate!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    #endregion

    #region Leech
    static void ILLeechAttached(ILContext il)
    {
        try
        {
            ILCursor val = new(il);
            ILLabel target = null;

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[6]
            {
                x => x.MatchLdloc(0),
                x => x.MatchCallvirt<BodyChunk>("get_owner"),
                x => x.MatchIsinst(typeof(Player)),
                x => x.MatchCallvirt<Creature>("get_stun"),
                x => x.MatchLdcI4(60),
                x => x.MatchBle(out target)
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldloc_0);
                val.Emit<BodyChunk>(OpCodes.Callvirt, "get_owner");
                val.EmitDelegate(LeechAttached);
                val.Emit(OpCodes.Brfalse_S, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILLeechAttached!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    public static bool LeechAttached(Creature self)
    {
        if (self.dead || self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return true;
        }

        state.bloodLoss += 0.01f;
        state.updateCapacities = true;

        return false;
    }
    #endregion

    #region LocustSwarm
    static void ILLocustSwarmUpdate(ILContext il)
    {
        try
        {
            ILCursor val = new(il);
            ILLabel target = null;

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[4]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<LocustSystem.Swarm>("killCounter"),
                x => x.MatchLdcI4(80),
                x => x.MatchBle(out target)
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldloc_0);
                val.Emit<LocustSystem.Swarm>(OpCodes.Ldfld, "target");
                val.EmitDelegate(LocustSwarmUpdate);
                val.Emit(OpCodes.Brfalse_S, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILLocustSwarmUpdate!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    public static bool LocustSwarmUpdate(Creature self)
    {
        if (self.dead || self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return true;
        }

        RWHealthState.Damage(self.State, state, new RWBite(), UnityEngine.Random.Range(0.8f, 1.2f), GetHitBodyPart(state), "Locust swarm - Mandibles");

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
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state) || RWHealthState.EatingSpeed(state) >= 1)
        {
            return;
        }

        self.eatCounter = Mathf.RoundToInt(Mathf.Lerp(60, 15, RWHealthState.EatingSpeed(state)));
    }
    public static void GrabUpdate(Player self, bool flag2)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state) || RWHealthState.EatingSpeed(state) >= 1 || flag2 || self.eatCounter < 40)
        {
            return;
        }

        if (self.eatCounter < Mathf.RoundToInt(Mathf.Lerp(90, 40, RWHealthState.EatingSpeed(state))))
        {
            self.eatCounter++;
        }
    }

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

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[2]
            {
                x => x.MatchLdarg(0),
                x => x.MatchCallvirt<Creature>("Die")
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(PlayerLungUpdateArti);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerLungUpdate Die!");
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
    public static void PlayerLungUpdateArti(Player self)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return;
        }

        for (int i = 0; i < state.bodyParts.Count; i++)
        {
            if (state.bodyParts[i] is Lung part && !IsDestroyed(part))
            {
                RWHealthState.Damage(self.State, state, new RWBomb(), 999999f, part, "Artificer - Explosion", "Artificer");
            }
        }
    }

    static void ILPlayerTerrainImpact(ILContext il)
    {
        try
        {
            ILCursor val = new(il);
            ILLabel target = null;

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[2]
            {
                x => x.MatchStfld<Creature>("enteringShortCut"),
                x => x.MatchBr(out target)
            }))
            {
                val.MoveAfterLabels();

                target = val.MarkLabel();
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerTerrainImpact death target!");
            }

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<UpdatableAndDeletable>("room"),
                x => x.MatchLdsfld<SoundID>("Slugcat_Terrain_Impact_Death")
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.Emit(OpCodes.Ldarg_1);
                val.EmitDelegate(PlayerTerrainImpactDeath);
                val.Emit(OpCodes.Brfalse, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerTerrainImpact death!");
            }

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[2]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdarg(3)
            }))
            {
                val.MoveAfterLabels();

                target = val.MarkLabel();
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerTerrainImpact hard target!");
            }

            if (val.TryGotoPrev(MoveType.Before, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<UpdatableAndDeletable>("room"),
                x => x.MatchLdsfld<SoundID>("Slugcat_Terrain_Impact_Hard")
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.Emit(OpCodes.Ldarg_1);
                val.EmitDelegate(PlayerTerrainImpactHard);
                val.Emit(OpCodes.Brfalse, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerTerrainImpact hard!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    public static bool PlayerTerrainImpactDeath(Player self, int hitChunk = 0)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return true;
        }

        RWBodyPart part = GetHitBodyPart(state, self.bodyChunks[hitChunk], null, false, true);

        part ??= GetHitBodyPart(state);

        RWHealthState.Damage(self.State, state, new RWBlunt(), UnityEngine.Random.Range(9.2f, 12.8f), part, "Ground Impact");

        if (self.dead)
        {
            self.room.PlaySound(SoundID.Slugcat_Terrain_Impact_Death, self.mainBodyChunk);
        }
        else
        {
            self.room.PlaySound(SoundID.Slugcat_Terrain_Impact_Hard, self.mainBodyChunk);
            self.Stun(140);
        }

        return false;
    }
    public static bool PlayerTerrainImpactHard(Player self, int hitChunk = 0)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return true;
        }

        RWBodyPart part = GetHitBodyPart(state, self.bodyChunks[hitChunk], null, false, true);

        part ??= GetHitBodyPart(state);

        RWHealthState.Damage(self.State, state, new RWBlunt(), UnityEngine.Random.Range(1.2f, 4.8f), part, "Ground Impact");

        if (self.dead)
        {
            self.room.PlaySound(SoundID.Slugcat_Terrain_Impact_Death, self.mainBodyChunk);
        }
        else
        {
            self.room.PlaySound(SoundID.Slugcat_Terrain_Impact_Hard, self.mainBodyChunk);
        }

        return false;
    }

    static void ILPlayerTongueUpdate(ILContext il)
    {
        ILCursor val = new(il);
        ILLabel target = null;

        if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[4]
        {
            x => x.MatchLdloc(0),
            x => x.MatchLdcI4(1),
            x => x.MatchAdd(),
            x => x.MatchStloc(0)
        }))
        {
            val.MoveAfterLabels();

            target = val.MarkLabel();
        }
        else
        {
            RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerTongueUpdate target!");
        }

        if (val.TryGotoPrev(MoveType.Before, new Func<Instruction, bool>[3]
        {
            x => x.MatchLdarg(0),
            x => x.MatchLdfld<Player.Tongue>("player"),
            x => x.MatchCallvirt<Creature>("Die")
        }))
        {
            val.Emit(OpCodes.Ldarg_0);
            val.EmitDelegate(PlayerTongueUpdate);
            val.Emit(OpCodes.Brfalse_S, target);
        }
        else
        {
            RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerTongueUpdate!");
        }
    }
    public static bool PlayerTongueUpdate(Player self)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return true;
        }

        for (int i = 0; i < state.bodyParts.Count; i++)
        {
            if (state.bodyParts[i] is Tongue part && !IsDestroyed(part))
            {
                RWHealthState.Damage(self.State, state, new RWElectricBurn(), UnityEngine.Random.Range(7.2f, 12.8f), part, "Zap-Coil");
            }
        }

        return false;
    }
    #endregion

    #region RoomRain
    static void ILRoomRainThrowAroundObjects(ILContext il)
    {
        ILCursor val = new(il);
        ILLabel target = null;

        if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[1]
        {
            x => x.MatchCallvirt<Creature>("Stun")
        })) {}
        else
        {
            RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILRoomRainThrowAroundObjects skip!");
        }

        if (val.TryGotoPrev(MoveType.Before, new Func<Instruction, bool>[3]
        {
            x => x.MatchLdloc(3),
            x => x.MatchCallvirt<BodyChunk>("get_owner"),
            x => x.MatchIsinst(typeof(Creature))
        }))
        {
            val.Emit(OpCodes.Ldloc_3);
            val.Emit<BodyChunk>(OpCodes.Callvirt, "get_owner");
            val.Emit(OpCodes.Isinst, typeof(Creature));
            val.EmitDelegate(RoomRainThrowAroundObjects);
        }
        else
        {
            RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILRoomRainThrowAroundObjects!");
        }

        if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[3]
        {
            x => x.MatchLdloc(5),
            x => x.MatchLdcR4(0.5f),
            x => x.MatchBleUn(out target)
        }))
        {
            val.MoveAfterLabels();

            target = val.MarkLabel();
        }
        else
        {
            RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILRoomRainThrowAroundObjects Die target!");
        }

        if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[4]
        {
            x => x.MatchLdloc(3),
            x => x.MatchCallvirt<BodyChunk>("get_owner"),
            x => x.MatchIsinst(typeof(Creature)),
            x => x.MatchCallvirt<Creature>("Die")
        }))
        {
            val.Emit(OpCodes.Ldloc_3);
            val.Emit<BodyChunk>(OpCodes.Callvirt, "get_owner");
            val.Emit(OpCodes.Isinst, typeof(Creature));
            val.EmitDelegate(RoomRainThrowAroundObjectsDie);
            val.Emit(OpCodes.Brfalse_S, target);
        }
        else
        {
            RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILRoomRainThrowAroundObjects Die!");
        }
    }
    public static void RoomRainThrowAroundObjects(Player self)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return;
        }

        RWHealthState.Damage(self.State, state, new RWBlunt(), UnityEngine.Random.Range(0.8f, 3.2f), GetHitBodyPart(state, null, null, false, true), "Rain");
    }
    public static bool RoomRainThrowAroundObjectsDie(Player self)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return true;
        }

        RWHealthState.Damage(self.State, state, new RWBlunt(), UnityEngine.Random.Range(14.2f, 24.8f), GetHitBodyPart(state, null, null, false, true), "Rain");

        return false;
    }
    #endregion

    #region Scavenger
    static void ILScavengerViolence(ILContext il)
    {
        ILCursor val = new(il);
        ILLabel target = null;

        if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[3]
        {
            x => x.MatchLdarg(6),
            x => x.MatchLdcR4(0.5f),
            x => x.MatchBleUn(out target)
        }))
        {
            val.Emit(OpCodes.Ldarg_0);
            val.EmitDelegate(CreatureHasState);
            val.Emit(OpCodes.Brfalse_S, target);
        }
        else
        {
            RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILScavengerViolence!");
        }
    }
    #endregion

    #region SmallNeedleWorm
    static void ILSmallNeedleWormBitByPlayer(ILContext il)
    {
        try
        {
            ILCursor val = new(il);
            ILLabel target = null;

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[2]
            {
                x => x.MatchLdcI4(0),
                x => x.MatchStloc(0)
            }))
            {
                val.MoveAfterLabels();

                target = val.MarkLabel();
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILSmallNeedleWormBitByPlayer target!");
            }

            if (val.TryGotoPrev(MoveType.Before, new Func<Instruction, bool>[2]
            {
                x => x.MatchLdarg(0),
                x => x.MatchCallvirt<Creature>("Die"),
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(CreatureHasState);
                val.Emit(OpCodes.Brfalse_S, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILSmallNeedleWormBitByPlayer!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }

    static void ILSmallNeedleWormScream(ILContext il)
    {
        try
        {
            ILCursor val = new(il);
            //ILLabel target = null;

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[2]
            {
                x => x.MatchLdarg(0),
                x => x.MatchCallvirt<Creature>("Die"),
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(SmallNeedleWormScream);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILSmallNeedleWormScream!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    public static void SmallNeedleWormScream(Player self)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState _))
        {
            return;
        }

        //Add heartAttack when I add the Heart attack
    }

    static void ILSmallNeedleWormUpdate(ILContext il)
    {
        try
        {
            ILCursor val = new(il);
            ILLabel target = null;

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdloc(1),
                x => x.MatchLdcR4(0.2f),
                x => x.MatchBgtUn(out target)
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(CreatureHasState);
                val.Emit(OpCodes.Brfalse_S, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILSmallNeedleWormUpdate!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    #endregion

    #region Spear
    static void ILSpearHitSomething(ILContext il)
    {
        try
        {
            ILCursor val = new(il);
            ILLabel target = null;

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[2]
            {
                x => x.MatchLdloc(4),
                x => x.MatchCallvirt<Creature>("Die")
            })) {}
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILSpearHitSomething skip!");
            }

            if (val.TryGotoPrev(MoveType.Before, new Func<Instruction, bool>[2]
            {
                x => x.MatchLdsfld<ModManager>("MSC"),
                x => x.MatchBrfalse(out target),
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_1);
                val.Emit<SharedPhysics.CollisionResult>(OpCodes.Ldfld, "obj");
                val.Emit(OpCodes.Isinst, typeof(Creature));
                val.EmitDelegate(CreatureHasState);
                val.Emit(OpCodes.Brfalse, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILSpearHitSomething!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    #endregion

    #region Spider
    static void ILSpiderAttached(ILContext il)
    {
        try
        {
            ILCursor val = new(il);
            ILLabel target = null;

            if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[5]
            {
                x => x.MatchLdloc(6),
                x => x.MatchLdloc(0),
                x => x.MatchCallvirt<BodyChunk>("get_owner"),
                x => x.MatchCallvirt<PhysicalObject>("get_TotalMass"),
                x => x.MatchBltUn(out target),
            }))
            {
                val.MoveBeforeLabels();

                val.Emit(OpCodes.Ldloc_0);
                val.Emit<BodyChunk>(OpCodes.Callvirt, "get_owner");
                val.Emit(OpCodes.Isinst, typeof(Creature));
                val.EmitDelegate(SpiderAttached);
                val.Emit(OpCodes.Brfalse_S, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILSpiderAttached!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    public static bool SpiderAttached(Player self)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return true;
        }

        if (UnityEngine.Random.value < 0.1f)
        {
            RWHealthState.Damage(self.State, state, new RWBite(), UnityEngine.Random.Range(0.8f, 1.2f), GetHitBodyPart(state), "Coalescipede - Fangs");
        }

        return false;
    }
    #endregion

    #region SporeCloud
    static void ILSporeCloudUpdate(ILContext il)
    {
        try
        {
            ILCursor val = new(il);
            ILLabel target = null;

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[2]
            {
                x => x.MatchLdloc(4),
                x => x.MatchCallvirt<Creature>("Die")
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldloc, 4);
                val.EmitDelegate(SporeCloudUpdateForceFull);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILSporeCloudUpdate Fly Die!");
            }

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdloc(4),
                x => x.MatchLdcI4(10),
                x => x.MatchLdcI4(120)
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldloc, 4);
                val.EmitDelegate(SporeCloudUpdateAdd);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILSporeCloudUpdate Fly Stun!");
            }

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[4]
            {
                x => x.MatchLdloc(7),
                x => x.MatchLdfld<InsectoidCreature>("poison"),
                x => x.MatchLdcR4(1f),
                x => x.MatchBltUn(out target)
            })) {}
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILSporeCloudUpdate target!");
            }

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdloc(9),
                x => x.MatchDup(),
                x => x.MatchCallvirt<HealthState>("get_health")
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldloc, 4);
                val.EmitDelegate(CreatureHasState);
                val.Emit(OpCodes.Brfalse_S, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILSporeCloudUpdate Damage!");
            }

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[2]
            {
                x => x.MatchLdloc(4),
                x => x.MatchCallvirt<Creature>("Die")
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldloc, 4);
                val.EmitDelegate(SporeCloudUpdateForceFull);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILSporeCloudUpdate Die!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    public static void SporeCloudUpdateForceFull(Player self)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return;
        }

        RWToxicBuildup toxicBuildup = null;

        for (int i = 0; i < state.wholeBodyAfflictions.Count; i++)
        {
            if (state.wholeBodyAfflictions[i] is RWToxicBuildup tempToxicBuildup)
            {
                toxicBuildup = tempToxicBuildup;
                break;
            }
        }

        if (toxicBuildup != null)
        {
            toxicBuildup.tendQuality = 1;
        }
        else
        {
            state.wholeBodyAfflictions.Add(new RWToxicBuildup(self.State, null, 1));
            state.updateCapacities = true;
        }
    }

    public static void SporeCloudUpdateAdd(Creature self)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return;
        }

        RWToxicBuildup toxicBuildup = null;

        for (int i = 0; i < state.wholeBodyAfflictions.Count; i++)
        {
            if (state.wholeBodyAfflictions[i] is RWToxicBuildup tempToxicBuildup)
            {
                toxicBuildup = tempToxicBuildup;
                break;
            }
        }

        float poison = UnityEngine.Random.Range(0.2f, 0.6f);

        if (toxicBuildup != null)
        {
            toxicBuildup.tendQuality += poison;
            toxicBuildup.tendQuality = Math.Min(1, toxicBuildup.tendQuality);
        }
        else
        {
            state.wholeBodyAfflictions.Add(new RWToxicBuildup(self.State, null, poison));
            state.updateCapacities = true;
        }
    }
    #endregion

    #region OracleBehavior
    static void ILThrowOutBehaviorUpdate(ILContext il)
    {
        try
        {
            ILCursor val = new(il);

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdarg(0),
                x => x.MatchCall<SSOracleBehavior.SubBehavior>("get_player"),
                x => x.MatchCallvirt<Creature>("Die")
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.Emit<SSOracleBehavior.SubBehavior>(OpCodes.Call, "get_player");
                val.EmitDelegate(ThrowOutBehaviorUpdate);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILThrowOutBehaviorUpdate!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    public static void ThrowOutBehaviorUpdate(Player self)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state) || state.consciousnessSource == null)
        {
            return;
        }

        RWHealthState.Damage(self.State, state, new RWBomb(), 999999f, state.consciousnessSource, "Oracle - Explosion", "Oracle");
    }
    #endregion

    #region TubeWorm
    static void ILTubeWormUpdate(ILContext il)
    {
        try
        {
            ILCursor val = new(il);

            if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[4]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<TubeWorm>("lungs"),
                x => x.MatchLdcR4(0.0055555557f)
            }))
            {
                val.MoveBeforeLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(LungsLossMultiplier);
                val.Emit(OpCodes.Mul);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILTubeWormUpdate loss!");
            }

            if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[4]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<TubeWorm>("lungs"),
                x => x.MatchLdcR4(0.02f)
            }))
            {
                val.MoveBeforeLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(LungsGainMultiplier);
                val.Emit(OpCodes.Mul);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILTubeWormUpdate gain!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }

    static void ILTubeWormTongueUpdate(ILContext il)
    {
        ILCursor val = new(il);
        ILLabel target = null;

        if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[4]
        {
            x => x.MatchLdloc(8),
            x => x.MatchLdcI4(1),
            x => x.MatchAdd(),
            x => x.MatchStloc(8)
        }))
        {
            val.MoveAfterLabels();

            target = val.MarkLabel();
        }
        else
        {
            RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILTubeWormTongueUpdate target!");
        }

        if (val.TryGotoPrev(MoveType.Before, new Func<Instruction, bool>[3]
        {
            x => x.MatchLdarg(0),
            x => x.MatchLdfld<TubeWorm.Tongue>("worm"),
            x => x.MatchCallvirt<Creature>("Die")
        }))
        {
            val.Emit(OpCodes.Ldarg_0);
            val.Emit<TubeWorm.Tongue>(OpCodes.Ldfld, "worm");
            val.EmitDelegate(TubeWormTongueUpdate);
            val.Emit(OpCodes.Brfalse_S, target);
        }
        else
        {
            RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILTubeWormTongueUpdate!");
        }
    }
    public static bool TubeWormTongueUpdate(TubeWorm self)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return true;
        }

        for (int i = 0; i < state.bodyParts.Count; i++)
        {
            if (state.bodyParts[i] is Tongue part && !IsDestroyed(part))
            {
                RWHealthState.Damage(self.State, state, new RWElectricBurn(), UnityEngine.Random.Range(7.2f, 12.8f), part, "Zap-Coil");
            }
        }

        return false;
    }
    #endregion

    #region VultureGrub
    static void ILVultureGrubUpdate(ILContext il)
    {
        try
        {
            ILCursor val = new(il);

            if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[4]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<VultureGrub>("lungs"),
                x => x.MatchLdcR4(0.0055555557f)
            }))
            {
                val.MoveBeforeLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(LungsLossMultiplier);
                val.Emit(OpCodes.Mul);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILVultureGrubUpdate loss!");
            }

            if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[4]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<VultureGrub>("lungs"),
                x => x.MatchLdcR4(0.02f)
            }))
            {
                val.MoveBeforeLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(LungsGainMultiplier);
                val.Emit(OpCodes.Mul);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILVultureGrubUpdate gain!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    #endregion

    #region Weapon
    static void ILWeaponHitSomethingWithoutStopping(ILContext il)
    {
        try
        {
            ILCursor val = new(il);
            ILLabel target = null;

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdarg(1),
                x => x.MatchIsinst(typeof(Creature)),
                x => x.MatchBrfalse(out target)
            })){}
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILWeaponHitSomethingWithoutStopping target!");
            }

            if (val.TryGotoNext(MoveType.AfterLabel, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdarg(0),
                x => x.MatchIsinst(typeof(Spear)),
                x => x.MatchBrfalse(out _)
            }))
            {
                val.Emit(OpCodes.Ldarg_1);
                val.Emit(OpCodes.Isinst, typeof(Creature));
                val.Emit(OpCodes.Ldarg_0);
                val.Emit<Weapon>(OpCodes.Ldfld, "thrownBy");
                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(WeaponHitSomethingWithoutStoppingSpear);
                val.Emit(OpCodes.Brfalse_S, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILWeaponHitSomethingWithoutStopping Spear!");
            }

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdarg(1),
                x => x.MatchIsinst(typeof(Creature)),
                x => x.MatchCallvirt<Creature>("Die")
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_1);
                val.Emit(OpCodes.Isinst, typeof(Creature));
                val.Emit(OpCodes.Ldarg_0);
                val.Emit<Weapon>(OpCodes.Ldfld, "thrownBy");
                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(WeaponHitSomethingWithoutStoppingRockDie);
                val.Emit(OpCodes.Brfalse_S, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILWeaponHitSomethingWithoutStopping Die!");
            }

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[4]
            {
                x => x.MatchLdarg(1),
                x => x.MatchIsinst(typeof(Creature)),
                x => x.MatchLdcI4(80),
                x => x.MatchCallvirt<Creature>("Stun")
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_1);
                val.Emit(OpCodes.Isinst, typeof(Creature));
                val.Emit(OpCodes.Ldarg_0);
                val.Emit<Weapon>(OpCodes.Ldfld, "thrownBy");
                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(WeaponHitSomethingWithoutStoppingRock);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILWeaponHitSomethingWithoutStopping Stun!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    public static bool WeaponHitSomethingWithoutStoppingSpear(Creature self, Creature attacker, Weapon weapon)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return true;
        }

        RWHealthState.Damage(self.State, state, new RWStab(), UnityEngine.Random.Range(2.2f, 4.8f), GetHitBodyPart(state, null, null, true), weapon.ToString(), attacker.ToString());

        return false;
    }
    public static bool WeaponHitSomethingWithoutStoppingRockDie(Creature self, Creature attacker, Weapon weapon)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return true;
        }

        BluntDamage(self.State, state, null, UnityEngine.Random.Range(0.2f, 1.8f), weapon);
        self.Stun(80);

        return false;
    }
    public static void WeaponHitSomethingWithoutStoppingRock(Creature self, Creature attacker, Weapon weapon)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return;
        }

        BluntDamage(self.State, state, null, UnityEngine.Random.Range(0.2f, 1.8f), weapon);
    }
    #endregion

    #region ZapCoil
    static void ILZapCoilUpdate(ILContext il)
    {
        ILCursor val = new(il);
        ILLabel target = null;

        if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[9]
        {
            x => x.MatchLdarg(0),
            x => x.MatchLdfld<UpdatableAndDeletable>("room"),
            x => x.MatchLdfld<Room>("physicalObjects"),
            x => x.MatchLdloc(1),
            x => x.MatchLdelemRef(),
            x => x.MatchLdloc(2),
            x => x.MatchCallvirt(typeof(List<PhysicalObject>).GetMethod("get_Item")),
            x => x.MatchIsinst(typeof(Creature)),
            x => x.MatchBrfalse(out target),
        }))
        {
            val.MoveBeforeLabels();

            val.Emit(OpCodes.Ldarg_0);
            val.Emit<UpdatableAndDeletable>(OpCodes.Ldfld, "room");
            val.Emit<Room>(OpCodes.Ldfld, "physicalObjects");
            val.Emit(OpCodes.Ldloc_1);
            val.Emit(OpCodes.Ldelem_Ref);
            val.Emit(OpCodes.Ldloc_2);
            val.Emit(OpCodes.Callvirt, typeof(List<PhysicalObject>).GetMethod("get_Item"));
            val.Emit(OpCodes.Isinst, typeof(Creature));
            val.EmitDelegate(ZapCoilUpdate);
            val.Emit(OpCodes.Brfalse_S, target);
        }
        else
        {
            RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILZapCoilUpdate!");
        }
    }
    public static bool ZapCoilUpdate(Creature self)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return true;
        }

        RWHealthState.Damage(self.State, state, new RWElectricBurn(), UnityEngine.Random.Range(7.2f, 12.8f), GetHitBodyPart(state), "Zap-Coil");

        return false;
    }
    #endregion
    #endregion

    #region MoreSlugcats
    #region BigJellyFish
    static void ILBigJellyFishCollide(ILContext il)
    {
        try
        {
            ILCursor val = new(il);
            ILLabel target = null;

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[5]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<MoreSlugcats.BigJellyFish>("consumedCreatures"),
                x => x.MatchLdarg(1),
                x => x.MatchIsinst(typeof(Creature)),
                x => x.MatchCallvirt(typeof(List<Creature>).GetMethod("Add"))
            })) 
            {
                val.MoveAfterLabels();

                target = val.MarkLabel();
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILBigJellyFishCollide target!");
            }

            if (val.TryGotoPrev(MoveType.Before, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdarg(1),
                x => x.MatchIsinst(typeof(Creature)),
                x => x.MatchCallvirt<Creature>("Die")
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.Emit(OpCodes.Isinst, typeof(Creature));
                val.EmitDelegate(BigJellyFishCollide);
                val.Emit(OpCodes.Brfalse_S, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILBigJellyFishCollide!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    public static bool BigJellyFishCollide(Creature self)
    {
        if (self.dead || self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return true;
        }

        RWHealthState.Damage(self.State, state, new RWElectricBurn(), UnityEngine.Random.Range(22.2f, 44.8f), GetHitBodyPart(state), "Big Jellyfish - Electricity");

        if (!self.dead)
        {
            self.Stun(StunMath(400, self, Creature.DamageType.Electric));
            self.room.AddObject(new CreatureSpasmer(self, false, self.stun));
            self.LoseAllGrasps();
        }

        return false;
    }
    #endregion

    #region EnergyCell
    static void ILEnergyCellExplode(ILContext il)
    {
        try
        {
            ILCursor val = new(il);
            ILLabel target = null;

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[9]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<UpdatableAndDeletable>("room"),
                x => x.MatchLdfld<Room>("physicalObjects"),
                x => x.MatchLdloc(8),
                x => x.MatchLdelemRef(),
                x => x.MatchLdloc(9),
                x => x.MatchCallvirt(typeof(List<PhysicalObject>).GetMethod("get_Item")),
                x => x.MatchIsinst(typeof(Creature)),
                x => x.MatchBrfalse(out target)
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.Emit<UpdatableAndDeletable>(OpCodes.Ldfld, "room");
                val.Emit<Room>(OpCodes.Ldfld, "physicalObjects");
                val.Emit(OpCodes.Ldloc, 8);
                val.Emit(OpCodes.Ldelem_Ref);
                val.Emit(OpCodes.Ldloc, 9);
                val.Emit(OpCodes.Callvirt, typeof(List<PhysicalObject>).GetMethod("get_Item"));
                val.Emit(OpCodes.Isinst, typeof(Creature));
                val.EmitDelegate(CreatureHasState);
                val.Emit(OpCodes.Brfalse_S, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILEnergyCellExplode!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    #endregion

    #region LillyPuck
    static void ILLillyPuckHitSomething(ILContext il)
    {
        try
        {
            ILCursor val = new(il);
            ILLabel target = null;

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[4]
            {
                x => x.MatchLdarg(1),
                x => x.MatchLdfld<SharedPhysics.CollisionResult>("obj"),
                x => x.MatchIsinst(typeof(Player)),
                x => x.MatchBrfalse(out target)
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_1);
                val.Emit<SharedPhysics.CollisionResult>(OpCodes.Ldfld, "obj");
                val.Emit(OpCodes.Isinst, typeof(Creature));
                val.EmitDelegate(CreatureHasState);
                val.Emit(OpCodes.Brfalse, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILLillyPuckHitSomething!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    #endregion

    #region SingularityBomb
    static void ILSingularityBombExplode(ILContext il)
    {
        try
        {
            ILCursor val = new(il);
            ILLabel target = null;

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[9]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<UpdatableAndDeletable>("room"),
                x => x.MatchLdfld<Room>("physicalObjects"),
                x => x.MatchLdloc(7),
                x => x.MatchLdelemRef(),
                x => x.MatchLdloc(8),
                x => x.MatchCallvirt(typeof(List<PhysicalObject>).GetMethod("get_Item")),
                x => x.MatchIsinst(typeof(MoreSlugcats.ElectricSpear)),
                x => x.MatchBrfalse(out target)
            }))
            {
                val.MoveAfterLabels();

                target = val.MarkLabel();
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILSingularityBombExplode target!");
            }

            if (val.TryGotoPrev(MoveType.Before, new Func<Instruction, bool>[9]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<UpdatableAndDeletable>("room"),
                x => x.MatchLdfld<Room>("physicalObjects"),
                x => x.MatchLdloc(7),
                x => x.MatchLdelemRef(),
                x => x.MatchLdloc(8),
                x => x.MatchCallvirt(typeof(List<PhysicalObject>).GetMethod("get_Item")),
                x => x.MatchIsinst(typeof(Creature)),
                x => x.MatchCallvirt<Creature>("Die"),
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.Emit<UpdatableAndDeletable>(OpCodes.Ldfld, "room");
                val.Emit<Room>(OpCodes.Ldfld, "physicalObjects");
                val.Emit(OpCodes.Ldloc, 7);
                val.Emit(OpCodes.Ldelem_Ref);
                val.Emit(OpCodes.Ldloc, 8);
                val.Emit(OpCodes.Callvirt, typeof(List<PhysicalObject>).GetMethod("get_Item"));
                val.Emit(OpCodes.Isinst, typeof(Creature));
                val.EmitDelegate(CreatureHasState);
                val.Emit(OpCodes.Brfalse_S, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILSingularityBombExplode!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    #endregion
    #endregion

    #region Watcher
    #region ARZapper
    static void ILARZapperZapperContact(ILContext il)
    {
        try
        {
            ILCursor val = new(il);
            ILLabel target = null;

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[5]
            {
                x => x.MatchLdarg(1),
                x => x.MatchCallvirt<BodyChunk>("get_owner"),
                x => x.MatchIsinst(typeof(Creature)),
                x => x.MatchLdcI4(100),
                x => x.MatchCallvirt<Creature>("Stun"),
            })) 
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_1);
                val.Emit<BodyChunk>(OpCodes.Callvirt, "get_owner");
                val.Emit(OpCodes.Isinst, typeof(Creature));
                val.EmitDelegate(ARZapperZapperContactStun);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILARZapperZapperContact Stun!");
            }

            if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[4]
            {
                x => x.MatchLdarg(1),
                x => x.MatchCallvirt<BodyChunk>("get_owner"),
                x => x.MatchIsinst(typeof(Creature)),
                x => x.MatchBrfalse(out target),
            }))
            {
                val.MoveBeforeLabels();

                val.Emit(OpCodes.Ldarg_1);
                val.Emit<BodyChunk>(OpCodes.Callvirt, "get_owner");
                val.Emit(OpCodes.Isinst, typeof(Creature));
                val.EmitDelegate(ARZapperZapperContactDie);
                val.Emit(OpCodes.Brfalse_S, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILARZapperZapperContact Die!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    public static void ARZapperZapperContactStun(Creature self)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return;
        }

        RWHealthState.Damage(self.State, state, new RWElectricBurn(), UnityEngine.Random.Range(7.2f, 12.8f), GetHitBodyPart(state), "Zapper");
    }
    public static bool ARZapperZapperContactDie(Creature self)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return true;
        }

        RWHealthState.Damage(self.State, state, new RWElectricBurn(), UnityEngine.Random.Range(7.2f, 12.8f), GetHitBodyPart(state), "Zapper");

        return false;
    }
    #endregion

    #region Barnacle
    static void ILBarnacleBitByPlayer(ILContext il)
    {
        try
        {
            ILCursor val = new(il);
            ILLabel target = null;

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[2]
            {
                x => x.MatchLdarg(0),
                x => x.MatchCall<PhysicalObject>("get_firstChunk")
            }))
            {
                val.MoveAfterLabels();

                target = val.MarkLabel();
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILBarnacleBitByPlayer target!");
            }

            if (val.TryGotoPrev(MoveType.Before, new Func<Instruction, bool>[2]
            {
                x => x.MatchLdarg(0),
                x => x.MatchCallvirt<Creature>("Die"),
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(CreatureHasState);
                val.Emit(OpCodes.Brfalse_S, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILBarnacleBitByPlayer!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    #endregion

    #region BigMoth
    static void ILBigMothUpdate(ILContext il)
    {
        try
        {
            ILCursor val = new(il);
            ILLabel target = null;

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[3]
            {
                x => x.MatchBr(out target),
                x => x.MatchLdnull(),
                x => x.MatchStloc(11),
            })) {}
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILBigMothUpdate target!");
            }

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[2]
            {
                x => x.MatchLdloc(11),
                x => x.MatchDup(),
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldloc_3);
                val.EmitDelegate(BigMothUpdateDamage);
                val.Emit(OpCodes.Brfalse_S, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILBigMothUpdate Health!");
            }

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdcI4(0),
                x => x.MatchStfld<Watcher.BigMoth>("eatCounter")
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldloc_3);
                val.EmitDelegate(BigMothUpdate);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILBigMothUpdate Food down!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    public static bool BigMothUpdateDamage(Creature self)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return true;
        }

        state.bloodLoss += 0.01f;
        state.updateCapacities = true;

        return false;
    }
    public static void BigMothUpdate(Creature self)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return;
        }

        Debug.Log("BigMoth Eat, meat left " + self.State.meatLeft);

        if (self.State.meatLeft == 0)
        {
            state.bloodLoss = 1f;
            state.updateCapacities = true;
        }
        else if (self.State.meatLeft == 1)
        {
            state.bloodLoss = Mathf.Max(state.bloodLoss, 0.8f);
            state.updateCapacities = true;
        }
        else
        {
            state.bloodLoss = Mathf.Max(state.bloodLoss, 0.5f);
            state.updateCapacities = true;
        }
    }
    #endregion

    #region Frog
    static void ILFrogBitByPlayer(ILContext il)
    {
        try
        {
            ILCursor val = new(il);
            ILLabel target = null;

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdarg(0),
                x => x.MatchCall<Creature>("get_dead"),
                x => x.MatchBrtrue(out target),
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(CreatureHasState);
                val.Emit(OpCodes.Brfalse, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILFrogBitByPlayer!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    #endregion

    #region Rat
    static void ILRatBitByPlayer(ILContext il)
    {
        try
        {
            ILCursor val = new(il);
            ILLabel target = null;

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdarg(0),
                x => x.MatchCall<Creature>("get_dead"),
                x => x.MatchBrtrue(out target),
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(CreatureHasState);
                val.Emit(OpCodes.Brfalse, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILRatBitByPlayer!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    #endregion

    #region Sandstorm
    static void ILSandstormAffectObjects(ILContext il)
    {
        ILCursor val = new(il);
        ILLabel target = null;

        if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[1]
        {
            x => x.MatchCallvirt<Creature>("Stun")
        })) { }
        else
        {
            RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILSandstormAffectObjects skip!");
        }

        if (val.TryGotoPrev(MoveType.Before, new Func<Instruction, bool>[3]
        {
            x => x.MatchLdloc(5),
            x => x.MatchLdcI4(1),
            x => x.MatchLdcI4(1)
        }))
        {
            val.Emit(OpCodes.Ldloc, 5);
            val.EmitDelegate(SandstormAffectObjects);
        }
        else
        {
            RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILSandstormAffectObjects!");
        }

        if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[4]
        {
            x => x.MatchLdloc(5),
            x => x.MatchLdfld<Creature>("rainDeath"),
            x => x.MatchLdcR4(1),
            x => x.MatchBleUn(out target)
        }))
        {
            val.MoveBeforeLabels();

            val.Emit(OpCodes.Ldloc, 5);
            val.EmitDelegate(SandstormAffectObjectsDie);
            val.Emit(OpCodes.Brfalse_S, target);
        }
        else
        {
            RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILRoomRainThrowAroundObjects Die!");
        }
    }
    public static void SandstormAffectObjects(Player self)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return;
        }

        RWHealthState.Damage(self.State, state, new RWBlunt(), UnityEngine.Random.Range(0.8f, 3.2f), GetHitBodyPart(state, null, null, false, true), "Sandstorm");
    }
    public static bool SandstormAffectObjectsDie(Player self)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return true;
        }

        RWHealthState.Damage(self.State, state, new RWBlunt(), UnityEngine.Random.Range(14.2f, 24.8f), GetHitBodyPart(state, null, null, false, true), "Sandstorm");

        return false;
    }
    #endregion

    #region Tardigrade
    private static void ILTardigradeBitByPlayer(ILContext il)
    {
        try
        {
            ILCursor val = new(il);
            ILLabel target = null;

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdarg(0),
                x => x.MatchCall<Creature>("get_dead"),
                x => x.MatchBrtrue(out target),
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(CreatureHasState);
                val.Emit(OpCodes.Brfalse, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILTardigradeBitByPlayer!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    #endregion
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

    }
}
*/