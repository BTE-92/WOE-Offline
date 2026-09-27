using System.Collections.Generic;
using UnityEngine;

public class UIMinigameBanner : UIComponent
{
	public static float m_defaultWidth = 1f;

	public static float m_defaultHeight = 0.36f;

	public static cpBB m_defaultMargins = new cpBB(0f);

	public static RelativeTo m_defaultWidthRelativeTo = RelativeTo.ParentWidth;

	public static RelativeTo m_defaultHeightRelativeTo = RelativeTo.OwnWidth;

	public static RelativeTo m_defaultMarginsRelativeTo = RelativeTo.ScreenShortest;

	protected MinigameMetaData m_minigameMetaData;

	public UICanvas m_headerCanvas;

	public UICanvas m_contentCanvas;

	public UICanvas m_footerCanvas;

	public Texture2D m_screenshot;

	public List<PrefabC> m_screenshotPrefabs;

	public Polygon m_screenshotPolygon;

	private float m_startDestroyPress;

	private Vector2[] m_bannerShape;

	public UIMinigameBanner(UIVerticalList _parent, string _tag, MinigameMetaData _minigameMetaData)
		: base(_parent, true, _tag, null, null, null)
	{
		m_minigameMetaData = _minigameMetaData;
		SetWidth(m_defaultWidth, m_defaultWidthRelativeTo);
		SetHeight(m_defaultHeight, m_defaultHeightRelativeTo);
		SetMargins(m_defaultMargins, m_defaultMarginsRelativeTo);
		UIProfileImage uIProfileImage = new UIProfileImage(this, true, _tag + "profile", _minigameMetaData.creatorFacebookId, _minigameMetaData.creatorGameCenterId);
		uIProfileImage.SetSize(0.22f, 0.22f, RelativeTo.ParentHeight);
		uIProfileImage.SetAlign(0f, 1f);
		uIProfileImage.SetMargins(0.025f, 0f, 0.025f, 0f, RelativeTo.ParentHeight);
		uIProfileImage.RemoveDrawHandler();
		uIProfileImage.SetTouchHandler(DestroyTouchHandler);
		Server.GetScreenshot(_minigameMetaData.id, ScreenshotOK, ScreenshotFAIL);
		m_headerCanvas = new UICanvas(this, _tag, null, string.Empty);
		m_headerCanvas.SetWidth(1f, RelativeTo.ParentWidth);
		m_headerCanvas.SetHeight(0.15f, RelativeTo.ParentHeight);
		m_headerCanvas.SetVerticalAlign(1f);
		m_headerCanvas.SetMargins(0.125f, 0f, 0f, 0f, RelativeTo.OwnWidth);
		m_headerCanvas.RemoveTouchAreas();
		m_headerCanvas.RemoveDrawHandler();
		UIText uIText = new UIText(m_headerCanvas, false, _tag, m_minigameMetaData.creatorName + " published a game", "Fonts/HurmeSemiBold", 0.5f, RelativeTo.ParentHeight);
		uIText.SetHorizontalAlign(0f);
		m_contentCanvas = new UICanvas(this, _tag, null, string.Empty);
		m_contentCanvas.SetWidth(1f, RelativeTo.ParentWidth);
		m_contentCanvas.SetHeight(0.7f, RelativeTo.ParentHeight);
		m_contentCanvas.SetVerticalAlign(0.5f);
		m_contentCanvas.SetMargins(0.125f, 0.35f, 0f, 0f, RelativeTo.OwnWidth);
		m_contentCanvas.RemoveTouchAreas();
		m_contentCanvas.RemoveDrawHandler();
		UIVerticalList uIVerticalList = new UIVerticalList(m_contentCanvas, _tag);
		uIVerticalList.SetVerticalAlign(1f);
		uIVerticalList.SetHorizontalAlign(0f);
		uIVerticalList.RemoveTouchAreas();
		uIVerticalList.RemoveDrawHandler();
		UIText uIText2 = new UIText(uIVerticalList, false, _tag, "<color=#afed32>" + m_minigameMetaData.name.ToLower() + "</color>", "Fonts/KGLetHerGo", 0.1f, RelativeTo.ParentWidth);
		uIText2.SetMargins(0f, 0f, 0.02f, -0.02f, RelativeTo.ParentWidth);
		uIText2.SetHorizontalAlign(0f);
		UIText uIText3 = new UIText(uIVerticalList, false, _tag, "<color=#8ea5ce>By " + m_minigameMetaData.creatorName + "</color>", "Fonts/HurmeSemiBold", 0.05f, RelativeTo.ParentWidth);
		uIText3.SetHorizontalAlign(0f);
		UITextbox uITextbox = new UITextbox(uIVerticalList, false, _tag, m_minigameMetaData.description, "Fonts/HurmeRegular", 0.04f, RelativeTo.ParentWidth);
		uITextbox.SetMargins(0f, 0f, 0.03f, 0.03f, RelativeTo.ParentWidth);
		uITextbox.SetMaxRows(3);
		m_footerCanvas = new UICanvas(this, _tag, null, string.Empty);
		m_footerCanvas.SetWidth(1f, RelativeTo.ParentWidth);
		m_footerCanvas.SetHeight(0.15f, RelativeTo.ParentHeight);
		m_footerCanvas.SetVerticalAlign(0f);
		m_footerCanvas.SetMargins(0.125f, 0f, 0f, 0f, RelativeTo.OwnWidth);
		m_footerCanvas.RemoveTouchAreas();
		m_footerCanvas.RemoveDrawHandler();
		UIHorizontalList uIHorizontalList = new UIHorizontalList(m_footerCanvas, _tag);
		uIHorizontalList.SetHorizontalAlign(0f);
		uIHorizontalList.RemoveTouchAreas();
		uIHorizontalList.RemoveDrawHandler();
		UIHorizontalList uIHorizontalList2 = new UIHorizontalList(uIHorizontalList, _tag);
		uIHorizontalList2.SetSpacing(0.1f, RelativeTo.ParentHeight);
		uIHorizontalList2.SetMargins(0f, 0.3f, 0f, 0f, RelativeTo.ParentHeight);
		uIHorizontalList2.RemoveTouchAreas();
		uIHorizontalList2.RemoveDrawHandler();
		UIFittedSprite uIFittedSprite = new UIFittedSprite(uIHorizontalList2, false, _tag, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("menu_icon_played_off"));
		uIFittedSprite.SetMargins(0.15f, RelativeTo.ParentHeight);
		new UIText(uIHorizontalList2, false, _tag, "<color=#8ea5ce>" + m_minigameMetaData.timesPlayed + "</color>", "Fonts/HurmeSemiBold", 0.5f, RelativeTo.ParentHeight);
		UIHorizontalList uIHorizontalList3 = new UIHorizontalList(uIHorizontalList, _tag);
		uIHorizontalList3.SetSpacing(0.1f, RelativeTo.ParentHeight);
		uIHorizontalList3.SetMargins(0f, 0.3f, 0f, 0f, RelativeTo.ParentHeight);
		uIHorizontalList3.RemoveTouchAreas();
		uIHorizontalList3.RemoveDrawHandler();
		UIFittedSprite uIFittedSprite2 = new UIFittedSprite(uIHorizontalList3, true, _tag, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("menu_icon_liked_off"));
		uIFittedSprite2.SetMargins(0.15f, RelativeTo.ParentHeight);
		uIFittedSprite2.SetTouchHandler(LikeButtonHandler);
		new UIText(uIHorizontalList3, false, _tag, "<color=#8ea5ce>" + m_minigameMetaData.timesLiked + "</color>", "Fonts/HurmeSemiBold", 0.5f, RelativeTo.ParentHeight);
	}

