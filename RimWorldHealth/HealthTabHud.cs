using HUD;
using System.Collections.Generic;
using UnityEngine;
using RWCustom;
using System.Linq;
using System;

using static ShadowOfRimWorldHealth.RimWorldHealth;

namespace ShadowOfRimWorldHealth;

public class HealthTab : HudPart
{
    public HealthTab(HUD.HUD hud, AbstractCreature owner) : base(hud)
    {
        if (owner != null)
        {
            player = owner;

            playerState = healthState.TryGetValue(player.state, out RimState state) ? state : null;
        }

        sprites = new FSprite[2];
        sprites[0] = new FSprite("pixel", true)
        {
            x = DrawPos().x,
            y = DrawPos().y,
            scaleX = 350,
            scaleY = 250,
            color = Color.gray
        };
        hud.fContainers[1].AddChild(sprites[0]);
        sprites[1] = new FSprite("pixel", true)
        {
            x = DrawPos().x - 250,
            y = DrawPos().y,
            scaleX = 150,
            scaleY = 250,
            color = Color.black
        };
        hud.fContainers[1].AddChild(sprites[1]);

        allowedMedicine = new FSprite("pixel", true)
        {
            x = DrawPos().x - 300,
            y = DrawPos().y - 116,
            scaleX = 14,
            scaleY = 14
        };
        hud.fContainers[1].AddChild(allowedMedicine);

        capacityName = new(Custom.GetFont(), "Name")
        {
            x = DrawPos().x - 250,
            y = DrawPos().y - 107,
            alignment = FLabelAlignment.Center,
            anchorX = 0.5f,
            anchorY = 1f
        };
        hud.fContainers[1].AddChild(capacityName);

        bloodLossPerCycle = new(Custom.GetFont(), "Blood loss")
        {
            x = DrawPos().x - 165,
            y = DrawPos().y - 105,
            alignment = FLabelAlignment.Left,
            anchorX = 0f,
            anchorY = 1f
        };
        hud.fContainers[1].AddChild(bloodLossPerCycle);

        for (int i = 0; i < 12; i++)
        {
            capacityValueNames.Add(new FLabel(Custom.GetFont(), "Capacity name"));
            capacityValueNames[i].x = DrawPos().x - 320;
            capacityValueNames[i].alignment = FLabelAlignment.Left;
            capacityValueNames[i].anchorX = 0f;
            capacityValueNames[i].anchorY = 1f;
            hud.fContainers[1].AddChild(capacityValueNames[i]);

            capacityValues.Add(new FLabel(Custom.GetFont(), "Stat value"));
            capacityValues[i].x = DrawPos().x - 180;
            capacityValues[i].alignment = FLabelAlignment.Left;
            capacityValues[i].anchorX = 1f;
            capacityValues[i].anchorY = 1f;
            hud.fContainers[1].AddChild(capacityValues[i]);
        }

        selectedSprite = new FSprite("pixel", true);
        hud.fContainers[1].AddChild(selectedSprite);

        afflictionsAboveLabel = new(Custom.GetFont(), "Aflictions above: 0")
        {
            x = DrawPos().x + 170,
            y = DrawPos().y + 125,
            alignment = FLabelAlignment.Right,
            anchorX = 1f,
            anchorY = 1f
        };
        hud.fContainers[1].AddChild(afflictionsAboveLabel);

        afflictionsBelowLabel = new(Custom.GetFont(), "Aflictions below: 0")
        {
            x = DrawPos().x + 170,
            y = DrawPos().y - 105,
            alignment = FLabelAlignment.Right,
            anchorX = 1f,
            anchorY = 1f
        };
        hud.fContainers[1].AddChild(afflictionsBelowLabel);

        overviewLabel = new(Custom.GetFont(), "Overview")
        {
            x = DrawPos().x - 287.5f,
            y = DrawPos().y + 120,
            alignment = FLabelAlignment.Center,
            anchorX = 0.5f,
            anchorY = 1f
        };
        hud.fContainers[1].AddChild(overviewLabel);

        operationsLabel = new(Custom.GetFont(), "Operations")
        {
            x = DrawPos().x - 212.5f,
            y = DrawPos().y + 120,
            alignment = FLabelAlignment.Center,
            anchorX = 0.5f,
            anchorY = 1f
        };
        hud.fContainers[1].AddChild(operationsLabel);

        addBillSprite = new FSprite("pixel", true)
        {
            x = DrawPos().x - 260,
            y = DrawPos().y + 82,
            scaleX = 120,
            scaleY = 20,
            color = Color.green
        };
        hud.fContainers[1].AddChild(addBillSprite);

        addBillLabel = new(Custom.GetFont(), "Add bill")
        {
            x = DrawPos().x - 260,
            y = DrawPos().y + 90,
            alignment = FLabelAlignment.Center,
            anchorX = 0.5f,
            anchorY = 1f
        };
        hud.fContainers[1].AddChild(addBillLabel);

        operationBackgrounds = new FSprite[4];
        for (int i = 0; i < operationBackgrounds.Length; i++)
        {
            operationBackgrounds[i] = new FSprite("pixel", true)
            {
                x = DrawPos().x - 255,
                y = DrawPos().y + 45 - (45 * i),
                scaleX = 132,
                scaleY = 40,
                color = Color.grey
            };
            hud.fContainers[1].AddChild(operationBackgrounds[i]);
        }

        for (int i = 0; i < 4; i++)
        {
            operationNames.Add(new FLabel(Custom.GetFont(), "Operation " + i));
            operationNames[i].x = DrawPos().x - 305;
            operationNames[i].y = DrawPos().y + 65 - (45 * i);
            operationNames[i].alignment = FLabelAlignment.Left;
            operationNames[i].anchorX = 0f;
            operationNames[i].anchorY = 1f;
            hud.fContainers[1].AddChild(operationNames[i]);
        }

        operationReorders = new FSprite[6];
        for (int i = 0; i < operationReorders.Length; i++)
        {
            float yOffset = i switch
            {
                1 => 25,
                2 => 45,
                3 => 70,
                4 => 90,
                5 => 115,
                _ => 0,
            };

            operationReorders[i] = new FSprite("pixel", true)
            {
                x = DrawPos().x - 313,
                y = DrawPos().y + 35 - yOffset,
                scale = 14,
                color = Color.yellow
            };
            hud.fContainers[1].AddChild(operationReorders[i]);
        }

        operationSuspends = new FSprite[4];
        for (int i = 0; i < operationSuspends.Length; i++)
        {
            operationSuspends[i] = new FSprite("pixel", true)
            {
                x = DrawPos().x - 197,
                y = DrawPos().y + 35 - (45 * i),
                scale = 14,
                color = Color.red
            };
            hud.fContainers[1].AddChild(operationSuspends[i]);
        }

        operationDeletes = new FSprite[4];
        for (int i = 0; i < operationDeletes.Length; i++)
        {
            operationDeletes[i] = new FSprite("pixel", true)
            {
                x = DrawPos().x - 197,
                y = DrawPos().y + 55 - (45 * i),

                scale = 14,
                color = Color.black
            };
            hud.fContainers[1].AddChild(operationDeletes[i]);
        }

        for (int i = 0; i < 4; i++)
        {
            operationSuspendLabels.Add(new FLabel(Custom.GetFont(), "SUSPENDED"));
            operationSuspendLabels[i].x = DrawPos().x - 255;
            operationSuspendLabels[i].y = DrawPos().y + 53 - (45 * i);
            operationSuspendLabels[i].alignment = FLabelAlignment.Center;
            operationSuspendLabels[i].anchorX = 0.5f;
            operationSuspendLabels[i].anchorY = 1f;
            operationSuspendLabels[i].scale = 1.2f;
            hud.fContainers[1].AddChild(operationSuspendLabels[i]);
        }

        treatedSprite = new FSprite("pixel", true)
        {
            anchorX = 0f,
            color = Color.yellow
        };
        hud.fContainers[1].AddChild(treatedSprite);

        healthTabWholeBody = new HealthTabWholeBody(this);
    }

    public Vector2 DrawPos()
    {
        if (!singleplayerHud)
        {
            int playerNumber = ((PlayerState)player.state).playerNumber;

            switch (playerNumber)
            {
                case 0:
                    return new Vector2(325, hud.rainWorld.screenSize.y - 125) + new Vector2(0.1f, 0.1f);
                case 1:
                    return new Vector2(hud.rainWorld.screenSize.x - 175, hud.rainWorld.screenSize.y - 125) + new Vector2(0.1f, 0.1f);
                case 2:
                    return new Vector2(325, 125) + new Vector2(0.1f, 0.1f);
                case 3:
                    return new Vector2(hud.rainWorld.screenSize.x - 175, 125) + new Vector2(0.1f, 0.1f);
            }
        }

        return new Vector2(hud.rainWorld.screenSize.x / 2, hud.rainWorld.screenSize.y / 2) + new Vector2(0.1f, 0.1f);
    }

