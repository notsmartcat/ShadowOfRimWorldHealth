using Mono.Cecil.Cil;
using MonoMod.Cil;
using RWCustom;
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

        #region DartMaggot
        IL.DartMaggot.ShotUpdate += ILDartMaggotShotUpdate;
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

        #region KarmicShockwave
        IL.KarmicShockwave.StunCreatures += ILKarmicShockwaveStunCreatures;
        #endregion

        #region KingTusk
        IL.KingTusks.Tusk.ShootUpdate += ILKingTuskShootUpdate;
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

        #region Overseer
        IL.OverseerAbstractAI.AbstractBehavior += OverseerAbstractAIAbstractBehavior;
        #endregion

        #region Player
        IL.Player.checkInput += ILPlayercheckInput;
        IL.Player.EatMeatUpdate += ILPlayerEatMeatUpdate;
        IL.Player.GrabUpdate += ILPlayerGrabUpdate;
        IL.Player.LungUpdate += ILPlayerLungUpdate;
        IL.Player.Jump += ILPlayerJump;
        IL.Player.MovementUpdate += ILPlayerMovementUpdate;
        IL.Player.TerrainImpact += ILPlayerTerrainImpact;
        IL.Player.UpdateAnimation += ILPlayerUpdateAnimation;
        IL.Player.UpdateBodyMode += ILPlayerUpdateBodyMode;
        IL.Player.WallJump += ILPlayerWallJump;
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

        #region SporePlant(Bee)
        IL.SporePlant.AttachedBee.ctor += ILNewSporePlantAttachedBee;

        IL.SporePlant.AttachedBee.Update += ILSporePlantAttachedBeeUpdate;
        #endregion

        #region OracleBehavior
        IL.SSOracleBehavior.ThrowOutBehavior.Update += ILThrowOutBehaviorUpdate;
        #endregion

        #region TubeWorm
        IL.TubeWorm.Update += ILTubeWormUpdate;
        IL.TubeWorm.Tongue.Update += ILTubeWormTongueUpdate;
        #endregion

        #region UnderwaterShock
        IL.UnderwaterShock.Update += ILUnderwaterShockUpdate;
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

        #region SlugNPCAI
        IL.MoreSlugcats.SlugNPCAI.Update += ILSlugNPCAIUpdate;
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

        #region FlameJet
        IL.Watcher.FlameJet.UpdateDamage += ILFlameJetUpdateDamage;
        #endregion

        #region Frog
        IL.Watcher.Frog.BitByPlayer += ILFrogBitByPlayer;
        #endregion

        #region Lightning
        IL.Watcher.LightningMaker.StrikeAOE.Update += ILLightningStrikeAOEUpdate;
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

        self.remainInDenCounter = Mathf.Max(10, Mathf.RoundToInt(200 * (1 + (state.eating - 1f) * 0.4f) * (1f + (state.digestion - 1) * 0.6f)));
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

        foreach (RWBodyPart part in state.bodyParts)
        {
            if (part is UpperTorso)
            {
                mainPart = part;
                break;
            }
        }

        if (mainPart == null)
        {
            return;
        }

        RWHealthState.Damage(self.State, state, new RWCut(), mainPart.maxHealth, 999, mainPart, "Big Spider - Spew Babies");
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

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdcR4(0.0f),
                x => x.MatchStfld<Creature>("HypothermiaExposure"),
            }))
            {
                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(CreatureHypothermiaUpdate);
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

        foreach (RWAffliction affliction in state.wholeBodyAfflictions)
        {
            if (affliction is RWHypothermia tempHypothermia)
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

        LethalWaterDamage(self.State, state);

        if (self.lavaContactCount > 3)
        {
            self.mainBodyChunk.vel.y = 5f;
        }

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

    #region DartMaggot
    static void ILDartMaggotShotUpdate(ILContext il)
    {
        try
        {
            ILCursor val = new(il);
            ILLabel target = null;

            if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[4]
            {
                x => x.MatchBr(out _),
                x => x.MatchLdloc(5),
                x => x.MatchLdfld<SharedPhysics.CollisionResult>("chunk"),
                x => x.MatchBrfalse(out target)
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.Emit(OpCodes.Ldloc, 5);
                val.Emit<SharedPhysics.CollisionResult>(OpCodes.Ldfld, "chunk");
                val.EmitDelegate(DartMaggotShotUpdateHitCheck);
                val.Emit(OpCodes.Brfalse, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILDartMaggotShotUpdate HitCheck!");
            }

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdarg(0),
                x => x.MatchCall<PhysicalObject>("get_firstChunk"),
                x => x.MatchLdflda<BodyChunk>("vel")
            }))
            {
                val.MoveAfterLabels();

                target = val.MarkLabel();
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILDartMaggotShotUpdate target!");
            }

            if (val.TryGotoPrev(MoveType.Before, new Func<Instruction, bool>[4]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<PhysicalObject>("abstractPhysicalObject"),
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<DartMaggot>("stuckInChunk")
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.Emit(OpCodes.Ldloc, 5);
                val.Emit<SharedPhysics.CollisionResult>(OpCodes.Ldfld, "chunk");
                val.EmitDelegate(DartMaggotShotUpdate);
                val.Emit(OpCodes.Brfalse, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILDartMaggotShotUpdate!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }

    public static bool DartMaggotShotUpdateHitCheck(DartMaggot self, BodyChunk chunk)
    {
        if (chunk.owner == null || chunk.owner is not Creature creature || !healthState.TryGetValue(creature.State, out RWState state) || !state.missingBodyChunks.Contains(chunk.index))
        {
            return true;
        }

        return false;
    }

    public static bool DartMaggotShotUpdate(DartMaggot self, BodyChunk chunk)
    {
        if (!weaponstat.TryGetValue(self.abstractPhysicalObject, out RWWeaponStats weaponState) || (!weaponState.wasDeflected && !weaponState.destroyedPart))
        {
            return true;
        }

        if (weaponState.wasDeflected)
        {
            self.room.AddObject(new Spark(chunk.pos + Custom.DegToVec(UnityEngine.Random.value * 360f) * (5f * UnityEngine.Random.value), self.firstChunk.vel * -0.1f + Custom.DegToVec(UnityEngine.Random.value * 360f) * (Mathf.Lerp(0.2f, 0.4f, UnityEngine.Random.value) * self.firstChunk.vel.magnitude), new Color(1f, 1f, 1f), null, 10, 170));
            self.room.PlaySound(SoundID.Spear_Bounce_Off_Creauture_Shell, self.firstChunk);

            self.firstChunk.vel = self.firstChunk.vel * -0.5f + Custom.DegToVec(UnityEngine.Random.value * 360f) * (Mathf.Lerp(0.1f, 0.4f, UnityEngine.Random.value) * self.firstChunk.vel.magnitude);
        }

        self.ChangeMode(DartMaggot.Mode.Free);
        self.stuckInChunk = null;

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
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state) || !singleExplosion.TryGetValue(obj, out OneTimeUseData data))
        {
            return true;
        }
        else if (data.creatures.Contains(self) || damage <= 0)
        {
            return false;
        }

        data.creatures.Add(self);

        bool isSuper = false;

        PhysicalObject sourceObj = obj.sourceObject ?? null;

        string attackName = obj.ToString();
        string attackerName = "";

        if (obj.killTagHolder != null)
        {
            attackerName = GetCreatureName(obj.killTagHolder);
        }

        if (sourceObj == null)
        {
            attackName = "Explosion";
        }
        else if (sourceObj is EggBug)
        {
            attackName = attackerName + " - Explosion";
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
            attackName = attackerName + " - Explosion";
        }
        else if (sourceObj is Vulture)
        {
            attackName = attackerName + " - Laser explosion";
        }
        else if (ModManager.MSC && sourceObj is MoreSlugcats.EnergyCell)
        {
            attackName = "Energy cell";

            isSuper = true;
        }
        else if (ModManager.MSC && sourceObj is MoreSlugcats.FireEgg)
        {
            attackName = "Fire egg";

            damage *= BombDamageMultiplier(self.State is HealthState, isSuper);

            BombDamage(self.State, state, damage, attackName, attackerName);

            if (damage >= 10)
            {
                RWHealthState.Burn(state);
            }

            return false;
        }
        else if (ModManager.MSC && sourceObj is MoreSlugcats.SingularityBomb)
        {
            attackName = "Singularity bomb";

            isSuper = true;
        }
        else if (sourceObj is Oracle)
        {
            attackName = attackerName + " - Explosion";
        }
        else if (sourceObj == null)
        {
            attackName = "Explosion";
        }

        BombDamage(self.State, state, damage * BombDamageMultiplier(self.State is HealthState, isSuper), attackName, attackerName);

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

        foreach (RWAffliction affliction in state.wholeBodyAfflictions)
        {
            if (affliction is RWAirInLungs tempAirInLungs)
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

        foreach (RWAffliction affliction in state.wholeBodyAfflictions)
        {
            if (affliction is RWAirInLungs tempAirInLungs)
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

    #region KarmicShockwave
    static void ILKarmicShockwaveStunCreatures(ILContext il)
    {
        try
        {
            ILCursor val = new(il);

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[2]
            {
                x => x.MatchLdloc(3),
                x => x.MatchLdnull()
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldloc_3);
                val.Emit(OpCodes.Ldarg_0);
                val.Emit<TemplarCircle>(OpCodes.Ldfld, "source");
                val.Emit(OpCodes.Isinst, typeof(Creature));
                val.EmitDelegate(KarmicShockwaveStunCreatures);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILKarmicShockwaveStunCreatures!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    public static void KarmicShockwaveStunCreatures(Creature self, Creature attacker)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return;
        }

        state.violenceAttackerOverride = GetCreatureName(attacker);
        state.violenceAttackOverride = state.violenceAttackerOverride + " - Karmic Shockwave";
    }
    #endregion

    #region KingTusk
    static void ILKingTuskShootUpdate(ILContext il)
    {
        try
        {
            ILCursor val = new(il);

            if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[5]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<KingTusks.Tusk>("impaleChunk"),
                x => x.MatchCallvirt<BodyChunk>("get_owner"),
                x => x.MatchIsinst(typeof(Creature)),
                x => x.MatchBrfalse(out _)
            }))
            {
                val.MoveBeforeLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.Emit<KingTusks.Tusk>(OpCodes.Ldfld, "impaleChunk");
                val.Emit<BodyChunk>(OpCodes.Callvirt, "get_owner");
                val.Emit(OpCodes.Isinst, typeof(Creature));
                val.Emit(OpCodes.Ldarg_0);
                val.Emit<KingTusks.Tusk>(OpCodes.Ldfld, "owner");
                val.Emit<KingTusks>(OpCodes.Ldfld, "vulture");
                val.EmitDelegate(KingTuskShootUpdate);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILKingTuskShootUpdate!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    public static void KingTuskShootUpdate(Creature self, Creature attacker)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return;
        }

        state.violenceAttackerOverride = GetCreatureName(attacker);
        state.violenceAttackOverride = state.violenceAttackerOverride + " - Tusk";
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

        LocustDamage(self.State, state);
        return false;
    }
    #endregion

    #region Overseer
    static void OverseerAbstractAIAbstractBehavior(ILContext il)
    {
        try
        {
            ILCursor val = new(il);

            if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdcI4(0),
                x => x.MatchStfld<OverseerAbstractAI>("targetCreatureTime")
            }))
            {
                val.Emit(OpCodes.Ldarg_0);
                val.Emit<OverseerAbstractAI>(OpCodes.Ldfld, "targetCreature");
                val.EmitDelegate(SafariControlCreature);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for OverseerAbstractAIAbstractBehavior control!");
            }

            if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdcI4(0),
                x => x.MatchStfld<OverseerAbstractAI>("targetCreatureTime")
            }))
            {
                val.EmitDelegate(SafariUnControlCreature);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for OverseerAbstractAIAbstractBehavior un-control!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }

    public static void SafariControlCreature(AbstractCreature self)
    {
        if (!healthState.TryGetValue(self.state, out RWState state))
        {
            return;
        }

        healthTabs[0].player = self;
        healthTabs[0].playerState = state;
    }
    public static void SafariUnControlCreature()
    {
        healthTabs[0].TriggeredOff();

        healthTabs[0].player = null;
        healthTabs[0].playerState = null;
    }
    #endregion

    #region Player
    static void ILPlayercheckInput(ILContext il)
    {
        try
        {
            ILCursor val = new(il);
            ILLabel target = null;

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdarg(0),
                x => x.MatchCall<Creature>("get_stun"),
                x => x.MatchBrtrue(out target)
            }))
            {
                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(PlayercheckInput);
                val.Emit(OpCodes.Brtrue, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayercheckInput bite!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    public static bool PlayercheckInput(Player self)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state) ||
            state.consciousState != 3 ||
            ModManager.MSC && self.State is MoreSlugcats.PlayerNPCState && ShadowOfOptions.player_uncon_movement.Value)
        {
            return true;
        }

        return false;
    }

    static void ILPlayerEatMeatUpdate(ILContext il)
    {
        try
        {
            ILCursor val = new(il);

            if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<UpdatableAndDeletable>("room"),
                x => x.MatchLdsfld<SoundID>("Slugcat_Eat_Meat_A")
            })) {}
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerEatMeatUpdate skip!");
            }

            if (val.TryGotoPrev(MoveType.Before, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<UpdatableAndDeletable>("room"),
                x => x.MatchLdsfld<SoundID>("Slugcat_Eat_Meat_B")
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.Emit(OpCodes.Ldarg_0);
                val.Emit<Creature>(OpCodes.Call, "get_grasps");
                val.Emit(OpCodes.Ldarg_1);
                val.Emit(OpCodes.Ldelem_Ref);
                val.Emit<Creature.Grasp>(OpCodes.Ldfld, "grabbed");
                val.Emit(OpCodes.Isinst, typeof(Creature));
                val.EmitDelegate(PlayerEatMeatUpdate);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerEatMeatUpdate!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    public static void PlayerEatMeatUpdate(Player self, Creature creature)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state) || creature.State == null || !healthState.TryGetValue(creature.State, out RWState otherState))
        {
            return;
        }

        List<RWBodyPart> ediblebodyParts = new();
        RWBodyPart focusedBodyPart;
        
        foreach (RWBodyPart part in otherState.bodyParts)
        {
            if (IsDestroyed(part) || !part.edible)
            {
                continue;
            }

            ediblebodyParts.Add(part);
        }

        while (true)
        {
            focusedBodyPart = ediblebodyParts[UnityEngine.Random.Range(0, ediblebodyParts.Count)];

            if (focusedBodyPart is UpperTorso && ediblebodyParts.Count > 1)
            {
                ediblebodyParts.Remove(focusedBodyPart);

                focusedBodyPart = null;
            }
            else if (focusedBodyPart is Neck)
            {
                bool containsHead = false;

                foreach (RWBodyPart part in ediblebodyParts)
                {
                    if (part is Head)
                    {
                        containsHead = true;
                        break;
                    }
                }

                if (containsHead)
                {
                    ediblebodyParts.Remove(focusedBodyPart);

                    focusedBodyPart = null;
                }
            }
            else if (focusedBodyPart is Shoulder shoulder)
            {
                for (int i = 0; i < state.armSetNames.Count; i++)
                {
                    if (!state.armSet[state.armSetNames[i]].BelongsToThis(shoulder))
                    {
                        continue;
                    }

                    focusedBodyPart = state.armSet[state.armSetNames[i]].EatPart();
                    break;
                }
            }
            else if (focusedBodyPart is Leg leg)
            {
                for (int i = 0; i < state.legSetNames.Count; i++)
                {
                    if (!state.legSet[state.legSetNames[i]].BelongsToThis(leg))
                    {
                        continue;
                    }

                    focusedBodyPart = state.legSet[state.legSetNames[i]].EatPart();
                    break;
                }
            }

            if (focusedBodyPart != null)
            {
                break;
            }
        }

        if (focusedBodyPart is UpperTorso)
        {
            creature.Destroy();
            return;
        }

        RWHealthState.Damage(creature.State, otherState, new RWBite(), 999999f, 999, focusedBodyPart, GetCreatureName(self) + " - Teeth", GetCreatureName(self));
    }

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

        SlugcatHooks.ArtiLungExplosion(self.State, state);
    }

    static void ILPlayerJump(ILContext il)
    {
        try
        {
            ILCursor val = new(il);

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdarg(0),
                x => x.MatchCall<Player>("get_isGourmand")
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldloc, 0);
                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(PlayerJump);
                val.Emit(OpCodes.Mul);
                val.Emit(OpCodes.Stloc, 0);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerJump!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }

    static void ILPlayerMovementUpdate(ILContext il)
    {
        try
        {
            ILCursor val = new(il);
            ILLabel target = null;

            #region Pole grab
            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[6]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<Player>("wantToGrab"),
                x => x.MatchLdcI4(0),
                x => x.MatchBle(out target),
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<Player>("noGrabCounter")
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(PlayerCanGrabPole);
                val.Emit(OpCodes.Brfalse, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerMovementUpdate pole grab!");
            }
            #endregion

            if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[9]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<UpdatableAndDeletable>("room"),
                x => x.MatchLdfld<Room>("aimap"),
                x => x.MatchLdarg(0),
                x => x.MatchCall<Creature>("get_mainBodyChunk"),
                x => x.MatchLdfld<BodyChunk>("pos"),
                x => x.MatchCallvirt<AImap>("getAItile"),
                x => x.MatchLdfld<AItile>("narrowSpace"),
                x => x.MatchBrfalse(out target)
            })) 
            {
                val.MoveBeforeLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(PlayerMovementUpdateNarrow);
                val.Emit(OpCodes.Brfalse, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerMovementUpdate narrow!");
            }

            if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[5]
            {
                x => x.MatchLdelemRef(),
                x => x.MatchLdfld<BodyChunk>("pos"),
                x => x.MatchCallvirt<AImap>("getAItile"),
                x => x.MatchLdfld<AItile>("narrowSpace"),
                x => x.MatchBrfalse(out target)
            }))
            {
                val.MoveBeforeLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.Emit(OpCodes.Ldloc, 29);
                val.EmitDelegate(PlayerMovementUpdateNarrowBoddyChunk);
                val.Emit(OpCodes.Brfalse, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerMovementUpdate narrow bodychynks!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    public static bool PlayerMovementUpdateNarrow(Player self)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state) || state.manipulation == 1 && state.moving == 1 && state.consciousState == 0)
        {
            return true;
        }

        float multiplyer = SlugcatHooks.GetPlayerMovementSpeedMultiplier(state, 0.4f);

        self.grasps[0].grabbedChunk.vel += self.input[0].IntVec.ToVector2().normalized * (self.slugcatStats.corridorClimbSpeedFac * 4f) / Mathf.Max(0.75f, self.grasps[0].grabbed.TotalMass) * multiplyer;
        
        return false;
    }
    public static bool PlayerMovementUpdateNarrowBoddyChunk(Player self, int n)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state) || state.manipulation == 1 && state.moving == 1 && state.consciousState == 0)
        {
            return true;
        }

        float multiplyer = SlugcatHooks.GetPlayerMovementSpeedMultiplier(state, 0.4f);

        self.grasps[0].grabbed.bodyChunks[n].vel *= 0.8f;

        self.grasps[0].grabbed.bodyChunks[n].vel.y += self.EffectiveRoomGravity * self.grasps[0].grabbed.gravity * 0.85f;
        self.grasps[0].grabbed.bodyChunks[n].vel += self.input[0].IntVec.ToVector2().normalized * (self.slugcatStats.corridorClimbSpeedFac * 1.5f) / ((float)self.grasps[0].grabbed.bodyChunks.Length * Mathf.Max(1f, (self.grasps[0].grabbed.TotalMass + 1f) / 2f)) * multiplyer;
        self.grasps[0].grabbed.bodyChunks[n].pos += self.input[0].IntVec.ToVector2().normalized * (self.slugcatStats.corridorClimbSpeedFac * 1.1f) / ((float)self.grasps[0].grabbed.bodyChunks.Length * Mathf.Max(1f, (self.grasps[0].grabbed.TotalMass + 2f) / 3f)) * multiplyer;

        return false;
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
            })){}
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
                val.Emit(OpCodes.Ldarg_3);
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
                val.Emit(OpCodes.Ldarg_3);
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
    public static bool PlayerTerrainImpactDeath(Player self, float speed, int hitChunk = 0)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return true;
        }

        TerrainImpactDamage(self, state, hitChunk, speed, true);

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
    public static bool PlayerTerrainImpactHard(Player self, float speed, int hitChunk = 0)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return true;
        }

        TerrainImpactDamage(self, state, hitChunk, speed, false);

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

    static void ILPlayerUpdateAnimation(ILContext il)
    {
        try
        {
            ILCursor val = new(il);
            ILLabel target = null;

            #region Hang from beam
            if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<Player>("animation"),
                x => x.MatchLdsfld<Player.AnimationIndex>("HangFromBeam")
            })) {}
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerUpdateAnimation hang from beam skip!");
            }

            if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[12]
            {
                x => x.MatchLdarg(0),
                x => x.MatchCall<PhysicalObject>("get_bodyChunks"),
                x => x.MatchLdcI4(1),
                x => x.MatchLdelemRef(),
                x => x.MatchCallvirt<BodyChunk>("get_ContactPoint"),
                x => x.MatchLdfld<RWCustom.IntVector2>("x"),
                x => x.MatchLdarg(0),
                x => x.MatchCall<Player>("get_input"),
                x => x.MatchLdcI4(0),
                x => x.MatchLdelema<Player.InputPackage>(),
                x => x.MatchLdfld<Player.InputPackage>("x"),
                x => x.MatchBeq(out target)
            }))
            {
                val.MoveBeforeLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(PlayerUpdateAnimationHangFromBeam);
                val.Emit(OpCodes.Brfalse, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerUpdateAnimation hang from beam!");
            }

            if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[4]
            {
                x => x.MatchLdarg(0),
                x => x.MatchCall<Player>("get_animationFrame"),
                x => x.MatchLdcI4(10),
                x => x.MatchBle(out target)
            })) {}
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerUpdateAnimation hang from beam target 2!");
            }

            if (val.TryGotoPrev(MoveType.Before, new Func<Instruction, bool>[2]
            {
                x => x.MatchLdarg(0),
                x => x.MatchCall<PhysicalObject>("get_bodyChunks"),
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(PlayerUpdateAnimationHangFromBeam2);
                val.Emit(OpCodes.Brfalse, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerUpdateAnimation hang from beam 2!");
            }
            #endregion

            #region Stand on beam
            if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[9]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<Player>("dynamicRunSpeed"),
                x => x.MatchLdcI4(1),
                x => x.MatchLdcR4(2.1f),
                x => x.MatchLdarg(0),
                x => x.MatchCall<Player>("get_slugcatStats"),
                x => x.MatchLdfld<SlugcatStats>("runspeedFac"),
                x => x.MatchMul(),
                x => x.MatchStelemR4()
            }))
            {
                val.MoveBeforeLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(PlayerUpdateStandSpeed);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerUpdateAnimation stand on beam!");
            }
            #endregion

            #region Climb on beam
            if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<Player>("animation"),
                x => x.MatchLdsfld<Player.AnimationIndex>("ClimbOnBeam")
            })) {}
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerUpdateAnimation climb on beam skip!");
            }

            if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[7]
            {
                x => x.MatchLdarg(0),
                x => x.MatchCall<Player>("get_input"),
                x => x.MatchLdcI4(0),
                x => x.MatchLdelema<Player.InputPackage>(),
                x => x.MatchLdfld<Player.InputPackage>("y"),
                x => x.MatchLdcI4(0),
                x => x.MatchBge(out target)
            })) {}
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerUpdateAnimation climb on beam target!");
            }

            if (val.TryGotoPrev(MoveType.Before, new Func<Instruction, bool>[2]
            {
                x => x.MatchLdarg(0),
                x => x.MatchCall<PhysicalObject>("get_bodyChunks"),
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(PlayerUpdateAnimationClimbOnBeam);
                val.Emit(OpCodes.Brfalse, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerUpdateAnimation climb on beam!");
            }
            #endregion

            #region Zero G pole grab
            if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[1]
            {
                x => x.MatchLdsfld<Player.AnimationIndex>("ZeroGPoleGrab")
            })) { }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerUpdateAnimation 0 g pole grab skip!");
            }

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[3]
            {
                x => x.MatchCall<Player.InputPackage>("get_ZeroGGamePadIntVec"),
                x => x.MatchLdfld<RWCustom.IntVector2>("x"),
                x => x.MatchBrfalse(out target)
            })) { }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerUpdateAnimation 0 g pole grab target!");
            }

            if (val.TryGotoPrev(MoveType.After, new Func<Instruction, bool>[2]
            {
                x => x.MatchStfld<Player>("curcuitJumpMeter"),
                x => x.MatchBr(out _),
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(PlayerCanGrabPole);
                val.Emit(OpCodes.Brfalse, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerUpdateAnimation 0 g pole grab!");
            }
            #endregion

            #region Zero G pole grab stand
            if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<Player>("animation"),
                x => x.MatchLdsfld<Player.AnimationIndex>("ZeroGPoleGrab")
            })) { }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerUpdateAnimation 0 g pole grab stand skip!");
            }

            if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<UpdatableAndDeletable>("room"),
                x => x.MatchLdsfld<SoundID>("Slugcat_Climb_Up_Vertical_Beam")
            })) {}
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerUpdateAnimation 0 g pole grab stand skip 2!");
            }

            if (val.TryGotoPrev(MoveType.After, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdarg(0),
                x => x.MatchCall<Player>("get_animationFrame"),
                x => x.MatchStloc(1)
            })) 
            {
                val.MoveAfterLabels();

                target = val.MarkLabel();
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerUpdateAnimation 0 g pole grab stand label!");
            }

            if (val.TryGotoPrev(MoveType.Before, new Func<Instruction, bool>[2]
            {
                x => x.MatchLdarg(0),
                x => x.MatchCall<Creature>("get_mainBodyChunk"),
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(PlayerUpdateAnimation0GStand);
                val.Emit(OpCodes.Brfalse, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerUpdateAnimation 0 g pole stand grab!");
            }
            #endregion

            #region Zero G pole grab crawl
            if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<UpdatableAndDeletable>("room"),
                x => x.MatchLdsfld<SoundID>("Slugcat_Climb_Up_Vertical_Beam")
            })) {}
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerUpdateAnimation 0 g pole grab crawl skip!");
            }

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdarg(0),
                x => x.MatchCall<Player>("get_animationFrame"),
                x => x.MatchStloc(1)
            }))
            {
                val.MoveAfterLabels();

                target = val.MarkLabel();
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerUpdateAnimation 0 g pole grab crawl label!");
            }

            if (val.TryGotoPrev(MoveType.Before, new Func<Instruction, bool>[2]
            {
                x => x.MatchLdarg(0),
                x => x.MatchCall<Creature>("get_mainBodyChunk"),
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(PlayerUpdateAnimation0GCrawl);
                val.Emit(OpCodes.Brfalse, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerUpdateAnimation 0 g pole grab crawl!");
            }
            #endregion
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    public static bool PlayerUpdateAnimationHangFromBeam(Player self)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state) || state.manipulation == 1 && state.consciousState == 0)
        {
            return true;
        }

        float multiplyer = SlugcatHooks.GetPlayerMovementSpeedMultiplier(state, 1f);

        self.bodyChunks[0].vel.x += (float)self.input[0].x * Mathf.Lerp(1.2f, 1.4f, self.Adrenaline) * self.slugcatStats.poleClimbSpeedFac * Custom.LerpMap((float)self.slowMovementStun, 0f, 10f, 1f, 0.5f) * multiplyer;

        return false;
    }
    public static bool PlayerUpdateAnimationHangFromBeam2(Player self)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state) || state.manipulation == 1 && state.consciousState == 0)
        {
            return true;
        }

        float multiplyer = SlugcatHooks.GetPlayerMovementSpeedMultiplier(state, 1f);

        self.bodyChunks[1].vel.x += (float)self.flipDirection * (0.5f + 0.5f * Mathf.Sin((float)self.animationFrame / 20f * 3.1415927f * 2f)) * -0.5f * multiplyer;

        return false;
    }
    public static bool PlayerUpdateAnimationClimbOnBeam(Player self)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state) || state.manipulation == 1 && state.moving == 1 && state.consciousState == 0)
        {
            return true;
        }

        float multiplyer = SlugcatHooks.GetPlayerMovementSpeedMultiplier(state, 0.7f);

        self.bodyChunks[0].vel.y += Mathf.Lerp(1f, 1.4f, self.Adrenaline) * self.slugcatStats.poleClimbSpeedFac * Custom.LerpMap((float)self.slowMovementStun, 0f, 10f, 1f, 0.2f) * multiplyer;

        return false;
    }
    public static bool PlayerUpdateAnimation0GStand(Player self)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state) || state.manipulation == 1 && state.consciousState == 0)
        {
            return true;
        }

        float multiplyer = SlugcatHooks.GetPlayerMovementSpeedMultiplier(state, 1f);

        self.mainBodyChunk.vel.y += (float)self.input[0].ZeroGGamePadIntVec.y * 1.05f * self.slugcatStats.poleClimbSpeedFac * multiplyer;
        
        return false;
    }
    public static bool PlayerUpdateAnimation0GCrawl(Player self)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state) || state.manipulation == 1 && state.consciousState == 0)
        {
            return true;
        }

        float multiplyer = SlugcatHooks.GetPlayerMovementSpeedMultiplier(state, 1f);

        self.mainBodyChunk.vel.x += (float)self.input[0].ZeroGGamePadIntVec.x * 1.05f * self.slugcatStats.poleClimbSpeedFac * multiplyer;

        return false;
    }

    static void ILPlayerUpdateBodyMode(ILContext il)
    {
        try
        {
            ILCursor val = new(il);
            ILLabel target = null;

            #region Multiply default stance movement speed
            if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[5]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<Player>("dynamicRunSpeed"),
                x => x.MatchLdcI4(1),
                x => x.MatchLdcR4(2),
                x => x.MatchStelemR4(),
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(PlayerUpdateStandSpeed);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerUpdateBodyMode default stand!");
            }

            if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[5]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<Player>("dynamicRunSpeed"),
                x => x.MatchLdcI4(0),
                x => x.MatchLdcR4(2.5f),
                x => x.MatchStelemR4(),
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(PlayerUpdateCrawlSpeed);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerUpdateBodyMode default crouch!");
            }
            #endregion

            #region Prevent default stance pole grabbing
            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[2]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdflda<Player>("dropGrabTile")
            })) {}
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerUpdateBodyMode default pole grab skip!");
            }

            if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[1]
            {
                x => x.MatchBrfalse(out target)
            })) {}
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerUpdateBodyMode default pole grab target!");
            }

            if (val.TryGotoPrev(MoveType.Before, new Func<Instruction, bool>[2]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdflda<Player>("dropGrabTile")
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(PlayerCanGrabPole);
                val.Emit(OpCodes.Brfalse_S, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerUpdateBodyMode default pole grab!");
            }
            #endregion

            #region Multiply stance movement speed
            if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[5]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<Player>("dynamicRunSpeed"),
                x => x.MatchLdcI4(0),
                x => x.MatchLdcR4(1),
                x => x.MatchStelemR4(),
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(PlayerUpdateCrawlSpeed);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerUpdateBodyMode crawl!");
            }

            if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[5]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<Player>("dynamicRunSpeed"),
                x => x.MatchLdcI4(1),
                x => x.MatchLdcR4(2),
                x => x.MatchStelemR4(),
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(PlayerUpdateStandSpeed);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerUpdateBodyMode stand!");
            }

            if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[12]
            {
                x => x.MatchLdloc(12),
                x => x.MatchLdcR4(0.1f),
                x => x.MatchLdcR4(1),
                x => x.MatchLdcR4(10),
                x => x.MatchLdcR4(0.0f),
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<Player>("slowMovementStun"),
                x => x.MatchConvR4(),
                x => x.MatchCall<UnityEngine.Mathf>("InverseLerp"),
                x => x.MatchCall<UnityEngine.Mathf>("Lerp"),
                x => x.MatchMul(),
                x => x.MatchStloc(12)
            }))
            {
                val.MoveBeforeLabels();

                val.Emit(OpCodes.Ldloc, 12);
                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(PlayerUpdateCorridorMult);
                val.Emit(OpCodes.Mul);
                val.Emit(OpCodes.Stloc, 12);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerUpdateBodyMode corridor!");
            }
            #endregion

            #region Prevent pole grabbing
            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[1]
            {
                x => x.MatchLdsfld<Player.AnimationIndex>("ClimbOnBeam")
            })) {}
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerUpdateBodyMode sorridor pole grab skip!");
            }

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[1]
            {
                x => x.MatchBrfalse(out target)
            })) {}
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerUpdateBodyMode sorridor pole grab target!");
            }

            if (val.TryGotoPrev(MoveType.Before, new Func<Instruction, bool>[5]
            {
                x => x.MatchLdarg(0),
                x => x.MatchCall<Player>("get_input"),
                x => x.MatchLdcI4(0),
                x => x.MatchLdelema<Player.InputPackage>(),
                x => x.MatchLdfld<Player.InputPackage>("y")
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(PlayerCanGrabPole);
                val.Emit(OpCodes.Brfalse_S, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerUpdateBodyMode!");
            }
            #endregion
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    public static float PlayerUpdateCorridorMult(Player self)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state) || state.manipulation == 1 && state.moving == 1 && state.consciousState == 0)
        {
            return 1;
        }

        float multiplyer = SlugcatHooks.GetPlayerMovementSpeedMultiplier(state, 0.4f);

        return multiplyer;
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

        TongueElectricDamage(self.State, state);

        return false;
    }

    static void ILPlayerWallJump(ILContext il)
    {
        try
        {
            ILCursor val = new(il);

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<Player>("exhausted"),
                x => x.MatchBrfalse(out _)
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldloc, 0);
                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(PlayerJump);
                val.Emit(OpCodes.Mul);
                val.Emit(OpCodes.Stloc, 0);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILPlayerWallJump!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }

    public static float PlayerJump(Player self)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state) || state.manipulation == 1 && state.moving == 1 && state.consciousState == 0)
        {
            return 1;
        }

        float multiplyer = SlugcatHooks.GetPlayerMovementSpeedMultiplier(state, 0.1f);

        return multiplyer;
    }

    public static void PlayerUpdateStandSpeed(Player self)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state) || state.moving == 1 && state.consciousState == 0)
        {
            return;
        }

        float multiplyer = SlugcatHooks.GetPlayerMovementSpeedMultiplier(state, 0f);

        self.dynamicRunSpeed[0] *= multiplyer;
        self.dynamicRunSpeed[1] *= multiplyer;
    }
    public static void PlayerUpdateCrawlSpeed(Player self)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state) || state.manipulation == 1 && state.moving == 1 && state.consciousState == 0)
        {
            return;
        }

        float multiplyer = SlugcatHooks.GetPlayerMovementSpeedMultiplier(state, 0.5f);

        self.dynamicRunSpeed[0] *= multiplyer;
        self.dynamicRunSpeed[1] *= multiplyer;
    }

    public static bool PlayerCanGrabPole(Player self)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state) || WorkingArms(state) > 0)
        {
            return true;
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

        RainDamage(self.State, state, false);
    }
    public static bool RoomRainThrowAroundObjectsDie(Player self)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return true;
        }

        RainDamage(self.State, state, true);

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

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[4]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdarg(1),
                x => x.MatchLdarg(2),
                x => x.MatchCall<Spear>("LodgeInCreature"),
            }))
            {
                val.MoveAfterLabels();

                target = val.MarkLabel();
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILSpearHitSomething sound target!");
            }

            if (val.TryGotoPrev(MoveType.Before, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<UpdatableAndDeletable>("room"),
                x => x.MatchLdsfld<SoundID>("Spear_Stick_In_Creature"),
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.EmitDelegate(SpearHitSomethingSound);
                val.Emit(OpCodes.Brfalse, target);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILSpearHitSomething sound!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }

    public static bool SpearHitSomethingSound(Spear self)
    {
        if (!weaponstat.TryGetValue(self.abstractPhysicalObject, out RWWeaponStats weaponState) || !weaponState.wasDeflected)
        {
            return true;
        }

        return false;
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
            SpiderDamage(self.State, state);
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

        foreach (RWAffliction affliction in state.wholeBodyAfflictions)
        {
            if (affliction is RWToxicBuildup tempToxicBuildup)
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

        foreach (RWAffliction affliction in state.wholeBodyAfflictions)
        {
            if (affliction is RWToxicBuildup tempToxicBuildup)
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

    #region SporePlant(Bee)
    static void ILNewSporePlantAttachedBee(ILContext il)
    {
        try
        {
            ILCursor val = new(il);

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[4]
            {
                x => x.MatchLdarg(3),
                x => x.MatchCallvirt<BodyChunk>("get_owner"),
                x => x.MatchIsinst(typeof(Creature)),
                x => x.MatchLdnull()
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_3);
                val.Emit<BodyChunk>(OpCodes.Callvirt, "get_owner");
                val.Emit(OpCodes.Isinst, typeof(Creature));
                val.EmitDelegate(SporePlantAttachedBee);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILNewSporePlantAttachedBee!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    static void ILSporePlantAttachedBeeUpdate(ILContext il)
    {
        try
        {
            ILCursor val = new(il);

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[5]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<SporePlant.AttachedBee>("attachedChunk"),
                x => x.MatchCallvirt<BodyChunk>("get_owner"),
                x => x.MatchIsinst(typeof(Creature)),
                x => x.MatchLdnull()
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.Emit<SporePlant.AttachedBee>(OpCodes.Ldfld, "attachedChunk");
                val.Emit<BodyChunk>(OpCodes.Callvirt, "get_owner");
                val.Emit(OpCodes.Isinst, typeof(Creature));
                val.EmitDelegate(SporePlantAttachedBee);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILSporePlantAttachedBeeUpdate!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }

    public static void SporePlantAttachedBee(Creature self)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return;
        }

        state.violenceAttackOverride = "Bee - Stinger";
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

        RWHealthState.Damage(self.State, state, new RWBomb(), 999999f, 999, state.consciousnessSource, "Oracle - Explosion", "Oracle");
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

        TongueElectricDamage(self.State, state);

        return false;
    }
    #endregion

    #region UnderwaterShock
    static void ILUnderwaterShockUpdate(ILContext il)
    {
        try
        {
            ILCursor val = new(il);

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[1]
            {
                x => x.MatchCallvirt<Creature>("Violence")
            })) {}
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILUnderwaterShockUpdate skip!");
            }

            if (val.TryGotoPrev(MoveType.After, new Func<Instruction, bool>[2]
            {
                x => x.MatchNewobj(typeof(UnderwaterShock.Flash)),
                x => x.MatchCallvirt<Room>("AddObject")
            }))
            {
                val.MoveBeforeLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.Emit<UpdatableAndDeletable>(OpCodes.Ldfld, "room");
                val.Emit<Room>(OpCodes.Callvirt, "get_abstractRoom");
                val.Emit<AbstractRoom>(OpCodes.Ldfld, "creatures");
                val.Emit(OpCodes.Ldloc_1);
                val.Emit(OpCodes.Callvirt, typeof(List<AbstractCreature>).GetMethod("get_Item"));
                val.Emit<AbstractCreature>(OpCodes.Callvirt, "get_realizedCreature");
                val.Emit(OpCodes.Ldarg_0);
                val.Emit<UnderwaterShock>(OpCodes.Ldfld, "killTagHolder");
                val.EmitDelegate(UnderwaterShockUpdate);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILUnderwaterShockUpdate!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    public static void UnderwaterShockUpdate(Creature self, Creature attacker)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return;
        }

        if (attacker != null)
        {
            state.violenceAttackerOverride = GetCreatureName(attacker);

            state.violenceAttackOverride = state.violenceAttackerOverride + " - Underwater Shock";
        }
        else
        {
            state.violenceAttackOverride = "Underwater Shock";
        }
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

        float damage = 10;
        float AP = 0;

        if (weaponstat.TryGetValue(weapon.abstractPhysicalObject, out RWWeaponStats weaponState))
        {
            damage = weaponState.damage;
            AP = weaponState.AP;
        }

        RWHealthState.Damage(self.State, state, new RWStab(), damage, AP, GetHitBodyPart(state, null, null, true), weapon.ToString(), attacker.ToString());

        return false;
    }
    public static bool WeaponHitSomethingWithoutStoppingRockDie(Creature self, Creature attacker, Weapon weapon)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return true;
        }

        float damage = 10;
        float AP = 0;

        if (weaponstat.TryGetValue(weapon.abstractPhysicalObject, out RWWeaponStats weaponState))
        {
            damage = weaponState.damage;
            AP = weaponState.AP;
        }

        BluntDamage(self.State, state, null, damage, AP, "Rock");
        self.Stun(80);

        return false;
    }
    public static void WeaponHitSomethingWithoutStoppingRock(Creature self, Creature attacker, Weapon weapon)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return;
        }

        float damage = 10;
        float AP = 0;

        if (weaponstat.TryGetValue(weapon.abstractPhysicalObject, out RWWeaponStats weaponState))
        {
            damage = weaponState.damage;
            AP = weaponState.AP;
        }

        BluntDamage(self.State, state, null, damage, AP, "Rock");
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

        ZapCoilDamage(self.State, state);

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

        BigJellyfishDamage(self.State, state);

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

    #region SlugNPCAI
    static void ILSlugNPCAIUpdate(ILContext il)
    {
        try
        {
            ILCursor val = new(il);

            if (val.TryGotoNext(MoveType.Before, new Func<Instruction, bool>[5]
            {
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<ArtificialIntelligence>("creature"),
                x => x.MatchLdfld<AbstractCreature>("abstractAI"),
                x => x.MatchLdloc(1),
                x => x.MatchCallvirt<AbstractCreatureAI>("SetDestination")
            }))
            {
                val.MoveAfterLabels();

                val.Emit(OpCodes.Ldarg_0);
                val.Emit(OpCodes.Ldloc_1);
                val.EmitDelegate(SlugNPCAIUpdate);
                val.Emit(OpCodes.Stloc_1);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILSlugNPCAIUpdate!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    public static WorldCoordinate SlugNPCAIUpdate(MoreSlugcats.SlugNPCAI self, WorldCoordinate destination)
    {
        if (self.creature.controlled || !healthState.TryGetValue(self.cat.State, out RWState state))
        {
            return destination;
        }

        if (self.behaviorType == SlugOnFire)
        {
            for (int i = 0; i < self.cat.grasps.Length; i++)
            {
                self.cat.ReleaseGrasp(i);
            }

            return new WorldCoordinate(self.cat.abstractCreature.pos.room, self.cat.abstractCreature.pos.x + (UnityEngine.Random.value > 0.5f ? 2 : -2), self.cat.abstractCreature.pos.y, self.cat.abstractCreature.pos.abstractNode);
        }

        if (self.behaviorType != SlugTend && self.behaviorType != SlugSelfTend)
        {
            return destination;
        }

        if (self.behaviorType == SlugSelfTend)
        {
            return self.cat.abstractCreature.pos;
        }

        if (state.tendTarget == null || state.tendTarget.State == null || !healthState.TryGetValue(state.tendTarget.State, out RWState otherState))
        {
            stopTend();

            return destination;
        }

        self.grabTarget = null;

        if (!self.HoldingThis(state.tendTarget))
        {
            if (NPCGrabCheck(state.tendTarget))
            {
                self.cat.NPCForceGrab(state.tendTarget);
            }
            return new WorldCoordinate(state.tendTarget.abstractCreature.pos.room, state.tendTarget.abstractCreature.pos.x + (self.cat.abstractCreature.pos.x < state.tendTarget.abstractCreature.pos.x ? 2 : -2), state.tendTarget.abstractCreature.pos.y, state.tendTarget.abstractCreature.pos.abstractNode);
        }

        if (state.tendAffliction != null)
        {
            if (state.tendAffliction.isTended || state.tendAffliction.part == null && state.tendAffliction is not RWDisease)
            {
                state.tendAffliction = null;

                stopTend();
            }

            return self.cat.abstractCreature.pos;
        }

        if (otherState.onFire)
        {
            state.tendTime = 10;
            state.tendAffliction = burningAffliction;
            state.tendTimeMax = state.tendTime;
            return self.cat.abstractCreature.pos;
        }

        RWInjury bleeding = null;
        RWDisease diseaseAffliction = null;
        RWInjury untendedAffliction = null;

        foreach (RWBodyPart part in otherState.bodyParts)
        {
            foreach (RWAffliction affliction in part.afflictions)
            {
                if (!affliction.isTended)
                {
                    if (affliction is RWInjury injury)
                    {
                        if (injury.isBleeding)
                        {
                            if (bleeding != null)
                            {
                                if (injury.healingDifficulty.bleeding * injury.damage > bleeding.healingDifficulty.bleeding * bleeding.damage)
                                {
                                    bleeding = injury;
                                }
                            }
                            else bleeding ??= injury;
                        }
                        else
                        {
                            if (untendedAffliction != null)
                            {
                                if (injury.damage > untendedAffliction.damage)
                                {
                                    untendedAffliction = injury;
                                }
                            }
                            else untendedAffliction ??= injury;

                        }
                    }
                    else if (affliction is RWDisease disease && disease.timeUntilTreatment <= 0)
                    {
                        if (diseaseAffliction != null)
                        {
                            if (disease.severity > diseaseAffliction.severity)
                            {
                                diseaseAffliction = disease;
                            }
                        }
                        else diseaseAffliction ??= disease;
                    }
                    else
                    {
                        Debug.Log(all + affliction + " does not belong to any tendable check");
                        RimWorldHealth.Logger.LogError(all + affliction + " does not belong to any tendable check");
                    }
                }
                else if (affliction is RWDisease disease && disease.timeUntilTreatment <= 0)
                {
                    if (diseaseAffliction != null)
                    {
                        if (disease.severity > diseaseAffliction.severity)
                        {
                            diseaseAffliction = disease;
                        }
                    }
                    else diseaseAffliction ??= disease;
                }
            }
        }

        if (bleeding == null)
        {
            foreach (RWAffliction affliction in otherState.wholeBodyAfflictions)
            {
                if (affliction is RWDisease disease && disease.timeUntilTreatment <= 0)
                {
                    if (diseaseAffliction != null)
                    {
                        if (disease.severity > diseaseAffliction.severity)
                        {
                            diseaseAffliction = disease;
                        }
                    }
                    else diseaseAffliction ??= disease;
                }
            }
        }

        if (bleeding != null)
        {
            startTending(bleeding);
        }
        else if (diseaseAffliction != null)
        {
            startTending(diseaseAffliction);
        }
        else if (untendedAffliction != null)
        {
            startTending(untendedAffliction);
        }

        return self.cat.abstractCreature.pos;

        void startTending(RWAffliction affliction)
        {
            state.tendAffliction = affliction;
            state.tendTime = Mathf.Round(state.tendTimeBase / RWHealthState.MedicalTendSpeed(state));
            state.tendTimeMax = state.tendTime;
        }

        bool NPCGrabCheck(PhysicalObject item)
        {
            if (item.room == null || item.room != self.cat.room || item.grabbedBy.Count > 0)
            {
                return false;
            }

            int num = 0;
            if (self.cat.Grabability(item) == Player.ObjectGrabability.Drag)
            {
                float dst = float.MaxValue;
                for (int i = 0; i < item.bodyChunks.Length; i++)
                {
                    if (Custom.DistLess(self.cat.mainBodyChunk.pos, item.bodyChunks[i].pos, dst))
                    {
                        dst = Vector2.Distance(self.cat.mainBodyChunk.pos, item.bodyChunks[i].pos);
                        num = i;
                    }
                }
            }
            return Custom.DistLess(self.cat.bodyChunks[0].pos, item.bodyChunks[num].pos, item.bodyChunks[num].rad + 40f) && (Custom.DistLess(self.cat.bodyChunks[0].pos, item.bodyChunks[num].pos, item.bodyChunks[num].rad + 20f) || self.cat.room.VisualContact(self.cat.bodyChunks[0].pos, item.bodyChunks[num].pos));
        }

        void stopTend()
        {
            state.wasLastBehaviourTend = false;

            state.tendAffliction = null;

            if (state.tendTarget != null)
            {
                foreach (var grasp in self.creature.realizedCreature.grasps)
                {
                    if (grasp != null && grasp.grabbedChunk != null && grasp.grabbedChunk.owner != null && grasp.grabbedChunk.owner == state.tendTarget)
                    {
                        (self.creature.realizedCreature as Player).ReleaseGrasp(grasp.graspUsed);
                    }
                }
            }

            state.tendTarget = null;
        }
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

        ARZapperDamage(self.State, state, false);
    }
    public static bool ARZapperZapperContactDie(Creature self)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return true;
        }

        ARZapperDamage(self.State, state, true);

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

    #region FlameJet
    static void ILFlameJetUpdateDamage(ILContext il)
    {
        try
        {
            ILCursor val = new(il);

            if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[3]
            {
                x => x.MatchLdloc(2),
                x => x.MatchCallvirt<Creature>("get_dead"),
                x => x.MatchBrtrue(out _)
            }))
            {
                val.MoveBeforeLabels();

                val.Emit(OpCodes.Ldloc_2);
                val.EmitDelegate(FlameJetUpdateDamage);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILFlameJetUpdateDamage!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    public static void FlameJetUpdateDamage(Creature self)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return;
        }

        state.violenceAttackOverride = "Flame Jet";
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

    #region Lightning
    static void ILLightningStrikeAOEUpdate(ILContext il)
    {
        try
        {
            ILCursor val = new(il);

            if (val.TryGotoNext(MoveType.After, new Func<Instruction, bool>[2]
            {
                x => x.MatchLdcR4(0.7f),
                x => x.MatchBgeUn(out _)
            }))
            {
                val.MoveBeforeLabels();

                val.Emit(OpCodes.Ldloc, 11);
                val.EmitDelegate(LightningStrikeAOEUpdate);
            }
            else
            {
                RimWorldHealth.Logger.LogInfo(all + "Could not find match for ILLightningStrikeAOEUpdate!");
            }
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    public static void LightningStrikeAOEUpdate(Creature self)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return;
        }

        state.violenceAttackOverride = "Lightning";
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

        SandstormDamage(self.State, state, false);
    }
    public static bool SandstormAffectObjectsDie(Player self)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return true;
        }

        SandstormDamage(self.State, state, true);

        return false;
    }
    #endregion

    #region Tardigrade
    static void ILTardigradeBitByPlayer(ILContext il)
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
        stun = (2 * 30f + stun) / self.Template.baseStunResistance;

        if (self.State is HealthState)
        {
            stun *= 1.5f;
        }
        if (type.Index != -1 && self.Template.damageRestistances[type.Index, 1] > 0f)
        {
            stun /= self.Template.damageRestistances[type.Index, 1];
        }

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