using HUD;
using System.Collections.Generic;
using UnityEngine;
using RWCustom;

using static ShadowOfRimWorldHealth.RimWorldHealth;

namespace ShadowOfRimWorldHealth;

public class HealthTab : HudPart
{
    public HealthTab(HUD.HUD hud, AbstractCreature owner) : base(hud)
    {
        this.owner = owner;

        sprites = new FSprite[2];

        capacityName = new(Custom.GetFont(), "Name")
        {
            alignment = FLabelAlignment.Left,
            anchorX = 0f,
            anchorY = 1f
        };

        bloodLossPerCycle = new(Custom.GetFont(), "blood Loss")
        {
            alignment = FLabelAlignment.Left,
            anchorX = 0f,
            anchorY = 1f
        };

        for (int i = 0; i < 12; i++)
        {
            capacityValueNames.Add(new FLabel(Custom.GetFont(), "capacityName"));
            capacityValueNames[i].alignment = FLabelAlignment.Left;
            capacityValueNames[i].anchorX = 0f;
            capacityValueNames[i].anchorY = 1f;

            capacityValues.Add(new FLabel(Custom.GetFont(), "StatValue"));
            capacityValues[i].alignment = FLabelAlignment.Left;
            capacityValues[i].anchorX = 1f;
            capacityValues[i].anchorY = 1f;
        }

        selectedSprite = new FSprite("pixel", true);
        treatedSprite = new FSprite("pixel", true);

        for (int k = 0; k < sprites.Length; k++)
        {
            sprites[k] = new FSprite("pixel", true);
            hud.fContainers[1].AddChild(sprites[k]);
        }

        hud.fContainers[1].AddChild(capacityName);
        hud.fContainers[1].AddChild(bloodLossPerCycle);

        for (int k = 0; k < capacityValueNames.Count; k++)
        {
            hud.fContainers[1].AddChild(capacityValueNames[k]);
            hud.fContainers[1].AddChild(capacityValues[k]);
        }

        hud.fContainers[1].AddChild(selectedSprite);
        hud.fContainers[1].AddChild(treatedSprite);

        healthTabWholeBody = new HealthTabWholeBody(this);
    }

    public Vector2 DrawPos()
    {
        return new Vector2(hud.rainWorld.screenSize.x / 2, hud.rainWorld.screenSize.y / 2) + new Vector2(0.1f, 0.1f);
    }

