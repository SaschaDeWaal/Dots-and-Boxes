using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

public class AccountManager : MonoBehaviour {

	private const string SERVER = "http://studenthome.hku.nl/~Sascha.deWaal/land/api/";
	private const int KEEP_ALIVE_TIME = 10; //<--min

	public GameObject LoginUI;

	private string sessionID = "";

	public IEnumerator Login(string mail, string pass, System.Action<bool, string> result)
	{
		StopCoroutine(KeepLoginAlive());

		WWWForm form = new WWWForm();
		form.AddField("mail", mail);
		form.AddField("pass", pass);

		UnityWebRequest www = UnityWebRequest.Post(SERVER + "login.php", form);
		yield return www.Send();

		if (www.isError)
		{
			result(true, www.error);
		}
		else
		{
			JSONNode data = JSON.Parse(www.downloadHandler.text);
			if (data["result"].Value.IndexOf("error") != -1)
			{
				result(true, data["data"].Value);
			}
			else
			{
				sessionID = data["sessionID"].Value;
				StartCoroutine(KeepLoginAlive());
				result(false, "");
			}
		}
	}

	private IEnumerator KeepLoginAlive()
	{
		while (true)
		{
			//wait for next
			yield return new WaitForSeconds(KEEP_ALIVE_TIME * 60);

			//Login to keep the session going.
			WWWForm form = new WWWForm();
			form.AddField("sessionID", sessionID);

			UnityWebRequest www = UnityWebRequest.Post(SERVER + "keepAlive.php", form);
			yield return www.Send();

			if (www.isError)
			{
				LostConnection();
			}
			else
			{
				JSONNode data = JSON.Parse(www.downloadHandler.text);
				if (data["result"].Value.IndexOf("error") != -1)
				{
					LostConnection();
				}
			}

		}
	}

	private void LostConnection()
	{
		Debug.Log("Need to be logged in again");
		LoginUI.SetActive(true);
	}
}
