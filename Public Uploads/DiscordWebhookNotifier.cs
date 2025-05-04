using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Net;
using System.Text;

[InitializeOnLoad]
public static class DiscordWebhookNotifier
{
    private static string webhookUrl = ""; // Replace with your webhook URL

    static DiscordWebhookNotifier()
    {
        EditorSceneManager.sceneSaved += OnSceneSaved;
    }

    private static void OnSceneSaved(Scene scene)
    {
        string currentTime = System.DateTime.Now.ToString("HH:mm:ss");
        string message = $"```Unity scene has been saved at {currentTime}```"; 

        SendDiscordMessage(message);
    }

    private static void SendDiscordMessage(string message)
    {
        using (var client = new WebClient())
        {
            var payload = "{\"content\":\"" + message + "\"}";
            client.Headers.Add(HttpRequestHeader.ContentType, "application/json");

            try
            {
                client.UploadString(webhookUrl, "POST", payload);
            }
            catch (WebException ex)
            {
                Debug.LogError("Failed to send message to Discord.");
            }
        }
    }
}
