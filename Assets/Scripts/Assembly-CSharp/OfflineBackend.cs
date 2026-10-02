using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using MiniJSON;

public class OfflineResponse
{
	public byte[] Data;
	public string Error;
	public float DelaySeconds;

	public OfflineResponse(byte[] data)
	{
		Data = data;
	}
}

public class OfflineBackend
{
	private class PlayerRec
	{
		public string id;
		public string name;
		public string facebookId = string.Empty;
		public string gameCenterId = string.Empty;
		public bool acceptNotifications;
	}

	private class MinigameRec
	{
		public string id;
		public string name;
		public string description = string.Empty;
		public string creatorId;
		public string creatorName;
		public List<string> tags = new List<string>();
		public bool published;
		public long publishTime;
		public long timesPlayed;
		public long timesFinished;
		public long timesLiked;
		public bool isNew = true;
		public bool isClassic;
	}

	private class ScoreRec
	{
		public string gameId;
		public string playerId;
		public string name;
		public long time;
		public long starts;
		public long createdAt;
	}

	private class CommentRec
	{
		public string id;
		public string gameId;
		public string playerId;
		public string comment;
		public string facebookId = string.Empty;
		public string gameCenterId = string.Empty;
		public long createdAt;
	}

	private static readonly string[] DEFAULT_TAGS = new string[] { "TestTag1", "TestTag2" };

	private static readonly DateTime EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

	private readonly string m_root;
	private readonly string m_levelDir;
	private readonly string m_thumbDir;
	private readonly string m_dbPath;

	private readonly Dictionary<string, PlayerRec> m_players = new Dictionary<string, PlayerRec>();
	private readonly List<MinigameRec> m_minigames = new List<MinigameRec>();
	private readonly List<ScoreRec> m_scores = new List<ScoreRec>();
	private readonly List<CommentRec> m_comments = new List<CommentRec>();
	private readonly Dictionary<string, List<string>> m_likes = new Dictionary<string, List<string>>();

	public string RootPath
	{
		get { return m_root; }
	}

	public OfflineBackend(string rootDir)
	{
		m_root = rootDir;
		m_levelDir = Path.Combine(rootDir, "levels");
		m_thumbDir = Path.Combine(rootDir, "thumbs");
		m_dbPath = Path.Combine(rootDir, "db.json");
		Directory.CreateDirectory(m_root);
		Directory.CreateDirectory(m_levelDir);
		Directory.CreateDirectory(m_thumbDir);
		LoadDb();
	}

	public OfflineResponse Handle(string method, string url, byte[] body, string fileSizes)
	{
		lock (this)
		{
			try
			{
				return Route(url, body ?? new byte[0], fileSizes);
			}
			catch (Exception e)
			{
				return JsonResponse(Obj("status", "ERROR", "error", e.Message));
			}
		}
	}

	private OfflineResponse Route(string url, byte[] body, string fileSizes)
	{
		string path;
		string rawQuery;
		SplitUrl(url, out path, out rawQuery);
		Dictionary<string, string> q = ParseQuery(rawQuery);
		string route = path.ToLowerInvariant().TrimEnd('/');

		switch (route)
		{
		case "/v1/player/login":
			return Login(body);
		case "/v1/player/save":
			return PlayerSave(q, body);
		case "/v1/minigame/save":
			return SaveMinigame(rawQuery, q, body, fileSizes, false);
		case "/v1/minigame/publish":
			return SaveMinigame(rawQuery, q, body, fileSizes, true);
		case "/v1/minigame/data/find":
			return LoadMinigame(rawQuery, q);
		case "/v1/minigame/meta/find":
			return ListMinigames(q);
		case "/v1/minigame/delete":
			return DeleteMinigame(rawQuery, q);
		case "/v1/minigame/screenshot/save":
			return SaveScreenshot(rawQuery, q, body);
		case "/v1/minigame/screenshot/find":
			return LoadScreenshot(rawQuery, q);
		case "/v1/highscore/send":
			return SendHighscore(rawQuery, q);
		case "/v1/highscore/find":
			return JsonResponse(Obj("status", "OK", "data", ScoreList(GetGameId(rawQuery, q))));
		case "/v1/highscore/quit":
			return QuitHighscore(rawQuery, q);
		case "/v1/minigame/like/save":
			return SaveLike(q);
		case "/v1/minigame/like/find":
			return FindLikes(q);
		case "/v1/minigame/comment/save":
			return SaveComment(q);
		case "/v1/minigame/comment/find":
			return FindComments(q);
		}

		if (route.IndexOf("find") >= 0 || route.IndexOf("list") >= 0)
		{
			return JsonResponse(Obj("status", "OK", "data", new List<object>()));
		}
		return JsonResponse(Obj("status", "OK", "id", NewId(), "data", new List<object>()));
	}

