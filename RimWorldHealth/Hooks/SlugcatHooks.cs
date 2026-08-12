using RWCustom;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using static ShadowOfRimWorldHealth.RimWorldHealth;

namespace ShadowOfRimWorldHealth;

internal class SlugcatHooks
{
    public static void Apply()
    {
        #region Player
        On.Player.ctor += NewPlayer;
        On.Player.AddFood += PlayerAddFood;
        On.Player.DeathByBiteMultiplier += PlayerDeathByBiteMultiplier;
        On.Player.GrabUpdate += PlayerGrabUpdate;
        On.Player.GraphicsModuleUpdated += PlayerGraphicsModuleUpdated;
        On.Player.LungUpdate += PlayerLungUpdate;
        On.Player.ObjectEaten += PlayerObjectEaten;
        On.Player.PyroDeath += PlayerPyroDeath;
        On.Player.SubtractFood += PlayerSubtractFood;
        On.Player.ThrownSpear += PlayerThrownSpear;
        On.Player.ThrowObject += PlayerThrowObject;
        On.Player.Update += PlayerUpdate;
        #endregion

        #region PlayerGraphics
        On.PlayerGraphics.DrawSprites += PlayerGraphicsDrawSprites;
        #endregion

        #region SlugNPCAI
        On.MoreSlugcats.SlugNPCAI.AttackingThreat += SlugNPCAIAttackingThreat;
        On.MoreSlugcats.SlugNPCAI.DecideBehavior += SlugNPCAIDecideBehavior;
        On.MoreSlugcats.SlugNPCAI.Move += SlugNPCAIMove;
        #endregion
    }

    #region Player
    static void NewPlayer(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
    {
        orig(self, abstractCreature, world);

        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return;
        }

        if (self.SlugCatClass.value == "Spear")
        {
            Tongue tongue = null;

            foreach (RWBodyPart part in state.bodyParts)
            {
                if (part is Tongue tempTongue)
                {
                    tongue = tempTongue;
                    state.talkingBP.Remove(tempTongue);
                }
                else if (part is Jaw jaw)
                {
                    jaw.capacity.Clear();
                    state.eatingBP.Remove(jaw);
                    state.talkingBP.Remove(jaw);
                }
                else if (part is Neck neck)
                {
                    neck.capacity.Remove("Eating");
                    state.eatingBP.Remove(neck);
                    neck.capacity.Remove("Talking");
                    state.talkingBP.Remove(neck);
                }
            }

            if (tongue != null)
            {
                state.bodyParts.Remove(tongue);
            }
        }

        state.poleClimbSpeedFac = self.slugcatStats.poleClimbSpeedFac;
        state.corridorClimbSpeedFac = self.slugcatStats.corridorClimbSpeedFac;
        state.runspeedFac = self.slugcatStats.runspeedFac;
        state.swimForceFac = self.slugcatStats.swimForceFac;

        state.updateCapacities = true;
        RWHealthState.Update(self.State, state);

        if (state.moving == 1 && state.manipulation == 1 && state.consciousState == 0)
        {
            return;
        }

        float tempMoving = state.consciousState == 0 ? state.moving : 0;
        float tempManipulation = state.consciousState == 0 ? state.manipulation : state.manipulation / 2;

        self.slugcatStats.poleClimbSpeedFac = Mathf.Max(0.05f, state.poleClimbSpeedFac * (1 + (tempMoving - 1f) * 0.4f) * (1 + (tempManipulation - 1f) * 0.6f));
        self.slugcatStats.corridorClimbSpeedFac = Mathf.Max(0.05f, state.corridorClimbSpeedFac * (1 + (tempMoving - 1f) * 0.6f) * (1 + (tempManipulation - 1f) * 0.4f));
        self.slugcatStats.runspeedFac = Mathf.Max(0.05f, state.runspeedFac * tempMoving);
        self.slugcatStats.swimForceFac = Mathf.Max(0.05f, state.swimForceFac * (1 + (tempMoving - 1f) * 0.4f) * (1 + (tempManipulation - 1f) * 0.6f));
    }

