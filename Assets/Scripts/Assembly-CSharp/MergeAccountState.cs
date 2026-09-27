using System.Collections;
using System.Collections.Generic;
using MiniJSON;

public class MergeAccountState : BasicState
{
	public UICanvas m_canvas;

	public UITextButton m_firstButton;

	public UITextButton m_secondButton;

	private PlayerData[] m_players;

	private PlayerData m_selectedPlayer;

	public MergeAccountState(PlayerData[] _players)
	{
		m_players = _players;
	}

	public override void Enter(IStatedObject _parent)
	{
		Debug.Log("Merge account state Enter");
		m_canvas = new UICanvas(null, "Canvas", null, string.Empty);
		m_canvas.SetMargins(0.025f);
		m_canvas.SetHeight(0.8f, RelativeTo.ScreenHeight);
		m_canvas.SetVerticalAlign(1f);
		UIHorizontalList uIHorizontalList = new UIHorizontalList(m_canvas, string.Empty);
		uIHorizontalList.SetSpacing(0.1f, RelativeTo.ParentHeight);
		uIHorizontalList.SetMargins(0.05f, RelativeTo.ParentHeight);
		m_firstButton = new UITextButton(uIHorizontalList, string.Empty, m_players[0].name, "Fonts/HurmeRegular", 0.05f);
		m_secondButton = new UITextButton(uIHorizontalList, string.Empty, m_players[1].name, "Fonts/HurmeRegular", 0.05f);
		m_canvas.Update();
	}

	public override void Execute()
	{
		if (m_firstButton != null)
		{
			if (m_firstButton.m_hit)
			{
				m_selectedPlayer = m_players[0];
				SendRequest(m_players[0].playerId, m_players[1].playerId);
			}
			else if (m_secondButton.m_hit)
			{
				m_selectedPlayer = m_players[1];
				SendRequest(m_players[1].playerId, m_players[0].playerId);
			}
		}
	}

	private void SendRequest(string _selected, string _notSelected)
	{
		Debug.Log(_selected + ", " + _notSelected);
		m_canvas.Destroy();
		m_canvas = null;
		m_firstButton = null;
		m_secondButton = null;
		string text = IpConfig.ServerUrl + "/v1/player/merge";
		text = text + "?activeId=" + _selected;
		text = text + "&removeId=" + _notSelected;
		text = text + "&hash=" + ClientTools.GenerateHash(_selected + _notSelected);
		WWWRequest wWWRequest = new PostRequest(text, string.Empty, false, 5f);
		wWWRequest.requestComplete += MergeOk;
		wWWRequest.requestFailed += MergeFailed;
	}

	private void MergeOk(WWWRequest req)
	{
		Dictionary<string, object> dict = ClientTools.ParseServerResponse(req.m_WWW.text);
		if (ClientTools.ServerResponseOk(dict))
		{
			Debug.Log("Merge success");
			if (m_selectedPlayer.playerId != PlayerPrefsX.GetUserId())
			{
				PlayerPrefsX.SetPlayerData(m_selectedPlayer);
			}
			ServerManager.Login(LoginSuccess, LoginFailure, GenerateLoginJson());
			Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new MenuState());
		}
		else
		{
			Debug.LogError("Merge Failed");
			Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new MenuState());
		}
	}

	private void MergeFailed(WWWRequest req)
	{
		Debug.LogError("Merge Failed");
		Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new MenuState());
	}

	private void LoginSuccess(WWWRequest req)
	{
		Debug.Log("Login after merge success");
		ServerManager.ReloadFriends();
		Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new MenuState());
	}

	private void LoginFailure(WWWRequest req)
	{
		Debug.Log("Login after merge failure");
		Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new MenuState());
	}

	private string GenerateLoginJson()
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("id", PlayerPrefsX.GetUserId());
		hashtable.Add("name", PlayerPrefsX.GetUserName());
		string gameCenterId = PlayerPrefsX.GetGameCenterId();
		if (gameCenterId != null)
		{
			hashtable.Add("gameCenterId", gameCenterId);
		}
		string facebookId = PlayerPrefsX.GetFacebookId();
		if (facebookId != null)
		{
			hashtable.Add("facebookId", facebookId);
		}
		string text = Json.Serialize(hashtable);
		Debug.Log("USER JSON GENERATED: " + text);
		return text;
	}
}
