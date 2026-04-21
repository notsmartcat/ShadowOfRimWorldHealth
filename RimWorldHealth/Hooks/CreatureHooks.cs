using System;
using UnityEngine;

using static ShadowOfRimWorldHealth.RimWorldHealth;

namespace ShadowOfRimWorldHealth;

internal class CreatureHooks
{
    public static void Apply()
    {
        #region Saving and Loading
        #region CreatureState
        On.CreatureState.ctor += NewCreatureState;
        On.CreatureState.LoadFromString += CreatureStateLoadFromString;
        #endregion

        #region SaveState
        On.SaveState.AbstractCreatureToStringStoryWorld_AbstractCreature_WorldCoordinate += SaveStateSaveAbstractCreature;
        #endregion
        #endregion

        #region AirBreatherCreature
        On.AirBreatherCreature.Update += AirBreatherCreatureUpdate;
        #endregion

        #region Creature
        On.Creature.Violence += CreatureViolence;
        #endregion

        #region InsectoidCreature
        On.InsectoidCreature.Update += InsectoidCreatureUpdate;
        #endregion
    }

    #region Saving and Loading
    #region CreatureState
    static void NewCreatureState(On.CreatureState.orig_ctor orig, CreatureState self, AbstractCreature creature)
    {
        orig(self, creature);

        if (creature.creatureTemplate.type != CreatureTemplate.Type.Slugcat && (!ModManager.MSC || creature.creatureTemplate.type != MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.SlugNPC))
        {
            return;
        }

        if (!healthState.TryGetValue(self, out RWState state))
        {
            healthState.Add(self, new RWState());
            healthState.TryGetValue(self, out state);
        }

        RWHealthState.NewRWHealthState(self, state);
    }
    static void CreatureStateLoadFromString(On.CreatureState.orig_LoadFromString orig, CreatureState self, string[] s)
    {
        orig(self, s);

        if (!healthState.TryGetValue(self, out RWState _))
        {
            return;
        }
    }
    #endregion

    static string SaveStateSaveAbstractCreature(On.SaveState.orig_AbstractCreatureToStringStoryWorld_AbstractCreature_WorldCoordinate orig, AbstractCreature critter, WorldCoordinate pos)
    {
        return orig(critter, pos);
    }
    #endregion