	private void ScreenshotOK(byte[] _bytes)
	{
		m_screenshot = new Texture2D(100, 100, TextureFormat.RGB24, false);
		m_screenshot.LoadImage(_bytes);
		if (m_screenshotPrefabs.Count > 0)
		{
			while (m_screenshotPrefabs.Count > 0)
			{
				int index = m_screenshotPrefabs.Count - 1;
				PrefabS.RemoveComponent(m_screenshotPrefabs[index]);
				m_screenshotPrefabs.RemoveAt(index);
			}
			Material material = Object.Instantiate(ResourceManager.GetMaterial("UI/ScreenshotMat")) as Material;
			material.mainTexture = m_screenshot;
			m_screenshotPrefabs = PrefabS.CreateFlatPrefabComponentsFromPolygon(m_TC, Vector3.forward * 2.5f, m_screenshotPolygon, DebugDraw.ColorToUInt(Color.white), DebugDraw.ColorToUInt(Color.white), material, m_camera, "Screenshot", UVRect.Normal());
		}
		Debug.Log("Screenshot OK");
	}

	private void ScreenshotFAIL(WWWRequest _request)
	{
		Debug.Log("Screenshot FAILED");
	}

	protected override void OnTouchRelease(TLTouch _touch, bool _inside)
	{
		base.OnTouchRelease(_touch, _inside);
		if (_inside && !_touch.m_primaryArea.m_wasDragged)
		{
			string[] propertyKeys = new string[1] { "metadata" };
			object[] propertyValues = new object[1] { m_minigameMetaData };
			EventS.Dispatch("MINIGAME_BANNER_EVENT", propertyKeys, propertyValues, false);
		}
	}

