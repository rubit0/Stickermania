using System.Text;
using UnityEngine;
using SA.CrossPlatform.GameServices;
using SA.CrossPlatform.UI;
using UnityEngine.UI;

namespace SA.CrossPlatform.Samples
{
    public class UM_GameServiceSavedGamesExample : MonoBehaviour
    {
        [Header("Fetch Games")]
        [SerializeField] private Button m_FetchSavedGamesButton = null;
        [SerializeField] private UM_SavaedGameMetaView m_SavedGameMetaView = null;
        
        [Header("Create New Save")]
        [SerializeField] private Button m_CreateNew = null;
        [SerializeField] private InputField m_SaveName = null;
        [SerializeField] private InputField m_SaveData = null;

        private UM_iSavedGamesClient m_Client;
        private void Start()
        {
            m_SavedGameMetaView.gameObject.SetActive(false);
            
            m_Client = UM_GameService.SavedGamesClient;
            m_CreateNew.onClick.AddListener(CreateNewSave);
            m_FetchSavedGamesButton.onClick.AddListener(FetchSavedGames);
        }

        private void FetchSavedGames()
        {
            //Remove Current List
            m_SavedGameMetaView.transform.parent.Clear(true);
            
            m_Client.FetchSavedGames(result =>
            {
                if (result.IsSucceeded)
                {
                    foreach (var snapshot in result.Snapshots)
                    {
                        var view = Instantiate(m_SavedGameMetaView.gameObject, m_SavedGameMetaView.transform.parent);
                        view.SetActive(true);
                        view.transform.localScale = Vector3.one;
                        
                        var meta = view.GetComponent<UM_SavaedGameMetaView>();
                        meta.SetTitle(snapshot.Name + " (" + snapshot.DeviceName + ")");
                        meta.DeleteButton.onClick.AddListener(() =>
                        {
                            DeleteGameSave(snapshot);
                        });
                        
                        meta.GetDataButton.onClick.AddListener(() =>
                        {
                            LoadData(snapshot);
                        });
                    }
                }
                else
                {
                    UM_DialogsUtility.DisplayResultMessage(result);
                }
            });
        }


        public void LoadData(UM_iSavedGameMetadata game)
        {
            var client = UM_GameService.SavedGamesClient;
            client.LoadGameData(game, (result) =>
            {
                if (result.IsSucceeded)
                {
                    var text = Encoding.ASCII.GetString(result.Data);
                    UM_DialogsUtility.ShowMessage("Saved Game Data Loaded", text);
                    //Restore your game progress here
                }
                else
                {
                    UM_DialogsUtility.DisplayResultMessage(result);
                }
            });
        }

        private void CreateNewSave()
        {
            var client = UM_GameService.SavedGamesClient;
            var bytes = Encoding.ASCII.GetBytes(m_SaveData.text);
            client.SaveGame(m_SaveName.text, bytes, result =>
            {
                if (result.IsSucceeded)
                {
                    FetchSavedGames();
                }
                else
                {
                    UM_DialogsUtility.DisplayResultMessage(result);
                }
            });
        }

        private void DeleteGameSave(UM_iSavedGameMetadata game)
        {
            m_Client.Delete(game, result =>
            {
                if (result.IsSucceeded)
                {
                    FetchSavedGames();
                }
                else
                {
                    UM_DialogsUtility.DisplayResultMessage(result);
                }
            });
        }
    }
}