    public override void Update()
    {
        base.Update();

        if (player == null || player.realizedCreature == null || !player.realizedCreature.dead && player.realizedCreature.Stunned)
        {
            visible = false;
        }

        if (visible && inspectedCreatureState.creature != null && player != inspectedCreatureState.creature)
        {
            bool stillHoolding = false;

            if (inspectedCreatureState.creature.realizedCreature != null)
            {
                foreach (Creature.Grasp grasp in player.realizedCreature.grasps)
                {
                    if (grasp != null && grasp.grabbedChunk != null && grasp.grabbedChunk.owner == inspectedCreatureState.creature.realizedCreature)
                    {
                        stillHoolding = true;
                    }
                }
            }

            if (!stillHoolding)
            {
                ToggleVisibility();
            }
        }

        if (!singleplayerHud && visible)
        {
            transparencyTimer++;

            if (transparencyTimer >= 5)
            {
                transparencyTimer = 0;

                float cameraPositionX = player.realizedCreature.room.game.cameras[0].pos.x;
                float cameraPositionY = player.realizedCreature.room.game.cameras[0].pos.y;

                bool forceTransparent = false;

                List<AbstractCreature> list = new(player.realizedCreature.abstractCreature.Room.creatures);
                foreach (AbstractCreature creature in list)
                {
                    if (creature.realizedCreature == null || creature == player || creature == inspectedCreatureState.creature)
                    {
                        continue;
                    }

                    float creaturePositionX = creature.pos.Tile.x * 20;
                    float creaturePositionY = creature.pos.Tile.y * 20;

                    float checkPositionX = 1366 + cameraPositionX - creaturePositionX;
                    float checkPositionY = 768 + cameraPositionY - creaturePositionY;

                    if (TransparencyCheck(checkPositionX, checkPositionY))
                    {
                        forceTransparent = true;
                        break;
                    }
                }

                transparent = forceTransparent;
            }
        }
        else
        {
            transparent = false;
        }

        #region Clearing add-ons
        for (int i = healthTabBodyParts.Count - 1; i >= 0; i--)
        {
            if (!visible || healthTabBodyParts[i].slatedForDeletion || inspectedState == null)
            {
                healthTabBodyParts[i].ClearSprites();
                healthTabBodyParts.RemoveAt(i);
            }
        }
        for (int i = healthTabInfos.Count - 1; i >= 0; i--)
        {
            if (!visible || healthTabInfos[i].slatedForDeletion || inspectedState == null)
            {
                healthTabInfos[i].ClearSprites();
                healthTabInfos.RemoveAt(i);
            }
        }
        for (int i = operationList.Count - 1; i >= 0; i--)
        {
            if (!visible || operationList[i].slatedForDeletion || !isOperationListActive)
            {
                operationList[i].ClearSprites();
                operationList.RemoveAt(i);
            }
        }
        #endregion

        #region Updates
        healthTabWholeBody.Update();

        foreach (HealthTabBodyPart healthTabBodyPart in healthTabBodyParts)
        {
            healthTabBodyPart.Update();
        }

        foreach (HealthTabInfo healthTabInfo in healthTabInfos)
        {
            healthTabInfo.Update();
        }

        foreach (OperationSelection operationSelection in operationList)
        {
            operationSelection.Update();
        }
        #endregion

        if (player == null || inspectedState == null || !visible)
        {
            return;
        }

        foreach (RimBodyPart part in inspectedState.bodyParts)
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
            if (horizontalOnce)
            {
                goto skipHorizontal;
            }

            horizontalOnce = true;

            bool positiveInput = input.x > 0;

            if (isOperationListActive)
            {
                if (operationListMaxHorizontal == 0)
                {
                    goto skipHorizontal;
                }

                operationListHorizontal += positiveInput ? 1 : -1;

                if (operationListHorizontal > operationListMaxHorizontal)
                {
                    operationListHorizontal = 0;
                }
                if (operationListHorizontal < 0 || operationListHorizontal == operationListMaxHorizontal)
                {
                    operationListHorizontal = operationListMaxHorizontal;

                    if (operationListVertical >= operationListMaxVerticalLast)
                    {
                        operationListVertical = operationListMaxVerticalLast - 1;
                    }
                }

                goto skipHorizontal;
            }

            bool afflictionActive = healthTabBodyParts.Count + healthTabWholeBody.afflictionNumber != 0;

            if (!afflictionActive && (selectedHorizontal == 0 && selectedVertical != 0 || selectedHorizontal == 1 && selectedVertical == 1))
            {
                goto skipHorizontal;
            }

            if (selectedHorizontal == 0 && selectedVertical != 0)
            {
                selectedHorizontal = 2;

                if (selectedVertical != -1)
                {
                    selectedVertical -= 1;
                }
            }
            else if (selectedHorizontal == 2)
            {
                selectedHorizontal = overviewTab;

                if (selectedVertical != -1)
                {
                    selectedVertical += 1;
                }
            }
            else if (selectedHorizontal != 0 && selectedHorizontal != 2)
            {
                if (selectedHorizontal == 0.9f)
                {
                    if (positiveInput)
                    {
                        selectedHorizontal = 1;

                        selectedVertical = selectedVertical switch
                        {
                            <= 2 => 2,
                            <= 4 => 3,
                            <= 6 => 4,
                            _ => 5
                        };
                    }
                    else
                    {
                        if (afflictionActive)
                        {
                            selectedHorizontal = 2;

                            selectedVertical = selectedVertical switch
                            {
                                <= 2 => 1,
                                <= 4 => 2,
                                <= 6 => 3,
                                _ => 4
                            };
                        }
                        else
                        {
                            selectedHorizontal = 1.1f;

                            selectedVertical = selectedVertical switch
                            {
                                <= 2 => 2,
                                <= 4 => 4,
                                <= 6 => 6,
                                _ => 8
                            };
                        }
                    }
                }
                else if (selectedHorizontal == 1)
                {
                    if (selectedVertical < 2)
                    {
                        if (selectedVertical == 1)
                        {
                            if (afflictionActive)
                            {
                                selectedHorizontal = 2;
                            }
                        }
                        else
                        {
                            selectedHorizontal += positiveInput ? 1 : -1;
                        }

                        if (selectedVertical != -1)
                        {
                            if (selectedHorizontal == 2 && selectedVertical > 0)
                            {
                                selectedVertical -= 1;
                            }
                            else if (selectedVertical != 0)
                            {
                                selectedVertical += 1;
                            }
                        }
                    }
                    else if (positiveInput)
                    {
                        selectedHorizontal = 1.1f;

                        selectedVertical = selectedVertical switch
                        {
                            2 => 2,
                            3 => 4,
                            4 => 6,
                            _ => 8
                        };
                    }
                    else
                    {
                        if (inspectedState.operationsList.Count == 1)
                        {
                            if (afflictionActive)
                            {
                                selectedHorizontal = 2;

                                selectedVertical = 1;
                            }
                            else
                            {
                                selectedHorizontal = 1.1f;

                                selectedVertical = selectedVertical switch
                                {
                                    2 => 2,
                                    3 => 4,
                                    4 => 6,
                                    _ => 8
                                };
                            }
                        }
                        else
                        {
                            selectedHorizontal = 0.9f;

                            selectedVertical = selectedVertical switch
                            {
                                2 => 2,
                                3 => 3,
                                4 => 5,
                                5 => 7,
                                _ => 2
                            };
                        }
                    }
                }
                else
                {
                    if (positiveInput)
                    {
                        if (afflictionActive)
                        {
                            selectedHorizontal = 2;

                            selectedVertical = selectedVertical switch
                            {
                                <= 3 => 1,
                                <= 5 => 2,
                                <= 7 => 3,
                                _ => 5
                            };
                        }
                        else
                        {
                            selectedHorizontal = 0.9f;

                            selectedVertical = selectedVertical switch
                            {
                                <= 3 => 2,
                                <= 5 => 3,
                                <= 7 => 5,
                                _ => 7
                            };
                        }
                    }
                    else
                    {
                        selectedHorizontal = 1;

                        selectedVertical = selectedVertical switch
                        {
                            <= 3 => 2,
                            <= 5 => 3,
                            <= 7 => 4,
                            _ => 5
                        };
                    }
                }
            }
            else
            {
                selectedHorizontal += positiveInput ? 1 : -1;

                if (selectedVertical != -1)
                {
                    if (selectedHorizontal == 2 && selectedVertical > 0)
                    {
                        selectedVertical -= 1;
                    }
                    else if (selectedVertical != 0)
                    {
                        selectedVertical += 1;
                    }
                }
            }

            selectedTimer = selectedTimerMax;

            if (selectedHorizontal > 2 || !afflictionActive && selectedHorizontal > 1.5f)
            {
                selectedHorizontal = 0;
            }
            else if (selectedHorizontal < 0)
            {
                if (afflictionActive)
                {
                    selectedHorizontal = 2;
                }
                else
                {
                    if (selectedVertical == 0)
                    {
                        selectedHorizontal = 1;
                    }
                    else
                    {
                        selectedHorizontal = overviewTab;
                    }
                }
            }

            if (selectedVertical != -1)
            {
                int maxVertical = getMaxVertical();

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
                else if (selectedVertical < 0)
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

        if (selectedHorizontal == 2 && healthTabBodyParts.Count + healthTabWholeBody.afflictionNumber == 0)
            selectedHorizontal = overviewTab;

        if (input.y != 0)
        {
            if (verticalOnce)
            {
                goto skipVertical;
            }

            bool positiveInput = input.y > 0;

            selected = true;

            selectedTimer = selectedTimerMax;

            verticalOnce = true;

            if (isOperationListActive)
            {
                operationListVertical += positiveInput ? -1 : 1;

                if (operationListVertical >= operationListMaxVertical || operationListHorizontal == operationListMaxHorizontal && operationListVertical >= operationListMaxVerticalLast)
                {
                    operationListVertical = 0;

                    if (operationListMaxHorizontal == 0)
                    {
                        goto skipVertical;
                    }

                    operationListHorizontal += 1;

                    if (operationListHorizontal > operationListMaxHorizontal)
                    {
                        operationListHorizontal = 0;
                    }
                }
                else if (operationListVertical < 0)
                {
                    operationListVertical = operationListMaxVertical - 1;

                    if (operationListMaxHorizontal == 0)
                    {
                        goto skipVertical;
                    }

                    operationListHorizontal -= 1;

                    if (operationListHorizontal < 0)
                    {
                        operationListHorizontal = operationListMaxHorizontal;

                        if (operationListVertical >= operationListMaxVerticalLast)
                        {
                            operationListVertical = operationListMaxVerticalLast - 1;
                        }
                    }
                }

                goto skipVertical;
            }

            if (selectedHorizontal != 2 && selectedVertical == 0)
            {
                selectedHorizontal = overviewTab;
            }

            if (selectedVertical == -1 && !positiveInput && selectedHorizontal != 2)
            {
                selectedVertical = 1;
            }
            else
            {
                selectedVertical += positiveInput ? -1 : 1;
            }

            int maxVertical = getMaxVertical();

            if (maxVertical == -1)
            {
                maxVertical = healthTabWholeBody.active ? healthTabWholeBody.afflictionNumber : 0;
                for (int i = 0; i < healthTabBodyParts.Count; i++)
                {
                    maxVertical += healthTabBodyParts[i].combinedAfflictions.Count + healthTabBodyParts[i].afflictions.Count;
                }
            }

            if ((selectedHorizontal == 0.9f || selectedHorizontal == 1.1f) && selectedVertical < 2)
            {
                selectedHorizontal = 1;
            }

            if (selectedVertical >= maxVertical)
            {
                if (selectedHorizontal != 0 || selectedVertical != maxVertical)
                {
                    selectedVertical = selectedHorizontal != 2 && !positiveInput ? 1 : 0;

                    if (selectedHorizontal == 0.9f || selectedHorizontal == 1.1f)
                    {
                        selectedHorizontal = 1;
                    }
                }
            }
            else if (selectedVertical <= -1)
            {
                selectedVertical = maxVertical - 1;

                if (selectedHorizontal == 0.9f || selectedHorizontal == 1.1f)
                {
                    selectedHorizontal = 1;
                }
            }
        }
        else
        {
            verticalOnce = false;
        }

    skipVertical:

        if (selectedHorizontal == 2)
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

        if (input.thrw)
        {
            if (cancelOnce)
            {
                goto skipCancel;
            }

            cancelOnce = true;

            if (isOperationListActive)
            {
                isOperationListActive = false;
            }
            else
            {
                ToggleVisibility();
                return;
            }
        }
        else
        {
            cancelOnce = false;
        }

    skipCancel:

        if (input.jmp || input.pckp)
        {
            if (interactOnce)
            { 
                if (playerState.tendAffliction == null && ((selectedHorizontal == 0 && selectedVertical != 0) || selectedHorizontal == 2))
                {
                    goto skipToTend;
                }

                return;
            }

            interactOnce = true;

            if (isOperationListActive)
            {
                if (inspectedState.operationsList.Count > 3)
                {
                    return;
                }

                string operationName = "";
                string operationSubName = "";

                foreach (OperationSelection operation in operationList)
                {
                    if (operation.horizontal == operationListHorizontal && operation.vertical == operationListVertical)
                    {
                        operationName = operation.name;
                        operationSubName = operation.subName;
                        break;
                    }
                }

                int forLoops = operationName switch
                {
                    "Harvest" => 6,
                    "Install peg leg" => 2,
                    "Install wooden hand" => 2,
                    "Install wooden foot" => 2,
                    _ => 1
                };

                for (int i = 0; i < forLoops; i++)
                {
                    bool forceContinue = false;

                    Type partType = null;
                    string partSubName = "";

                    switch (operationName)
                    {
                        case "Harvest":
                            switch (i)
                            {
                                case 0:
                                    partType = typeof(Heart);
                                    break;
                                case 1:
                                    partType = typeof(Lung);
                                    partSubName = "Left";
                                    break;
                                case 2:
                                    partType = typeof(Lung);
                                    partSubName = "Right";
                                    break;
                                case 3:
                                    partType = typeof(Kidney);
                                    partSubName = "Left";
                                    break;
                                case 4:
                                    partType = typeof(Kidney);
                                    partSubName = "Right";
                                    break;
                                case 5:
                                    partType = typeof(Liver);
                                    break;
                            }
                            break;
                        case "Remove (tongue)":
                            partType = typeof(Tongue);
                            break;
                        case "Install wooden hand":
                            switch (i)
                            {
                                case 0:
                                    partType = typeof(Arm);
                                    partSubName = "Left";
                                    break;
                                case 1:
                                    partType = typeof(Arm);
                                    partSubName = "Right";
                                    break;
                            }
                            break;
                        case "Install wooden foot":
                            switch (i)
                            {
                                case 0:
                                    partType = typeof(Leg);
                                    partSubName = "Left";
                                    break;
                                case 1:
                                    partType = typeof(Leg);
                                    partSubName = "Right";
                                    break;
                            }
                            break;
                        case "Install denture (jaw)":
                            partType = typeof(Head);
                            break;
                    }

                    foreach (RimOperation operation in inspectedState.operationsList)
                    {
                        if (operation.name == operationName && operation.subName == operationSubName)
                        {
                            forceContinue = true;
                            break;
                        }
                    }

                    if (forceContinue)
                    {
                        continue;
                    }

                    if (partType == null)
                    {
                        //goto skipBodyPart;
                    }

                    forceContinue = true;

                    foreach (RimBodyPart part in inspectedState.bodyParts)
                    {
                        if (part.GetType() == partType && part.subName == partSubName && !IsDestroyed(part))
                        {
                            forceContinue = false;
                            break;
                        }
                    }

                    if (forceContinue)
                    {
                        continue;
                    }

                //skipBodyPart:


                }

                //"Remove bionic spine",
                //"Remove bionic stomach",
                //"Remove bionic heart",
                //"Harvest",
                //(All "Remove bionic" below are ordered by left first and right last no matter which prosthetic is installed)
                //"Remove bionic eye",
                //"Remove bionic ear",
                //"Remove bionic jaw",
                //"Remove (tongue)", (same for default and bionic tongue)
                //"Remove bionic arm",
                //"Remove wooden hand",
                //"Remove wooden foot",
                //"Remove bionic leg",
                //"Amputate",
                //"Euthanize by Cut", "Anesthetize",
                //"Install archotech eye",
                //"Install archotech arm",
                //"Install archotech leg",
                //"Install bionic eye",
                //"Install bionic arm",
                //"Install bionic leg",
                //"Install bionic spine",
                //"Install bionic heart",
                //"Install bionic stomach",
                //"Install bionic ear",
                //"Install bionic tongue",
                //"Install bionic jaw",
                //"Install peg leg",
                //"Install wooden hand",
                //"Install wooden foot",
                //"Install denture (jaw)",
                //"Install heart",
                //"Install lung",
                //"Install kidney",
                //"Install liver",
                //"Install prosthetic heart",

                List<int> medicalRequirments = new() {0, 1, 1};

                if (operationName == "Euthanize by Cut" || operationName == "Anesthetize")
                {
                    inspectedState.operationsList.Add(new RimOperation(operationName, operationSubName, null, medicalRequirments, null));
                }
                else
                {
                    string limbName = "";
                    string limbSubName = "";
                    Func<RimArtificialPartBase> replacement = null;
                    RimBodyPart operatedPart = null;

                    string nameOverride = "";

                    string operationNameOverride = "";
                    string operationSubNameOverride = "";

                    Type requiredObject = null;

                    switch (operationName)
                    {
                        case "Harvest":
                            switch (operationSubName)
                            {
                                case "(heart)":
                                    foreach (RimBodyPart part in inspectedState.bodyParts)
                                    {
                                        if (part is Heart heart)
                                        {
                                            operatedPart = part;
                                            limbName = heart.name;
                                            break;
                                        }
                                    }
                                    break;
                                case "(left lung)":
                                    foreach (RimBodyPart part in inspectedState.bodyParts)
                                    {
                                        if (part is Lung lung && lung.subName == "Left")
                                        {
                                            operatedPart = part;
                                            limbName = lung.name;
                                            limbSubName = lung.subName;
                                            break;
                                        }
                                    }
                                    break;
                                case "(right lung)":
                                    foreach (RimBodyPart part in inspectedState.bodyParts)
                                    {
                                        if (part is Lung lung && lung.subName == "Right")
                                        {
                                            operatedPart = part;
                                            limbName = lung.name;
                                            limbSubName = lung.subName;
                                            break;
                                        }
                                    }
                                    break;
                                case "(left kidney)":
                                    foreach (RimBodyPart part in inspectedState.bodyParts)
                                    {
                                        if (part is Kidney kidney && kidney.subName == "Left")
                                        {
                                            operatedPart = part;
                                            limbName = kidney.name;
                                            limbSubName = kidney.subName;
                                            break;
                                        }
                                    }
                                    break;
                                case "(right kidney)":
                                    foreach (RimBodyPart part in inspectedState.bodyParts)
                                    {
                                        if (part is Kidney kidney && kidney.subName == "Right")
                                        {
                                            operatedPart = part;
                                            limbName = kidney.name;
                                            limbSubName = kidney.subName;
                                            break;
                                        }
                                    }
                                    break;
                                case "(liver)":
                                    foreach (RimBodyPart part in inspectedState.bodyParts)
                                    {
                                        if (part is Liver liver)
                                        {
                                            operatedPart = part;
                                            limbName = liver.name;
                                            break;
                                        }
                                    }
                                    break;
                            }
                            break;
                        case "Remove (tongue)":
                            foreach (RimBodyPart part in inspectedState.bodyParts)
                            {
                                if (part is Tongue tongue)
                                {
                                    operatedPart = part;
                                    limbName = tongue.name;
                                    break;
                                }
                            }
                            break;
                        case "Amputate":

                            break;
                        case "Install archotech eye":
                            medicalRequirments = new() { 5, 2, 1 };
                            break;
                        case "Install archotech arm":
                            medicalRequirments = new() { 5, 2, 1 };
                            break;
                        case "Install archotech leg":
                            medicalRequirments = new() { 5, 2, 1 };
                            break;
                        case "Install bionic arm":
                            medicalRequirments = new() { 5, 2, 1 };
                            break;
                        case "Install peg leg":
                            switch (operationSubName)
                            {
                                case "(left leg)":
                                    foreach (RimBodyPart part in inspectedState.bodyParts)
                                    {
                                        if (part is Leg leg && leg.subName == "Left")
                                        {
                                            operatedPart = part;
                                            limbName = leg.name;
                                            break;
                                        }
                                    }
                                    break;
                                case "(right leg)":
                                    foreach (RimBodyPart part in inspectedState.bodyParts)
                                    {
                                        if (part is Leg leg && leg.subName == "Right")
                                        {
                                            operatedPart = part;
                                            limbName = leg.name;
                                            break;
                                        }
                                    }
                                    break;
                            }

                            replacement = () => new RimPegLeg(inspectedCreatureState, operatedPart);

                            medicalRequirments = new() { 3, 2, 1 };
                            requiredObject = typeof(Spear);
                            break;
                        case "Install wooden hand":
                            switch (operationSubName)
                            {
                                case "(left hand)":
                                    foreach (RimBodyPart part in inspectedState.bodyParts)
                                    {
                                        if (part is Hand hand && hand.subName == "Left")
                                        {
                                            operatedPart = part;
                                            limbName = hand.name;
                                            break;
                                        }
                                    }
                                    break;
                                case "(right hand)":
                                    foreach (RimBodyPart part in inspectedState.bodyParts)
                                    {
                                        if (part is Hand hand && hand.subName == "Right")
                                        {
                                            operatedPart = part;
                                            limbName = hand.name;
                                            break;
                                        }
                                    }
                                    break;
                            }

                            replacement = () => new RimWoodenHand(inspectedCreatureState, operatedPart);

                            medicalRequirments = new() { 3, 2, 1 };
                            break;
                        case "Install wooden foot":
                            switch (operationSubName)
                            {
                                case "(left foot)":
                                    foreach (RimBodyPart part in inspectedState.bodyParts)
                                    {
                                        if (part is Foot foot && foot.subName == "Left")
                                        {
                                            operatedPart = part;
                                            limbName = foot.name;

                                            if (foot is SlugcatPaw)
                                            {
                                                nameOverride = "paw";
                                                operationNameOverride = "Install wooden paw";
                                                operationSubNameOverride = "(left paw)";
                                            }

                                            break;
                                        }
                                    }
                                    break;
                                case "(right foot)":
                                    foreach (RimBodyPart part in inspectedState.bodyParts)
                                    {
                                        if (part is Foot foot && foot.subName == "Right")
                                        {
                                            operatedPart = part;
                                            limbName = foot.name;

                                            if (foot is SlugcatPaw)
                                            {
                                                nameOverride = "paw";
                                                operationNameOverride = "Install wooden paw";
                                                operationSubNameOverride = "(right paw)";
                                            }

                                            break;
                                        }
                                    }
                                    break;
                            }

                            replacement = () => new RimWoodenFoot(inspectedCreatureState, operatedPart, nameOverride);

                            medicalRequirments = new() { 3, 2, 1 };
                            break;
                        case "Install denture (jaw)":
                            foreach (RimBodyPart part in inspectedState.bodyParts)
                            {
                                if (part is Jaw jaw)
                                {
                                    operatedPart = part;
                                    limbName = jaw.name;
                                    break;
                                }
                            }

                            replacement = () => new RimDenture(inspectedCreatureState, operatedPart);

                            medicalRequirments = new() { 3, 2, 1 };
                            break;
                    }

                    if (operationNameOverride != "")
                    {
                        operationName = operationNameOverride;
                    }
                    if (operationSubNameOverride != "")
                    {
                        operationSubName = operationSubNameOverride;
                    }

                    inspectedState.operationsList.Add(new RimLimbReplacement(operationName, operationSubName, operatedPart, medicalRequirments, requiredObject, limbName, limbSubName, replacement));
                }

                isOperationListActive = false;
                return;
            }

            if (selectedHorizontal == 0 && selectedVertical == getMaxVertical())
            {
                inspectedState.allowedMedicine++;

                if (inspectedState.allowedMedicine > 3)
                {
                    inspectedState.allowedMedicine = 0;
                }
            }
            if ((selectedHorizontal != 0 || selectedVertical == 0) && selectedHorizontal != 2) //use this to select Overview tabs and select operation stuff, otherwise regular tending code will trigger.
            {
                if (selectedVertical == 0)
                {
                    overviewTab = selectedHorizontal == 0 ? 0 : 1;
                }
                else if (selectedVertical == 1)
                {
                    isOperationListActive = true;

                    SpawnOperationList();
                }
                else if (selectedHorizontal == 1)
                {
                    switch (selectedVertical)
                    {
                        case 2:
                            if (inspectedState.operationsList.Count < 1 || inspectedState.operationsList[0] == null || inspectedState.operationsList[0].isSuspended || playerState.tendOperation == inspectedState.operationsList[0])
                            {
                                break;
                            }

                            startOperating(inspectedState.operationsList[0]);

                            break;
                        case 3:
                            if (inspectedState.operationsList.Count < 2 || inspectedState.operationsList[1] == null || inspectedState.operationsList[1].isSuspended || playerState.tendOperation == inspectedState.operationsList[1])
                            {
                                break;
                            }

                            startOperating(inspectedState.operationsList[1]);
                            break;
                        case 4:
                            if (inspectedState.operationsList.Count < 3 || inspectedState.operationsList[2] == null || inspectedState.operationsList[2].isSuspended || playerState.tendOperation == inspectedState.operationsList[2])
                            {
                                break;
                            }

                            startOperating(inspectedState.operationsList[2]);
                            break;
                        case 5:
                            if (inspectedState.operationsList.Count < 4 || inspectedState.operationsList[3] == null || inspectedState.operationsList[3].isSuspended || playerState.tendOperation == inspectedState.operationsList[3])
                            {
                                break;
                            }

                            startOperating(inspectedState.operationsList[3]);
                            break;
                    }
                }
                else if (selectedHorizontal == 1.1f)
                {
                    switch (selectedVertical)
                    {
                        case 2:
                            if (playerState.tendOperation != null && playerState.tendOperation == inspectedState.operationsList[0])
                            {
                                playerState.tendOperation = null;
                            }

                            inspectedState.operationsList.RemoveAt(0);

                            if (inspectedState.operationsList.Count == 0)
                            {
                                selectedHorizontal = 1;
                                selectedVertical = 1;
                            }
                            break;
                        case 3:
                            inspectedState.operationsList[0].isSuspended = !inspectedState.operationsList[0].isSuspended;
                            break;
                        case 4:
                            if (playerState.tendOperation != null && playerState.tendOperation == inspectedState.operationsList[1])
                            {
                                playerState.tendOperation = null;
                            }

                            inspectedState.operationsList.RemoveAt(1);
                            selectedVertical = 3;
                            break;
                        case 5:
                            inspectedState.operationsList[1].isSuspended = !inspectedState.operationsList[1].isSuspended;
                            break;
                        case 6:
                            if (playerState.tendOperation != null && playerState.tendOperation == inspectedState.operationsList[2])
                            {
                                playerState.tendOperation = null;
                            }

                            inspectedState.operationsList.RemoveAt(2);
                            selectedVertical = 5;
                            break;
                        case 7:
                            inspectedState.operationsList[2].isSuspended = !inspectedState.operationsList[2].isSuspended;
                            break;
                        case 8:
                            if (playerState.tendOperation != null && playerState.tendOperation == inspectedState.operationsList[3])
                            {
                                playerState.tendOperation = null;
                            }

                            inspectedState.operationsList.RemoveAt(3);
                            selectedVertical = 7;
                            break;
                        case 9:
                            inspectedState.operationsList[3].isSuspended = !inspectedState.operationsList[3].isSuspended;
                            break;
                    }
                }
                else if (selectedHorizontal == 0.9f)
                {
                    List<RimOperation> list;
                    List<int> order;

                    switch (selectedVertical)
                    {
                        case <= 3:
                            list = inspectedState.operationsList;

                            if (inspectedState.operationsList.Count == 2)
                            {
                                order = new List<int> { 1, 0 };
                            }
                            else if (inspectedState.operationsList.Count == 3)
                            {
                                order = new List<int> { 1, 0, 2 };
                            }
                            else
                            {
                                order = new List<int> { 1, 0, 2, 3 };
                            }

                            inspectedState.operationsList = order.Select(i => list[i]).ToList();
                            break;
                        case <= 5:
                            list = inspectedState.operationsList;

                            if (inspectedState.operationsList.Count == 3)
                            {
                                order = new() { 0, 2, 1 };
                            }
                            else
                            {
                                order = new List<int> { 0, 2, 1, 3 };
                            }

                            inspectedState.operationsList = order.Select(i => list[i]).ToList();
                            break;
                        case <= 7:
                            list = inspectedState.operationsList;

                            order = new() { 0, 1, 3, 2 };

                            inspectedState.operationsList = order.Select(i => list[i]).ToList();
                            break;
                    }
                }

                return;
            }

        skipToTend:

            if (playerState.tendAffliction != null || (playerState.consciousState != 0 && ShadowOfOptions.downed_tend.Value == "No one") || (playerState.consciousState == 3 && !ShadowOfOptions.player_uncon_movement.Value))
            {
                return;
            }

            #region Tending
            if (inspectedState.onFire)
            {
                playerState.tendTarget = inspectedState == playerState && inspectedCreatureState != null && inspectedCreatureState.creature != null && inspectedCreatureState.creature.realizedCreature != null ? null : inspectedCreatureState.creature.realizedCreature;

                playerState.tendTime = 10;
                playerState.tendTimeMax = playerState.tendTime;
                playerState.tendAffliction = burningAffliction;   

                return;
            }

            RimInjury bleeding = null;
            RimDisease diseaseAffliction = null;
            RimInjury untendedAffliction = null;

            foreach (RimBodyPart part in inspectedState.bodyParts)
            {
                foreach (RimAffliction baseAffliction in part.afflictions)
                {
                    if (baseAffliction is not RimTendableAffliction affliction || affliction.isCharacterSpecific)
                    {
                        continue;
                    }

                    if (!affliction.isTended)
                    {
                        if (affliction is RimInjury injury)
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
                        else if (affliction is RimDisease disease && disease.timeUntilTreatment <= 0)
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
                    else if (affliction is RimDisease disease && (disease.timeUntilTreatment <= 0 || !disease.isTended))
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
                foreach (RimAffliction affliction in inspectedState.wholeBodyAfflictions)
                {
                    if (affliction is not RimDisease disease || (disease.timeUntilTreatment > 0 && disease.isTended))
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
            #endregion
        }
        else
        {
            interactOnce = false;
        }

        void startTending(RimTendableAffliction affliction)
        {
            playerState.tendOperation = null;

            playerState.tendTime = Mathf.Round(playerState.tendTimeBase / RimHealthState.MedicalTendSpeed(playerState));
            playerState.tendTimeMax = playerState.tendTime;
            playerState.tendAffliction = affliction;
        }
        void startOperating(RimOperation operation)
        {
            playerState.tendAffliction = null;

            playerState.tendTime = Mathf.Round(playerState.tendTimeBase / RimHealthState.MedicalTendSpeed(playerState));
            playerState.tendTimeMax = playerState.tendTime;
            playerState.tendOperation = operation;

            if (operation.name == "Anesthetize")
            {
                return;
            }

            RimAnesthetic anesthetic = null;

            foreach (RimAffliction affliction in inspectedState.wholeBodyAfflictions)
            {
                if (affliction is RimAnesthetic tempAnesthetic)
                {
                    anesthetic = tempAnesthetic;
                    break;
                }
            }

            int ticks = Mathf.RoundToInt(UnityEngine.Random.Range(cycleLength * 0.75f, cycleLength * 2f)) * 40 * 60;

            if (anesthetic != null)
            {
                if (anesthetic.lastAmount < ticks)
                {
                    anesthetic.lastAmount = ticks;
                }

                anesthetic.amount = anesthetic.lastAmount;
            }
            else
            {
                inspectedState.wholeBodyAfflictions.Add(new RimAnesthetic(inspectedCreatureState, null, ticks));
                inspectedState.updateCapacities = true;
            }    
        }

        void SpawnOperationList()
        {
            operationListHorizontal = 0;
            operationListVertical = 0;

            int horizontal = 0;
            int vertical = 0;

            operationListHorizontalWidth.Clear();
            operationListHorizontalWidth.Add(0);

            foreach (string possibleOperation in possibleOperations)
            {
                if (possibleOperation == "Install archotech arm" || possibleOperation == "Install bionic arm" || possibleOperation == "Amputate") //Temporary
                {
                    continue;
                }

                int forLoops = possibleOperation switch
                {
                    "Harvest" => 6,
                    "Install peg leg" => 2,
                    "Install wooden hand" => 2,
                    "Install wooden foot" => 2,
                    _ => 1
                };

                for (int i = 0; i < forLoops; i++)
                {
                    bool forceContinue = false;

                    string subName = "";
                    Type partType = null;
                    Type operationType = null;
                    string partSubName = "";

                    switch (possibleOperation)
                    {
                        case "Harvest":
                            switch (i)
                            {
                                case 0:
                                    subName = "(heart)";
                                    partType = typeof(Heart);
                                    break;
                                case 1:
                                    subName = "(left lung)";
                                    partType = typeof(Lung);
                                    partSubName = "Left";
                                    break;
                                case 2:
                                    subName = "(right lung)";
                                    partType = typeof(Lung);
                                    partSubName = "Right";
                                    break;
                                case 3:
                                    subName = "(left kidney)";
                                    partType = typeof(Kidney);
                                    partSubName = "Left";
                                    break;
                                case 4:
                                    subName = "(right kidney)";
                                    partType = typeof(Kidney);
                                    partSubName = "Right";
                                    break;
                                case 5:
                                    subName = "(liver)";
                                    partType = typeof(Liver);
                                    break;
                                default:
                                    subName = "(null)";
                                    break;
                            }
                            break;
                        case "Remove (tongue)":
                            partType = typeof(Tongue);
                            break;
                        case "Install peg leg":
                            partType = typeof(Leg);
                            switch (i)
                            {
                                case 0:
                                    subName = "(left leg)";
                                    partSubName = "Left";
                                    break;
                                case 1:
                                    subName = "(right leg)";
                                    partSubName = "Right";
                                    break;
                                default:
                                    subName = "(null)";
                                    break;
                            }

                            operationType = typeof(RimPegLeg);
                            break;
                        case "Install wooden hand":
                            partType = typeof(Hand);
                            switch (i)
                            {
                                case 0:
                                    subName = "(left hand)";
                                    partSubName = "Left";
                                    break;
                                case 1:
                                    subName = "(right hand)";
                                    partSubName = "Right";
                                    break;
                                default:
                                    subName = "(null)";
                                    break;
                            }

                            operationType = typeof(RimWoodenHand);
                            break;
                        case "Install wooden foot":
                            partType = typeof(Foot);
                            switch (i)
                            {
                                case 0:
                                    subName = "(left foot)";
                                    partSubName = "Left";
                                    break;
                                case 1:
                                    subName = "(right foot)";
                                    partSubName = "Right";
                                    break;
                                default:
                                    subName = "(null)";
                                    break;
                            }

                            operationType = typeof(RimWoodenFoot);
                            break;
                        case "Install denture (jaw)":
                            partType = typeof(Jaw);
                            operationType = typeof(RimDenture);
                            break;
                    }

                    foreach (RimOperation operation in inspectedState.operationsList)
                    {
                        if (operation.name == possibleOperation && operation.subName == subName)
                        {
                            forceContinue = true;
                            break;
                        }
                    }

                    if (forceContinue)
                    {
                        continue;
                    }

                    string nameOverride = "";

                    if (partType != null)
                    {
                        forceContinue = true;

                        foreach (RimBodyPart part in inspectedState.bodyParts)
                        {
                            if (possibleOperation == "Harvest" && part.GetType() == partType && part.subName == partSubName && !IsDestroyed(part) ||
                                possibleOperation != "Harvest" && partType.IsAssignableFrom(part.GetType()) && part.subName == partSubName && !IsSubPartDestroyed(inspectedState, part) && (operationType == null || !part.isBionic || part.afflictions.Count == 0 || part.afflictions[0].GetType() != operationType))
                            {
                                forceContinue = false;

                                if (part is SlugcatPaw)
                                {
                                    nameOverride = "Wooden Paw";

                                    if (partSubName == "Right")
                                    {
                                        nameOverride += " (right paw)";
                                    }
                                    else
                                    {
                                        nameOverride += " (left paw)";
                                    }
                                }
                                break;
                            }
                        }

                        if (forceContinue)
                        {
                            continue;
                        }
                    }

                    OperationSelection operationSelection = new(this, possibleOperation, subName, horizontal, vertical, nameOverride);

                    vertical++;
                    operationListMaxVerticalLast = vertical;

                    operationListMaxHorizontal = horizontal;

                    if (vertical >= operationListMaxVertical)
                    {
                        vertical = 0;
                        horizontal++;
                        operationListHorizontalWidth.Add(0);
                    }

                    operationList.Add(operationSelection);
                }
            }
        }

        int getMaxVertical()
        {
            if (selectedHorizontal == 2)
            {
                return -1;
            }
            else if (overviewTab == 0)
            {
                return !inspectedCreatureState.dead ? 13 : 2;
            }

            int numberOfOperations = inspectedState.operationsList.Count;
            int additional = 0;

            if (selectedHorizontal == 0.9f)
            {
                additional = numberOfOperations switch
                {
                    <= 1 => 0,
                    2 => 2,
                    3 => 4,
                    4 => 6,
                    _ => 0,
                };

                return 2 + additional;
            }
            else if (selectedHorizontal == 1)
            {
                return 2 + numberOfOperations;
            }
            else
            {
                return 2 + (numberOfOperations * 2);
            }    
        }
    }

    public override void Draw(float timeStacker)
    {
        base.Draw(timeStacker);

        #region Visibility
        foreach (FSprite sprite in sprites)
        {
            sprite.isVisible = visible;
            sprite.alpha = SetTransparency();
        }

        for (int i = 0; i < capacityValueNames.Count; i++)
        {
            capacityValueNames[i].isVisible = visible && overviewTab == 0;
            capacityValues[i].isVisible = visible && overviewTab == 0;
        }

        capacityName.isVisible = visible && overviewTab == 0;

        allowedMedicine.isVisible = visible && overviewTab == 0;

        bloodLossPerCycle.isVisible = visible && inspectedState != null && inspectedState.bloodLossPerCycle >= 1 && inspectedState.bloodLoss < 1 && inspectedCreatureState != null && !inspectedCreatureState.dead;

        selectedSprite.isVisible = visible && selected;

        treatedSprite.isVisible = visible && playerState != null && (playerState.tendAffliction != null && playerState.tendAffliction != burningAffliction || playerState.tendOperation != null && overviewTab == 1);

        afflictionsAboveLabel.isVisible = visible && afflictionsAbove > 0;

        afflictionsBelowLabel.isVisible = visible && afflictionsBelow > 0;

        overviewLabel.isVisible = visible;

        operationsLabel.isVisible = visible;

        addBillLabel.isVisible = visible && overviewTab == 1;

        addBillSprite.isVisible = visible && overviewTab == 1;

        foreach (FSprite sprite in operationBackgrounds)
        {
            sprite.isVisible = visible && overviewTab == 1;
        }

        foreach (FLabel label in operationNames)
        {
            label.isVisible = visible && overviewTab == 1;
        }

        foreach (FSprite sprite in operationReorders)
        {
            sprite.isVisible = visible && overviewTab == 1;
        }

        foreach (FSprite sprite in operationSuspends)
        {
            sprite.isVisible = visible && overviewTab == 1;
        }

        foreach (FSprite sprite in operationDeletes)
        {
            sprite.isVisible = visible && overviewTab == 1;
        }

        foreach (FLabel sprite in operationSuspendLabels)
        {
            sprite.isVisible = visible && overviewTab == 1;
        }
        #endregion

        afflictionsAbove = 0;
        afflictionsBelow = 0;

        #region Updates
        healthTabWholeBody.Draw();

        foreach (HealthTabBodyPart bodyPartTab in healthTabBodyParts)
        {
            bodyPartTab.Draw();
        }

        foreach (HealthTabInfo infoTab in healthTabInfos)
        {
            infoTab.Draw();
        }

        foreach (OperationSelection operationSelection in operationList)
        {
            operationSelection.Draw();
        }
        #endregion

        if (!visible || playerState == null || inspectedState == null || inspectedCreatureState == null)
        {
            return;
        }

        #region AfflictionLabels
        if (afflictionsAboveLabel.isVisible)
        {
            afflictionsAboveLabel.text = "Aflictions above: " + afflictionsAbove;
            afflictionsAboveLabel.alpha = SetTransparency();
        }

        if (afflictionsBelowLabel.isVisible)
        {
            afflictionsBelowLabel.text = "Aflictions below: " + afflictionsBelow;
            afflictionsBelowLabel.alpha = SetTransparency();
        }
        #endregion

        if (bloodLossPerCycle.isVisible)
        {
            float bloodLoss = cycleLength * (1 - inspectedState.bloodLoss) / (inspectedState.bloodLossPerCycle / 100);

            bloodLossPerCycle.text = "Bleeding: " + Mathf.Round(inspectedState.bloodLossPerCycle) + "%/c " + (bloodLoss > cycleLength ? "(no immediate danger)" : "(death in " + GetTimeString(bloodLoss) + ")");
            bloodLossPerCycle.alpha = SetTransparency();
        }

        #region tabLabels
        overviewLabel.color = Color.white;
        overviewLabel.alpha = SetTransparency();

        operationsLabel.color = Color.white;
        operationsLabel.alpha = SetTransparency();
        #endregion

        if (selectedVertical == 0 && selectedHorizontal != 2)
        {
            selectedSprite.isVisible = false;
            selectedTimer = selectedTimerMax;

            if (selectedHorizontal == 0)
            {
                overviewLabel.color = Color.yellow;
            }
            else
            {
                operationsLabel.color = Color.yellow;
            }
        }

        if (overviewTab == 0)
        {
            allowedMedicine.color = inspectedState.allowedMedicine switch
            {
                0 => Color.white,
                1 => Color.green,
                2 => Color.blue,
                _ => Color.cyan
            };

            capacityName.text = GetCreatureName(inspectedCreatureState.creature.realizedCreature);
            capacityName.alpha = SetTransparency();

            if (!inspectedCreatureState.dead)
            {
                capacityValueNamesNames = new(12) { "Pain", "Consiousness", "Moving", "Manipulation", "Talking", "Eating", "Sight", "Hearing", "Breathing", "Blood filtrarion", "Blood pumping", "Digestion" };
            }
            else
            {
                capacityValueNamesNames = new(1) { "Pain" };
            }

            if (selectedHorizontal == 0 && selectedVertical > capacityValueNamesNames.Count + 1)
            {
                selectedVertical = capacityValueNamesNames.Count;
            }

            for (int i = 0; i < capacityValueNames.Count; i++)
            {
                if (i >= capacityValueNamesNames.Count)
                {
                    capacityValueNames[i].isVisible = false;
                    capacityValues[i].isVisible = false;
                    continue;
                }

                capacityValueNames[i].y = DrawPos().y + 95 - (17 * i);
                capacityValueNames[i].alpha = SetTransparency();
                capacityValueNames[i].text = capacityValueNamesNames[i];

                capacityValues[i].y = DrawPos().y + 95 - (17 * i);
                capacityValues[i].alpha = SetTransparency();

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
                        case < 0.01f:
                            capacityValues[i].text = "None";
                            capacityValues[i].color = Color.green;
                            break;
                        case < 0.15f:
                            capacityValues[i].text = "Minor";
                            capacityValues[i].color = Color.Lerp(Color.white, Color.black, 0.1f);
                            break;
                        case < 0.4f:
                            capacityValues[i].text = "Serious";
                            capacityValues[i].color = Color.Lerp(Color.white, Color.black, 0.4f);
                            break;
                        case < 0.8f:
                            capacityValues[i].text = "Intense";
                            capacityValues[i].color = Color.Lerp(Color.white, Color.black, 0.6f);
                            break;
                        default:
                            capacityValues[i].text = "Mind-shattering";
                            capacityValues[i].color = Color.red;
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
        }
        else
        {
            if (selectedHorizontal != 0 && selectedHorizontal != 2)
            {
                selectedSprite.isVisible = false;

                if (!isOperationListActive && (selectedHorizontal != 1 || selectedVertical < 2))
                    selectedTimer = selectedTimerMax;
            }

            #region addBill
            addBillSprite.color = selectedHorizontal == 1 && selectedVertical == 1 ? Color.magenta : Color.green;
            addBillLabel.alpha = SetTransparency();
            addBillSprite.alpha = SetTransparency();
            #endregion

            int numberOfOperations = inspectedState.operationsList.Count - 1;

            #region operations
            for (int i = 0; i < operationBackgrounds.Length; i++)
            {
                if (i > numberOfOperations || (selectedHorizontal != 1 || selectedVertical != (2 + i)) && i % 2 != 0)
                {
                    operationBackgrounds[i].isVisible = false;
                    continue;
                }

                operationBackgrounds[i].color = selectedHorizontal == 1 && selectedVertical == (2 + i) ? Color.white : Color.grey;
                operationBackgrounds[i].alpha = SetTransparency();
            }

            for (int i = 0; i < operationNames.Count; i++)
            {
                if (i > numberOfOperations)
                {
                    operationNames[i].isVisible = false;
                    continue;
                }

                string text = inspectedState.operationsList[i].name;

                if (inspectedState.operationsList[i].subName != "")
                {
                    text += " " + inspectedState.operationsList[i].subName;
                }

                operationNames[i].text = text;
                Menu.MenuLabel.WordWrapLabel(operationNames[i], 100);
                operationNames[i].alpha = SetTransparency();
            }

            for (int i = 0; i < operationReorders.Length; i++)
            {
                if (numberOfOperations <= 0 || numberOfOperations == 1 && i > 1 || numberOfOperations == 2 && i > 3)
                {
                    operationReorders[i].isVisible = false;
                    continue;
                }

                operationReorders[i].color = selectedHorizontal == 0.9f && selectedVertical == (2 + i) ? Color.magenta : Color.red;
                operationReorders[i].alpha = SetTransparency();
            }

            for (int i = 0; i < operationDeletes.Length; i++)
            {
                if (i > numberOfOperations)
                {
                    operationDeletes[i].isVisible = false;
                    continue;
                }

                int currentVertical = i switch
                {
                    0 => 2,
                    1 => 4,
                    2 => 6,
                    3 => 8,
                    _ => -1,
                };

                operationDeletes[i].color = selectedHorizontal == 1.1f && selectedVertical == currentVertical ? Color.white : Color.black;
                operationDeletes[i].alpha = SetTransparency();
            }

            for (int i = 0; i < operationSuspends.Length; i++)
            {
                if (i > numberOfOperations)
                {
                    operationSuspends[i].isVisible = false;
                    continue;
                }

                int currentVertical = i switch
                {
                    0 => 3,
                    1 => 5,
                    2 => 7,
                    3 => 9,
                    _ => -1,
                };

                operationSuspends[i].color = selectedHorizontal == 1.1f && selectedVertical == currentVertical ? Color.white : Color.red;
                operationSuspends[i].alpha = SetTransparency();
            }

            for (int i = 0; i < operationSuspendLabels.Count; i++)
            {
                if (i > numberOfOperations || !inspectedState.operationsList[i].isSuspended)
                {
                    operationSuspendLabels[i].isVisible = false;
                    continue;
                }

                operationSuspendLabels[i].alpha = SetTransparency();
            }
            #endregion
        }

        if (treatedSprite.isVisible && (playerState.tendAffliction != null || playerState.tendOperation != null))
        {
            float tendProgress = 1 - playerState.tendTime / playerState.tendTimeMax;

            treatedSprite.alpha = transparent ? 0.3f : 0.6f;

            if (playerState.tendOperation != null)
            {
                RimOperation operation = playerState.tendOperation;

                FSprite sprite = null;

                if (operation == inspectedState.operationsList[0])
                {
                    sprite = operationBackgrounds[0];
                }
                else if (operation == inspectedState.operationsList[1])
                {
                    sprite = operationBackgrounds[1];
                }
                else if (operation == inspectedState.operationsList[2])
                {
                    sprite = operationBackgrounds[2];
                }
                else if (operation == inspectedState.operationsList[3])
                {
                    sprite = operationBackgrounds[3];
                }

                treatedSprite.x = sprite.x - 66;
                treatedSprite.y = sprite.y;

                treatedSprite.scaleX = Mathf.Lerp(0, 132, tendProgress);
                treatedSprite.scaleY = 40;

                goto skipTend;
            }

            treatedSprite.x = DrawPos().x - 50;

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
                            if (healthTabBodyParts[i].combinedAfflictions.TryGetValue(healthTabBodyParts[i].CombinedAfflictionName(healthTabBodyParts[i].bodyPart, j), out List<RimAffliction> list) && list.Contains(playerState.tendAffliction))
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

    skipTend:

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
        selectedSprite.alpha = transparent ? 0.2f : 0.4f;
        selectedSprite.color = Color.white;

        selectedSprite.scaleY = 15;

        if (selectedHorizontal == 0)
        {
            if (selectedVertical == (!inspectedCreatureState.dead ? 13 : 2))
            {
                selectedSprite.x = allowedMedicine.x;
                selectedSprite.y = allowedMedicine.y;
                selectedSprite.scale = 16;

                selectedTimer = selectedTimerMax;
            }
            else
            {
                selectedSprite.x -= 250;

                selectedSprite.y += 111 - 7 - (17 * selectedVertical - 1);

                selectedSprite.scaleX = 145;

                if (selectedTimer <= 0)
                {
                    CapacitySelector();
                }
            }
        }
        else if (selectedHorizontal == 2)
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
                else if ((selectedSprite.y + (selectedSprite.scaleY / 2)) > ActualTopLimit())
                {
                    pivotOffset = healthTabWholeBody.GetPivotOffset(selectedVertical, true);
                }
                else if ((selectedSprite.y - (selectedSprite.scaleY / 2)) < ActualBottomLimit())
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
                else if ((selectedSprite.y + (selectedSprite.scaleY / 2)) > ActualTopLimit())
                {
                    pivotOffset = healthTabBodyParts[selectedBodyPart].GetPivotOffset(selectedAffliction, true);
                }
                else if ((selectedSprite.y - (selectedSprite.scaleY / 2)) < ActualBottomLimit())
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
        else if (selectedTimer <= 0)
        {
            if (isOperationListActive)
            {
                foreach (OperationSelection operation in operationList)
                {
                    if (operation.horizontal == operationListHorizontal && operation.vertical == operationListVertical)
                    {
                        selectedSprite.x = operation.backgrounds[0].x + (operation.backgrounds[0].scaleX / 2) - 3;

                        selectedSprite.y = operation.backgrounds[0].y + 12;

                        selectedSprite.scaleX = operation.backgrounds[0].scaleX;
                        break;
                    }
                }

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

                string operationName = "";

                foreach (OperationSelection operation in operationList)
                {
                    if (operation.horizontal == operationListHorizontal && operation.vertical == operationListVertical)
                    {
                        operationName = operation.name;
                        break;
                    }
                }

                int medicalRequired = 0;
                int medicineAmount = 1;
                int medicineQuality = 1;

                Type requiredObject = null;

                RimBodyPart operationPart = null;

                switch (operationName)
                {
                    case "Install archotech eye":
                        medicalRequired = 5;
                        medicineAmount = 2;
                        break;
                    case "Install archotech arm":
                        medicalRequired = 5;
                        medicineAmount = 2;
                        break;
                    case "Install archotech leg":
                        medicalRequired = 5;
                        medicineAmount = 2;
                        break;
                    case "Install bionic arm":
                        medicalRequired = 5;
                        medicineAmount = 2;
                        break;
                    case "Install peg leg":
                        medicalRequired = 3;
                        medicineAmount = 2;

                        requiredObject = typeof(Spear);
                        break;
                    case "Install wooden hand":
                        medicalRequired = 3;
                        medicineAmount = 2;
                        break;
                    case "Install wooden foot":
                        medicalRequired = 3;
                        medicineAmount = 2;
                        break;
                    case "Install denture (jaw)":
                        medicalRequired = 3;
                        medicineAmount = 2;
                        break;
                }

                if (medicalRequired != 0)
                {
                    healthTabInfos[0].name.text = "Requires: ";
                    healthTabInfos[0].nameStatus.text = "Medical " + medicalRequired;

                    healthTabInfos[0].colouredName.text = "Ingredients: ";

                    healthTabInfos[0].description.text = "  " + medicineAmount + "x ";
                    healthTabInfos[0].description.text += medicineQuality switch
                    {
                        1 => "Any Medicine",
                        2 => "Regular or better medicine",
                        _ => "Glitterworld Medicine"
                    };

                    if (requiredObject != null)
                    {
                        healthTabInfos[0].description.text += "\n  1x " + requiredObject.ToString();
                    }

                    foreach (RimBodyPart part in inspectedState.bodyParts)
                    {
                        if (part == operationPart && part.isBionic && part.afflictions.Count > 0 && part.afflictions[0] is RimArtificialPartBase artificialPart)
                        {
                            healthTabInfos[0].colouredName2.text = "Replaces: " + artificialPart.name;
                            break;
                        }
                    }
                }
                else
                {
                    healthTabInfos[0].name.text = "Ingredients: ";
                    healthTabInfos[0].nameStatus.text = "";

                    healthTabInfos[0].colouredName.text = "";

                    healthTabInfos[0].description.text = "  " + medicineAmount + "x ";
                    healthTabInfos[0].description.text += medicineQuality switch
                    {
                        1 => "Any Medicine",
                        2 => "Regular or better medicine",
                        _ => "Glitterworld Medicine"
                    };

                    if (requiredObject != null)
                    {
                        healthTabInfos[0].description.text += "\n  1x " + requiredObject.ToString();
                    }

                    healthTabInfos[0].colouredName2.text = "";

                    foreach (RimBodyPart part in inspectedState.bodyParts)
                    {
                        if (part == operationPart && part.isBionic && part.afflictions.Count > 0 && part.afflictions[0] is RimArtificialPartBase artificialPart)
                        {
                            healthTabInfos[0].colouredName2.text = "Replaces: " + artificialPart.name;
                            break;
                        }
                    }
                }
            }
            else if (selectedHorizontal == 1)
            {
                selectedSprite.x -= 258;

                selectedSprite.y = selectedVertical switch
                {
                    2 => operationBackgrounds[0].y + 27,
                    3 => operationBackgrounds[1].y + 27,
                    4 => operationBackgrounds[2].y + 27,                    5 => operationBackgrounds[3].y + 27,
                    _ => operationBackgrounds[0].y + 27
                };

                selectedSprite.scaleX = operationBackgrounds[0].scaleX;

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

                RimOperation operation = inspectedState.operationsList[selectedVertical - 2];

                if (operation.medicalRequired != 0)
                {
                    healthTabInfos[0].name.text = "Requires: ";
                    healthTabInfos[0].nameStatus.text = "Medical " + operation.medicalRequired;

                    healthTabInfos[0].colouredName.text = "Ingredients: ";

                    healthTabInfos[0].description.text = "  " + operation.medicineAmount + "x ";
                    healthTabInfos[0].description.text += operation.medicineQuality switch
                    {
                        1 => "Any Medicine",
                        2 => "Regular or better medicine",
                        _ => "Glitterworld Medicine"
                    };

                    if (operation.requiredObject != null)
                    {
                        healthTabInfos[0].description.text += "\n  1x " + operation.requiredObject.ToString();
                    }

                    foreach (RimBodyPart part in inspectedState.bodyParts)
                    {
                        if (part == operation.part && part.isBionic && part.afflictions.Count > 0 && part.afflictions[0] is RimArtificialPartBase artificialPart)
                        {
                            healthTabInfos[0].colouredName2.text = "Replaces: " + artificialPart.name;
                            break;
                        }
                    }
                }
                else
                {
                    healthTabInfos[0].name.text = "Ingredients: ";
                    healthTabInfos[0].nameStatus.text = "";

                    healthTabInfos[0].colouredName.text = "";

                    healthTabInfos[0].description.text = "  " + operation.medicineAmount + "x ";
                    healthTabInfos[0].description.text += operation.medicineQuality switch
                    {
                        1 => "Any Medicine",
                        2 => "Regular or better medicine",
                        _ => "Glitterworld Medicine"
                    };

                    if (operation.requiredObject != null)
                    {
                        healthTabInfos[0].description.text += "\n  1x " + operation.requiredObject.ToString();
                    }

                    healthTabInfos[0].colouredName2.text = "";

                    foreach (RimBodyPart part in inspectedState.bodyParts)
                    {
                        if (part == operation.part && part.isBionic && part.afflictions.Count > 0 && part.afflictions[0] is RimArtificialPartBase artificialPart)
                        {
                            healthTabInfos[0].colouredName2.text = "Replaces: " + artificialPart.name;
                            break;
                        }
                    }
                }
            }
        }

        void CapacitySelector()
        {
            if (selectedVertical == 1)
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
                    healthTabInfos[0].nameStatus.text = Mathf.Clamp(Mathf.Round((!inspectedCreatureState.dead ? inspectedState.pain : 0) * 100), 0, 100) + "%";

                    healthTabInfos[0].description.text = "";

                    healthTabInfos[0].colouredName.text = "";
                    healthTabInfos[0].colouredName2.text = "";
                }
            }
            else
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

                string stringValue = capacityValueNamesNames[selectedVertical - 1];
                string description = "\n" +
                    "\n" +
                    "Affected by:\n";

                bool isCapacityAffected = false;

                float value = 0;

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

                            foreach (RimBodyPart part in inspectedState.movingBP)
                            {
                                CheckCapacityBodypart(part);
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

                            foreach (RimBodyPart part in inspectedState.manipulationBP)
                            {
                                CheckCapacityBodypart(part);
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

                            if (inspectedCreatureState.creature.realizedCreature is Player player && player.SlugCatClass.value == "Spear")
                            {
                                isCapacityAffected = true;
                                description += "  Lack of Mouth";
                                break;
                            }

                            if (inspectedState.consciousness < 1f)
                            {
                                isCapacityAffected = true;
                                description += "  Consciousness " + Mathf.Round(inspectedState.consciousness * 100) + "%\n";
                            }

                            foreach (RimBodyPart part in inspectedState.talkingBP)
                            {
                                if (part is Tongue)
                                {
                                    bool jawIsBionic = false;

                                    foreach (RimBodyPart part2 in inspectedState.talkingBP)
                                    {
                                        if (part2 is Jaw jaw && jaw.isBionic)
                                        {
                                            jawIsBionic = true;
                                            break;
                                        }
                                    }

                                    if (jawIsBionic)
                                    {
                                        continue;
                                    }
                                }

                                CheckCapacityBodypart(part);
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

                            foreach (RimBodyPart part in inspectedState.eatingBP)
                            {
                                if (part is Tongue)
                                {
                                    bool jawIsBionic = false;

                                    foreach (RimBodyPart part2 in inspectedState.eatingBP)
                                    {
                                        if (part2 is Jaw jaw && jaw.isBionic)
                                        {
                                            jawIsBionic = true;
                                            break;
                                        }
                                    }

                                    if (jawIsBionic)
                                    {
                                        continue;
                                    }
                                }

                                CheckCapacityBodypart(part);
                            }

                            break;
                        }
                    case "Sight":
                        {
                            value = inspectedState.sight;

                            foreach (RimBodyPart part in inspectedState.sightBP)
                            {
                                CheckCapacityBodypart(part);
                            }

                            break;
                        }
                    case "Hearing":
                        {
                            value = inspectedState.hearing;

                            foreach (RimBodyPart part in inspectedState.hearingBP)
                            {
                                CheckCapacityBodypart(part);
                            }

                            break;
                        }
                    case "Breathing":
                        {
                            value = inspectedState.breathing;

                            foreach (RimBodyPart part in inspectedState.breathingBP)
                            {
                                CheckCapacityBodypart(part);
                            }

                            break;
                        }
                    case "Blood filtrarion":
                        {
                            value = inspectedState.bloodFiltration;

                            foreach (RimBodyPart part in inspectedState.bloodFiltrationBP)
                            {
                                CheckCapacityBodypart(part);
                            }

                            break;
                        }
                    case "Blood pumping":
                        {
                            value = inspectedState.bloodPumping;

                            foreach (RimBodyPart part in inspectedState.bloodPumpingBP)
                            {
                                CheckCapacityBodypart(part);
                            }

                            break;
                        }
                    case "Digestion":
                        {
                            value = inspectedState.digestion;

                            foreach (RimBodyPart part in inspectedState.digestionBP)
                            {
                                CheckCapacityBodypart(part);
                            }

                            break;
                        }
                }

                CapacityAffectingAffliction();

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
                    healthTabInfos[0].name.text = capacityValueNamesNames[selectedVertical - 1] + ": ";
                    healthTabInfos[0].nameStatus.text = stringValue;

                    healthTabInfos[0].description.text = description;

                    healthTabInfos[0].colouredName.text = "";
                    healthTabInfos[0].colouredName2.text = "";
                }

                string CapacityAffectingAffliction()
                {
                    foreach (RimAffliction affliction in inspectedState.capacityAffectingAffliction)
                    {
                        if (affliction is RimDisease disease)
                        {
                            if (disease is RimFlu)
                            {
                                if (stringValue != "Consiousness" && stringValue != "Manipulation" && stringValue != "Breathing")
                                {
                                    continue;
                                }

                                isCapacityAffected = true;

                                string text = "  " + disease.name;

                                description += disease.severity switch
                                {
                                    < 0.665f => text + " (minor)\n",
                                    < 0.832f => text + " (major)\n",
                                    _ => text + " (extreme)\n"
                                };

                                continue;
                            }
                            else if (disease is RimInfection)
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
                                else if (disease.severity >= 0.86f)
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
                        else if (affliction is RimAilment informational)
                        {
                            if (informational is RimHypothermia)
                            {
                                if (stringValue != "Consiousness" && stringValue != "Manipulation" && stringValue != "Moving")
                                {
                                    continue;
                                }

                                if (informational.amount < 0.2f)
                                {
                                    if (stringValue == "Moving")
                                    {
                                        continue;
                                    }

                                    isCapacityAffected = true;
                                    description += "  Hypothermia (shivering)\n";
                                }
                                else if (informational.amount < 0.35f)
                                {
                                    if (stringValue == "Moving")
                                    {
                                        continue;
                                    }

                                    isCapacityAffected = true;
                                    description += "  Hypothermia (minor)\n";
                                }
                                else if (informational.amount < 0.62f)
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
                            else if (informational is RimToxicBuildup)
                            {
                                if (stringValue != "Consiousness")
                                {
                                    continue;
                                }

                                isCapacityAffected = true;

                                description += informational.amount switch
                                {
                                    < 0.2f => "  Toxic buildup (initial)\n",
                                    < 0.4f => "  Toxic buildup (minor)\n",
                                    < 0.6f => "  Toxic buildup (moderate)\n",
                                    < 0.8f => "  Toxic buildup (serious)\n",
                                    _ => "  Toxic buildup (extreme)\n"
                                };

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

                void CheckCapacityBodypart(RimBodyPart part)
                {
                    if (part.isBionic && part.afflictions.Count > 0 && part.afflictions[0] is RimArtificialPartBase partBase && partBase.efficiency != 1)
                    {
                        isCapacityAffected = true;
                        description += "  " + partBase.name + "\n";
                    }
                    if (part.health < part.maxHealth)
                    {
                        isCapacityAffected = true;
                        description += "  " + part.name + ": " + Mathf.Round(part.health) + " / " + part.maxHealth + "\n";
                    }
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
                    "supply.\n\n";

                healthTabInfos[0].description.text += inspectedState.bloodLoss switch
                {
                    < 0.3f => "  - Consciousness: -10%",
                    < 0.45f => "  - Consciousness: -20%",
                    < 0.6f => "  - Consciousness: -40%",
                    _ => "  - Consciousness: -40%\n" +
                        "  - Consciousness: Max 10%"
                };

                healthTabInfos[0].colouredName.text = "";
                healthTabInfos[0].colouredName2.text = "";
            }
            else
            {
                int selectedAffliction = selectedVertical - (healthTabWholeBody.bloodLossVisible ? 1 : 0);

                if (healthTabWholeBody.afflictionVisuals.Count <= selectedAffliction || inspectedState.wholeBodyAfflictions.Count <= selectedAffliction)
                {
                    return;
                }

                healthTabInfos[0].name.text = healthTabWholeBody.afflictionVisuals[selectedAffliction].name.text;

                if (inspectedState.wholeBodyAfflictions[selectedAffliction] is RimDisease disease)
                {
                    healthTabInfos[0].name.text += ": ";

                    healthTabInfos[0].nameStatus.text = Mathf.Clamp(Mathf.Round(disease.severity * 1000) / 10, 0, 100) + "%";

                    if (disease is RimFlu)
                    {
                        healthTabInfos[0].description.text = "An infectious disease.\n\n";

                        healthTabInfos[0].description.text += disease.severity switch
                        {
                            < 0.665f => "  - Consciousness: -5%\n" +
                                "  - Manipulation: -5%\n" +
                                "  - Breathing: -10%\n",
                            < 0.832f => "  - Consciousness: -10%\n" +
                                "  - Manipulation: -10%\n" +
                                "  - Breathing: -15%\n",
                            _ => "  - Pain: +5%\n" +
                                "  - Consciousness: -15%\n" +
                                "  - Manipulation: -20%\n" +
                                "  - Breathing: -20%\n\n"
                        };

                        if (!disease.isTended)
                        {
                            healthTabInfos[0].description.text += "Needs tending now\n";
                        }
                        else
                        {
                            healthTabInfos[0].description.text += "Tend quality: " + (Mathf.Round(disease.tendQuality * 1000) / 10) + "%\n";

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
                else if (inspectedState.wholeBodyAfflictions[selectedAffliction] is RimAilment ailment)
                {
                    switch (ailment)
                    {
                        case RimAirInLungs:
                            healthTabInfos[0].name.text += ": ";

                            healthTabInfos[0].description.text = "The amount of oxygen in a creature's lungs.";

                            healthTabInfos[0].nameStatus.text = Mathf.Round(ailment.amount * 100) + "%";
                            break;
                        case RimHypothermia:
                            healthTabInfos[0].name.text += ": ";

                            healthTabInfos[0].description.text = "Dangerously low core body temperature.\n" +
                                "Unless re-warmed, hypothermia gets\n" +
                                "worse and ends in death. Recovery is\n" +
                                "quick once the victim is re-warmed.\n\n";

                            healthTabInfos[0].description.text += ailment.amount switch
                            {
                                < 0.2f => "  - Consciousness: -5%\n" +
                                    "  - Manipulation: -8%\n",
                                < 0.35f => "  - Consciousness: -10%\n" +
                                    "  - Manipulation: -20%\n" +
                                    "  - Moving: -10%\n",
                                < 0.62f => "  - Consciousness: -20%\n" +
                                    "  - Manipulation: -50%\n" +
                                    "  - Moving: -30%\n" +
                                    "  - Pain: +15%\n",
                                _ => "  - Consciousness: Max 10%\n" +
                                    "  - Pain: +30%"
                            };

                            healthTabInfos[0].nameStatus.text = Mathf.Round(ailment.amount * 100) + "%";
                            break;
                        case RimToxicBuildup:
                            healthTabInfos[0].name.text += ": ";

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
                                "higher the chance.\n\n";

                            healthTabInfos[0].description.text += ailment.amount switch
                            {
                                < 0.2f => "  - Consciousness: -5%",
                                < 0.4f => "  - Consciousness: -10%",
                                < 0.6f => "  - Consciousness: -15%",
                                < 0.8f => "  - Consciousness: -25%",
                                _ => "  - Consciousness: Max 10%\n" +
                                    "  - Consciousness: -25%"
                            };

                            healthTabInfos[0].nameStatus.text = ": " + Mathf.Round(ailment.amount * 100) + "%";
                            break;
                        case RimAnesthetic:
                            healthTabInfos[0].description.text = "Sleep drugs in the bloodstream.\n" +
                                "Anestheic renders creatures\n" +
                                "unconscious.\n\n";

                            healthTabInfos[0].description.text += ailment.amount switch
                            {
                                < 0.6f => "  - Pain: x95%\n" +
                                    "  - Consciousness: Max 90%\n" +
                                    "  - Manipulation: -10%\n" +
                                    "  - Moving: -5%",
                                < 0.8f => "  - Pain: x80%\n" +
                                    "  - Consciousness: Max 70%\n" +
                                    "  - Moving: -20%\n" +
                                    "  - Manipulation: -20%\n" +
                                    "  - Talking: -20%\n" +
                                    "  - Digestion: -20%\n" +
                                    "  - Sight: -15%\n" +
                                    "  - Affects: Memory", //does not affect memory
                                _ => "  - Pain: x0%\n" +
                                    "  - Consciousness: Max 1%"
                            };

                            healthTabInfos[0].nameStatus.text = "";
                            break;
                        default:
                            healthTabInfos[0].description.text = "The description was not set, report this is seen";
                            healthTabInfos[0].nameStatus.text = "the name status has not been set, report this if seen";
                            break;
                    }
                }

                healthTabInfos[0].colouredName.text = "";
                healthTabInfos[0].colouredName2.text = "";
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

            List<RimAffliction> afflictions = null;
            RimAffliction affliction = healthTabBodyParts[selectedBodyPart].allAfflictions[selectedAffliction];

            for (int j = 0; j < healthTabBodyParts[selectedBodyPart].combinedAfflictions.Count; j++)
            {
                if (healthTabBodyParts[selectedBodyPart].combinedAfflictions.TryGetValue(healthTabBodyParts[selectedBodyPart].CombinedAfflictionName(healthTabBodyParts[selectedBodyPart].allAfflictions, selectedAffliction), out List<RimAffliction> list) && list.Contains(affliction))
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

                    if (affliction is RimInjury combinedInjury)
                    {
                        healthTabInfos[j].name.text = combinedInjury.healingDifficulty.name + (combinedInjury.attackName != "" ? " (" + combinedInjury.attackName + ")" : "");

                        healthTabInfos[j].nameStatus.text = " : " + (Mathf.Round(combinedInjury.damage * 10) / 10).ToString();

                        string description = "";

                        if (!inspectedCreatureState.dead)
                        {
                            float bleeding = Mathf.Max(1f, Mathf.Round(combinedInjury.healingDifficulty.bleeding * combinedInjury.damage * inspectedState.bodySizeFactor * RimHealthState.BloodLossMultiplier(healthTabBodyParts[selectedBodyPart].bodyPart)));

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

            if (affliction is RimInjury injury)
            {
                if (injury is RimDestroyed)
                {
                    healthTabInfos[1].nameStatus.text = "";

                    string description = "A body part is entirely missing.";

                    if (!inspectedCreatureState.dead && !injury.isTended)
                    {
                        description += "\n\n";

                        float bleeding = Mathf.Max(1f, Mathf.Round(injury.healingDifficulty.bleeding * injury.part.maxHealth * 2 * inspectedState.bodySizeFactor * RimHealthState.BloodLossMultiplier(healthTabBodyParts[selectedBodyPart].bodyPart)));

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
                else if (injury is RimScar scar && scar.isRevealed)
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
                        float bleeding = Mathf.Max(1f, Mathf.Round(injury.healingDifficulty.bleeding * injury.damage * inspectedState.bodySizeFactor * RimHealthState.BloodLossMultiplier(healthTabBodyParts[selectedBodyPart].bodyPart)));

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
            else if (affliction is RimDisease disease)
            {
                healthTabInfos[1].nameStatus.text = " : " + (Mathf.Round(disease.severity * 1000) / 10) + "%";

                if (disease is RimInfection)
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
                    healthTabInfos[1].description.text += "Immunity: " + Mathf.Clamp(Mathf.Round(disease.immunity * 1000) / 10, 0, 100) + "%";
                }
            }  
            else if (affliction is RimArtificialPartBase artificialPart)
            {
                healthTabInfos[1].name.text = artificialPart.name;
                healthTabInfos[1].nameStatus.text = "";

                healthTabInfos[1].description.text = artificialPart.descrition;
            }

            foreach (HealthTabInfo info in healthTabInfos)
            {
                info.colouredName.text = "";
                info.colouredName2.text = "";
            }
        }
    }

    public override void ClearSprites()
    {
        base.ClearSprites();

        #region Clearing others
        healthTabWholeBody.ClearSprites();

        foreach (HealthTabBodyPart bodyPartTab in healthTabBodyParts)
        {
            bodyPartTab.ClearSprites();
        }

        foreach (HealthTabInfo infoTab in healthTabInfos)
        {
            infoTab.ClearSprites();
        }
        #endregion

        #region Sprites and Labels
        foreach (FSprite sprite in sprites)
        {
            sprite.RemoveFromContainer();
        }

        capacityName.RemoveFromContainer();

        allowedMedicine.RemoveFromContainer();

        bloodLossPerCycle.RemoveFromContainer();

        foreach (FLabel label in capacityValueNames)
        {
            label.RemoveFromContainer();
        }
        foreach (FLabel label in capacityValues)
        {
            label.RemoveFromContainer();
        }

        selectedSprite.RemoveFromContainer();

        afflictionsAboveLabel.RemoveFromContainer();
        afflictionsBelowLabel.RemoveFromContainer();

        overviewLabel.RemoveFromContainer();
        operationsLabel.RemoveFromContainer();

        addBillSprite.RemoveFromContainer();
        addBillLabel.RemoveFromContainer();

        foreach (FSprite sprite in operationBackgrounds)
        {
            sprite.RemoveFromContainer();
        }
        foreach (FLabel label in operationNames)
        {
            label.RemoveFromContainer();
        }
        foreach (FSprite sprite in operationReorders)
        {
            sprite.RemoveFromContainer();
        }
        foreach (FSprite sprite in operationSuspends)
        {
            sprite.RemoveFromContainer();
        }
        foreach (FSprite sprite in operationDeletes)
        {
            sprite.RemoveFromContainer();
        }
        foreach (FLabel label in operationSuspendLabels)
        {
            label.RemoveFromContainer();
        }

        treatedSprite.RemoveFromContainer();
        #endregion
    }

    public void ToggleVisibility(CreatureState self, RimState state)
    {
        visible = !visible && player != null;

        if (visible)
        {
            inspectedState = state ?? playerState;
            inspectedCreatureState = self ?? player.state;

            TriggeredOn();
        }
        else
        {
            if (playerState != null)
                playerState.tendAffliction = null;

            inspectedState = null;
            inspectedCreatureState = null;

            TriggeredOff();
        }
    }
    public void ToggleVisibility()
    {
        visible = false;

        if (playerState != null)
            playerState.tendAffliction = null;

        inspectedState = null;
        inspectedCreatureState = null;

        TriggeredOff();
    }

    void TriggeredOn()
    {
        selected = false;

        selectedHorizontal = 0;
        selectedVertical = -1;

        topLimit = 510;

        horizontalOnce = true;
        verticalOnce = true;

        selectedTimer = selectedTimerMax;

        if (playerState != null)
            playerState.tendAffliction = null;

        overviewTab = 0;
    }
    public void TriggeredOff()
    {
        isOperationListActive = false;

        playerState.tendAffliction = null;
        playerState.tendOperation = null;
    }

    public string ScarName(RimScar scar)
    {
        return (scar.isPermanent && scar.healingDifficulty.permanentScar != "" ? scar.healingDifficulty.permanentScar : scar.healingDifficulty.scar) + ((scar.attackName != "" || scar.painCategory != "") ? " (" + (scar.attackName != "" ? scar.attackName + (scar.painCategory != "" ? ", " + scar.painCategory : "") : scar.painCategory) + ")" : "");
    }

    public float ActualTopLimit()
    {
        return topLimit + (DrawPos().y > hud.rainWorld.screenSize.y / 2 ? Mathf.Abs((hud.rainWorld.screenSize.y / 2) - DrawPos().y) : DrawPos().y - (hud.rainWorld.screenSize.y / 2));
    }
    public float ActualBottomLimit()
    {
        return bottomLimit + (DrawPos().y > hud.rainWorld.screenSize.y / 2 ? Mathf.Abs((hud.rainWorld.screenSize.y / 2) - DrawPos().y) : DrawPos().y - (hud.rainWorld.screenSize.y / 2));
    }

    public bool TransparencyCheck(float posX, float posY)
    {
        bool top = DrawPos().y > hud.rainWorld.screenSize.y / 2;
        bool left = DrawPos().x > hud.rainWorld.screenSize.x / 2;

        if (!left && posX >= 866)
        {
            if (!top && posY >= 518)
            {
                return true;
            }
            else if (top && posY <= 250)
            {
                return true;
            }
        }
        else if (left && posX <= 500)
        {
            if (!top && posY >= 518)
            {
                return true;
            }
            else if (top && posY <= 250)
            {
                return true;
            }
        }

        return false;
    }
    
    public float SetTransparency()
    {
        return transparent ? 0.5f : 1;
    }
    
    public void OperationRemoved()
    {
        if (selectedHorizontal == 0 || selectedHorizontal == 2 || selectedVertical < 2)
        {
            return;
        }

        if (selectedHorizontal == 0.9f)
        {
            if (inspectedState.operationsList.Count == 0)
            {
                if (selectedVertical > 1)
                {
                    selectedHorizontal = 1;
                    selectedVertical = 1;
                }
            }
            else if (inspectedState.operationsList.Count == 1)
            {
                if (selectedVertical > 1)
                {
                    selectedHorizontal = 1;
                    selectedVertical = 1;
                }
            }
            else if (inspectedState.operationsList.Count == 2)
            {
                if (selectedVertical > 4)
                {
                    selectedVertical = 3;
                }
            }
            else if (inspectedState.operationsList.Count == 3)
            {
                if (selectedVertical > 6)
                {
                    selectedVertical = 5;
                }
            }
        }
        else if (selectedHorizontal == 1)
        {
            if (inspectedState.operationsList.Count == 0)
            {
                if (selectedVertical > 1)
                {
                    selectedVertical = 1;
                }
            }
            else if (inspectedState.operationsList.Count == 1)
            {
                if (selectedVertical > 2)
                {
                    selectedVertical = 2;
                }
            }
            else if (inspectedState.operationsList.Count == 2)
            {
                if (selectedVertical > 3)
                {
                    selectedVertical = 3;
                }
            }
            else if (inspectedState.operationsList.Count == 3)
            {
                if (selectedVertical > 4)
                {
                    selectedVertical = 4;
                }
            }
        }
        else if (selectedHorizontal == 1.1f)
        {
            if (inspectedState.operationsList.Count == 0)
            {
                if (selectedVertical > 1)
                {
                    selectedHorizontal = 1;
                    selectedVertical = 1;
                }
            }
            else if (inspectedState.operationsList.Count == 1)
            {
                if (selectedVertical > 3)
                {
                    selectedVertical = 3;
                }
            }
            else if (inspectedState.operationsList.Count == 2)
            {
                if (selectedVertical > 5)
                {
                    selectedVertical = 5;
                }
            }
            else if (inspectedState.operationsList.Count == 3)
            {
                if (selectedVertical > 7)
                {
                    selectedVertical = 7;
                }
            }
        }
    }

    public static string GetTimeString(float timeInMin)
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

    #region Values
    public RimState playerState;
    public AbstractCreature player;

    public RimState inspectedState;
    public CreatureState inspectedCreatureState;

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

    private readonly FLabel overviewLabel;

    private List<string> capacityValueNamesNames;

    private readonly FSprite allowedMedicine;

    #region Operations related
    private readonly FLabel operationsLabel;

    private readonly FLabel addBillLabel;
    public readonly FSprite addBillSprite;

    private readonly List<FLabel> operationNames = new();
    private readonly List<FLabel> operationSuspendLabels = new();
    private readonly FSprite[] operationBackgrounds;
    private readonly FSprite[] operationReorders;
    private readonly FSprite[] operationSuspends;
    private readonly FSprite[] operationDeletes;

    public bool isOperationListActive = false; //turned On when the "Add bill" button is pressed and turned off when the 'throw' button is pressed, a selection is made or the hud is turned off
    public int operationListHorizontal = 0;
    public int operationListVertical = 0;

    public int operationListMaxHorizontal = 0; //how many times the list goes horizontally (aka amount of columns)
    public int operationListMaxVerticalLast = 0; //how far down vertically the list goes. Only counts for the last column as a full column will alwasy go the same amount vertically

    public int operationListMaxVertical = 10; //how far down a column can go vertically, will start a new column when the column reaches this verticality

    public List<float> operationListHorizontalWidth = new();

    public List<OperationSelection> operationList = new(); //the entire list of all operations that can be chosen from, the list is created when isOperationListActive is turned on

    public List<string> possibleOperationsOld = new() {
        "Harvest (heart)", "Harvest (left lung)", "Harvest (right lung)", "Harvest (left kidney)", "Harvest (right kidney)", "Harvest (liver)",
        "Remove (tongue)", "Euthanize by Cut", "Anesthetize",
        "Install peg leg (left leg)", "Install peg leg (right leg)",
        "Install wooden hand (left hand)", "Install wooden hand (right hand)",
        "Install wooden foot (left foot)", "Install wooden foot (right foot)",
        "Install denture (jaw)"
    };

    public List<string> possibleOperations = new() {
        "Harvest",
        "Remove (tongue)", 
        "Amputate",
        "Euthanize by Cut", "Anesthetize",
        "Install archotech arm",
        "Install bionic arm",
        "Install peg leg",
        "Install wooden hand",
        "Install wooden foot",
        "Install denture (jaw)"
    };

    //the entire list of all possible operations. when the OperationList is being made I check each of these options individually, if it is possible it is added to the operationList otherwise it is skipped
    #endregion

    public bool visible = false;
    public bool transparent = false;
    public int transparencyTimer = 0;

    public readonly HealthTabWholeBody healthTabWholeBody;

    public readonly List<HealthTabBodyPart> healthTabBodyParts = new();

    public readonly List<HealthTabInfo> healthTabInfos = new();

    public bool selected = false;
    public float selectedTimer = 5;
    private readonly float selectedTimerMax = 5;

    public float selectedHorizontal = 0;
    public int selectedVertical = -1;

    public int overviewTab = 0;

    public float topLimit = 510;
    public readonly float bottomLimit = 280;

    public float pivotOffset = 0;

    private bool horizontalOnce = true;
    private bool verticalOnce = true;
    private bool interactOnce = true;
    private bool cancelOnce = true;

    public Player.InputPackage input;
    #endregion
}

public class HealthTabBodyPart
{
    public HealthTabBodyPart(HealthTab owner, RimBodyPart bodyPart)
    {
        this.owner = owner;

        this.bodyPart = bodyPart;

        background = new("pixel", true)
        {
            x = DrawPos().x,
            scaleX = 340,
            color = Color.black
        };
        owner.hud.fContainers[1].AddChild(background);

        string name = bodyPart.name;
        if (bodyPart.subName != "")
        {
            name = char.ToLowerInvariant(name[0]) + name.Substring(1);

            name = bodyPart.subName + " " + name;
        }
        bodyPartName = new(Custom.GetFont(), name)
        {
            x = DrawPos().x - 165,
            alignment = FLabelAlignment.Left,
            anchorX = 0f,
            anchorY = 1f
        };
        owner.hud.fContainers[1].AddChild(bodyPartName);

        if (owner.isOperationListActive && owner.operationList.Count > 0)
        {
            background.MoveBehindOtherNode(owner.operationList[0].backgrounds[0]);
            bodyPartName.MoveBehindOtherNode(owner.operationList[0].backgrounds[0]);
        }
        if (owner.healthTabInfos.Count > 0)
        {
            background.MoveBehindOtherNode(owner.healthTabInfos[0].backgrounds[0]);
            bodyPartName.MoveBehindOtherNode(owner.healthTabInfos[0].backgrounds[0]);
        }

        afflictionVisuals.Add(new(owner, DrawPos().x));
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
        if (bodyPart == null || bodyPart.afflictions.Count == 0)
        {
            slatedForDeletion = true;
            return;
        }

        afflictionNumber = bodyPart.afflictions.Count;

        combinedAfflictions = new();
        afflictions = new();

        for (int i = 0; i < bodyPart.afflictions.Count; i++)
        {
            if (bodyPart.afflictions[i] is RimInjury injury && injury is not RimDestroyed && injury.healingDifficulty.combines && (injury is not RimScar scar || !scar.isRevealed))
            {
                if (combinedAfflictions.TryGetValue(CombinedAfflictionName(bodyPart, i), out List<RimAffliction> dic))
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
                afflictionVisuals.Add(new(owner, DrawPos().x));
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
            background.y = DrawPos().y + 3;
            background.scaleY = 0;
        }

        bodyPartName.y = DrawPos().y;

        List<string> usedCombinedAfflictions = new();
        List<RimAffliction> usedAfflictions = new();

        allAfflictions = new();
        allAfflictionsHeight = new();

        extraHeight = 0;

        isVisible = false;

        topVisible = false;
        bottomReached = false;

        for (int i = 0; i < afflictionVisuals.Count; i++)
        {
            afflictionVisuals[i].name.y = DrawPos().y - 18f * i;

            afflictionVisuals[i].icon.y = DrawPos().y - 7 - 18f * i;

            for (int j = 0; j < bodyPart.afflictions.Count; j++)
            {
                if (!usedCombinedAfflictions.Contains(CombinedAfflictionName(bodyPart, j)) && combinedAfflictions.TryGetValue(CombinedAfflictionName(bodyPart, j), out List<RimAffliction> dic))
                {
                    afflictionVisuals[i].name.color = bodyPart.afflictions[j].labelColour;

                    usedCombinedAfflictions.Add(CombinedAfflictionName(bodyPart, j));

                    if (bodyPart.afflictions[j] is RimInjury injury)
                    {
                        string text;

                        text = injury.healingDifficulty.name + (injury.attackName != "" ? " (" + injury.attackName + ")" : "") + (dic.Count > 1 ? " x" + dic.Count : "");

                        afflictionVisuals[i].name.text = text;
                        Menu.MenuLabel.WordWrapLabel(afflictionVisuals[i].name, wordWrap);

                        allAfflictionsHeight.Add(new(2) { Mathf.RoundToInt(afflictionVisuals[i].name.textRect.height / afflictionVisuals[i].name.FontLineHeight), extraHeight });

                        IsVisible(i);

                        afflictionVisuals[i].icon.color = injury.isTended ? Color.white : Color.red;

                        afflictionVisuals[i].icon.isVisible = isVisible && (injury.isTended || !owner.inspectedCreatureState.dead && injury.isBleeding);
                    }

                    allAfflictions.Add(bodyPart.afflictions[j]);

                    break;
                }
                else if (!usedAfflictions.Contains(bodyPart.afflictions[j]) && afflictions.Contains(bodyPart.afflictions[j]))
                {
                    afflictionVisuals[i].name.color = bodyPart.afflictions[j].labelColour;

                    usedAfflictions.Add(bodyPart.afflictions[j]);

                    if (bodyPart.afflictions[j] is RimInjury injury)
                    {
                        string text;

                        if (injury is RimDestroyed destroyed)
                        {
                            if (bodyPart is RimOrgan && destroyed.damageType is RimSurgicalCut)
                            {
                                text = "Removed";
                            }
                            else
                            {
                                text = (bodyPart.isInternal ? bodyPart.isSolid ? "Shattered" : destroyed.healingDifficulty.destroyedOut : destroyed.healingDifficulty.destroyed) + (destroyed.isFresh ? " (fresh)" : "");
                            }
                        }
                        else if (injury is RimScar scar && scar.isRevealed)
                        {
                            text = owner.ScarName(scar);
                        }
                        else
                        {
                            text = injury.healingDifficulty.name + (injury.attackName != "" ? " (" + injury.attackName + ") " : "");
                        }

                        afflictionVisuals[i].name.text = text;

                        Menu.MenuLabel.WordWrapLabel(afflictionVisuals[i].name, wordWrap);
                        allAfflictionsHeight.Add(new(2) { Mathf.RoundToInt(afflictionVisuals[i].name.textRect.height / afflictionVisuals[i].name.FontLineHeight), extraHeight });

                        IsVisible(i);

                        afflictionVisuals[i].icon.color = injury.isTended ? Color.white : Color.red;

                        afflictionVisuals[i].icon.isVisible = isVisible && (injury.isTended && injury is not RimDestroyed || !owner.inspectedCreatureState.dead && injury.isBleeding);
                    }
                    else if (bodyPart.afflictions[j] is RimDisease disease)
                    {
                        string text = disease.name;

                        if (disease is RimInfection && disease.severity > 0)
                        {
                            text += disease.severity switch
                            {
                                < 0.32f => " (minor)",
                                < 0.77f => " (major)",
                                _ => " (extreme)"
                            };
                        }

                        afflictionVisuals[i].name.text = text;

                        Menu.MenuLabel.WordWrapLabel(afflictionVisuals[i].name, wordWrap);

                        allAfflictionsHeight.Add(new(2) { Mathf.RoundToInt(afflictionVisuals[i].name.textRect.height / afflictionVisuals[i].name.FontLineHeight), extraHeight });

                        IsVisible(i);

                        afflictionVisuals[i].icon.color = disease.isTended ? Color.white : Color.grey;
                    }
                    else if (bodyPart.afflictions[j] is RimArtificialPartBase artificialPart)
                    {
                        afflictionVisuals[i].name.text = artificialPart.name;

                        Menu.MenuLabel.WordWrapLabel(afflictionVisuals[i].name, wordWrap);
                        allAfflictionsHeight.Add(new(2) { Mathf.RoundToInt(afflictionVisuals[i].name.textRect.height / afflictionVisuals[i].name.FontLineHeight), extraHeight });

                        IsVisible(i);

                        afflictionVisuals[i].icon.isVisible = false;
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

            bodyPartName.color = Color.white; //Make this change colour depenidng on the remaining health of the part
            bodyPartName.alpha = owner.SetTransparency();

            background.alpha = owner.SetTransparency();
        }

        void IsVisible(int i)
        {
            afflictionVisuals[i].name.y -= 15 * extraHeight;
            afflictionVisuals[i].icon.y -= 15 * extraHeight;

            if (afflictionVisuals[i].name.y > owner.ActualTopLimit())
            {
                afflictionVisuals[i].name.isVisible = false;
                afflictionVisuals[i].icon.isVisible = false;

                owner.afflictionsAbove++;

                background.y -= 18;
            }
            else if (afflictionVisuals[i].name.y - afflictionVisuals[i].name.textRect.height < owner.ActualBottomLimit())
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
                afflictionVisuals[i].name.alpha = owner.SetTransparency();
                afflictionVisuals[i].icon.alpha = owner.SetTransparency();
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

        bodyPart = null;
    }

    public string CombinedAfflictionName(RimBodyPart bodyPart, int i)
    {
        string name = "";

        if (bodyPart.afflictions[i] is RimInjury injury)
        {
            name = injury.healingDifficulty.name + injury.attackName + injury.isTended;
        }
        else if (bodyPart.afflictions[i] is RimDisease disease)
        {
            name = disease.name;
        }

        return name;
    }
    public string CombinedAfflictionName(List<RimAffliction> bodyPart, int i)
    {
        string name = "";

        if (bodyPart[i] is RimInjury injury)
        {
            name = injury.healingDifficulty.name + injury.attackName + injury.isTended;
        }
        else if (bodyPart[i] is RimDisease disease)
        {
            name = disease.name;
        }

        return name;
    }

    public float GetPivotOffset(int afflictionNumber, bool top)
    {
        float offset = 0;

        List<string> usedCombinedAfflictions = new();
        List<RimAffliction> usedAfflictions = new();

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
            offset = owner.ActualTopLimit() - offset - 5;
        }
        else
        {
            offset = owner.ActualBottomLimit() - offset;
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

    public RimBodyPart bodyPart;

    public FSprite background;

    public FLabel bodyPartName;

    public List<HealthTabAffliction> afflictionVisuals = new();

    public int afflictionNumber = 1;
    public int extraHeight = 0;

    public Dictionary<string, List<RimAffliction>> combinedAfflictions;
    public List<RimAffliction> afflictions;

    public List<RimAffliction> allAfflictions;
    public List<List<int>> allAfflictionsHeight;

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

        background = new("pixel", true)
        {
            x = DrawPos().x,
            scaleX = 340,
            color = Color.black,
        };
        owner.hud.fContainers[1].AddChild(background);

        name = new(Custom.GetFont(), "Whole Body")
        {

            x = DrawPos().x - 165,
            y = DrawPos().y,
            alignment = FLabelAlignment.Left,
            anchorX = 0f,
            anchorY = 1f,
            color = Color.grey
        };
        owner.hud.fContainers[1].AddChild(name);

        bloodLossName = new(Custom.GetFont(), "Blood loss ()")
        {
            x = DrawPos().x - 45,
            alignment = FLabelAlignment.Left,
            anchorX = 0f,
            anchorY = 1f,
            color = Color.white
        };
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
                afflictionVisuals.Add(new(owner, DrawPos().x));
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
            background.y = DrawPos().y + 3;
            background.scaleY = 0;
        }

        isVisible = false;

        topVisible = false;
        bottomReached = false;

        if (bloodLossVisible)
        {
            bloodLossName.y = DrawPos().y;

            bloodLossName.text = owner.inspectedState.bloodLoss switch
            {
                < 0.3f => "Blood loss (minor)",
                < 0.45f => "Blood loss (moderate)",
                < 0.6f => "Blood loss (severe)",
                _ => "Blood loss (extreme)"
            };

            if (bloodLossName.y > owner.ActualTopLimit())
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
            afflictionVisuals[i].name.y = DrawPos().y - ((bloodLossVisible ? 18f : 0) + (18f * i));
            afflictionVisuals[i].name.color = owner.inspectedState.wholeBodyAfflictions[i].labelColour;

            afflictionVisuals[i].icon.y = DrawPos().y - 7 - ((bloodLossVisible ? 18f : 0) + (18f * i));

            if (owner.inspectedState.wholeBodyAfflictions[i] is RimDisease disease)
            {
                string text = disease.name;

                if (disease is RimFlu && disease.severity > 0)
                {
                    text += disease.severity switch
                    {
                        < 0.665f => " (minor)",
                        < 0.832f => " (major)",
                        _ => " (extreme)"
                    };
                }

                afflictionVisuals[i].name.text = text;

                Menu.MenuLabel.WordWrapLabel(afflictionVisuals[i].name, wordWrap);

                afflictionsHeight.Add(new(2) { Mathf.RoundToInt(afflictionVisuals[i].name.textRect.height / afflictionVisuals[i].name.FontLineHeight), extraHeight });

                IsVisible(i);

                afflictionVisuals[i].icon.color = disease.isTended ? Color.white : Color.grey;
            }
            else if (owner.inspectedState.wholeBodyAfflictions[i] is RimAilment ailment)
            {
                string text = "null";
                switch (ailment)
                {
                    case RimAirInLungs:
                        text = "Lung capacity";

                        text += ailment.amount switch
                        {
                            < 0f => " (empty)",
                            < 0.4f => " (low)",
                            < 0.7f => " (medium)",
                            _ => " (high)"
                        };
                        break;
                    case RimHypothermia:
                        text = "Hypothermia";

                        text += ailment.amount switch
                        {
                            < 0.2f => " (shivering)",
                            < 0.35f => " (minor)",
                            < 0.62f => " (serious)",
                            _ => " (extreme)"
                        };
                        break;
                    case RimToxicBuildup:
                        text = "Toxic buildup";

                        text += ailment.amount switch
                        {
                            < 0.2f => " (initial)",
                            < 0.4f => " (minor)",
                            < 0.6f => " (moderate)",
                            < 0.8f => " (serious)",
                            _ => " (extreme)"
                        };
                        break;
                    case RimAnesthetic:
                        text = "Anesthetic";

                        text += ailment.amount switch
                        {
                            < 0.6f => " (wearing off, ",
                            < 0.8f => " (woozy, ",
                            _ => " (sedated, "
                        };

                        text += HealthTab.GetTimeString(ailment.amount / (40 * 60)) + ")";
                        break;
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
            name.alpha = owner.SetTransparency();

            bloodLossName.alpha = owner.SetTransparency();

            background.alpha = owner.SetTransparency();
        }

        void IsVisible(int i)
        {
            afflictionVisuals[i].name.y -= 15 * extraHeight;
            afflictionVisuals[i].icon.y -= 15 * extraHeight;

            if (afflictionVisuals[i].name.y > owner.ActualTopLimit())
            {
                afflictionVisuals[i].name.isVisible = false;
                afflictionVisuals[i].icon.isVisible = false;

                owner.afflictionsAbove++;

                background.y -= 18;
            }
            else if (afflictionVisuals[i].name.y - afflictionVisuals[i].name.textRect.height < owner.ActualBottomLimit())
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
                afflictionVisuals[i].name.alpha = owner.SetTransparency();
                afflictionVisuals[i].icon.alpha = owner.SetTransparency();
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

        offset = top ? owner.ActualTopLimit() - offset - 5 : owner.ActualBottomLimit() - offset;

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
    public HealthTabAffliction(HealthTab owner, float drawPos)
    {
        name = new FLabel(Custom.GetFont(), "capacityName")
        {
            x = drawPos - 45,
            alignment = FLabelAlignment.Left,
            anchorX = 0f,
            anchorY = 1f
        };
        owner.hud.fContainers[1].AddChild(name);

        icon = new FSprite("pixel", true)
        {
            x = drawPos + 160,
            scale = 14
        };
        owner.hud.fContainers[1].AddChild(icon);

        if (owner.isOperationListActive && owner.operationList.Count > 0)
        {
            name.MoveBehindOtherNode(owner.operationList[0].backgrounds[0]);
            icon.MoveBehindOtherNode(owner.operationList[0].backgrounds[0]);
        }
        if (owner.healthTabInfos.Count > 0)
        {
            name.MoveBehindOtherNode(owner.healthTabInfos[0].backgrounds[0]);
            icon.MoveBehindOtherNode(owner.healthTabInfos[0].backgrounds[0]);
        }
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
        backgrounds[0] = new FSprite("pixel", true)
        {
            anchorX = 0f,
            color = Color.black
        };
        owner.hud.fContainers[1].AddChild(backgrounds[0]);
        backgrounds[1] = new FSprite("pixel", true)
        {
            anchorX = 0f,
            color = Color.grey
        };
        owner.hud.fContainers[1].AddChild(backgrounds[1]);

        name = new(Custom.GetFont(), "Name")
        {
            alignment = FLabelAlignment.Left,
            anchorX = 0f,
            anchorY = 1f
        };
        owner.hud.fContainers[1].AddChild(name);

        nameStatus = new(Custom.GetFont(), " : Status")
        {
            alignment = FLabelAlignment.Left,
            anchorX = 0f,
            anchorY = 1f
        };
        owner.hud.fContainers[1].AddChild(nameStatus);

        description = new(Custom.GetFont(), "")
        {
            alignment = FLabelAlignment.Left,
            anchorX = 0f,
            anchorY = 1f
        };
        owner.hud.fContainers[1].AddChild(description);

        colouredName = new(Custom.GetFont(), "")
        {
            alignment = FLabelAlignment.Left,
            anchorX = 0f,
            anchorY = 1f,
            color = Color.yellow
        };
        owner.hud.fContainers[1].AddChild(colouredName);

        colouredName2 = new(Custom.GetFont(), "")
        {
            alignment = FLabelAlignment.Left,
            anchorX = 0f,
            anchorY = 1f,
            color = Color.red
        };
        owner.hud.fContainers[1].AddChild(colouredName2);
    }

    public Vector2 DrawPos()
    {
        Vector2 pos = new(owner.selectedSprite.x, owner.selectedSprite.y);

        if (!singleplayerHud && pos.x > owner.hud.rainWorld.screenSize.x / 2 && owner.selectedHorizontal != 0)
        {
            pos.x -= (owner.selectedSprite.scaleX / 2) + 270;
        }
        else
        {
            pos.x += (owner.selectedSprite.scaleX / 2) + 10;
        }

        pos.y -= (owner.selectedSprite.scaleY / 2) + 5;

        for (int i = 0; i < owner.healthTabInfos.IndexOf(this); i++)
        {
            pos.y -= 2 + owner.healthTabInfos[i].backgrounds[0].scaleY;

            pos.y += owner.healthTabInfos[i].borderCorrection;
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

        colouredName.isVisible = visible && colouredName.text != "";
        colouredName2.isVisible = visible && colouredName2.text != "";

        if (!visible)
        {
            return;
        }

        if (owner.isOperationListActive)
        {
            foreach (OperationSelection operationSelection in owner.operationList)
            {
                foreach (FSprite sprite in operationSelection.backgrounds)
                {
                    sprite.MoveBehindOtherNode(backgrounds[0]);
                }
                    
                operationSelection.nameLabel.MoveBehindOtherNode(backgrounds[0]);
                operationSelection.icon.MoveBehindOtherNode(backgrounds[0]);
            }
        }

        if (owner.healthTabInfos.IndexOf(this) != 0 || owner.healthTabInfos.IndexOf(this) == 0 && (owner.selectedHorizontal == 1 || owner.isOperationListActive || owner.selectedHorizontal == 2 && owner.healthTabWholeBody.active && owner.selectedVertical < owner.healthTabWholeBody.afflictionNumber))
        {
            name.color = Color.yellow;
        }
        else
        {
            name.color = Color.white;
        }

        name.x = DrawPos().x;
        name.y = DrawPos().y;
        name.alpha = owner.SetTransparency();

        nameStatus.x = DrawPos().x + name.textRect.width;
        nameStatus.y = DrawPos().y;

        nameStatus.alpha = owner.SetTransparency();

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

        if (colouredName.text != "")
        {
            colouredName.x = name.x;
            colouredName.y = name.y - name.textRect.height;

            if (backgroundPos.x < colouredName.textRect.width)
            {
                backgroundPos.x = colouredName.textRect.width;
            }

            backgroundPos.y -= colouredName.textRect.height;

            if (backgroundSize.x < colouredName.textRect.width)
            {
                backgroundSize.x = colouredName.textRect.width;
            }

            backgroundSize.y += colouredName.textRect.height;
        }

        if (description.text != "")
        {
            FLabel labelAbove = colouredName.text != "" ? colouredName : name;

            if (owner.healthTabInfos.IndexOf(this) == 0)
            {
                if (owner.selectedHorizontal == 2 && owner.healthTabWholeBody.active && owner.selectedVertical < owner.healthTabWholeBody.afflictionNumber)
                {
                    description.x = labelAbove.x;
                    description.y = labelAbove.y - (labelAbove.textRect.height * 2);

                    backgroundPos.y -= labelAbove.textRect.height;
                    backgroundSize.y += labelAbove.textRect.height;
                }
                else
                {
                    description.x = labelAbove.x;
                    description.y = labelAbove.y - labelAbove.textRect.height;
                }
            }
            else
            {
                description.x = labelAbove.x;
                description.y = labelAbove.y - (labelAbove.textRect.height * 2);

                backgroundPos.y -= labelAbove.textRect.height;
                backgroundSize.y += labelAbove.textRect.height;
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

        if (colouredName2.text != "")
        {
            FLabel labelAbove = description.text != "" ? description : colouredName.text != "" ? colouredName : name;

            colouredName2.x = labelAbove.x;
            colouredName2.y = labelAbove.y - labelAbove.textRect.height;

            if (backgroundPos.x < colouredName2.textRect.width)
            {
                backgroundPos.x = colouredName2.textRect.width;
            }

            backgroundPos.y -= colouredName2.textRect.height;

            if (backgroundSize.x < colouredName2.textRect.width)
            {
                backgroundSize.x = colouredName2.textRect.width;
            }

            backgroundSize.y += colouredName2.textRect.height;
        }

        backgrounds[0].x = DrawPos().x - 5;
        backgrounds[0].y = DrawPos().y + (backgroundPos.y / 2) - 15;

        backgrounds[0].scaleX = backgroundSize.x + 10;
        backgrounds[0].scaleY = backgroundSize.y + 10;

        backgrounds[1].x = DrawPos().x - 4;
        backgrounds[1].y = DrawPos().y + (backgroundPos.y / 2) - 15;

        backgrounds[1].scaleX = backgroundSize.x + 8;
        backgrounds[1].scaleY = backgroundSize.y + 8;

        backgrounds[0].alpha = owner.SetTransparency();
        backgrounds[1].alpha = owner.SetTransparency();

        borderCorrection = Mathf.CeilToInt(backgrounds[0].y - (backgrounds[0].scaleY / 2) < 0 ? Mathf.Abs(backgrounds[0].y - (backgrounds[0].scaleY / 2)) : 0);

        if (borderCorrection > 0)
        {
            name.y += borderCorrection;
            nameStatus.y += borderCorrection;
            description.y += borderCorrection;
            colouredName.y += borderCorrection;
            colouredName2.y += borderCorrection;
            backgrounds[0].y += borderCorrection;
            backgrounds[1].y += borderCorrection;

            for (int i = 0; i < owner.healthTabInfos.IndexOf(this); i++)
            {
                owner.healthTabInfos[i].name.y += borderCorrection;
                owner.healthTabInfos[i].nameStatus.y += borderCorrection;
                owner.healthTabInfos[i].description.y += borderCorrection;
                owner.healthTabInfos[i].colouredName.y += borderCorrection;
                owner.healthTabInfos[i].colouredName2.y += borderCorrection;
                owner.healthTabInfos[i].backgrounds[0].y += borderCorrection;
                owner.healthTabInfos[i].backgrounds[1].y += borderCorrection;
            }
        }
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

    public readonly FSprite[] backgrounds;

    public FLabel name;
    public FLabel nameStatus;

    public FLabel description;

    public FLabel colouredName;
    public FLabel colouredName2;

    public bool slatedForDeletion = false;

    public int borderCorrection = 0;
    #endregion
}

public class OperationSelection
{
    public OperationSelection(HealthTab owner, string operationName, string operationSubName, int horizontal, int vertical, string nameOverride = "")
    {
        this.owner = owner;

        this.horizontal = horizontal;
        this.vertical = vertical;

        name = operationName;
        subName = operationSubName;

        string nameForLabel = nameOverride == "" ? name : nameOverride;

        if (nameOverride == "" && subName != "")
        {
            nameForLabel += " " + subName;
        }

        #region backgrounds
        backgrounds = new FSprite[2];
        backgrounds[0] = new FSprite("pixel", true)
        {
            anchorX = 0,
            color = Color.white, //white is default meaning this is redundant but I will keep it here for when I update all the colours
            scaleY = 20
        };
        owner.hud.fContainers[1].AddChild(backgrounds[0]);

        backgrounds[1] = new FSprite("pixel", true)
        {
            anchorX = 0,
            color = Color.grey,
            scaleY = 18
        };
        owner.hud.fContainers[1].AddChild(backgrounds[1]);
        #endregion

        nameLabel = new(Custom.GetFont(), nameForLabel)
        {
            alignment = FLabelAlignment.Left,
            anchorX = 0f,
            anchorY = 0.5f
        };
        owner.hud.fContainers[1].AddChild(nameLabel);

        if (owner.operationListHorizontalWidth[horizontal] < nameLabel.textRect.width)
            owner.operationListHorizontalWidth[horizontal] = nameLabel.textRect.width;

        icon = new FSprite("pixel", true)
        {
            scale = 14,
        };
        owner.hud.fContainers[1].AddChild(icon);
    }

    public Vector2 DrawPos()
    {
        Vector2 pos = new(owner.addBillSprite.x, owner.addBillSprite.y);

        float horizontalOffset = 0;

        for (int i = 0; i < horizontal; i++)
        {
            horizontalOffset += 34 + owner.operationListHorizontalWidth[i];
        }

        pos.x += 30 + horizontalOffset;
        pos.y += 20 - (20 * vertical);

        return pos;
    }

    public void Update()
    {
        if (!owner.visible || !owner.isOperationListActive)
        {
            slatedForDeletion = true;
        }
    }
    public void Draw()
    {
        bool isSelected = owner.operationListHorizontal == horizontal && owner.operationListVertical == vertical;

        backgrounds[0].x = DrawPos().x;
        backgrounds[0].y = DrawPos().y;

        backgrounds[0].scaleX = 35 + owner.operationListHorizontalWidth[horizontal];

        backgrounds[1].x = DrawPos().x + 1f;
        backgrounds[1].y = DrawPos().y;

        backgrounds[1].scaleX = 33 + owner.operationListHorizontalWidth[horizontal];

        backgrounds[1].color = isSelected ? Color.blue : Color.grey;

        backgrounds[0].alpha = owner.SetTransparency();
        backgrounds[1].alpha = owner.SetTransparency();

        nameLabel.x = DrawPos().x + 20 + (isSelected ? 5 : 0);
        nameLabel.y = DrawPos().y;

        icon.x = DrawPos().x + 10 + (isSelected ? 5 : 0);
        icon.y = DrawPos().y;
    }

    public void ClearSprites()
    {
        foreach (FSprite background in backgrounds)
        {
            background.RemoveFromContainer();
        }

        nameLabel.RemoveFromContainer();
        icon.RemoveFromContainer();

        operation = null;
    }

    #region Values
    private readonly HealthTab owner;

    public RimOperation operation;

    public int horizontal = 0;
    public int vertical = 0;

    public float width = 0;

    public readonly FSprite[] backgrounds;

    public FLabel nameLabel;
    public FSprite icon;

    public string name = "";
    public string subName = "";

    public bool slatedForDeletion = false;
    #endregion
}