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
    public Text ListText;
    public Text RoomInfoText;
    public Text[] ChatText;
    public InputField ChatInput;

    [Header("ETC")]
    public Text StatusText;
    public PhotonView PV ;
    public GameObject[] P;
    public Image[] img;
    public GameObject timer;

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
        // �ִ�������
        maxPage = (myList.Count % CellBtn.Length == 0) ? myList.Count / CellBtn.Length : myList.Count / CellBtn.Length + 1;

        // ����, ������ư
        PreviousBtn.interactable = (currentPage <= 1) ? false : true;
        NextBtn.interactable = (currentPage >= maxPage) ? false : true;

        // �������� �´� ����Ʈ ����
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
        // ��Ʈ��ũ ����ǥ�� ����
        StatusText.text = PhotonNetwork.NetworkClientState.ToString();

        //�κ� ���Ӽ� �� �� ���Ӽ� ǥ�� ����
        LobbyInfoText.text = (PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms) + "�κ� / " + PhotonNetwork.CountOfPlayers + "����";
   
    }

    //������ ���� , Master������ �����ϸ� OnConnectedToMaster �ݹ�
    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    // Master����, ���°� �Ǹ� �κ� ����
    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();

    //JoinLobby() �ݹ��Լ��� ����
    public override void OnJoinedLobby()
    {
        Debug.Log("�κ�����");
        if (NickNameInput.text != "")
        {
            LobbyPanel.SetActive(true);
            PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
            WelcomeText.text = PhotonNetwork.LocalPlayer.NickName + "�� ȯ���մϴ�.";
            if (Max_Player == 2)
                RemoveParray(2);
            else if (Max_Player == 3)
                RemoveParray(3);
        }
        else
        {
            NickNameInput.GetComponent<Animator>().SetTrigger("on");
            PhotonNetwork.Disconnect();
        }

    }

    //���� �������
    public void Disconnect() => PhotonNetwork.Disconnect();

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (NickNameInput.text != "")
        {
            LobbyPanel.SetActive(false);
            RoomPanel.SetActive(false);
            NickNameInput.text = " ";
        }
        
    }

    public void CreateRoom()
    {
        if (RoomInput.text == "")
        {

        }
        else
        {
            Debug.Log("�����");

            // ���̸��� RoomInput.text�� ��ɼ��� �ִ��ο��� 2�� ���������� ����Ǹ� OnJoinedRoom �ݹ��Լ�����
            if (Max_Player == 2)
            {
               
                PhotonNetwork.CreateRoom(RoomInput.text, new RoomOptions { MaxPlayers = 2 });
                P[2].SetActive(false);
                P[3].SetActive(false);
            }
            else if (Max_Player == 3)
            {
                PhotonNetwork.CreateRoom(RoomInput.text, new RoomOptions { MaxPlayers = 3 });
                P[3].SetActive(false);
            }

            else if (Max_Player == 4)
                PhotonNetwork.CreateRoom(RoomInput.text, new RoomOptions { MaxPlayers = 4 });
            else
                PhotonNetwork.CreateRoom(RoomInput.text, new RoomOptions { MaxPlayers = 4 });
        }
    }

    public void RemoveParray(int num)
    {
        PV.RPC("RemoveParrayRPC", RpcTarget.All, num);
    }

    [PunRPC]
    public void RemoveParrayRPC(int num)
    {
        
        if(num == 2)
        {
            P[2].SetActive(false);
            P[3].SetActive(false);
  
        }
        else if(num == 3)
        {
            

        }
    }

    // �Լ� createRoom�� ���������� �������� ������ ��� ����
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        RoomInput.text = "";
    }

    // �Լ� JoinRandomRoom�� ���������� �������� ������ ��� ����
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomInput.text = "";
    }

    public void JoinRandomRoom() => PhotonNetwork.JoinRandomRoom();

    public void LeaveRoom() => PhotonNetwork.JoinRandomRoom();


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RoomRenewal();
        PV.RPC("ChatRPC", RpcTarget.All, "<color=yellow>" + newPlayer.NickName + "���� �����ϼ̽��ϴ�</color>");
        Debug.Log("����");


    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RoomRenewal();
        PV.RPC("ChatRPC", RpcTarget.All, "<color=yellow>" + otherPlayer.NickName + "���� �����ϼ̽��ϴ�</color>");
    }

    public void ImageSelect(int num)
    {
        
        int[] array = new int[2];
        array[0] = num;
        int i = 0;
        for (i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (PhotonNetwork.PlayerList[i].NickName == PhotonNetwork.LocalPlayer.NickName)
            {
                break;
            }


        }
        array[1] = i;
        PV.RPC("ImageSelectRPC", RpcTarget.All,array);


    }

    public void Ready()
    {
        int count = 0;
        int i = 0;
        for (i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (PhotonNetwork.PlayerList[i].NickName == PhotonNetwork.LocalPlayer.NickName)
            {
                break;
            }


        }

        PV.RPC("ReadyRPC", RpcTarget.All,i);


    }
    public void Gotimer()
    {
        PV.RPC("GotimerRPC", RpcTarget.All);
    }

    [PunRPC]
    public void GotimerRPC()
    {
        timer.SetActive(true);
    }


    void RoomRenewal()
    {
        List<string> namelist = new List<string>();
        
        
        ListText.text = "";
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            ListText.text += PhotonNetwork.PlayerList[i].NickName + ((i + 1 == PhotonNetwork.PlayerList.Length) ? "" : ", ");
            namelist.Add(PhotonNetwork.PlayerList[i].NickName);
        }
        for(int i = PhotonNetwork.PlayerList.Length; i<4;i++)
        {
            namelist.Add("");
        }
        for (int i = 0; i < P.Length; i++)
        {     
            P[i].transform.GetChild(1).GetComponent<Text>().text = namelist[i];
        }

        RoomInfoText.text = PhotonNetwork.CurrentRoom.Name + " / " + PhotonNetwork.CurrentRoom.PlayerCount + "�� / " + PhotonNetwork.CurrentRoom.MaxPlayers + "�ִ�";


    }


    public override void OnJoinedRoom()
    {
        RoomPanel.SetActive(true);
        RoomRenewal();
        ChatInput.text = "";
        for (int i = 0; i < ChatText.Length; i++) ChatText[i].text = "";
        Debug.Log("������");
    }

    public void Send()
    {
        string msg = PhotonNetwork.NickName + " : " + ChatInput.text;
        PV.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.NickName + " : " + ChatInput.text);
        ChatInput.text = "";
        
    }

    [PunRPC] // RPC�� �÷��̾ �����ִ� �� ��� �ο����� �����Ѵ�
    void ChatRPC(string msg)
    {
        bool isInput = false;
        for (int i = 0; i < ChatText.Length; i++)
            if (ChatText[i].text == "")
            {
                isInput = true;
                ChatText[i].text = msg;
                break;
            }
        if (!isInput) // ������ ��ĭ�� ���� �ø�
        {
            for (int i = 1; i < ChatText.Length; i++) ChatText[i - 1].text = ChatText[i].text;
            ChatText[ChatText.Length - 1].text = msg;
        }
    }

    [PunRPC]
    public void ImageSelectRPC(int[] num)
    {
        Debug.Log("�̹�������");


            P[num[1]].transform.GetChild(0).GetComponent<Image>().sprite = img[num[0]].GetComponent<Image>().sprite;

    }

    [PunRPC]
    public void ReadyRPC(int num)
    {
        int count = 0;
        
        if (P[num].transform.GetChild(2).GetComponent<Text>().text == "<color=#ff0000>" + "Ready" + "</color>")
        {
            P[num].transform.GetChild(2).GetComponent<Text>().text = "<color=#000000>" + "Ready" + "</color>";
        }
        else
        {
            P[num].transform.GetChild(2).GetComponent<Text>().text = "<color=#ff0000>" + "Ready" + "</color>";
        }

        for (int i = 0; i< PhotonNetwork.PlayerList.Length; i++)
        {
            if (P[i].transform.GetChild(2).GetComponent<Text>().text == "<color=#ff0000>" + "Ready" + "</color>")
                count = count + 1;
        }

        if (count == PhotonNetwork.PlayerList.Length && PhotonNetwork.PlayerList.Length == Max_Player)
            Gotimer();

    }
    

}
