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

    public Vector2 DrawPos(float timeStacker)
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
            if (!horizontalOnce && healthTabBodyParts.Count > 0)
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
            }
        }
        else
        {
            horizontalOnce = false;
        }

        if (healthTabBodyParts.Count <= 0)
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
            }
        }
        else
        {
            verticalOnce = false;
        }

        if (selectedVertical != -1)
        {
            int maxVertical = selectedHorizontal == 0 ? capacityValueNamesNames.Count : -1;

            if (maxVertical == -1)
            {
                maxVertical = healthTabWholeBody.active ? healthTabWholeBody.afflictionNumber : 0;

                for (int i = 0; i < healthTabBodyParts.Count; i++)
                {
                    maxVertical += healthTabBodyParts[i].combinedAfflictions.Count;
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

        if (treating)
        {
            treatTime--;

            if (treatedAffliction == null)
            {
                treating = false;
            }
            else if (treatTime <= 0)
            {
                treatedAffliction.isTended = true;
                treating = false;

                if (treatedAffliction is RWInjury injury)
                {
                    injury.isBleeding = false;
                }

                Debug.Log("Treated Affliction " + treatedAffliction + " has been treated");

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
                        else if (state.bodyParts[i].afflictions[j] is RWDisease disease)
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
                }
            }

            if (destroyedAffliction == null && bleeding == null)
            {
                for (int i = 0; i < state.wholeBodyAfflictions.Count; i++)
                {
                    if (state.wholeBodyAfflictions[i] is RWDisease disease)
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

        healthTabWholeBody.Draw(timeStacker);

        for (int i = healthTabBodyParts.Count - 1; i >= 0; i--)
        {
            healthTabBodyParts[i].Draw(timeStacker);
        }
        for (int i = healthTabInfos.Count - 1; i >= 0; i--)
        {
            healthTabInfos[i].Draw(timeStacker);
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

        sprites[0].x = DrawPos(timeStacker).x;
        sprites[0].y = DrawPos(timeStacker).y;
        sprites[0].scaleX = 350;
        sprites[0].scaleY = 250;
        sprites[0].color = Color.gray;

        sprites[1].x = DrawPos(timeStacker).x - 250;
        sprites[1].y = DrawPos(timeStacker).y;
        sprites[1].scaleX = 150;
        sprites[1].scaleY = 250;
        sprites[1].color = Color.black;

        capacityName.color = Color.white;
        capacityName.x = DrawPos(timeStacker).x - 320;
        capacityName.y = DrawPos(timeStacker).y + 125;
        capacityName.text = state.creature.ToString();

        if (bloodLossPerCycle.isVisible)
        {
            float bloodLoss = (state.cycleLength * (1 - state.bloodLoss)) / (state.bloodLossPerCycle / 100);

            string bloodLossTime = bloodLoss < 0 ? Mathf.Floor(bloodLoss * 10000) / 100 + (Mathf.Floor(bloodLoss * 10000) / 100 == 1 ? " second" : " seconds") : bloodLoss > 60 ? Mathf.Floor(bloodLoss / 60 * 10) / 10 + (Mathf.Floor(bloodLoss / 60 * 10) / 10 == 1 ? " hour" : " hours") : Mathf.Floor(bloodLoss * 10) / 10 + (Mathf.Floor(bloodLoss * 10) / 10 == 1 ? " minute" : " minutes");

            bloodLossPerCycle.color = Color.white;
            bloodLossPerCycle.x = DrawPos(timeStacker).x - 165;
            bloodLossPerCycle.y = DrawPos(timeStacker).y - 105;
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
            capacityValueNames[i].x = DrawPos(timeStacker).x - 320;
            capacityValueNames[i].y = DrawPos(timeStacker).y + 100 - (17 * i);

            capacityValueNames[i].text = capacityValueNamesNames[i];

            capacityValues[i].x = DrawPos(timeStacker).x - 180;
            capacityValues[i].y = DrawPos(timeStacker).y + 100 - (17 * i);

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
            treatedSprite.alpha = 0.6f;
            treatedSprite.color = Color.magenta;

            treatedSprite.scaleY = 20;

            Debug.Log("treatTimeMax " + treatTimeMax);
            Debug.Log("treatTime " + treatTime);

            Debug.Log(1 - treatTime/ treatTimeMax);

            treatedSprite.scaleX = Mathf.Lerp(0, 200, 1 - treatTime / treatTimeMax);

            treatedSprite.x = Mathf.Lerp(DrawPos(timeStacker).x, DrawPos(timeStacker).x + 70, 1 - treatTime / treatTimeMax);

            for (int i = 0; i < healthTabBodyParts.Count; i++)
            {
                if (healthTabBodyParts[i].bodyPart == treatedAffliction.part)
                {
                    for (int j = 0; j < healthTabBodyParts[i].combinedAfflictions.Count; j++)
                    {
                        if (healthTabBodyParts[i].combinedAfflictions.TryGetValue(healthTabBodyParts[i].CombinedAfflictionName(healthTabBodyParts[i].bodyPart, j), out List<RWAffliction> list) && list.Contains(treatedAffliction))
                        {
                            treatedSprite.y = healthTabBodyParts[i].DrawPos(timeStacker).y - 7 - 20 * j;

                            treatedSprite.MoveInFrontOfOtherNode(healthTabBodyParts[i].background);

                            Debug.Log("Treated Sprite bodypart is " + i + " and combinedAffliction is " + j);
                        }
                    }
                    break;
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

        selectedSprite.x = DrawPos(timeStacker).x;
        selectedSprite.y = DrawPos(timeStacker).y;
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
            selectedSprite.x += 70;

            selectedSprite.y += 120;

            selectedSprite.y -= 11.6f;

            if (healthTabWholeBody.active && selectedVertical < healthTabWholeBody.afflictionNumber)
            {
                selectedSprite.y += 10 * healthTabWholeBody.afflictionNumber / 2;

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
                    maxVertical += healthTabBodyParts[i].combinedAfflictions.Count;

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

                selectedSprite.y = healthTabBodyParts[selectedBodyPart].DrawPos(timeStacker).y - 7 - 20 * selectedAffliction;

                selectedSprite.MoveInFrontOfOtherNode(healthTabBodyParts[selectedBodyPart].background);

                if (selectedTimer <= 0)
                {
                    AfflictionSelector(selectedBodyPart);
                }
            }

            selectedSprite.scaleX = 200;
            selectedSprite.scaleY = 20;
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
                    for (int i = 1; i < healthTabInfos.Count; i--)
                    {
                        healthTabInfos[i].slatedForDeletion = true;
                    }
                }

                if (healthTabInfos.Count > 0)
                {
                    healthTabInfos[0].name.text = "Pain: ";
                    healthTabInfos[0].nameStatus.text = " " + Mathf.Clamp(Mathf.Floor(value * 100), 0, 100) + "%";

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
                for (int i = 1; i < healthTabInfos.Count; i--)
                {
                    healthTabInfos[i].slatedForDeletion = true;
                }
            }

            if (healthTabWholeBody.bloodLossVisible && selectedVertical == 0)
            {
                healthTabInfos[0].name.text = healthTabWholeBody.bloodLossName.text + ": ";
                healthTabInfos[0].nameStatus.text = Mathf.Floor(state.bloodLoss * 100) + "%";

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
                healthTabInfos[0].name.text = healthTabWholeBody.afflictionNames[selectedVertical - (healthTabWholeBody.bloodLossVisible ? 1 : 0)].text + ": ";

                if (state.wholeBodyAfflictions[selectedVertical - (healthTabWholeBody.bloodLossVisible ? 1 : 0)] is RWDisease disease)
                {
                    healthTabInfos[0].nameStatus.text = Mathf.Floor(disease.severity * 100) + "%";
                }

                healthTabInfos[0].description.text = "";
            }

        }
        void AfflictionSelector(int selectedBodyPart)
        {
            if (healthTabInfos.Count == 0)
            {
                healthTabInfos.Add(new(this));
            }

            if (healthTabInfos.Count > 0)
            {
                healthTabInfos[0].name.text = healthTabBodyParts[selectedBodyPart].bodyPart.name + ": ";
                healthTabInfos[0].nameStatus.text = Mathf.Floor(healthTabBodyParts[selectedBodyPart].bodyPart.health) + " / " + healthTabBodyParts[selectedBodyPart].bodyPart.maxHealth;

                float efficiency = Mathf.Floor(healthTabBodyParts[selectedBodyPart].bodyPart.efficiency * 100);

                healthTabInfos[0].description.text = "Efficiency: " + ((efficiency < 1 && efficiency > 0) ? 1 : efficiency) + "%";
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
}

public class HealthTabBodyPart
{
    public HealthTabBodyPart(HealthTab owner, RWBodyPart bodyPart)
    {
        this.owner = owner;

        this.bodyPart = bodyPart;

        background = new("pixel", true);

        bodyPartName = new(Custom.GetFont(), bodyPart.name)
        {
            alignment = FLabelAlignment.Left,
            anchorX = 0f,
            anchorY = 1f
        };

        afflictionIcons = new FSprite[6];

        for (int i = 0; i < 6; i++)
        {
            afflictionNames.Add(new FLabel(Custom.GetFont(), "capacityName"));
            afflictionNames[i].alignment = FLabelAlignment.Left;
            afflictionNames[i].anchorX = 0f;
            afflictionNames[i].anchorY = 1f;

            afflictionIcons[i] = new FSprite("pixel", true);
        }

        owner.hud.fContainers[1].AddChild(background);
        owner.hud.fContainers[1].AddChild(bodyPartName);

        for (int i = 0; i < 6; i++)
        {
            owner.hud.fContainers[1].AddChild(afflictionNames[i]);
            owner.hud.fContainers[1].AddChild(afflictionIcons[i]);
        }
    }

    public Vector2 DrawPos(float timeStacker)
    {
        Vector2 pos = owner.DrawPos(timeStacker);

        pos.y += 120;

        if (owner.healthTabWholeBody.active)
        {
            pos.y -= 10 + (owner.healthTabWholeBody.afflictionNumber * 10) + ((owner.healthTabWholeBody.afflictionNumber - 1) * 10);
        }

        for (int i = 0; i < owner.healthTabBodyParts.IndexOf(this); i++)
        {
            pos.y -= 10 + (owner.healthTabBodyParts[i].afflictionNumber * 10) + ((owner.healthTabBodyParts[i].afflictionNumber - 1) * 10);
        }

        return pos;
    }

    public void Update()
    {
        afflictionNumber = bodyPart.afflictions.Count;

        combinedAfflictions = new();

        for (int i = 0; i < bodyPart.afflictions.Count; i++)
        {
            if (bodyPart.afflictions[i] is RWInjury injury && injury.healingDifficulty.combines)
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
                combinedAfflictions.Add(CombinedAfflictionName(bodyPart, i), new(1) { bodyPart.afflictions[i] });
            }
        }

        if (bodyPart == null || bodyPart.afflictions.Count <= 0)
        {
            slatedForDeletion = true;
        }
    }

    public void Draw(float timeStacker)
    {
        background.isVisible = owner.visible && owner.healthTabWholeBody.active ? !(owner.healthTabBodyParts.IndexOf(this) % 2 == 0) : owner.healthTabBodyParts.IndexOf(this) % 2 == 0;

        bodyPartName.isVisible = owner.visible;

        for (int i = 0; i < afflictionNames.Count; i++)
        {
            afflictionNames[i].isVisible = owner.visible && combinedAfflictions.Count > i;
        }

        for (int i = 0; i < afflictionIcons.Length; i++)
        {
            afflictionIcons[i].isVisible = owner.visible && combinedAfflictions.Count > i;
        }

        if (owner == null || !owner.visible)
        {
            return;
        }

        if (background.isVisible)
        {
            background.x = DrawPos(timeStacker).x;
            background.y = DrawPos(timeStacker).y - 2 - ((afflictionNumber > 1 ? 5f : 0) + (5 * afflictionNumber));
            background.scaleX = 340;
            background.scaleY = 10 + ((afflictionNumber > 1 ? 10f : 0) + (10 * afflictionNumber));
            background.color = Color.black;
        }

        bodyPartName.x = DrawPos(timeStacker).x - 165;
        bodyPartName.y = DrawPos(timeStacker).y;
        bodyPartName.color = Color.white;

        List<string> usedAfflictions = new();

        for (int i = 0; i < combinedAfflictions.Count; i++)
        {
            afflictionNames[i].x = DrawPos(timeStacker).x - 25;
            afflictionNames[i].y = DrawPos(timeStacker).y - 20 * i;
            afflictionNames[i].color = Color.white;

            afflictionIcons[i].x = DrawPos(timeStacker).x + 160;
            afflictionIcons[i].y = DrawPos(timeStacker).y - 7 - 20 * i;
            afflictionIcons[i].scale = 14;

            for (int j = 0; j < bodyPart.afflictions.Count; j++)
            {
                if (!usedAfflictions.Contains(CombinedAfflictionName(bodyPart, j)) && combinedAfflictions.TryGetValue(CombinedAfflictionName(bodyPart, j), out List<RWAffliction> dic))
                {
                    usedAfflictions.Add(CombinedAfflictionName(bodyPart, j));

                    if (bodyPart.afflictions[j] is RWInjury injury)
                    {
                        string text;

                        if (injury is RWDestroyed)
                        {
                            text = (bodyPart.isInternal ? bodyPart.isSolid ? "Shattered" : injury.healingDifficulty.destroyedOut : injury.healingDifficulty.destroyed) + (injury.isBleeding && !injury.isTended ? " (fresh)" : "");
                        }
                        else
                        {
                            text = injury.healingDifficulty.name + (injury.attackerName != "" ? " (" + injury.attackerName + ") " : "") + (dic.Count > 1 ? " x" + dic.Count : "");
                        }

                        afflictionNames[i].text = text;

                        afflictionIcons[i].color = injury.isTended ? Color.white : injury.isBleeding ? Color.red : Color.blue;
                    }


                    break;
                }
            }
        }
    }

    public void ClearSprites()
    {
        background.RemoveFromContainer();

        bodyPartName.RemoveFromContainer();

        for (int i = 0; i < afflictionNames.Count; i++)
        {
            afflictionNames[i].RemoveFromContainer();
        }

        for (int i = 0; i < afflictionIcons.Length; i++)
        {
            afflictionIcons[i].RemoveFromContainer();
        }

        combinedAfflictions.Clear();
    }

    public string CombinedAfflictionName(RWBodyPart bodyPart, int i)
    {
        string name = "";

        if (bodyPart.afflictions[i] is RWInjury injury)
        {
            name = injury.healingDifficulty.name + injury.attackerName + injury.isTended;
        }
        else if (bodyPart.afflictions[i] is RWDisease disease)
        {
            name = disease.name;
        }

        return name;
    }

    public HealthTab owner;

    public RWBodyPart bodyPart;

    public FSprite background;

    public FLabel bodyPartName;

    public List<FLabel> afflictionNames = new();
    public FSprite[] afflictionIcons;

    public int afflictionNumber = 1;

    public Dictionary<string, List<RWAffliction>> combinedAfflictions;

    public bool slatedForDeletion = false;
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

        afflictionIcons = new FSprite[6];

        bloodLossName = new(Custom.GetFont(), "Blood loss ()")
        {
            alignment = FLabelAlignment.Left,
            anchorX = 0f,
            anchorY = 1f,
            color = Color.white
        };

        for (int i = 0; i < 6; i++)
        {
            afflictionNames.Add(new FLabel(Custom.GetFont(), "capacityName"));
            afflictionNames[i].alignment = FLabelAlignment.Left;
            afflictionNames[i].anchorX = 0f;
            afflictionNames[i].anchorY = 1f;

            afflictionIcons[i] = new FSprite("pixel", true);
        }

        owner.hud.fContainers[1].AddChild(background);
        owner.hud.fContainers[1].AddChild(name);

        owner.hud.fContainers[1].AddChild(bloodLossName);

        for (int i = 0; i < 6; i++)
        {
            owner.hud.fContainers[1].AddChild(afflictionNames[i]);
            owner.hud.fContainers[1].AddChild(afflictionIcons[i]);
        }
    }

    public Vector2 DrawPos(float timeStacker)
    {
        Vector2 pos = owner.DrawPos(timeStacker);

        pos.y += 120;

        return pos;
    }

    public void Update()
    {
        bloodLossVisible = owner.state != null && owner.state.bloodLoss >= 0.15f;

        afflictionNumber = bloodLossVisible ? 1 : 0 + (owner.state != null ? owner.state.wholeBodyAfflictions.Count : 0);

        active = afflictionNumber > 0;
    }
    public void Draw(float timeStacker)
    {
        background.isVisible = owner.visible && active;

        name.isVisible = owner.visible && active;

        bloodLossName.isVisible = owner.visible && active && bloodLossVisible;

        for (int i = 0; i < afflictionNames.Count; i++)
        {
            afflictionNames[i].isVisible = owner.visible && active && afflictionNumber - (bloodLossVisible ? 1 : 0) > i;
        }

        for (int i = 0; i < afflictionIcons.Length; i++)
        {
            afflictionIcons[i].isVisible = owner.visible && active && afflictionNumber - (bloodLossVisible ? 1 : 0) > i;
        }

        if (owner == null || owner.state == null || !owner.visible || !active)
        {
            return;
        }

        if (background.isVisible)
        {
            background.x = DrawPos(timeStacker).x;
            background.y = DrawPos(timeStacker).y - 2 - (10 * afflictionNumber) / 2;
            background.scaleX = 340;
            background.scaleY = 10 + (10 * afflictionNumber);
            background.color = Color.black;
        }

        name.x = DrawPos(timeStacker).x - 165;
        name.y = DrawPos(timeStacker).y;

        if (bloodLossVisible)
        {
            bloodLossName.x = DrawPos(timeStacker).x - 25;
            bloodLossName.y = DrawPos(timeStacker).y;

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

        for (int i = 0; i < afflictionNumber - (bloodLossVisible ? 1 : 0); i++)
        {
            afflictionNames[i].x = DrawPos(timeStacker).x - 25;
            afflictionNames[i].y = DrawPos(timeStacker).y - (bloodLossVisible ? 15 : 0 + (15 * i));
            afflictionNames[i].color = Color.yellow;

            afflictionIcons[i].x = DrawPos(timeStacker).x + 160;
            afflictionIcons[i].y = DrawPos(timeStacker).y - 7 - (bloodLossVisible ? 10 : 0 + (10 * i)) / 2;
            afflictionIcons[i].scale = 14;

            if (owner.state.wholeBodyAfflictions[i] is RWDisease diesease)
            {
                string text;

                text = diesease.name;

                afflictionNames[i].text = text;

                afflictionIcons[i].color = diesease.isTended ? Color.white : Color.grey;
            }
        }
    }

    public void ClearSprites()
    {
        background.RemoveFromContainer();

        name.RemoveFromContainer();

        bloodLossName.RemoveFromContainer();

        for (int i = 0; i < afflictionNames.Count; i++)
        {
            afflictionNames[i].RemoveFromContainer();
        }

        for (int i = 0; i < afflictionIcons.Length; i++)
        {
            afflictionIcons[i].RemoveFromContainer();
        }
    }

    public HealthTab owner;

    public FSprite background;

    public FLabel name;

    public FLabel bloodLossName;

    public List<FLabel> afflictionNames = new();
    public FSprite[] afflictionIcons;

    public int afflictionNumber = 0;

    public bool bloodLossVisible = false;

    public bool active = false;
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

        nameStatus = new(Custom.GetFont(), ": Status")
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

    public Vector2 DrawPos(float timeStacker)
    {
        Vector2 pos = new(owner.selectedSprite.x, owner.selectedSprite.y);

        pos.x += (owner.selectedSprite.scaleX / 2) + 10;
        pos.y -= (owner.selectedSprite.scaleY / 2) + 5;

        for (int i = 0; i < owner.healthTabInfos.IndexOf(this); i++)
        {
            pos.y -= 10 + owner.healthTabInfos[i].backgrounds[0].scaleY;
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
    public void Draw(float timeStacker)
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

        name.x = DrawPos(timeStacker).x;
        name.y = DrawPos(timeStacker).y;

        nameStatus.x = DrawPos(timeStacker).x;

        nameStatus.x += name.textRect.width;

        nameStatus.y = DrawPos(timeStacker).y;

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

        if (owner.selectedHorizontal == 1)
        {
            if (owner.healthTabInfos.IndexOf(this) == 0 && owner.healthTabWholeBody.active && owner.selectedVertical < owner.healthTabWholeBody.afflictionNumber)
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
            else if (owner.healthTabInfos.IndexOf(this) == 0)
            {
                description.x = name.x;
                description.y = name.y - name.textRect.height;
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

        backgrounds[0].x = DrawPos(timeStacker).x + (backgroundPos.x / 2);
        backgrounds[0].y = DrawPos(timeStacker).y + (backgroundPos.y / 2) - 15;

        backgrounds[0].scaleX = backgroundSize.x + 10;
        backgrounds[0].scaleY = backgroundSize.y + 10;
        backgrounds[0].color = Color.black;

        backgrounds[1].x = DrawPos(timeStacker).x + (backgroundPos.x / 2);
        backgrounds[1].y = DrawPos(timeStacker).y + (backgroundPos.y / 2) - 15;

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

        description.RemoveFromContainer();
    }

    public HealthTab owner;

    public FSprite[] backgrounds;

    public FLabel name;
    public FLabel nameStatus;

    public FLabel description;

    public bool slatedForDeletion = false;
}