using System.Collections.Generic;
using UnityEngine;

public class EditorPublishState : BasicState
{
	private UICanvas m_publishPopupContainer;

	public override void Enter(IStatedObject _parent)
	{
		m_publishPopupContainer = new UICanvas(null, "PublishPopupContainer", null, string.Empty);
		m_publishPopupContainer.SetWidth(1f, RelativeTo.ScreenWidth);
		m_publishPopupContainer.SetHeight(1f, RelativeTo.ScreenHeight);
		m_publishPopupContainer.SetMargins(0.125f, 0.125f, 0f, 0f, RelativeTo.ScreenWidth);
		m_publishPopupContainer.SetDrawHandler(UIDrawHandlers.EditorPopupBackground);
		UIVerticalList uIVerticalList = new UIVerticalList(m_publishPopupContainer, "PublishPopup");
		uIVerticalList.SetHeight(0.764f, RelativeTo.ParentHeight);
		uIVerticalList.RemoveDrawHandler();
		new UIPopupHeader(uIVerticalList, "SavePopupHeader", "publishing...", "Sending to server, please wait.");
		m_publishPopupContainer.Update();
		SendLevelToServer();
	}

	private void SendLevelToServer()
	{
		if (UserLogin.m_userLoginState == UserLoginState.LOGGED_IN)
		{
			Minigame minigame = LevelManager.m_currentLevel as Minigame;
			minigame.m_published = true;
			DataBlob dataBlob = ClientTools.CreateLevelDataBlob(minigame);
			string text = string.Empty;
			if ((LevelManager.m_currentLevel as Minigame).m_minigameId != null)
			{
				text = "?id=" + minigame.m_minigameId;
			}
			WWWRequest wWWRequest = new PostRequest(IpConfig.ServerUrl + "/v1/minigame/publish" + text, dataBlob.data, dataBlob.header, string.Empty, false, 5f);
			wWWRequest.requestComplete += LevelSendToServerOk;
			wWWRequest.requestFailed += LevelSendToServerFailed;
		}
		else
		{
			Debug.LogError("User not logged in!!");
			Main.m_currentGame.m_sceneManager.m_currentScene.m_stateMachine.ChangeState(new EditorBaseState());
		}
	}

	private void LevelSendToServerOk(WWWRequest _request)
	{
		Dictionary<string, object> dictionary = ClientTools.ParseServerResponse(_request.m_WWW.text);
		if (ClientTools.ServerResponseOk(dictionary))
		{
			(LevelManager.m_currentLevel as Minigame).m_minigameId = (string)dictionary["id"];
			PsState.FlushPendingScreenshot((string)dictionary["id"]);
			Debug.Log("MINIGAME SAVED");
		}
		else
		{
			Debug.LogError("MINIGAME SAVE FAILED");
		}
		Main.m_currentGame.m_sceneManager.ChangeScene(new MenuScene("MenuScene"), new FadeLoadingScene(Color.black));
	}

	private void LevelSendToServerFailed(WWWRequest _request)
	{
		Debug.LogError("MINIGAME SAVE FAILED");
		Main.m_currentGame.m_sceneManager.m_currentScene.m_stateMachine.ChangeState(new EditorBaseState());
	}

	public override void Execute()
	{
	}

	public override void Exit()
	{
		if (m_publishPopupContainer != null)
		{
			m_publishPopupContainer.Destroy();
			m_publishPopupContainer = null;
		}
	}
}
