using UnityEngine;

public class DataTranslator : MonoBehaviour {
    public int TextToData(string dataText, string dataName) {
        string[] allData = dataText.Split('/');
        // Debug.Log("allData: " + allData);

        int dataValue = -1;
        foreach(string data in allData) {
            // Debug.Log("data: " + data);
            if(data.Contains(dataName)) {
                string completeData = data.Replace("(" + dataName + ")", "");
                Debug.Log("(" + dataName + "): " + completeData);
                dataValue = int.Parse(completeData);
                break;
            }
        }
        
        return dataValue;
    }

    public string DataToText(int kills, int deaths) {
        //Get Account Data
        UserInfo currentUser = DatabaseManager.instance.GetUserData();

        int total_kills = currentUser.total_kills + kills;
        int total_deaths = currentUser.total_deaths + deaths;

        string dataText = "(kills)" + total_kills + "/" + "(deaths)" + total_deaths;
        Debug.Log("dataText: " + dataText);
        return dataText;
    }
}