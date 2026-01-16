using UnityEditor;
using UnityEngine;
using SA.Foundation.Editor;

namespace SA.CrossPlatform
{
    public static class UM_Skin
    {
        private const string ICONS_PATH = UM_Settings.PLUGIN_FOLDER + "Editor/Art/Icons/";

        public static Texture2D SettingsWindowIcon 
        {
            get
            {
                if (EditorGUIUtility.isProSkin) 
                {
                    return GetDefaultIcon("ultimate_icon_pro.png");
                }

                return GetDefaultIcon("ultimate_icon.png");
            }
        }

        public static Texture2D GetServiceIcon(string iconName) 
        {
            return SA_EditorAssets.GetTextureAtPath(ICONS_PATH + "Services/" + iconName);
        }

        public static Texture2D GetPlatformIcon(string iconName) 
        {
            return SA_EditorAssets.GetTextureAtPath(ICONS_PATH + "Platforms/" + iconName);
        }

        public static Texture2D GetDefaultIcon(string iconName) 
        {
            return SA_EditorAssets.GetTextureAtPath(ICONS_PATH + "Default/" + iconName);
        }

        private static GUIStyle m_PlatformBlockHeader = null;
        public static GUIStyle PlatformBlockHeader 
        {
            get 
            {
                if (m_PlatformBlockHeader == null) 
                {
                    m_PlatformBlockHeader = new GUIStyle(SA_PluginSettingsWindowStyles.ServiceBlockHeader);
                    m_PlatformBlockHeader.fontSize = 11;
                }

                return m_PlatformBlockHeader;
            }
        }
    }
}