using System.IO;
using System.Net.Http;
using BepInEx;
using TMPro;
using UnityEngine;

namespace REALMessageOfTheDay;

[BepInPlugin(Constants.PluginGuid, Constants.PluginName, Constants.PluginVersion)]
public class Plugin : BaseUnityPlugin
{
    private TextMeshPro motdTmp;

    private string messageOfTheDay;

    private bool canStart;

    private void OnGameInitialized()
    {
        motdTmp = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/motdBodyText")
            .GetComponent<TextMeshPro>();

        using (HttpClient client = new())
        {
            HttpResponseMessage motdResponse = client
                .GetAsync("https://raw.githubusercontent.com/HanSolo1000Falcon/GorillaInfo/main/MessageOfTheDay.txt")
                .Result;
            motdResponse.EnsureSuccessStatusCode();

            using (Stream stream = motdResponse.Content.ReadAsStreamAsync().Result)
            using (StreamReader reader = new(stream))
                messageOfTheDay = reader.ReadToEnd();

            canStart = true;
        }
    }

    private void Update()
    {
        if (!canStart)
            return;
        
        if (motdTmp.text != messageOfTheDay)
            motdTmp.text = messageOfTheDay;
    }

    private void Start() => GorillaTagger.OnPlayerSpawned(OnGameInitialized);
}