	private OfflineResponse Login(byte[] body)
	{
		Dictionary<string, object> req = ParseJsonObject(body);
		string name = StringField(req, "name");
		string id = StringField(req, "id");
		string facebookId = StringField(req, "facebookId");
		string gameCenterId = StringField(req, "gameCenterId");

		if (id.Length == 0)
		{
			id = NewId();
		}

		PlayerRec player;
		if (!m_players.TryGetValue(id, out player))
		{
			player = new PlayerRec();
			player.id = id;
			player.name = "Player";
			m_players[id] = player;
		}
		if (name.Length > 0)
		{
			player.name = name;
		}
		if (facebookId.Length > 0)
		{
			player.facebookId = facebookId;
		}
		if (gameCenterId.Length > 0)
		{
			player.gameCenterId = gameCenterId;
		}
		SaveDb();

		return JsonResponse(Obj(
			"status", "OK",
			"id", player.id,
			"name", player.name,
			"acceptNotifications", player.acceptNotifications,
			"facebookId", player.facebookId,
			"gameCenterId", player.gameCenterId));
	}

	private OfflineResponse PlayerSave(Dictionary<string, string> q, byte[] body)
	{
		string id = Get(q, "id");
		Dictionary<string, object> req = ParseJsonObject(body);
		string name = StringField(req, "name");
		if (id.Length > 0 && name.Length > 0)
		{
			PlayerRec player;
			if (!m_players.TryGetValue(id, out player))
			{
				player = new PlayerRec();
				player.id = id;
				m_players[id] = player;
			}
			player.name = name;
			SaveDb();
		}
		return JsonResponse(Obj("status", "OK", "id", id.Length > 0 ? id : NewId(), "data", new List<object>()));
	}

	private OfflineResponse SaveMinigame(string rawQuery, Dictionary<string, string> q, byte[] body, string fileSizes, bool publish)
	{
		string id = GetGameId(rawQuery, q);
		if (id.Length == 0)
		{
			id = NewId();
		}
		else if (!IsSafeId(id))
		{
			return JsonResponse(Obj("status", "ERROR", "error", "invalid id"));
		}

		byte[] jsonPart;
		byte[] dataPart;
		SplitBody(body, fileSizes, out jsonPart, out dataPart);
		Dictionary<string, object> meta = ParseJsonObject(jsonPart);

		MinigameRec rec = FindMinigame(id);
		if (rec == null)
		{
			rec = new MinigameRec();
			rec.id = id;
			m_minigames.Add(rec);
		}

		string name = StringField(meta, "name");
		rec.name = name.Length > 0 ? name : "Untitled";
		rec.description = StringField(meta, "description");
		string creatorId = StringField(meta, "creatorId");
		rec.creatorId = creatorId.Length > 0 ? creatorId : "offline_player";
		string creatorName = StringField(meta, "creatorName");
		rec.creatorName = creatorName.Length > 0 ? creatorName : "Player";
		rec.tags = TagsField(meta);
		rec.published = publish;
		rec.publishTime = publish ? NowMs() : 0;

		File.WriteAllBytes(Path.Combine(m_levelDir, id + ".bin"), dataPart);
		SaveDb();

		return JsonResponse(Obj("status", "OK", "id", id));
	}

