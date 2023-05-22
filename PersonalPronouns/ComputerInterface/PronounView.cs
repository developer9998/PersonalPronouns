using ComputerInterface;
using ComputerInterface.Interfaces;
using ComputerInterface.ViewLib;
using PersonalPronouns.Scripts;
using System;
using System.Text;
using UnityEngine;

namespace PersonalPronouns.ComputerInterface
{
    public class PronounEntry : IComputerModEntry
    {
        public string EntryName => PluginInfo.Name;

        public Type EntryViewType => typeof(PronounView);
    }

    public class PronounView : ComputerView
    {
        private readonly UISelectionHandler _selectionManager;

        private readonly UISelectionHandler _selectionManagerFirstChoice;
        private readonly UISelectionHandler _selectionManagerSecondChoice;

        private bool PronounsSet = false;
        private float PronounSetTime;

        public PronounView()
        {
            _selectionManager = new UISelectionHandler(EKeyboardKey.Up, EKeyboardKey.Down)
            {
                MaxIdx = 1
            };
            _selectionManager.ConfigureSelectionIndicator($"<color=#{PrimaryColor}> ></color> ", "", "   ", "");

            _selectionManagerFirstChoice = new UISelectionHandler(EKeyboardKey.Left, EKeyboardKey.Right)
            {
                MaxIdx = Enum.GetNames(typeof(Utils.PronounFirst)).Length - 1
            };
            _selectionManagerSecondChoice = new UISelectionHandler(EKeyboardKey.Left, EKeyboardKey.Right)
            {
                MaxIdx = Enum.GetNames(typeof(Utils.PronounSecond)).Length - 1
            };
        }

        public override void OnShow(object[] args)
        {
            base.OnShow(args);

            _selectionManagerFirstChoice.CurrentSelectionIndex = (int)Utils.First;
            _selectionManagerSecondChoice.CurrentSelectionIndex = (int)Utils.Second;
            Redraw();
        }

        public void Redraw()
        {
            StringBuilder str = new StringBuilder();
            str.Repeat("=", SCREEN_WIDTH).AppendLine();
            str.BeginCenter().AppendClr(PluginInfo.Name, "#BCC3F2FF").AppendLine();
            str.AppendClr("A mod by ", "ffffff50").Append("Dev.").EndAlign().AppendLine();
            str.Repeat("=", SCREEN_WIDTH).AppendLines(3);

            str.Append(" Pronouns: ").AppendLine();

            str.AppendClr(_selectionManager.GetIndicatedText(0, ((Utils.PronounFirst)_selectionManagerFirstChoice.CurrentSelectionIndex).ToString()), TextColour(0)).AppendLine();
            str.AppendClr(_selectionManager.GetIndicatedText(1, ((Utils.PronounSecond)_selectionManagerSecondChoice.CurrentSelectionIndex).ToString()), TextColour(1));

            str.AppendLines(3)
                .AppendClr(!PronounsSet ? " * Press Enter to update your pronouns." : " * Pronouns update been updated.", "ffffff50").AppendLine();

            SetText(str);
        }

        public string TextColour(int index)
            => _selectionManager.CurrentSelectionIndex == index ? "ffffffff" : "ffffff90";

        public override void OnKeyPressed(EKeyboardKey key)
        {
            base.OnKeyPressed(key);
            PronounsSet = false;

            switch (key)
            {
                case EKeyboardKey.Enter:
                    if (Time.time >= (PronounSetTime + 10))
                    {
                        PronounSetTime = Time.time;
                        Utils.UpdatePronouns((Utils.PronounFirst)_selectionManagerFirstChoice.CurrentSelectionIndex, (Utils.PronounSecond)_selectionManagerSecondChoice.CurrentSelectionIndex);
                        PronounsSet = true;
                        Redraw();
                    }
                    break;
                case EKeyboardKey.Back:
                    ReturnToMainMenu();
                    break;
                default:
                    if (_selectionManager.HandleKeypress(key)) Redraw();
                    var selection = (_selectionManager.CurrentSelectionIndex == 0) ? _selectionManagerFirstChoice : _selectionManagerSecondChoice;
                    if (selection.HandleKeypress(key)) Redraw();
                    break;
            }
        }
    }
}
