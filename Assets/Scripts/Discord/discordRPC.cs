using UnityEngine;
using Discord;
using UnityEditor;
using UnityEngine.SceneManagement;
using Mirror;

public class discordRPC : MonoBehaviour
{

    public bool running;
    Discord.Discord discord;

    void Start()
    {
        //Debug.Log("start");

        discord = new Discord.Discord(990054065100689468, (System.UInt64)Discord.CreateFlags.Default);
        UpdatePresence();
        SceneManager.sceneLoaded += LoadScene;
    }
    void Update()
    {
        discord.RunCallbacks();
    }

    void OnApplicationQuit()
    {
        //Debug.Log("Disposed Discord!");
        discord.Dispose();
    }

    void LoadScene(Scene scene, LoadSceneMode mode)
    {
        UpdatePresence();
    }

    public void UpdatePresence()
    {
        var activityManager = discord.GetActivityManager();
        Discord.Activity activity;

        activity = new Discord.Activity
        {
            Details = "On " + SceneManager.GetActiveScene().name,
            Assets = new ActivityAssets
            {
                LargeImage = "oxideicon_1024",
                LargeText = "Oxide",
            },
            Timestamps = new Discord.ActivityTimestamps
            {
                Start = System.DateTimeOffset.Now.ToUnixTimeSeconds()
            }
        };
        if (NetworkClient.isConnected)
        {
            activity.State = "In Game, " + NetworkServer.connections.Count + "/100 survivors";
        }
        else
        {
            activity.State = "In Menu";
        }

        activityManager.UpdateActivity(activity, (res) =>
    {
        if (res == Discord.Result.Ok)
        {
            //Debug.Log("Updated!");
        }
        else
        {
            Debug.LogError("DiscordRPC Error!");
        }
    });
    }

}

#if UNITY_EDITOR
[InitializeOnLoad]
//[CustomEditor(typeof(discordRPC))]
public class discordRPC_editor : EditorWindow
{
    Discord.Discord discord;

    // Add menu named "My Window" to the Window menu
    [MenuItem("Oxide/DiscordRPC")]
    static void Init()
    {
        Debug.Log("Begin");

        discordRPC_editor window = (discordRPC_editor)EditorWindow.GetWindow(typeof(discordRPC_editor));
        window.Innit();
        window.Show();
    }

    void Innit()
    {
        discord = new Discord.Discord(990054065100689468, (System.UInt64)Discord.CreateFlags.Default);
        var activityManager = discord.GetActivityManager();
        Discord.Activity activity;
        activity = new Discord.Activity
        {
            Details = "Oxide Editor v" + Application.version,
            State = "In Unity Editor",
            Assets = new ActivityAssets
            {
                LargeImage = "oxideicon_1024",
                LargeText = "Oxide",
                SmallImage = "unitylogo",
                SmallText = "Unity Engine"
            },
            Timestamps = new Discord.ActivityTimestamps
            {
                Start = System.DateTimeOffset.Now.ToUnixTimeSeconds()
            }
        };
        activityManager.UpdateActivity(activity, (res) =>
                {
                    if (res == Discord.Result.Ok)
                    {
                        Debug.Log("Updated!");
                    }
                    else
                    {
                        Debug.LogError("DiscordRPC Error!");
                    }
                });
    }
    void OnGUI()
    {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
    }
    void OnDestroy()
    {
        Debug.Log("Dispose");
        discord.Dispose();
    }

    void Update()
    {
        discord.RunCallbacks();
    }
}
#endif