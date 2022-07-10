using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class developerConsole : MonoBehaviour
{
    public TMP_Text m_output;
    public TMP_InputField m_input;
    public Button m_submit;
    public GameObject m_console;
    public RectTransform m_content;
    bool m_enabled = false;

    void Start()
    {
        UpdateState(false);
        Application.logMessageReceived += TrackError;
        m_submit.onClick.AddListener(Submit);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            UpdateState(!m_enabled);
        }
        if (m_enabled && Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            Submit();
        }
    }

    void UpdateState(bool newState)
    {
        m_enabled = newState;
        m_console.SetActive(newState);
    }

    void AddTextToConsole(string str)
    {
        m_output.text += str + "\n";
        m_content.sizeDelta = new Vector2(m_output.rectTransform.sizeDelta.x, 30 * m_output.text.Split('\n').Length);
        m_output.rectTransform.sizeDelta = new Vector2(m_output.rectTransform.sizeDelta.x, 30 * m_output.text.Split('\n').Length);
    }

    void Submit()
    {
        AddTextToConsole(m_input.text);
        ExecuteCommand(m_input.text);
        m_input.text = "";
    }

    void TrackError(string cond, string stack, LogType type)
    {
        if (type != LogType.Warning)
        {
            //AddTextToConsole(cond);
        }
    }

    void ExecuteCommand(string input)
    {
        input = input.ToLower();
        string[] query = input.Split(" ");
        switch (query[0])
        {
            case "give":
                if (!NetworkClient.isHostClient) { AddTextToConsole("<color=#ff0000ff>No privileges!</color>"); break; }
                inv_item item = ScriptableObject.CreateInstance<inv_item>();
                item.id = int.Parse(query[1]);
                item.amount = int.Parse(query[2]);
                inv_item_data data = itemDictionary.singleton.GetDataFromItemID(item.id);
                if (data.maxAmmo > 0)
                {
                    item.ammoLoaded = data.maxAmmo;
                }
                if (data.maxDurability > 0)
                {
                    item.durability = data.maxDurability;
                }
                FindObjectOfType<ui_inventory>().GiveItem(item);
                AddTextToConsole("<color=#ff0000ff>Given " + data.title + "</color>");
                break;
            case "suicide":
                if (FindObjectOfType<ui_inventory>().player)
                {
                    FindObjectOfType<ui_inventory>().player.GetComponent<playerHealth>().CMD_TakeDamage(1000f, FindObjectOfType<ui_inventory>().player.GetComponent<NetworkIdentity>());
                    AddTextToConsole("<color=#ff0000ff>You commited suicide, you silly sod!</color>");
                }
                break;
            case "summon":
                if (!NetworkClient.isHostClient) { AddTextToConsole("<color=#ff0000ff>No privileges!</color>"); break; }
                GameObject summon_prefab = Resources.Load("SpawnablePrefabs/" + query[1]) as GameObject;
                GameObject summon = Instantiate(summon_prefab);
                NetworkServer.Spawn(summon);
                AddTextToConsole("<color=#ff0000ff>Summoned " + "SpawnablePrefabs/" + query[1] + "</color>");
                break;
            default:
                AddTextToConsole("<color=#ff0000ff>Unknown command :(</color>");
                break;
        }
    }
}
