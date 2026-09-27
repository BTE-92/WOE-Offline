using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class PostRequestQueue
{
	private enum SendState
	{
		IDLE = 0,
		SENDING = 1
	}

	private const float TIME_BETWEEN_FAILED_SENDS = 5f;

	private static List<PostCommand> m_sendQueue = new List<PostCommand>();

	private static WWWRequest m_currentRequest;

	private static SendState m_state;

	private static float m_time;

	public static void Update()
	{
		if (m_state == SendState.IDLE && m_sendQueue.Count > 0 && m_time - Time.realtimeSinceStartup <= 0f)
		{
			PostCommand postCommand = m_sendQueue[0];
			Debug.Log("Sending command: " + postCommand.m_command);
			m_currentRequest = new PostRequest(postCommand.m_command, string.Empty, false, 5f);
			m_currentRequest.requestComplete += DataSendOk;
			if (postCommand.m_successEventHandler != null)
			{
				m_currentRequest.requestComplete += postCommand.m_successEventHandler;
			}
			m_currentRequest.requestFailed += DataSendFailed;
			if (postCommand.m_failEventHandler != null)
			{
				m_currentRequest.requestFailed += postCommand.m_failEventHandler;
			}
			m_state = SendState.SENDING;
		}
	}

	public static void AddToQueue(string _command, Action<WWWRequest> _successEventHandler = null, Action<WWWRequest> _failEventHandler = null)
	{
		Debug.Log("Adding to send queue: " + _command);
		m_sendQueue.Add(new PostCommand(_command, _successEventHandler, _failEventHandler));
	}

	public static void RemoveCustomEventHandlersFromPostRequest(string _command)
	{
		for (int i = 0; i < m_sendQueue.Count; i++)
		{
			PostCommand postCommand = m_sendQueue[i];
			if (!(postCommand.m_command == _command))
			{
				continue;
			}
			if (i == 0)
			{
				if (postCommand.m_successEventHandler != null)
				{
					m_currentRequest.requestComplete -= postCommand.m_successEventHandler;
				}
				if (postCommand.m_failEventHandler != null)
				{
					m_currentRequest.requestFailed -= postCommand.m_failEventHandler;
				}
			}
			m_sendQueue[i].m_successEventHandler = null;
			m_sendQueue[i].m_failEventHandler = null;
		}
	}

	private static void DataSendOk(WWWRequest req)
	{
		Dictionary<string, object> dictionary = ClientTools.ParseServerResponse(req.m_WWW.text);
		m_sendQueue.RemoveAt(0);
		m_state = SendState.IDLE;
		m_time = Time.realtimeSinceStartup;
		if (ClientTools.ServerResponseOk(dictionary))
		{
			Debug.Log("DATAQUEUE: Data send OK");
		}
		else
		{
			Debug.LogError("DATAQUEUE SERVER ERROR: " + (string)dictionary["status"]);
		}
	}

	private static void DataSendFailed(WWWRequest req)
	{
		m_state = SendState.IDLE;
		m_time = Time.realtimeSinceStartup + 5f;
		Debug.Log("DATAQUEUE: Data Send Failed");
	}

	public static void SaveQueueToDisk()
	{
		if (m_sendQueue.Count <= 0)
		{
			return;
		}
		if (File.Exists(GetFilePath()))
		{
			File.Delete(GetFilePath());
		}
		StreamWriter streamWriter = new StreamWriter(GetFilePath());
		foreach (PostCommand item in m_sendQueue)
		{
			streamWriter.WriteLine(item.m_command);
		}
		streamWriter.Close();
		Debug.Log("DATAQUEUE: Saved to disk. Len: " + m_sendQueue.Count);
	}

	public static void LoadQueueFromDisk()
	{
		if (File.Exists(GetFilePath()))
		{
			m_sendQueue.Clear();
			StreamReader streamReader = new StreamReader(GetFilePath());
			while (streamReader.Peek() > 0)
			{
				m_sendQueue.Add(new PostCommand(streamReader.ReadLine()));
			}
			streamReader.Close();
			Debug.Log("DATAQUEUE: Loaded from disk. Len: " + m_sendQueue.Count);
		}
	}

	private static string GetFilePath()
	{
		return LevelIO.GetApplicationRunPath() + "/DataQueue.dat";
	}
}
