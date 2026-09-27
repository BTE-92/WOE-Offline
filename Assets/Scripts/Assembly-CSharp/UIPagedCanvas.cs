using System;
using System.Collections.Generic;
using UnityEngine;

public class UIPagedCanvas : UIScrollableCanvas
{
	private List<UIComponent> m_childsWithUniqueCameras;

	protected int m_currentPage;

	protected bool m_changingPage;

	public UIPagedCanvas(UIComponent _parent, string _tag)
		: base(_parent, _tag)
	{
		m_currentPage = 0;
		m_maxScrollInertialX = 50f;
		RemoveTouchAreas();
	}

	public override void Step()
	{
		if (m_changingPage)
		{
			float num = 0f;
			if (m_contentWidth > m_actualWidth - m_actualMargins.l - m_actualMargins.r)
			{
				num = m_contentCenterX - m_contentWidth * 0.5f + m_actualWidth * 0.5f - m_actualMargins.l;
			}
			m_scrollInertiaX = Mathf.Min(m_maxScrollInertialX, Mathf.Max(0f - m_maxScrollInertialX, m_scrollInertiaX));
			float num2 = (float)m_currentPage * m_actualWidth;
			float num3 = m_scrollTC.transform.localPosition.x - num;
			float num4 = Mathf.Max(m_actualWidth, 0f);
			if (m_overrideScrollPosition)
			{
				num3 = num4 * m_currentScrollX + num;
				TransformS.SetPosition(m_scrollTC, new Vector3(num3, 0f, 0f));
				m_overrideScrollPosition = false;
				return;
			}
			float scrollInertiaX = m_scrollInertiaX;
			if (num3 <= num2)
			{
				m_scrollInertiaX = (num3 - num2) * (0f - (1f - m_scrollFallOff * 0.618f));
				m_scrollInertiaX = Mathf.Max(m_scrollInertiaX, (num3 - num2) * (0f - (1f - m_scrollFallOff * 0.618f)));
				if (Math.Abs(m_scrollInertiaX) < 0.1f)
				{
					m_scrollInertiaX = num3 - num2;
					m_changingPage = false;
					if (m_childs.Count > m_currentPage)
					{
						m_childs[m_currentPage].Focus();
					}
					EnableTouchAreas();
				}
				scrollInertiaX = m_scrollInertiaX;
			}
			else if (num3 > num2)
			{
				m_scrollInertiaX = (0f - (num2 - num3)) * (0f - (1f - m_scrollFallOff * 0.618f));
				m_scrollInertiaX = Mathf.Min(m_scrollInertiaX, (0f - (num2 - num3)) * (0f - (1f - m_scrollFallOff * 0.618f)));
				if (Math.Abs(m_scrollInertiaX) < 0.1f)
				{
					m_scrollInertiaX = 0f - (num2 - num3);
					m_changingPage = false;
					if (m_childs.Count > m_currentPage)
					{
						m_childs[m_currentPage].Focus();
					}
					EnableTouchAreas();
				}
				scrollInertiaX = m_scrollInertiaX;
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
			if (num4 > 0f)
			{
				m_currentScrollX = num3 / num4;
			}
			else
			{
				m_currentScrollX = 0f;
			}
			if (scrollInertiaX != 0f)
			{
				TransformS.Move(m_scrollTC, new Vector3(scrollInertiaX, 0f, 0f));
			}
			ModifyChildCameras(m_scrollTC.transform.localPosition.x);
		}
		base.Step();
	}

	public virtual void NextPage()
	{
		m_currentPage++;
		m_changingPage = true;
	}

	public virtual void PreviousPage()
	{
		m_currentPage--;
		m_changingPage = true;
	}

	public virtual void GoToPage(int _pageIndex, bool _immediate)
	{
		m_currentPage = _pageIndex;
		m_changingPage = true;
		if (_immediate)
		{
			float num = (float)_pageIndex * m_actualWidth;
			float num2 = (float)m_childs.Count * m_actualWidth;
			if (m_childs.Count == 0)
			{
				m_currentScrollX = 0f;
			}
			else
			{
				m_currentScrollX = num / num2;
			}
			m_overrideScrollPosition = true;
			Step();
		}
	}

	public virtual bool IsChangingPage()
	{
		if (m_changingPage)
		{
			return true;
		}
		return false;
	}

	public override void Update()
	{
		base.Update();
		m_childsWithUniqueCameras = GetChildsWithUniqueCamera(this);
		ModifyChildCameras(m_scrollTC.transform.localPosition.x);
	}

	public override void UpdateSize()
	{
		base.UpdateSize();
	}

	public override void DestroyChildren()
	{
		base.DestroyChildren();
		m_currentPage = 0;
	}

	protected void ModifyChildCameras(float _posX)
	{
		for (int i = 0; i < m_childsWithUniqueCameras.Count; i++)
		{
			Vector3 localPosition = m_childsWithUniqueCameras[i].m_camera.transform.localPosition;
			localPosition.x = _posX - m_actualWidth * (float)i;
			m_childsWithUniqueCameras[i].m_camera.transform.localPosition = localPosition;
		}
	}

	protected List<UIComponent> GetChildsWithUniqueCamera(UIComponent _parent)
	{
		List<UIComponent> list = new List<UIComponent>();
		for (int i = 0; i < _parent.m_childs.Count; i++)
		{
			if (_parent.m_childs[i].m_uniqueCamera)
			{
				list.Add(_parent.m_childs[i]);
			}
			else
			{
				list.AddRange(GetChildsWithUniqueCamera(_parent.m_childs[i]));
			}
		}
		return list;
	}
}