	protected override void OnTouchDragStart(TLTouch _touch)
	{
		Highlight(false);
		HighlightSecondary(false);
		m_end = true;
	}

	private void DestroyTouchHandler(TouchAreaC _touchArea, int _touchCount, TLTouch[] _touches, TouchAreaPhase[] _touchPhases, bool[] _touchIsSecondary)
	{
		if (!_touchIsSecondary[0])
		{
			if (_touchPhases[0] == TouchAreaPhase.Began)
			{
				m_startDestroyPress = Main.m_gameTime;
			}
			else if ((_touchPhases[0] == TouchAreaPhase.MoveIn || _touchPhases[0] == TouchAreaPhase.StationaryIn) && Main.m_gameTime - m_startDestroyPress > 5f)
			{
				Server.DeleteMiniGame(m_minigameMetaData.id);
				CacheManager.RemoveCache("DISCOVER_CACHE");
				UIComponent parent = m_parent;
				Destroy();
				parent.Update();
			}
		}
	}

	private void LikeButtonHandler(TouchAreaC _touchArea, int _touchCount, TLTouch[] _touches, TouchAreaPhase[] _touchPhases, bool[] _touchIsSecondary)
	{
		if (!_touchIsSecondary[0] && _touchPhases[0] == TouchAreaPhase.ReleaseIn)
		{
			Server.SaveLike(m_minigameMetaData.id);
		}
	}

