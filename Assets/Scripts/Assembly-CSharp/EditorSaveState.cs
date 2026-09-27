using System.Collections.Generic;

public class EditorSaveState : BasicState
{
	private UICanvas m_savePopupContainer;

	public override void Enter(IStatedObject _parent)
	{
		m_savePopupContainer = new UICanvas(null, "SavePopupContainer", null, string.Empty);
		m_savePopupContainer.SetWidth(1f, RelativeTo.ScreenWidth);
		m_savePopupContainer.SetHeight(1f, RelativeTo.ScreenHeight);
		m_savePopupContainer.SetMargins(0.125f, 0.125f, 0f, 0f, RelativeTo.ScreenWidth);
		m_savePopupContainer.SetDrawHandler(UIDrawHandlers.EditorPopupBackground);
		UIVerticalList uIVerticalList = new UIVerticalList(m_savePopupContainer, "SavePopup");
		uIVerticalList.SetHeight(0.764f, RelativeTo.ParentHeight);
		uIVerticalList.RemoveDrawHandler();
		new UIPopupHeader(uIVerticalList, "SavePopupHeader", "saving...", "Sending to server, please wait.");
		m_savePopupContainer.Update();
		SendLevelToServer();
	}

	private void SendLevelToServer()
	{
		if (UserLogin.m_userLoginState == UserLoginState.LOGGED_IN)
		{
			Minigame minigame = LevelManager.m_currentLevel as Minigame;
			minigame.m_name = LevelManager.m_currentLevel.m_name;
			minigame.m_description = LevelManager.m_currentLevel.m_description;
			minigame.m_published = false;
			DataBlob dataBlob = ClientTools.CreateLevelDataBlob(minigame);
			string text = string.Empty;
			if ((LevelManager.m_currentLevel as Minigame).m_minigameId != null)
			{
				text = "?id=" + minigame.m_minigameId;
			}
			WWWRequest wWWRequest = new PostRequest(IpConfig.ServerUrl + "/v1/minigame/save" + text, dataBlob.data, dataBlob.header, string.Empty, false, 5f);
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
			PsState.m_lastDownloadedLevelId = (string)dictionary["id"];
			Debug.Log("MINIGAME SAVED");
		}
		else
		{
			Debug.LogError("MINIGAME SAVE FAILED");
		}
		Main.m_currentGame.m_sceneManager.m_currentScene.m_stateMachine.ChangeState(new EditorBaseState());
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
		if (m_savePopupContainer != null)
		{
			m_savePopupContainer.Destroy();
			m_savePopupContainer = null;
		}
	}
}
