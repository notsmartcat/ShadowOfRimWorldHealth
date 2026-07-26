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
        if (owner != null)
        {
            player = owner;

            playerState = healthState.TryGetValue(player.state, out RWState state) ? state : null;
        }

        sprites = new FSprite[2];

        capacityName = new(Custom.GetFont(), "Name")
        {
            alignment = FLabelAlignment.Left,
            anchorX = 0f,
            anchorY = 1f
        };

        bloodLossPerCycle = new(Custom.GetFont(), "Blood loss")
        {
            alignment = FLabelAlignment.Left,
            anchorX = 0f,
            anchorY = 1f
        };

        for (int i = 0; i < 12; i++)
        {
            capacityValueNames.Add(new FLabel(Custom.GetFont(), "Capacity name"));
            capacityValueNames[i].alignment = FLabelAlignment.Left;
            capacityValueNames[i].anchorX = 0f;
            capacityValueNames[i].anchorY = 1f;

            capacityValues.Add(new FLabel(Custom.GetFont(), "Stat value"));
            capacityValues[i].alignment = FLabelAlignment.Left;
            capacityValues[i].anchorX = 1f;
            capacityValues[i].anchorY = 1f;
        }

        selectedSprite = new FSprite("pixel", true);
        treatedSprite = new FSprite("pixel", true);

        afflictionsAboveLabel = new(Custom.GetFont(), "Aflictions above: 0")
        {
            alignment = FLabelAlignment.Right,
            anchorX = 1f,
            anchorY = 1f
        };
        
        afflictionsBelowLabel = new(Custom.GetFont(), "Aflictions below: 0")
        {
            alignment = FLabelAlignment.Right,
            anchorX = 1f,
            anchorY = 1f
        };

        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i] = new FSprite("pixel", true);
            hud.fContainers[1].AddChild(sprites[i]);
        }

        hud.fContainers[1].AddChild(capacityName);
        hud.fContainers[1].AddChild(bloodLossPerCycle);

        for (int i = 0; i < capacityValueNames.Count; i++)
        {
            hud.fContainers[1].AddChild(capacityValueNames[i]);
            hud.fContainers[1].AddChild(capacityValues[i]);
        }

        hud.fContainers[1].AddChild(selectedSprite);
        hud.fContainers[1].AddChild(treatedSprite);

        hud.fContainers[1].AddChild(afflictionsAboveLabel);
        hud.fContainers[1].AddChild(afflictionsBelowLabel);

        healthTabWholeBody = new HealthTabWholeBody(this);
    }

    public Vector2 DrawPos()
    {
        return new Vector2(hud.rainWorld.screenSize.x / 2, hud.rainWorld.screenSize.y / 2) + new Vector2(0.1f, 0.1f);
    }

    public override void Update()
    {
        base.Update();

        if (player == null || player.realizedCreature == null || !player.realizedCreature.dead && player.realizedCreature.Stunned)
        {
            visible = false;
        }

        for (int i = healthTabBodyParts.Count - 1; i >= 0; i--)
        {
            if (healthTabBodyParts[i].slatedForDeletion || inspectedState == null || !visible)
            {
                healthTabBodyParts[i].ClearSprites();
                healthTabBodyParts.RemoveAt(i);
            }
        }
        for (int i = healthTabInfos.Count - 1; i >= 0; i--)
        {
            if (healthTabInfos[i].slatedForDeletion || inspectedState == null || !visible)
            {
                healthTabInfos[i].ClearSprites();
                healthTabInfos.RemoveAt(i);
            }
        }

        healthTabWholeBody.Update();

        for (int i = 0; i < healthTabBodyParts.Count; i++)
        {
            healthTabBodyParts[i].Update();
        }

        for (int i = healthTabInfos.Count - 1; i >= 0; i--)
        {
            healthTabInfos[i].Update();
        }

        if (inspectedState == null || !visible)
        {
            return;
        }

        foreach (RWBodyPart part in inspectedState.bodyParts)
        {
            if (part.afflictions.Count == 0)
            {
                continue;
            }

            bool alreadyExists = false;
            foreach (var bodyPartTab in healthTabBodyParts)
            {
                if (bodyPartTab.bodyPart == part)
                {
                    alreadyExists = true;

                    if (IsSubPartDestroyed(inspectedState, bodyPartTab.bodyPart))
                    {
                        bodyPartTab.slatedForDeletion = true;
                    }
                    break;
                }
            }

            if (!alreadyExists && !IsSubPartDestroyed(inspectedState, part))
            {
                HealthTabBodyPart partTab = new(this, part);

                healthTabBodyParts.Add(partTab);

                partTab.Update();
            }
        }

        if (input.x != 0)
        {
            if (horizontalOnce || healthTabBodyParts.Count + healthTabWholeBody.afflictionNumber == 0)
            {
                goto skipHorizontal;
            }

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
        else
        {
            horizontalOnce = false;
        }

    skipHorizontal:

        if (healthTabBodyParts.Count + healthTabWholeBody.afflictionNumber <= 0)
            selectedHorizontal = 0;

        if (input.y != 0)
        {
            if (verticalOnce)
            {
                goto skipVertical;
            }

            selected = true;

            selectedTimer = selectedTimerMax;

            verticalOnce = true;

            selectedVertical += input.y > 0 ? -1 : 1;

            int maxVertical = selectedHorizontal == 0 ? !inspectedCreatureState.dead ? 12 : 1 : -1;

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
        else
        {
            verticalOnce = false;
        }

    skipVertical:

        if (selectedHorizontal == 1)
        {
            int maxVertical = healthTabWholeBody.active ? healthTabWholeBody.afflictionNumber : 0;

            foreach (HealthTabBodyPart bodyPartTab in healthTabBodyParts)
            {
                maxVertical += bodyPartTab.combinedAfflictions.Count + bodyPartTab.afflictions.Count;
            }

            if (selectedVertical >= maxVertical)
            {
                selectedVertical = maxVertical - 1;
            }
        }

        if (playerState.tendAffliction != null)
        {
            playerState.tendTime--;

            if (playerState.tendAffliction.state == null || playerState.tendAffliction.isTended && (playerState.tendAffliction is not RWDisease tendedDisease || tendedDisease.timeUntilTreatment > 0))
            {
                playerState.tendAffliction = null;
            }
            else if (playerState.tendTime <= 0)
            {
                playerState.tendAffliction.isTended = true;
                playerState.tendAffliction.tendQuality = Mathf.Clamp(RWHealthState.MedicalTendQuality(playerState) * 0.3f * (player == inspectedCreatureState.creature ? 0.7f : 1) * Random.Range(0.75f, 1.25f), 0, 0.7f);

                if (playerState.tendAffliction is RWInjury injury)
                {
                    injury.isBleeding = false;

                    if (injury is RWDestroyed destroyed)
                    {
                        destroyed.isFresh = false;
                    }
                }
                else if (playerState.tendAffliction is RWDisease disease)
                {
                    disease.timeUntilTreatment = cycleLength * disease.treatmentTimes;
                    disease.totalTendQuality += disease.tendQuality;
                }

                playerState.tendAffliction = null;

                inspectedState.updateCapacities = true;
            }

            return;
        }

        if (input.pckp)
        {
            RWInjury bleeding = null;
            RWDisease diseaseAffliction = null;
            RWInjury untendedAffliction = null;

            foreach (RWBodyPart part in inspectedState.bodyParts)
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
                    else if (affliction is RWDisease disease && (disease.timeUntilTreatment <= 0 || !disease.isTended))
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
                foreach (RWAffliction affliction in inspectedState.wholeBodyAfflictions)
                {
                    if (affliction is not RWDisease disease || (disease.timeUntilTreatment > 0 && disease.isTended))
                    {
                        continue;
                    }

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
        }

        void startTending(RWAffliction affliction)
        {
            Debug.Log("tending " + affliction);

            playerState.tendAffliction = affliction;
            playerState.tendTime = Mathf.Round(playerState.tendTimeBase / RWHealthState.MedicalTendSpeed(playerState));
            playerState.tendTimeMax = playerState.tendTime;
        }
    }

    public void ToggleVisibility(CreatureState self, RWState state)
    {
        visible = !visible && player != null;

        if (visible)
        {
            inspectedState = state;
            inspectedCreatureState = self;

            TriggeredOn();
        }
        else
        {
            if (playerState != null)
                playerState.tendAffliction = null;

            inspectedState = null;

            TriggeredOff();
        }
    }

    public override void Draw(float timeStacker)
    {
        base.Draw(timeStacker);

        #region Visibility
        foreach (FSprite sprite in sprites)
        {
            sprite.isVisible = visible;
        }

        for (int i = 0; i < capacityValueNames.Count; i++)
        {
            capacityValueNames[i].isVisible = visible;
            capacityValues[i].isVisible = visible;
        }

        capacityName.isVisible = visible;

        bloodLossPerCycle.isVisible = visible && inspectedState != null && inspectedState.bloodLossPerCycle >= 1 && inspectedState.bloodLoss < 1 && inspectedCreatureState != null && !inspectedCreatureState.dead;

        selectedSprite.isVisible = visible && selected;

        treatedSprite.isVisible = visible && playerState != null && playerState.tendAffliction != null;
        #endregion

        afflictionsAbove = 0;
        afflictionsBelow = 0;

        #region Updates
        healthTabWholeBody.Draw();

        foreach (HealthTabBodyPart bodyPartTab in healthTabBodyParts)
        {
            bodyPartTab.Draw();
        }

        foreach (var infoTab in healthTabInfos)
        {
            infoTab.Draw();
        }
        #endregion

        afflictionsAboveLabel.isVisible = visible && afflictionsAbove > 0;
        afflictionsBelowLabel.isVisible = visible && afflictionsBelow > 0;

        if (!visible || playerState == null || inspectedState == null)
        {
            return;
        }

        if (!inspectedCreatureState.dead)
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
        capacityName.text = GetCreatureName(inspectedCreatureState.creature.realizedCreature);

        afflictionsAboveLabel.color = Color.white;
        afflictionsAboveLabel.x = DrawPos().x + 170;
        afflictionsAboveLabel.y = DrawPos().y + 125;
        afflictionsAboveLabel.text = "Aflictions above: " + afflictionsAbove;

        afflictionsBelowLabel.color = Color.white;
        afflictionsBelowLabel.x = DrawPos().x + 170;
        afflictionsBelowLabel.y = DrawPos().y - 105;
        afflictionsBelowLabel.text = "Aflictions below: " + afflictionsBelow;

        if (bloodLossPerCycle.isVisible)
        {
            float bloodLoss = cycleLength * (1 - inspectedState.bloodLoss) / (inspectedState.bloodLossPerCycle / 100);

            bloodLossPerCycle.color = Color.white;
            bloodLossPerCycle.x = DrawPos().x - 165;
            bloodLossPerCycle.y = DrawPos().y - 105;
            bloodLossPerCycle.text = "Bleeding: " + Mathf.Round(inspectedState.bloodLossPerCycle) + "%/c " + (bloodLoss > cycleLength ? "(no immediate danger)" : "(death in " + GetTimeString(bloodLoss) + ")");
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

            float value = capacityValueNamesNames[i] switch
            {
                "Pain" => !inspectedCreatureState.dead ? inspectedState.pain : 0,
                "Consiousness" => inspectedState.consciousness,
                "Moving" => inspectedState.moving,
                "Manipulation" => inspectedState.manipulation,
                "Talking" => inspectedState.talking,
                "Eating" => inspectedState.eating,
                "Sight" => inspectedState.sight,
                "Hearing" => inspectedState.hearing,
                "Breathing" => inspectedState.breathing,
                "Blood filtrarion" => inspectedState.bloodFiltration,
                "Blood pumping" => inspectedState.bloodPumping,
                "Digestion" => inspectedState.digestion,
                _ => 0,
            };

            if (capacityValueNamesNames[i] == "Pain")
            {
                switch (value)
                {
                    case >= 0.8f:
                        capacityValues[i].text = "Mind-shattering";
                        capacityValues[i].color = Color.red;
                        break;
                    case >= 0.4f:
                        capacityValues[i].text = "Intense";
                        capacityValues[i].color = Color.Lerp(Color.white, Color.black, 0.6f);
                        break;
                    case >= 0.15f:
                        capacityValues[i].text = "Serious";
                        capacityValues[i].color = Color.Lerp(Color.white, Color.black, 0.4f);
                        break;
                    case >= 0.01f:
                        capacityValues[i].text = "Minor";
                        capacityValues[i].color = Color.Lerp(Color.white, Color.black, 0.1f);
                        break;
                    default:
                        capacityValues[i].text = "None";
                        capacityValues[i].color = Color.green;
                        break;
                }
            }
            else
            {
                capacityValues[i].text = Mathf.Round(value * 100) + "%";

                capacityValues[i].color = value switch
                {
                    > 1 => Color.blue,
                    1 => Color.green,
                    >= 0.8f => Color.Lerp(Color.white, Color.black, 0.1f),
                    >= 0.4f => Color.Lerp(Color.white, Color.black, 0.4f),
                    >= 0.15f => Color.Lerp(Color.white, Color.black, 0.6f),
                    >= 0.01f => Color.Lerp(Color.white, Color.black, 0.8f),
                    _ => Color.red,
                };
            }
        }

        if (treatedSprite.isVisible && playerState.tendAffliction != null)
        {
            float tendProgress = 1 - playerState.tendTime / playerState.tendTimeMax;

            treatedSprite.x = Mathf.Lerp(DrawPos().x - 50, DrawPos().x + 60, tendProgress);
            treatedSprite.alpha = 0.6f;
            treatedSprite.color = Color.yellow;

            treatedSprite.scaleX = Mathf.Lerp(0, 220, tendProgress);
            treatedSprite.scaleY = 20;

            int selectedBodyPart = 0;
            int selectedAffliction = 0;

            if (inspectedState.wholeBodyAfflictions.Count > 0 && inspectedState.wholeBodyAfflictions.Contains(playerState.tendAffliction))
            {
                for (int j = 0; j < inspectedState.wholeBodyAfflictions.Count - (healthTabWholeBody.bloodLossVisible ? 1 : 0); j++)
                {
                    if (inspectedState.wholeBodyAfflictions[j] == inspectedState.tendAffliction)
                    {
                        selectedAffliction = j;
                        break;
                    }
                }

                treatedSprite.isVisible = healthTabWholeBody.afflictionVisuals[selectedAffliction].name.isVisible;

                if (treatedSprite.isVisible)
                {
                    int extraHeight = 0;
                    int prevHeight = 0;

                    if (healthTabWholeBody.afflictionsHeight.Count >= selectedAffliction)
                    {
                        extraHeight = healthTabWholeBody.afflictionsHeight[selectedAffliction][0] - 1;
                        prevHeight = healthTabWholeBody.afflictionsHeight[selectedAffliction][1];
                    }

                    treatedSprite.y = healthTabWholeBody.DrawPos().y - ((healthTabWholeBody.bloodLossVisible ? 18f : 0) + -1 + (18f * selectedAffliction) + (15 * prevHeight) + (7.5f * (extraHeight + 1)));
                    treatedSprite.MoveInFrontOfOtherNode(healthTabWholeBody.background);
                    treatedSprite.scaleY = 17f + (15 * extraHeight);
                }
            }
            else
            {
                for (int i = 0; i < healthTabBodyParts.Count; i++)
                {
                    if (healthTabBodyParts[i].bodyPart == playerState.tendAffliction.part)
                    {
                        selectedBodyPart = i;

                        if (healthTabBodyParts[i].allAfflictions.Contains(playerState.tendAffliction))
                        {
                            selectedAffliction = healthTabBodyParts[i].allAfflictions.IndexOf(playerState.tendAffliction);
                            break;
                        }

                        for (int j = 0; j < healthTabBodyParts[i].combinedAfflictions.Count; j++)
                        {
                            if (healthTabBodyParts[i].combinedAfflictions.TryGetValue(healthTabBodyParts[i].CombinedAfflictionName(healthTabBodyParts[i].bodyPart, j), out List<RWAffliction> list) && list.Contains(playerState.tendAffliction))
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

                treatedSprite.isVisible = healthTabBodyParts[selectedBodyPart].afflictionVisuals[selectedAffliction].name.isVisible;

                if (treatedSprite.isVisible)
                {
                    int extraHeight = healthTabBodyParts[selectedBodyPart].allAfflictionsHeight[selectedAffliction][0] - 1;
                    int prevHeight = healthTabBodyParts[selectedBodyPart].allAfflictionsHeight[selectedAffliction][1];

                    treatedSprite.y = healthTabBodyParts[selectedBodyPart].DrawPos().y - (-1 + (18f * selectedAffliction) + (15 * prevHeight) + (7.5f * (extraHeight + 1)));

                    treatedSprite.MoveInFrontOfOtherNode(healthTabBodyParts[selectedBodyPart].background);

                    treatedSprite.scaleY = 17f + (15 * extraHeight);
                }
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

            selectedSprite.y += 100 - 7 - (17 * selectedVertical);

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
                    selectedSprite.isVisible = healthTabWholeBody.bloodLossName.isVisible;

                    selectedSprite.y = healthTabWholeBody.DrawPos().y - 6.5f;
                    selectedSprite.scaleY = 17f;
                }
                else
                {
                    if (healthTabWholeBody.afflictionVisuals.Count <= selectedVertical - (healthTabWholeBody.bloodLossVisible ? 1 : 0))
                    {
                        return;
                    }

                    selectedSprite.isVisible = healthTabWholeBody.afflictionVisuals[selectedVertical - (healthTabWholeBody.bloodLossVisible ? 1 : 0)].name.isVisible;

                    int extraHeight = healthTabWholeBody.afflictionsHeight[selectedVertical][0] - 1;
                    int prevHeight = healthTabWholeBody.afflictionsHeight[selectedVertical][1];

                    selectedSprite.y = healthTabWholeBody.DrawPos().y - ((healthTabWholeBody.bloodLossVisible ? -2 : -1) + (18f * selectedVertical) + (15 * prevHeight) + (7.5f * (extraHeight + 1)));

                    selectedSprite.scaleY = (healthTabWholeBody.bloodLossVisible ? 1 : 0) + 17f + (15 * extraHeight);
                }

                selectedSprite.MoveInFrontOfOtherNode(healthTabWholeBody.background);

                if (selectedVertical == 0)
                {
                    pivotOffset = 0;

                    topLimit = 510;
                }
                else if ((selectedSprite.y + (selectedSprite.scaleY / 2)) > topLimit)
                {
                    pivotOffset = healthTabWholeBody.GetPivotOffset(selectedVertical, true);
                }
                else if ((selectedSprite.y - (selectedSprite.scaleY / 2)) < bottomLimit)
                {
                    pivotOffset = healthTabWholeBody.GetPivotOffset(selectedVertical, false);

                    topLimit = 495;
                }

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

                if (healthTabBodyParts[selectedBodyPart].afflictionVisuals.Count <= selectedAffliction || healthTabBodyParts[selectedBodyPart].allAfflictionsHeight.Count <= selectedAffliction)
                {
                    return;
                }

                selectedSprite.isVisible = healthTabBodyParts[selectedBodyPart].afflictionVisuals[selectedAffliction].name.isVisible;

                int extraHeight = healthTabBodyParts[selectedBodyPart].allAfflictionsHeight[selectedAffliction][0] - 1;
                int prevHeight = healthTabBodyParts[selectedBodyPart].allAfflictionsHeight[selectedAffliction][1];

                selectedSprite.y = healthTabBodyParts[selectedBodyPart].DrawPos().y - (-1 + (18f * selectedAffliction) + (15 * prevHeight) + (7.5f * (extraHeight + 1)));

                selectedSprite.MoveInFrontOfOtherNode(healthTabBodyParts[selectedBodyPart].background);

                selectedSprite.scaleY = 17f + (15 * extraHeight);

                if (!healthTabWholeBody.active && selectedBodyPart == 0 && selectedAffliction == 0)
                {
                    pivotOffset = 0;

                    topLimit = 510;
                }
                else if ((selectedSprite.y + (selectedSprite.scaleY / 2)) > topLimit)
                {
                    pivotOffset = healthTabBodyParts[selectedBodyPart].GetPivotOffset(selectedAffliction, true);      
                }
                else if ((selectedSprite.y - (selectedSprite.scaleY / 2)) < bottomLimit)
                {
                    pivotOffset = healthTabBodyParts[selectedBodyPart].GetPivotOffset(selectedAffliction, false);

                    topLimit = 495;
                }

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
                SetPainInfo(!inspectedCreatureState.dead ? inspectedState.pain : 0);
            }
            else
            {
                SetInfo();
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
                    healthTabInfos[0].nameStatus.text = Mathf.Clamp(Mathf.Round(value * 100), 0, 100) + "%";

                    healthTabInfos[0].description.text = "";
                }
            }
            void SetInfo()
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

                string stringValue = capacityValueNamesNames[selectedVertical];
                string description = "\n" +
                    "\n" +
                    "Affected by:\n";

                bool isCapacityAffected = false;

                switch (stringValue)
                {
                    case "Consiousness":
                        value = inspectedState.consciousness;

                        if (inspectedState.consciousnessSource != null && inspectedState.consciousnessSource.efficiency != 1)
                        {
                            isCapacityAffected = true;
                            description += "  " + inspectedState.consciousnessSource.name + ": " + Mathf.Round(inspectedState.consciousnessSource.health) + " / " + inspectedState.consciousnessSource.maxHealth + "\n";
                        }
                        if (inspectedState.bloodLoss >= 0.15f)
                        {
                            isCapacityAffected = true;
                            description += "  " + healthTabWholeBody.bloodLossName.text + "\n";
                        }
                        if (inspectedState.pain > 0.1f)
                        {
                            isCapacityAffected = true;
                            description += "  Pain " + Mathf.Round(inspectedState.pain * 100) + "%\n";
                        }
                        if (inspectedState.bloodPumping < 1f)
                        {
                            isCapacityAffected = true;
                            description += "  Blood pumping " + Mathf.Round(inspectedState.bloodPumping * 100) + "%\n";
                        }
                        if (inspectedState.breathing < 1f)
                        {
                            isCapacityAffected = true;
                            description += "  Breathing " + Mathf.Round(inspectedState.breathing * 100) + "%\n";
                        }
                        if (inspectedState.bloodFiltration < 1f)
                        {
                            isCapacityAffected = true;
                            description += "  Blood filtration " + Mathf.Round(inspectedState.bloodFiltration * 100) + "%\n";
                        }

                        break;
                    case "Moving":
                        {
                            value = inspectedState.moving;

                            if (inspectedState.consciousness < 1f)
                            {
                                isCapacityAffected = true;
                                description += "  Consciousness " + Mathf.Round(inspectedState.consciousness * 100) + "%\n";
                            }
                            if (inspectedState.bloodPumping < 1f)
                            {
                                isCapacityAffected = true;
                                description += "  Blood pumping " + Mathf.Round(inspectedState.bloodPumping * 100) + "%\n";
                            }
                            if (inspectedState.breathing < 1f)
                            {
                                isCapacityAffected = true;
                                description += "  Breathing " + Mathf.Round(inspectedState.breathing * 100) + "%\n";
                            }

                            for (int i = 0; i < inspectedState.movingBP.Count; i++)
                            {
                                if (inspectedState.movingBP[i].efficiency < 1)
                                {
                                    isCapacityAffected = true;
                                    description += "  " + inspectedState.movingBP[i].name + ": " + Mathf.Round(inspectedState.movingBP[i].health) + " / " + inspectedState.movingBP[i].maxHealth + "\n";
                                }
                            }

                            for (int i = 0; i < inspectedState.legSetNames.Count; i++)
                            {
                                string temp = inspectedState.legSet[inspectedState.legSetNames[i]].CapacityAffectingAffliction();

                                if (temp != "")
                                {
                                    isCapacityAffected = true;
                                    description += temp;
                                }
                            }

                            break;
                        }
                    case "Manipulation":
                        {
                            value = inspectedState.manipulation;

                            if (inspectedState.consciousness < 1f)
                            {
                                isCapacityAffected = true;
                                description += "  Consciousness " + Mathf.Round(inspectedState.consciousness * 100) + "%\n";
                            }

                            for (int i = 0; i < inspectedState.manipulationBP.Count; i++)
                            {
                                if (inspectedState.manipulationBP[i].efficiency < 1)
                                {
                                    isCapacityAffected = true;
                                    description += "  " + inspectedState.manipulationBP[i].name + ": " + Mathf.Round(inspectedState.manipulationBP[i].health) + " / " + inspectedState.manipulationBP[i].maxHealth + "\n";
                                }
                            }

                            for (int i = 0; i < inspectedState.armSetNames.Count; i++)
                            {
                                string temp = inspectedState.armSet[inspectedState.armSetNames[i]].CapacityAffectingAffliction();

                                if (temp != "")
                                {
                                    isCapacityAffected = true;
                                    description += temp;
                                }
                            }

                            break;
                        }
                    case "Talking":
                        {
                            value = inspectedState.talking;

                            if (inspectedState.consciousness < 1f)
                            {
                                isCapacityAffected = true;
                                description += "  Consciousness " + Mathf.Round(inspectedState.consciousness * 100) + "%\n";
                            }

                            for (int i = 0; i < inspectedState.talkingBP.Count; i++)
                            {
                                if (inspectedState.talkingBP[i].efficiency < 1)
                                {
                                    isCapacityAffected = true;
                                    description += "  " + inspectedState.talkingBP[i].name + ": " + Mathf.Round(inspectedState.talkingBP[i].health) + " / " + inspectedState.talkingBP[i].maxHealth + "\n";
                                }
                            }

                            break;
                        }
                    case "Eating":
                        {
                            value = inspectedState.eating;

                            if (inspectedState.consciousness < 1f)
                            {
                                isCapacityAffected = true;
                                description += "  Consciousness " + Mathf.Round(inspectedState.consciousness * 100) + "%\n";
                            }

                            for (int i = 0; i < inspectedState.eatingBP.Count; i++)
                            {
                                if (inspectedState.eatingBP[i].efficiency < 1)
                                {
                                    isCapacityAffected = true;
                                    description += "  " + inspectedState.eatingBP[i].name + ": " + Mathf.Round(inspectedState.eatingBP[i].health) + " / " + inspectedState.eatingBP[i].maxHealth + "\n";
                                }
                            }

                            break;
                        }
                    case "Sight":
                        {
                            value = inspectedState.sight;

                            for (int i = 0; i < inspectedState.sightBP.Count; i++)
                            {
                                if (inspectedState.sightBP[i].efficiency < 1)
                                {
                                    isCapacityAffected = true;
                                    description += "  " + inspectedState.sightBP[i].name + ": " + Mathf.Round(inspectedState.sightBP[i].health) + " / " + inspectedState.sightBP[i].maxHealth + "\n";
                                }
                            }

                            break;
                        }
                    case "Hearing":
                        {
                            value = inspectedState.hearing;

                            for (int i = 0; i < inspectedState.hearingBP.Count; i++)
                            {
                                if (inspectedState.hearingBP[i].efficiency < 1)
                                {
                                    isCapacityAffected = true;
                                    description += "  " + inspectedState.hearingBP[i].name + ": " + Mathf.Round(inspectedState.hearingBP[i].health) + " / " + inspectedState.hearingBP[i].maxHealth + "\n";
                                }
                            }

                            break;
                        }
                    case "Breathing":
                        {
                            value = inspectedState.breathing;

                            for (int i = 0; i < inspectedState.breathingBP.Count; i++)
                            {
                                if (inspectedState.breathingBP[i].efficiency < 1)
                                {
                                    isCapacityAffected = true;
                                    description += "  " + inspectedState.breathingBP[i].name + ": " + Mathf.Round(inspectedState.breathingBP[i].health) + " / " + inspectedState.breathingBP[i].maxHealth + "\n";
                                }
                            }

                            break;
                        }
                    case "Blood filtrarion":
                        {
                            value = inspectedState.bloodFiltration;

                            for (int i = 0; i < inspectedState.bloodFiltrationBP.Count; i++)
                            {
                                if (inspectedState.bloodFiltrationBP[i].efficiency < 1)
                                {
                                    isCapacityAffected = true;
                                    description += "  " + inspectedState.bloodFiltrationBP[i].name + ": " + Mathf.Round(inspectedState.bloodFiltrationBP[i].health) + " / " + inspectedState.bloodFiltrationBP[i].maxHealth + "\n";
                                }
                            }

                            break;
                        }
                    case "Blood pumping":
                        {
                            value = inspectedState.bloodPumping;

                            for (int i = 0; i < inspectedState.bloodPumpingBP.Count; i++)
                            {
                                if (inspectedState.bloodPumpingBP[i].efficiency < 1)
                                {
                                    isCapacityAffected = true;
                                    description += "  " + inspectedState.bloodPumpingBP[i].name + ": " + Mathf.Round(inspectedState.bloodPumpingBP[i].health) + " / " + inspectedState.bloodPumpingBP[i].maxHealth + "\n";
                                }
                            }

                            break;
                        }
                    case "Digestion":
                        {
                            value = inspectedState.digestion;

                            for (int i = 0; i < inspectedState.digestionBP.Count; i++)
                            {
                                if (inspectedState.digestionBP[i].efficiency < 1)
                                {
                                    isCapacityAffected = true;
                                    description += "  " + inspectedState.digestionBP[i].name + ": " + Mathf.Round(inspectedState.digestionBP[i].health) + " / " + inspectedState.digestionBP[i].maxHealth + "\n";
                                }
                            }

                            break;
                        }
                }

                description = CapacityAffectingAffliction(description);

                if (!isCapacityAffected)
                {
                    description = "";
                }

                stringValue = value switch
                {
                    > 1 => "Enhanced",
                    1 => "OK",
                    >= 0.4f => "Weakened",
                    >= 0.15f => "Poor",
                    >= 0.01f => "Very Poor",
                    _ => "None",
                };

                if (healthTabInfos.Count > 0)
                {
                    healthTabInfos[0].name.text = capacityValueNamesNames[selectedVertical] + ": ";
                    healthTabInfos[0].nameStatus.text = stringValue;

                    healthTabInfos[0].description.text = description;
                }

                string CapacityAffectingAffliction(string description)
                {
                    foreach (RWAffliction affliction in inspectedState.capacityAffectingAffliction)
                    {
                        if (affliction is RWDisease disease)
                        {
                            if (disease is RWFlu)
                            {
                                if (stringValue != "Consiousness" && stringValue != "Manipulation" && stringValue != "Breathing")
                                {
                                    continue;
                                }

                                isCapacityAffected = true;

                                string text = "  " + disease.name;

                                if (disease.severity < 0.665f)
                                {
                                    description += text + " (minor)\n";
                                }
                                else if (disease.severity < 0.832f)
                                {
                                    description += text + " (major)\n";
                                }
                                else
                                {
                                    description += text + " (extreme)\n";
                                }

                                continue;
                            }
                            else if (disease is RWInfection)
                            {
                                if (stringValue != "Consiousness" && stringValue != "Breathing")
                                {
                                    continue;
                                }

                                if (disease.severity >= 0.77f && disease.severity < 0.86f)
                                {
                                    if (stringValue == "Breathing")
                                    {
                                        continue;
                                    }

                                    isCapacityAffected = true;
                                    description += "  " + disease.name + " (major)\n";
                                }
                                else if (disease.severity > 0.86f)
                                {
                                    isCapacityAffected = true;
                                    description += "  " + disease.name + " (extreme)\n";
                                }

                                continue;
                            }
                            else
                            {
                                Debug.Log(all + affliction + " is not a valid type of disease");
                                RimWorldHealth.Logger.LogError(all + affliction + " is not a valid type of disease");
                                continue;
                            }
                        }
                        else if (affliction is RWInformational informational)
                        {
                            if (informational is RWHypothermia)
                            {
                                if (stringValue != "Consiousness" && stringValue != "Manipulation" && stringValue != "Moving")
                                {
                                    continue;
                                }

                                if (informational.tendQuality < 0.2f)
                                {
                                    if (stringValue == "Moving")
                                    {
                                        continue;
                                    }

                                    isCapacityAffected = true;
                                    description += "  Hypothermia (shivering)\n";
                                }
                                else if (informational.tendQuality < 0.35f)
                                {
                                    if (stringValue == "Moving")
                                    {
                                        continue;
                                    }

                                    isCapacityAffected = true;
                                    description += "  Hypothermia (minor)\n";
                                }
                                else if (informational.tendQuality < 0.62f)
                                {
                                    isCapacityAffected = true;
                                    description += "  Hypothermia (serious)\n";
                                }
                                else
                                {
                                    if (stringValue == "Moving" || stringValue == "Manipulation")
                                    {
                                        continue;
                                    }

                                    isCapacityAffected = true;
                                    description += "  Hypothermia (extreme)\n";
                                }

                                continue;
                            }
                            else if (informational is RWToxicBuildup)
                            {
                                if (stringValue != "Consiousness")
                                {
                                    continue;
                                }

                                isCapacityAffected = true;

                                if (informational.tendQuality < 0.2f)
                                {                           
                                    description += "  Toxic buildup (initial)\n";
                                }
                                else if (informational.tendQuality < 0.4f)
                                {
                                    description += "  Toxic buildup (minor)\n";
                                }
                                else if (informational.tendQuality < 0.6f)
                                {
                                    description += "  Toxic buildup (moderate)\n";
                                }
                                else if (informational.tendQuality < 0.8f)
                                {
                                    description += "  Toxic buildup (serious)\n";
                                }
                                else
                                {
                                    description += "  Toxic buildup (extreme)\n";
                                }

                                continue;
                            }
                        }
                        else
                        {
                            Debug.Log(all + affliction + " is not a valid type of affliction");
                            RimWorldHealth.Logger.LogError(all + affliction + " is not a valid type of affliction");
                            continue;
                        }
                    }

                    return description;
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
                healthTabInfos[0].nameStatus.text = " : " + Mathf.Round(inspectedState.bloodLoss * 100) + "%";

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

                if (inspectedState.bloodLoss >= 1)
                {

                }
                else if (inspectedState.bloodLoss >= 0.6f)
                {
                    healthTabInfos[0].description.text += "\n" +
                        "\n" +
                        "  - Consciousness: -40%";

                    healthTabInfos[0].description.text += "\n" +
                        "  - Consciousness: Max 10%";
                }
                else if (inspectedState.bloodLoss >= 0.45f)
                {
                    healthTabInfos[0].description.text += "\n" +
                        "\n" +
                        "  - Consciousness: -40%";
                }
                else if (inspectedState.bloodLoss >= 0.3f)
                {
                    healthTabInfos[0].description.text += "\n" +
                        "\n" +
                        "  - Consciousness: -20%";
                }
                else if (inspectedState.bloodLoss >= 0.15f)
                {
                    healthTabInfos[0].description.text += "\n" +
                        "\n" +
                        "  - Consciousness: -10%";
                }
            }
            else
            {
                int selectedAffliction = selectedVertical - (healthTabWholeBody.bloodLossVisible ? 1 : 0);

                if (healthTabWholeBody.afflictionVisuals.Count <= selectedAffliction || inspectedState.wholeBodyAfflictions.Count <= selectedAffliction)
                {
                    return;
                }

                healthTabInfos[0].name.text = healthTabWholeBody.afflictionVisuals[selectedAffliction].name.text;

                if (inspectedState.wholeBodyAfflictions[selectedAffliction] is RWDisease disease)
                {
                    healthTabInfos[0].nameStatus.text = ": " + Mathf.Clamp(Mathf.Round(disease.severity * 1000) / 10, 0, 100) + "%";

                    if (disease is RWFlu)
                    {
                        healthTabInfos[0].description.text = "An infectious disease.";

                        if (disease.severity < 0.665f)
                        {
                            healthTabInfos[0].description.text += "\n\n" +
                                "  - Consciousness: -5%\n" +
                                "  - Manipulation: -5%\n" +
                                "  - Breathing: -10%\n";
                        }
                        else if (disease.severity < 0.832f)
                        {
                            healthTabInfos[0].description.text += "\n\n" +
                                "  - Consciousness: -10%\n" +
                                "  - Manipulation: -10%\n" +
                                "  - Breathing: -15%\n";
                        }
                        else
                        {
                            healthTabInfos[0].description.text += "\n\n" +
                                "  - Pain: +5%\n" +
                                "  - Consciousness: -15%\n" +
                                "  - Manipulation: -20%\n" +
                                "  - Breathing: -20%\n";
                        }

                        if (!disease.isTended)
                        {
                            healthTabInfos[0].description.text += "\nNeeds tending now\n";
                        }
                        else
                        {
                            healthTabInfos[0].description.text += "\nTend quality: " + (Mathf.Round(disease.tendQuality * 1000) / 10) + "%\n";

                            if (disease.timeUntilTreatment < 0)
                            {
                                healthTabInfos[0].description.text += "Needs tending now\n"; ;
                            }
                            else
                            {
                                healthTabInfos[0].description.text += "Can be tended in " + GetTimeString(disease.timeUntilTreatment) + "\n";
                            }


                            healthTabInfos[0].description.text += "Tending expires in " + GetTimeString(disease.timeUntilTreatment + 3) + "\n";
                        }
                        healthTabInfos[0].description.text += "Immunity: " + Mathf.Clamp(Mathf.Round(disease.immunity * 1000) / 10, 0, 100) + "%";
                    }
                }
                else if (inspectedState.wholeBodyAfflictions[selectedAffliction] is RWInformational informational)
                {
                    healthTabInfos[0].description.text = "The description was not set, report this is seen";
                    healthTabInfos[0].nameStatus.text = ": the name status has not been set, report this if seen";

                    if (informational is RWAirInLungs)
                    {
                        healthTabInfos[0].description.text = "The amount of oxygen in a creature's lungs.";

                        healthTabInfos[0].nameStatus.text = ": " + Mathf.Round(informational.tendQuality * 100) + "%";
                    }
                    else if (informational is RWHypothermia)
                    {
                        healthTabInfos[0].description.text = "Dangerously low core body temperature.\n" +
                            "Unless re-warmed, hypothermia gets\n" +
                            "worse and ends in death. Recovery is\n" +
                            "quick once the victim is re-warmed.";

                        if (informational.tendQuality < 0.2f)
                        {
                            healthTabInfos[0].description.text += "\n" +
                                "\n" +
                                "  - Consciousness: -5%\n" +
                                "  - Manipulation: -8%\n";
                        }
                        else if (informational.tendQuality < 0.35f)
                        {
                            healthTabInfos[0].description.text += "\n" +
                                "\n" +
                                "  - Consciousness: -10%\n" +
                                "  - Manipulation: -20%\n" +
                                "  - Moving: -10%\n";
                        }
                        else if (informational.tendQuality < 0.62f)
                        {
                            healthTabInfos[0].description.text += "\n" +
                                "\n" +
                                "  - Consciousness: -20%\n" +
                                "  - Manipulation: -50%\n" +
                                "  - Moving: -30%\n" +
                                "  - Pain: +15%\n";
                        }
                        else
                        {
                            healthTabInfos[0].description.text += "\n" +
                                "\n" +
                                "  - Consciousness: Max 10%\n" +
                                "  - Pain: +30%";
                        }

                        healthTabInfos[0].nameStatus.text = ": " + Mathf.Round(informational.tendQuality * 100) + "%";
                    }
                    else if (informational is RWToxicBuildup)
                    {
                        healthTabInfos[0].description.text = "Poison in the bloodstream. This can\n" +
                            "come from various sources, incluidng\n" +
                            "environmental toxins, venomous bites,\n" +
                            "or poisoned weapons.\n" +
                            "\n" +
                            "At high doses, toxic buildup is lethal.\n" +
                            "Even at low doses, it can generate\n" +
                            "cancers.\n" +
                            "\n" +
                            "If a creature dies with toxic buildup,\n" +
                            "there's a chance that they cannot be\n" +
                            "eaten. The higher the toxic buildup, the\n" +
                            "higher the chance.";

                        if (informational.tendQuality < 0.2f)
                        {
                            healthTabInfos[0].description.text += "\n" +
                                "\n" +
                                "  - Consciousness: -5%\n";
                        }
                        else if (informational.tendQuality < 0.4f)
                        {
                            healthTabInfos[0].description.text += "\n" +
                                "\n" +
                                "  - Consciousness: -10%\n";
                        }
                        else if (informational.tendQuality < 0.6f)
                        {
                            healthTabInfos[0].description.text += "\n" +
                                "\n" +
                                "  - Consciousness: -15%\n";
                        }
                        else if (informational.tendQuality < 0.8f)
                        {
                            healthTabInfos[0].description.text += "\n" +
                                "\n" +
                                "  - Consciousness: -25%";
                        }
                        else
                        {
                            healthTabInfos[0].description.text += "\n" +
                                "\n" +
                                "  - Consciousness: Max 10%\n" +
                                "  - Consciousness: -25%";
                        }

                        healthTabInfos[0].nameStatus.text = ": " + Mathf.Round(informational.tendQuality * 100) + "%";
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
            healthTabInfos[0].nameStatus.text = " : " + Mathf.Round(healthTabBodyParts[selectedBodyPart].bodyPart.health) + " / " + healthTabBodyParts[selectedBodyPart].bodyPart.maxHealth;

            float efficiency = Mathf.Round(healthTabBodyParts[selectedBodyPart].bodyPart.efficiency * 100);

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

                    if (affliction is RWInjury combinedInjury)
                    {
                        healthTabInfos[j].name.text = combinedInjury.healingDifficulty.name + (combinedInjury.attackName != "" ? " (" + combinedInjury.attackName + ")" : "");

                        healthTabInfos[j].nameStatus.text = " : " + (Mathf.Round(combinedInjury.damage * 10) / 10).ToString();

                        string description = "";

                        if (!inspectedCreatureState.dead)
                        {
                            float bleeding = Mathf.Max(1f, Mathf.Round(combinedInjury.healingDifficulty.bleeding * combinedInjury.damage * inspectedState.bodySizeFactor * RWHealthState.BloodLossMultiplier(healthTabBodyParts[selectedBodyPart].bodyPart)));

                            if (combinedInjury.isBleeding)
                            {
                                description += "  - Bleeding: ";
                                description += bleeding.ToString();
                                description += "%/c";
                            }

                            float pain = Mathf.Round(combinedInjury.damage * combinedInjury.healingDifficulty.pain / inspectedState.bodySizeFactor);

                            if (combinedInjury.healingDifficulty.pain > 0 && pain > 0f)
                            {
                                if (description != "")
                                {
                                    description += "\n";
                                }

                                description += "  - Pain: +";
                                description += pain.ToString();
                                description += "%";
                            }

                            if (!combinedInjury.isTended)
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
                                    description += combinedInjury.healingDifficulty.solidTreated;
                                }
                                else if (healthTabBodyParts[selectedBodyPart].bodyPart.isInternal)
                                {
                                    description += combinedInjury.healingDifficulty.innerTreated;
                                }
                                else
                                {
                                    description += combinedInjury.healingDifficulty.treated;
                                }

                                description += " (quality " + (Mathf.Round(combinedInjury.tendQuality * 1000) / 10) + "%)";
                            }
                        }

                        healthTabInfos[j].description.text = description;
                    }
                }

                return;
            }

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

                    string description = "A body part is entirely missing.";

                    if (!inspectedCreatureState.dead && !injury.isTended)
                    {
                        description += "\n\n";

                        float bleeding = Mathf.Max(1f, Mathf.Round(injury.healingDifficulty.bleeding * injury.part.maxHealth * 2 * inspectedState.bodySizeFactor * RWHealthState.BloodLossMultiplier(healthTabBodyParts[selectedBodyPart].bodyPart)));

                        if (injury.isBleeding)
                        {
                            description += "  - Bleeding: " + bleeding.ToString() + "%/c";
                        }

                        float pain = Mathf.Round(injury.part.maxHealth * 2 * injury.healingDifficulty.pain / inspectedState.bodySizeFactor);

                        if (injury.healingDifficulty.pain > 0 && pain > 0f)
                        {
                            if (description != "")
                            {
                                description += "\n";
                            }

                            description += "  - Pain: +" + pain.ToString() + "%";
                        }

                        if (description != "")
                        {
                            description += "\n\n";
                        }

                        description += "Needs tending now";
                    }

                    healthTabInfos[1].description.text = description;
                }
                else if (injury is RWScar scar && scar.isRevealed)
                {
                    healthTabInfos[1].name.text = ScarName(scar);

                    healthTabInfos[1].nameStatus.text = " : " + (Mathf.Round(scar.damage * 10) / 10).ToString();

                    string description = "";

                    if (!inspectedCreatureState.dead && scar.painCategory != "")
                    {
                        description = "  - Pain: +";

                        if (scar.painCategory == "painful")
                        {
                            description += Mathf.Round(scar.scarDamage * 1.5f * injury.healingDifficulty.scarPain / inspectedState.bodySizeFactor).ToString();
                        }
                        else if (scar.painCategory == "aching")
                        {
                            description += Mathf.Round(scar.scarDamage * injury.healingDifficulty.scarPain / inspectedState.bodySizeFactor).ToString();
                        }
                        else if (scar.painCategory == "itchy")
                        {
                            description += Mathf.Round(scar.scarDamage * 0.5f * injury.healingDifficulty.scarPain / inspectedState.bodySizeFactor).ToString();
                        }

                        description += "%";
                    }

                    healthTabInfos[1].description.text = description;
                }
                else
                {
                    healthTabInfos[1].name.text = injury.healingDifficulty.name + (injury.attackName != "" ? " (" + injury.attackName + ")" : "");
                    healthTabInfos[1].nameStatus.text = " : " + (Mathf.Round(injury.damage * 10) / 10).ToString();

                    string description = "";

                    if (!inspectedCreatureState.dead)
                    {
                        float bleeding = Mathf.Max(1f, Mathf.Round(injury.healingDifficulty.bleeding * injury.damage * inspectedState.bodySizeFactor * RWHealthState.BloodLossMultiplier(healthTabBodyParts[selectedBodyPart].bodyPart)));

                        if (injury.isBleeding)
                        {
                            description += "  - Bleeding: " + bleeding.ToString() + "%/c";
                        }

                        float pain = Mathf.Round(injury.damage * injury.healingDifficulty.pain / inspectedState.bodySizeFactor);

                        if (injury.healingDifficulty.pain > 0 && pain > 0f)
                        {
                            if (description != "")
                            {
                                description += "\n";
                            }

                            description += "  - Pain: +" + pain.ToString() + "%";
                        }

                        if (!injury.isTended)
                        {
                            if (description != "")
                            {
                                description += "\n\n";
                            }

                            description += "Needs tending now";
                        }
                        else
                        {
                            if (description != "")
                            {
                                description += "\n\n";
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

                            description += " (quality " + (Mathf.Round(injury.tendQuality * 1000) / 10) + "%)";
                        }
                    }

                    healthTabInfos[1].description.text = description;
                }
            }
            else if (affliction is RWDisease disease)
            {
                healthTabInfos[1].nameStatus.text = " : " + (Mathf.Round(disease.severity * 1000) / 10) + "%";

                if (disease is RWInfection)
                {
                    healthTabInfos[1].description.text = "Bacterial infection in a wound. Without\n" +
                        "treatment, the bacteria will multiply,\n" +
                        "killing local tissue, and eventually\n" +
                        "causing blood poisoning and death.";

                    if (disease.severity < 0.32f)
                    {
                        healthTabInfos[1].description.text += "\n\n" +
                            "  - Pain: +5%\n";
                    }
                    else if (disease.severity < 0.77f)
                    {
                        healthTabInfos[1].description.text += "\n\n" +
                            "  - Pain: +8%\n";
                    }
                    else if (disease.severity < 0.86f)
                    {
                        healthTabInfos[1].description.text += "\n\n" +
                            "  - Pain: +12%\n" +
                            "  - Consciousness: -5%\n";
                    }
                    else
                    {
                        healthTabInfos[1].description.text += "\n\n" +
                            "  - Pain: +12%\n" +
                            "  - Consciousness: -5%\n" +
                            "  - Breathing: -5%";
                    }

                    if (!disease.isTended)
                    {
                        healthTabInfos[1].description.text += "\nNeeds tending now\n";
                    }
                    else
                    {
                        healthTabInfos[1].description.text += "\nTend quality: " + (Mathf.Round(disease.tendQuality * 1000) / 10) + "%\n";

                        if (disease.timeUntilTreatment < 0)
                        {
                            healthTabInfos[1].description.text += "Needs tending now\n"; ;
                        }
                        else
                        {
                            healthTabInfos[1].description.text += "Can be tended in " + GetTimeString(disease.timeUntilTreatment) + "\n";
                        }


                        healthTabInfos[1].description.text += "Tending expires in " + GetTimeString(disease.timeUntilTreatment + 3) + "\n";
                    }
                    healthTabInfos[1].description.text += "Immunity: " + Mathf.Clamp((Mathf.Round(disease.immunity * 1000) / 10), 0, 0) + "%";
                }
            }  
        }

        string GetTimeString(float timeInMin)
        {
            string text;
            if (timeInMin < 1)
            {
                text = Mathf.Max(Mathf.Round(timeInMin * 10000) / 100, 0) + (Mathf.Round(timeInMin * 10000) / 100 == 1 ? " second" : " seconds");
            }
            else
            {
                text = (Mathf.Round(timeInMin * 10) / 10) + (Mathf.Round(timeInMin * 10) / 10 == 1 ? " minute" : " minutes");
            }

            return text;
        }
    }

    public override void ClearSprites()
    {
        base.ClearSprites();

        healthTabWholeBody.ClearSprites();

        foreach (HealthTabBodyPart bodyPartTab in healthTabBodyParts)
        {
            bodyPartTab.ClearSprites();
        }

        foreach (HealthTabInfo infoTab in healthTabInfos)
        {
            infoTab.ClearSprites();
        }

        capacityName.RemoveFromContainer();

        foreach (FLabel capacityName in capacityValueNames)
        {
            capacityName.RemoveFromContainer();
        }

        foreach (FLabel capacityValue in capacityValues)
        {
            capacityValue.RemoveFromContainer();
        }

        foreach (FSprite sprite in sprites)
        {
            sprite.RemoveFromContainer();
        }

        selectedSprite.RemoveFromContainer();
        treatedSprite.RemoveFromContainer();

        afflictionsAboveLabel.RemoveFromContainer();
        afflictionsBelowLabel.RemoveFromContainer();
    }

    void TriggeredOn()
    {
        selected = false;

        selectedHorizontal = 0;
        selectedVertical = -1;

        scrollPivot = 0;

        topLimit = 510;

        horizontalOnce = true;
        verticalOnce = true;

        selectedTimer = selectedTimerMax;

        if (playerState != null)
            playerState.tendAffliction = null;
    }
    public void TriggeredOff()
    {
        selected = false;

        selectedHorizontal = 0;
        selectedVertical = 0;

        scrollPivot = 0;

        topLimit = 510;

        selectedTimer = selectedTimerMax;
    }

    public string ScarName(RWScar scar)
    {
        return (scar.isPermanent && scar.healingDifficulty.permanentScar != "" ? scar.healingDifficulty.permanentScar : scar.healingDifficulty.scar) + ((scar.attackName != "" || scar.painCategory != "") ? " (" + (scar.attackName != "" ? scar.attackName + (scar.painCategory != "" ? ", " + scar.painCategory : "") : scar.painCategory) + ")" : "");
    }

    #region Values
    public RWState playerState;
    public AbstractCreature player;

    public RWState inspectedState;
    private CreatureState inspectedCreatureState;

    private readonly FLabel capacityName;

    private readonly FLabel bloodLossPerCycle;

    private readonly FLabel afflictionsAboveLabel;
    private readonly FLabel afflictionsBelowLabel;
    public int afflictionsAbove = 0;
    public int afflictionsBelow = 0;

    private readonly List<FLabel> capacityValueNames = new();
    private readonly List<FLabel> capacityValues = new();
    private readonly FSprite[] sprites;

    public readonly FSprite selectedSprite;

    private readonly FSprite treatedSprite;

    private List<string> capacityValueNamesNames;

    public bool visible = false;

    public readonly HealthTabWholeBody healthTabWholeBody;

    public readonly List<HealthTabBodyPart> healthTabBodyParts = new();

    public readonly List<HealthTabInfo> healthTabInfos = new();

    public bool selected = false;
    public float selectedTimer = 5;
    private readonly float selectedTimerMax = 5;

    public int selectedHorizontal = 0;
    public int selectedVertical = -1;

    public int scrollPivot = 0; //if this number is positive it means the pivot is the top affliction, if this number is negative it means that the pivot is the bottom affliction

    public float topLimit = 510;
    public readonly float bottomLimit = 280;

    public float pivotOffset = 0;

    private bool horizontalOnce = true;
    private bool verticalOnce = true;

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

    public Vector2 DrawPos(bool usePivot = true)
    {
        Vector2 pos = owner.DrawPos();

        pos.y += 120;

        if (owner.healthTabWholeBody.active)
        {
            pos.y -= 8f + ((owner.healthTabWholeBody.afflictionNumber * 9f) + ((owner.healthTabWholeBody.afflictionNumber - 1) * 9f) + (owner.healthTabWholeBody.extraHeight * 7.5f) + (owner.healthTabWholeBody.extraHeight * 7.5f));
        }

        for (int i = 0; i < owner.healthTabBodyParts.IndexOf(this); i++)
        {
            pos.y -= 8f + ((owner.healthTabBodyParts[i].afflictionNumber * 9f) + ((owner.healthTabBodyParts[i].afflictionNumber - 1) * 9f) + (owner.healthTabBodyParts[i].extraHeight * 7.5f) + (owner.healthTabBodyParts[i].extraHeight * 7.5f));
        }

        if (usePivot)
            pos.y += Mathf.RoundToInt(owner.pivotOffset);

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
            if (bodyPart.afflictions[i] is RWInjury injury && injury is not RWDestroyed && injury.healingDifficulty.combines && (injury is not RWScar scar || !scar.isRevealed))
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
        background.isVisible = owner.visible && owner.healthTabWholeBody.active ? owner.healthTabBodyParts.IndexOf(this) % 2 != 0 : owner.healthTabBodyParts.IndexOf(this) % 2 == 0;
        bodyPartName.isVisible = owner.visible;

        if (owner == null || !owner.visible)
        {
            return;
        }

        if (background.isVisible)
        {
            background.x = DrawPos().x;
            background.y = DrawPos().y + 3;
            background.scaleX = 340;
            background.scaleY = 0;
            background.color = Color.black;
        }

        bodyPartName.x = DrawPos().x - 165;
        bodyPartName.y = DrawPos().y;
        bodyPartName.color = Color.white;

        List<string> usedCombinedAfflictions = new();
        List<RWAffliction> usedAfflictions = new();

        allAfflictions = new();
        allAfflictionsHeight = new();

        extraHeight = 0;

        isVisible = false;

        topVisible = false;
        bottomReached = false;

        for (int i = 0; i < afflictionVisuals.Count; i++)
        {
            afflictionVisuals[i].name.x = DrawPos().x - 45;
            afflictionVisuals[i].name.y = DrawPos().y - 18f * i;
            afflictionVisuals[i].name.color = Color.white;

            afflictionVisuals[i].icon.x = DrawPos().x + 160;
            afflictionVisuals[i].icon.y = DrawPos().y - 7 - 18f * i;
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

                        allAfflictionsHeight.Add(new(2) { Mathf.RoundToInt(afflictionVisuals[i].name.textRect.height / afflictionVisuals[i].name.FontLineHeight), extraHeight });

                        IsVisible(i);

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

                        if (injury is RWDestroyed destroyed)
                        {
                            text = (bodyPart.isInternal ? bodyPart.isSolid ? "Shattered" : destroyed.healingDifficulty.destroyedOut : destroyed.healingDifficulty.destroyed) + (destroyed.isFresh ? " (fresh)" : "");
                        }
                        else if (injury is RWScar scar && scar.isRevealed)
                        {
                            text = owner.ScarName(scar);
                        }
                        else
                        {
                            text = injury.healingDifficulty.name + (injury.attackName != "" ? " (" + injury.attackName + ") " : "");
                        }

                        afflictionVisuals[i].name.text = text;
                        afflictionVisuals[i].name.color = Color.white;

                        Menu.MenuLabel.WordWrapLabel(afflictionVisuals[i].name, wordWrap);
                        allAfflictionsHeight.Add(new(2) { Mathf.RoundToInt(afflictionVisuals[i].name.textRect.height / afflictionVisuals[i].name.FontLineHeight), extraHeight });

                        IsVisible(i);

                        afflictionVisuals[i].icon.color = injury.isTended ? Color.white : injury.isBleeding ? Color.red : Color.blue;
                    }
                    else if (bodyPart.afflictions[j] is RWDisease disease)
                    {
                        string text = disease.name;

                        if (disease is RWInfection && disease.severity > 0)
                        {
                            if (disease.severity < 0.32f)
                            {
                                text += " (minor)";
                            }
                            else if (disease.severity < 0.77f)
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

                        allAfflictionsHeight.Add(new(2) { Mathf.RoundToInt(afflictionVisuals[i].name.textRect.height / afflictionVisuals[i].name.FontLineHeight), extraHeight });

                        IsVisible(i);

                        afflictionVisuals[i].icon.color = disease.isTended ? Color.white : Color.grey;
                    }

                    allAfflictions.Add(bodyPart.afflictions[j]);

                    break;
                }
            }
        }

        if (!isVisible)
        {
            bodyPartName.isVisible = false;

            background.isVisible = false;
        }
        else
        {
            bodyPartName.isVisible = true;
        }

        void IsVisible(int i)
        {
            afflictionVisuals[i].name.y -= 15 * extraHeight;
            afflictionVisuals[i].icon.y -= 15 * extraHeight;

            if (afflictionVisuals[i].name.y > owner.topLimit)
            {
                afflictionVisuals[i].name.isVisible = false;
                afflictionVisuals[i].icon.isVisible = false;

                owner.afflictionsAbove++;

                background.y -= 18;
            }
            else if (afflictionVisuals[i].name.y - afflictionVisuals[i].name.textRect.height < owner.bottomLimit)
            {
                afflictionVisuals[i].name.isVisible = false;
                afflictionVisuals[i].icon.isVisible = false;

                owner.afflictionsBelow++;

                if (!bottomReached)
                {
                    bottomReached = true;
                }
            }
            else
            {
                afflictionVisuals[i].name.isVisible = true;
                afflictionVisuals[i].icon.isVisible = true;

                isVisible = true;

                background.y -= 9;
                background.scaleY += 17f + (topVisible ? 1 : 0);

                if (!topVisible)
                {
                    topVisible = true;

                    bodyPartName.y = afflictionVisuals[i].name.y;
                }
            }

            int height = Mathf.RoundToInt(afflictionVisuals[i].name.textRect.height / afflictionVisuals[i].name.FontLineHeight);

            extraHeight += height - 1;

            if (bottomReached)
            {
                return;
            }

            if (topVisible)
            {
                background.y -= 7.5f * (height - 1);

                background.scaleY += 15 * (height - 1);
            }
            else
            {
                background.y -= 15f * (height - 1);
            }
        }
    }

    public void ClearSprites()
    {
        background.RemoveFromContainer();

        bodyPartName.RemoveFromContainer();

        foreach (HealthTabAffliction afflictionVisual in afflictionVisuals)
        {
            afflictionVisual.ClearSprites();
        }

        afflictionVisuals.Clear();

        combinedAfflictions.Clear();
        afflictions.Clear();

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

    public float GetPivotOffset(int afflictionNumber, bool top)
    {
        float offset = 0;

        List<string> usedCombinedAfflictions = new();
        List<RWAffliction> usedAfflictions = new();

        int totalHeight = 0;
        int extraHeight = 0;

        for (int i = 0; i < afflictionNumber + 1; i++)
        {
            if (i == afflictionNumber)
                offset = DrawPos(false).y - 18f * i;

            for (int j = 0; j < bodyPart.afflictions.Count; j++)
            {
                if (!usedCombinedAfflictions.Contains(CombinedAfflictionName(bodyPart, j)) && combinedAfflictions.TryGetValue(CombinedAfflictionName(bodyPart, j), out _))
                {
                    usedCombinedAfflictions.Add(CombinedAfflictionName(bodyPart, j));

                    HeightChanges(i, Mathf.RoundToInt(afflictionVisuals[i].name.textRect.height / afflictionVisuals[i].name.FontLineHeight));
                    
                    break;
                }
                else if (!usedAfflictions.Contains(bodyPart.afflictions[j]) && afflictions.Contains(bodyPart.afflictions[j]))
                {
                    usedAfflictions.Add(bodyPart.afflictions[j]);

                    HeightChanges(i, Mathf.RoundToInt(afflictionVisuals[i].name.textRect.height / afflictionVisuals[i].name.FontLineHeight));             

                    break;
                }
            }
        }

        if (top)
        {
            offset = owner.topLimit - offset - 5;
        }
        else
        {
            offset = owner.bottomLimit - offset;
        }


        return offset;

        void HeightChanges(int i, int height)
        {
            if (i == afflictionNumber)
            {
                offset -= 15 * extraHeight;

                if (!top)
                {
                    offset -= 15 * height;
                }
            }

            totalHeight += height;
            extraHeight += height - 1;
        }
    }

    #region Values
    private readonly HealthTab owner;

    public RWBodyPart bodyPart;

    public FSprite background;

    public FLabel bodyPartName;

    public List<HealthTabAffliction> afflictionVisuals = new();

    public int afflictionNumber = 1;
    public int extraHeight = 0;

    public Dictionary<string, List<RWAffliction>> combinedAfflictions;
    public List<RWAffliction> afflictions;

    public List<RWAffliction> allAfflictions;
    public List<List<int>> allAfflictionsHeight;

    //public int partPriority;

    public bool slatedForDeletion = false;

    const float wordWrap = 200;

    public bool isVisible = true;

    public bool topVisible = false;
    public bool bottomReached = false;
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

    public Vector2 DrawPos(bool usePivot = true)
    {
        Vector2 pos = owner.DrawPos();

        pos.y += 120;

        if (usePivot)
            pos.y += Mathf.RoundToInt(owner.pivotOffset);

        return pos;
    }

    public void Update()
    {
        bloodLossVisible = owner.inspectedState != null && owner.inspectedState.bloodLoss >= 0.15f;

        afflictionNumber = (bloodLossVisible ? 1 : 0) + (owner.inspectedState != null ? owner.inspectedState.wholeBodyAfflictions.Count : 0);

        active = afflictionNumber > 0;

        if (afflictionNumber - (bloodLossVisible ? 1 : 0) <= afflictionVisuals.Count)
        {
            List<HealthTabAffliction> list = new(afflictionVisuals);

            for (int i = list.Count - 1; i >= afflictionNumber - (bloodLossVisible ? 1 : 0); i--)
            {
                afflictionVisuals[i].ClearSprites();

                afflictionVisuals.Remove(list[i]);
            }
        }
        else if (afflictionNumber - (bloodLossVisible ? 1 : 0) > afflictionVisuals.Count)
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

        if (owner == null || owner.inspectedState == null || !owner.visible || !active)
        {
            for (int i = 0; i < afflictionVisuals.Count; i++)
            {
                afflictionVisuals[i].ClearSprites();
            }

            afflictionVisuals.Clear();
            afflictionsHeight.Clear();

            return;
        }

        if (background.isVisible)
        {
            background.x = DrawPos().x;
            background.y = DrawPos().y + 3;
            background.scaleX = 340;
            background.scaleY = 0;
            background.color = Color.black;
        }

        name.x = DrawPos().x - 165;
        name.y = DrawPos().y;

        isVisible = false;

        topVisible = false;
        bottomReached = false;

        if (bloodLossVisible)
        {
            bloodLossName.x = DrawPos().x - 45;
            bloodLossName.y = DrawPos().y;

            if (owner.inspectedState.bloodLoss >= 0.6f)
            {
                bloodLossName.text = "Blood loss (extreme)";
            }
            else if (owner.inspectedState.bloodLoss >= 0.45f)
            {
                bloodLossName.text = "Blood loss (severe)";
            }
            else if (owner.inspectedState.bloodLoss >= 0.3f)
            {
                bloodLossName.text = "Blood loss (moderate)";
            }
            else if (owner.inspectedState.bloodLoss >= 0.15f)
            {
                bloodLossName.text = "Blood loss (minor)";
            }
            else
            {
                bloodLossName.text = "Blood loss ()";
            }

            bloodLossName.color = Color.white;

            if (bloodLossName.y > owner.topLimit)
            {
                bloodLossName.isVisible = false;

                owner.afflictionsAbove++;

                background.y -= 18f;
            }
            else
            {
                isVisible = true;

                topVisible = true;

                background.y -= 9;
                background.scaleY += 17f;
            }
        }

        for (int i = 0; i < afflictionVisuals.Count && i < owner.inspectedState.wholeBodyAfflictions.Count; i++)
        {
            afflictionVisuals[i].name.x = DrawPos().x - 45;
            afflictionVisuals[i].name.y = DrawPos().y - ((bloodLossVisible ? 18f : 0) + (18f * i));
            afflictionVisuals[i].name.color = Color.yellow;

            afflictionVisuals[i].icon.x = DrawPos().x + 160;
            afflictionVisuals[i].icon.y = DrawPos().y - 7 - ((bloodLossVisible ? 18f : 0) + (18f * i));
            afflictionVisuals[i].icon.scale = 14;

            if (owner.inspectedState.wholeBodyAfflictions[i] is RWDisease disease)
            {
                string text = disease.name;

                if (disease is RWFlu && disease.severity > 0)
                {
                    if (disease.severity < 0.665f)
                    {
                        text += " (minor)";
                    }
                    else if (disease.severity < 0.832f)
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

                afflictionsHeight.Add(new(2) { Mathf.RoundToInt(afflictionVisuals[i].name.textRect.height / afflictionVisuals[i].name.FontLineHeight), extraHeight });

                IsVisible(i);

                afflictionVisuals[i].icon.color = disease.isTended ? Color.white : Color.grey;
            }
            else if (owner.inspectedState.wholeBodyAfflictions[i] is RWInformational informational)
            {
                string text = "null";

                if (informational is RWAirInLungs)
                {
                    text = "Lung capacity";

                    if (informational.tendQuality >= 0.7f)
                    {
                        text += " (high)";
                    }
                    else if (informational.tendQuality >= 0.4f)
                    {
                        text += " (medium)";
                    }
                    else if (informational.tendQuality > 0f)
                    {
                        text += " (low)";
                    }
                    else
                    {
                        text += " (empty)";
                    }
                }
                else if (informational is RWHypothermia)
                {
                    text = "Hypothermia";

                    if (informational.tendQuality >= 0.62f)
                    {
                        text += " (extreme)";
                    }
                    else if (informational.tendQuality >= 0.35f)
                    {
                        text += " (serious)";
                    }
                    else if (informational.tendQuality >= 0.2f)
                    {
                        text += " (minor)";
                    }
                    else
                    {
                        text += " (shivering)";
                    }
                }
                else if (informational is RWToxicBuildup)
                {
                    text = "Toxic buildup";

                    if (informational.tendQuality >= 0.8f)
                    {
                        text += " (extreme)";
                    }
                    else if (informational.tendQuality >= 0.6f)
                    {
                        text += " (serious)";
                    }
                    else if (informational.tendQuality >= 0.4f)
                    {
                        text += " (moderate)";
                    }
                    else if (informational.tendQuality >= 0.2f)
                    {
                        text += " (minor)";
                    }
                    else
                    {
                        text += " (initial)";
                    }
                }

                afflictionVisuals[i].name.text = text;

                Menu.MenuLabel.WordWrapLabel(afflictionVisuals[i].name, wordWrap);

                afflictionsHeight.Add(new(2) { Mathf.RoundToInt(afflictionVisuals[i].name.textRect.height / afflictionVisuals[i].name.FontLineHeight), extraHeight });

                IsVisible(i);

                afflictionVisuals[i].icon.isVisible = false;
            }
        }

        if (!isVisible)
        {
            name.isVisible = false;

            background.isVisible = false;
        }
        else
        {
            name.isVisible = true;
        }

        void IsVisible(int i)
        {
            afflictionVisuals[i].name.y -= 15 * extraHeight;
            afflictionVisuals[i].icon.y -= 15 * extraHeight;

            if (afflictionVisuals[i].name.y > owner.topLimit)
            {
                afflictionVisuals[i].name.isVisible = false;
                afflictionVisuals[i].icon.isVisible = false;

                owner.afflictionsAbove++;

                background.y -= 18;
            }
            else if (afflictionVisuals[i].name.y - afflictionVisuals[i].name.textRect.height < owner.bottomLimit)
            {
                afflictionVisuals[i].name.isVisible = false;
                afflictionVisuals[i].icon.isVisible = false;

                owner.afflictionsBelow++;

                if (!bottomReached)
                {
                    bottomReached = true;
                }
            }
            else
            {
                afflictionVisuals[i].name.isVisible = true;
                afflictionVisuals[i].icon.isVisible = true;

                isVisible = true;

                background.y -= 9;
                background.scaleY += 17f + (topVisible ? 1 : 0);

                if (!topVisible)
                {
                    topVisible = true;

                    name.y = afflictionVisuals[i].name.y;
                }
            }

            int height = Mathf.RoundToInt(afflictionVisuals[i].name.textRect.height / afflictionVisuals[i].name.FontLineHeight);

            extraHeight += height - 1;

            if (bottomReached)
            {
                return;
            }

            if (topVisible)
            {
                background.y -= 7.5f * (height - 1);

                background.scaleY += 15 * (height - 1);
            }
            else
            {
                background.y -= 15f * (height - 1);
            }
        }
    }

    public void ClearSprites()
    {
        background.RemoveFromContainer();

        name.RemoveFromContainer();

        bloodLossName.RemoveFromContainer();

        foreach (HealthTabAffliction afflictionVisual in afflictionVisuals)
        {
            afflictionVisual.ClearSprites();
        }

        afflictionVisuals.Clear();
        afflictionsHeight.Clear();
    }

    public float GetPivotOffset(int afflictionNumber, bool top)
    {
        float offset = DrawPos(false).y;

        int totalHeight = 0;
        int extraHeight = 0;

        if (bloodLossVisible)
        {
            offset -= 17;

            afflictionNumber -= 1;
        }

        for (int i = 0; i < afflictionNumber + 1; i++)
        {
            if (i == afflictionNumber)
                offset -= 18f * i;

            HeightChanges(i, Mathf.RoundToInt(afflictionVisuals[i].name.textRect.height / afflictionVisuals[i].name.FontLineHeight));
        }

        if (top)
        {
            offset = owner.topLimit - offset - 5;
        }
        else
        {
            offset = owner.bottomLimit - offset;
        }


        return offset;

        void HeightChanges(int i, int height)
        {
            if (i == afflictionNumber)
            {
                offset -= 15 * extraHeight;

                if (!top)
                {
                    offset -= 15 * height;
                }
            }

            totalHeight += height;
            extraHeight += height - 1;
        }
    }

    #region Values
    private readonly HealthTab owner;

    public FSprite background;

    private readonly FLabel name;

    public FLabel bloodLossName;

    public List<HealthTabAffliction> afflictionVisuals = new();

    public List<List<int>> afflictionsHeight = new();

    public int afflictionNumber = 0;
    public int extraHeight = 0;

    public bool bloodLossVisible = false;

    public bool active = false;

    const float wordWrap = 200;

    public bool isVisible = true;

    private bool topVisible = false;
    private bool bottomReached = false;
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

        if (owner.healthTabInfos.IndexOf(this) != 0 || owner.healthTabInfos.IndexOf(this) == 0 && owner.selectedHorizontal == 1 && owner.healthTabWholeBody.active && owner.selectedVertical < owner.healthTabWholeBody.afflictionNumber)
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

        if (description.text != "")
        {
            if (owner.healthTabInfos.IndexOf(this) == 0)
            {
                if (owner.selectedHorizontal == 1 && owner.healthTabWholeBody.active && owner.selectedVertical < owner.healthTabWholeBody.afflictionNumber)
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
        foreach (FSprite background in backgrounds)
        {
            background.RemoveFromContainer();
        }

        name.RemoveFromContainer();
        nameStatus.RemoveFromContainer();

        description.RemoveFromContainer();
    }

    #region Values
    private readonly HealthTab owner;

    private readonly FSprite[] backgrounds;

    public FLabel name;
    public FLabel nameStatus;

    public FLabel description;

    public bool slatedForDeletion = false;
    #endregion
}