using UnityEngine;
using UnityEditor;
using SA.iOS;
using SA.Android;
using SA.CrossPlatform.GameServices;
using SA.Foundation.Editor;


namespace SA.CrossPlatform
{
    public class UM_GameServicesUI : UM_ServiceSettingsUI
    {
        private class GameKitSettings : UM_NativeServiceLayoutBasedSetting
        {
            public override SA_ServiceLayout Layout { get {return CreateInstance<ISN_GameKitUI>();}}

            public override bool IsEnabled {
                get {
                    return ISN_Preprocessor.GetResolver<ISN_GameKitResolver>().IsSettingsEnabled;
                }
            }
        }

        private class GooglePlaySettings : UM_NativeServiceLayoutBasedSetting
        {
            public override SA_ServiceLayout Layout { get { return CreateInstance<AN_GooglePlayFeaturesUI>(); } }

            public override bool IsEnabled 
            {
                get { return AN_Preprocessor.GetResolver<AN_GooglePlayResolver>().IsSettingsEnabled; }
            }
        }
        
        public override void OnLayoutEnable() 
        {
            base.OnLayoutEnable();
            AddPlatfrom(UM_UIPlatform.IOS, new GameKitSettings());
            AddPlatfrom(UM_UIPlatform.Android, new GooglePlaySettings());

            AddFeatureUrl("Getting Started", "https://unionassets.com/ultimate-mobile-pro/getting-started-750");


            AddFeatureUrl("Sing In", "https://unionassets.com/ultimate-mobile-pro/authentication-729#sing-in");
            AddFeatureUrl("Sing Out", "https://unionassets.com/ultimate-mobile-pro/authentication-729#sing-out");
            AddFeatureUrl("Player Avatar Image", "https://unionassets.com/ultimate-mobile-pro/authentication-729#player-avatar-image");
            AddFeatureUrl("Auth State", "https://unionassets.com/ultimate-mobile-pro/authentication-729#check-auth-state");
            
            AddFeatureUrl("Achievements", "https://unionassets.com/ultimate-mobile-pro/achievements-730");
            AddFeatureUrl("Leaderboards", "https://unionassets.com/ultimate-mobile-pro/leaderboards-731");
            AddFeatureUrl("Saved Games", "https://unionassets.com/ultimate-mobile-pro/saved-games-732");

            AddFeatureUrl("Editor API Emulation", "https://unionassets.com/ultimate-mobile-pro/editor-api-emulation-765");
        }


        public override string Title 
        {
            get { return "Game Services"; }
        }

        public override string Description 
        {
            get
            {
                return "Enables your users to track their best scores on a leaderboard, " +
                       "compare their achievements, save game progress and more.";
            }
        }

        protected override Texture2D Icon 
        {
            get { return UM_Skin.GetServiceIcon("um_game_icon.png"); }
        }


        protected override void OnServiceUI() 
        {
            Settings();
            EditorAPIEmulation();
        }


        private static GUIContent s_AndoirdSavedGamesContent = new GUIContent("Saved Games API [?]:", "Without Games API enabled only basic sing in flow can be used");
        private static GUIContent s_AndoirdRequestProfileContent = new GUIContent("Request Profile [?]:", "Request User Profile will be added to sing-in builder");
        private static GUIContent s_AndoirdRequestEmailContent = new GUIContent("Request Email [?]:", "Request User Email will be added to sing-in builder");
        private static GUIContent s_AndoirdRequestServerAuthCodeContent = new GUIContent("Request Server Auth Code[?]:", "Request  Server Auth Code will be added to sing-in builder");

        private static GUIContent s_IOSSavingAGameContent = new GUIContent("Saving A Game[?]:", "The saves API will allow you to provide your player an ability to save & load game progress at any time.");


