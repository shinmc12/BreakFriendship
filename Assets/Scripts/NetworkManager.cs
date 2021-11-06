using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("DisconnectPanel")]
    public InputField NickNameInput;
    public GameObject DisconnectPanel;

    [Header("LobbyPanel")]
    public GameObject LobbyPanel;
    public InputField RoomInput;
    public Text WelcomeText;
    public Text LobbyInfoText;
    public Button[] CellBtn;
    public Button PreviousBtn;
    public Button NextBtn;

    [Header("RoomPanel")]
    public GameObject RoomPanel;

    [Header("ETC")]
    public Text StatusText;

    int Max_Player = 0;

    List<RoomInfo> myList = new List<RoomInfo>();
    int currentPage = 1, maxPage, multiple;

    public void MaxPlayer(int num)
    {
        Max_Player = num;
    }

    public void MyListClick(int num)
    {
        if (num == -2) --currentPage;
        else if (num == -1) ++currentPage;
        else PhotonNetwork.JoinRoom(myList[multiple + num].Name);
        MyListRenewal();
    }

    void MyListRenewal()
    {
        // 최대페이지
        maxPage = (myList.Count % CellBtn.Length == 0) ? myList.Count / CellBtn.Length : myList.Count / CellBtn.Length + 1;

        // 이전, 다음버튼
        PreviousBtn.interactable = (currentPage <= 1) ? false : true;
        NextBtn.interactable = (currentPage >= maxPage) ? false : true;

        // 페이지에 맞는 리스트 대입
        multiple = (currentPage - 1) * CellBtn.Length;
        for (int i = 0; i < CellBtn.Length; i++)
        {
            CellBtn[i].interactable = (multiple + i < myList.Count) ? true : false;
            CellBtn[i].transform.GetChild(0).GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].Name : "";
            CellBtn[i].transform.GetChild(1).GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].PlayerCount + "/" + myList[multiple + i].MaxPlayers : "";
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        int roomCount = roomList.Count;
        for (int i = 0; i < roomCount; i++)
        {
            if (!roomList[i].RemovedFromList)
            {
                if (!myList.Contains(roomList[i])) myList.Add(roomList[i]);
                else myList[myList.IndexOf(roomList[i])] = roomList[i];
            }
            else if (myList.IndexOf(roomList[i]) != -1) myList.RemoveAt(myList.IndexOf(roomList[i]));
        }
        MyListRenewal();
    }

    private void Awake()
    {
        Screen.SetResolution(960, 540, false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
    }

    private void Update()
    {
        // 네트워크 상태표시 변경
        StatusText.text = PhotonNetwork.NetworkClientState.ToString();

        //로비 접속수 및 총 접속수 표시 변경
        LobbyInfoText.text = (PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms) + "로비 / " + PhotonNetwork.CountOfPlayers + "접속";
   
    }

    //서버의 연결 , Master서버에 연결하면 OnConnectedToMaster 콜백
    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    // Master서버, 상태가 되면 로비에 참가
    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();

    //JoinLobby() 콜백함수로 실행
    public override void OnJoinedLobby()
    {
        LobbyPanel.SetActive(true);
        PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
        WelcomeText.text = PhotonNetwork.LocalPlayer.NickName + "님 환영합니다.";
        
    }

    //포톤 연결끊기
    public void Disconnect() => PhotonNetwork.Disconnect();

    public override void OnDisconnected(DisconnectCause cause)
    {
        LobbyPanel.SetActive(false);
        RoomPanel.SetActive(false);
        NickNameInput.text = " ";
    }

    public void CreateRoom()
    {
        if (RoomInput.text == "")
        {

        }
        else
        {
            // 방이름을 RoomInput.text로 방옵션을 최대인원수 2로 성공적으로 수행되면 OnJoinedRoom 콜백함수실행
            if(Max_Player == 2)
                PhotonNetwork.CreateRoom(RoomInput.text, new RoomOptions { MaxPlayers = 2 });
            else if (Max_Player == 3)
                PhotonNetwork.CreateRoom(RoomInput.text, new RoomOptions { MaxPlayers = 3 });
            else if (Max_Player == 4)
                PhotonNetwork.CreateRoom(RoomInput.text, new RoomOptions { MaxPlayers = 4 });   
            else
                PhotonNetwork.CreateRoom(RoomInput.text, new RoomOptions { MaxPlayers = 4 });
        }
    }

    // 함수 createRoom이 성공적으로 수행하지 못했을 경우 실행
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        RoomInput.text = "";
    }

    // 함수 JoinRandomRoom이 성공적으로 수행하지 못했을 경우 실행
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomInput.text = "";
    }

    public void JoinRandomRoom() => PhotonNetwork.JoinRandomRoom();

    public void LeaveRoom() => PhotonNetwork.JoinRandomRoom();

    public override void OnJoinedRoom()
    {
        RoomPanel.SetActive(true);

    }


}