	public override void DrawHandler(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		if (m_bannerShape == null)
		{
			m_bannerShape = new Vector2[8];
			m_bannerShape[0] = new Vector2(_c.m_actualWidth * -0.5f, _c.m_actualHeight * 0.5f);
			m_bannerShape[1] = new Vector2(_c.m_actualWidth * -0.191f, _c.m_actualHeight * 0.5f);
			m_bannerShape[2] = new Vector2(_c.m_actualWidth * 0.5f, _c.m_actualHeight * 0.5f);
			m_bannerShape[3] = new Vector2(_c.m_actualWidth * 0.5f, _c.m_actualHeight * 0.191f);
			m_bannerShape[4] = new Vector2(_c.m_actualWidth * 0.5f, _c.m_actualHeight * -0.5f);
			m_bannerShape[5] = new Vector2(_c.m_actualWidth * 0.191f, _c.m_actualHeight * -0.5f);
			m_bannerShape[6] = new Vector2(_c.m_actualWidth * -0.5f, _c.m_actualHeight * -0.5f);
			m_bannerShape[7] = new Vector2(_c.m_actualWidth * -0.5f, _c.m_actualHeight * -0.191f);
			DebugDraw.AddRandom(m_bannerShape, _c.m_actualHeight * 0.035f);
		}
		Vector2[] p = new Vector2[6]
		{
			new Vector2(_c.m_actualWidth * -0.6f, _c.m_actualHeight * 0.6f),
			new Vector2(_c.m_actualWidth * 0.6f, _c.m_actualHeight * 0.6f),
			new Vector2(_c.m_actualWidth * 0.6f, _c.m_actualHeight * 0.35f),
			new Vector2(_c.m_actualWidth * -0.4f, _c.m_actualHeight * 0.35f),
			new Vector2(_c.m_actualWidth * -0.415f, _c.m_actualHeight * 0.275f),
			new Vector2(_c.m_actualWidth * -0.6f, _c.m_actualHeight * 0.2f)
		};
		Vector2[] p2 = new Vector2[4]
		{
			new Vector2(_c.m_actualWidth * -0.6f, _c.m_actualHeight * -0.6f),
			new Vector2(_c.m_actualWidth * 0.6f, _c.m_actualHeight * -0.6f),
			new Vector2(_c.m_actualWidth * 0.6f, _c.m_actualHeight * -0.35f),
			new Vector2(_c.m_actualWidth * -0.6f, _c.m_actualHeight * -0.35f)
		};
		Vector2[] p3 = new Vector2[4]
		{
			new Vector2(_c.m_actualWidth * 0.175f, _c.m_actualHeight * 0.37f),
			new Vector2(_c.m_actualWidth * 0.6f, _c.m_actualHeight * 0.37f),
			new Vector2(_c.m_actualWidth * 0.6f, _c.m_actualHeight * -0.6f),
			new Vector2(_c.m_actualWidth * 0.175f, _c.m_actualHeight * -0.6f)
		};
		Polygon polygon = new Polygon();
		polygon.AddContour(new VertexList(m_bannerShape), false);
		Polygon polygon2 = new Polygon();
		polygon2.AddContour(new VertexList(p), false);
		Polygon polygon3 = new Polygon();
		polygon3.AddContour(new VertexList(p2), false);
		Polygon polygon4 = new Polygon();
		polygon4.AddContour(new VertexList(p3), false);
		m_screenshotPolygon = polygon4.Clip(GpcOperation.Intersection, polygon);
		Polygon polygon5 = polygon.Clip(GpcOperation.Difference, polygon2);
		polygon5 = polygon5.Clip(GpcOperation.Difference, polygon3);
		Polygon polygon6 = polygon2.Clip(GpcOperation.Intersection, polygon);
		Polygon polygon7 = polygon3.Clip(GpcOperation.Intersection, polygon);
		Camera camera = CameraS.m_uiCamera;
		if (_c.m_parent != null)
		{
			camera = _c.m_parent.m_camera;
		}
		PrefabS.CreateFlatPrefabComponentsFromPolygon(_c.m_TC, Vector3.forward * 0f, polygon6, DebugDraw.GetColor(81f, 200f, 10f), ResourceManager.GetMaterial("Framework/SolidMat"), camera);
		PrefabS.CreatePathPrefabComponentFromPolygon(_c.m_TC, Vector3.forward * -0.5f, polygon6, 4f, DebugDraw.GetColor(81f, 200f, 10f), ResourceManager.GetMaterial("Framework/Line4Mat"), camera, Position.Center, true);
		PrefabS.CreateFlatPrefabComponentsFromPolygon(_c.m_TC, Vector3.forward * 5f, polygon5, DebugDraw.GetColor(0f, 59f, 111f), ResourceManager.GetMaterial("Framework/SolidMat"), camera);
		PrefabS.CreatePathPrefabComponentFromPolygon(_c.m_TC, Vector3.forward * 4.5f, polygon5, 4f, DebugDraw.GetColor(0f, 59f, 111f), ResourceManager.GetMaterial("Framework/Line4Mat"), camera, Position.Center, true);
		PrefabS.CreateFlatPrefabComponentsFromPolygon(_c.m_TC, Vector3.forward * 10f, polygon7, DebugDraw.GetColor(21f, 75f, 130f), ResourceManager.GetMaterial("Framework/SolidMat"), camera);
		PrefabS.CreatePathPrefabComponentFromPolygon(_c.m_TC, Vector3.forward * 9.5f, polygon7, 4f, DebugDraw.GetColor(21f, 75f, 130f), ResourceManager.GetMaterial("Framework/Line4Mat"), camera, Position.Center, true);
		Material material = Object.Instantiate(ResourceManager.GetMaterial("UI/ScreenshotMat")) as Material;
		if (m_screenshot != null)
		{
			material.mainTexture = m_screenshot;
		}
		m_screenshotPrefabs = PrefabS.CreateFlatPrefabComponentsFromPolygon(_c.m_TC, Vector3.forward * 2.5f, m_screenshotPolygon, DebugDraw.ColorToUInt(Color.white), DebugDraw.ColorToUInt(Color.white), material, camera, "Screenshot", UVRect.Normal());
	}

	public override void Destroy()
	{
		base.Destroy();
		m_screenshotPrefabs.Clear();
	}
}
