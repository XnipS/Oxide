using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
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
    public TMP_InputField inp_name;
    string myName;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            Host();
        }
    }

    void Start()
    {
        myName = "Survivor";
        string path = Application.persistentDataPath + "/saved.nip";
        if (File.Exists(path))
        {
            string[] data = ReadSettings();
            inp_ip.text = data[0];
            inp_name.text = data[1];
            myName = data[1];
            FindObjectOfType<oxideNetworkManager>().playerName = data[1];
        }
        else
        {
            string[] data = new string[2];
            data[0] = "localhost";
            data[1] = "Survivor";
            WriteSettings(data);
        }

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
        string[] data = new string[2];
        data[0] = inp_ip.text;
        data[1] = inp_name.text;
        WriteSettings(data);
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

    public static void WriteSettings(string[] data)
    {
        string path = Application.persistentDataPath + "/saved.nip";
        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, false);
        string write = "";
        foreach (string s in data)
        {
            write += s + "\n";
        }
        writer.Write(write);
        writer.Close();
        StreamReader reader = new StreamReader(path);
        reader.Close();
    }
    public static string[] ReadSettings()
    {
        string path = Application.persistentDataPath + "/saved.nip";
        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path);
        string input = reader.ReadToEnd();
        string[] output = input.Split("\n");
        reader.Close();
        return output;
    }
}
