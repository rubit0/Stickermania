using System.Collections.Generic;
using SA.Android.GMS.Games;
using SA.Android.GMS.Games.Multiplayer;
using SA.Android.OS;
using SA.CrossPlatform.UI;
using UnityEngine;
using UnityEngine.UI;

namespace SA.CrossPlatform.Samples
{
	public class UM_GameserviceMultiplayerSample : MonoBehaviour, AN_iRoomUpdateCallback, AN_iOnRealTimeMessageReceivedListener, AN_iRoomStatusUpdateCallback
	{
		[SerializeField] private Button m_CreateRoomButton = null;

		private void Start()
		{
			m_CreateRoomButton.onClick.AddListener(() =>
			{
				var exclusiveBitMask = 1;
				var minAutoMatchPlayers = 2;
				var maxAutoMatchPlayers = 4;
				var autoMatchCriteria = AN_RoomConfig.CreateAutoMatchCriteria(minAutoMatchPlayers, maxAutoMatchPlayers, exclusiveBitMask);

				var builder = AN_RoomConfig.NewBuilder(this);
				builder.SetOnMessageReceivedListener(this);
				builder.SetRoomStatusUpdateCallback(this);
				builder.SetAutoMatchCriteria(autoMatchCriteria);

				var roomConfig = builder.Build();
				var client = AN_Games.GetRealTimeMultiplayerClient();
				client.Create(roomConfig);
			});
		}

		public void OnJoinedRoom(int statusCode, AN_Room room)
		{
			throw new System.NotImplementedException();
		}

		public void OnLeftRoom(int statusCode, string roomId)
		{
			throw new System.NotImplementedException();
		}

		public void OnRoomConnected(int statusCode, AN_Room room)
		{
			throw new System.NotImplementedException();
		}

		public void OnRoomCreated(int statusCode, AN_Room room)
		{
			UM_DialogsUtility.ShowMessage("OnRoomCreated", "Status Code: " + statusCode);
		}

		public void OnRealTimeMessageReceived(AN_RealTimeMessage message)
		{
			throw new System.NotImplementedException();
		}

		public void OnConnectedToRoom(AN_Room room)
		{
			throw new System.NotImplementedException();
		}

		public void OnDisconnectedFromRoom(AN_Room room)
		{
			throw new System.NotImplementedException();
		}

		public void OnP2PConnected(string participantId)
		{
			throw new System.NotImplementedException();
		}

		public void OnP2PDisconnected(string participantId)
		{
			throw new System.NotImplementedException();
		}

		public void OnPeerDeclined(AN_Room room, List<string> participantIds)
		{
			throw new System.NotImplementedException();
		}

		public void OnPeerInvitedToRoom(AN_Room room, List<string> participantIds)
		{
			throw new System.NotImplementedException();
		}

		public void OnPeerJoined(AN_Room room, List<string> participantIds)
		{
			throw new System.NotImplementedException();
		}

		public void OnPeerLeft(AN_Room room, List<string> participantIds)
		{
			throw new System.NotImplementedException();
		}

		public void OnPeersConnected(AN_Room room, List<string> participantIds)
		{
			throw new System.NotImplementedException();
		}

		public void OnPeersDisconnected(AN_Room room, List<string> participantIds)
		{
			throw new System.NotImplementedException();
		}

		public void OnRoomAutoMatching(AN_Room room)
		{
			throw new System.NotImplementedException();
		}

		public void OnRoomConnecting(AN_Room room)
		{
			throw new System.NotImplementedException();
		}
	}
}