        private void Settings() 
        {
            using (new SA_WindowBlockWithSpace(new GUIContent("Settings"))) 
            {
                using (new SA_H2WindowBlockWithSpace(new GUIContent("ANDROID"))) 
                {
                    AN_Settings.Instance.GooglePlayGamesAPI = SA_EditorGUILayout.ToggleFiled(AN_GooglePlayFeaturesUI.GamesLabelContent, AN_Settings.Instance.GooglePlayGamesAPI, SA_StyledToggle.ToggleType.EnabledDisabled);

                    UM_Settings.Instance.AndroidRequestEmail = SA_EditorGUILayout.ToggleFiled(s_AndoirdRequestEmailContent, UM_Settings.Instance.AndroidRequestEmail, SA_StyledToggle.ToggleType.EnabledDisabled);
                    UM_Settings.Instance.AndroidRequestProfile = SA_EditorGUILayout.ToggleFiled(s_AndoirdRequestProfileContent, UM_Settings.Instance.AndroidRequestProfile, SA_StyledToggle.ToggleType.EnabledDisabled);

                    UM_Settings.Instance.AndroidSavedGamesEnabled = SA_EditorGUILayout.ToggleFiled(s_AndoirdSavedGamesContent, UM_Settings.Instance.AndroidSavedGamesEnabled, SA_StyledToggle.ToggleType.EnabledDisabled);
                    UM_Settings.Instance.AndroidRequestServerAuthCode = SA_EditorGUILayout.ToggleFiled(s_AndoirdRequestServerAuthCodeContent, UM_Settings.Instance.AndroidRequestServerAuthCode, SA_StyledToggle.ToggleType.EnabledDisabled);

                    if (UM_Settings.Instance.AndroidRequestServerAuthCode) 
                    {
                        UM_Settings.Instance.AndroidGMSServerId = SA_EditorGUILayout.TextField("Server Id", UM_Settings.Instance.AndroidGMSServerId);
                    }
                }

                using (new SA_H2WindowBlockWithSpace(new GUIContent("iOS"))) 
                {
                    ISN_Settings.Instance.SavingAGame = SA_EditorGUILayout.ToggleFiled(s_IOSSavingAGameContent, ISN_Settings.Instance.SavingAGame, SA_StyledToggle.ToggleType.EnabledDisabled);
                }

                using (new SA_H2WindowBlockWithSpace(new GUIContent("EDITOR"))) 
                {
                    EditorGUILayout.HelpBox("Platform does not require any additional settings.", MessageType.Info);
                }
            }
        }

        private static readonly GUIContent s_PlayerId = new GUIContent("Id[?]:", "Player identifier.");
        private static readonly GUIContent s_PlayerAlias = new GUIContent("Alias[?]:", "Player Alias. " +
            "Typically, you never display the alias string directly in your user interface. " +
            "Instead DisplayName property.");
        private static readonly GUIContent s_PlayerDisplayName = new GUIContent("DisplayName[?]:", "Player display name.");
        private static readonly GUIContent s_PlayerAvatar = new GUIContent("Avatar Image[?]:", "The image will be used as signed player " +
            "avatar while you testing in editor mode.");



        private void EditorAPIEmulation() {
            using (new SA_WindowBlockWithSpace(new GUIContent("Editor API Emulation"))) 
            {
                using (new SA_H2WindowBlockWithSpace(new GUIContent("PLAYER"))) 
                {
                    var player = UM_Settings.Instance.GSEditorPlayer;
                    player.Id = SA_EditorGUILayout.TextField(s_PlayerId, player.Id);
                    player.Alias = SA_EditorGUILayout.TextField(s_PlayerAlias, player.Alias);
                    player.DisplayName = SA_EditorGUILayout.TextField(s_PlayerDisplayName, player.DisplayName);

                    using (new SA_GuiBeginHorizontal()) 
                    {
                        EditorGUILayout.LabelField(s_PlayerAvatar);
                        player.Avatar = (Texture2D)EditorGUILayout.ObjectField(player.Avatar, typeof(Texture2D), false, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    }
                }

                using (new SA_H2WindowBlockWithSpace(new GUIContent("ACHIEVEMENTS"))) 
                {
                    EditorGUILayout.HelpBox("Achievements Editor list is bound with iOS Achivement's list.", MessageType.Info);
                    ISN_GameKitUI.DrawAchievementsList();
                }

                using (new SA_H2WindowBlockWithSpace(new GUIContent("LEADERBOARDS"))) 
                {
                    EditorGUILayout.HelpBox("This data will only be used for an editor API emulation.", MessageType.Info);
                    DrawLeaderboardsList();
                }

            }
        }
        
        private static readonly GUIContent s_LeaderboardIdDLabel = new GUIContent("Leaderboard Id[?]:", "A unique identifier of this leaderboard.");
        private static readonly GUIContent s_LeaderboardNameLabel = new GUIContent("Leaderboard Title[?]:", "The Title of the leaderboard.");

        private void DrawLeaderboardsList() 
        {
            SA_EditorGUILayout.ReorderablList(UM_Settings.Instance.GSLeaderboards, GetLeaderboardDisplayName, DrawLeaderboardContent, () => {
                UM_Settings.Instance.GSLeaderboards.Add(new UM_LeaderboardMeta("my.new.leaderboard.id", "New Leaderboard"));
            });
        }

        private string GetLeaderboardDisplayName(UM_LeaderboardMeta leaderboard) 
        {
            return leaderboard.Title + "(" + leaderboard.Identifier + ")";
        }
        
        private static void DrawLeaderboardContent(UM_LeaderboardMeta leaderboard) 
        {
            leaderboard.Identifier = SA_EditorGUILayout.TextField(s_LeaderboardIdDLabel, leaderboard.Identifier);
            leaderboard.Title = SA_EditorGUILayout.TextField(s_LeaderboardNameLabel, leaderboard.Title);
        }
    }
}