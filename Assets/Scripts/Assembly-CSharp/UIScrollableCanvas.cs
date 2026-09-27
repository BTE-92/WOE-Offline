using UnityEngine;

public class UIScrollableCanvas : UICanvas
{
	public TransformC m_scrollTC;

	public float m_scrollInertiaY;

	public float m_scrollInertiaX;

	public float m_maxScrollInertialX;

	public float m_maxScrollInertialY;

	public float m_scrollFallOff;

	public float m_currentScrollX;

	public float m_currentScrollY;

	public bool m_overrideScrollPosition;

	public float m_scrollInertiaMultiplerX;

	public float m_scrollInertiaMultiplerY;

	public bool m_allowScroll;

	public UIScrollableCanvas(UIComponent _parent, string _tag)
		: base(_parent, _tag, null, string.Empty)
	{
		m_TAC.m_allowSecondary = true;
		m_TAC.m_clip = true;
		SetScrollable();
		SetScrollPosition(0f, 0f);
		m_maxScrollInertialX = 0f;
		m_maxScrollInertialY = 200f;
		m_scrollFallOff = 0.9f;
		m_scrollInertiaMultiplerX = 1f;
		m_scrollInertiaMultiplerY = 1f;
		m_allowScroll = false;
	}

	protected virtual void SetScrollable()
	{
		m_uniqueCamera = true;
		m_camera = CameraS.AddCamera("UI Container Camera", true);
		m_scrollTC = TransformS.AddComponent(m_TAC.p_entity, "Scroll Transform");
		TransformS.ParentComponent(m_scrollTC, m_TC, Vector3.zero);
		m_camera.transform.parent = m_scrollTC.transform;
		m_camera.transform.localPosition = Vector3.zero;
		GameObject gameObject = m_TC.transform.gameObject;
		GameObject gameObject2 = m_scrollTC.transform.gameObject;
		gameObject2.layer = gameObject.layer;
		if (m_TAC != null)
		{
			MeshCollider collider = m_TAC.m_collider;
			TouchAreaBootstrap component = gameObject.GetComponent<TouchAreaBootstrap>();
			MeshCollider meshCollider = gameObject2.AddComponent<MeshCollider>();
			meshCollider.sharedMesh = collider.sharedMesh;
			m_TAC.m_collider = meshCollider;
			m_TAC.m_TC = m_scrollTC;
			TouchAreaBootstrap touchAreaBootstrap = gameObject2.AddComponent<TouchAreaBootstrap>();
			touchAreaBootstrap.m_TAC = m_TAC;
			Object.DestroyImmediate(collider);
			Object.DestroyImmediate(component);
		}
		AssignCameraToComponentAndChildren(m_camera);
	}

	protected virtual void AssignCameraToComponentAndChildren(Camera _camera)
	{
		if (m_TAC != null)
		{
			m_TAC.m_camera = _camera;
			m_TAC.m_TC.transform.gameObject.layer = _camera.gameObject.layer;
		}
		m_TC.transform.gameObject.layer = _camera.gameObject.layer;
		for (int i = 0; i < m_TC.p_entity.m_components.Count; i++)
		{
			IComponent component = m_TC.p_entity.m_components[i];
			if (component.m_componentType == ComponentType.TouchArea)
			{
				TouchAreaS.SetCamera(component as TouchAreaC, _camera);
			}
			else if (component.m_componentType == ComponentType.Prefab)
			{
				PrefabS.SetCamera(component as PrefabC, _camera);
			}
		}
	}

	public virtual void SetScrollPosition(float _horizontal, float _vertical)
	{
		m_currentScrollX = _horizontal;
		m_currentScrollY = 0f - _vertical;
		m_overrideScrollPosition = true;
	}

	public override void FreezeHorizontalScroll(bool _affectWholeHierarchy)
	{
		m_scrollInertiaMultiplerX = 0f;
		if (_affectWholeHierarchy)
		{
			base.FreezeHorizontalScroll(_affectWholeHierarchy);
		}
	}

	public override void UnfreezeHorizontalScroll(bool _affectWholeHierarchy)
	{
		m_scrollInertiaMultiplerX = 1f;
		if (_affectWholeHierarchy)
		{
			base.UnfreezeHorizontalScroll(_affectWholeHierarchy);
		}
	}

	public override void FreezeVerticalScroll(bool _affectWholeHierarchy)
	{
		m_scrollInertiaMultiplerY = 0f;
		if (_affectWholeHierarchy)
		{
			base.FreezeVerticalScroll(_affectWholeHierarchy);
		}
	}

	public override void UnfreezeVerticalScroll(bool _affectWholeHierarchy)
	{
		m_scrollInertiaMultiplerY = 1f;
		if (_affectWholeHierarchy)
		{
			base.UnfreezeVerticalScroll(_affectWholeHierarchy);
		}
	}

	public override void Update()
	{
		CalculateReferenceSizes();
		UpdateSize();
		UpdateAlign();
		UpdateMargins();
		if (d_Draw != null)
		{
			d_Draw(this);
		}
		UpdateUniqueCamera();
		UpdateChildren();
		ArrangeContents();
	}

