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
        if (type == Creature.DamageType.Explosion || hitChunk == null || self.State == null || !healthState.TryGetValue(self.State, out RWState state))
        {
            orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
            return;
        }

        if (type == Creature.DamageType.Blunt)
        {
            PhysicalObject tempSource = null;

            if (source != null && source.owner != null)
            {
                tempSource = source.owner;
            }

            BluntDamage(self.State, state, hitChunk, damage, tempSource);

            return;
        }

        RWBodyPart focusedBodyPart = GetHitBodyPart(state, hitChunk, null, false);

        if (focusedBodyPart != null)
        {
            float tempDamage = damage;

            Damage(focusedBodyPart, tempDamage);
        }

        orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);

        void Damage(RWBodyPart focusedBodyPart, float damage)
        {
            string attackName = "";

            if (source != null && source.owner != null)
            {
                attackName = source.owner.ToString();
            }

            RWHealthState.Damage(self.State, state, new(), damage, focusedBodyPart, attackName);
        }
    } //This is a backup hook in case a attack causes Violence, all attacks will be hooked into where Violence is called so the right DamageType can be used, this will mean that Violence is never actually meant to be called
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