	private OfflineResponse LoadMinigame(string rawQuery, Dictionary<string, string> q)
	{
		string id = GetGameId(rawQuery, q);
		if (IsSafeId(id))
		{
			string file = Path.Combine(m_levelDir, id + ".bin");
			if (File.Exists(file))
			{
				return new OfflineResponse(File.ReadAllBytes(file));
			}
		}
		OfflineResponse notFound = JsonResponse(Obj("status", "ERROR", "data", new List<object>()));
		notFound.Error = "404 Not Found";
		return notFound;
	}

	private OfflineResponse ListMinigames(Dictionary<string, string> q)
	{
		string published = Get(q, "published");
		string creator = Get(q, "creatorId");
		if (creator.Length == 0)
		{
			creator = Get(q, "playerId");
		}

		List<object> entries = new List<object>();
		foreach (MinigameRec r in m_minigames)
		{
			if (published.Length > 0 && r.published != (published.ToLowerInvariant() == "true"))
			{
				continue;
			}
			if (creator.Length > 0 && r.creatorId != creator)
			{
				continue;
			}

			List<object> tags = new List<object>();
			foreach (string tag in r.tags)
			{
				tags.Add(tag);
			}

			Dictionary<string, object> item = Obj(
				"id", r.id,
				"name", r.name,
				"description", r.description,
				"creatorId", r.creatorId,
				"creatorName", r.creatorName,
				"tags", tags,
				"timesPlayed", r.timesPlayed,
				"timesFinished", r.timesFinished,
				"timesLiked", r.timesLiked,
				"new", r.isNew,
				"classic", r.isClassic,
				"published", r.published);
			if (r.published && r.publishTime != 0)
			{
				item["publishTime"] = Obj("$date", r.publishTime);
			}
			entries.Add(item);
		}
		return JsonResponse(Obj("status", "OK", "data", entries));
	}

	private OfflineResponse DeleteMinigame(string rawQuery, Dictionary<string, string> q)
	{
		string id = GetGameId(rawQuery, q);
		if (IsSafeId(id))
		{
			m_minigames.RemoveAll(delegate(MinigameRec r) { return r.id == id; });
			m_comments.RemoveAll(delegate(CommentRec c) { return c.gameId == id; });
			m_scores.RemoveAll(delegate(ScoreRec s) { return s.gameId == id; });
			foreach (List<string> liked in m_likes.Values)
			{
				liked.Remove(id);
			}
			TryDelete(Path.Combine(m_levelDir, id + ".bin"));
			TryDelete(Path.Combine(m_thumbDir, id + ".png"));
			SaveDb();
		}
		return JsonResponse(Obj("status", "OK", "id", id));
	}

	private OfflineResponse SaveScreenshot(string rawQuery, Dictionary<string, string> q, byte[] body)
	{
		string id = GetGameId(rawQuery, q);
		if (IsSafeId(id))
		{
			File.WriteAllBytes(Path.Combine(m_thumbDir, id + ".png"), body);
		}
		return JsonResponse(Obj("status", "OK"));
	}

	private OfflineResponse LoadScreenshot(string rawQuery, Dictionary<string, string> q)
	{
		string id = GetGameId(rawQuery, q);
		byte[] data = null;
		if (IsSafeId(id))
		{
			string file = Path.Combine(m_thumbDir, id + ".png");
			if (File.Exists(file))
			{
				data = File.ReadAllBytes(file);
			}
		}
		OfflineResponse response;
		if (data == null)
		{
			// Thumbnails are only uploaded after the creator finishes the level, so a freshly published level has
			// none yet. A 404 makes the banner keep its placeholder instead of drawing a fake image.
			response = JsonResponse(Obj("status", "ERROR", "data", new List<object>()));
			response.Error = "404 Not Found";
		}
		else
		{
			response = new OfflineResponse(data);
		}
		response.DelaySeconds = 0.15f;
		return response;
	}

