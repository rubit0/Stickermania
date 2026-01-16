using System;
using UnityEngine;
using UnityEditor;
using SA.Foundation.Editor;

namespace SA.CrossPlatform
{
    public static class UM_SettingsUtil 
    {
        private const string k_PlaymakerAddon = "https://dl.dropboxusercontent.com/s/k2ivxt6j8dhhb9h/PlayMakerAddon.unitypackage";
        private const string k_AdMobAddon = "https://dl.dropboxusercontent.com/s/9q62oytkw7ws59j/GoogleMobileAdsClientv3.unitypackage";

        public static void DrawAddonRequestUI(UM_Addon addon) 
        {
            EditorGUILayout.HelpBox("Ultimate Mobile " + addon + " Addon required", MessageType.Warning);
            using (new SA_GuiBeginHorizontal()) 
            {
                GUILayout.FlexibleSpace();
                var content = new GUIContent(" " + addon + " Addon", UM_Skin.GetPlatformIcon("unity_icon.png"));
                var click = GUILayout.Button(content, EditorStyles.miniButton, GUILayout.Width(120), GUILayout.Height(18));
                if (!click) return;
                
                string url;
                switch (addon) {
                    case UM_Addon.AdMob:
                        url = k_AdMobAddon;
                        break;
                    case UM_Addon.Playmaker:
                        url = k_PlaymakerAddon;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("addon", addon, null);
                }
                SA_PackageManager.DownloadAndImport(addon + " Addon", url, interactive: false);
            }
        }
    }
}