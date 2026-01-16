using SA.Android.Utilities;
using SA.CrossPlatform.App;
using UnityEngine;
using UnityEngine.UI;

namespace SA.CrossPlatform.Samples
{
    public class UM_ContactsSample : MonoBehaviour
    {
        [SerializeField] Button m_LoadContactsButton = null;
        
        private void Start() 
        {
            m_LoadContactsButton.onClick.AddListener(() => 
            {
                LoadContacts();
            });
        }

        private void LoadContacts() 
        {
            var client = UM_Application.ContactsService;
            client.Retrieve(result => 
            {
                if(result.IsSucceeded) 
                {
                    foreach(var contact in result.Contacts) 
                    {
                        AN_Logger.Log("---------->");
                        AN_Logger.Log("contact.Name:" + contact.Name);
                        AN_Logger.Log("contact.Phone:" + contact.Phone);
                        AN_Logger.Log("contact.Email:" + contact.Email);
                    }
                } else {
                    AN_Logger.Log("Failed to load contacts: " + result.Error.FullMessage);
                }
            });
        }
    }
}