    public override void Update()
    {
        base.Update();

        if (owner == null || owner.realizedCreature == null || !owner.realizedCreature.dead && owner.realizedCreature.Stunned)
        {
            visible = false;
        }

        healthTabWholeBody.Update();

        for (int i = healthTabBodyParts.Count - 1; i >= 0; i--)
        {
            if (healthTabBodyParts[i].slatedForDeletion || state == null || !visible)
            {
                healthTabBodyParts[i].ClearSprites();
                healthTabBodyParts.RemoveAt(i);
            }
            else
            {
                healthTabBodyParts[i].Update();
            }
        }
        for (int i = healthTabInfos.Count - 1; i >= 0; i--)
        {
            if (healthTabInfos[i].slatedForDeletion || state == null || !visible)
            {
                healthTabInfos[i].ClearSprites();
                healthTabInfos.RemoveAt(i);
            }
            else
            {
                healthTabInfos[i].Update();
            }
        }

        if (state == null || !visible)
        {
            return;
        }

        for (int i = 0; i < state.bodyParts.Count; i++)
        {
            if (state.bodyParts[i].afflictions.Count > 0)
            {
                bool alreadyExists = false;
                for (int j = 0; j < healthTabBodyParts.Count; j++)
                {
                    if (healthTabBodyParts[j].bodyPart == state.bodyParts[i])
                    {
                        alreadyExists = true;

                        if (IsSubPartDestroyed(state, healthTabBodyParts[j].bodyPart))
                        {
                            healthTabBodyParts[j].slatedForDeletion = true;
                        }
                        break;
                    }
                }

                if (!alreadyExists && !IsSubPartDestroyed(state, state.bodyParts[i]))
                {
                    HealthTabBodyPart part = new( this, state.bodyParts[i]);

                    healthTabBodyParts.Add(part);

                    part.Update();
                }
            }
        }

        if (input.x != 0)
        {
            if (!horizontalOnce && healthTabBodyParts.Count + healthTabWholeBody.afflictionNumber > 0)
            {
                horizontalOnce = true;

                selectedTimer = selectedTimerMax;

                selectedHorizontal += input.x > 0 ? -1 : 1;

                if (selectedHorizontal >= 2)
                {
                    selectedHorizontal = 0;
                }
                else if (selectedHorizontal <= -1)
                {
                    selectedHorizontal = 1;
                }

                if (selectedVertical != -1)
                {
                    int maxVertical = selectedHorizontal == 0 ? 12 : -1;

                    if (maxVertical == -1)
                    {
                        maxVertical = healthTabWholeBody.active ? healthTabWholeBody.afflictionNumber : 0;

                        for (int i = 0; i < healthTabBodyParts.Count; i++)
                        {
                            maxVertical += healthTabBodyParts[i].combinedAfflictions.Count + healthTabBodyParts[i].afflictions.Count;
                        }
                    }

                    if (selectedVertical >= maxVertical)
                    {
                        selectedVertical = maxVertical - 1;
                    }
                    else if (selectedVertical <= -1)
                    {
                        selectedVertical = 0;
                    }
                }
            }
        }
        else
        {
            horizontalOnce = false;
        }

        if (healthTabBodyParts.Count + healthTabWholeBody.afflictionNumber <= 0)
        {
            selectedHorizontal = 0;
        }

        if (input.y != 0)
        {
            if (!verticalOnce)
            {
                selected = true;

                selectedTimer = selectedTimerMax;

                verticalOnce = true;

                selectedVertical += input.y > 0 ? -1 : 1;

                int maxVertical = selectedHorizontal == 0 ? 12 : -1;

                if (maxVertical == -1)
                {
                    maxVertical = healthTabWholeBody.active ? healthTabWholeBody.afflictionNumber : 0;
                    for (int i = 0; i < healthTabBodyParts.Count; i++)
                    {
                        maxVertical += healthTabBodyParts[i].combinedAfflictions.Count + healthTabBodyParts[i].afflictions.Count;
                    }
                }

                if (selectedVertical >= maxVertical)
                {
                    selectedVertical = 0;
                }
                else if (selectedVertical <= -1)
                {
                    selectedVertical = maxVertical - 1;
                }
            }
        }
        else
        {
            verticalOnce = false;
        }

        if (selectedHorizontal == 1)
        {
            int maxVertical = healthTabWholeBody.active ? healthTabWholeBody.afflictionNumber : 0;

            for (int i = 0; i < healthTabBodyParts.Count; i++)
            {
                maxVertical += healthTabBodyParts[i].combinedAfflictions.Count + healthTabBodyParts[i].afflictions.Count;
            }

            if (selectedVertical >= maxVertical)
            {
                selectedVertical = maxVertical - 1;
            }
        }

        if (treating)
        {
            treatTime--;

            if (treatedAffliction == null || treatedAffliction.isTended)
            {
                treating = false;
                treatedAffliction = null;
            }
            else if (treatTime <= 0)
            {
                treatedAffliction.isTended = true;
                treating = false;
                treatedAffliction.tendQuality = Random.value;

                if (treatedAffliction is RWInjury injury)
                {
                    injury.isBleeding = false;
                }
                else if (treatedAffliction is RWDisease disease)
                {
                    disease.timeUntilTreatment = state.cycleLength * disease.treatmentTimes;
                    disease.totalTendQuality += disease.tendQuality;
                }

                treatedAffliction = null;
            }

            return;
        }

        if (input.pckp)
        {
            RWDestroyed destroyedAffliction = null;

            RWInjury bleeding = null;
            RWDisease diseaseAffliction = null;
            RWInjury untendedAffliction = null;

            for (int i = 0; i < state.bodyParts.Count; i++)
            {
                for (int j = 0; j < state.bodyParts[i].afflictions.Count; j++)
                {
                    if (!state.bodyParts[i].afflictions[j].isTended)
                    {
                        if (state.bodyParts[i].afflictions[j] is RWDestroyed destroyed && !IsSubPartDestroyed(state, state.bodyParts[i]) && destroyed.isBleeding)
                        {
                            destroyedAffliction = destroyed;
                            break;
                        }
                        else if (state.bodyParts[i].afflictions[j] is RWInjury injury)
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

            if (destroyedAffliction == null && bleeding == null)
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

            if (destroyedAffliction != null)
            {
                startTreating(destroyedAffliction);
            }
            else if (bleeding != null)
            {
                startTreating(bleeding);
            }
            else if (diseaseAffliction != null)
            {
                startTreating(diseaseAffliction);
            }
            else if (untendedAffliction != null)
            {
                startTreating(untendedAffliction);
            }
        }

        void startTreating(RWAffliction affliction)
        {
            treatedAffliction = affliction;
            treating = true;
            treatTime = Mathf.Floor(treatTimeBase / Mathf.Max(state.manipulation, 0.1f));
            treatTimeMax = treatTime;
        }
    }

    public void ToggleVisibility(RWPlayerHealthState state)
    {
        visible = !visible;

        if (visible)
        {
            this.state = state;

            TriggeredOn();
        }
        else
        {
            this.state = null;

            TriggeredOff();
        }
    }

    public override void Draw(float timeStacker)
    {
        base.Draw(timeStacker);

        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].isVisible = visible;
        }

        for (int i = 0; i < capacityValueNames.Count; i++)
        {
            capacityValueNames[i].isVisible = visible;
            capacityValues[i].isVisible = visible;
        }

        capacityName.isVisible = visible;

        bloodLossPerCycle.isVisible = visible && state.bloodLossPerCycle >= 1 && state.bloodLoss < 1;

        selectedSprite.isVisible = visible && selected;

        treatedSprite.isVisible = visible && treating;

        healthTabWholeBody.Draw();

        for (int i = healthTabBodyParts.Count - 1; i >= 0; i--)
        {
            healthTabBodyParts[i].Draw();
        }
        for (int i = healthTabInfos.Count - 1; i >= 0; i--)
        {
            healthTabInfos[i].Draw();
        }

        if (!visible || state == null)
        {
            return;
        }

        if (!state.dead)
        {
            capacityValueNamesNames = new(12) { "Pain", "Consiousness", "Moving", "Manipulation", "Talking", "Eating", "Sight", "Hearing", "Breathing", "Blood filtrarion", "Blood pumping", "Digestion" };
        }
        else
        {
            capacityValueNamesNames = new(1) { "Pain" };
        }

        sprites[0].x = DrawPos().x;
        sprites[0].y = DrawPos().y;
        sprites[0].scaleX = 350;
        sprites[0].scaleY = 250;
        sprites[0].color = Color.gray;

        sprites[1].x = DrawPos().x - 250;
        sprites[1].y = DrawPos().y;
        sprites[1].scaleX = 150;
        sprites[1].scaleY = 250;
        sprites[1].color = Color.black;

        capacityName.color = Color.white;
        capacityName.x = DrawPos().x - 320;
        capacityName.y = DrawPos().y + 125;
        capacityName.text = state.creature.ToString();

        if (bloodLossPerCycle.isVisible)
        {
            float bloodLoss = state.cycleLength * (1 - state.bloodLoss) / (state.bloodLossPerCycle / 100);

            string bloodLossTime = bloodLoss < 0 ? Mathf.Floor(bloodLoss * 10000) / 100 + (Mathf.Floor(bloodLoss * 10000) / 100 == 1 ? " second" : " seconds") : bloodLoss > 60 ? Mathf.Floor(bloodLoss / 60 * 10) / 10 + (Mathf.Floor(bloodLoss / 60 * 10) / 10 == 1 ? " hour" : " hours") : Mathf.Floor(bloodLoss * 10) / 10 + (Mathf.Floor(bloodLoss * 10) / 10 == 1 ? " minute" : " minutes");

            bloodLossPerCycle.color = Color.white;
            bloodLossPerCycle.x = DrawPos().x - 165;
            bloodLossPerCycle.y = DrawPos().y - 105;
            bloodLossPerCycle.text = "Bleeding: " + Mathf.Floor(state.bloodLossPerCycle) + "%/c (death in " + bloodLossTime + ")";
        }

        for (int i = 0; i < capacityValueNames.Count; i++)
        {
            if (i >= capacityValueNamesNames.Count)
            {
                capacityValueNames[i].isVisible = false;
                capacityValues[i].isVisible = false;
                continue;
            }

            capacityValueNames[i].color = Color.white;
            capacityValueNames[i].x = DrawPos().x - 320;
            capacityValueNames[i].y = DrawPos().y + 100 - (17 * i);

            capacityValueNames[i].text = capacityValueNamesNames[i];

            capacityValues[i].x = DrawPos().x - 180;
            capacityValues[i].y = DrawPos().y + 100 - (17 * i);

            float value = 0;

            switch (capacityValueNamesNames[i])
            {
                case "Pain":
                    value = state.pain;
                    break;
                case "Consiousness":
                    value = state.consciousness;
                    break;
                case "Moving":
                    value = state.moving;
                    break;
                case "Manipulation":
                    value = state.manipulation;
                    break;
                case "Talking":
                    value = state.talking;
                    break;
                case "Eating":
                    value = state.eating;
                    break;
                case "Sight":
                    value = state.sight;
                    break;
                case "Hearing":
                    value = state.hearing;
                    break;
                case "Breathing":
                    value = state.breathing;
                    break;
                case "Blood filtrarion":
                    value = state.bloodFiltration;
                    break;
                case "Blood pumping":
                    value = state.bloodPumping;
                    break;
                case "Digestion":
                    value = state.digestion;
                    break;
            }

            if (capacityValueNamesNames[i] == "Pain")
            {
                if (value >= 0.8f)
                {
                    capacityValues[i].text = "Mind-shattering";
                    capacityValues[i].color = Color.red;
                }
                else if (value >= 0.4f)
                {
                    capacityValues[i].text = "Intense";
                    capacityValues[i].color = Color.Lerp(Color.white, Color.black, 0.6f);
                }
                else if (value >= 0.15f)
                {
                    capacityValues[i].text = "Serious";
                    capacityValues[i].color = Color.Lerp(Color.white, Color.black, 0.4f);
                }
                else if (value >= 0.01f)
                {
                    capacityValues[i].text = "Minor";
                    capacityValues[i].color = Color.Lerp(Color.white, Color.black, 0.1f);
                }
                else
                {
                    capacityValues[i].text = "None";
                    capacityValues[i].color = Color.green;
                }
            }
            else
            {
                capacityValues[i].text = Mathf.Floor(value * 100) + "%";

                if (value > 1)
                {
                    capacityValues[i].color = Color.blue;
                }
                else if(value == 1)
                {
                    capacityValues[i].color = Color.green;
                }
                else if (value >= 0.8f)
                {
                    capacityValues[i].color = Color.Lerp(Color.white, Color.black, 0.1f);
                }
                else if (value >= 0.4f)
                {
                    capacityValues[i].color = Color.Lerp(Color.white, Color.black, 0.4f);
                }
                else if (value >= 0.15f)
                {
                    capacityValues[i].color = Color.Lerp(Color.white, Color.black, 0.6f);
                }
                else if (value >= 0.01f)
                {
                    capacityValues[i].color = Color.Lerp(Color.white, Color.black, 0.8f);
                }
                else
                {
                    capacityValues[i].color = Color.red;
                }
            }
        }

        if (treatedSprite.isVisible && treatedAffliction != null)
        {
            treatedSprite.x = Mathf.Lerp(DrawPos().x, DrawPos().x + 60, 1 - treatTime / treatTimeMax);
            treatedSprite.alpha = 0.6f;
            treatedSprite.color = Color.magenta;

            treatedSprite.scaleY = 20;
            treatedSprite.scaleX = Mathf.Lerp(0, 220, 1 - treatTime / treatTimeMax);

            int selectedBodyPart = 0;
            int selectedAffliction = 0;

            if (state.wholeBodyAfflictions.Count > 0 && state.wholeBodyAfflictions.Contains(treatedAffliction))
            {
                for (int j = 0; j < state.wholeBodyAfflictions.Count - (healthTabWholeBody.bloodLossVisible ? 1 : 0); j++)
                {
                    if (state.wholeBodyAfflictions[j] == treatedAffliction)
                    {
                        selectedAffliction = j;
                        break;
                    }
                }

                int extraHeight = 0;
                int prevHeight = 0;

                if (healthTabWholeBody.afflictionsHeight.Count >= selectedAffliction)
                {
                    extraHeight = healthTabWholeBody.afflictionsHeight[selectedAffliction][0] - 1;
                    prevHeight = healthTabWholeBody.afflictionsHeight[selectedAffliction][1];
                }

                treatedSprite.y = healthTabWholeBody.DrawPos().y - ((healthTabWholeBody.bloodLossVisible ? 17.5f : 0) + -1 + (17.5f * selectedAffliction) + (15 * prevHeight) + (7.5f * (extraHeight + 1)));
                treatedSprite.MoveInFrontOfOtherNode(healthTabWholeBody.background);
                treatedSprite.scaleY = 2 + 17.5f + (15 * extraHeight);
            }
            else
            {
                for (int i = 0; i < healthTabBodyParts.Count; i++)
                {
                    if (healthTabBodyParts[i].bodyPart == treatedAffliction.part)
                    {
                        selectedBodyPart = i;

                        if (healthTabBodyParts[i].allAfflictions.Contains(treatedAffliction))
                        {
                            selectedAffliction = healthTabBodyParts[i].allAfflictions.IndexOf(treatedAffliction);
                            break;
                        }

                        for (int j = 0; j < healthTabBodyParts[i].combinedAfflictions.Count; j++)
                        {
                            if (healthTabBodyParts[i].combinedAfflictions.TryGetValue(healthTabBodyParts[i].CombinedAfflictionName(healthTabBodyParts[i].bodyPart, j), out List<RWAffliction> list) && list.Contains(treatedAffliction))
                            {
                                for (int k = 0; k < list.Count; k++)
                                {
                                    if (healthTabBodyParts[i].allAfflictions.Contains(list[k]))
                                    {
                                        selectedAffliction = healthTabBodyParts[i].allAfflictions.IndexOf(list[k]);
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                        break;
                    }
                }

                int extraHeight = healthTabBodyParts[selectedBodyPart].allAfflictionsHeight[selectedAffliction][0] - 1;
                int prevHeight = healthTabBodyParts[selectedBodyPart].allAfflictionsHeight[selectedAffliction][1];

                treatedSprite.y = healthTabBodyParts[selectedBodyPart].DrawPos().y - (-1 + (17.5f * selectedAffliction) + (15 * prevHeight) + (7.5f * (extraHeight + 1)));

                treatedSprite.MoveInFrontOfOtherNode(healthTabBodyParts[selectedBodyPart].background);

                treatedSprite.scaleY = 2 + 17.5f + (15 * extraHeight);
            }
        }

        if (!selected)
        {
            return;
        }

        if (selectedTimer > 0)
        {
            selectedTimer -= timeStacker;
        }

        selectedSprite.x = DrawPos().x;
        selectedSprite.y = DrawPos().y;
        selectedSprite.alpha = 0.4f;
        selectedSprite.color = Color.white;

        selectedSprite.scaleY = 15;

        if (selectedHorizontal == 0)
        {
            selectedSprite.x -= 250;

            selectedSprite.y += 100 -7 - (17 * selectedVertical);

            selectedSprite.scaleX = 145;

            if (selectedTimer <= 0)
            {
                CapacitySelector();
            }
        }
        else if (selectedHorizontal == 1)
        {
            selectedSprite.x += 60;

            selectedSprite.y += 120;
            selectedSprite.y -= 11.6f;

            selectedSprite.scaleX = 220;
            selectedSprite.scaleY = 20;

            if (healthTabWholeBody.active && selectedVertical < healthTabWholeBody.afflictionNumber)
            {
                if (healthTabWholeBody.bloodLossVisible && selectedVertical == 0)
                {
                    selectedSprite.y += 5;

                    selectedSprite.scaleY = 2 + 17.5f;
                }
                else
                {
                    int extraHeight = healthTabWholeBody.afflictionsHeight[selectedVertical][0] - 1;
                    int prevHeight = healthTabWholeBody.afflictionsHeight[selectedVertical][1];

                    selectedSprite.y = healthTabWholeBody.DrawPos().y - (-1 + (17.5f * selectedVertical) + (15 * prevHeight) + (7.5f * (extraHeight + 1)));

                    selectedSprite.scaleY = 2 + 17.5f + (15 * extraHeight);
                }

                selectedSprite.MoveInFrontOfOtherNode(healthTabWholeBody.background);

                if (selectedTimer <= 0)
                {
                    WholeBodyAfflictionSelector();
                }
            }
            else
            {
                int maxVertical = 0;
                int lastMaxVertical = 0;

                int selectedBodyPart = 0;
                int selectedAffliction = 0;

                for (int i = 0; i < healthTabBodyParts.Count; i++)
                {
                    maxVertical += healthTabBodyParts[i].allAfflictions.Count;

                    if (lastMaxVertical <= selectedVertical - (healthTabWholeBody.active ? healthTabWholeBody.afflictionNumber : 0) && maxVertical > selectedVertical - (healthTabWholeBody.active ? healthTabWholeBody.afflictionNumber : 0))
                    {
                        selectedBodyPart = i;
                        selectedAffliction = selectedVertical - lastMaxVertical - healthTabWholeBody.afflictionNumber;
                        break;
                    }
                    else
                    {
                        lastMaxVertical = maxVertical;
                    }
                }

                int extraHeight = healthTabBodyParts[selectedBodyPart].allAfflictionsHeight[selectedAffliction][0] - 1;
                int prevHeight = healthTabBodyParts[selectedBodyPart].allAfflictionsHeight[selectedAffliction][1];

                selectedSprite.y = healthTabBodyParts[selectedBodyPart].DrawPos().y - (-1 + (17.5f * selectedAffliction) + (15 * prevHeight) + (7.5f * (extraHeight + 1)));

                selectedSprite.MoveInFrontOfOtherNode(healthTabBodyParts[selectedBodyPart].background);

                selectedSprite.scaleY = 2 + 17.5f + (15 * extraHeight);

                if (selectedTimer <= 0)
                {
                    AfflictionSelector(selectedBodyPart, selectedAffliction);
                }
            }
        }

        void CapacitySelector()
        {
            float value = 0;

            if (selectedVertical == 0)
            {
                SetPainInfo(state.pain);
            }
            else
            {
                switch (capacityValueNamesNames[selectedVertical])
                {
                    case "Pain":
                        value = state.pain;
                        break;
                    case "Consiousness":
                        value = state.consciousness;
                        break;
                    case "Moving":
                        value = state.moving;
                        break;
                    case "Manipulation":
                        value = state.manipulation;
                        break;
                    case "Talking":
                        value = state.talking;
                        break;
                    case "Eating":
                        value = state.eating;
                        break;
                    case "Sight":
                        value = state.sight;
                        break;
                    case "Hearing":
                        value = state.hearing;
                        break;
                    case "Breathing":
                        value = state.breathing;
                        break;
                    case "Blood filtrarion":
                        value = state.bloodFiltration;
                        break;
                    case "Blood pumping":
                        value = state.bloodPumping;
                        break;
                    case "Digestion":
                        value = state.digestion;
                        break;
                }

                SetInfo(value);
            }

            void SetPainInfo(float value)
            {
                if (healthTabInfos.Count == 0)
                {
                    healthTabInfos.Add(new(this));
                }
                else if (healthTabInfos.Count > 1)
                {
                    for (int i = 1; i < healthTabInfos.Count; i++)
                    {
                        healthTabInfos[i].slatedForDeletion = true;
                    }
                }

                if (healthTabInfos.Count > 0)
                {
                    healthTabInfos[0].name.text = "Pain: ";
                    healthTabInfos[0].nameStatus.text = Mathf.Clamp(Mathf.Floor(value * 100), 0, 100) + "%";

                    healthTabInfos[0].description.text = "";
                }

            }
            void SetInfo(float value)
            {
                if (healthTabInfos.Count == 0)
                {
                    healthTabInfos.Add(new(this));
                }
                else if (healthTabInfos.Count > 1)
                {
                    for (int i = 1; i < healthTabInfos.Count; i--)
                    {
                        healthTabInfos[i].slatedForDeletion = true;
                    }
                }

                string stringValue;

                if (value > 1)
                {
                    stringValue = "Enhanced";
                }
                else if (value == 1)
                {
                    stringValue = "OK";
                }
                else if (value >= 0.4f)
                {
                    stringValue = "Weakened";
                }
                else if (value >= 0.15f)
                {
                    stringValue = "Poor";
                }
                else if (value >= 0.01f)
                {
                    stringValue = "Very Poor";
                }
                else
                {
                    stringValue = "None";
                }

                if (healthTabInfos.Count > 0)
                {
                    healthTabInfos[0].name.text = capacityValueNamesNames[selectedVertical] + ": ";
                    healthTabInfos[0].nameStatus.text = stringValue;

                    healthTabInfos[0].description.text = "";
                }
            }
        }

        void WholeBodyAfflictionSelector()
        {
            if (healthTabInfos.Count == 0)
            {
                healthTabInfos.Add(new(this));
            }
            else if (healthTabInfos.Count > 1)
            {
                for (int i = 1; i < healthTabInfos.Count; i++)
                {
                    healthTabInfos[i].slatedForDeletion = true;
                }
            }

            if (healthTabWholeBody.bloodLossVisible && selectedVertical == 0)
            {
                healthTabInfos[0].name.text = healthTabWholeBody.bloodLossName.text;
                healthTabInfos[0].nameStatus.text = " : " + Mathf.Floor(state.bloodLoss * 100) + "%";

                healthTabInfos[0].description.text = "A reduction in the normal blood volume.\n" +
                    "Minor blood loss has relatively mild\n" +
                    "effects, but when blood loss becomes\n" +
                    "severe, oxygen transport becomes badly\n" +
                    "impaired and the victim loses the ability\n" +
                    "to move. Extreme blood loss leads to\n" +
                    "death.\n" +
                    "\n" +
                    "Blood loss naturally recovers over time\n" +
                    "as the body slowly regenerates its blood\n" +
                    "supply.";

                if (state.bloodLoss >= 1)
                {

                }
                else if (state.bloodLoss >= 0.6f)
                {
                    healthTabInfos[0].description.text += "\n" +
                        "\n" +
                        "  - Consciousness: -40%";

                    healthTabInfos[0].description.text += "\n" +
                        "  - Consciousness: Max 10%";
                }
                else if (state.bloodLoss >= 0.45f)
                {
                    healthTabInfos[0].description.text += "\n" +
                        "\n" +
                        "  - Consciousness: -40%";
                }
                else if (state.bloodLoss >= 0.3f)
                {
                    healthTabInfos[0].description.text += "\n" +
                        "\n" +
                        "  - Consciousness: -20%";
                }
                else if (state.bloodLoss >= 0.15f)
                {
                    healthTabInfos[0].description.text += "\n" +
                        "\n" +
                        "  - Consciousness: -10%";
                }
            }
            else
            {
                healthTabInfos[0].name.text = healthTabWholeBody.afflictionVisuals[selectedVertical - (healthTabWholeBody.bloodLossVisible ? 1 : 0)].name.text + ": ";

                if (state.wholeBodyAfflictions[selectedVertical - (healthTabWholeBody.bloodLossVisible ? 1 : 0)] is RWDisease disease)
                {
                    healthTabInfos[0].nameStatus.text = (Mathf.Floor(disease.severity * 1000) / 10) + "%";

                    if (disease is RWFlu)
                    {
                        healthTabInfos[0].description.text = "An infectious disease.";

                        if (disease.severity <= 0.665f)
                        {
                            healthTabInfos[0].description.text += "\n" +
                                "\n" +
                                "  - Consciousness: -5%\n" +
                                "  - Manipulation: -5%\n" +
                                "  - Breathing: -10%\n";
                        }
                        else if (disease.severity <= 0.832f)
                        {
                            healthTabInfos[0].description.text += "\n" +
                                "\n" +
                                "  - Consciousness: -10%\n" +
                                "  - Manipulation: -10%\n" +
                                "  - Breathing: -15%\n";
                        }
                        else
                        {
                            healthTabInfos[0].description.text += "\n" +
                                "\n" +
                                "  - Pain: -5%\n" +
                                "  - Consciousness: -15%\n" +
                                "  - Manipulation: -20%\n" +
                                "  - Breathing: -20%\n";
                        }

                        if (!disease.isTended)
                        {
                            healthTabInfos[0].description.text += "\n" +
                                "Needs tending now\n";
                        }
                        else
                        {
                            float tendedIn = disease.timeUntilTreatment;

                            string canBeTendedIn = tendedIn < 0 ? Mathf.Floor(tendedIn * 10000) / 100 + (Mathf.Floor(tendedIn * 10000) / 100 == 1 ? " second" : " seconds") : tendedIn > 60 ? Mathf.Floor(tendedIn / 60 * 10) / 10 + (Mathf.Floor(tendedIn / 60 * 10) / 10 == 1 ? " hour" : " hours") : Mathf.Floor(tendedIn * 10) / 10 + (Mathf.Floor(tendedIn * 10) / 10 == 1 ? " minute" : " minutes");

                            float expiresIn = disease.timeUntilTreatment + 3;

                            string tendingExpiresIn = expiresIn < 0 ? Mathf.Floor(expiresIn * 10000) / 100 + (Mathf.Floor(expiresIn * 10000) / 100 == 1 ? " second" : " seconds") : expiresIn > 60 ? Mathf.Floor(expiresIn / 60 * 10) / 10 + (Mathf.Floor(expiresIn / 60 * 10) / 10 == 1 ? " hour" : " hours") : Mathf.Floor(expiresIn * 10) / 10 + (Mathf.Floor(expiresIn * 10) / 10 == 1 ? " minute" : " minutes");


                            healthTabInfos[0].description.text += "\n" +
                                "Tend quality: " + Mathf.Floor(disease.tendQuality * 100) + "%\n" +
                                "Can be tended in " + canBeTendedIn + "\n" +
                                "Tending expires in " + tendingExpiresIn + "\n";
                        }
                        healthTabInfos[0].description.text += "Immunity: " + (Mathf.Floor(disease.immunity * 1000)/10) + "%";
                    }
                }
            }

        }
        void AfflictionSelector(int selectedBodyPart, int selectedAffliction)
        {
            if (healthTabInfos.Count == 0)
            {
                healthTabInfos.Add(new(this));
            }

            string name = healthTabBodyParts[selectedBodyPart].bodyPart.name;

            if (healthTabBodyParts[selectedBodyPart].bodyPart.subName != "")
            {
                name = char.ToLowerInvariant(name[0]) + name.Substring(1);

                name = healthTabBodyParts[selectedBodyPart].bodyPart.subName + " " + name;
            }

            healthTabInfos[0].name.text = name;
            healthTabInfos[0].nameStatus.text = " : " + Mathf.Floor(healthTabBodyParts[selectedBodyPart].bodyPart.health) + " / " + healthTabBodyParts[selectedBodyPart].bodyPart.maxHealth;

            float efficiency = Mathf.Floor(healthTabBodyParts[selectedBodyPart].bodyPart.efficiency * 100);

            healthTabInfos[0].description.text = "Efficiency: " + ((efficiency < 1 && efficiency > 0) ? 1 : efficiency) + "%";

            bool isCombined = false;

            List<RWAffliction> afflictions = null;
            RWAffliction affliction = healthTabBodyParts[selectedBodyPart].allAfflictions[selectedAffliction];

            for (int j = 0; j < healthTabBodyParts[selectedBodyPart].combinedAfflictions.Count; j++)
            {
                if (healthTabBodyParts[selectedBodyPart].combinedAfflictions.TryGetValue(healthTabBodyParts[selectedBodyPart].CombinedAfflictionName(healthTabBodyParts[selectedBodyPart].bodyPart, j), out List<RWAffliction> list) && list.Contains(affliction))
                {
                    for (int k = 0; k < list.Count; k++)
                    {
                        if (healthTabBodyParts[selectedBodyPart].allAfflictions.Contains(list[k]))
                        {
                            isCombined = true;
                            afflictions = list;
                            break;
                        }
                    }
                    break;
                }
            }

            if (isCombined)
            {
                if (afflictions == null)
                {
                    if (healthTabInfos.Count > 1)
                    {
                        for (int i = 1; i < healthTabInfos.Count; i++)
                        {
                            healthTabInfos[i].slatedForDeletion = true;
                        }
                    }
                    return;
                }

                for (int i = 0; i < afflictions.Count; i++)
                {
                    affliction = afflictions[i];

                    if (healthTabInfos.Count < i + 2)
                    {
                        healthTabInfos.Add(new(this));
                    }
                    else if (healthTabInfos.Count > afflictions.Count + 1)
                    {
                        for (int k = afflictions.Count + 1; k < healthTabInfos.Count; k++)
                        {
                            healthTabInfos[k].slatedForDeletion = true;
                        }
                    }

                    int j = i + 1;

                    if (affliction is RWInjury injury)
                    {
                        healthTabInfos[j].name.text = injury.healingDifficulty.name + (injury.attackName != "" ? " (" + injury.attackName + ") " : "") + (afflictions.Count > 1 ? " x" + afflictions.Count : "");

                        healthTabInfos[j].nameStatus.text = ": " + (Mathf.Floor(injury.damage * 10) / 10).ToString();

                        string description = "";

                        float bleeding = Mathf.Max(1f, Mathf.Floor(injury.healingDifficulty.bleeding * injury.damage * state.bodySizeFactor * state.BloodLossMultiplier(healthTabBodyParts[selectedBodyPart].bodyPart)));

                        if (injury.isBleeding)
                        {
                            description += "  - Bleeding: ";
                            description += bleeding.ToString();
                            description += "%/c";
                        }

                        float pain = Mathf.Floor(injury.damage * injury.healingDifficulty.pain / state.bodySizeFactor / 100);

                        if (injury.healingDifficulty.pain > 0 && pain > 0f)
                        {
                            if (description != "")
                            {
                                description += "\n";
                            }

                            description += "  - Pain: +";
                            description += pain.ToString();
                            description += "%";
                        }

                        if (!injury.isTended)
                        {
                            if (description != "")
                            {
                                description += "\n";
                                description += "\n";
                            }

                            description += "Needs tending now";
                        }
                        else
                        {
                            if (description != "")
                            {
                                description += "\n";
                                description += "\n";
                            }

                            if (healthTabBodyParts[selectedBodyPart].bodyPart.isSolid)
                            {
                                description += injury.healingDifficulty.solidTreated;
                            }
                            else if (healthTabBodyParts[selectedBodyPart].bodyPart.isInternal)
                            {
                                description += injury.healingDifficulty.innerTreated;
                            }
                            else
                            {
                                description += injury.healingDifficulty.treated;
                            }

                            description += " (quality " + Mathf.Floor(injury.tendQuality) + "%)";
                        }

                        healthTabInfos[j].description.text = description;
                    }
                }
            }
            else
            {
                if (affliction == null)
                {
                    if (healthTabInfos.Count > 1)
                    {
                        for (int i = 1; i < healthTabInfos.Count; i++)
                        {
                            healthTabInfos[i].slatedForDeletion = true;
                        }
                    }

                    return;
                }

                if (healthTabInfos.Count < 2)
                {
                    healthTabInfos.Add(new(this));
                }
                else if (healthTabInfos.Count > 2)
                {
                    for (int i = 2; i < healthTabInfos.Count; i++)
                    {
                        healthTabInfos[i].slatedForDeletion = true;
                    }
                }

                healthTabInfos[1].name.text = healthTabBodyParts[selectedBodyPart].afflictionVisuals[selectedAffliction].name.text;

                if (affliction is RWInjury injury)
                {
                    if (injury is RWDestroyed)
                    {
                        healthTabInfos[1].nameStatus.text = "";

                        healthTabInfos[1].description.text = "A body part is entirely missing.";
                    }
                    else if (injury is RWScar scar && scar.isRevealed)
                    {
                        if (scar.isPermanent)
                        {
                            name = injury.healingDifficulty.name;

                            name = char.ToLowerInvariant(name[0]) + name.Substring(1);

                            healthTabInfos[1].name.text = "Permanent " + name + ((injury.attackName != "" || scar.painCategory != "") ? " (" + (injury.attackName != "" ? injury.attackName + (scar.painCategory != "" ? ", " + scar.painCategory : "") : scar.painCategory) + ") " : "");
                        }
                        else
                        {
                            healthTabInfos[1].name.text = injury.healingDifficulty.name + " scar" + ((injury.attackName != "" || scar.painCategory != "") ? " (" + (injury.attackName != "" ? injury.attackName + (scar.painCategory != "" ? ", " + scar.painCategory : "") : scar.painCategory) + ") " : "");
                        }

                        healthTabInfos[1].nameStatus.text = ": " + (Mathf.Floor(scar.damage * 10) / 10).ToString();

                        string description = "";

                        if (scar.painCategory != "")
                        {
                            description = "  - Pain: +";

                            if (scar.painCategory == "painful")
                            {
                                description += (scar.scarDamage * 1.5f * injury.healingDifficulty.scarPain / state.bodySizeFactor / 100).ToString();
                            }
                            else if (scar.painCategory == "aching")
                            {
                                description += (scar.scarDamage * injury.healingDifficulty.scarPain / state.bodySizeFactor / 100).ToString();
                            }
                            else if (scar.painCategory == "itchy")
                            {
                                description += (scar.scarDamage * 0.5f * injury.healingDifficulty.scarPain / state.bodySizeFactor / 100).ToString();
                            }

                            description += "%";
                        }

                        healthTabInfos[1].description.text = description;
                    }
                    else
                    {
                        healthTabInfos[1].name.text = injury.healingDifficulty.name + (injury.attackName != "" ? " (" + injury.attackName + ")" : "");
                        healthTabInfos[1].nameStatus.text = ": " + (Mathf.Floor(injury.damage * 10) / 10).ToString();

                        string description = "";

                        float bleeding = Mathf.Max(1f, Mathf.Floor(injury.healingDifficulty.bleeding * injury.damage * state.bodySizeFactor * state.BloodLossMultiplier(healthTabBodyParts[selectedBodyPart].bodyPart)));

                        if (injury.isBleeding)
                        {
                            description += "  - Bleeding: ";

                            description += bleeding.ToString();

                            description += "%/c";
                        }

                        float pain = Mathf.Floor(injury.damage * injury.healingDifficulty.pain / state.bodySizeFactor / 100);

                        if (injury.healingDifficulty.pain > 0 && pain > 0f)
                        {
                            if (description != "")
                            {
                                description += "\n";
                            }

                            description += "  - Pain: +";

                            description += pain.ToString();

                            description += "%";
                        }

                        if (!injury.isTended)
                        {
                            if (description != "")
                            {
                                description += "\n";
                                description += "\n";
                            }

                            description += "Needs tending now";
                        }
                        else
                        {
                            if (description != "")
                            {
                                description += "\n";
                                description += "\n";
                            }

                            if (healthTabBodyParts[selectedBodyPart].bodyPart.isSolid)
                            {
                                description += injury.healingDifficulty.solidTreated;
                            }
                            else if (healthTabBodyParts[selectedBodyPart].bodyPart.isInternal)
                            {
                                description += injury.healingDifficulty.innerTreated;
                            }
                            else
                            {
                                description += injury.healingDifficulty.treated;
                            }

                            description += " (quality " + Mathf.Floor(injury.tendQuality) + "%)";
                        }

                        healthTabInfos[1].description.text = description;
                    }
                }
                else if (affliction is RWDisease disease)
                {
                    healthTabInfos[1].nameStatus.text = ": " + (Mathf.Floor(disease.severity * 1000) / 10) + "%";

                    if (disease is RWInfection)
                    {
                        healthTabInfos[1].description.text = "Bacterial infection in a wound. Without\n" +
                            "treatment, the bacteria will multiply,\n" +
                            "killing local tissue, and eventually\n" +
                            "causing blood poisoning and death.";

                        if (disease.severity <= 0.32f)
                        {
                            healthTabInfos[1].description.text += "\n" +
                                "\n" +
                                "  - Pain: +5%\n";
                        }
                        else if (disease.severity <= 0.77f)
                        {
                            healthTabInfos[1].description.text += "\n" +
                                "\n" +
                                "  - Pain: +8%\n";
                        }
                        else if (disease.severity <= 0.86f)
                        {
                            healthTabInfos[1].description.text += "\n" +
                                "\n" +
                                "  - Pain: +12%\n" +
                                "  - Consciousness: -5%\n";
                        }
                        else
                        {
                            healthTabInfos[1].description.text += "\n" +
                                "\n" +
                                "  - Pain: +12%\n" +
                                "  - Consciousness: -5%\n" +
                                "  - Breathing: -5%";
                        }

                        if (!disease.isTended)
                        {
                            healthTabInfos[1].description.text += "\n" +
                                "Needs tending now\n";
                        }
                        else
                        {
                            float tendedIn = disease.timeUntilTreatment;

                            string canBeTendedIn = tendedIn < 0 ? Mathf.Floor(tendedIn * 10000) / 100 + (Mathf.Floor(tendedIn * 10000) / 100 == 1 ? " second" : " seconds") : tendedIn > 60 ? Mathf.Floor(tendedIn / 60 * 10) / 10 + (Mathf.Floor(tendedIn / 60 * 10) / 10 == 1 ? " hour" : " hours") : Mathf.Floor(tendedIn * 10) / 10 + (Mathf.Floor(tendedIn * 10) / 10 == 1 ? " minute" : " minutes");

                            float expiresIn = disease.timeUntilTreatment + 3;

                            string tendingExpiresIn = expiresIn < 0 ? Mathf.Floor(expiresIn * 10000) / 100 + (Mathf.Floor(expiresIn * 10000) / 100 == 1 ? " second" : " seconds") : expiresIn > 60 ? Mathf.Floor(expiresIn / 60 * 10) / 10 + (Mathf.Floor(expiresIn / 60 * 10) / 10 == 1 ? " hour" : " hours") : Mathf.Floor(expiresIn * 10) / 10 + (Mathf.Floor(expiresIn * 10) / 10 == 1 ? " minute" : " minutes");


                            healthTabInfos[1].description.text += "\n" +
                                "Tend quality: " + Mathf.Floor(disease.tendQuality * 100) + "%\n" +
                                "Can be tended in " + canBeTendedIn + "\n" +
                                "Tending expires in " + tendingExpiresIn + "\n";
                        }
                        healthTabInfos[1].description.text += "Immunity: " + (Mathf.Floor(disease.immunity * 1000) / 10) + "%";
                    }
                }
            }
        }
    }

    public override void ClearSprites()
    {
        base.ClearSprites();

        healthTabWholeBody.ClearSprites();

        for (int i = 0; i < healthTabBodyParts.Count; i++)
        {
            healthTabBodyParts[i].ClearSprites();
        }

        for (int i = 0; i < healthTabInfos.Count; i++)
        {
            healthTabInfos[i].ClearSprites();
        }

        capacityName.RemoveFromContainer();

        for (int i = 0; i < capacityValueNames.Count; i++)
        {
            capacityValueNames[i].RemoveFromContainer();
        }

        for (int i = 0; i < capacityValues.Count; i++)
        {
            capacityValues[i].RemoveFromContainer();
        }

        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].RemoveFromContainer();
        }

        selectedSprite.RemoveFromContainer();
        treatedSprite.RemoveFromContainer();
    }

    void TriggeredOn()
    {
        selected = false;

        selectedHorizontal = 0;
        selectedVertical = -1;

        horizontalOnce = true;
        verticalOnce = true;

        selectedTimer = selectedTimerMax;

        treating = false;
        treatedAffliction = null;
    }
    void TriggeredOff()
    {
        selected = false;

        selectedHorizontal = 0;
        selectedVertical = 0;

        selectedTimer = selectedTimerMax;

        treating = false;
        treatedAffliction = null;
    }

    #region Values
    public RWPlayerHealthState state;

    public FLabel capacityName;

    public FLabel bloodLossPerCycle;

    public List<FLabel> capacityValueNames = new();
    public List<FLabel> capacityValues = new();
    public FSprite[] sprites;

    public FSprite selectedSprite;

    public FSprite treatedSprite;

    public List<string> capacityValueNamesNames;

    public AbstractCreature owner;

    public bool visible = false;

    public HealthTabWholeBody healthTabWholeBody;

    public List<HealthTabBodyPart> healthTabBodyParts = new();

    public List<HealthTabInfo> healthTabInfos = new();

    public bool selected = false;
    public float selectedTimer = 5;
    readonly float selectedTimerMax = 5;

    public int selectedHorizontal = 0;
    public int selectedVertical = -1;

    bool horizontalOnce = true;
    bool verticalOnce = true;

    bool treating = false;

    float treatTime = 10;
    float treatTimeMax = 10;
    readonly float treatTimeBase = 60;

    RWAffliction treatedAffliction = null;

    public Player.InputPackage input;
    #endregion
}

public class HealthTabBodyPart
{
    public HealthTabBodyPart(HealthTab owner, RWBodyPart bodyPart)
    {
        this.owner = owner;

        this.bodyPart = bodyPart;

        background = new("pixel", true);

        string name = bodyPart.name;

        if (bodyPart.subName != "")
        {
            name = char.ToLowerInvariant(name[0]) + name.Substring(1);

            name = bodyPart.subName + " " + name;
        }

        bodyPartName = new(Custom.GetFont(), name)
        {
            alignment = FLabelAlignment.Left,
            anchorX = 0f,
            anchorY = 1f
        };

        owner.hud.fContainers[1].AddChild(background);
        owner.hud.fContainers[1].AddChild(bodyPartName);

        afflictionVisuals.Add(new(owner));
    }

    public Vector2 DrawPos()
    {
        Vector2 pos = owner.DrawPos();

        pos.y += 120;

        if (owner.healthTabWholeBody.active)
        {
            pos.y -= 10f + ((owner.healthTabWholeBody.afflictionNumber * 8.75f) + ((owner.healthTabWholeBody.afflictionNumber - 1) * 8.75f) + (owner.healthTabWholeBody.extraHeight * 7.5f) + (owner.healthTabWholeBody.extraHeight * 7.5f));
        }

        for (int i = 0; i < owner.healthTabBodyParts.IndexOf(this); i++)
        {
            pos.y -= 10f + ((owner.healthTabBodyParts[i].afflictionNumber * 8.75f) + ((owner.healthTabBodyParts[i].afflictionNumber - 1) * 8.75f) + (owner.healthTabBodyParts[i].extraHeight * 7.5f) + (owner.healthTabBodyParts[i].extraHeight * 7.5f));
        }

        return pos;
    }

    public void Update()
    {
        if (bodyPart == null || bodyPart.afflictions.Count <= 0)
        {
            slatedForDeletion = true;
            return;
        }

        afflictionNumber = bodyPart.afflictions.Count;

        combinedAfflictions = new();
        afflictions = new();

        for (int i = 0; i < bodyPart.afflictions.Count; i++)
        {
            if (bodyPart.afflictions[i] is RWInjury injury && bodyPart.afflictions[i] is not RWDestroyed && injury.healingDifficulty.combines)
            {
                if (combinedAfflictions.TryGetValue(CombinedAfflictionName(bodyPart, i), out List<RWAffliction> dic))
                {
                    dic.Add(injury);
                    afflictionNumber--;
                }
                else
                {
                    combinedAfflictions.Add(CombinedAfflictionName(bodyPart, i), new(1) { injury });
                }
            }
            else
            {
                afflictions.Add(bodyPart.afflictions[i]);
            }
        }

        if (afflictionNumber < afflictionVisuals.Count)
        {
            List<HealthTabAffliction> list = new(afflictionVisuals);

            for (int i = list.Count - 1; i >= afflictionNumber; i--)
            {
                afflictionVisuals[i].ClearSprites();

                afflictionVisuals.Remove(list[i]);
            }
        }
        else if (afflictionNumber > afflictionVisuals.Count)
        {
            List<HealthTabAffliction> list = new(afflictionVisuals);

            for (int i = 0; i < afflictionNumber - list.Count; i++)
            {
                afflictionVisuals.Add(new(owner));
            }
        }
    }

    public void Draw()
    {
        background.isVisible = owner.visible && owner.healthTabWholeBody.active ? !(owner.healthTabBodyParts.IndexOf(this) % 2 == 0) : owner.healthTabBodyParts.IndexOf(this) % 2 == 0;
        bodyPartName.isVisible = owner.visible;

        if (owner == null || !owner.visible)
        {
            return;
        }

        if (background.isVisible)
        {
            background.x = DrawPos().x;
            background.y = DrawPos().y - (-2 + (8.75f * afflictionNumber));
            background.scaleX = 340;
            background.scaleY = 2 + 17.5f * afflictionNumber;
            background.color = Color.black;
        }

        bodyPartName.x = DrawPos().x - 165;
        bodyPartName.y = DrawPos().y;
        bodyPartName.color = Color.white;

        List<string> usedCombinedAfflictions = new();
        List<RWAffliction> usedAfflictions = new();

        combinedAfflictionsHeight = new();
        afflictionsHeight = new();
        allAfflictions = new();
        allAfflictionsHeight = new();
        totalHeight = 0;
        extraHeight = 0;

        for (int i = 0; i < afflictionVisuals.Count; i++)
        {
            afflictionVisuals[i].name.x = DrawPos().x - 45;
            afflictionVisuals[i].name.y = DrawPos().y - 17.5f * i;
            afflictionVisuals[i].name.color = Color.white;

            afflictionVisuals[i].icon.x = DrawPos().x + 160;
            afflictionVisuals[i].icon.y = DrawPos().y - 7 - 17.5f * i;
            afflictionVisuals[i].icon.scale = 14;

            for (int j = 0; j < bodyPart.afflictions.Count; j++)
            {
                if (!usedCombinedAfflictions.Contains(CombinedAfflictionName(bodyPart, j)) && combinedAfflictions.TryGetValue(CombinedAfflictionName(bodyPart, j), out List<RWAffliction> dic))
                {
                    usedCombinedAfflictions.Add(CombinedAfflictionName(bodyPart, j));

                    if (bodyPart.afflictions[j] is RWInjury injury)
                    {
                        string text;

                        text = injury.healingDifficulty.name + (injury.attackName != "" ? " (" + injury.attackName + ")" : "") + (dic.Count > 1 ? " x" + dic.Count : "");

                        afflictionVisuals[i].name.text = text;
                        Menu.MenuLabel.WordWrapLabel(afflictionVisuals[i].name, wordWrap);

                        allAfflictionsHeight.Add(new(2) { Mathf.FloorToInt(afflictionVisuals[i].name.textRect.height / afflictionVisuals[i].name.FontLineHeight), extraHeight });

                        HeightChanges(i, Mathf.FloorToInt(afflictionVisuals[i].name.textRect.height / afflictionVisuals[i].name.FontLineHeight));

                        afflictionVisuals[i].icon.color = injury.isTended ? Color.white : injury.isBleeding ? Color.red : Color.blue;
                    }

                    allAfflictions.Add(bodyPart.afflictions[j]);

                    break;
                }
                else if (!usedAfflictions.Contains(bodyPart.afflictions[j]) && afflictions.Contains(bodyPart.afflictions[j]))
                {
                    usedAfflictions.Add(bodyPart.afflictions[j]);

                    if (bodyPart.afflictions[j] is RWInjury injury)
                    {
                        string text;

                        if (injury is RWDestroyed)
                        {
                            text = (bodyPart.isInternal ? bodyPart.isSolid ? "Shattered" : injury.healingDifficulty.destroyedOut : injury.healingDifficulty.destroyed) + (injury.isBleeding && !injury.isTended ? " (fresh)" : "");
                        }
                        else if (injury is RWScar scar && scar.isRevealed)
                        {
                            if (scar.isPermanent)
                            {
                                string name = injury.healingDifficulty.name;

                                name = char.ToLowerInvariant(name[0]) + name.Substring(1);

                                text = "Permanent " + name + ((injury.attackName != "" || scar.painCategory != "") ? " (" + (injury.attackName != "" ? injury.attackName + (scar.painCategory != "" ? ", " + scar.painCategory : "") : scar.painCategory) + ") " : "");
                            }
                            else
                            {
                                text = injury.healingDifficulty.name + " scar" + ((injury.attackName != "" || scar.painCategory != "") ? " (" + (injury.attackName != "" ? injury.attackName + (scar.painCategory != "" ? ", " + scar.painCategory : "") : scar.painCategory) + ") " : "");
                            }
                        }
                        else
                        {
                            text = injury.healingDifficulty.name + (injury.attackName != "" ? " (" + injury.attackName + ") " : "");
                        }

                        afflictionVisuals[i].name.text = text;
                        afflictionVisuals[i].name.color = Color.white;

                        Menu.MenuLabel.WordWrapLabel(afflictionVisuals[i].name, wordWrap);
                        allAfflictionsHeight.Add(new(2) { Mathf.FloorToInt(afflictionVisuals[i].name.textRect.height / afflictionVisuals[i].name.FontLineHeight), extraHeight });

                        HeightChanges(i, Mathf.FloorToInt(afflictionVisuals[i].name.textRect.height / afflictionVisuals[i].name.FontLineHeight));

                        afflictionVisuals[i].icon.color = injury.isTended ? Color.white : injury.isBleeding ? Color.red : Color.blue;
                    }
                    else if (bodyPart.afflictions[j] is RWDisease disease)
                    {
                        string text = disease.name;

                        if (disease is RWInfection && disease.severity > 0)
                        {
                            if (disease.severity <= 0.32f)
                            {
                                text += " (minor)";
                            }
                            else if (disease.severity <= 0.77f)
                            {
                                text += " (major)";
                            }
                            else
                            {
                                text += " (extreme)";
                            }
                        }

                        afflictionVisuals[i].name.text = text;
                        afflictionVisuals[i].name.color = Color.yellow;

                        Menu.MenuLabel.WordWrapLabel(afflictionVisuals[i].name, wordWrap);

                        allAfflictionsHeight.Add(new(2) { Mathf.FloorToInt(afflictionVisuals[i].name.textRect.height / afflictionVisuals[i].name.FontLineHeight), extraHeight });

                        HeightChanges(i, Mathf.FloorToInt(afflictionVisuals[i].name.textRect.height / afflictionVisuals[i].name.FontLineHeight));

                        afflictionVisuals[i].icon.color = disease.isTended ? Color.white : Color.grey;
                    }

                    allAfflictions.Add(bodyPart.afflictions[j]);

                    break;
                }
            }
        }

        void HeightChanges(int i, int height)
        {
            afflictionVisuals[i].name.y -= 15 * extraHeight;
            afflictionVisuals[i].icon.y -= 15 * extraHeight;

            background.y -= 7.5f * (height - 1);
            background.scaleY += 15 * (height - 1);

            totalHeight += height;
            extraHeight += height - 1;
        }
    }

    public void ClearSprites()
    {
        background.RemoveFromContainer();

        bodyPartName.RemoveFromContainer();

        for (int i = 0; i < afflictionVisuals.Count; i++)
        {
            afflictionVisuals[i].ClearSprites();
        }

        afflictionVisuals.Clear();

        combinedAfflictions.Clear();
        combinedAfflictionsHeight.Clear();
        afflictions.Clear();
        afflictionsHeight.Clear();

        allAfflictions.Clear();
        allAfflictionsHeight.Clear();
    }

    public string CombinedAfflictionName(RWBodyPart bodyPart, int i)
    {
        string name = "";

        if (bodyPart.afflictions[i] is RWInjury injury)
        {
            name = injury.healingDifficulty.name + injury.attackName + injury.isTended;
        }
        else if (bodyPart.afflictions[i] is RWDisease disease)
        {
            name = disease.name;
        }

        return name;
    }

    #region Values
    public HealthTab owner;

    public RWBodyPart bodyPart;

    public FSprite background;

    public FLabel bodyPartName;

    public List<HealthTabAffliction> afflictionVisuals = new();

    public int afflictionNumber = 1;
    public int totalHeight = 1;
    public int extraHeight = 0;

    public Dictionary<string, List<RWAffliction>> combinedAfflictions;
    public Dictionary<string, List<int>> combinedAfflictionsHeight;
    public List<RWAffliction> afflictions;
    public List<List<int>> afflictionsHeight;

    public List<RWAffliction> allAfflictions;
    public List<List<int>> allAfflictionsHeight;

    public int partPriority;

    public bool slatedForDeletion = false;

    const float wordWrap = 200;
    #endregion
}

public class HealthTabWholeBody
{
    public HealthTabWholeBody(HealthTab owner)
    {
        this.owner = owner;

        background = new("pixel", true);

        name = new(Custom.GetFont(), "Whole Body")
        {
            alignment = FLabelAlignment.Left,
            anchorX = 0f,
            anchorY = 1f,
            color = Color.grey
        };

        bloodLossName = new(Custom.GetFont(), "Blood loss ()")
        {
            alignment = FLabelAlignment.Left,
            anchorX = 0f,
            anchorY = 1f,
            color = Color.white
        };

        owner.hud.fContainers[1].AddChild(background);
        owner.hud.fContainers[1].AddChild(name);

        owner.hud.fContainers[1].AddChild(bloodLossName);
    }

    public Vector2 DrawPos()
    {
        Vector2 pos = owner.DrawPos();

        pos.y += 120;

        return pos;
    }

    public void Update()
    {
        bloodLossVisible = owner.state != null && owner.state.bloodLoss >= 0.15f;

        afflictionNumber = (bloodLossVisible ? 1 : 0) + (owner.state != null ? owner.state.wholeBodyAfflictions.Count : 0);

        active = afflictionNumber > 0;

        if ((owner.state != null ? owner.state.wholeBodyAfflictions.Count : 0) < afflictionVisuals.Count)
        {
            List<HealthTabAffliction> list = new(afflictionVisuals);

            for (int i = list.Count - 1; i >= afflictionNumber; i--)
            {
                afflictionVisuals[i].ClearSprites();

                afflictionVisuals.Remove(list[i]);
            }
        }
        else if ((owner.state != null ? owner.state.wholeBodyAfflictions.Count : 0) > afflictionVisuals.Count)
        {
            List<HealthTabAffliction> list = new(afflictionVisuals);

            for (int i = 0; i < afflictionNumber - list.Count; i++)
            {
                afflictionVisuals.Add(new(owner));
            }
        }
    }
    public void Draw()
    {
        background.isVisible = owner.visible && active;
        name.isVisible = owner.visible && active;
        bloodLossName.isVisible = owner.visible && active && bloodLossVisible;

        if (owner == null || owner.state == null || !owner.visible || !active)
        {
            return;
        }

        if (background.isVisible)
        {
            background.x = DrawPos().x;
            background.y = DrawPos().y - (-2 + (8.75f * afflictionNumber));
            background.scaleX = 340;
            background.scaleY = 2 + (17.5f * afflictionNumber);
            background.color = Color.black;
        }

        name.x = DrawPos().x - 165;
        name.y = DrawPos().y;

        if (bloodLossVisible)
        {
            bloodLossName.x = DrawPos().x - 45;
            bloodLossName.y = DrawPos().y;

            if (owner.state.bloodLoss >= 0.6f)
            {
                bloodLossName.text = "Blood loss (extreme)";
            }
            else if (owner.state.bloodLoss >= 0.45f)
            {
                bloodLossName.text = "Blood loss (severe)";
            }
            else if (owner.state.bloodLoss >= 0.3f)
            {
                bloodLossName.text = "Blood loss (moderate)";
            }
            else if (owner.state.bloodLoss >= 0.15f)
            {
                bloodLossName.text = "Blood loss (minor)";
            }
            else
            {
                bloodLossName.text = "Blood loss ()";
            }

            bloodLossName.color = Color.white;
        }

        for (int i = 0; i < afflictionVisuals.Count && i < owner.state.wholeBodyAfflictions.Count; i++)
        {
            afflictionVisuals[i].name.x = DrawPos().x - 45;
            afflictionVisuals[i].name.y = DrawPos().y - ((bloodLossVisible ? 17.5f : 0) + (17.5f * i));
            afflictionVisuals[i].name.color = Color.yellow;

            afflictionVisuals[i].icon.x = DrawPos().x + 160;
            afflictionVisuals[i].icon.y = DrawPos().y - 7 - ((bloodLossVisible ? 17.5f : 0) + (17.5f * i));
            afflictionVisuals[i].icon.scale = 14;

            if (owner.state.wholeBodyAfflictions[i] is RWDisease disease)
            {
                string text = disease.name;

                if (disease is RWFlu && disease.severity > 0)
                {
                    if (disease.severity <= 0.665f)
                    {
                        text += " (minor)";
                    }
                    else if (disease.severity <= 0.832f)
                    {
                        text += " (major)";
                    }
                    else
                    {
                        text += " (extreme)";
                    }
                }

                afflictionVisuals[i].name.text = text;

                Menu.MenuLabel.WordWrapLabel(afflictionVisuals[i].name, wordWrap);

                afflictionsHeight.Add(new(2) { Mathf.FloorToInt(afflictionVisuals[i].name.textRect.height / afflictionVisuals[i].name.FontLineHeight), extraHeight });

                HeightChanges(i, Mathf.FloorToInt(afflictionVisuals[i].name.textRect.height / afflictionVisuals[i].name.FontLineHeight));

                afflictionVisuals[i].icon.color = disease.isTended ? Color.white : Color.grey;
            }
        }

        void HeightChanges(int i, int height)
        {
            afflictionVisuals[i].name.y -= 15 * extraHeight;
            afflictionVisuals[i].icon.y -= 15 * extraHeight;

            background.y -= 7.5f * (height - 1);
            background.scaleY += 15 * (height - 1);

            totalHeight += height;
            extraHeight += height - 1;
        }
    }

    public void ClearSprites()
    {
        background.RemoveFromContainer();

        name.RemoveFromContainer();

        bloodLossName.RemoveFromContainer();

        for (int i = 0; i < afflictionVisuals.Count; i++)
        {
            afflictionVisuals[i].ClearSprites();
        }

        afflictionVisuals.Clear();
        afflictionsHeight.Clear();
    }

    #region Values
    public HealthTab owner;

    public FSprite background;

    public FLabel name;

    public FLabel bloodLossName;

    public List<HealthTabAffliction> afflictionVisuals = new();

    public List<List<int>> afflictionsHeight = new();

    public int afflictionNumber = 0;
    public int totalHeight = 1;
    public int extraHeight = 0;

    public bool bloodLossVisible = false;

    public bool active = false;

    const float wordWrap = 200;
    #endregion
}

public class HealthTabAffliction
{
    public HealthTabAffliction(HealthTab owner)
    {
        name = new FLabel(Custom.GetFont(), "capacityName")
        {
            alignment = FLabelAlignment.Left,
            anchorX = 0f,
            anchorY = 1f
        };

        icon = new FSprite("pixel", true);

        owner.hud.fContainers[1].AddChild(name);
        owner.hud.fContainers[1].AddChild(icon);
    }

    public void ClearSprites()
    {
        name.RemoveFromContainer();
        icon.RemoveFromContainer();
    }

    #region Values
    public RWBodyPart bodyPart;

    public FLabel name;
    public FSprite icon;
    #endregion
}

public class HealthTabInfo
{
    public HealthTabInfo(HealthTab owner)
    {
        this.owner = owner;

        backgrounds = new FSprite[2];

        name = new(Custom.GetFont(), "Name")
        {
            alignment = FLabelAlignment.Left,
            anchorX = 0f,
            anchorY = 1f
        };

        nameStatus = new(Custom.GetFont(), " : Status")
        {
            alignment = FLabelAlignment.Left,
            anchorX = 0f,
            anchorY = 1f
        };

        description = new(Custom.GetFont(), "")
        {
            alignment = FLabelAlignment.Left,
            anchorX = 0f,
            anchorY = 1f
        };

        for (int k = 0; k < backgrounds.Length; k++)
        {
            backgrounds[k] = new FSprite("pixel", true);
            owner.hud.fContainers[1].AddChild(backgrounds[k]);
        }

        owner.hud.fContainers[1].AddChild(name);
        owner.hud.fContainers[1].AddChild(nameStatus);

        owner.hud.fContainers[1].AddChild(description);
    }

    public Vector2 DrawPos()
    {
        Vector2 pos = new(owner.selectedSprite.x, owner.selectedSprite.y);

        pos.x += (owner.selectedSprite.scaleX / 2) + 10;
        pos.y -= (owner.selectedSprite.scaleY / 2) + 5;

        for (int i = 0; i < owner.healthTabInfos.IndexOf(this); i++)
        {
            pos.y -= 2 + owner.healthTabInfos[i].backgrounds[0].scaleY;
        }

        return pos;
    }

    public void Update()
    {
        if (!owner.selected)
        {
            slatedForDeletion = true;
        }
    }
    public void Draw()
    {
        bool visible = owner.visible && owner.selectedTimer <= 0;

        for (int i = 0; i < backgrounds.Length; i++)
        {
            backgrounds[i].isVisible = visible;
        }

        name.isVisible = visible;
        nameStatus.isVisible = visible;

        description.isVisible = visible && description.text != "";

        if (!visible)
        {
            return;
        }

        if (owner.healthTabInfos.IndexOf(this) != 0 || owner.healthTabInfos.IndexOf(this) == 0 && owner.healthTabWholeBody.active && owner.selectedVertical < owner.healthTabWholeBody.afflictionNumber)
        {
            name.color = Color.yellow;
        }
        else
        {
            name.color = Color.white;
        }

        name.x = DrawPos().x;
        name.y = DrawPos().y;

        nameStatus.x = DrawPos().x;

        nameStatus.x += name.textRect.width;

        nameStatus.y = DrawPos().y;

        Vector2 backgroundPos = new()
        {
            x = name.textRect.width + nameStatus.textRect.width,
            y = name.textRect.height
        };

        Vector2 backgroundSize = new()
        {
            x = name.textRect.width + nameStatus.textRect.width,
            y = name.textRect.height
        };

        if (owner.selectedHorizontal == 1 && description.text != "")
        {
            if (owner.healthTabInfos.IndexOf(this) == 0)
            {
                if (owner.healthTabWholeBody.bloodLossVisible && owner.selectedVertical == 0)
                {
                    description.x = name.x;
                    description.y = name.y - (name.textRect.height * 2);

                    backgroundPos.y -= name.textRect.height;
                    backgroundSize.y += name.textRect.height;
                }
                else
                {
                    description.x = name.x;
                    description.y = name.y - name.textRect.height;
                }
            }
            else
            {
                description.x = name.x;
                description.y = name.y - (name.textRect.height * 2);

                backgroundPos.y -= name.textRect.height;
                backgroundSize.y += name.textRect.height;
            }

            if (backgroundPos.x < description.textRect.width)
            {
                backgroundPos.x = description.textRect.width;
            }

            backgroundPos.y -= description.textRect.height;

            if (backgroundSize.x < description.textRect.width)
            {
                backgroundSize.x = description.textRect.width;
            }

            backgroundSize.y += description.textRect.height;
        }

        backgrounds[0].x = DrawPos().x + (backgroundPos.x / 2);
        backgrounds[0].y = DrawPos().y + (backgroundPos.y / 2) - 15;

        backgrounds[0].scaleX = backgroundSize.x + 10;
        backgrounds[0].scaleY = backgroundSize.y + 10;
        backgrounds[0].color = Color.black;

        backgrounds[1].x = DrawPos().x + (backgroundPos.x / 2);
        backgrounds[1].y = DrawPos().y + (backgroundPos.y / 2) - 15;

        backgrounds[1].scaleX = backgroundSize.x + 8;
        backgrounds[1].scaleY = backgroundSize.y + 8;
        backgrounds[1].color = Color.grey;
    }

    public void ClearSprites()
    {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            backgrounds[i].RemoveFromContainer();
        }

        name.RemoveFromContainer();
        nameStatus.RemoveFromContainer();

        description.RemoveFromContainer();
    }

    #region Values
    public HealthTab owner;

    public FSprite[] backgrounds;

    public FLabel name;
    public FLabel nameStatus;

    public FLabel description;

    public bool slatedForDeletion = false;
    #endregion
}