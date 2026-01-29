using HUD;
using RWCustom;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace ShadowOfRimWorldHealth;

public class HealthTab : HudPart
{
    public HealthTab(HUD.HUD hud, AbstractCreature owner) : base(hud)
    {
        this.owner = owner;

        InitiateSprites();
    }

    public Vector2 DrawPos(float timeStacker)
    {
        return new Vector2(hud.rainWorld.screenSize.x / 2, hud.rainWorld.screenSize.y / 2);
    }

    public override void Update()
    {
        base.Update();
    }

    public void InitiateSprites()
    {
        sprites = new FSprite[1];

        sprites[0] = new FSprite("pixel", true);

        for (int k = 0; k < this.sprites.Length; k++)
        {
            this.hud.fContainers[1].AddChild(this.sprites[k]);
        }
    }

    public override void Draw(float timeStacker)
    {
        base.Draw(timeStacker);

        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].isVisible = visible;

            if (!visible)
            {
                return;
            }

            if (owner != null)
            {
                sprites[i].x = DrawPos(timeStacker).x;
                sprites[i].y = DrawPos(timeStacker).y;
                sprites[i].scale = 10;
                sprites[i].color = Color.blue;
            }
        }
    }

    public override void ClearSprites()
    {
        base.ClearSprites();
    }

    public FLabel label;
    public FSprite[] sprites;

    public float actualWidth;
    public Color currentColor;

    public AbstractCreature owner;

    public bool visible = false;
}
