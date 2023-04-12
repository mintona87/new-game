using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class LeaderboardDisplay : MonoBehaviour
{
    public void GetLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "PlayerRankings",
            StartPosition = 0,
            MaxResultsCount = 10
        };
        PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardGet, OnError);
    }
    void OnLeaderboardGet(GetLeaderboardResult result)
    {

        foreach (var item in result.Leaderboard)
        {
            Debug.Log(string.Format("PLACE: {0} | ID: {1} | VALUE: {2}",
                item.Position+1, item.DisplayName, item.StatValue));
        }
    }

    void OnError(PlayFabError error)
    {
        Debug.Log("Error while executing Playfab call!");
        Debug.Log(error.GenerateErrorReport());
    }

    private void Start()
    {
        GetLeaderboard();
    }
}