	public override void UpdateUniqueCamera()
	{
		m_camera.orthographicSize = m_actualHeight * 0.5f;
		Vector3 position = m_TC.transform.position;
		m_camera.transform.localPosition = Vector3.forward * -500f;
		Rect rect = new Rect((position.x - m_actualWidth * 0.5f) / (float)Screen.width + 0.5f, (position.y - m_actualHeight * 0.5f) / (float)Screen.height + 0.5f, m_actualWidth / (float)Screen.width, m_actualHeight / (float)Screen.height);
		m_camera.rect = rect;
		if (m_TAC != null)
		{
			m_TAC.m_clip = true;
			m_TAC.m_clipBB.l = rect.xMin * (float)Screen.width;
			m_TAC.m_clipBB.r = rect.xMax * (float)Screen.width;
			m_TAC.m_clipBB.b = rect.yMin * (float)Screen.height;
			m_TAC.m_clipBB.t = rect.yMax * (float)Screen.height;
		}
		AdjustCamera(true);
	}

	public void AdjustCamera(bool _force = false)
	{
		UIComponent parent = m_parent;
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		bool flag = false;
		while (parent != null)
		{
			if (parent.GetType() == typeof(UIScrollableCanvas))
			{
				UIScrollableCanvas uIScrollableCanvas = parent as UIScrollableCanvas;
				zero += uIScrollableCanvas.m_scrollTC.transform.localPosition;
				flag = true;
			}
			zero2 += parent.m_TC.transform.localPosition;
			parent = parent.m_parent;
		}
		if (flag || _force)
		{
			Vector3 position = m_TC.transform.position;
			float num = m_scrollTC.transform.position.x - zero.x + m_actualWidth * 0.5f;
			float b = m_scrollTC.transform.position.x - zero.x - m_actualWidth * 0.5f;
			float num2 = m_scrollTC.transform.position.y - zero.y + m_actualHeight * 0.5f;
			float b2 = m_scrollTC.transform.position.y - zero.y - m_actualHeight * 0.5f;
			num = Mathf.Max(0f, 0f - num);
			b = Mathf.Max(0f, b);
			num2 = Mathf.Max(0f, 0f - num2);
			b2 = Mathf.Max(0f, b2);
			float y = m_camera.transform.localPosition.y;
			m_camera.transform.localPosition = new Vector3((0f - b + num) * 0.5f, y, -500f);
			Rect rect = new Rect((position.x + (0f - zero.x) + num - m_actualWidth * 0.5f) / (float)Screen.width + 0.5f, (position.y + (0f - zero.y) - m_actualHeight * 0.5f) / (float)Screen.height + 0.5f, (m_actualWidth - num - b) / (float)Screen.width, m_actualHeight / (float)Screen.height);
			if (rect.width < 0f)
			{
				rect.width = 0f;
			}
			if (rect.height < 0f)
			{
				rect.height = 0f;
			}
			m_camera.rect = rect;
			if (m_TAC != null)
			{
				m_TAC.m_clip = true;
				m_TAC.m_clipBB.l = rect.xMin * (float)Screen.width;
				m_TAC.m_clipBB.r = rect.xMax * (float)Screen.width;
				m_TAC.m_clipBB.b = rect.yMin * (float)Screen.height;
				m_TAC.m_clipBB.t = rect.yMax * (float)Screen.height;
			}
		}
	}