	private OfflineResponse SendHighscore(string rawQuery, Dictionary<string, string> q)
	{
		string gameId = GetGameId(rawQuery, q);
		string playerId = Get(q, "playerId");
		if (playerId.Length == 0)
		{
			playerId = "offline";
		}
		string playerName = Get(q, "name");
		if (playerName.Length == 0)
		{
			playerName = "Player";
		}
		long score = ParseLong(Get(q, "time"), 0);
		long starts = ParseLong(Get(q, "starts"), 1);
		bool returnScores = Get(q, "returnScores").ToLowerInvariant() == "true";

		if (gameId.Length > 0)
		{
			ScoreRec rec = null;
			foreach (ScoreRec s in m_scores)
			{
				if (s.gameId == gameId && s.playerId == playerId)
				{
					rec = s;
					break;
				}
			}
			if (rec == null)
			{
				rec = new ScoreRec();
				rec.gameId = gameId;
				rec.playerId = playerId;
				rec.time = score;
				rec.starts = starts;
				m_scores.Add(rec);
			}
			else
			{
				if (score < rec.time)
				{
					rec.time = score;
				}
				rec.starts += starts;
			}
			rec.name = playerName;
			rec.createdAt = NowMs() / 1000;

			MinigameRec game = FindMinigame(gameId);
			if (game != null)
			{
				game.timesFinished += 1;
				game.timesPlayed += starts;
			}
			SaveDb();
		}

		if (returnScores)
		{
			return JsonResponse(Obj("status", "OK", "data", ScoreList(gameId)));
		}
		return JsonResponse(Obj("status", "OK", "data", new List<object>()));
	}

	private OfflineResponse QuitHighscore(string rawQuery, Dictionary<string, string> q)
	{
		string gameId = GetGameId(rawQuery, q);
		long starts = ParseLong(Get(q, "starts"), 1);
		MinigameRec game = FindMinigame(gameId);
		if (game != null)
		{
			game.timesPlayed += starts;
			SaveDb();
		}
		return JsonResponse(Obj("status", "OK"));
	}

	private OfflineResponse SaveLike(Dictionary<string, string> q)
	{
		string gameId = Get(q, "gameId");
		string playerId = Get(q, "playerId");
		if (playerId.Length == 0)
		{
			playerId = "default_player";
		}
		if (gameId.Length > 0)
		{
			List<string> liked;
			if (!m_likes.TryGetValue(playerId, out liked))
			{
				liked = new List<string>();
				m_likes[playerId] = liked;
			}
			if (!liked.Contains(gameId))
			{
				liked.Add(gameId);
			}

			MinigameRec game = FindMinigame(gameId);
			if (game != null)
			{
				long count = 0;
				foreach (List<string> list in m_likes.Values)
				{
					if (list.Contains(gameId))
					{
						count++;
					}
				}
				game.timesLiked = count;
			}
			SaveDb();
		}
		return JsonResponse(Obj("status", "OK"));
	}

	private OfflineResponse FindLikes(Dictionary<string, string> q)
	{
		string playerId = Get(q, "playerId");
		if (playerId.Length == 0)
		{
			playerId = "default_player";
		}
		List<object> ids = new List<object>();
		List<string> liked;
		if (m_likes.TryGetValue(playerId, out liked))
		{
			foreach (string id in liked)
			{
				ids.Add(id);
			}
		}
		return JsonResponse(Obj("status", "OK", "data", ids));
	}

	private OfflineResponse SaveComment(Dictionary<string, string> q)
	{
		string gameId = Get(q, "gameId");
		string text = Get(q, "comment");
		if (gameId.Length > 0 && text.Length > 0)
		{
			CommentRec c = new CommentRec();
			c.id = NewId();
			c.gameId = gameId;
			c.playerId = Get(q, "playerId");
			c.comment = text;
			c.facebookId = Get(q, "facebookId");
			c.gameCenterId = Get(q, "gameCenterId");
			c.createdAt = NowMs() / 1000;
			m_comments.Add(c);
			SaveDb();
		}
		return JsonResponse(Obj("status", "OK"));
	}

