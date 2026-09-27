using UnityEngine;

public class UIMinigameCard : UIVerticalList
{
	public static float m_defaultWidth = 1f;

	public static cpBB m_defaultMargins = new cpBB(0f);

	public static RelativeTo m_defaultWidthRelativeTo = RelativeTo.ParentWidth;

	public static RelativeTo m_defaultHeightRelativeTo = RelativeTo.OwnWidth;

	public static RelativeTo m_defaultMarginsRelativeTo = RelativeTo.ScreenShortest;

	protected MinigameMetaData m_minigameMetaData;

	public UICanvas m_headerCanvas;

	public UICanvas m_contentCanvas;

	public UICanvas m_footerCanvas;

	private Vector2[] m_bannerShape;

	public UIMinigameCard(UIVerticalList _parent, string _tag, MinigameMetaData _minigameMetaData)
		: base(_parent, _tag)
	{
		RemoveTouchAreas();
		m_minigameMetaData = _minigameMetaData;
		SetWidth(m_defaultWidth, m_defaultWidthRelativeTo);
		SetMargins(m_defaultMargins, m_defaultMarginsRelativeTo);
		m_contentCanvas = new UICanvas(this, _tag, null, string.Empty);
		m_contentCanvas.SetWidth(1f, RelativeTo.ParentWidth);
		m_contentCanvas.SetHeight(0.7f, RelativeTo.ParentHeight);
		m_contentCanvas.SetVerticalAlign(0.5f);
		m_contentCanvas.SetMargins(0.125f, 0.3f, 0f, 0f, RelativeTo.OwnWidth);
		m_contentCanvas.RemoveTouchAreas();
		m_contentCanvas.RemoveDrawHandler();
		UIVerticalList uIVerticalList = new UIVerticalList(m_contentCanvas, _tag);
		uIVerticalList.SetVerticalAlign(1f);
		uIVerticalList.SetHorizontalAlign(0f);
		uIVerticalList.RemoveTouchAreas();
		uIVerticalList.RemoveDrawHandler();
		UIText uIText = new UIText(uIVerticalList, false, _tag, "<color=#afed32>" + m_minigameMetaData.name.ToLower() + "</color>", "Fonts/KGLetHerGo", 0.1f, RelativeTo.ParentWidth);
		uIText.SetMargins(0f, 0f, 0.06f, -0.02f, RelativeTo.ParentWidth);
		uIText.SetHorizontalAlign(0f);
		UIText uIText2 = new UIText(uIVerticalList, false, _tag, "<color=#8ea5ce>By " + m_minigameMetaData.creatorName + "</color>", "Fonts/HurmeSemiBold", 0.05f, RelativeTo.ParentWidth);
		uIText2.SetHorizontalAlign(0f);
		string text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam sodales varius leo, et commodo nunc. Ut hendrerit enim ac dictum feugiat. Sed id nunc ut tortor congue porta. Suspendisse sed nisi ipsum. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Donec non sagittis odio. Sed lacinia, arcu vel sagittis consequat, purus arcu iaculis elit, quis tempor odio diam at magna. Nam at dolor blandit, sollicitudin lorem at, accumsan nunc. Mauris facilisis adipiscing tempus.";
		UITextbox uITextbox = new UITextbox(uIVerticalList, false, _tag, text, "Fonts/HurmeRegular", 0.04f, RelativeTo.ParentWidth);
		uITextbox.SetMargins(0f, 0f, 0.03f, 0.03f, RelativeTo.ParentWidth);
		m_footerCanvas = new UICanvas(this, _tag, null, string.Empty);
		m_footerCanvas.SetWidth(1f, RelativeTo.ParentWidth);
		m_footerCanvas.SetHeight(0.05f, RelativeTo.ParentWidth);
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
		UIFittedSprite uIFittedSprite = new UIFittedSprite(uIHorizontalList2, false, _tag, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("hud_icon_settings"));
		uIFittedSprite.SetMargins(0.15f, RelativeTo.ParentHeight);
		new UIText(uIHorizontalList2, false, _tag, "<color=#8ea5ce>" + m_minigameMetaData.timesPlayed + "</color>", "Fonts/HurmeSemiBold", 0.5f, RelativeTo.ParentHeight);
		UIHorizontalList uIHorizontalList3 = new UIHorizontalList(uIHorizontalList, _tag);
		uIHorizontalList3.SetSpacing(0.1f, RelativeTo.ParentHeight);
		uIHorizontalList3.SetMargins(0f, 0.3f, 0f, 0f, RelativeTo.ParentHeight);
		uIHorizontalList3.RemoveTouchAreas();
		uIHorizontalList3.RemoveDrawHandler();
		UIFittedSprite uIFittedSprite2 = new UIFittedSprite(uIHorizontalList3, false, _tag, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("hud_icon_settings"));
		uIFittedSprite2.SetMargins(0.15f, RelativeTo.ParentHeight);
		new UIText(uIHorizontalList3, false, _tag, "<color=#8ea5ce>" + m_minigameMetaData.timesLiked + "</color>", "Fonts/HurmeSemiBold", 0.5f, RelativeTo.ParentHeight);
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
			DebugDraw.AddRandom(m_bannerShape, _c.m_actualHeight * 0.05f);
		}
		Vector2[] p = new Vector2[4]
		{
			new Vector2(_c.m_actualWidth * -0.6f, _c.m_actualHeight * 0.6f),
			new Vector2(_c.m_actualWidth * 0.6f, _c.m_actualHeight * 0.6f),
			new Vector2(_c.m_actualWidth * 0.6f, _c.m_actualHeight * 0.5f - (float)Screen.height * 0.025f),
			new Vector2(_c.m_actualWidth * -0.6f, _c.m_actualHeight * 0.5f - (float)Screen.height * 0.025f)
		};
		Vector2[] p2 = new Vector2[4]
		{
			new Vector2(_c.m_actualWidth * -0.6f, _c.m_actualHeight * -0.6f),
			new Vector2(_c.m_actualWidth * 0.6f, _c.m_actualHeight * -0.6f),
			new Vector2(_c.m_actualWidth * 0.6f, _c.m_actualHeight * -0.35f),
			new Vector2(_c.m_actualWidth * -0.6f, _c.m_actualHeight * -0.35f)
		};
		Polygon polygon = new Polygon();
		polygon.AddContour(new VertexList(m_bannerShape), false);
		Polygon polygon2 = new Polygon();
		polygon2.AddContour(new VertexList(p), false);
		Polygon polygon3 = new Polygon();
		polygon3.AddContour(new VertexList(p2), false);
		Polygon polygon4 = polygon.Clip(GpcOperation.Difference, polygon2);
		polygon4 = polygon4.Clip(GpcOperation.Difference, polygon3);
		Polygon polygon5 = polygon2.Clip(GpcOperation.Intersection, polygon);
		Polygon polygon6 = polygon3.Clip(GpcOperation.Intersection, polygon);
		Camera camera = CameraS.m_uiCamera;
		if (_c.m_parent != null)
		{
			camera = _c.m_parent.m_camera;
		}
		PrefabS.CreateFlatPrefabComponentsFromPolygon(_c.m_TC, Vector3.forward * 0f, polygon5, DebugDraw.GetColor(81f, 200f, 10f), ResourceManager.GetMaterial("Framework/SolidMat"), camera);
		PrefabS.CreatePathPrefabComponentFromPolygon(_c.m_TC, Vector3.forward * -0.5f, polygon5, 4f, DebugDraw.GetColor(81f, 200f, 10f), ResourceManager.GetMaterial("Framework/Line4Mat"), camera, Position.Center, true);
		PrefabS.CreateFlatPrefabComponentsFromPolygon(_c.m_TC, Vector3.forward * 5f, polygon4, DebugDraw.GetColor(0f, 59f, 111f), ResourceManager.GetMaterial("Framework/SolidMat"), camera);
		PrefabS.CreatePathPrefabComponentFromPolygon(_c.m_TC, Vector3.forward * 4.5f, polygon4, 4f, DebugDraw.GetColor(0f, 59f, 111f), ResourceManager.GetMaterial("Framework/Line4Mat"), camera, Position.Center, true);
		PrefabS.CreateFlatPrefabComponentsFromPolygon(_c.m_TC, Vector3.forward * 10f, polygon6, DebugDraw.GetColor(21f, 75f, 130f), ResourceManager.GetMaterial("Framework/SolidMat"), camera);
		PrefabS.CreatePathPrefabComponentFromPolygon(_c.m_TC, Vector3.forward * 9.5f, polygon6, 4f, DebugDraw.GetColor(21f, 75f, 130f), ResourceManager.GetMaterial("Framework/Line4Mat"), camera, Position.Center, true);
	}
}
