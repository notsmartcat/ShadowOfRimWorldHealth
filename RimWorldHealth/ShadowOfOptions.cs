using System.Collections.Generic;
using Menu.Remix.MixedUI;
using UnityEngine;

namespace ShadowOfRimWorldHealth;

public class ShadowOfOptions : OptionInterface
{
    public ShadowOfOptions(RimWorldHealth _)
    {
        debug_keys = config.Bind("debug_keys", false, new ConfigurableInfo("If turned On N deals a small amountt of damage to the player's head and M cut's off the players right arm and gives the player the Flu. (Default = false)", null, "", new object[1] { "Debug Keys" }));
        debug_logs = config.Bind("debug_logs", false, new ConfigurableInfo("If turned On messages that include a lot of info about Lizards will show up when you turn on Debug Logs, these will also appear in the 'consoleLog.txt' all logs from this mod start with 'ShadowOfRWHealth:' for easy locating. (Default = false)", null, "", new object[1] { "Debug Logs" }));

        karma_flower = config.Bind("karma_flower", true, new ConfigurableInfo("If turned On eating a Karma Flower will heal the most severe ailment the creature that eats it has. (Default = true)", null, "", new object[1] { "Karma Flower Healer" }));
        karma_flower_reinforced = config.Bind("karma_flower_reinforced", true, new ConfigurableInfo("If turned On Karma Flowers eaten by a Slugcat or Slugpup will only heal if the Player has their Karma Reinforcement active. (Default = true)", null, "", new object[1] { "Karma Flower Reinforcement Required" }));

        after_cycle_length = config.Bind("after_cycle_length", 13, new ConfigurableInfo("How much time (in minutes) passes during hibernation, this time is used to heal afflictions of all creatures. (Default = 13)", new ConfigAcceptableRange<int>(0, 30), "", new object[1] { "Hibernation Time" }));
        update_abstracted = config.Bind("update_abstracted", 3, new ConfigurableInfo("How often (if at all) will abstracted creatures have their health info updated. (Default = 13)", new ConfigAcceptableRange<int>(0, 5), "", new object[1] { "Abstracted Update" }));

        downed_tend = config.Bind("downed_tend", "None, Player only or All", new ConfigurableInfo("Tend when \nDowned"));
        downed_combat = config.Bind("downed_combat", "None, Player only or All", new ConfigurableInfo("Combat when \nDowned"));

        player_1_key = config.Bind("player_1_key", KeyCode.H);
        player_2_key = config.Bind("player_2_key", KeyCode.G);
        player_3_key = config.Bind("player_3_key", KeyCode.H);
        player_4_key = config.Bind("player_4_key", KeyCode.H);

        #region Player Specific
        coop_dead_saving = config.Bind("coop_dead_saving", "Clear all afflictions or clear life threatening afflictions", new ConfigurableInfo("Dead Saving"));

        player_uncon_movement = config.Bind("player_uncon_movement", true, new ConfigurableInfo("If turned On Players will still be able to move while they are Unconscious (Consciousness less then 30%) at a significantly slower rate. (Default = true)", null, "", new object[1] { "Uncon Player Movement" }));
        #endregion
    }

    #region Misc Values
    readonly float font_height = 20f;
    readonly float spacing = 20f;
    readonly int number_of_check_boxes = 3;
    readonly float check_box_size = 24f;

    Vector2 margin_x = default;
    Vector2 position = default;
    readonly List<OpLabel> text_labels = new();
    readonly List<float> box_end_positions = new();

    readonly List<Configurable<bool>> check_box_configurables = new();
    readonly List<OpLabel> check_boxes_text_labels = new();

    readonly List<Configurable<int>> slider_configurables = new();
    readonly List<string> slider_main_text_labels = new();
    readonly List<OpLabel> slider_text_labels_left = new();
    readonly List<OpLabel> slider_text_labels_right = new();

    float Check_Box_With_Spacing => check_box_size + 0.25f * spacing;
    #endregion

