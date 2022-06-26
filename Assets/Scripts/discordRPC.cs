using UnityEngine;
using Discord;
using UnityEditor;
#if UNITY_EDITOR
[InitializeOnLoad]
#endif
[ExecuteInEditMode]
public class discordRPC : MonoBehaviour
{

    public bool running;
    Discord.Discord discord;

    void OnEnable()
    {
        Debug.Log("start");
        if (discord == null)
        {
            discord = new Discord.Discord(990054065100689468, (System.UInt64)Discord.CreateFlags.Default);
        }
        else
        {
            Debug.Log("Disposed Discord!");
            discord.Dispose();
            discord = new Discord.Discord(990054065100689468, (System.UInt64)Discord.CreateFlags.Default);
        }
#if UNITY_EDITOR
        UpdatePresence(true);
#else
UpdatePresence(false);
#endif

    }
    void Update()
    {
        if (discord != null)
        {
            running = true;
            discord.RunCallbacks();
        }
        else
        {
            running = false;
        }
    }

    void OnDisable()
    {
        Debug.Log("Disposed Discord!");
        discord.Dispose();
    }
    public void UpdatePresence(bool editor)
    {
        var activityManager = discord.GetActivityManager();
        Discord.Activity activity;
        if (editor)
        {
            activity = new Discord.Activity
            {
                Details = "Developing v" + Application.version,
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
        }
        else
        {
            activity = new Discord.Activity
            {
                Details = "Playing v" + Application.version,
                State = "In Game",
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
        }

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
    void Callback()
    {
        discord.RunCallbacks();
        Debug.Log("callback");
    }

}
#if UNITY_EDITOR
[CustomEditor(typeof(discordRPC))]
public class discordRPC_editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        discordRPC man = (discordRPC)target;
        if (GUILayout.Button("Update Presence"))
        {
            man.UpdatePresence(true);

        }
    }
}
#endif