using System.Collections.Generic;

namespace ClipperLib
{
	public class PolyNode
	{
		internal PolyNode m_Parent;

		internal List<IntPoint> m_polygon = new List<IntPoint>();

		internal int m_Index;

		internal List<PolyNode> m_Childs = new List<PolyNode>();

		public int ChildCount
		{
			get
			{
				return m_Childs.Count;
			}
		}

		public List<IntPoint> Contour
		{
			get
			{
				return m_polygon;
			}
		}

		public List<PolyNode> Childs
		{
			get
			{
				return m_Childs;
			}
		}

		public PolyNode Parent
		{
			get
			{
				return m_Parent;
			}
		}

		public bool IsHole
		{
			get
			{
				return IsHoleNode();
			}
		}

		public bool IsOpen { get; set; }

		private bool IsHoleNode()
		{
			bool flag = true;
			for (PolyNode parent = m_Parent; parent != null; parent = parent.m_Parent)
			{
				flag = !flag;
			}
			return flag;
		}

		internal void AddChild(PolyNode Child)
		{
			int count = m_Childs.Count;
			m_Childs.Add(Child);
			Child.m_Parent = this;
			Child.m_Index = count;
		}

		public PolyNode GetNext()
		{
			if (m_Childs.Count > 0)
			{
				return m_Childs[0];
			}
			return GetNextSiblingUp();
		}

		internal PolyNode GetNextSiblingUp()
		{
			if (m_Parent == null)
			{
				return null;
			}
			if (m_Index == m_Parent.m_Childs.Count - 1)
			{
				return m_Parent.GetNextSiblingUp();
			}
			return m_Parent.m_Childs[m_Index + 1];
		}
	}
}
