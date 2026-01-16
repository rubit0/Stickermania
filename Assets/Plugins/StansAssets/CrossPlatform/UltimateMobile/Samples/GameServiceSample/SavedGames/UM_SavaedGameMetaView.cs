using UnityEngine;
using UnityEngine.UI;

public class UM_SavaedGameMetaView : MonoBehaviour {
    
    [SerializeField] private Text m_Title = null;
    [SerializeField] private Button m_DeleteButton = null;
    [SerializeField] private Button m_GetDataButton = null;
        
    public Button DeleteButton
    {
        get { return m_DeleteButton; }
    }

    public Button GetDataButton
    {
        get { return m_GetDataButton; }
    }

    public void SetTitle(string title)
    {
        m_Title.text = title;
    }
}
