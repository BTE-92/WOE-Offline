using System.Collections.Generic;
using UnityEngine;

public class FBScreen
{
	public class Layout
	{
		public class OptionLeft : Layout
		{
			public float Amount;
		}

		public class OptionTop : Layout
		{
			public float Amount;
		}

		public class OptionCenterHorizontal : Layout
		{
		}

		public class OptionCenterVertical : Layout
		{
		}
	}

	private static bool resizable;

	public static bool FullScreen
	{
		get
		{
			return Screen.fullScreen;
		}
		set
		{
			Screen.fullScreen = value;
		}
	}

	public static bool Resizable
	{
		get
		{
			return resizable;
		}
	}

	public static int Width
	{
		get
		{
			return Screen.width;
		}
	}

	public static int Height
	{
		get
		{
			return Screen.height;
		}
	}

	public static void SetResolution(int width, int height, bool fullscreen, int preferredRefreshRate = 0, params Layout[] layoutParams)
	{
		Screen.SetResolution(width, height, fullscreen, preferredRefreshRate);
	}

	public static void SetAspectRatio(int width, int height, params Layout[] layoutParams)
	{
		int width2 = Screen.height / height * width;
		Screen.SetResolution(width2, Screen.height, Screen.fullScreen);
	}

	public static void SetUnityPlayerEmbedCSS(string key, string value)
	{
		Application.ExternalEval(string.Format("$(\"#unityPlayerEmbed\").css(\"{0}\",\"{1}\")", key, value));
	}

	public static Layout.OptionLeft Left(float amount)
	{
		Layout.OptionLeft optionLeft = new Layout.OptionLeft();
		optionLeft.Amount = amount;
		return optionLeft;
	}

	public static Layout.OptionTop Top(float amount)
	{
		Layout.OptionTop optionTop = new Layout.OptionTop();
		optionTop.Amount = amount;
		return optionTop;
	}

	public static Layout.OptionCenterHorizontal CenterHorizontal()
	{
		return new Layout.OptionCenterHorizontal();
	}

	public static Layout.OptionCenterVertical CenterVertical()
	{
		return new Layout.OptionCenterVertical();
	}

	private static void SetLayout(IEnumerable<Layout> parameters)
	{
		foreach (Layout parameter in parameters)
		{
			Layout.OptionLeft optionLeft = parameter as Layout.OptionLeft;
			if (optionLeft != null)
			{
				SetUnityPlayerEmbedCSS("margin-left", optionLeft.Amount + "px");
				SetUnityPlayerEmbedCSS("padding-left", "0px");
				Application.ExternalEval("\n                    if (typeof fbCenterWebPlayerHorizontally == \"function\") \n                    {\n                        $(window).off(\"resize\", fbCenterWebPlayerHorizontally);\n                    }\n                ");
				continue;
			}
			Layout.OptionTop optionTop = parameter as Layout.OptionTop;
			if (optionTop != null)
			{
				SetUnityPlayerEmbedCSS("margin-top", optionTop.Amount + "px");
				SetUnityPlayerEmbedCSS("padding-top", "0px");
				Application.ExternalEval("\n                    if (typeof fbCenterWebPlayerVertically == \"function\") \n                    {\n                        $(window).off(\"resize\", fbCenterWebPlayerVertically);\n                    }\n                ");
				continue;
			}
			Layout.OptionCenterHorizontal optionCenterHorizontal = parameter as Layout.OptionCenterHorizontal;
			if (optionCenterHorizontal != null)
			{
				Application.ExternalEval("\n                    function fbCenterWebPlayerHorizontally(){\n                        $(\"#unityPlayerEmbed\").css(\n                            \"margin-left\", \n                            ($(window).innerWidth()/2 - $(\"#unityPlayerEmbed\").children(\"object, embed\").width()/2) + \"px\")\n                    }; \n                    fbCenterWebPlayerHorizontally(); \n                    $(window).resize(fbCenterWebPlayerHorizontally)\n                ");
				continue;
			}
			Layout.OptionCenterVertical optionCenterVertical = parameter as Layout.OptionCenterVertical;
			if (optionCenterVertical != null)
			{
				Application.ExternalEval("\n                    function fbCenterWebPlayerVertically(){\n                        $(\"#unityPlayerEmbed\").css(\n                            \"margin-top\", \n                            ($(window).innerHeight()/2 - $(\"#unityPlayerEmbed\").children(\"object, embed\").height()/2) + \"px\")\n                    }; \n                    fbCenterWebPlayerVertically(); \n                    $(window).resize(fbCenterWebPlayerVertically)\n                ");
			}
			else
			{
				FbDebug.Error("Unknown Layout type: " + parameter.GetType());
			}
		}
	}
}
