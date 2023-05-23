using ComputerInterface;
using ComputerInterface.Interfaces;
using ComputerInterface.ViewLib;
using GorillaNetworking;
using HarmonyLib;
using PersonalPronouns.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private readonly UISelectionHandler _subjectManager;
        private readonly UISelectionHandler _objectManager;
        private readonly UISelectionHandler _possessiveManager;

        private bool PronounsSet = false;
        private float PronounSetTime;

        private CustomScreenInfo screenInfo;

        public PronounView()
        {
            _selectionManager = new UISelectionHandler(EKeyboardKey.Up, EKeyboardKey.Down)
            {
                MaxIdx = 2
            };
            _selectionManager.ConfigureSelectionIndicator($"<color=#{PrimaryColor}> ></color> ", "", "   ", "");

            _subjectManager = new UISelectionHandler(EKeyboardKey.Left, EKeyboardKey.Right)
            {
                MaxIdx = Utils.Subject.Count - 1
            };
            _objectManager = new UISelectionHandler(EKeyboardKey.Left, EKeyboardKey.Right)
            {
                MaxIdx = Utils.Object.Count - 1
            };
            _possessiveManager = new UISelectionHandler(EKeyboardKey.Left, EKeyboardKey.Right)
            {
                MaxIdx = Utils.Possessive.Count - 1
            };
        }

        public override void OnShow(object[] args)
        {
            base.OnShow(args);

            _subjectManager.CurrentSelectionIndex = Utils.SubjectPronoun;
            _objectManager.CurrentSelectionIndex = Utils.ObjectPronoun;
            _possessiveManager.CurrentSelectionIndex = Utils.PossessivePronoun;

            var field = AccessTools.Field(GorillaComputer.instance.GetComponent<CustomComputer>().GetType(), "_customScreenInfos");
            var screenInfos = (List<CustomScreenInfo>)field.GetValue(GorillaComputer.instance.GetComponent<CustomComputer>());
            screenInfo = screenInfos.First();

            Redraw();
        }

        public void Redraw()
        {
            StringBuilder str = new StringBuilder();
            str.Repeat("=", SCREEN_WIDTH).AppendLine();
            str.BeginCenter().AppendClr(PluginInfo.Name, "#BCC3F2FF").AppendLine();
            str.AppendClr("A mod by ", "ffffff50").Append("Dev.").EndAlign().AppendLine();
            str.Repeat("=", SCREEN_WIDTH).AppendLines(2);

            float smallerFontSize = screenInfo.FontSize / 1.5f;

            str.Append(" Current Pronouns").AppendLine();
            str.AppendClr(_selectionManager.GetIndicatedText(0, _selectionManager.CurrentSelectionIndex, Utils.GetPronoun(Utils.PronounType.Subject, _subjectManager.CurrentSelectionIndex)), TextColour(0));
            str.AppendClr($"<size={smallerFontSize}>  Current: {Utils.CurrentPronouns.Subject}</size>", "ffffff50").AppendLine();
            str.AppendClr(_selectionManager.GetIndicatedText(1, _selectionManager.CurrentSelectionIndex, Utils.GetPronoun(Utils.PronounType.Object, _objectManager.CurrentSelectionIndex)), TextColour(1));
            str.AppendClr($"<size={smallerFontSize}>  Current: {Utils.CurrentPronouns.Object}</size>", "ffffff50").AppendLine();
            str.AppendClr(_selectionManager.GetIndicatedText(2, _selectionManager.CurrentSelectionIndex, Utils.GetPronoun(Utils.PronounType.Possessive, _possessiveManager.CurrentSelectionIndex)), TextColour(2));
            str.AppendClr($"<size={smallerFontSize}>  Current: {Utils.CurrentPronouns.Possessive}</size>", "ffffff50");

            str.AppendLines(2)
                .AppendClr(!PronounsSet ? " * Press Enter to update your pronouns." : " * Pronouns have been updated.", "ffffff50")
                .AppendLine();

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
                    if ((PronounSetTime + 5) > Time.time)
                    {
                        Redraw();
                        return;
                    }
                    PronounSetTime = Time.time;
                    PronounsSet = true;
                    Utils.UpdatePronouns(_subjectManager.CurrentSelectionIndex, _objectManager.CurrentSelectionIndex, _possessiveManager.CurrentSelectionIndex);
                    Redraw();
                    break;
                case EKeyboardKey.Back:
                    ReturnToMainMenu();
                    break;
                default:
                    if (_selectionManager.HandleKeypress(key)) Redraw();
                    var selection = (_selectionManager.CurrentSelectionIndex == 0) ? _subjectManager : ((_selectionManager.CurrentSelectionIndex == 1) ? _objectManager : _possessiveManager);
                    if (selection.HandleKeypress(key)) Redraw();
                    break;
            }
        }
    }
}
