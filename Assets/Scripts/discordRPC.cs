using UnityEngine;
using Discord;
using UnityEditor;
#if UNITY_EDITOR
[InitializeOnLoad]
#endif
[ExecuteInEditMode]
public class discordRPC : MonoBehaviour
{
    //public bool running;
    Discord.Discord discord = new Discord.Discord(990054065100689468, (System.UInt64)Discord.CreateFlags.Default);
#if !UNITY_EDITOR
    void Start()
    {
        UpdatePresence(false);
    }
    void Update() {
        discord.RunCallbacks();
    }
#endif
    void OnDisable()
    {

    }
#if UNITY_EDITOR
    public void StopUpdate()
    {
        EditorApplication.update -= Callback;
        discord.Dispose();
        Debug.Log("Stop!");
    }
    public void StartUpdate()
    {
        //discord.Dispose();
        EditorApplication.update += Callback;
        UpdatePresence(true);
        Debug.Log("Start!");
    }
#endif
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
        if (GUILayout.Button("Start"))
        {
            man.StartUpdate();

        }
        if (GUILayout.Button("Stop"))
        {
            man.StopUpdate();

        }
        if (GUILayout.Button("Update Presence"))
        {
            man.UpdatePresence(true);

        }
    }
}
#endif