	private OfflineResponse FindComments(Dictionary<string, string> q)
	{
		string gameId = Get(q, "gameId");
		long limit = ParseLong(Get(q, "limit"), 0);

		List<object> list = new List<object>();
		if (gameId.Length > 0)
		{
			for (int i = m_comments.Count - 1; i >= 0; i--)
			{
				CommentRec c = m_comments[i];
				if (c.gameId != gameId)
				{
					continue;
				}
				if (limit > 0 && list.Count >= limit)
				{
					break;
				}
				list.Add(Obj(
					"id", c.id,
					"gameId", c.gameId,
					"playerId", c.playerId,
					"comment", c.comment,
					"facebookId", c.facebookId,
					"gameCenterId", c.gameCenterId,
					"date", c.createdAt));
			}
		}
		return JsonResponse(Obj("status", "OK", "data", list));
	}

	private List<object> ScoreList(string gameId)
	{
		List<ScoreRec> rows = new List<ScoreRec>();
		if (gameId.Length > 0)
		{
			foreach (ScoreRec s in m_scores)
			{
				if (s.gameId == gameId)
				{
					rows.Add(s);
				}
			}
		}
		List<ScoreRec> sorted = StableSortByTime(rows);
		List<object> result = new List<object>();
		foreach (ScoreRec s in sorted)
		{
			result.Add(Obj("n", s.name, "t", s.time));
		}
		return result;
	}

	private static List<ScoreRec> StableSortByTime(List<ScoreRec> rows)
	{
		List<ScoreRec> sorted = new List<ScoreRec>();
		foreach (ScoreRec row in rows)
		{
			int index = sorted.Count;
			while (index > 0 && sorted[index - 1].time > row.time)
			{
				index--;
			}
			sorted.Insert(index, row);
		}
		return sorted;
	}

	private MinigameRec FindMinigame(string id)
	{
		foreach (MinigameRec r in m_minigames)
		{
			if (r.id == id)
			{
				return r;
			}
		}
		return null;
	}

	private void LoadDb()
	{
		if (!File.Exists(m_dbPath))
		{
			return;
		}

		try
		{
			Dictionary<string, object> db = Json.Deserialize(File.ReadAllText(m_dbPath, Encoding.UTF8)) as Dictionary<string, object>;
			if (db == null)
			{
				throw new FormatException("db.json root is not an object");
			}

			foreach (Dictionary<string, object> d in ObjectList(db, "players"))
			{
				PlayerRec p = new PlayerRec();
				p.id = StringField(d, "id");
				p.name = StringField(d, "name");
				p.facebookId = StringField(d, "facebookId");
				p.gameCenterId = StringField(d, "gameCenterId");
				p.acceptNotifications = BoolField(d, "acceptNotifications", false);
				if (p.id.Length > 0)
				{
					m_players[p.id] = p;
				}
			}
			foreach (Dictionary<string, object> d in ObjectList(db, "minigames"))
			{
				MinigameRec r = new MinigameRec();
				r.id = StringField(d, "id");
				r.name = StringField(d, "name");
				r.description = StringField(d, "description");
				r.creatorId = StringField(d, "creatorId");
				r.creatorName = StringField(d, "creatorName");
				r.tags = TagsField(d);
				r.published = BoolField(d, "published", false);
				r.publishTime = LongField(d, "publishTime");
				r.timesPlayed = LongField(d, "timesPlayed");
				r.timesFinished = LongField(d, "timesFinished");
				r.timesLiked = LongField(d, "timesLiked");
				r.isNew = BoolField(d, "isNew", true);
				r.isClassic = BoolField(d, "isClassic", false);
				if (r.id.Length > 0)
				{
					m_minigames.Add(r);
				}
			}
			foreach (Dictionary<string, object> d in ObjectList(db, "highscores"))
			{
				ScoreRec s = new ScoreRec();
				s.gameId = StringField(d, "gameId");
				s.playerId = StringField(d, "playerId");
				s.name = StringField(d, "name");
				s.time = LongField(d, "time");
				s.starts = LongField(d, "starts");
				s.createdAt = LongField(d, "createdAt");
				m_scores.Add(s);
			}
			foreach (Dictionary<string, object> d in ObjectList(db, "comments"))
			{
				CommentRec c = new CommentRec();
				c.id = StringField(d, "id");
				c.gameId = StringField(d, "gameId");
				c.playerId = StringField(d, "playerId");
				c.comment = StringField(d, "comment");
				c.facebookId = StringField(d, "facebookId");
				c.gameCenterId = StringField(d, "gameCenterId");
				c.createdAt = LongField(d, "createdAt");
				m_comments.Add(c);
			}
			Dictionary<string, object> likes = db.ContainsKey("likes") ? db["likes"] as Dictionary<string, object> : null;
			if (likes != null)
			{
				foreach (KeyValuePair<string, object> kv in likes)
				{
					List<object> ids = kv.Value as List<object>;
					if (ids == null)
					{
						continue;
					}
					List<string> liked = new List<string>();
					foreach (object o in ids)
					{
						string s = o as string;
						if (s != null)
						{
							liked.Add(s);
						}
					}
					m_likes[kv.Key] = liked;
				}
			}
		}
		catch (Exception)
		{
			m_players.Clear();
			m_minigames.Clear();
			m_scores.Clear();
			m_comments.Clear();
			m_likes.Clear();
			string backup = Path.Combine(m_root, "db.corrupt-" + NowMs() + ".json");
			File.Copy(m_dbPath, backup, true);
		}
	}

