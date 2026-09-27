using UnityEngine;

public class MinigameLoadState : BasicState
{
	private string m_id;

	public MinigameLoadState(string _id)
	{
		m_id = _id;
	}

	private void MinigameDownloadOk(WWWRequest _request)
	{
		PsState.m_lastDownloadedLevelBytesZipped = _request.m_WWW.bytes;
		Main.m_currentGame.m_sceneManager.ChangeScene(new GameScene("GameScene"), new FadeLoadingScene(Color.black));
	}

	private void MinigameDownloadFailed(WWWRequest _request)
	{
		PsState.m_lastDownloadedLevelId = null;
		PsState.m_lastDownloadedLevelBytesZipped = null;
		Debug.LogError("Minigame Download Failed");
	}

	public override void Enter(IStatedObject _parent)
	{
		string text = "?id=" + m_id;
		WWWRequest wWWRequest = new GetRequest(IpConfig.ServerUrl + "/v1/minigame/data/find" + text, "MINIGAME_DOWNLOAD", false, 5f);
		PsState.m_lastDownloadedLevelId = m_id;
		wWWRequest.requestComplete += MinigameDownloadOk;
		wWWRequest.requestFailed += MinigameDownloadFailed;
	}

	public override void Execute()
	{
	}

	public override void Exit()
	{
		WWWRequestManager.RemoveRequestsWithTag("MINIGAME_DOWNLOAD");
	}
}