    public override void Initialize()
    {
        base.Initialize();
        Tabs = new OpTab[2];

        #region Main Options
        Tabs[0] = new OpTab(this, "Main Options");
        InitializeMarginAndPos();

        AddNewLine();
        AddBox();
        AddCheckBox(debug_keys);
        AddCheckBox(debug_logs);
        DrawCheckBoxes(ref Tabs[0]);
        DrawBox(ref Tabs[0]);

        AddNewLine();
        AddBox();
        AddCheckBox(karma_flower);
        AddCheckBox(karma_flower_reinforced);
        DrawCheckBoxes(ref Tabs[0]);
        DrawBox(ref Tabs[0]);

        AddNewLine();
        AddBox();
        AddSlider(after_cycle_length);
        DrawSliders(ref Tabs[0]);
        DrawBox(ref Tabs[0]);

        AddNewLine();
        AddBox();
        AddUpdateAbstractedSlider(ref Tabs[0]);
        DrawBox(ref Tabs[0]);

        AddNewLine();
        AddBox();
        AddNewLine();
        DrawComboBox(ref Tabs[0], downed_tend, new List<string> { "No one", "Player only", "Everyone" });
        DrawBox(ref Tabs[0]);

        AddNewLine();
        AddBox();
        AddNewLine();
        DrawComboBox(ref Tabs[0], downed_combat, new List<string> { "No one", "Player only", "Everyone" });
        DrawBox(ref Tabs[0]);
        #endregion

        #region Player Specific Options
        Tabs[1] = new OpTab(this, "Player");
        InitializeMarginAndPos();

        AddNewLine();
        AddBox();
        AddCheckBox(player_uncon_movement);
        DrawCheckBoxes(ref Tabs[1]);
        DrawBox(ref Tabs[1]);

        AddNewLine();
        AddBox();
        AddNewLine();
        DrawComboBox(ref Tabs[1], coop_dead_saving, new List<string> { "Clear All Afflictions", "Clear Life Threatening Afflictions Only" });
        DrawBox(ref Tabs[1]);

        AddNewLine();
        AddBox();
        DrawKeyCodes(ref Tabs[1]);
        DrawBox(ref Tabs[1]);
        #endregion
    }

    public override void Update()
    {
        string text = update_abstracted.BoundUIconfig.value switch
        {
            "0" => "Never",
            "1" => "5 seconds",
            "2" => "10 seconds",
            "3" => "30 seconds",
            "4" => "1 Minute",
            "5" => "2 Minutes",
            _ => null,
        };

        OPupdateAbstractedSlider._label.text = text;
    }

    #region Functions
    void InitializeMarginAndPos()
    {
        margin_x = new Vector2(20f, 550f);
        position = new Vector2(20f, 600f);
    }

    void AddNewLine(float spacingModifier = 1f)
    {
        position.x = margin_x.x;
        position.y -= spacingModifier * spacing;
    }

    void AddBox()
    {
        margin_x += new Vector2(spacing, 0f - spacing);
        box_end_positions.Add(position.y);
        AddNewLine();
    }

    void DrawBox(ref OpTab tab)
    {
        margin_x += new Vector2(0f - spacing, spacing);
        AddNewLine();
        float num = margin_x.y - margin_x.x + 20;
        int index = box_end_positions.Count - 1;
        tab.AddItems((UIelement[])(object)new UIelement[1] { new OpRect(position, new Vector2(num, box_end_positions[index] - position.y), 0.3f) });
        box_end_positions.RemoveAt(index);
    }

    void AddCheckBox(Configurable<bool> configurable, string text = null)
    {
        check_box_configurables.Add(configurable);

        text ??= (string)configurable.info.Tags[0];

        check_boxes_text_labels.Add(new OpLabel(default, default, text, (FLabelAlignment)1, false, null));
    }

    void DrawCheckBoxes(ref OpTab tab)
    {
        if (check_box_configurables.Count != check_boxes_text_labels.Count)
        {
            return;
        }
        float num = margin_x.y - margin_x.x;
        float num2 = (num - (number_of_check_boxes - 1) * 0.5f * spacing) / number_of_check_boxes;
        position.y -= check_box_size;
        float num3 = position.x;
        for (int i = 0; i < check_box_configurables.Count; i++)
        {
            Configurable<bool> val = check_box_configurables[i];
            OpCheckBox val2 = new(val, new Vector2(num3, position.y))
            {
                description = (val.info?.description ?? "")
            };
            tab.AddItems((UIelement[])(object)new UIelement[1] { val2 });
            num3 += Check_Box_With_Spacing;
            OpLabel val3 = check_boxes_text_labels[i];
            ((UIelement)val3).pos = new Vector2(num3, position.y + 2f);
            val3.size = new Vector2(num2 - Check_Box_With_Spacing, font_height);
            tab.AddItems((UIelement[])(object)new UIelement[1] { val3 });
            if (i < check_box_configurables.Count - 1)
            {
                if ((i + 1) % number_of_check_boxes == 0)
                {
                    AddNewLine();
                    position.y -= check_box_size;
                    num3 = position.x;
                }
                else
                {
                    num3 += num2 - Check_Box_With_Spacing + 0.5f * spacing;
                }
            }
        }
        check_box_configurables.Clear();
        check_boxes_text_labels.Clear();
    }

