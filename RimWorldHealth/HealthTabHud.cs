using HUD;
using System.Collections.Generic;
using UnityEngine;
using RWCustom;

namespace ShadowOfRimWorldHealth;

public class HealthTab : HudPart
{
    public HealthTab(HUD.HUD hud, AbstractCreature owner) : base(hud)
    {
        this.owner = owner;

        sprites = new FSprite[2];

        statName = new(Custom.GetFont(), "Name")
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
            statValueNames.Add(new FLabel(Custom.GetFont(), "StatName"));
            statValueNames[i].alignment = FLabelAlignment.Left;
            statValueNames[i].anchorX = 0f;
            statValueNames[i].anchorY = 1f;

            statValues.Add(new FLabel(Custom.GetFont(), "StatValue"));
            statValues[i].alignment = FLabelAlignment.Left;
            statValues[i].anchorX = 1f;
            statValues[i].anchorY = 1f;
        }

        selectedSprite = new FSprite("pixel", true);

        for (int k = 0; k < sprites.Length; k++)
        {
            sprites[k] = new FSprite("pixel", true);
            hud.fContainers[1].AddChild(sprites[k]);
        }

        hud.fContainers[1].AddChild(statName);
        hud.fContainers[1].AddChild(bloodLossPerCycle);

        for (int k = 0; k < statValueNames.Count; k++)
        {
            hud.fContainers[1].AddChild(statValueNames[k]);
            hud.fContainers[1].AddChild(statValues[k]);
        }

        hud.fContainers[1].AddChild(selectedSprite);

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

