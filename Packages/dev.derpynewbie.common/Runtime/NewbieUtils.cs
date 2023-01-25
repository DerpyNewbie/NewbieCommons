using System;
using JetBrains.Annotations;
using UnityEngine;
using VRC.SDKBase;

namespace DerpyNewbie.Common
{
    public static class NewbieUtils
    {
        public static bool IsJapaneseTimeZone() => TimeZoneInfo.Local.Id == "Tokyo Standard Time" ||
                                                   TimeZoneInfo.Local.Id == "JST" ||
                                                   TimeZoneInfo.Local.Id == "Asia/Tokyo";

        public static float GetDefaultGain() => 15F;
        public static float GetDefaultFar() => 25F;
        public static float GetDefaultNear() => 0;
        public static float GetDefaultRadius() => 0;

        [PublicAPI]
        public static void ResetVoice(VRCPlayerApi api)
        {
            if (!Utilities.IsValid(api)) return;
            api.SetVoiceGain(GetDefaultGain());
            api.SetVoiceDistanceFar(GetDefaultFar());
            api.SetVoiceDistanceNear(GetDefaultNear());
            api.SetVoiceVolumetricRadius(GetDefaultRadius());
        }

        [PublicAPI]
        public static string GetPlayerName(VRCPlayerApi api)
        {
            return GetPlayerName(api, -1);
        }

        [PublicAPI]
        public static string GetPlayerName(int playerId)
        {
            return GetPlayerName(VRCPlayerApi.GetPlayerById(playerId), playerId);
        }

        [PublicAPI]
        public static string GetPlayerName(VRCPlayerApi api, int playerId)
        {
            return !Utilities.IsValid(api) ? $"{playerId}:InvalidPlayer" : $"{api.playerId}:{api.displayName}";
        }

        private static string ReplaceColorToTMP(string source)
        {
            return source
                .Replace("<color=aqua>", "<color=#00ffffff>")
                .Replace("<color=black>", "<color=#000000ff>")
                .Replace("<color=blue>", "<color=#0000ffff>")
                .Replace("<color=brown>", "<color=#a52a2aff>")
                .Replace("<color=cyan>", "<color=#00ffffff>")
                .Replace("<color=darkblue>", "<color=#0000a0ff>")
                .Replace("<color=fuchsia>", "<color=#ff00ffff>")
                .Replace("<color=green>", "<color=#008000ff>")
                .Replace("<color=grey>", "<color=#808080ff>")
                .Replace("<color=lightblue>", "<color=#add8e6ff>")
                .Replace("<color=lime>", "<color=#00ff00ff>")
                .Replace("<color=magenta>", "<color=#ff00ffff>")
                .Replace("<color=maroon>", "<color=#800000ff>")
                .Replace("<color=navy>", "<color=#800000ff>")
                .Replace("<color=olive>", "<color=#808000ff>")
                .Replace("<color=orange>", "<color=#ffa500ff>")
                .Replace("<color=purple>", "<color=#800080ff>")
                .Replace("<color=red>", "<color=#ff0000ff>")
                .Replace("<color=silver>", "<color=#c0c0c0ff>")
                .Replace("<color=teal>", "<color=#008080ff>")
                .Replace("<color=white>", "<color=#ffffffff>")
                .Replace("<color=yellow>", "<color=#ffff00ff>");
        }
        
        public static Color GetColorFromAlias(string name)
        {
            switch (name.ToLower())
            {
                case "red":
                case "r":
                    return Color.red;
                case "green":
                case "g":
                    return Color.green;
                case "blue":
                case "b":
                    return Color.blue;
                default:
                case "white":
                    return Color.white;
                case "black":
                    return Color.black;
                case "yellow":
                case "y":
                    return Color.yellow;
                case "cyan":
                case "c":
                    return Color.cyan;
                case "magenta":
                case "m":
                    return Color.magenta;
                case "grey":
                case "gray":
                    return Color.gray;
                case "clear":
                    return Color.clear;
            }
        }

        public static string[] GetColorAliases()
        {
            return new[] { "red", "green", "blue", "white", "black", "yellow", "cyan", "magenta", "gray", "clear" };
        }
    }
}