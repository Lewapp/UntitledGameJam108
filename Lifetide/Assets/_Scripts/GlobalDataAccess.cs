using UnityEngine;

public class GlobalDataAccess : MonoBehaviour
{
    public PlayerInfo playerInfo;

    private void Awake()
    {
        GlobalData.playerInfo = null;

        if (!GlobalData.playerInfo)
        {
            GlobalData.playerInfo = playerInfo;
        }

        playerInfo.timeSurvived = 0;
        playerInfo.kills = 0;
        playerInfo.specialsKilled = 0;
    }

}

public static class GlobalData
{
    public static PlayerInfo playerInfo;
}
