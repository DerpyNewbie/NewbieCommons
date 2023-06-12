using System;
using UnityEditor;

namespace DerpyNewbie.Common.Editor
{
    public readonly struct Issue
    {
        public string Message { get; }
        public MessageType MessageType { get; }
        public Action AutoFixAction { get; }
        public Action SelectAction { get; }

        public Issue(string message, MessageType messageType, Action autoFixAction = null, Action selectAction = null)
        {
            Message = message;
            MessageType = messageType;
            AutoFixAction = autoFixAction;
            SelectAction = selectAction;
        }

        public void HelpBoxWithButton(float buttonWidth = 100F)
        {
            if (AutoFixAction != null && SelectAction != null)
            {
                NewbieCommonsEditorUtil.HelpBoxWithButton(
                    Message, MessageType,
                    "Select", SelectAction,
                    "Auto Fix", AutoFixAction,
                    buttonWidth
                );
            }
            else if (AutoFixAction != null)
            {
                NewbieCommonsEditorUtil.HelpBoxWithButton(
                    Message, MessageType,
                    "Auto Fix", AutoFixAction,
                    buttonWidth: buttonWidth
                );
            }
            else if (SelectAction != null)
            {
                NewbieCommonsEditorUtil.HelpBoxWithButton(
                    Message, MessageType,
                    "Select", SelectAction,
                    buttonWidth: buttonWidth
                );
            }
            else
            {
                NewbieCommonsEditorUtil.HelpBoxWithButton(
                    Message, MessageType
                );
            }
        }
    }
}