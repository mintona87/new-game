using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;

public class SendStats : MonoBehaviour
{
	private int wins;
	private int losses;
	private bool won = false;

    private void Start()
    {
		Debug.Log("started");
        GetAndUpdateMatchResults();

    }

	// Send match results
	public void SendMatchResult()
	{
		var request = new UpdateUserDataRequest
		{
			Data = new Dictionary<string, string> {
				{"Wins", wins.ToString() },
				{"Losses", losses.ToString() },
			}
		};
		PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
	}
	void OnDataSend(UpdateUserDataResult result)
	{
		Debug.Log("Successful user WIN/LOSS data send!");
	}


	// Get previous match results, to account for win or loss
	public void GetAndUpdateMatchResults()
	{
		PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnReceiveToUpdate, OnError);
	}
	void OnReceiveToUpdate(GetUserDataResult result)
	{
		Debug.Log("Recieved user data!");
		if (result.Data != null && result.Data.ContainsKey("Wins") && result.Data.ContainsKey("Losses"))
		{
			if (won == true)
			{
				wins = int.Parse(result.Data["Wins"].Value) + 1;
				losses = int.Parse(result.Data["Losses"].Value);

				Debug.Log("WINS: " + wins);
				Debug.Log("LOSSES: " + losses);


				SendMatchResult();
			}
			else
			{
				wins = int.Parse(result.Data["Wins"].Value);
				losses = int.Parse(result.Data["Losses"].Value) + 1;

				Debug.Log("WINS: " + wins);
				Debug.Log("LOSSES: " + losses);


				SendMatchResult();
			}
		}
		else
		{
			Debug.Log("Player data not complete!");
		}

	}




	//UPDATE LEADERBOARD 
	public void SendLeaderboard(int score)
	{
		var request = new UpdatePlayerStatisticsRequest
		{
			Statistics = new List<StatisticUpdate> {
				new StatisticUpdate {
					StatisticName = "Zeitaku_Leaderboard", // <-  CHANGE YOUR LEADERBOARD NAME HERE!
                    Value = score
                    //Value = Random.Range(10,100) <-  Use this to test out random send data
                }
			}
		};
		PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdate, OnError);
	}
	void OnLeaderboardUpdate(UpdatePlayerStatisticsResult result)
	{
		Debug.Log("Successfull leaderboard sent!");
	}

	void OnError(PlayFabError error)
	{
		Debug.Log("Error: " + error.ErrorMessage);
	}
}
