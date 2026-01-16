using UnityEngine;
using UnityEngine.UI;
using SA.CrossPlatform.GameServices;
using SA.CrossPlatform.UI;

namespace SA.CrossPlatform.Samples
{
    public class UM_GameServiceAchievmentsExample : MonoBehaviour
    {
        [SerializeField] private Button m_NativeUIButton = null;
        [SerializeField] private Button m_LoadButton = null;

        [SerializeField] private UM_AchievmentsMetaView m_AchievmentMetaView = null;
        private void Start() 
        {
            m_AchievmentMetaView.gameObject.SetActive(false);
            
            m_LoadButton.onClick.AddListener(LoadMeta);
            m_NativeUIButton.onClick.AddListener(() => {
                var client = UM_GameService.AchievementsClient;
                client.ShowUI(UM_DialogsUtility.DisplayResultMessage);
            });
        }
        private void LoadMeta() 
        {
            var client = UM_GameService.AchievementsClient;
            client.Load(result => 
            {
                if(result.IsSucceeded) 
                {
                    foreach(var achievement in result.Achievements)
                    {
                        PrintAchievementInfo(achievement);
                        var view = Instantiate(m_AchievmentMetaView.gameObject, m_AchievmentMetaView.transform.parent);
                        view.SetActive(true);
                        view.transform.localScale = Vector3.one;

                        var meta = view.GetComponent<UM_AchievmentsMetaView>();
                        meta.SetTitle(achievement.Name + " / " + achievement.State);
                    }
                } else {
                    UM_DialogsUtility.DisplayResultMessage(result);
                }
            });
        }

        private void PrintAchievementInfo(UM_iAchievement achievement)
        {
            UM_Logger.Log("------------------------------------------------");
            UM_Logger.Log("achievement.Identifier: " + achievement.Identifier);
            UM_Logger.Log("achievement.Name: " + achievement.Name);
            UM_Logger.Log("achievement.State: " + achievement.State);
            UM_Logger.Log("achievement.Type: " + achievement.Type);
            UM_Logger.Log("achievement.TotalSteps: " + achievement.TotalSteps);
            UM_Logger.Log("achievement.CurrentSteps: " + achievement.CurrentSteps);
        }
    }
}
