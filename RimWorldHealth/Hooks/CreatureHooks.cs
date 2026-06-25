using RWCustom;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
        On.SaveState.ctor += NewSaveState;

        On.SaveState.AbstractCreatureToStringStoryWorld_AbstractCreature_WorldCoordinate += SaveStateSaveAbstractCreature;

        On.SaveState.BringUpToDate += SaveStateBringUpToDate;

        On.PlayerState.ctor += NewPlayerState;
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

        #region StaticWorld
        On.StaticWorld.InitStaticWorld += StaticWorldInitStaticWorld;
        #endregion
    }

    static void NewSaveState(On.SaveState.orig_ctor orig, SaveState self, SlugcatStats.Name saveStateNumber, PlayerProgression progression)
    {
        orig(self, saveStateNumber, progression);

        rimWorldHealthHandler.Load(progression.rainWorld.options.saveSlot.ToString(), progression.PlayingAsSlugcat.ToString());
    }

    static void SaveStateBringUpToDate(On.SaveState.orig_BringUpToDate orig, SaveState self, RainWorldGame game)
    {
        orig(self, game);

        rimWorldHealthHandler.Save(game, game.rainWorld.options.saveSlot.ToString(), self.progression.PlayingAsSlugcat.ToString());
    }

    static void NewPlayerState(On.PlayerState.orig_ctor orig, PlayerState self, AbstractCreature crit, int playerNumber, SlugcatStats.Name slugcatCharacter, bool isGhost)
    {
        orig(self, crit, playerNumber, slugcatCharacter, isGhost);

        Debug.Log("playerState load pre for " + crit);

        if (!healthState.TryGetValue(self, out RWState state) || !rimWorldHealthHandler.unrecognizedSaveStrings.ContainsKey(playerNumber.ToString()))
        {
            return;
        }

        Dictionary<string, string> savedData = rimWorldHealthHandler.unrecognizedSaveStrings[playerNumber.ToString()];

        if (savedData.ContainsKey("LastCycle"))
        {
            state.lastCycle = int.Parse(savedData["LastCycle"]);
        }

        for (int bodyPartNumber = 0; bodyPartNumber < state.bodyParts.Count; bodyPartNumber++)
        {
            RWBodyPart part = state.bodyParts[bodyPartNumber];
            string bodyPartName = GetBodyPartKeyName(part);

            Debug.Log("checking for existing afflictions for " + bodyPartName);

            if (savedData.TryGetValue(bodyPartName, out string bodyPartAfflictions))
            {
                string[] allAfflictions = Regex.Split(bodyPartAfflictions, ";");

                Debug.Log("existing affliction found for for " + bodyPartName);

                for (int i = 0; i < allAfflictions.Length; i++)
                {
                    Debug.Log(allAfflictions[i]);

                    string[] afflictionInfo = Regex.Split(allAfflictions[i], ":");

                    Debug.Log(afflictionInfo[0]);

                    part.afflictions.Add(LoadAffliction(afflictionInfo, part, self));
                }
            }
        }

        state.updateCapacities = true;

        Debug.Log(all + crit + " lastCycle = " + state.lastCycle);
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

        if (!healthState.TryGetValue(self, out RWState state))
        {
            return;
        }

        try
        {
            Dictionary<string, string> savedData = self.unrecognizedSaveStrings;

            if (savedData.ContainsKey("ShadowOfLizardUpdatedCycle"))
            {
                state.lastCycle = int.Parse(savedData["ShadowOfLizardUpdatedCycle"]);
                savedData.Remove("ShadowOfLizardUpdatedCycle");
            }

            Debug.Log(all + self.creature + " updatedCycle = " + state.lastCycle);
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }
    }
    #endregion

    static string SaveStateSaveAbstractCreature(On.SaveState.orig_AbstractCreatureToStringStoryWorld_AbstractCreature_WorldCoordinate orig, AbstractCreature self, WorldCoordinate cc)
    {
        if (self == null || self.state == null || self.state.unrecognizedSaveStrings == null || !healthState.TryGetValue(self.state, out RWState state))
        {
            return orig(self, cc);
        }

        try
        {
            Dictionary<string, string> savedData = self.state.unrecognizedSaveStrings;

            savedData["LastCycle"] = self.world.game.GetStorySession.saveState.cycleNumber.ToString();

            //run code to automatically treat all afflictions, then run code to heal afflictions based on the after-cycle-time

            for (int i = 0; i < state.bodyParts.Count; i++)
            {
                if (state.bodyParts[i].afflictions.Count > 0)
                {
                    savedData[GetBodyPartKeyName(state.bodyParts[i])] = GetAllAfflictionValueName(state.bodyParts[i]);
                }
            }

            //Debug.Log("saving data for " + self);
        }
        catch (Exception e) { RimWorldHealth.Logger.LogError(e); }

        return orig(self, cc);
    }

    public static string GetBodyPartKeyName(RWBodyPart part)
    {
        return part.subName + part.name;
    }

    public static string GetAllAfflictionValueName(RWBodyPart part)
    {
        string name = "";
        RWAffliction affliction;
        string nameEnding;

        for (int i = 0; i < part.afflictions.Count; i++)
        {
            if (i > 0)
                name += ";";

            affliction = part.afflictions[i];

            if (affliction is RWInjury injury)
            {
                nameEnding = "";

                if (injury is RWScar scar)
                {
                    name += "Scar:";

                    nameEnding = $"{scar.isRevealed}:{scar.isPermanent}:{scar.scarDamage}:{scar.painCategory}";
                }
                else if (injury is RWDestroyed)
                {
                    name += "Destroyed:";
                }
                else
                {
                    name += "Injury:";
                }

                name += $"{affliction.tendQuality}:{injury.attackName}:{injury.attackerName}:{injury.damage}:{injury.damageType}:{injury.infectionTimer}:{injury.healingDifficulty.name}";

                name += nameEnding;
            }
            else if (affliction is RWDisease disease)
            {
                name = $"{disease.name}:{disease.severity}:{disease.isImmune}:{disease.immunity}:{disease.timeUntilTreatment}:{disease.totalTendQuality}:{disease.InfectionLuck}";
            }
        }

        return name;
    }

    public static RWAffliction LoadAffliction(string[] afflictionInfo, RWBodyPart part, CreatureState state)
    {
        if (afflictionInfo[0] == "Injury" || afflictionInfo[0] == "Scar" || afflictionInfo[0] == "Destroyed")
        {
            float tendQuality = float.Parse(afflictionInfo[1]);
            string attackName = afflictionInfo[2];
            string attackerName = afflictionInfo[3];
            float damage = float.Parse(afflictionInfo[4]);
            RWDamageType damageType = RWDamageType.GetRWDamageType(afflictionInfo[5]);
            float infectionTimer = float.Parse(afflictionInfo[6]);

            RWHealingDifficulty healingDifficulty = RWHealingDifficulty.GetRWHealingDifficulty(afflictionInfo[7]);

            bool isRevealed = false;
            bool isPermanent = false;
            float scarDamage = 0;
            string painCategory = "";

            if (afflictionInfo[0] == "Scar")
            {
                isRevealed = afflictionInfo[8] == "True";
                isPermanent = afflictionInfo[9] == "True";
                scarDamage = float.Parse(afflictionInfo[10]);
                painCategory = afflictionInfo[11];
            }

            RWAffliction affliction = afflictionInfo[0] switch
            {
                "Scar" => new RWScar(state, part, tendQuality, attackName, attackerName, damage, damageType, infectionTimer, healingDifficulty, isRevealed, isPermanent, scarDamage, painCategory),
                "Destroyed" => new RWDestroyed(state, part, tendQuality, attackName, attackerName, damage, damageType, infectionTimer, healingDifficulty),
                _ => new RWInjury(state, part, tendQuality, attackName, attackerName, damage, damageType, infectionTimer, healingDifficulty)
            };

            Debug.Log("created new affliction " + affliction + " for " + part);

            return affliction;
        }
        else
        {
            return null;
        }
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
        if (hitChunk == null || damage <= 0 || self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
            return;
        }

        #region Regular Violence Code
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

        float AP = 0;

        PhysicalObject attacker = source != null && source.owner != null ? source.owner : null;

        if (attacker == null)
        {
            if (type == Creature.DamageType.Electric)
            {
                attackName = "Electricity";

                Override();

                RWHealthState.Damage(self.State, state, new RWElectricalBurn(), damage, AP, GetHitBodyPart(state, hitChunk), attackName, attackerName);
            }
            else if (type == Creature.DamageType.Explosion)
            {
                attackName = "Explosion";

                Override();

                BombDamage(self.State, state, damage * BombDamageMultiplier(self.State is HealthState, false), attackName);
            }
            else if (type == Creature.DamageType.Stab)
            {
                attackName = "Stab";

                Override();

                RWHealthState.Damage(self.State, state, new RWStab(), damage, AP, GetHitBodyPart(state, hitChunk), attackName, attackerName);
            }
            else if (type == Creature.DamageType.Blunt)
            {
                attackName = "Blunt";

                Override();

                BluntDamage(self.State, state, hitChunk, damage, AP, attackName, attackerName);

                return;
            }
            else if (type == Creature.DamageType.Bite)
            {
                attackName = "Bite";

                Override();

                RWHealthState.Damage(self.State, state, new RWBite(), damage, AP, GetHitBodyPart(state, hitChunk), attackName, attackerName);
            }
            else if (type == Creature.DamageType.Water)
            {
                attackName = "Water";

                Override();

                BluntDamage(self.State, state, hitChunk, damage, AP, attackName, attackerName);
            }
            else
            {
                attackName = "None";

                Override();

                BluntDamage(self.State, state, hitChunk, damage, AP, attackName, attackerName);
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

                if (weaponstat.TryGetValue(boomerang.abstractPhysicalObject, out RWWeaponStats weaponState))
                {
                    damage = weaponState.damage;
                }
                else
                {
                    damage = 1;
                }

                BluntDamage(self.State, state, hitChunk, damage, AP, attackName, attackerName);
            }
            else if (attacker is DartMaggot dartMaggot)
            {
                if (dartMaggot.shotBy != null)
                {
                    attackerName = GetCreatureName(dartMaggot.shotBy);
                }
                attackName = "Dart Maggot";

                Override();

                damage = 0.5f;

                RWHealthState.Damage(self.State, state, new RWStab(), damage, AP, GetHitBodyPart(state, hitChunk), attackName, attackerName);
            }
            else if (attacker is JellyFish jellyFish)
            {
                if (jellyFish.thrownBy != null)
                {
                    attackerName = GetCreatureName(jellyFish.thrownBy);
                }
                attackName = "Jellyfish";

                Override();

                damage = 1.5f;

                RWHealthState.Damage(self.State, state, new RWElectricalBurn(), damage, AP, GetHitBodyPart(state, hitChunk), attackName, attackerName);
            }
            else if (attacker is Pomegranate pomegranate)
            {
                if (pomegranate.killTagHolder != null)
                {
                    attackerName = GetCreatureName(pomegranate.killTagHolder);
                }
                attackName = "Pomegranate";

                Override();

                damage = 25f;

                BluntDamage(self.State, state, hitChunk, damage, AP, attackName, attackerName);
            }
            else if (attacker is Rock rock)
            {
                if (rock.thrownBy != null)
                {
                    attackerName = GetCreatureName(rock.thrownBy);
                }
                attackName = "Rock";

                Override();

                if (weaponstat.TryGetValue(rock.abstractPhysicalObject, out RWWeaponStats weaponState))
                {
                    damage = weaponState.damage;
                }
                else
                {
                    damage = 1;
                }

                BluntDamage(self.State, state, hitChunk, damage, AP, attackName, attackerName);
            }
            else if (attacker is ScavengerBomb bomb)
            {
                if (bomb.thrownBy != null)
                {
                    attackerName = GetCreatureName(bomb.thrownBy);
                }
                attackName = "Bomb";

                Override();

                BombDamage(self.State, state, damage * BombDamageMultiplier(self.State is HealthState, false), attackName, attackerName);
            }
            else if (attacker is Spear spear)
            {
                if (spear.thrownBy != null)
                {
                    attackerName = GetCreatureName(spear.thrownBy);
                }

                Debug.Log("Spear");

                if (type != Creature.DamageType.Explosion)
                {
                    if (weaponstat.TryGetValue(spear.abstractPhysicalObject, out RWWeaponStats weaponState))
                    {
                        damage = weaponState.damage;
                        AP = weaponState.AP;
                    }
                    else
                    {
                        damage = 8.3f; //1/3 of the RimWorld pila damage due to the spear not having a long cooldown like the pila does
                    }

                    damage *= spear.spearDamageBonus; //if thrown by the non-exhausted Gourmand the damage will match the RimWorld damage
                }

                if (spear.bugSpear)
                {
                    attackName = "Fire Spear";

                    Override();

                    RWHealthState.Damage(self.State, state, new RWStab(), damage, AP, GetHitBodyPart(state, hitChunk, null, true), attackName, attackerName);

                    RWHealthState.Damage(self.State, state, new RWBurn(), 5, AP, GetHitBodyPart(state, hitChunk), attackName, attackerName);

                    return;
                }
                else if (spear is ExplosiveSpear)
                {
                    attackName = "Explosive Spear";

                    Override();

                    Debug.Log("Explosive Spear");

                    if (type == Creature.DamageType.Explosion)
                    {
                        BombDamage(self.State, state, damage * BombDamageMultiplier(self.State is HealthState, false), attackName, attackerName);
                    }
                    else
                    {
                        RWHealthState.Damage(self.State, state, new RWStab(), damage, AP, GetHitBodyPart(state, hitChunk, null, true), attackName, attackerName);
                    }

                    return;
                }
                else if (ModManager.MSC && spear is MoreSlugcats.ElectricSpear)
                {
                    attackName = "Electric Spear";

                    Override();

                    if (type == Creature.DamageType.Electric)
                    {
                        RWHealthState.Damage(self.State, state, new RWElectricalBurn(), 5, AP, GetHitBodyPart(state, hitChunk), attackName, attackerName);
                    }
                    else
                    {
                        RWHealthState.Damage(self.State, state, new RWStab(), damage, AP, GetHitBodyPart(state, hitChunk, null, true), attackName, attackerName);
                    }

                    return;
                }

                attackName = "Spear";

                Override();

                RWHealthState.Damage(self.State, state, new RWStab(), damage, AP, GetHitBodyPart(state, hitChunk, null, true), attackName, attackerName);
            }
            else if (ModManager.MSC && attacker is MoreSlugcats.Bullet bullet)
            {
                if (bullet.thrownBy != null)
                {
                    attackerName = GetCreatureName(bullet.thrownBy);
                }

                damage = 18;

                AP = 27;

                if (bullet.abstractBullet.bulletType == JokeRifle.AbstractRifle.AmmoType.Rock)
                {
                    damage = 5;
                    attackName = "JokeRifle - Rock";
                }
                else if (bullet.abstractBullet.bulletType == JokeRifle.AbstractRifle.AmmoType.Light)
                {
                    damage = 1;
                    attackName = "JokeRifle - Light";
                }
                else if (bullet.abstractBullet.bulletType == JokeRifle.AbstractRifle.AmmoType.Ash)
                {
                    damage = 0.5f;
                    attackName = "JokeRifle - Ash";
                }
                else if (bullet.abstractBullet.bulletType == JokeRifle.AbstractRifle.AmmoType.Void)
                {
                    attackName = "JokeRifle - Void";
                }
                else if (bullet.abstractBullet.bulletType == JokeRifle.AbstractRifle.AmmoType.Fruit)
                {
                    damage = 2;
                    attackName = "JokeRifle - Fruit";
                }
                else
                {
                    attackName = "Bullet";
                }

                Override();

                RWHealthState.Damage(self.State, state, new RWBullet(), damage, AP, GetHitBodyPart(state, hitChunk, null, true), attackName, attackerName);
            }
            else if (ModManager.MSC && attacker is MoreSlugcats.LillyPuck lillyPuck)
            {
                if (lillyPuck.thrownBy != null)
                {
                    attackerName = GetCreatureName(lillyPuck.thrownBy);
                }
                attackName = "Lilypuck";

                Override();

                if (weaponstat.TryGetValue(lillyPuck.abstractPhysicalObject, out RWWeaponStats weaponState))
                {
                    damage = weaponState.damage;
                }
                else
                {
                    damage = 0.8f;
                }

                BluntDamage(self.State, state, hitChunk, damage, AP, attackName, attackerName);
            }
            else
            {
                attackName = attacker.ToString();

                Override();

                BluntDamage(self.State, state, hitChunk, damage, AP, attackName, attackerName);
            }
        }
        else if (attacker is BigNeedleWorm)
        {
            attackerName = GetCreatureName((Creature)attacker);
            attackName = attackerName + " - Needle";

            Override();

            RWHealthState.Damage(self.State, state, new RWStab(), damage, AP, GetHitBodyPart(state, hitChunk, null, true), attackName, attackerName);
        }
        else if (attacker is BigSpider)
        {
            attackerName = GetCreatureName((Creature)attacker);

            attackName = attackerName + " - Fangs";

            Override();

            RWHealthState.Damage(self.State, state, new RWBite(), damage, AP, GetHitBodyPart(state, hitChunk, null, true), attackName, attackerName);
        }
        else if (attacker is DropBug)
        {
            attackerName = GetCreatureName((Creature)attacker);

            attackName = attackerName + " - Mandibles";

            Override();

            RWHealthState.Damage(self.State, state, new RWBite(), damage, AP, GetHitBodyPart(state, hitChunk, null, true), attackName, attackerName);
        }
        else if (attacker is EggBug)
        {
            attackerName = GetCreatureName((Creature)attacker);

            attackName = attackerName + " - Spine Spikes";

            Override();

            RWHealthState.Damage(self.State, state, new RWStab(), damage, AP, GetHitBodyPart(state, hitChunk, null, true), attackName, attackerName);
        }
        else if (attacker is JetFish)
        {
            attackerName = GetCreatureName((Creature)attacker);

            attackName = attackerName + " - Head";

            Override();

            BluntDamage(self.State, state, hitChunk, damage, AP, attackName, attackerName);
        }
        else if (attacker is Leech)
        {
            attackerName = GetCreatureName((Creature)attacker);

            attackName = attackerName + " - Teeth";

            Override();

            RWHealthState.Damage(self.State, state, new RWBite(), damage, AP, GetHitBodyPart(state, hitChunk), attackName, attackerName);
        }
        else if (attacker is Lizard lizard)
        {
            attackerName = GetCreatureName((Creature)attacker);

            if (type == Creature.DamageType.Explosion && damage == 1.5f || type == Creature.DamageType.Electric && damage == 0.1f)
            {
                attackName = attackerName + " - Blizzard Laser";

                Override();

                RWHealthState.Damage(self.State, state, new RWFrostbite(), damage, AP, GetHitBodyPart(state, hitChunk), attackName, attackerName);
            }
            else
            {
                attackName = attackerName + " - Teeth";

                Override();

                damage = Custom.LerpMap(lizard.lizardParams.maxMusclePower, 0, 16, 4, 22);

                Debug.Log(damage);

                RWHealthState.Damage(self.State, state, new RWBite(), damage, AP, GetHitBodyPart(state, hitChunk, null, true), attackName, attackerName);
            }
        }
        else if (attacker is MirosBird)
        {
            attackerName = GetCreatureName((Creature)attacker);

            attackName = attackerName + " - Beak";

            Override();

            RWHealthState.Damage(self.State, state, new RWBite(), damage, AP, GetHitBodyPart(state, hitChunk, null, true), attackName, attackerName);
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

                BluntDamage(self.State, state, hitChunk, damage, AP, attackName, attackerName);
            }
            else
            {
                attackName = attackerName + " - Teeth";

                Override();

                RWHealthState.Damage(self.State, state, new RWBite(), damage, AP, GetHitBodyPart(state, hitChunk, null, true), attackName, attackerName);
            }

        }
        else if (attacker is SkyWhale)
        {
            attackerName = GetCreatureName((Creature)attacker);

            attackName = attackerName + " - Head";

            Override();

            BluntDamage(self.State, state, hitChunk, damage, AP, attackName, attackerName);
        }
        else if (attacker is Vulture)
        {
            attackerName = GetCreatureName((Creature)attacker);

            attackName = attackerName + " - Beak";

            Override();

            RWHealthState.Damage(self.State, state, new RWBite(), damage, AP, GetHitBodyPart(state, hitChunk, null, true), attackName, attackerName);
        }
        else if (ModManager.MSC && attacker is MoreSlugcats.StowawayBug)
        {
            attackerName = GetCreatureName((Creature)attacker);

            attackName = attackerName + " - Tendril";

            Override();

            RWHealthState.Damage(self.State, state, new RWStab(), damage, AP, GetHitBodyPart(state, hitChunk, null, true), attackName, attackerName);
        }
        else if (ModManager.Watcher && attacker is Watcher.BoxWorm)
        {
            attackerName = GetCreatureName((Creature)attacker);

            attackName = attackerName + " - Steam";

            Override();

            RWHealthState.Damage(self.State, state, new RWBurn(), damage, AP, GetHitBodyPart(state, hitChunk), attackName, attackerName);
        }
        else if (ModManager.Watcher && attacker is Watcher.DrillCrab)
        {
            attackerName = GetCreatureName((Creature)attacker);

            attackName = attackerName + " - Drill";

            Override();

            CutDamage(self.State, state, hitChunk, damage, AP, attackName, attackerName);
        }
        else if (ModManager.Watcher && attacker is Watcher.Frog)
        {
            attackerName = GetCreatureName((Creature)attacker);

            if (type == Creature.DamageType.Bite)
            {
                attackName = attackerName + " - Tendril";

                Override();

                RWHealthState.Damage(self.State, state, new RWStab(), AP, damage, GetHitBodyPart(state, hitChunk), attackName, attackerName);
            }
            else
            {
                attackName = attackerName + " - Body";

                Override();

                BluntDamage(self.State, state, hitChunk, damage, AP, attackName, attackerName);
            }
        }
        else if (ModManager.Watcher && attacker is Watcher.Loach)
        {
            attackerName = GetCreatureName((Creature)attacker);

            attackName = attackerName + " - Body";

            Override();

            BluntDamage(self.State, state, hitChunk, damage, AP, attackName, attackerName);
        }
        else if (ModManager.Watcher && attacker is Watcher.Rat)
        {
            attackerName = GetCreatureName((Creature)attacker);

            attackName = attackerName + " - Body";

            Override();

            BluntDamage(self.State, state, hitChunk, damage, AP, attackName, attackerName);
        }
        else if (ModManager.Watcher && attacker is Watcher.RippleSpider)
        {
            attackerName = GetCreatureName((Creature)attacker);

            attackName = attackerName + " - Body";

            Override();

            BluntDamage(self.State, state, hitChunk, damage, AP, attackName, attackerName);
        }
        else
        {
            attackerName = GetCreatureName((Creature)attacker);

            attackName = attackerName + " - Unknown";

            Override();

            BluntDamage(self.State, state, hitChunk, damage, AP, attackName, attackerName);
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

    #region StaticWorld
    static void StaticWorldInitStaticWorld(On.StaticWorld.orig_InitStaticWorld orig)
    {
        orig();

        for (int i = 0; i < StaticWorld.creatureTemplates.Length; i++)
        {
            if (StaticWorld.creatureTemplates[i].IsLizard)
            {
                (StaticWorld.creatureTemplates[i].breedParameters as LizardBreedParams).biteDamageChance = 1;
            }
        }
    }
    #endregion
}