    void DrawComboBox(ref OpTab tab, Configurable<string> config, List<string> list)
    {
        float num = margin_x.y - margin_x.x;
        float num2 = num * 0.5f * spacing;
        float num3 = position.x;

        List<ListItem> items = new();

        for (int i = 0; i < list.Count; i++)
        {
            items.Add(new ListItem(list[i], i));
        }

        if (items.Count == 0)
        {
            return;
        }

        OpLabel val3 = new(default, default, config.info.description, (FLabelAlignment)1, true, null);
        ((UIelement)val3).pos = new Vector2(num3, position.y + 4f);
        val3.size = new Vector2(num2, font_height);
        num3 += 150;
        tab.AddItems((UIelement[])(object)new UIelement[1] { val3 });

        ComboBox val2 = new(config, new Vector2(num3, position.y), 350, items);
        tab.AddItems((UIelement[])(object)new UIelement[1] { val2 });
    }

    void DrawKeyCodes(ref OpTab tab)
    {
        float num = margin_x.y - margin_x.x;
        float num2 = num * 0.5f * spacing;
        float num3 = position.x;

        AddNewLine();

        OpLabel label = new(default, default, "Health Tab Hud Keybinds", 0, true, null);
        ((UIelement)label).pos = new Vector2(num3 + 225, position.y + 10f);
        label.size = new Vector2(num3, font_height);
        tab.AddItems((UIelement[])(object)new UIelement[1] { label });

        AddNewLine(1.5f);

        OpKeyBinder keyBinder;
        OpLabel playerNumberLabel;

        Configurable<KeyCode> keyCode = null;
        string playerNumber = "";

        for (int i = 1; i < 5; i++)
        {
            switch (i)
            {
                case 1:
                    keyCode = player_1_key;
                    playerNumber = "Player 1";
                    break;
                case 2:
                    keyCode = player_2_key;
                    playerNumber = "Player 2";
                    break;
                case 3:
                    keyCode = player_3_key;
                    playerNumber = "Player 3";
                    break;
                case 4:
                    keyCode = player_4_key;
                    playerNumber = "Player 4";
                    break;
            }

            keyBinder = new(keyCode, new Vector2(num3, position.y), new Vector2(15, 15));
            tab.AddItems((UIelement[])(object)new UIelement[1] { keyBinder });

            num3 += 40;

            playerNumberLabel = new(default, default, playerNumber, (FLabelAlignment)1, false, null);
            ((UIelement)playerNumberLabel).pos = new Vector2(num3, position.y + 5f);
            playerNumberLabel.size = new Vector2(num2, font_height);
            tab.AddItems((UIelement[])(object)new UIelement[1] { playerNumberLabel });

            num3 += 100;
        }
    }

