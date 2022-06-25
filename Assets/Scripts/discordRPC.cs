using System.Collections;
using UnityEngine;
using Discord;
using UnityEditor;
#if UNITY_EDITOR
[InitializeOnLoad]
[ExecuteInEditMode]
public class discordRPC : MonoBehaviour
{
    //public bool running;
    Discord.Discord discord = new Discord.Discord(990054065100689468, (System.UInt64)Discord.CreateFlags.Default);
    void OnEnable()
    {
        //Debug.Log("Enabled!");
        //EditorApplication.update += Callback;
    }
    void OnDisable()
    {

    }

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
        UpdatePresence();
        Debug.Log("Start!");
    }
    public void UpdatePresence()
    {
        var activityManager = discord.GetActivityManager();
        var activity = new Discord.Activity
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
        Debug.Log("callbacl");
    }

}
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
            man.UpdatePresence();

        }
    }
}
#endif