    #region AirBreatherCreature
    static void AirBreatherCreatureUpdate(On.AirBreatherCreature.orig_Update orig, AirBreatherCreature self, bool eu)
    {
        orig(self, eu);

        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state) || (self is Lizard lizard && lizard.Swimmer))
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

        if (self.lungs >= 1)
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
                airInLungs.tendQuality = Mathf.InverseLerp(-0.5f, 1, self.lungs);
            }
            else
            {
                state.wholeBodyAfflictions.Add(new RWAirInLungs(self.State, null, Mathf.InverseLerp(-0.5f, 1, self.lungs)));
            }
        }
    }
    #endregion

    #region Creature
    static void CreatureViolence(On.Creature.orig_Violence orig, Creature self, BodyChunk source, Vector2? directionAndMomentum, BodyChunk hitChunk, PhysicalObject.Appendage.Pos hitAppendage, Creature.DamageType type, float damage, float stunBonus)
    {
        if (hitChunk == null || damage == 0 || self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
            return;
        }

        #region Regulat Violence Code
        if (source != null && source.owner is Creature)
        {
            self.SetKillTag((source.owner as Creature).abstractCreature);
        }
        if (directionAndMomentum != null)
        {
            if (hitChunk != null)
            {
                hitChunk.vel += Vector2.ClampMagnitude(directionAndMomentum.Value / hitChunk.mass, 10f);
            }
            else if (hitAppendage != null && self is PhysicalObject.IHaveAppendages)
            {
                (self as PhysicalObject.IHaveAppendages).ApplyForceOnAppendage(hitAppendage, directionAndMomentum.Value);
            }
        }

        float stunFactor = (damage * 30f + stunBonus) / self.Template.baseStunResistance;

        if (type.Index != -1)
        {
            if (self.Template.damageRestistances[type.Index, 1] > 0f)
            {
                stunFactor /= self.Template.damageRestistances[type.Index, 1];
            }
        }

        self.stunDamageType = type;
        self.Stun((int)stunFactor);
        self.stunDamageType = Creature.DamageType.None;
        #endregion

        string attackName;
        string attackerName = "";

        PhysicalObject attacker = source != null && source.owner != null ? source.owner : null;

        if (attacker == null)
        {
            if (type == Creature.DamageType.Electric)
            {
                attackName = "Electricity";

                Override();

                RWHealthState.Damage(self.State, state, new RWElectricBurn(), damage, GetHitBodyPart(state, hitChunk), attackName, attackerName);
            }
            else if (type == Creature.DamageType.Explosion)
            {
                attackName = "Explosion";

                Override();

                RWHealthState.Damage(self.State, state, new RWBomb(), damage, GetHitBodyPart(state, hitChunk), attackName, attackerName);
            }
            else if (type == Creature.DamageType.Stab)
            {
                attackName = "Explosion";

                Override();

                RWHealthState.Damage(self.State, state, new RWStab(), damage, GetHitBodyPart(state, hitChunk), attackName, attackerName);
            }
            else if (type == Creature.DamageType.Blunt)
            {
                attackName = "Blunt";

                Override();

                BluntDamage(self.State, state, hitChunk, damage, attackName, attackerName);

                return;
            }
        }
        else if (attacker is not Creature)
        {
            if (attacker is Boomerang boomerang)
            {
                if (boomerang.thrownBy != null)
                {
                    attackerName = GetCreatureName(boomerang.thrownBy);
                }
                attackName = "Boomerang";

                Override();

                BluntDamage(self.State, state, hitChunk, damage, attackName, attackerName);
            }
            else if (attacker is DartMaggot dartMaggot)
            {
                if (dartMaggot.shotBy != null)
                {
                    attackerName = GetCreatureName(dartMaggot.shotBy);
                }
                attackName = "Dart Maggot";

                Override();

                RWHealthState.Damage(self.State, state, new RWStab(), damage, GetHitBodyPart(state, hitChunk), attackName, attackerName);
            }
            else if (attacker is ExplosiveSpear explosiveSpear)
            {
                if (explosiveSpear.thrownBy != null)
                {
                    attackerName = GetCreatureName(explosiveSpear.thrownBy);
                }
                attackName = "Explosive Spear";

                Override();

                if (type == Creature.DamageType.Explosion)
                {
                    BombDamage(self.State, state, damage, attackName, attackerName);
                }
                else
                {
                    RWHealthState.Damage(self.State, state, new RWStab(), damage, GetHitBodyPart(state, hitChunk, null, true), attackName, attackerName);
                }
            }
            else if (attacker is JellyFish jellyFish)
            {
                if (jellyFish.thrownBy != null)
                {
                    attackerName = GetCreatureName(jellyFish.thrownBy);
                }
                attackName = "Jellyfish";

                Override();

                RWHealthState.Damage(self.State, state, new RWElectricBurn(), damage, GetHitBodyPart(state, hitChunk), attackName, attackerName);
            }
            else if (attacker is Pomegranate pomegranate)
            {
                if (pomegranate.killTagHolder != null)
                {
                    attackerName = GetCreatureName(pomegranate.killTagHolder);
                }
                attackName = "Pomegranate";

                Override();

                BluntDamage(self.State, state, hitChunk, damage, attackName, attackerName);
            }
            else if (attacker is Rock rock)
            {
                if (rock.thrownBy != null)
                {
                    attackerName = GetCreatureName(rock.thrownBy);
                }
                attackName = "Rock";

                Override();

                BluntDamage(self.State, state, hitChunk, damage, attackName, attackerName);
            }
            else if (attacker is ScavengerBomb bomb)
            {
                if (bomb.thrownBy != null)
                {
                    attackerName = GetCreatureName(bomb.thrownBy);
                }
                attackName = "Bomb";

                Override();

                BombDamage(self.State, state, damage, attackName, attackerName);
            }
            else if (attacker is Spear spear)
            {
                if (spear.thrownBy != null)
                {
                    attackerName = GetCreatureName(spear.thrownBy);
                }
                attackName = "Spear";

                Override();

                RWHealthState.Damage(self.State, state, new RWStab(), damage, GetHitBodyPart(state, hitChunk, null, true), attackName, attackerName);
            }
            else if (ModManager.MSC && attacker is MoreSlugcats.Bullet bullet)
            {
                if (bullet.thrownBy != null)
                {
                    attackerName = GetCreatureName(bullet.thrownBy);
                }

                if (bullet.abstractBullet.bulletType == JokeRifle.AbstractRifle.AmmoType.Rock)
                {
                    attackName = "JokeRifle - Rock";
                }
                else if (bullet.abstractBullet.bulletType == JokeRifle.AbstractRifle.AmmoType.Grenade)
                {
                    attackName = "JokeRifle - Granade";
                }
                else if (bullet.abstractBullet.bulletType == JokeRifle.AbstractRifle.AmmoType.Firecracker)
                {
                    attackName = "JokeRifle - Firecracker";
                }
                else if (bullet.abstractBullet.bulletType == JokeRifle.AbstractRifle.AmmoType.Pearl)
                {
                    attackName = "JokeRifle - Pearl";
                }
                else if (bullet.abstractBullet.bulletType == JokeRifle.AbstractRifle.AmmoType.Light)
                {
                    attackName = "JokeRifle - Light";
                }
                else if (bullet.abstractBullet.bulletType == JokeRifle.AbstractRifle.AmmoType.Ash)
                {
                    attackName = "JokeRifle - Ash";
                }
                else if (bullet.abstractBullet.bulletType == JokeRifle.AbstractRifle.AmmoType.Bees)
                {
                    attackName = "JokeRifle - Bees";
                }
                else if (bullet.abstractBullet.bulletType == JokeRifle.AbstractRifle.AmmoType.Void)
                {
                    attackName = "JokeRifle - Void";
                }
                else if (bullet.abstractBullet.bulletType == JokeRifle.AbstractRifle.AmmoType.Fruit)
                {
                    attackName = "JokeRifle - Fruit";
                }
                else if (bullet.abstractBullet.bulletType == JokeRifle.AbstractRifle.AmmoType.Noodle)
                {
                    attackName = "JokeRifle - Noodle";
                }
                else if (bullet.abstractBullet.bulletType == JokeRifle.AbstractRifle.AmmoType.FireEgg)
                {
                    attackName = "JokeRifle - Fire Egg";
                }
                else if (bullet.abstractBullet.bulletType == JokeRifle.AbstractRifle.AmmoType.Singularity)
                {
                    attackName = "JokeRifle - Singularity";
                }
                else
                {
                    attackName = "Bullet";
                }

                Override();

                RWHealthState.Damage(self.State, state, new RWBullet(), damage, GetHitBodyPart(state, hitChunk, null, true), attackName, attackerName);
            }
            else if (ModManager.MSC && attacker is MoreSlugcats.ElectricSpear electricSpear)
            {
                if (electricSpear.thrownBy != null)
                {
                    attackerName = GetCreatureName(electricSpear.thrownBy);
                }
                attackName = "Electric Spear";

                Override();

                RWHealthState.Damage(self.State, state, new RWElectricBurn(), damage, GetHitBodyPart(state, hitChunk, null, true), attackName, attackerName);
            }
            else if (ModManager.MSC && attacker is MoreSlugcats.LillyPuck lillyPuck)
            {
                if (lillyPuck.thrownBy != null)
                {
                    attackerName = GetCreatureName(lillyPuck.thrownBy);
                }
                attackName = "Electric Spear";

                Override();

                BluntDamage(self.State, state, hitChunk, damage, attackName, attackerName);
            }
        }
        else if (attacker is BigNeedleWorm)
        {
            attackerName = GetCreatureName((Creature)attacker);
            attackName = attackerName + " - Needle";

            Override();

            RWHealthState.Damage(self.State, state, new RWStab(), damage, GetHitBodyPart(state, hitChunk, null, true), attackName, attackerName);
        }
        else if (attacker is BigSpider)
        {
            attackerName = GetCreatureName((Creature)attacker);

            attackName = attackerName + " - Fangs";

            Override();

            RWHealthState.Damage(self.State, state, new RWBite(), damage, GetHitBodyPart(state, hitChunk, null, true), attackName, attackerName);
        }
        else if (attacker is DropBug)
        {
            attackerName = GetCreatureName((Creature)attacker);

            attackName = attackerName + " - Mandibles";

            Override();

            RWHealthState.Damage(self.State, state, new RWBite(), damage, GetHitBodyPart(state, hitChunk, null, true), attackName, attackerName);
        }
        else if (attacker is EggBug)
        {
            attackerName = GetCreatureName((Creature)attacker);

            attackName = attackerName + " - Spine Spikes";

            Override();

            RWHealthState.Damage(self.State, state, new RWStab(), damage, GetHitBodyPart(state, hitChunk, null, true), attackName, attackerName);
        }
        else if (attacker is JetFish)
        {
            attackerName = GetCreatureName((Creature)attacker);

            attackName = attackerName + " - Head";

            Override();

            BluntDamage(self.State, state, hitChunk, damage, attackName, attackerName);
        }
        else if (attacker is Leech)
        {
            attackerName = GetCreatureName((Creature)attacker);

            attackName = attackerName + " - Teeth";

            Override();

            RWHealthState.Damage(self.State, state, new RWBite(), damage, GetHitBodyPart(state, hitChunk), attackName, attackerName);
        }
        else if (attacker is Lizard)
        {
            attackerName = GetCreatureName((Creature)attacker);

            if (type == Creature.DamageType.Explosion && damage == 1.5f || type == Creature.DamageType.Electric && damage == 0.1f)
            {
                attackName = attackerName + " - Blizzard Laser";

                Override();

                RWHealthState.Damage(self.State, state, new RWFrostbite(), damage, GetHitBodyPart(state, hitChunk), attackName, attackerName);
            }
            else
            {
                attackName = attackerName + " - Teeth";

                Override();

                RWHealthState.Damage(self.State, state, new RWBite(), damage, GetHitBodyPart(state, hitChunk, null, true), attackName, attackerName);
            }
        }
        else if (attacker is MirosBird)
        {
            attackerName = GetCreatureName((Creature)attacker);

            attackName = attackerName + " - Beak";

            Override();

            RWHealthState.Damage(self.State, state, new RWBite(), damage, GetHitBodyPart(state, hitChunk, null, true), attackName, attackerName);
        }
        else if (attacker is Player)
        {
            attackerName = GetCreatureName((Creature)attacker);

            if (type == Creature.DamageType.Blunt)
            {
                if (damage == 1)
                {
                    attackName = attackerName + " - Roll";
                }
                else
                {
                    attackName = attackerName + " - Slam";
                }

                Override();

                BluntDamage(self.State, state, hitChunk, damage, attackName, attackerName);
            }
            else
            {
                attackName = attackerName + " - Teeth";

                Override();

                RWHealthState.Damage(self.State, state, new RWBite(), damage, GetHitBodyPart(state, hitChunk, null, true), attackName, attackerName);
            }

        }
        else if (attacker is SkyWhale)
        {
            attackerName = GetCreatureName((Creature)attacker);

            attackName = attackerName + " - Head";

            Override();

            BluntDamage(self.State, state, hitChunk, damage, attackName, attackerName);
        }
        else if (attacker is Vulture)
        {
            attackerName = GetCreatureName((Creature)attacker);

            attackName = attackerName + " - Beak";

            Override();

            RWHealthState.Damage(self.State, state, new RWBite(), damage, GetHitBodyPart(state, hitChunk, null, true), attackName, attackerName);
        }
        else if (ModManager.MSC && attacker is MoreSlugcats.StowawayBug)
        {
            attackerName = GetCreatureName((Creature)attacker);

            attackName = attackerName + " - Tendril";

            Override();

            RWHealthState.Damage(self.State, state, new RWStab(), damage, GetHitBodyPart(state, hitChunk, null, true), attackName, attackerName);
        }
        else if (ModManager.Watcher && attacker is Watcher.BoxWorm)
        {
            attackerName = GetCreatureName((Creature)attacker);

            attackName = attackerName + " - Steam";

            Override();

            RWHealthState.Damage(self.State, state, new RWBurn(), damage, GetHitBodyPart(state, hitChunk), attackName, attackerName);
        }
        else if (ModManager.Watcher && attacker is Watcher.DrillCrab)
        {
            attackerName = GetCreatureName((Creature)attacker);

            attackName = attackerName + " - Drill";

            Override();

            CutDamage(self.State, state, hitChunk, damage, attackName, attackerName);
        }
        else if (ModManager.Watcher && attacker is Watcher.Frog)
        {
            attackerName = GetCreatureName((Creature)attacker);

            if (type == Creature.DamageType.Bite)
            {
                attackName = attackerName + " - Tendril";

                Override();

                RWHealthState.Damage(self.State, state, new RWStab(), damage, GetHitBodyPart(state, hitChunk), attackName, attackerName);
            }
            else
            {
                attackName = attackerName + " - Body";

                Override();

                BluntDamage(self.State, state, hitChunk, damage, attackName, attackerName);
            }
        }
        else if (ModManager.Watcher && attacker is Watcher.Loach)
        {
            attackerName = GetCreatureName((Creature)attacker);

            attackName = attackerName + " - Body";

            Override();

            BluntDamage(self.State, state, hitChunk, damage, attackName, attackerName);
        }
        else if (ModManager.Watcher && attacker is Watcher.Rat)
        {
            attackerName = GetCreatureName((Creature)attacker);

            attackName = attackerName + " - Body";

            Override();

            BluntDamage(self.State, state, hitChunk, damage, attackName, attackerName);
        }
        else if (ModManager.Watcher && attacker is Watcher.RippleSpider)
        {
            attackerName = GetCreatureName((Creature)attacker);

            attackName = attackerName + " - Body";

            Override();

            BluntDamage(self.State, state, hitChunk, damage, attackName, attackerName);
        }

        RWBodyPart focusedBodyPart = GetHitBodyPart(state, hitChunk, null, false);

        if (focusedBodyPart != null)
        {
            float tempDamage = damage;

            Damage(focusedBodyPart, tempDamage);
        }

        //orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);

        void Damage(RWBodyPart focusedBodyPart, float damage)
        {
            string attackName = "";

            if (source != null && source.owner != null)
            {
                attackName = source.owner.ToString();
            }

            RWHealthState.Damage(self.State, state, new(), damage, focusedBodyPart, attackName);
        }

        void Override()
        {
            if (state.violenceAttackOverride != "")
            {
                attackName = state.violenceAttackOverride;
                state.violenceAttackOverride = "";
            }
            if (state.violenceAttackerOverride != "")
            {
                attackerName = state.violenceAttackerOverride;
                state.violenceAttackerOverride = "";
            }
        }
    }
    #endregion

    #region InsectoidCreature
    static void InsectoidCreatureUpdate(On.InsectoidCreature.orig_Update orig, InsectoidCreature self, bool eu)
    {
        orig(self, eu);

        if (self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            return;
        }

        SetToxicBuildup(state, self);
    }
    #endregion
}