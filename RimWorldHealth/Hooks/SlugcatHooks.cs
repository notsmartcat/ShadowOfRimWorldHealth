using RWCustom;
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
        On.Player.GrabUpdate += PlayerGrabUpdate;
        On.Player.GraphicsModuleUpdated += PlayerGraphicsModuleUpdated;
        On.Player.LungUpdate += PlayerLungUpdate;
        On.Player.PyroDeath += PlayerPyroDeath;
        On.Player.SubtractFood += PlayerSubtractFood;
        On.Player.Update += PlayerUpdate;
        #endregion

        #region PlayerGraphics
        On.PlayerGraphics.DrawSprites += PlayerGraphicsDrawSprites;
        #endregion

        #region SlugNPCAI
        On.MoreSlugcats.SlugNPCAI.DecideBehavior += SlugNPCAIDecideBehavior;
        #endregion
    }

    #region Player
    static void NewPlayer(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
    {
        orig(self, abstractCreature, world);

        if (!healthState.TryGetValue(self.State, out RWState state))
        {
            return;
        }

        state.poleClimbSpeedFac = self.slugcatStats.poleClimbSpeedFac;
        state.corridorClimbSpeedFac = self.slugcatStats.corridorClimbSpeedFac;
        state.runspeedFac = self.slugcatStats.runspeedFac;
        state.swimForceFac = self.slugcatStats.swimForceFac;

        if (state.moving == 1 && state.manipulation == 1)
        {
            return;
        }

        self.slugcatStats.poleClimbSpeedFac = Mathf.Max(0.1f, state.poleClimbSpeedFac * (1 + (state.moving - 1f) * 0.4f) * (1 + (state.manipulation - 1f) * 0.6f));
        self.slugcatStats.corridorClimbSpeedFac = Mathf.Max(0.1f, state.corridorClimbSpeedFac * (1 + (state.moving - 1f) * 0.6f) * (1 + (state.manipulation - 1f) * 0.4f));
        self.slugcatStats.runspeedFac = Mathf.Max(0.1f, state.runspeedFac * state.moving);
        self.slugcatStats.swimForceFac = Mathf.Max(0.1f, state.swimForceFac * (1 + (state.moving - 1f) * 0.4f) * (1 + (state.manipulation - 1f) * 0.6f));
    }

    static void PlayerAddFood(On.Player.orig_AddFood orig, Player self, int add)
    {
        orig(self, add);

        if (!healthState.TryGetValue(self.State, out RWState state))
        {
            return;
        }

        state.hasEaten = true;
    }

    static void PlayerGrabUpdate(On.Player.orig_GrabUpdate orig, Player self, bool eu)
    {
        orig(self, eu);

        if (!healthState.TryGetValue(self.State, out RWState _))
        {
            return;
        }
    }

    static void PlayerGraphicsDrawSprites(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        orig(self, sLeaser, rCam, timeStacker, camPos);

        if (!healthState.TryGetValue(self.player.State, out RWState state))
        {
            return;
        }

        if (!ArmCheck(state) && JawCheck(state) && self.player.grasps[0] != null)
        {
            sLeaser.sprites[5].isVisible = false;
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

        for (int i = 0; i < state.wholeBodyAfflictions.Count; i++)
        {
            if (state.wholeBodyAfflictions[i] is RWAirInLungs tempAirInLungs)
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

    static void PlayerPyroDeath(On.Player.orig_PyroDeath orig, Player self)
    {
        orig(self);

        if (!healthState.TryGetValue(self.State, out RWState state))
        {
            return;
        }

        for (int i = 0; i < state.bodyParts.Count; i++)
        {
            if (state.bodyParts[i] is Lung part && !IsDestroyed(part))
            {
                RWHealthState.Damage(self.State, state, new RWBomb(), 999999f, 999, part, "Artificer - Explosion", "Artificer");
            }
        }
    }

    static void PlayerSubtractFood(On.Player.orig_SubtractFood orig, Player self, int sub)
    {
        orig(self, sub);

        if (!healthState.TryGetValue(self.State, out RWState state) || self.playerState.foodInStomach > 0)
        {
            return;
        }

        state.hasEaten = false;
    }

    static void PlayerUpdate(On.Player.orig_Update orig, Player self, bool eu)
    {
        if (!healthState.TryGetValue(self.State, out RWState state))
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
            if (!JawCheck(state))
            {
                self.grasps[0]?.Release();
            }

            self.grasps[1]?.Release();
        }
        else
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
    }
    #endregion

    #region PlayerGraphics
    static void PlayerGraphicsModuleUpdated(On.Player.orig_GraphicsModuleUpdated orig, Player self, bool actuallyViewed, bool eu)
    {
        orig(self, actuallyViewed, eu);

        if (!healthState.TryGetValue(self.State, out RWState state) || self.grasps[0] == null || ArmCheck(state) || !JawCheck(state))
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
    #endregion

    #region SlugNPCAI
    static void SlugNPCAIDecideBehavior(On.MoreSlugcats.SlugNPCAI.orig_DecideBehavior orig, MoreSlugcats.SlugNPCAI self)
    {
        orig(self);

        if (!healthState.TryGetValue(self.cat.State, out RWState state))
        {
            return;
        }

        if (self.behaviorType == MoreSlugcats.SlugNPCAI.BehaviorType.BeingHeld || self.behaviorType == MoreSlugcats.SlugNPCAI.BehaviorType.OnHead || self.behaviorType == MoreSlugcats.SlugNPCAI.BehaviorType.Thrown && self.cat.bodyMode == Player.BodyModeIndex.Default || self.behaviorType == MoreSlugcats.SlugNPCAI.BehaviorType.Attacking || self.behaviorType == MoreSlugcats.SlugNPCAI.BehaviorType.Fleeing)
        {
            if (state.tendAffliction != null)
            {
                state.tendAffliction = null;
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

        for (int i = 0; i < state.bodyParts.Count; i++)
        {
            for (int j = 0; j < state.bodyParts[i].afflictions.Count; j++)
            {
                if (!state.bodyParts[i].afflictions[j].isTended)
                {
                    if (state.bodyParts[i].afflictions[j] is RWInjury injury)
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
                    else if (state.bodyParts[i].afflictions[j] is RWDisease disease && disease.timeUntilTreatment <= 0)
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
                        Debug.Log("Error affliction " + state.bodyParts[i].afflictions[j] + " does not belong to any tendable check");
                    }
                }
                else if (state.bodyParts[i].afflictions[j] is RWDisease disease && disease.timeUntilTreatment <= 0)
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
            for (int i = 0; i < state.wholeBodyAfflictions.Count; i++)
            {
                if (state.wholeBodyAfflictions[i] is RWDisease disease && disease.timeUntilTreatment <= 0)
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
            for (int i = 0; i < otherState.bodyParts.Count; i++)
            {
                for (int j = 0; j < otherState.bodyParts[i].afflictions.Count; j++)
                {
                    if (!otherState.bodyParts[i].afflictions[j].isTended)
                    {
                        tendOther(self.friendTracker.friend);
                        return;
                    }
                    else if (otherState.bodyParts[i].afflictions[j] is RWDisease disease && disease.timeUntilTreatment <= 0)
                    {
                        tendOther(self.friendTracker.friend);
                        return;
                    }
                }
            }

            for (int i = 0; i < state.wholeBodyAfflictions.Count; i++)
            {
                if (otherState.wholeBodyAfflictions[i] is RWDisease disease && disease.timeUntilTreatment <= 0)
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

        for (int k = 0; k < self.creature.Room.creatures.Count; k++)
        {
            Creature creature = self.creature.Room.creatures[k].realizedCreature;

            if (creature == null || creature.dead || !creature.Stunned || (creature.grabbedBy.Count != 0 && !grabbedByThis(creature)) || creature.State == null || !healthState.TryGetValue(creature.State, out otherState) || (creature is not Player && (creature is not Lizard lizard || lizard.AI == null || lizard.AI.friendTracker.friend == null || lizard.AI.friendTracker.friend is not Player)) || (tendTarget != null && Custom.WorldCoordFloatDist(tendTarget.abstractCreature.pos, self.cat.abstractCreature.pos) > Custom.WorldCoordFloatDist(creature.abstractCreature.pos, self.cat.abstractCreature.pos)))
            {
                continue;
            }

            for (int i = 0; i < otherState.bodyParts.Count; i++)
            {
                for (int j = 0; j < otherState.bodyParts[i].afflictions.Count; j++)
                {
                    if (!otherState.bodyParts[i].afflictions[j].isTended)
                    {
                        tendTarget = creature;
                        break;
                    }
                    else if (otherState.bodyParts[i].afflictions[j] is RWDisease disease && disease.timeUntilTreatment <= 0)
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

            for (int i = 0; i < state.wholeBodyAfflictions.Count; i++)
            {
                if (tendTarget == creature)
                {
                    break;
                }

                if (otherState.wholeBodyAfflictions[i] is RWDisease disease && disease.timeUntilTreatment <= 0)
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
            for (int i = 0; i < creature.grabbedBy.Count; i++)
            {
                if (creature.grabbedBy[i].grabber == self.cat)
                {
                    return true;
                }
            }

            return false;
        }
    }
    #endregion

    static bool JawCheck(RWState state)
    {
        for (int i = 0; i < state.bodyParts.Count; i++)
        {
            if (state.bodyParts[i] is Jaw jaw)
            {
                return jaw.efficiency > 0;
            }
        }

        return false;
    }
}