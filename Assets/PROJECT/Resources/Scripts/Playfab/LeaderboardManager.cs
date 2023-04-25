using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;

public class LeaderboardManager : MonoBehaviour
{
    public GameObject playerEntryPrefab;
    public Transform leaderboardContent;

    private void Start()
    {
        GetLeaderboard();
    }

    private void GetLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "Honor_Leaderboard",
            StartPosition = 0,
            MaxResultsCount = 100
        };

        PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardReceived, OnPlayFabError);
    }

    private void OnLeaderboardReceived(GetLeaderboardResult result)
    {
        foreach (var entry in result.Leaderboard)
        {
            GameObject newEntry = Instantiate(playerEntryPrefab, leaderboardContent);

            newEntry.transform.SetParent(leaderboardContent);
            
            newEntry.GetComponent<PlayerRoomObjHandler>().SetUpPlayerInfo(
                0,
                entry.DisplayName,
                entry.StatValue.ToString(),
                "leaderboard"
                ) ;
            // You can set the player image here, e.g., by using a Coroutine to download it from a URL.
        }
    }

    private void OnPlayFabError(PlayFabError error)
    {
        Debug.LogError("PlayFab Error: " + error.GenerateErrorReport());
    }
}