    void DrawCheckBoxAndSliderCombo(ref OpTab tab)
    {
        if (check_box_configurables.Count != check_boxes_text_labels.Count)
        {
            return;
        }
        float num = margin_x.y - margin_x.x;
        float num2 = (num - (number_of_check_boxes - 1) * 0.5f * spacing) / number_of_check_boxes;
        position.y -= check_box_size;
        float num3 = position.x;
        for (int i = 0; i < check_box_configurables.Count; i++)
        {
            Configurable<bool> val = check_box_configurables[i];
            OpCheckBox val2 = new(val, new Vector2(num3, position.y))
            {
                description = (val.info?.description ?? "")
            };
            tab.AddItems((UIelement[])(object)new UIelement[1] { val2 });
            num3 += Check_Box_With_Spacing;
            OpLabel val3 = check_boxes_text_labels[i];
            ((UIelement)val3).pos = new Vector2(num3, position.y + 2f);
            val3.size = new Vector2(num2 - Check_Box_With_Spacing, font_height);
            tab.AddItems((UIelement[])(object)new UIelement[1] { val3 });
        }
        check_box_configurables.Clear();
        check_boxes_text_labels.Clear();

        if (slider_configurables.Count != slider_main_text_labels.Count || slider_configurables.Count != slider_text_labels_left.Count || slider_configurables.Count != slider_text_labels_right.Count)
        {
            return;
        }
        num = margin_x.y - margin_x.x;
        num2 = margin_x.x + 0.7f * num;
        num3 = 0.2f * num;
        float num4 = num - 2f * num3 - 20;
        for (int i = 0; i < slider_configurables.Count; i++)
        {
            //AddNewLine(2f);
            OpLabel val = slider_text_labels_left[i];
            ((UIelement)val).pos = new Vector2(margin_x.x + 90f, position.y);
            val.size = new Vector2(num3, font_height);
            tab.AddItems((UIelement[])(object)new UIelement[1] { val });
            Configurable<int> val2 = slider_configurables[i];
            OpSlider val3 = new(val2, new Vector2(num2 - 0.5f * num4, position.y - 5f), (int)num4, false)
            {
                size = new Vector2(num4, font_height),
                description = (val2.info?.description ?? "")
            };
            tab.AddItems((UIelement[])(object)new UIelement[1] { val3 });
            val = slider_text_labels_right[i];
            ((UIelement)val).pos = new Vector2(num2 + 0.5f * num4 + 0.5f * 20, position.y);
            val.size = new Vector2(num3, font_height);
            tab.AddItems((UIelement[])(object)new UIelement[1] { val });
        }
        slider_configurables.Clear();
        slider_main_text_labels.Clear();
        slider_text_labels_left.Clear();
        slider_text_labels_right.Clear();
    }

    void AddSlider(Configurable<int> configurable, string text = null, string sliderTextLeft = "", string sliderTextRight = "")
    {
        slider_configurables.Add(configurable);

        text ??= (string)configurable.info.Tags[0];

        slider_main_text_labels.Add(text);
        slider_text_labels_left.Add(new OpLabel(default, default, sliderTextLeft, (FLabelAlignment)2, false, null));
        slider_text_labels_right.Add(new OpLabel(default, default, sliderTextRight, (FLabelAlignment)1, false, null));
    }

    void DrawSliders(ref OpTab tab)
    {
        if (slider_configurables.Count != slider_main_text_labels.Count || slider_configurables.Count != slider_text_labels_left.Count || slider_configurables.Count != slider_text_labels_right.Count)
        {
            return;
        }
        float num = margin_x.y - margin_x.x;
        float num2 = margin_x.x + 0.5f * num;
        float num3 = 0.2f * num;
        float num4 = num - 2f * num3 - spacing;
        for (int i = 0; i < slider_configurables.Count; i++)
        {
            AddNewLine(1.5f);
            OpLabel val = slider_text_labels_left[i];
            ((UIelement)val).pos = new Vector2(margin_x.x, position.y + 5f);
            val.size = new Vector2(num3, font_height);
            tab.AddItems((UIelement[])(object)new UIelement[1] { val });
            Configurable<int> val2 = slider_configurables[i];
            OpSlider val3 = new(val2, new Vector2(num2 - 0.5f * num4, position.y), (int)num4, false)
            {
                size = new Vector2(num4, font_height),
                description = (val2.info?.description ?? "")
            };
            tab.AddItems((UIelement[])(object)new UIelement[1] { val3 });
            val = slider_text_labels_right[i];
            ((UIelement)val).pos = new Vector2(num2 + 0.5f * num4 + 0.5f * spacing, position.y + 5f);
            val.size = new Vector2(num3, font_height);
            tab.AddItems((UIelement[])(object)new UIelement[1] { val });
            AddTextLabel(slider_main_text_labels[i], 0);
            DrawTextLabels(ref tab);
            if (i < slider_configurables.Count - 1)
            {
                AddNewLine();
            }
        }
        slider_configurables.Clear();
        slider_main_text_labels.Clear();
        slider_text_labels_left.Clear();
        slider_text_labels_right.Clear();
    }