	private void SaveDb()
	{
		List<object> players = new List<object>();
		foreach (PlayerRec p in m_players.Values)
		{
			players.Add(Obj(
				"id", p.id,
				"name", p.name,
				"facebookId", p.facebookId,
				"gameCenterId", p.gameCenterId,
				"acceptNotifications", p.acceptNotifications));
		}

		List<object> minigames = new List<object>();
		foreach (MinigameRec r in m_minigames)
		{
			List<object> tags = new List<object>();
			foreach (string tag in r.tags)
			{
				tags.Add(tag);
			}
			minigames.Add(Obj(
				"id", r.id,
				"name", r.name,
				"description", r.description,
				"creatorId", r.creatorId,
				"creatorName", r.creatorName,
				"tags", tags,
				"published", r.published,
				"publishTime", r.publishTime,
				"timesPlayed", r.timesPlayed,
				"timesFinished", r.timesFinished,
				"timesLiked", r.timesLiked,
				"isNew", r.isNew,
				"isClassic", r.isClassic));
		}

		List<object> scores = new List<object>();
		foreach (ScoreRec s in m_scores)
		{
			scores.Add(Obj(
				"gameId", s.gameId,
				"playerId", s.playerId,
				"name", s.name,
				"time", s.time,
				"starts", s.starts,
				"createdAt", s.createdAt));
		}

		List<object> comments = new List<object>();
		foreach (CommentRec c in m_comments)
		{
			comments.Add(Obj(
				"id", c.id,
				"gameId", c.gameId,
				"playerId", c.playerId,
				"comment", c.comment,
				"facebookId", c.facebookId,
				"gameCenterId", c.gameCenterId,
				"createdAt", c.createdAt));
		}

		Dictionary<string, object> likes = new Dictionary<string, object>();
		foreach (KeyValuePair<string, List<string>> kv in m_likes)
		{
			List<object> ids = new List<object>();
			foreach (string id in kv.Value)
			{
				ids.Add(id);
			}
			likes[kv.Key] = ids;
		}

		string json = Json.Serialize(Obj(
			"version", 1,
			"players", players,
			"minigames", minigames,
			"highscores", scores,
			"comments", comments,
			"likes", likes));

		string tmp = m_dbPath + ".tmp";
		File.WriteAllText(tmp, json, new UTF8Encoding(false));
		if (File.Exists(m_dbPath))
		{
			File.Delete(m_dbPath);
		}
		File.Move(tmp, m_dbPath);
	}

	private static void SplitBody(byte[] body, string fileSizes, out byte[] jsonPart, out byte[] dataPart)
	{
		if (!string.IsNullOrEmpty(fileSizes))
		{
			string[] parts = fileSizes.Split(',');
			int a;
			int b;
			if (parts.Length == 2 && int.TryParse(parts[0].Trim(), out a) && int.TryParse(parts[1].Trim(), out b)
				&& a >= 0 && b >= 0 && (long)a + b == body.Length)
			{
				jsonPart = new byte[a];
				dataPart = new byte[b];
				Buffer.BlockCopy(body, 0, jsonPart, 0, a);
				Buffer.BlockCopy(body, a, dataPart, 0, b);
				return;
			}
		}

		int zip = IndexOfZipMagic(body);
		if (zip >= 4)
		{
			jsonPart = new byte[zip - 4];
			dataPart = new byte[body.Length - (zip - 4)];
			Buffer.BlockCopy(body, 0, jsonPart, 0, jsonPart.Length);
			Buffer.BlockCopy(body, zip - 4, dataPart, 0, dataPart.Length);
			return;
		}
		jsonPart = body;
		dataPart = body;
	}

