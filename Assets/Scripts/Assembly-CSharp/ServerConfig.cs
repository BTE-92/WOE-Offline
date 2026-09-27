public static class ServerConfig
{
	public const string HPW = "bfid3Z53SFib325PJGFasae";

	public const string SERVER_BASE = "http://PlayDevLB-1049210432.us-west-2.elb.amazonaws.com/"; //See IpConfig.cs

	public const string PLAYER_CREATE = "v1/player/login";

	public const string PLAYER_UPDATE = "v1/player/save";

	public const string PLAYER_MERGE = "v1/player/merge";

	public const string FRIEND_SEND = "v1/player/friend/save";

	public const string FRIEND_FIND = "v1/player/friend/find";

	public const string MINIGAME_META_FIND = "v1/minigame/meta/find";

	public const string MINIGAME_DATA_FIND = "v1/minigame/data/find";

	public const string MINIGAME_SAVE = "v1/minigame/save";

	public const string MINIGAME_PUBLISH = "v1/minigame/publish";

	public const string MINIGAME_LIKE = "v1/minigame/like/save";

	public const string MINIGAME_FIND_LIKES = "v1/minigame/like/find";

	public const string MINIGAME_DELETE = "v1/minigame/delete";

	public const string SCREENSHOT_SAVE = "v1/minigame/screenshot/save";

	public const string SCREENSHOT_FIND = "v1/minigame/screenshot/find";

	public const string HIGHSCORE_SEND = "v1/highscore/send";

	public const string HIGHSCORE_FIND = "v1/highscore/find";

	public const string HIGHSCORE_QUIT = "v1/highscore/quit";

	public const string COMMENT_SAVE = "v1/minigame/comment/save";

	public const string COMMENT_FIND = "v1/minigame/comment/find";

	public const string PUSH_TOKEN_SEND = "v1/push/token/save";

	public const string NOTIFICATIONS_FIND = "v1/notification/find";

	public const string NOTIFICATIONS_COUNT = "v1/notification/count";

	public const string GAME_FEED_FIND = "v1/feed/find";

	public static string GetHPW()
	{
		return "bfid3Z53SFib325PJGFasae";
	}
}