	public override void Step()
	{
		if (m_allowScroll)
		{
			float num = 0f;
			float num2 = 0f;
			if (m_contentWidth > m_actualWidth - m_actualMargins.l - m_actualMargins.r)
			{
				num = m_contentCenterX - m_contentWidth * 0.5f + m_actualWidth * 0.5f - m_actualMargins.l;
			}
			if (m_contentHeight > m_actualHeight - m_actualMargins.t - m_actualMargins.b)
			{
				num2 = m_contentCenterY + m_contentHeight * 0.5f - m_actualHeight * 0.5f + m_actualMargins.t;
			}
			m_scrollInertiaX = Mathf.Min(m_maxScrollInertialX, Mathf.Max(0f - m_maxScrollInertialX, m_scrollInertiaX));
			m_scrollInertiaY = Mathf.Min(m_maxScrollInertialY, Mathf.Max(0f - m_maxScrollInertialY, m_scrollInertiaY));
			float num3 = m_scrollTC.transform.localPosition.x - num;
			float num4 = Mathf.Max(m_contentWidth + m_actualMargins.l + m_actualMargins.r - m_actualWidth, 0f);
			float num5 = m_scrollTC.transform.localPosition.y - num2;
			float num6 = Mathf.Max(m_contentHeight + m_actualMargins.t + m_actualMargins.b - m_actualHeight, 0f);
			if (m_overrideScrollPosition)
			{
				num3 = num4 * m_currentScrollX + num;
				num5 = num6 * m_currentScrollY + num2;
				TransformS.SetPosition(m_scrollTC, new Vector3(num3, num5, 0f));
				m_overrideScrollPosition = false;
				return;
			}
			float scrollInertiaX = m_scrollInertiaX;
			if ((num3 < 0f || num3 > num4) && m_TAC != null && m_TAC.m_touchCount > 0)
			{
				scrollInertiaX *= 0.382f;
			}
			else if (num3 < 0f)
			{
				m_scrollInertiaX += num3 * (0f - (1f - m_scrollFallOff * 0.618f));
				m_scrollInertiaX = Mathf.Max(m_scrollInertiaX, num3 * (0f - (1f - m_scrollFallOff * 0.618f)));
				scrollInertiaX = (m_scrollInertiaX *= 0.382f);
			}
			else if (num3 > num4)
			{
				m_scrollInertiaX += (num3 - num4) * (0f - (1f - m_scrollFallOff * 0.618f));
				m_scrollInertiaX = Mathf.Min(m_scrollInertiaX, (num3 - num4) * (0f - (1f - m_scrollFallOff * 0.618f)));
				scrollInertiaX = (m_scrollInertiaX *= 0.382f);
			}
			else
			{
				if (Mathf.Abs(m_scrollInertiaX) > 0.1f)
				{
					m_scrollInertiaX *= m_scrollFallOff;
				}
				else
				{
					m_scrollInertiaX = 0f;
				}
				scrollInertiaX = m_scrollInertiaX;
			}
			float scrollInertiaY = m_scrollInertiaY;
			if ((num5 > 0f || num5 < 0f - num6) && m_TAC != null && m_TAC.m_touchCount > 0)
			{
				scrollInertiaY *= 0.382f;
			}
			else if (num5 > 0f)
			{
				m_scrollInertiaY += num5 * (0f - (1f - m_scrollFallOff * 0.618f));
				m_scrollInertiaY = Mathf.Max(m_scrollInertiaY, num5 * (0f - (1f - m_scrollFallOff * 0.618f)));
				scrollInertiaY = m_scrollInertiaY * 0.382f;
			}
			else if (num5 < 0f - num6)
			{
				m_scrollInertiaY += (num5 - (0f - num6)) * (0f - (1f - m_scrollFallOff * 0.618f));
				m_scrollInertiaY = Mathf.Min(m_scrollInertiaY, (num5 - (0f - num6)) * (0f - (1f - m_scrollFallOff * 0.618f)));
				scrollInertiaY = m_scrollInertiaY * 0.382f;
			}
			else
			{
				if (Mathf.Abs(m_scrollInertiaY) > 0.1f)
				{
					m_scrollInertiaY *= m_scrollFallOff;
				}
				else
				{
					m_scrollInertiaY = 0f;
				}
				scrollInertiaY = m_scrollInertiaY;
			}
			if (num4 > 0f)
			{
				m_currentScrollX = num3 / num4;
			}
			else
			{
				m_currentScrollX = 0f;
			}
			if (num6 > 0f)
			{
				m_currentScrollY = num5 / (0f - num6);
			}
			else
			{
				m_currentScrollY = 0f;
			}
			scrollInertiaX *= m_scrollInertiaMultiplerX;
			scrollInertiaY *= m_scrollInertiaMultiplerY;
			if (scrollInertiaX != 0f || scrollInertiaY != 0f)
			{
				TransformS.Move(m_scrollTC, new Vector3(scrollInertiaX, scrollInertiaY, 0f));
			}
		}
		AdjustCamera();
		base.Step();
	}

	public override void TouchHandler(TouchAreaC _touchArea, int _touchCount, TLTouch[] _touches, TouchAreaPhase[] _touchPhases, bool[] _touchIsSecondary)
	{
		base.TouchHandler(_touchArea, _touchCount, _touches, _touchPhases, _touchIsSecondary);
		if (_touchCount != 1)
		{
			return;
		}
		TLTouch tLTouch = _touches[0];
		TouchAreaPhase touchAreaPhase = _touchPhases[0];
		bool flag = _touchIsSecondary[0];
		if (touchAreaPhase == TouchAreaPhase.Began)
		{
			m_allowScroll = false;
		}
		if (flag)
		{
			if (!tLTouch.m_secondaryLocked && touchAreaPhase == TouchAreaPhase.RollIn)
			{
				TouchAreaS.LockSecondaryTouchArea(tLTouch);
				m_allowScroll = false;
			}
			touchAreaPhase = tLTouch.m_secondaryPhase;
		}
		switch (touchAreaPhase)
		{
		case TouchAreaPhase.DragStart:
			m_allowScroll = true;
			break;
		case TouchAreaPhase.MoveIn:
		case TouchAreaPhase.MoveOut:
		case TouchAreaPhase.StationaryIn:
		case TouchAreaPhase.StationaryOut:
			m_scrollInertiaX = 0f - tLTouch.m_deltaPosition.x;
			m_scrollInertiaY = 0f - tLTouch.m_deltaPosition.y;
			break;
		}
	}
}
