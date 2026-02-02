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

        for (int k = 0; k < sprites.Length; k++)
        {
            sprites[k] = new FSprite("pixel", true);
            hud.fContainers[1].AddChild(sprites[k]);
        }

        hud.fContainers[1].AddChild(statName);

        for (int k = 0; k < statValueNames.Count; k++)
        {
            hud.fContainers[1].AddChild(statValueNames[k]);
            hud.fContainers[1].AddChild(statValues[k]);
        }
    }

    public Vector2 DrawPos(float timeStacker)
    {
        return new Vector2(hud.rainWorld.screenSize.x / 2, hud.rainWorld.screenSize.y / 2);
    }

    public override void Update()
    {
        base.Update();

        if (owner == null || owner.realizedCreature == null || !owner.realizedCreature.Consious)
        {
            visible = false;

            if (state == null || !visible)
            {
                for (int i = 0; i < healthTabBodyParts.Count; i++)
                {
                    healthTabBodyParts[i].ClearSprites();
                }

                healthTabBodyParts.Clear();

                return;
            }
        }

        List<HealthTabBodyPart> tempList = new(healthTabBodyParts);

        for (int i = 0; i < tempList.Count; i++)
        {
            if (tempList[i] == null || !visible)
            {
                healthTabBodyParts.Remove(tempList[i]);
            }
        }

        if (state == null || !visible)
        {
            for (int i = 0; i < healthTabBodyParts.Count; i++)
            {
                healthTabBodyParts[i].ClearSprites();
            }

            healthTabBodyParts.Clear();

            return;
        }

        for (int i = 0; i < state.bodyParts.Count; i++)
        {
            if (state.bodyParts[i].injuries.Count > 0)
            {
                bool alreadyExists = false;
                for (int j = 0; j < healthTabBodyParts.Count; j++)
                {
                    if (healthTabBodyParts[j].bodyPart == state.bodyParts[i])
                    {
                        alreadyExists = true;
                        break;
                    }
                }

                if (!alreadyExists)
                {
                    HealthTabBodyPart part = new( this, state.bodyParts[i]);

                    healthTabBodyParts.Add(part);
                }
            }
        }

        for (int i = 0; i < healthTabBodyParts.Count; i++)
        {
            healthTabBodyParts[i].Update();
        }
    }

    public void ToggleVisibility(RWPlayerHealthState state)
    {
        visible = !visible;

        if (visible)
        {
            this.state = state;
        }
        else
        {
            this.state = null;

            if (state == null || !visible)
            {
                for (int i = 0; i < healthTabBodyParts.Count; i++)
                {
                    healthTabBodyParts[i].ClearSprites();
                }

                healthTabBodyParts.Clear();

                return;
            }
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

        for (int i = 0; i < healthTabBodyParts.Count; i++)
        {
            healthTabBodyParts[i].Draw(timeStacker);
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
                    value = Mathf.Floor(state.pain * 10) / 10;
                    break;
                case "Consiousness":
                    value = Mathf.Floor(state.consciousness * 10) / 10;
                    break;
                case "Moving":
                    value = Mathf.Floor(state.moving * 10) / 10;
                    break;
                case "Manipulation":
                    value = Mathf.Floor(state.manipulation * 10) / 10;
                    break;
                case "Talking":
                    value = Mathf.Floor(state.talking * 10) / 10;
                    break;
                case "Eating":
                    value = Mathf.Floor(state.eating * 10) / 10;
                    break;
                case "Sight":
                    value = Mathf.Floor(state.sight * 10) / 10;
                    break;
                case "Hearing":
                    value = Mathf.Floor(state.hearing * 10) / 10;
                    break;
                case "Breathing":
                    value = Mathf.Floor(state.breathing * 10) / 10;
                    break;
                case "Blood filtrarion":
                    value = Mathf.Floor(state.bloodFiltration * 10) / 10;
                    break;
                case "Blood pumping":
                    value = Mathf.Floor(state.bloodPumping * 10) / 10;
                    break;
                case "Digestion":
                    value = Mathf.Floor(state.digestion * 10) / 10;
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
                    statValues[i].color = Color.black;
                }
            }
        }
    }

    public override void ClearSprites()
    {
        base.ClearSprites();

        for (int i = 0; i < healthTabBodyParts.Count; i++)
        {
            healthTabBodyParts[i].ClearSprites();
        }

        for (int i = 0; i < this.sprites.Length; i++)
        {
            this.sprites[i].RemoveFromContainer();
        }
    }

    public RWPlayerHealthState state;

    public FLabel statName;

    public List<FLabel> statValueNames = new();
    public List<FLabel> statValues = new();
    public FSprite[] sprites;

    public List<string> statValueNamesNames;

    public AbstractCreature owner;

    public bool visible = false;

    public List<HealthTabBodyPart> healthTabBodyParts = new();
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
            afflictionNames.Add(new FLabel(Custom.GetFont(), "StatName"));
            afflictionNames[i].alignment = FLabelAlignment.Left;
            afflictionNames[i].anchorX = 0f;
            afflictionNames[i].anchorY = 1f;

            afflictionIcons[i] = new FSprite("pixel", true);

            owner.hud.fContainers[1].AddChild(afflictionNames[i]);
            owner.hud.fContainers[1].AddChild(afflictionIcons[i]);
        }
    }

    public Vector2 DrawPos(float timeStacker)
    {
        Vector2 pos = owner.DrawPos(timeStacker);

        pos.y += 110;

        for (int i = 0; i < owner.healthTabBodyParts.IndexOf(this); i++)
        {
            pos.y -= 10 + owner.healthTabBodyParts[i].afflictionNumber * 10;
        }

        return pos;
    }

    public void Update()
    {
        afflictionNumber = bodyPart.injuries.Count;

        combinedAfflictions = new();


        for (int i = 0; i < bodyPart.injuries.Count; i++)
        {
            if (bodyPart.injuries[i].healingDifficulty.combines)
            {
                if (combinedAfflictions.TryGetValue(bodyPart.injuries[i].healingDifficulty.name + bodyPart.injuries[i].attackerName, out List<RWInjury> dic))
                {
                    dic.Add(bodyPart.injuries[i]);
                    afflictionNumber--;
                }
                else
                {
                    combinedAfflictions.Add(bodyPart.injuries[i].healingDifficulty.name + bodyPart.injuries[i].attackerName, new(1) { bodyPart.injuries[i] });
                }
            }
            else
            {
                combinedAfflictions.Add(bodyPart.injuries[i].healingDifficulty.name + bodyPart.injuries[i].attackerName, new(1) { bodyPart.injuries[i] });
            }
        }

        if (owner == null || !owner.visible || bodyPart == null || bodyPart.injuries.Count <= 0)
        {
            slatedForDeletion = true;
        }
    }
    public void Draw(float timeStacker)
    {
        background.isVisible = owner.visible && owner.healthTabBodyParts.IndexOf(this) % 2 == 0;

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
            background.y = DrawPos(timeStacker).y - 1 - (10 * afflictionNumber)/2;
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

            for (int j = 0; j < bodyPart.injuries.Count; j++)
            {
                if (!usedAfflictions.Contains(bodyPart.injuries[j].healingDifficulty.name + bodyPart.injuries[j].attackerName) && combinedAfflictions.TryGetValue(bodyPart.injuries[j].healingDifficulty.name + bodyPart.injuries[j].attackerName, out List<RWInjury> dic))
                {
                    usedAfflictions.Add(bodyPart.injuries[j].healingDifficulty.name + bodyPart.injuries[j].attackerName);

                    afflictionNames[i].text = bodyPart.injuries[j].healingDifficulty.name + bodyPart.injuries[j].attackerName + (dic.Count > 1 ? " x" + dic.Count : "");
                }
            }
        }
    }

    public void ClearSprites()
    {
        background.RemoveFromContainer();

        bodyPartName.RemoveFromContainer();

        for (int i = 0; i < this.afflictionNames.Count; i++)
        {
            this.afflictionNames[i].RemoveFromContainer();
        }

        for (int i = 0; i < this.afflictionIcons.Length; i++)
        {
            this.afflictionIcons[i].RemoveFromContainer();
        }
    }

    public HealthTab owner;

    public RWBodyPart bodyPart;

    public FSprite background;

    public FLabel bodyPartName;

    public List<FLabel> afflictionNames = new();
    public FSprite[] afflictionIcons;

    public int afflictionNumber = 1;

    Dictionary<string, List<RWInjury>> combinedAfflictions;

    public bool slatedForDeletion = false;
}