                        if (isSubPartDestroyed(healthTabBodyParts[j].bodyPart))
                        {
                            healthTabBodyParts[j].slatedForDeletion = true;
                        }
                        break;
                    }
                }

                if (!alreadyExists && !isSubPartDestroyed(state.bodyParts[i]))
                {
                    HealthTabBodyPart part = new( this, state.bodyParts[i]);

                    healthTabBodyParts.Add(part);

                    part.Update();
                }
            }
        }

        if (state.bloodLoss >= 15)
        {

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

                if (selectedVertical != -1)
                {
                    int maxVertical = selectedHorizontal == 0 ? 12 : -1;

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

                int maxVertical = selectedHorizontal == 0 ? 12 : -1;

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

        bool isSubPartDestroyed(RWBodyPart self)
        {
            if (self.afflictions.Count == 1 && self.afflictions[0] is RWDestroyed && self.subPartOf != "")
            {
                for (int i = 0; i < state.bodyParts.Count; i++)
                {
                    if (state.bodyParts[i].name == self.subPartOf && state.bodyParts[i] is not UpperTorso && state.bodyParts[i] is not LowerTorso && state.bodyParts[i].afflictions.Count == 1 && state.bodyParts[i].afflictions[0] is RWDestroyed)
                    {
                        return true;
                    }
                }
            }

            return false;
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

        for (int i = 0; i < statValueNames.Count; i++)
        {
            statValueNames[i].isVisible = visible;
            statValues[i].isVisible = visible;
        }

        statName.isVisible = visible;

        bloodLossPerCycle.isVisible = visible && state.bloodLossPerCycle >= 1;

        selectedSprite.isVisible = visible && selected;

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

        statValueNamesNames = new(12) { "Pain", "Consiousness", "Moving", "Manipulation", "Talking", "Eating", "Sight", "Hearing", "Breathing", "Blood filtrarion", "Blood pumping", "Digestion" };

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

        statName.color = Color.white;
        statName.x = DrawPos(timeStacker).x - 320;
        statName.y = DrawPos(timeStacker).y + 125;
        statName.text = state.creature.ToString();

        if (bloodLossPerCycle.isVisible)
        {
            float bloodLoss = state.cycleLength / (state.bloodLossPerCycle / 100); //need to account for the blood already lost

            bloodLossPerCycle.color = Color.white;
            bloodLossPerCycle.x = DrawPos(timeStacker).x - 165;
            bloodLossPerCycle.y = DrawPos(timeStacker).y - 105;
            bloodLossPerCycle.text = "Bleeding: " + Mathf.Floor(state.bloodLossPerCycle) + "%/c (death in " + Mathf.Floor(bloodLoss) + " minutes)"; //if the time would be less then a minute change it to seconds
        }

        for (int i = 0; i < statValueNames.Count; i++)
        {
            if (i > statValueNamesNames.Count)
            {
                statValueNames[i].isVisible = false;
                statValues[i].isVisible = false;
                continue;
            }

            statValueNames[i].color = Color.white;
            statValueNames[i].x = DrawPos(timeStacker).x - 320;
            statValueNames[i].y = DrawPos(timeStacker).y + 100 - (17 * i);

            statValueNames[i].text = statValueNamesNames[i];

            statValues[i].x = DrawPos(timeStacker).x - 180;
            statValues[i].y = DrawPos(timeStacker).y + 100 - (17 * i);

            float value = 0;

            switch (statValueNamesNames[i])
            {
                case "Pain":
                    value = Mathf.Floor(state.pain);
                    break;
                case "Consiousness":
                    value = Mathf.Floor(state.consciousness);
                    break;
                case "Moving":
                    value = Mathf.Floor(state.moving);
                    break;
                case "Manipulation":
                    value = Mathf.Floor(state.manipulation);
                    break;
                case "Talking":
                    value = Mathf.Floor(state.talking);
                    break;
                case "Eating":
                    value = Mathf.Floor(state.eating);
                    break;
                case "Sight":
                    value = Mathf.Floor(state.sight);
                    break;
                case "Hearing":
                    value = Mathf.Floor(state.hearing);
                    break;
                case "Breathing":
                    value = Mathf.Floor(state.breathing);
                    break;
                case "Blood filtrarion":
                    value = Mathf.Floor(state.bloodFiltration);
                    break;
                case "Blood pumping":
                    value = Mathf.Floor(state.bloodPumping);
                    break;
                case "Digestion":
                    value = Mathf.Floor(state.digestion);
                    break;
            }

            if (statValueNamesNames[i] == "Pain")
            {
                if (value >= 80)
                {
                    statValues[i].text = "Mind-shattering";
                    statValues[i].color = Color.red;
                }
                else if (value >= 40)
                {
                    statValues[i].text = "Intense";
                    statValues[i].color = Color.Lerp(Color.white, Color.black, 0.6f);
                }
                else if (value >= 15)
                {
                    statValues[i].text = "Serious";
                    statValues[i].color = Color.Lerp(Color.white, Color.black, 0.4f);
                }
                else if (value >= 1)
                {
                    statValues[i].text = "Minor";
                    statValues[i].color = Color.Lerp(Color.white, Color.black, 0.1f);
                }
                else
                {
                    statValues[i].text = "None";
                    statValues[i].color = Color.green;
                }
            }
            else
            {
                statValues[i].text = value + "%";

                if (value > 100)
                {
                    statValues[i].color = Color.blue;
                }
                else if(value == 100)
                {
                    statValues[i].color = Color.green;
                }
                else if (value >= 80)
                {
                    statValues[i].color = Color.Lerp(Color.white, Color.black, 0.1f);
                }
                else if (value >= 40)
                {
                    statValues[i].color = Color.Lerp(Color.white, Color.black, 0.4f);
                }
                else if (value >= 15)
                {
                    statValues[i].color = Color.Lerp(Color.white, Color.black, 0.6f);
                }
                else if (value >= 1)
                {
                    statValues[i].color = Color.Lerp(Color.white, Color.black, 0.8f);
                }
                else
                {
                    statValues[i].color = Color.red;
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
                StatSelector();
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

                for (int i = 0; i < healthTabBodyParts.Count; i++)
                {
                    maxVertical += healthTabBodyParts[i].combinedAfflictions.Count;

                    Debug.Log(selectedVertical - (healthTabWholeBody.active ? healthTabWholeBody.afflictionNumber : 0));

                    if (lastMaxVertical <= selectedVertical - (healthTabWholeBody.active ? healthTabWholeBody.afflictionNumber : 0) && maxVertical > selectedVertical - (healthTabWholeBody.active ? healthTabWholeBody.afflictionNumber : 0))
                    {
                        selectedBodyPart = i;
                        break;
                    }
                    else
                    {
                        lastMaxVertical = maxVertical;
                    }
                }

                if (healthTabWholeBody.active)
                {
                    selectedSprite.y -= 10 + healthTabWholeBody.afflictionNumber * 10;
                }

                for (int i = 0; i < selectedBodyPart; i++)
                {
                    selectedSprite.y -= 10 + healthTabBodyParts[i].afflictionNumber * 10;
                }

                selectedSprite.y += 10 * healthTabBodyParts[selectedBodyPart].combinedAfflictions.Count / 2;

                selectedSprite.MoveInFrontOfOtherNode(healthTabBodyParts[selectedBodyPart].background);

                if (selectedTimer <= 0)
                {
                    AfflictionSelector(selectedBodyPart);
                }
            }

            selectedSprite.scaleX = 200;
            selectedSprite.scaleY = 20;
        }

        void StatSelector()
        {
            float value = 0;

            if (selectedVertical == 0)
            {
                SetPainInfo(Mathf.Floor(state.pain));
            }
            else
            {
                switch (statValueNamesNames[selectedVertical])
                {
                    case "Pain":
                        value = Mathf.Floor(state.pain);
                        break;
                    case "Consiousness":
                        value = Mathf.Floor(state.consciousness);
                        break;
                    case "Moving":
                        value = Mathf.Floor(state.moving);
                        break;
                    case "Manipulation":
                        value = Mathf.Floor(state.manipulation);
                        break;
                    case "Talking":
                        value = Mathf.Floor(state.talking);
                        break;
                    case "Eating":
                        value = Mathf.Floor(state.eating);
                        break;
                    case "Sight":
                        value = Mathf.Floor(state.sight);
                        break;
                    case "Hearing":
                        value = Mathf.Floor(state.hearing);
                        break;
                    case "Breathing":
                        value = Mathf.Floor(state.breathing);
                        break;
                    case "Blood filtrarion":
                        value = Mathf.Floor(state.bloodFiltration);
                        break;
                    case "Blood pumping":
                        value = Mathf.Floor(state.bloodPumping);
                        break;
                    case "Digestion":
                        value = Mathf.Floor(state.digestion);
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
                    healthTabInfos[0].nameStatus.text = " " + value + "%";

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

                if (value > 100)
                {
                    stringValue = "Enhanced";
                }
                else if (value == 100)
                {
                    stringValue = "OK";
                }
                else if (value >= 40)
                {
                    stringValue = "Weakened";
                }
                else if (value >= 15)
                {
                    stringValue = "Poor";
                }
                else if (value >= 1)
                {
                    stringValue = "Very Poor";
                }
                else
                {
                    stringValue = "None";
                }

                if (healthTabInfos.Count > 0)
                {
                    healthTabInfos[0].name.text = statValueNamesNames[selectedVertical] + ": ";
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
                healthTabInfos[0].nameStatus.text = Mathf.Floor(state.bloodLoss) + "%";

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

                if (state.bloodLoss >= 100)
                {

                }
                else if (state.bloodLoss >= 60)
                {
                    healthTabInfos[0].description.text += "\n" +
                        "\n" +
                        "  - Consciousness: -40%";

                    healthTabInfos[0].description.text += "\n" +
                        "  - Consciousness: Max 10%";
                }
                else if (state.bloodLoss >= 45)
                {
                    healthTabInfos[0].description.text += "\n" +
                        "\n" +
                        "  - Consciousness: -40%";
                }
                else if (state.bloodLoss >= 30)
                {
                    healthTabInfos[0].description.text += "\n" +
                        "\n" +
                        "  - Consciousness: -20%";
                }
                else if (state.bloodLoss >= 15)
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
                    healthTabInfos[0].nameStatus.text = Mathf.Floor(disease.severity) + "%";
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

                healthTabInfos[0].description.text = "Efficiency: " + healthTabBodyParts[selectedBodyPart].bodyPart.efficiency + "%";
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

        statName.RemoveFromContainer();

        for (int i = 0; i < statValueNames.Count; i++)
        {
            statValueNames[i].RemoveFromContainer();
        }

        for (int i = 0; i < statValues.Count; i++)
        {
            statValues[i].RemoveFromContainer();
        }

        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].RemoveFromContainer();
        }

        selectedSprite.RemoveFromContainer();
    }

    void TriggeredOn()
    {
        selected = false;

        selectedHorizontal = 0;
        selectedVertical = -1;

        horizontalOnce = true;
        verticalOnce = true;

        selectedTimer = selectedTimerMax;
    }
    void TriggeredOff()
    {
        selected = false;

        selectedHorizontal = 0;
        selectedVertical = 0;

        selectedTimer = selectedTimerMax;
    }

    public RWPlayerHealthState state;

    public FLabel statName;

    public FLabel bloodLossPerCycle;

    public List<FLabel> statValueNames = new();
    public List<FLabel> statValues = new();
    public FSprite[] sprites;

    public FSprite selectedSprite;

    public List<string> statValueNamesNames;

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
            afflictionNames.Add(new FLabel(Custom.GetFont(), "StatName"));
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
            pos.y -= 10 + owner.healthTabWholeBody.afflictionNumber * 10;
        }

        for (int i = 0; i < owner.healthTabBodyParts.IndexOf(this); i++)
        {
            pos.y -= 10 + owner.healthTabBodyParts[i].afflictionNumber * 10;
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
            background.y = DrawPos(timeStacker).y - 2 - (10 * afflictionNumber)/2;
            background.scaleX = 340;
            background.scaleY = 10 + (10 * afflictionNumber);
            background.color = Color.black;
        }

        bodyPartName.x = DrawPos(timeStacker).x - 165;
        bodyPartName.y = DrawPos(timeStacker).y;
        bodyPartName.color = Color.white;

        List<string> usedAfflictions = new();

        for (int i = 0; i < combinedAfflictions.Count; i++)
        {
            afflictionNames[i].x = DrawPos(timeStacker).x - 25;
            afflictionNames[i].y = DrawPos(timeStacker).y - 15 * i;
            afflictionNames[i].color = Color.white;

            afflictionIcons[i].x = DrawPos(timeStacker).x + 160;
            afflictionIcons[i].y = DrawPos(timeStacker).y - 7 - (10 * i) / 2;
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
                            text = (bodyPart.isInternal ? bodyPart.isSolid ? "Shattered" : injury.healingDifficulty.destroyedOut : injury.healingDifficulty.destroyed) + (injury.isBleeding ? " (fresh)" : "");
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

    string CombinedAfflictionName(RWBodyPart bodyPart, int i)
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
            afflictionNames.Add(new FLabel(Custom.GetFont(), "StatName"));
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
        bloodLossVisible = owner.state != null && owner.state.bloodLoss >= 15;

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

            if (owner.state.bloodLoss >= 60)
            {
                bloodLossName.text = "Blood loss (extreme)";
            }
            else if (owner.state.bloodLoss >= 45)
            {
                bloodLossName.text = "Blood loss (severe)";
            }
            else if (owner.state.bloodLoss >= 30)
            {
                bloodLossName.text = "Blood loss (moderate)";
            }
            else if (owner.state.bloodLoss >= 15)
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