    void AddUpdateAbstractedSlider(ref OpTab tab)
    {
        slider_configurables.Add(update_abstracted);

        string text = (string)update_abstracted.info.Tags[0];

        slider_main_text_labels.Add(text);
        slider_text_labels_left.Add(new OpLabel(default, default, "Never", (FLabelAlignment)2, false, null));
        slider_text_labels_right.Add(new OpLabel(default, default, "2 Minutes", (FLabelAlignment)1, false, null));

        float num = margin_x.y - margin_x.x;
        float num2 = margin_x.x + 0.5f * num;
        float num3 = 0.2f * num;
        float num4 = num - 2f * num3 - spacing;
        for (int i = 0; i < slider_configurables.Count; i++)
        {
            AddNewLine(1.5f);
            OpLabel val = slider_text_labels_left[i];
            ((UIelement)val).pos = new Vector2(margin_x.x, position.y + 5f);
            val.size = new Vector2(num3, font_height);
            tab.AddItems((UIelement[])(object)new UIelement[1] { val });
            Configurable<int> val2 = slider_configurables[i];
            OPupdateAbstractedSlider = new(val2, new Vector2(num2 - 0.5f * num4, position.y), (int)num4, false)
            {
                size = new Vector2(num4, font_height),
                description = (val2.info?.description ?? "")
            };
            tab.AddItems((UIelement[])(object)new UIelement[1] { OPupdateAbstractedSlider });
            val = slider_text_labels_right[i];
            ((UIelement)val).pos = new Vector2(num2 + 0.5f * num4 + 0.5f * spacing, position.y + 5f);
            val.size = new Vector2(num3, font_height);
            tab.AddItems((UIelement[])(object)new UIelement[1] { val });
            AddTextLabel(slider_main_text_labels[i], 0);
            DrawTextLabels(ref tab);
            if (i < slider_configurables.Count - 1)
            {
                AddNewLine();
            }
        }
        slider_configurables.Clear();
        slider_main_text_labels.Clear();
        slider_text_labels_left.Clear();
        slider_text_labels_right.Clear();
    }

    void AddTextLabel(string text, FLabelAlignment alignment = 0, bool bigText = false)
    {
        float num = (bigText ? 2f : 1f) * font_height;
        if (text_labels.Count == 0)
        {
            position.y -= num;
        }
        OpLabel item = new(default, new Vector2(20f, num), text, alignment, bigText, null)
        {
            autoWrap = true
        };
        text_labels.Add(item);
    }

    void DrawTextLabels(ref OpTab tab)
    {
        if (text_labels.Count == 0)
        {
            return;
        }
        float num = (margin_x.y - margin_x.x) / text_labels.Count;
        foreach (OpLabel text_label in text_labels)
        {
            text_label.pos = position;
            text_label.size += new Vector2(num - 20f, 0f);
            tab.AddItems((UIelement[])(object)new UIelement[1] { text_label });
            position.x += num;
        }
        position.x = margin_x.x;
        text_labels.Clear();
    }
    #endregion

    public static Configurable<bool> debug_keys;
    public static Configurable<bool> debug_logs;

    public static Configurable<bool> karma_flower;
    public static Configurable<bool> karma_flower_reinforced;

    public static Configurable<int> after_cycle_length;

    public static Configurable<int> update_abstracted;
    public OpSlider OPupdateAbstractedSlider;

    public static Configurable<string> downed_tend;
    public static Configurable<string> downed_combat;

    public static Configurable<KeyCode> player_1_key;
    public static Configurable<KeyCode> player_2_key;
    public static Configurable<KeyCode> player_3_key;
    public static Configurable<KeyCode> player_4_key;

    #region Player Specific
    public static Configurable<string> coop_dead_saving;

    public static Configurable<bool> player_uncon_movement;
    #endregion
}
internal class ComboBox : OpComboBox
{
    public const int BG_SPRITE_INDEX_RANGE = 9;

    public ComboBox(Configurable<string> config, Vector2 pos, float width, string[] array) : base(config, pos, width, array)
    {
    }

    public ComboBox(Configurable<string> config, Vector2 pos, float width, List<ListItem> list) : base(config, pos, width, list)
    {
    }

    public override void GrafUpdate(float timeStacker)
    {
        base.GrafUpdate(timeStacker);
        if (_rectList != null && !_rectList.isHidden)
        {
            myContainer.MoveToFront();

            for (int i = 0; i < BG_SPRITE_INDEX_RANGE; i++)
            {
                _rectList.sprites[i].alpha = 1;
            }
        }
    }
}