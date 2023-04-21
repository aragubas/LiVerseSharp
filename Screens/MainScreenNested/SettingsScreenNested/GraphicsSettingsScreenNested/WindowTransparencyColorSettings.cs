﻿using LiVerse.AnaBanUI;
using LiVerse.AnaBanUI.Containers;
using LiVerse.AnaBanUI.Controls;
using LiVerse.AnaBanUI.Controls.ComboBox;
using LiVerse.AnaBanUI.Events;
using LiVerse.Stores;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiVerse.Screens.MainScreenNested.SettingsScreenNested.GraphicsSettingsScreenNested {
  public class WindowTransparencyColorSettings : ControlBase {
    ScrollableList optionsList { get; }
    Label transparentBackgroudWarning { get; }

    public WindowTransparencyColorSettings() {
      optionsList = new() { ParentControl = this, Gap = 4 };

      DockFillContainer DockFill = new() { ParentControl = this, DockType = DockFillContainerDockType.Left, Gap = 6 };
      Label backgroundTransparencyTypeTitle = new("Background Transparency Type: ") { Color = Color.Black };
      List<ComboBoxOption> options = new();
      ComboBoxOption defaultOption = new();

      if (SettingsStore.WindowTransparencyColor == Color.Transparent) {
        defaultOption.OptionText = "Transparent";
        defaultOption.ExtraData = 0;

      } else if (SettingsStore.WindowTransparencyColor == Color.Green) {
        defaultOption.OptionText = "Green";
        defaultOption.ExtraData = 1;

      } else if (SettingsStore.WindowTransparencyColor == Color.Blue) {
        defaultOption.OptionText = "Blue";
        defaultOption.ExtraData = 2;

      } else if (SettingsStore.WindowTransparencyColor == Color.Magenta) {
        defaultOption.OptionText = "Magenta";
        defaultOption.ExtraData = 3;

      } else {
        defaultOption.OptionText = "Custom Color";
        defaultOption.ExtraData = -1;
      }

      options.Add(new ComboBoxOption("Transparent", 0));
      options.Add(new ComboBoxOption("Green", 1));
      options.Add(new ComboBoxOption("Blue", 2));
      options.Add(new ComboBoxOption("Magenta", 3));
      options.Add(new ComboBoxOption("Custom Color", -1));

      ComboBoxControl backgroundTransparencyOptionsComboBox = new(defaultOption, options);
      backgroundTransparencyOptionsComboBox.SelectedOptionChanged += BackgroundTransparencyOptionsComboBox_SelectedOptionChanged; ;

      DockFill.DockElement = backgroundTransparencyTypeTitle;
      DockFill.FillElement = backgroundTransparencyOptionsComboBox;

      optionsList.Elements.Add(DockFill);

      transparentBackgroudWarning = new Label("For transparent background to work you will need to use GameCapture on OBS\nor any capture method that supports the alpha channel") { HorizontalAlignment = LabelHorizontalAlignment.Left };
      optionsList.Elements.Add(transparentBackgroudWarning);
    }

    private void BackgroundTransparencyOptionsComboBox_SelectedOptionChanged(ComboBoxOption obj) {
      if (obj.ExtraData is int selectionNumber) {
        switch (selectionNumber) {
          case 0: { // Transparent
              SettingsStore.WindowTransparencyColor = Color.Transparent;
              break;
            }

          case 1: { // Green
              SettingsStore.WindowTransparencyColor = Color.FromNonPremultiplied(0, 255, 0, 255);
              break;
            }

          case 2: { // Blue
              SettingsStore.WindowTransparencyColor = Color.FromNonPremultiplied(0, 0, 255, 255);
              break;
            }

          case 3: { // Magenta
              SettingsStore.WindowTransparencyColor = Color.FromNonPremultiplied(255, 0, 255, 255);
              break;
            }

          default: { // Invalid/Custom Color
              break;
            }
        }
      }
    }

    public override void UpdateUI(double deltaTime) {
      FillElement(optionsList);

      transparentBackgroudWarning.Visible = SettingsStore.WindowTransparencyColor == Color.Transparent;
    }

    public override void DrawElement(SpriteBatch spriteBatch, double deltaTime) {
      optionsList.Draw(spriteBatch, deltaTime);
    }

    public override bool InputUpdate(PointerEvent pointerEvent) {
      return optionsList.InputUpdate(pointerEvent);
    }

    public override bool InputUpdate(KeyboardEvent keyboardEvent) {
      return optionsList.InputUpdate(keyboardEvent);
    }

    public override void Update(double deltaTime) {
      optionsList.Update(deltaTime);
    }
  }
}