    static void PlayerAddFood(On.Player.orig_AddFood orig, Player self, int add)
    {
        orig(self, add);

        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return;
        }

        state.hasEaten = true;
    }

    static float PlayerDeathByBiteMultiplier(On.Player.orig_DeathByBiteMultiplier orig, Player self)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out _))
        {
            return orig(self);
        }

        return 1;
    }

    static void PlayerGrabUpdate(On.Player.orig_GrabUpdate orig, Player self, bool eu)
    {
        orig(self, eu);

        if (self.State == null || !healthState.TryGetValue(self.State, out RWState _))
        {
            return;
        }

        //???
    }

    static void PlayerGraphicsModuleUpdated(On.Player.orig_GraphicsModuleUpdated orig, Player self, bool actuallyViewed, bool eu)
    {
        orig(self, actuallyViewed, eu);

        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state) || self.grasps[0] == null || ArmCheck(state) || !JawCheck(state))
        {
            return;
        }

        Vector2 headPos = self.bodyChunks[0].pos;
        if (self.graphicsModule != null)
        {
            headPos = (self.graphicsModule as PlayerGraphics).head.pos - Custom.DirVec(self.bodyChunks[1].pos, headPos) * 4f + (self.graphicsModule as PlayerGraphics).lookDirection * 4f;
        }

        if (!self.HeavyCarry(self.grasps[0].grabbed) && actuallyViewed)
        {
            self.grasps[0].grabbed.firstChunk.vel = self.bodyChunks[0].vel;
            self.grasps[0].grabbed.firstChunk.MoveFromOutsideMyUpdate(eu, headPos);

            if (self.grasps[0].grabbed is Weapon weapon)
            {
                weapon.setRotation = new Vector2?(Custom.PerpendicularVector(Custom.DirVec(self.bodyChunks[1].pos, headPos) * -1f));
                weapon.rotationSpeed = 0f;
                weapon.ChangeOverlap(true);
            }
        }
        else
        {
            if (!self.HeavyCarry(self.grasps[0].grabbed))
            {
                self.grasps[0].grabbed.firstChunk.pos = headPos;
                self.grasps[0].grabbed.firstChunk.vel = self.mainBodyChunk.vel;
            }
        }
    }

    static void PlayerLungUpdate(On.Player.orig_LungUpdate orig, Player self)
    {
        orig(self);

        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return;
        }

        RWAirInLungs airInLungs = null;

        foreach (RWAffliction affliction in state.wholeBodyAfflictions)
        {
            if (affliction is RWAirInLungs tempAirInLungs)
            {
                airInLungs = tempAirInLungs;
                break;
            }
        }

        if (self.airInLungs >= 1)
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
                airInLungs.tendQuality = self.airInLungs;
            }
            else
            {
                state.wholeBodyAfflictions.Add(new RWAirInLungs(self.State, null, self.airInLungs));
            }
        }
    }

    static void PlayerObjectEaten(On.Player.orig_ObjectEaten orig, Player self, IPlayerEdible edible)
    {
        orig(self, edible);

        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state) || !ShadowOfOptions.karma_flower.Value || edible is not KarmaFlower || self.room.game.session is StoryGameSession && ShadowOfOptions.karma_flower.Value && !(self.room.game.session as StoryGameSession).saveState.deathPersistentSaveData.reinforcedKarma)
        {
            return;
        }

        KarmaFlowerHeal(self, state);
    }

    static void PlayerPyroDeath(On.Player.orig_PyroDeath orig, Player self)
    {
        orig(self);

        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return;
        }

        ArtiLungExplosion(self.State, state);
    }

    static void PlayerSubtractFood(On.Player.orig_SubtractFood orig, Player self, int sub)
    {
        orig(self, sub);

        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state) || self.playerState.foodInStomach > 0)
        {
            return;
        }

        state.hasEaten = false;
    }

    static void PlayerThrownSpear(On.Player.orig_ThrownSpear orig, Player self, Spear spear)
    {
        orig(self, spear);

        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state) || state.consciousState == 0)
        {
            return;
        }

        spear.spearDamageBonus /= 2;
    }

    static void PlayerThrowObject(On.Player.orig_ThrowObject orig, Player self, int grasp, bool eu)
    {
        if (self.grasps[grasp].grabbed is not Weapon || self.State == null || !healthState.TryGetValue(self.State, out RWState state) || state.consciousState == 0 || ShadowOfOptions.downed_combat.Value != "No one" || ShadowOfOptions.downed_combat.Value == "Player only" && ModManager.MSC && self.State is MoreSlugcats.PlayerNPCState || (state.consciousState == 3 && !ShadowOfOptions.player_uncon_movement.Value))
        {
            orig(self, grasp, eu);
            return;
        }

        if (ModManager.MSC && self.SlugCatClass == MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Gourmand && self.grasps[grasp].grabbed is Spear)
        {
            self.aerobicLevel = 1f;
        }
        else
        {
            self.AerobicIncrease(0.75f);
        }
           
        IntVector2 intVector = new(self.ThrowDirection, 0);
        bool flag = self.input[0].y < 0;
        if (ModManager.MMF && MoreSlugcats.MMF.cfgUpwardsSpearThrow.Value)
        {
            flag = (self.input[0].y != 0);
        }
        if (self.animation == Player.AnimationIndex.Flip && flag && self.input[0].x == 0)
        {
            intVector = new(0, (ModManager.MMF && MoreSlugcats.MMF.cfgUpwardsSpearThrow.Value) ? self.input[0].y : -1);
        }
        if (ModManager.MMF && self.bodyMode == Player.BodyModeIndex.ZeroG && MoreSlugcats.MMF.cfgUpwardsSpearThrow.Value)
        {
            int y = self.input[0].y;
            if (y != 0)
            {
                intVector = new(0, y);
            }
            else
            {
                intVector = new(self.ThrowDirection, 0);
            }
        }

        self.TossObject(grasp, eu);

        self.ThrownSpear(self.grasps[grasp].grabbed as Spear);

        if (self.animation == Player.AnimationIndex.BellySlide && self.rollCounter > 8 && self.rollCounter < 15)
        {
            if (intVector.x == self.rollDirection && self.slugcatStats.throwingSkill > 0)
            {
                self.grasps[grasp].grabbed.firstChunk.vel.x += (float)intVector.x * 15f;
                if ((self.grasps[grasp].grabbed as Weapon).HeavyWeapon)
                {
                    (self.grasps[grasp].grabbed as Weapon).floorBounceFrames = 30;
                    if (self.grasps[grasp].grabbed is Spear)
                    {
                        (self.grasps[grasp].grabbed as Spear).alwaysStickInWalls = true;
                    }
                    self.grasps[grasp].grabbed.firstChunk.goThroughFloors = false;
                    self.grasps[grasp].grabbed.firstChunk.vel.y -= 5f;
                }
                (self.grasps[grasp].grabbed as Weapon).changeDirCounter = 0;
            }
            else if (intVector.x == -self.rollDirection && !self.longBellySlide)
            {
                self.grasps[grasp].grabbed.firstChunk.vel.y += ((self.grasps[grasp].grabbed is Spear) ? 3f : 5f);
                (self.grasps[grasp].grabbed as Weapon).changeDirCounter = 0;
                self.rollCounter = 8;
                self.mainBodyChunk.pos.x += (float)self.rollDirection * 6f;
                self.room.AddObject(new ExplosionSpikes(self.room, self.bodyChunks[1].pos + new Vector2((float)self.rollDirection * -40f, 0f), 6, 5.5f, 4f, 4.5f, 21f, new Color(1f, 1f, 1f, 0.25f)));
                self.bodyChunks[1].pos.x += (float)self.rollDirection * 6f;
                self.bodyChunks[1].pos.y += 17f;
                self.mainBodyChunk.vel.x += (float)self.rollDirection * 16f;
                self.bodyChunks[1].vel.x += (float)self.rollDirection * 16f;
                self.room.PlaySound(SoundID.Slugcat_Rocket_Jump, self.mainBodyChunk, false, 1f, 1f);
                self.exitBellySlideCounter = 0;
                self.longBellySlide = true;
            }
        }
        if (self.animation == Player.AnimationIndex.ClimbOnBeam && ModManager.MMF && MoreSlugcats.MMF.cfgClimbingGrip.Value)
        {
            self.bodyChunks[0].vel += intVector.ToVector2() * 2f;
            self.bodyChunks[1].vel -= intVector.ToVector2() * 8f;
        }
        else
        {
            self.bodyChunks[0].vel += intVector.ToVector2() * 8f;
            self.bodyChunks[1].vel -= intVector.ToVector2() * 4f;
        }
        if (self.graphicsModule != null)
        {
            (self.graphicsModule as PlayerGraphics).ThrowObject(grasp, self.grasps[grasp].grabbed);
        }
        self.Blink(15);

        self.dontGrabStuff = (self.isNPC ? 45 : 15);
        if (self.graphicsModule != null)
        {
            (self.graphicsModule as PlayerGraphics).LookAtObject(self.grasps[grasp].grabbed);
        }
        if (self.grasps[grasp].grabbed is PlayerCarryableItem)
        {
            (self.grasps[grasp].grabbed as PlayerCarryableItem).Forbid();
        }
        self.ReleaseGrasp(grasp);
    }

    static void PlayerUpdate(On.Player.orig_Update orig, Player self, bool eu)
    {
        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            orig(self, eu);
            return;
        }

        if (!LegCheck(state))
        {
            self.standing = false;

            if (self.animation == Player.AnimationIndex.StandOnBeam)
            {
                self.animation = Player.AnimationIndex.ClimbOnBeam;
            }
        }

        orig(self, eu);

        if (!ArmCheck(state))
        {
            if (self.grasps.Length > 0 && !JawCheck(state))
            {
                self.grasps[0]?.Release();
            }

            if (self.grasps.Length > 1)
                self.grasps[1]?.Release();
        }
        else if (state.armSetNames.Count > 1)
        {
            if (state.armSet[state.armSetNames[0]].efficiency <= 0 && state.armSet[state.armSetNames[1]].efficiency > 0 && self.grasps[1] == null)
            {
                self.SwitchGrasps(0, 1);

                return;
            }

            for (int i = 0; i < state.armSetNames.Count; i++)
            {
                if (state.armSet[state.armSetNames[i]].efficiency <= 0)
                {
                    self.grasps[i]?.Release();
                }
            }
        }

        if (self.dead)
        {
            return;
        }

        for (int i = 0; i < state.visualBleedAmount.Count; i++)
        {
            List<float> bleedInfo = state.visualBleedAmount[i];

            if (bleedInfo[0] > 0 && self.bodyChunks.Count() >= i)
            {
                bleedInfo[1]--;

                if (bleedInfo[1] <= 0)
                {
                    self.room.AddObject(new BloodDrip(self.bodyChunks[i].pos + new Vector2(UnityEngine.Random.Range(-self.bodyChunks[i].rad, self.bodyChunks[i].rad), UnityEngine.Random.Range(-self.bodyChunks[i].rad, self.bodyChunks[i].rad)), default, false));

                    bleedInfo[1] = Custom.LerpMap(bleedInfo[0], 0, 120, 25, 1);
                }
            }
        }

        if (state.visualDisease[0] > 0)
        {
            state.visualDisease[1]--;

            if (state.visualDisease[1] <= 0)
            {
                Vector2 pos = self.firstChunk.pos;
                if (self.graphicsModule != null)
                {
                    PlayerGraphics playerGraphics = self.graphicsModule as PlayerGraphics;
                    float num = Mathf.Sin(playerGraphics.breath * 3.1415927f * 2f);
                    float num2 = Mathf.Sin(playerGraphics.lastBreath * 3.1415927f * 2f);
                    if (playerGraphics != null && num < num2 && num < 0.5f && num > -0.5f)
                    {
                        Vector2 vector = playerGraphics.lookDirection * 8f;
                        Vector2 b = new(0f, 5f);
                        if (self.bodyMode == Player.BodyModeIndex.Crawl)
                        {
                            vector = playerGraphics.lookDirection * 16f;
                            b.x = (float)self.flipDirection * 20f;
                        }
                        //self.room.AddObject(new MoreSlugcats.ColdRoom.ColdBreath(pos + b + vector, Custom.RNV() * 0.2f + vector * 0.1f + self.firstChunk.vel * 0.25f, UnityEngine.Random.value * 20f + 5f));
                        DiseaseCloud sporeCloud = new(pos + b + vector, Custom.RNV() * 0.2f + vector * 0.1f + self.firstChunk.vel * 0.25f, new Color(0.2f, 1f, 0.2f), 1f, null, 20, null, self.abstractPhysicalObject.rippleLayer)
                        {
                            nonToxic = true
                        };
                        self.room.AddObject(sporeCloud);
                    }
                }

                state.visualDisease[1] = Custom.LerpMap(state.visualDisease[0], 0, 100, 25, 1);
            }
        }
    }
    #endregion

    #region PlayerGraphics
    static void PlayerGraphicsDrawSprites(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        orig(self, sLeaser, rCam, timeStacker, camPos);

        if (sLeaser.deleteMeNextFrame || !healthState.TryGetValue(self.player.State, out RWState state))
        {
            return;
        }

        if (!ArmCheck(state)) //&& JawCheck(state) && self.player.grasps[0] != null
        {
            sLeaser.sprites[5].isVisible = false;
            sLeaser.sprites[6].isVisible = false;
            sLeaser.sprites[7].isVisible = false;
            sLeaser.sprites[8].isVisible = false;
        }
    }
    #endregion

    #region SlugNPCAI
    static bool SlugNPCAIAttackingThreat(On.MoreSlugcats.SlugNPCAI.orig_AttackingThreat orig, MoreSlugcats.SlugNPCAI self)
    {
        if (self.cat.State != null && healthState.TryGetValue(self.cat.State, out RWState state) && state.consciousState != 0 && ShadowOfOptions.downed_combat.Value != "Everyone")
        {
            return false;
        }

        return orig(self);
    }
    static void SlugNPCAIDecideBehavior(On.MoreSlugcats.SlugNPCAI.orig_DecideBehavior orig, MoreSlugcats.SlugNPCAI self)
    {
        orig(self);

        if (self.creature.controlled || !healthState.TryGetValue(self.cat.State, out RWState state) || state.consciousState == 3)
        {
            return;
        }

        if (state.consciousState != 0)
        {
            if (self.behaviorType == MoreSlugcats.SlugNPCAI.BehaviorType.Attacking && ShadowOfOptions.downed_combat.Value != "Everyone")
            {
                if (self.friendTracker.friend != null && self.abstractAI.toldToStay == null)
                {
                    self.behaviorType = MoreSlugcats.SlugNPCAI.BehaviorType.Following;
                }
                else
                {
                    self.behaviorType = MoreSlugcats.SlugNPCAI.BehaviorType.Idle;
                }
            }

            if (ShadowOfOptions.downed_tend.Value != "Everyone")
            {
                return;
            }
        }

        if (self.behaviorType == MoreSlugcats.SlugNPCAI.BehaviorType.BeingHeld || self.behaviorType == MoreSlugcats.SlugNPCAI.BehaviorType.OnHead || self.behaviorType == MoreSlugcats.SlugNPCAI.BehaviorType.Thrown && self.cat.bodyMode == Player.BodyModeIndex.Default || self.behaviorType == MoreSlugcats.SlugNPCAI.BehaviorType.Attacking || self.behaviorType == MoreSlugcats.SlugNPCAI.BehaviorType.Fleeing)
        {
            if (state.tendAffliction != null)
            {
                state.tendAffliction = null;
            }

            foreach (var grasp in self.creature.realizedCreature.grasps)
            {
                if (grasp.grabbedChunk != null && grasp.grabbedChunk.owner != null && grasp.grabbedChunk.owner is Creature)
                {
                    (self.creature.realizedCreature as Player).ReleaseGrasp(grasp.graspUsed);
                }
            }

            return;
        }

        if (self.behaviorType == SlugTend || self.behaviorType == SlugSelfTend)
        {
            return;
        }

        RWInjury bleeding = null;
        RWDisease diseaseAffliction = null;
        RWInjury untendedAffliction = null;

        foreach (RWBodyPart part in state.bodyParts)
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
            foreach (RWAffliction affliction in state.wholeBodyAfflictions)
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
            startSelfTend(bleeding);
            return;
        }
        else if (diseaseAffliction != null)
        {
            startSelfTend(diseaseAffliction);
            return;
        }
        else if (untendedAffliction != null)
        {
            startSelfTend(untendedAffliction);
            return;
        }

        Creature tendTarget = null;

        if (self.friendTracker.friend != null && !self.friendTracker.friend.dead && self.friendTracker.friend.Stunned && (grabbedByThis(self.friendTracker.friend) || self.friendTracker.friend.grabbedBy.Count == 0) && self.friendTracker.friend.State != null && healthState.TryGetValue(self.friendTracker.friend.State, out RWState otherState))
        {
            foreach (RWBodyPart part in otherState.bodyParts)
            {
                foreach (RWAffliction affliction in part.afflictions)
                {
                    if (!affliction.isTended)
                    {
                        tendOther(self.friendTracker.friend);
                        return;
                    }
                    else if (affliction is RWDisease disease && disease.timeUntilTreatment <= 0)
                    {
                        tendOther(self.friendTracker.friend);
                        return;
                    }
                }
            }

            foreach (RWAffliction affliction in state.wholeBodyAfflictions)
            {
                if (affliction is RWDisease disease && disease.timeUntilTreatment <= 0)
                {
                    tendOther(self.friendTracker.friend);
                    return;
                }
            }
        }

        if (self.friendTracker.friend != null && self.friendTracker.friend.room != self.cat.room)
        {
            if (state.tendAffliction != null)
            {
                state.tendAffliction = null;
            }

            return;
        }

        foreach (AbstractCreature abstrCrit in self.creature.Room.creatures)
        {
            if (abstrCrit.realizedCreature == null)
            {
                continue;
            }

            Creature creature = abstrCrit.realizedCreature;

            if (creature == null || creature.dead || !creature.Stunned || (creature.grabbedBy.Count != 0 && !grabbedByThis(creature)) || creature.State == null || !healthState.TryGetValue(creature.State, out otherState) || (creature is not Player && (creature is not Lizard lizard || lizard.AI == null || lizard.AI.friendTracker.friend == null || lizard.AI.friendTracker.friend is not Player)) || (tendTarget != null && Custom.WorldCoordFloatDist(tendTarget.abstractCreature.pos, self.cat.abstractCreature.pos) > Custom.WorldCoordFloatDist(creature.abstractCreature.pos, self.cat.abstractCreature.pos)))
            {
                continue;
            }

            foreach (RWBodyPart part in otherState.bodyParts)
            {
                foreach (RWAffliction affliction in part.afflictions)
                {
                    if (!affliction.isTended)
                    {
                        tendTarget = creature;
                        break;
                    }
                    else if (affliction is RWDisease disease && disease.timeUntilTreatment <= 0)
                    {
                        tendTarget = creature;
                        break;
                    }
                }

                if (tendTarget == creature)
                {
                    break;
                }
            }

            foreach (RWAffliction affliction in state.wholeBodyAfflictions)
            {
                if (tendTarget == creature)
                {
                    break;
                }

                if (affliction is RWDisease disease && disease.timeUntilTreatment <= 0)
                {
                    tendTarget = creature;
                    break;
                }
            }
        }

        if (tendTarget == null)
        {
            if (state.tendAffliction != null)
            {
                state.tendAffliction = null;
            }

            foreach (var grasp in self.creature.realizedCreature.grasps)
            {
                if (grasp.grabbedChunk != null && grasp.grabbedChunk.owner != null && grasp.grabbedChunk.owner is Creature)
                {
                    (self.creature.realizedCreature as Player).ReleaseGrasp(grasp.graspUsed);
                }
            }
        }
        else
        {
            tendOther(tendTarget);
        }

        void startSelfTend(RWAffliction affliction)
        {
            self.behaviorType = SlugSelfTend;

            if (state.tendAffliction == affliction)
            {
                return;
            }

            state.tendAffliction = affliction;
            state.tendTime = Mathf.Round(state.tendTimeBase / RWHealthState.MedicalTendSpeed(state));
            state.tendTimeMax = state.tendTime;
        }

        void tendOther(Creature creature)
        {
            state.tendTarget = creature;
            self.behaviorType = SlugTend;
        }

        bool grabbedByThis(Creature creature)
        {
            foreach (var grabbed in creature.grabbedBy)
            {
                if (grabbed.grabber == self.cat)
                {
                    return true;
                }
            }

            return false;
        }
    }
    static void SlugNPCAIMove(On.MoreSlugcats.SlugNPCAI.orig_Move orig, MoreSlugcats.SlugNPCAI self)
    {
        orig(self);

        if (!self.creature.controlled || healthTabs.Count < 1 || healthTabs[0] == null || !healthTabs[0].visible || !healthState.TryGetValue(self.cat.State, out _))
        {
            return;
        }

        Player.InputPackage inputPackage = default;

        inputPackage.x = (self.cat.inputWithDiagonals != null) ? self.cat.inputWithDiagonals.Value.x : 0;
        inputPackage.y = (self.cat.inputWithDiagonals != null) ? self.cat.inputWithDiagonals.Value.y : 0;
        inputPackage.jmp = self.cat.inputWithDiagonals != null && self.cat.inputWithDiagonals.Value.jmp;
        inputPackage.mp = self.cat.inputWithDiagonals != null && self.cat.inputWithDiagonals.Value.mp;
        inputPackage.pckp = self.cat.inputWithDiagonals != null && self.cat.inputWithDiagonals.Value.pckp;
        inputPackage.thrw = self.cat.inputWithDiagonals != null && self.cat.inputWithDiagonals.Value.thrw;

        healthTabs[0].input = inputPackage;

        inputPackage.x = 0;
        inputPackage.y = 0;
        inputPackage.jmp = false;
        inputPackage.mp = false;
        inputPackage.pckp = false;
        inputPackage.thrw = false;

        self.cat.input[0] = inputPackage;
    }
    #endregion

    static bool JawCheck(RWState state)
    {
        foreach (RWBodyPart part in state.bodyParts)
        {
            if (part is Jaw jaw)
            {
                return jaw.efficiency > 0;
            }
        }

        return false;
    }

    public static void ArtiLungExplosion(CreatureState self, RWState state)
    {
        foreach (RWBodyPart part in state.bodyParts)
        {
            if (part is Lung && !IsDestroyed(part))
            {
                RWHealthState.Damage(self, state, new RWBomb(), 999999f, 999, part, "Artificer - Explosion", "Artificer");
            }
        }
    }
}