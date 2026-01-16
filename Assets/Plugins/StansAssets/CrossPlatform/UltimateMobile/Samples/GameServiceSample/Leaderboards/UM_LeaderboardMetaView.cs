using UnityEngine;
using UnityEngine.UI;

namespace SA.CrossPlatform.Samples
{
	public class UM_LeaderboardMetaView : MonoBehaviour
	{
		[SerializeField] private RawImage m_Icon;
		[SerializeField] private Text m_Title = null;

		public void SetTitle(string title)
		{
			m_Title.text = title;
		}
	}
}