	private static int IndexOfZipMagic(byte[] data)
	{
		for (int i = 0; i + 3 < data.Length; i++)
		{
			if (data[i] == 0x50 && data[i + 1] == 0x4B && data[i + 2] == 0x03 && data[i + 3] == 0x04)
			{
				return i;
			}
		}
		return -1;
	}

	private static Dictionary<string, object> ParseJsonObject(byte[] bytes)
	{
		if (bytes == null || bytes.Length == 0)
		{
			return null;
		}
		int start = 0;
		while (start < bytes.Length && (bytes[start] == 0x20 || bytes[start] == 0x09 || bytes[start] == 0x0A || bytes[start] == 0x0D))
		{
			start++;
		}
		if (start >= bytes.Length || bytes[start] != (byte)'{')
		{
			return null;
		}

		int depth = 0;
		bool inString = false;
		bool escaped = false;
		int end = -1;
		for (int i = start; i < bytes.Length; i++)
		{
			byte c = bytes[i];
			if (inString)
			{
				if (escaped)
				{
					escaped = false;
				}
				else if (c == (byte)'\\')
				{
					escaped = true;
				}
				else if (c == (byte)'"')
				{
					inString = false;
				}
				continue;
			}
			if (c == (byte)'"')
			{
				inString = true;
			}
			else if (c == (byte)'{')
			{
				depth++;
			}
			else if (c == (byte)'}')
			{
				depth--;
				if (depth == 0)
				{
					end = i;
					break;
				}
			}
		}
		if (end < 0)
		{
			return null;
		}

		try
		{
			return Json.Deserialize(Encoding.UTF8.GetString(bytes, start, end - start + 1)) as Dictionary<string, object>;
		}
		catch (Exception)
		{
			return null;
		}
	}

	private static void SplitUrl(string url, out string path, out string rawQuery)
	{
		string rest = url;
		int scheme = rest.IndexOf("://");
		if (scheme >= 0)
		{
			int slash = rest.IndexOf('/', scheme + 3);
			rest = slash >= 0 ? rest.Substring(slash) : "/";
		}
		int question = rest.IndexOf('?');
		if (question >= 0)
		{
			path = rest.Substring(0, question);
			rawQuery = rest.Substring(question + 1);
		}
		else
		{
			path = rest;
			rawQuery = string.Empty;
		}
	}

	private static Dictionary<string, string> ParseQuery(string rawQuery)
	{
		Dictionary<string, string> result = new Dictionary<string, string>();
		if (string.IsNullOrEmpty(rawQuery))
		{
			return result;
		}
		foreach (string part in rawQuery.Split('&'))
		{
			int eq = part.IndexOf('=');
			if (eq <= 0)
			{
				continue;
			}
			string key = UrlDecode(part.Substring(0, eq));
			string value = UrlDecode(part.Substring(eq + 1));
			if (value.Length == 0 || result.ContainsKey(key))
			{
				continue;
			}
			result[key] = value;
		}
		return result;
	}

	// SendQuitData in the game builds "?gameId<id>" (no '='), so fall back to a regex on the raw query.
	private static string GetGameId(string rawQuery, Dictionary<string, string> q)
	{
		string id = Get(q, "gameId");
		if (id.Length == 0)
		{
			id = Get(q, "id");
		}
		if (id.Length == 0)
		{
			Match m = Regex.Match(rawQuery, "gameid=?([A-Za-z0-9]+)", RegexOptions.IgnoreCase);
			if (m.Success)
			{
				id = m.Groups[1].Value;
			}
		}
		return id;
	}

