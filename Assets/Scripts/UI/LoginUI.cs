using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour {

	private const string WAITING_BUTTON_TEXT = "Loading....";
	private const string LOGIN_BUTTON_TEXT = "Login";

	public GameObject msgPanel;
	public InputField mailInput;
	public InputField passInput;
	public Text buttonText;

	public GameObject MatchMaker;

	private bool waitingOnLogin = false;

	private void Start() {
		mailInput.text = PlayerPrefs.GetString("mail");
		passInput.text = PlayerPrefs.GetString("pass");
	}

	private void OnEnable() {
		gameObject.SetActive(!Manager.Find<AccountManager>().IsLoggedIn);
	}

	public void CreateAccount() {
		Application.OpenURL("http://studenthome.hku.nl/~Sascha.deWaal/land/index.php?p=create");
	}

	public void Login() {
		if (mailInput.text.Length < 3 && passInput.text.Length < 3) {
			ShowMsg("Login error", "Please, enter a valid mail and password");
		} else {
			if (!waitingOnLogin) {
				waitingOnLogin  = true;
				buttonText.text = WAITING_BUTTON_TEXT;
				StartCoroutine(Manager.Find<AccountManager>().Login(mailInput.text, passInput.text, LoginReady));
			}
		}
	}

	public void CloseMsgBox() {
		if (msgPanel) {
			msgPanel.SetActive(false);
		} else {
			Debug.LogWarning("msgPanel missing");
		}
	}

	private void LoginReady(bool isError, string result){

		waitingOnLogin  = false;
		buttonText.text = LOGIN_BUTTON_TEXT;

		if (isError) {
			ShowMsg("Error", result);
		} else {

			PlayerPrefs.SetString("mail", mailInput.text);
			PlayerPrefs.SetString("pass", passInput.text);
			PlayerPrefs.Save();

			MatchMaker.SetActive(true);
			gameObject.SetActive(false);
		}
	}

	private void ShowMsg(string title, string text) {
		if (msgPanel) {
			msgPanel.transform.Find("Title").GetComponent<Text>().text = title;
			msgPanel.transform.Find("Text").GetComponent<Text>().text  = text;
			msgPanel.SetActive(true);
		} else {
			Debug.LogWarning("msgPanel missing");
		}
	}
}
