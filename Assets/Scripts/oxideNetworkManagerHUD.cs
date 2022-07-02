using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mirror;

public class oxideNetworkManagerHUD : MonoBehaviour
{
    public Slider loadingBar;
    public GameObject menu;
    public GameObject loading;
    public Button btn_host;
    public Button btn_stop;
    public Button btn_client;
    public TMP_InputField inp_ip;
    public TMP_Text txt_connStatus;
    public GameObject unconnected;
    public GameObject connected;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            Host();
        }
    }

    void Start()
    {
        //Update the canvas text if you have manually changed network managers address from the game object before starting the game scene
        if (NetworkManager.singleton.networkAddress != "localhost") { inp_ip.text = NetworkManager.singleton.networkAddress; }

        //Adds a listener to the main input field and invokes a method when the value changes.
        inp_ip.onValueChanged.AddListener(delegate { ValueChangeCheck(); });

        //Make sure to attach these Buttons in the Inspector
        //btn_host.onClick.AddListener(Host);
        btn_client.onClick.AddListener(Client);
        btn_stop.onClick.AddListener(Stop);

    }
    void Stop()
    {
        // stop host if host mode
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        // stop client if client-only
        else if (NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopClient();
        }
        // stop server if server-only
        else if (NetworkServer.active)
        {
            NetworkManager.singleton.StopServer();
        }
        else
        {
            NetworkClient.Disconnect();
            // NetworkManager.singleton.disco
        }
        UpdateUI();
    }
    void ValueChangeCheck()
    {
        NetworkManager.singleton.networkAddress = inp_ip.text;
    }
    void UpdateUI()
    {
        if (!NetworkClient.isConnected && !NetworkServer.active)
        {
            if (NetworkClient.active)
            {
                unconnected.SetActive(false);
                connected.SetActive(true);
                txt_connStatus.text = "Connecting to " + NetworkManager.singleton.networkAddress + "..";
            }
            else
            {
                unconnected.SetActive(true);
                connected.SetActive(false);
            }
        }
        else
        {
            unconnected.SetActive(false);
            connected.SetActive(true);

            // server / client status message
            if (NetworkServer.active)
            {
                //serverText.text = "Server: active. Transport: " + Transport.activeTransport;
            }
            if (NetworkClient.isConnected)
            {
                //clientText.text = "Client: address=" + NetworkManager.singleton.networkAddress;
            }
        }
    }
    void Host()
    {
        NetworkManager.singleton.StartHost();
        UpdateUI();
    }
    void Client()
    {
        NetworkManager.singleton.StartClient();
        UpdateUI();
    }
}