	private static string UrlDecode(string s)
	{
		if (s.IndexOf('%') < 0 && s.IndexOf('+') < 0)
		{
			return s;
		}
		List<byte> bytes = new List<byte>(s.Length);
		for (int i = 0; i < s.Length; i++)
		{
			char c = s[i];
			if (c == '+')
			{
				bytes.Add((byte)' ');
			}
			else if (c == '%' && i + 2 < s.Length && IsHex(s[i + 1]) && IsHex(s[i + 2]))
			{
				bytes.Add((byte)Convert.ToInt32(s.Substring(i + 1, 2), 16));
				i += 2;
			}
			else if (c < 128)
			{
				bytes.Add((byte)c);
			}
			else
			{
				bytes.AddRange(Encoding.UTF8.GetBytes(c.ToString()));
			}
		}
		return Encoding.UTF8.GetString(bytes.ToArray());
	}

	private static bool IsHex(char c)
	{
		return (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
	}

	private static bool IsSafeId(string id)
	{
		if (string.IsNullOrEmpty(id) || id.Length > 64)
		{
			return false;
		}
		foreach (char c in id)
		{
			bool ok = (c >= '0' && c <= '9') || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_' || c == '-';
			if (!ok)
			{
				return false;
			}
		}
		return true;
	}

	private static string Get(Dictionary<string, string> q, string key)
	{
		string value;
		return q.TryGetValue(key, out value) ? value : string.Empty;
	}

	private static long ParseLong(string s, long fallback)
	{
		long value;
		return long.TryParse(s, out value) ? value : fallback;
	}

	private static string StringField(Dictionary<string, object> d, string key)
	{
		object value;
		if (d != null && d.TryGetValue(key, out value))
		{
			string s = value as string;
			if (s != null)
			{
				return s;
			}
		}
		return string.Empty;
	}

	private static long LongField(Dictionary<string, object> d, string key)
	{
		object value;
		if (d != null && d.TryGetValue(key, out value) && value is long)
		{
			return (long)value;
		}
		return 0;
	}

	private static bool BoolField(Dictionary<string, object> d, string key, bool fallback)
	{
		object value;
		if (d != null && d.TryGetValue(key, out value) && value is bool)
		{
			return (bool)value;
		}
		return fallback;
	}

	private static List<string> TagsField(Dictionary<string, object> d)
	{
		List<string> tags = new List<string>();
		object value;
		if (d != null && d.TryGetValue("tags", out value))
		{
			List<object> list = value as List<object>;
			if (list != null)
			{
				foreach (object o in list)
				{
					string s = o as string;
					if (s != null)
					{
						tags.Add(s);
					}
				}
				return tags;
			}
		}
		tags.AddRange(DEFAULT_TAGS);
		return tags;
	}

	private static List<Dictionary<string, object>> ObjectList(Dictionary<string, object> d, string key)
	{
		List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
		object value;
		if (d.TryGetValue(key, out value))
		{
			List<object> list = value as List<object>;
			if (list != null)
			{
				foreach (object o in list)
				{
					Dictionary<string, object> item = o as Dictionary<string, object>;
					if (item != null)
					{
						result.Add(item);
					}
				}
			}
		}
		return result;
	}

	private static Dictionary<string, object> Obj(params object[] keyValues)
	{
		Dictionary<string, object> d = new Dictionary<string, object>();
		for (int i = 0; i + 1 < keyValues.Length; i += 2)
		{
			d[(string)keyValues[i]] = keyValues[i + 1];
		}
		return d;
	}

	private static OfflineResponse JsonResponse(Dictionary<string, object> d)
	{
		return new OfflineResponse(Encoding.UTF8.GetBytes(MiniJSON.Json.Serialize(d)));
	}

	private static string NewId()
	{
		return Guid.NewGuid().ToString("N").Substring(0, 24);
	}

	private static long NowMs()
	{
		return (long)(DateTime.UtcNow - EPOCH).TotalMilliseconds;
	}

	private static void TryDelete(string path)
	{
		try
		{
			if (File.Exists(path))
			{
				File.Delete(path);
			}
		}
		catch (IOException)
		{
		}
	}
}
