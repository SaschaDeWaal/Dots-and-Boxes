using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using SimpleJSON;

public class AccountManager : MonoBehaviour {

	private const string SERVER = "http://studenthome.hku.nl/~Sascha.deWaal/land/api/";
	private const int KEEP_ALIVE_TIME = 10; //<--min

	public GameObject LoginUI;

	private string sessionID = "";
	private bool isLoggedIn = false;
	private JSONNode accountData = JSONNode.Parse("{}");

	public JSONNode AccountData {
		get {
			return accountData;
		}
	}

	public bool IsLoggedIn {
		get {
			return isLoggedIn;
		}
	}

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
			Debug.Log(www.downloadHandler.text);
			JSONNode data = JSON.Parse(www.downloadHandler.text);
			if (data["result"].Value.IndexOf("error") != -1)
			{
				result(true, data["data"].Value);
			}
			else
			{
				sessionID = data["sessionID"].Value;
				StartCoroutine(KeepLoginAlive());
				isLoggedIn = true;
				GetUserData((value) => accountData = value);

				result(false, "");
			}
		}
	}



	private IEnumerator KeepLoginAlive() {
		while (true) {

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
				Debug.Log(www.downloadHandler.text);
				JSONNode data = JSON.Parse(www.downloadHandler.text);
				if (data["result"].Value.IndexOf("error") != -1)
				{
					LostConnection();
				}
			}

		}
	}

	private IEnumerator ReqeustPage(string page, Dictionary<string, string> formToSend, System.Action<bool, JSONNode> result)
	{
		WWWForm form = new WWWForm();
		form.AddField("sessionID", sessionID);

		foreach (KeyValuePair<string, string> entry in formToSend) {
			form.AddField(entry.Key, entry.Value);
		}

		UnityWebRequest www = UnityWebRequest.Post(SERVER + page, form);
		yield return www.Send();

		if (www.isError){
			Debug.Log(www.error);
			if (result != null) result(true, null);
		}else{
			Debug.Log(www.downloadHandler.text);
			JSONNode data = JSON.Parse(www.downloadHandler.text);
			if (result != null) result(false, data);
		}
	}

	private void LostConnection()
	{
		LoginUI.SetActive(true);
		isLoggedIn = false;
	}

	public void GetUserData(System.Action<JSONNode> result) {
		Dictionary<string, string> form = new Dictionary<string, string>();
		StartCoroutine(ReqeustPage("accountInfo.php", form, (value, json) => result(json)));
	}

	public void UploadScore(int score, int place) {
		Dictionary<string, string> form = new Dictionary<string, string>();
		form.Add("score", score.ToString());
		form.Add("place", place.ToString());
		StartCoroutine(ReqeustPage("addScore.php", form, (value, json) => Debug.Log(json.ToString())));
	}

	public static IEnumerator LoadAvatar(string url, Image target) {
		Texture2D tex;
		tex = new Texture2D(4, 4, TextureFormat.DXT1, false);
		WWW www = new WWW(url);
		yield return www;
		www.LoadImageIntoTexture(tex);

		Debug.Log(url);

		if (tex && tex.width > 50 && tex.height > 50 && target)
		{
			Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
			target.sprite = sprite;
		}
	}
}
