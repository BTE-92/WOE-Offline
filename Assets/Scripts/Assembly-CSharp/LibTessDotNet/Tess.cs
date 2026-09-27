using System;

namespace LibTessDotNet
{
	public class Tess
	{
		internal class ActiveRegion
		{
			internal MeshUtils.Edge _eUp;

			internal Dict<ActiveRegion>.Node _nodeUp;

			internal int _windingNumber;

			internal bool _inside;

			internal bool _sentinel;

			internal bool _dirty;

			internal bool _fixUpperEdge;
		}

		private Mesh _mesh;

		private Vec3 _normal;

		private Vec3 _sUnit;

		private Vec3 _tUnit;

		private float _bminX;

		private float _bminY;

		private float _bmaxX;

		private float _bmaxY;

		private WindingRule _windingRule;

		private Dict<ActiveRegion> _dict;

		private PriorityQueue<MeshUtils.Vertex> _pq;

		private MeshUtils.Vertex _event;

		private CombineCallback _combineCallback;

		private ContourVertex[] _vertices;

		private int _vertexCount;

		private int[] _elements;

		private int _elementCount;

		public float SUnitX = 1f;

		public float SUnitY = 1f;

		public Vec3 Normal
		{
			get
			{
				return _normal;
			}
			set
			{
				_normal = value;
			}
		}

		public ContourVertex[] Vertices
		{
			get
			{
				return _vertices;
			}
		}

		public int VertexCount
		{
			get
			{
				return _vertexCount;
			}
		}

		public int[] Elements
		{
			get
			{
				return _elements;
			}
		}

		public int ElementCount
		{
			get
			{
				return _elementCount;
			}
		}

		public Tess()
		{
			_normal = Vec3.Zero;
			_bminX = (_bminY = (_bmaxX = (_bmaxY = 0f)));
			_windingRule = WindingRule.EvenOdd;
			_mesh = null;
			_vertices = null;
			_vertexCount = 0;
			_elements = null;
			_elementCount = 0;
		}

		private ActiveRegion RegionBelow(ActiveRegion reg)
		{
			return reg._nodeUp._prev._key;
		}

		private ActiveRegion RegionAbove(ActiveRegion reg)
		{
			return reg._nodeUp._next._key;
		}

		private bool EdgeLeq(ActiveRegion reg1, ActiveRegion reg2)
		{
			MeshUtils.Edge eUp = reg1._eUp;
			MeshUtils.Edge eUp2 = reg2._eUp;
			if (eUp._Dst == _event)
			{
				if (eUp2._Dst == _event)
				{
					if (Geom.VertLeq(eUp._Org, eUp2._Org))
					{
						return Geom.EdgeSign(eUp2._Dst, eUp._Org, eUp2._Org) <= 0f;
					}
					return Geom.EdgeSign(eUp._Dst, eUp2._Org, eUp._Org) >= 0f;
				}
				return Geom.EdgeSign(eUp2._Dst, _event, eUp2._Org) <= 0f;
			}
			if (eUp2._Dst == _event)
			{
				return Geom.EdgeSign(eUp._Dst, _event, eUp._Org) >= 0f;
			}
			float num = Geom.EdgeEval(eUp._Dst, _event, eUp._Org);
			float num2 = Geom.EdgeEval(eUp2._Dst, _event, eUp2._Org);
			return num >= num2;
		}

		private void DeleteRegion(ActiveRegion reg)
		{
			if (reg._fixUpperEdge)
			{
			}
			reg._eUp._activeRegion = null;
			_dict.Remove(reg._nodeUp);
		}

		private void FixUpperEdge(ActiveRegion reg, MeshUtils.Edge newEdge)
		{
			_mesh.Delete(reg._eUp);
			reg._fixUpperEdge = false;
			reg._eUp = newEdge;
			newEdge._activeRegion = reg;
		}

		private ActiveRegion TopLeftRegion(ActiveRegion reg)
		{
			MeshUtils.Vertex org = reg._eUp._Org;
			do
			{
				reg = RegionAbove(reg);
			}
			while (reg._eUp._Org == org);
			if (reg._fixUpperEdge)
			{
				MeshUtils.Edge newEdge = _mesh.Connect(RegionBelow(reg)._eUp._Sym, reg._eUp._Lnext);
				FixUpperEdge(reg, newEdge);
				reg = RegionAbove(reg);
			}
			return reg;
		}

		private ActiveRegion TopRightRegion(ActiveRegion reg)
		{
			MeshUtils.Vertex dst = reg._eUp._Dst;
			do
			{
				reg = RegionAbove(reg);
			}
			while (reg._eUp._Dst == dst);
			return reg;
		}

		private ActiveRegion AddRegionBelow(ActiveRegion regAbove, MeshUtils.Edge eNewUp)
		{
			ActiveRegion activeRegion = new ActiveRegion();
			activeRegion._eUp = eNewUp;
			activeRegion._nodeUp = _dict.InsertBefore(regAbove._nodeUp, activeRegion);
			activeRegion._fixUpperEdge = false;
			activeRegion._sentinel = false;
			activeRegion._dirty = false;
			eNewUp._activeRegion = activeRegion;
			return activeRegion;
		}

		private void ComputeWinding(ActiveRegion reg)
		{
			reg._windingNumber = RegionAbove(reg)._windingNumber + reg._eUp._winding;
			reg._inside = Geom.IsWindingInside(_windingRule, reg._windingNumber);
		}

		private void FinishRegion(ActiveRegion reg)
		{
			MeshUtils.Edge eUp = reg._eUp;
			MeshUtils.Face lface = eUp._Lface;
			lface._inside = reg._inside;
			lface._anEdge = eUp;
			DeleteRegion(reg);
		}

		private MeshUtils.Edge FinishLeftRegions(ActiveRegion regFirst, ActiveRegion regLast)
		{
			ActiveRegion activeRegion = regFirst;
			MeshUtils.Edge eUp = regFirst._eUp;
			while (activeRegion != regLast)
			{
				activeRegion._fixUpperEdge = false;
				ActiveRegion activeRegion2 = RegionBelow(activeRegion);
				MeshUtils.Edge edge = activeRegion2._eUp;
				if (edge._Org != eUp._Org)
				{
					if (!activeRegion2._fixUpperEdge)
					{
						FinishRegion(activeRegion);
						break;
					}
					edge = _mesh.Connect(eUp._Lprev, edge._Sym);
					FixUpperEdge(activeRegion2, edge);
				}
				if (eUp._Onext != edge)
				{
					_mesh.Splice(edge._Oprev, edge);
					_mesh.Splice(eUp, edge);
				}
				FinishRegion(activeRegion);
				eUp = activeRegion2._eUp;
				activeRegion = activeRegion2;
			}
			return eUp;
		}

		private void AddRightEdges(ActiveRegion regUp, MeshUtils.Edge eFirst, MeshUtils.Edge eLast, MeshUtils.Edge eTopLeft, bool cleanUp)
		{
			bool flag = true;
			MeshUtils.Edge edge = eFirst;
			do
			{
				AddRegionBelow(regUp, edge._Sym);
				edge = edge._Onext;
			}
			while (edge != eLast);
			if (eTopLeft == null)
			{
				eTopLeft = RegionBelow(regUp)._eUp._Rprev;
			}
			ActiveRegion activeRegion = regUp;
			MeshUtils.Edge edge2 = eTopLeft;
			while (true)
			{
				ActiveRegion activeRegion2 = RegionBelow(activeRegion);
				edge = activeRegion2._eUp._Sym;
				if (edge._Org != edge2._Org)
				{
					break;
				}
				if (edge._Onext != edge2)
				{
					_mesh.Splice(edge._Oprev, edge);
					_mesh.Splice(edge2._Oprev, edge);
				}
				activeRegion2._windingNumber = activeRegion._windingNumber - edge._winding;
				activeRegion2._inside = Geom.IsWindingInside(_windingRule, activeRegion2._windingNumber);
				activeRegion._dirty = true;
				if (!flag && CheckForRightSplice(activeRegion))
				{
					Geom.AddWinding(edge, edge2);
					DeleteRegion(activeRegion);
					_mesh.Delete(edge2);
				}
				flag = false;
				activeRegion = activeRegion2;
				edge2 = edge;
			}
			activeRegion._dirty = true;
			if (cleanUp)
			{
				WalkDirtyRegions(activeRegion);
			}
		}

		private void SpliceMergeVertices(MeshUtils.Edge e1, MeshUtils.Edge e2)
		{
			_mesh.Splice(e1, e2);
		}

		private void VertexWeights(MeshUtils.Vertex isect, MeshUtils.Vertex org, MeshUtils.Vertex dst, out float w0, out float w1)
		{
			float num = Geom.VertL1dist(org, isect);
			float num2 = Geom.VertL1dist(dst, isect);
			w0 = 0.5f * num2 / (num + num2);
			w1 = 0.5f * num / (num + num2);
			isect._coords.X += w0 * org._coords.X + w1 * dst._coords.X;
			isect._coords.Y += w0 * org._coords.Y + w1 * dst._coords.Y;
			isect._coords.Z += w0 * org._coords.Z + w1 * dst._coords.Z;
		}

		private void GetIntersectData(MeshUtils.Vertex isect, MeshUtils.Vertex orgUp, MeshUtils.Vertex dstUp, MeshUtils.Vertex orgLo, MeshUtils.Vertex dstLo)
		{
			isect._coords = Vec3.Zero;
			float w;
			float w2;
			VertexWeights(isect, orgUp, dstUp, out w, out w2);
			float w3;
			float w4;
			VertexWeights(isect, orgLo, dstLo, out w3, out w4);
			if (_combineCallback != null)
			{
				isect._data = _combineCallback(isect._coords, new object[4] { orgUp._data, dstUp._data, orgLo._data, dstLo._data }, new float[4] { w, w2, w3, w4 });
			}
		}

		private bool CheckForRightSplice(ActiveRegion regUp)
		{
			ActiveRegion activeRegion = RegionBelow(regUp);
			MeshUtils.Edge eUp = regUp._eUp;
			MeshUtils.Edge eUp2 = activeRegion._eUp;
			if (Geom.VertLeq(eUp._Org, eUp2._Org))
			{
				if (Geom.EdgeSign(eUp2._Dst, eUp._Org, eUp2._Org) > 0f)
				{
					return false;
				}
				if (!Geom.VertEq(eUp._Org, eUp2._Org))
				{
					_mesh.SplitEdge(eUp2._Sym);
					_mesh.Splice(eUp, eUp2._Oprev);
					regUp._dirty = (activeRegion._dirty = true);
				}
				else if (eUp._Org != eUp2._Org)
				{
					_pq.Remove(eUp._Org._pqHandle);
					SpliceMergeVertices(eUp2._Oprev, eUp);
				}
			}
			else
			{
				if (Geom.EdgeSign(eUp._Dst, eUp2._Org, eUp._Org) < 0f)
				{
					return false;
				}
				RegionAbove(regUp)._dirty = (regUp._dirty = true);
				_mesh.SplitEdge(eUp._Sym);
				_mesh.Splice(eUp2._Oprev, eUp);
			}
			return true;
		}

		private bool CheckForLeftSplice(ActiveRegion regUp)
		{
			ActiveRegion activeRegion = RegionBelow(regUp);
			MeshUtils.Edge eUp = regUp._eUp;
			MeshUtils.Edge eUp2 = activeRegion._eUp;
			if (Geom.VertLeq(eUp._Dst, eUp2._Dst))
			{
				if (Geom.EdgeSign(eUp._Dst, eUp2._Dst, eUp._Org) < 0f)
				{
					return false;
				}
				RegionAbove(regUp)._dirty = (regUp._dirty = true);
				MeshUtils.Edge edge = _mesh.SplitEdge(eUp);
				_mesh.Splice(eUp2._Sym, edge);
				edge._Lface._inside = regUp._inside;
			}
			else
			{
				if (Geom.EdgeSign(eUp2._Dst, eUp._Dst, eUp2._Org) > 0f)
				{
					return false;
				}
				regUp._dirty = (activeRegion._dirty = true);
				MeshUtils.Edge edge2 = _mesh.SplitEdge(eUp2);
				_mesh.Splice(eUp._Lnext, eUp2._Sym);
				edge2._Rface._inside = regUp._inside;
			}
			return true;
		}

		private bool CheckForIntersect(ActiveRegion regUp)
		{
			ActiveRegion activeRegion = RegionBelow(regUp);
			MeshUtils.Edge eUp = regUp._eUp;
			MeshUtils.Edge eUp2 = activeRegion._eUp;
			MeshUtils.Vertex org = eUp._Org;
			MeshUtils.Vertex org2 = eUp2._Org;
			MeshUtils.Vertex dst = eUp._Dst;
			MeshUtils.Vertex dst2 = eUp2._Dst;
			if (org == org2)
			{
				return false;
			}
			float num = Math.Min(org._t, dst._t);
			float num2 = Math.Max(org2._t, dst2._t);
			if (num > num2)
			{
				return false;
			}
			if (Geom.VertLeq(org, org2))
			{
				if (Geom.EdgeSign(dst2, org, org2) > 0f)
				{
					return false;
				}
			}
			else if (Geom.EdgeSign(dst, org2, org) < 0f)
			{
				return false;
			}
			MeshUtils.Vertex vertex = new MeshUtils.Vertex();
			Geom.EdgeIntersect(dst, org, dst2, org2, vertex);
			if (Geom.VertLeq(vertex, _event))
			{
				vertex._s = _event._s;
				vertex._t = _event._t;
			}
			MeshUtils.Vertex vertex2 = ((!Geom.VertLeq(org, org2)) ? org2 : org);
			if (Geom.VertLeq(vertex2, vertex))
			{
				vertex._s = vertex2._s;
				vertex._t = vertex2._t;
			}
			if (Geom.VertEq(vertex, org) || Geom.VertEq(vertex, org2))
			{
				CheckForRightSplice(regUp);
				return false;
			}
			if ((!Geom.VertEq(dst, _event) && Geom.EdgeSign(dst, _event, vertex) >= 0f) || (!Geom.VertEq(dst2, _event) && Geom.EdgeSign(dst2, _event, vertex) <= 0f))
			{
				if (dst2 == _event)
				{
					_mesh.SplitEdge(eUp._Sym);
					_mesh.Splice(eUp2._Sym, eUp);
					regUp = TopLeftRegion(regUp);
					eUp = RegionBelow(regUp)._eUp;
					FinishLeftRegions(RegionBelow(regUp), activeRegion);
					AddRightEdges(regUp, eUp._Oprev, eUp, eUp, true);
					return true;
				}
				if (dst == _event)
				{
					_mesh.SplitEdge(eUp2._Sym);
					_mesh.Splice(eUp._Lnext, eUp2._Oprev);
					activeRegion = regUp;
					regUp = TopRightRegion(regUp);
					MeshUtils.Edge rprev = RegionBelow(regUp)._eUp._Rprev;
					activeRegion._eUp = eUp2._Oprev;
					eUp2 = FinishLeftRegions(activeRegion, null);
					AddRightEdges(regUp, eUp2._Onext, eUp._Rprev, rprev, true);
					return true;
				}
				if (Geom.EdgeSign(dst, _event, vertex) >= 0f)
				{
					RegionAbove(regUp)._dirty = (regUp._dirty = true);
					_mesh.SplitEdge(eUp._Sym);
					eUp._Org._s = _event._s;
					eUp._Org._t = _event._t;
				}
				if (Geom.EdgeSign(dst2, _event, vertex) <= 0f)
				{
					regUp._dirty = (activeRegion._dirty = true);
					_mesh.SplitEdge(eUp2._Sym);
					eUp2._Org._s = _event._s;
					eUp2._Org._t = _event._t;
				}
				return false;
			}
			_mesh.SplitEdge(eUp._Sym);
			_mesh.SplitEdge(eUp2._Sym);
			_mesh.Splice(eUp2._Oprev, eUp);
			eUp._Org._s = vertex._s;
			eUp._Org._t = vertex._t;
			eUp._Org._pqHandle = _pq.Insert(eUp._Org);
			if (eUp._Org._pqHandle._handle == PQHandle.Invalid)
			{
				throw new InvalidOperationException("PQHandle should not be invalid");
			}
			GetIntersectData(eUp._Org, org, dst, org2, dst2);
			RegionAbove(regUp)._dirty = (regUp._dirty = (activeRegion._dirty = true));
			return false;
		}

		private void WalkDirtyRegions(ActiveRegion regUp)
		{
			ActiveRegion activeRegion = RegionBelow(regUp);
			while (true)
			{
				if (activeRegion._dirty)
				{
					regUp = activeRegion;
					activeRegion = RegionBelow(activeRegion);
					continue;
				}
				if (!regUp._dirty)
				{
					activeRegion = regUp;
					regUp = RegionAbove(regUp);
					if (regUp == null || !regUp._dirty)
					{
						break;
					}
				}
				regUp._dirty = false;
				MeshUtils.Edge eUp = regUp._eUp;
				MeshUtils.Edge eUp2 = activeRegion._eUp;
				if (eUp._Dst != eUp2._Dst && CheckForLeftSplice(regUp))
				{
					if (activeRegion._fixUpperEdge)
					{
						DeleteRegion(activeRegion);
						_mesh.Delete(eUp2);
						activeRegion = RegionBelow(regUp);
						eUp2 = activeRegion._eUp;
					}
					else if (regUp._fixUpperEdge)
					{
						DeleteRegion(regUp);
						_mesh.Delete(eUp);
						regUp = RegionAbove(activeRegion);
						eUp = regUp._eUp;
					}
				}
				if (eUp._Org != eUp2._Org)
				{
					if (eUp._Dst != eUp2._Dst && !regUp._fixUpperEdge && !activeRegion._fixUpperEdge && (eUp._Dst == _event || eUp2._Dst == _event))
					{
						if (CheckForIntersect(regUp))
						{
							break;
						}
					}
					else
					{
						CheckForRightSplice(regUp);
					}
				}
				if (eUp._Org == eUp2._Org && eUp._Dst == eUp2._Dst)
				{
					Geom.AddWinding(eUp2, eUp);
					DeleteRegion(regUp);
					_mesh.Delete(eUp);
					regUp = RegionAbove(activeRegion);
				}
			}
		}

		private void ConnectRightVertex(ActiveRegion regUp, MeshUtils.Edge eBottomLeft)
		{
			MeshUtils.Edge edge = eBottomLeft._Onext;
			ActiveRegion activeRegion = RegionBelow(regUp);
			MeshUtils.Edge eUp = regUp._eUp;
			MeshUtils.Edge eUp2 = activeRegion._eUp;
			bool flag = false;
			if (eUp._Dst != eUp2._Dst)
			{
				CheckForIntersect(regUp);
			}
			if (Geom.VertEq(eUp._Org, _event))
			{
				_mesh.Splice(edge._Oprev, eUp);
				regUp = TopLeftRegion(regUp);
				edge = RegionBelow(regUp)._eUp;
				FinishLeftRegions(RegionBelow(regUp), activeRegion);
				flag = true;
			}
			if (Geom.VertEq(eUp2._Org, _event))
			{
				_mesh.Splice(eBottomLeft, eUp2._Oprev);
				eBottomLeft = FinishLeftRegions(activeRegion, null);
				flag = true;
			}
			if (flag)
			{
				AddRightEdges(regUp, eBottomLeft._Onext, edge, edge, true);
				return;
			}
			MeshUtils.Edge eDst = ((!Geom.VertLeq(eUp2._Org, eUp._Org)) ? eUp : eUp2._Oprev);
			eDst = _mesh.Connect(eBottomLeft._Lprev, eDst);
			AddRightEdges(regUp, eDst, eDst._Onext, eDst._Onext, false);
			eDst._Sym._activeRegion._fixUpperEdge = true;
			WalkDirtyRegions(regUp);
		}

		private void ConnectLeftDegenerate(ActiveRegion regUp, MeshUtils.Vertex vEvent)
		{
			MeshUtils.Edge eUp = regUp._eUp;
			if (Geom.VertEq(eUp._Org, vEvent))
			{
				throw new InvalidOperationException("Vertices should have been merged before");
			}
			if (!Geom.VertEq(eUp._Dst, vEvent))
			{
				_mesh.SplitEdge(eUp._Sym);
				if (regUp._fixUpperEdge)
				{
					_mesh.Delete(eUp._Onext);
					regUp._fixUpperEdge = false;
				}
				_mesh.Splice(vEvent._anEdge, eUp);
				SweepEvent(vEvent);
				return;
			}
			throw new InvalidOperationException("Vertices should have been merged before");
		}

		private void ConnectLeftVertex(MeshUtils.Vertex vEvent)
		{
			ActiveRegion activeRegion = new ActiveRegion();
			activeRegion._eUp = vEvent._anEdge._Sym;
			ActiveRegion key = _dict.Find(activeRegion).Key;
			ActiveRegion activeRegion2 = RegionBelow(key);
			if (activeRegion2 == null)
			{
				return;
			}
			MeshUtils.Edge eUp = key._eUp;
			MeshUtils.Edge eUp2 = activeRegion2._eUp;
			if (Geom.EdgeSign(eUp._Dst, vEvent, eUp._Org) == 0f)
			{
				ConnectLeftDegenerate(key, vEvent);
				return;
			}
			ActiveRegion activeRegion3 = ((!Geom.VertLeq(eUp2._Dst, eUp._Dst)) ? activeRegion2 : key);
			if (key._inside || activeRegion3._fixUpperEdge)
			{
				MeshUtils.Edge edge = ((activeRegion3 != key) ? _mesh.Connect(eUp2._Dnext, vEvent._anEdge)._Sym : _mesh.Connect(vEvent._anEdge._Sym, eUp._Lnext));
				if (activeRegion3._fixUpperEdge)
				{
					FixUpperEdge(activeRegion3, edge);
				}
				else
				{
					ComputeWinding(AddRegionBelow(key, edge));
				}
				SweepEvent(vEvent);
			}
			else
			{
				AddRightEdges(key, vEvent._anEdge, vEvent._anEdge, null, true);
			}
		}

		private void SweepEvent(MeshUtils.Vertex vEvent)
		{
			_event = vEvent;
			MeshUtils.Edge edge = vEvent._anEdge;
			while (edge._activeRegion == null)
			{
				edge = edge._Onext;
				if (edge == vEvent._anEdge)
				{
					ConnectLeftVertex(vEvent);
					return;
				}
			}
			ActiveRegion activeRegion = TopLeftRegion(edge._activeRegion);
			ActiveRegion activeRegion2 = RegionBelow(activeRegion);
			MeshUtils.Edge eUp = activeRegion2._eUp;
			MeshUtils.Edge edge2 = FinishLeftRegions(activeRegion2, null);
			if (edge2._Onext == eUp)
			{
				ConnectRightVertex(activeRegion, edge2);
			}
			else
			{
				AddRightEdges(activeRegion, edge2._Onext, eUp, eUp, true);
			}
		}

		private void AddSentinel(float smin, float smax, float t)
		{
			MeshUtils.Edge edge = _mesh.MakeEdge();
			edge._Org._s = smax;
			edge._Org._t = t;
			edge._Dst._s = smin;
			edge._Dst._t = t;
			_event = edge._Dst;
			ActiveRegion activeRegion = new ActiveRegion();
			activeRegion._eUp = edge;
			activeRegion._windingNumber = 0;
			activeRegion._inside = false;
			activeRegion._fixUpperEdge = false;
			activeRegion._sentinel = true;
			activeRegion._dirty = false;
			activeRegion._nodeUp = _dict.Insert(activeRegion);
		}

		private void InitEdgeDict()
		{
			_dict = new Dict<ActiveRegion>(EdgeLeq);
			float num = _bmaxX - _bminX;
			float num2 = _bmaxY - _bminY;
			float smin = _bminX - num;
			float smax = _bmaxX + num;
			float t = _bminY - num2;
			float t2 = _bmaxY + num2;
			AddSentinel(smin, smax, t);
			AddSentinel(smin, smax, t2);
		}

		private void DoneEdgeDict()
		{
			int num = 0;
			ActiveRegion key;
			while ((key = _dict.Min().Key) != null)
			{
				if (!key._sentinel)
				{
				}
				DeleteRegion(key);
			}
			_dict = null;
		}

		private void RemoveDegenerateEdges()
		{
			MeshUtils.Edge eHead = _mesh._eHead;
			MeshUtils.Edge edge = eHead._next;
			while (edge != eHead)
			{
				MeshUtils.Edge next = edge._next;
				MeshUtils.Edge lnext = edge._Lnext;
				if (Geom.VertEq(edge._Org, edge._Dst) && edge._Lnext._Lnext != edge)
				{
					SpliceMergeVertices(lnext, edge);
					_mesh.Delete(edge);
					edge = lnext;
					lnext = edge._Lnext;
				}
				if (lnext._Lnext == edge)
				{
					if (lnext != edge)
					{
						if (lnext == next || lnext == next._Sym)
						{
							next = next._next;
						}
						_mesh.Delete(lnext);
					}
					if (edge == next || edge == next._Sym)
					{
						next = next._next;
					}
					_mesh.Delete(edge);
				}
				edge = next;
			}
		}

		private void InitPriorityQ()
		{
			MeshUtils.Vertex vHead = _mesh._vHead;
			int num = 0;
			for (MeshUtils.Vertex next = vHead._next; next != vHead; next = next._next)
			{
				num++;
			}
			num += 8;
			_pq = new PriorityQueue<MeshUtils.Vertex>(num, Geom.VertLeq);
			vHead = _mesh._vHead;
			for (MeshUtils.Vertex next = vHead._next; next != vHead; next = next._next)
			{
				next._pqHandle = _pq.Insert(next);
				if (next._pqHandle._handle == PQHandle.Invalid)
				{
					throw new InvalidOperationException("PQHandle should not be invalid");
				}
			}
			_pq.Init();
		}

		private void DonePriorityQ()
		{
			_pq = null;
		}

		private void RemoveDegenerateFaces()
		{
			MeshUtils.Face face = _mesh._fHead._next;
			while (face != _mesh._fHead)
			{
				MeshUtils.Face next = face._next;
				MeshUtils.Edge anEdge = face._anEdge;
				if (anEdge._Lnext._Lnext == anEdge)
				{
					Geom.AddWinding(anEdge._Onext, anEdge);
					_mesh.Delete(anEdge);
				}
				face = next;
			}
		}

		protected void ComputeInterior()
		{
			RemoveDegenerateEdges();
			InitPriorityQ();
			RemoveDegenerateFaces();
			InitEdgeDict();
			MeshUtils.Vertex vertex;
			while ((vertex = _pq.ExtractMin()) != null)
			{
				while (true)
				{
					MeshUtils.Vertex vertex2 = _pq.Minimum();
					if (vertex2 == null || !Geom.VertEq(vertex2, vertex))
					{
						break;
					}
					vertex2 = _pq.ExtractMin();
					SpliceMergeVertices(vertex._anEdge, vertex2._anEdge);
				}
				SweepEvent(vertex);
			}
			DoneEdgeDict();
			DonePriorityQ();
			RemoveDegenerateFaces();
			_mesh.Check();
		}

		private void ComputeNormal(ref Vec3 norm)
		{
			MeshUtils.Vertex next = _mesh._vHead._next;
			float[] array = new float[3]
			{
				next._coords.X,
				next._coords.Y,
				next._coords.Z
			};
			MeshUtils.Vertex[] array2 = new MeshUtils.Vertex[3] { next, next, next };
			float[] array3 = new float[3]
			{
				next._coords.X,
				next._coords.Y,
				next._coords.Z
			};
			MeshUtils.Vertex[] array4 = new MeshUtils.Vertex[3] { next, next, next };
			while (next != _mesh._vHead)
			{
				if (next._coords.X < array[0])
				{
					array[0] = next._coords.X;
					array2[0] = next;
				}
				if (next._coords.Y < array[1])
				{
					array[1] = next._coords.Y;
					array2[1] = next;
				}
				if (next._coords.Z < array[2])
				{
					array[2] = next._coords.Z;
					array2[2] = next;
				}
				if (next._coords.X > array3[0])
				{
					array3[0] = next._coords.X;
					array4[0] = next;
				}
				if (next._coords.Y > array3[1])
				{
					array3[1] = next._coords.Y;
					array4[1] = next;
				}
				if (next._coords.Z > array3[2])
				{
					array3[2] = next._coords.Z;
					array4[2] = next;
				}
				next = next._next;
			}
			int num = 0;
			if (array3[1] - array[1] > array3[0] - array[0])
			{
				num = 1;
			}
			if (array3[2] - array[2] > array3[num] - array[num])
			{
				num = 2;
			}
			if (array[num] >= array3[num])
			{
				norm = default(Vec3);
				Vec3 vec = norm;
				vec.Z = 1f;
				norm = vec;
				return;
			}
			float num2 = 0f;
			MeshUtils.Vertex vertex = array2[num];
			MeshUtils.Vertex vertex2 = array4[num];
			Vec3 result;
			Vec3.Sub(ref vertex._coords, ref vertex2._coords, out result);
			Vec3 vec2 = default(Vec3);
			for (next = _mesh._vHead._next; next != _mesh._vHead; next = next._next)
			{
				Vec3 result2;
				Vec3.Sub(ref next._coords, ref vertex2._coords, out result2);
				vec2.X = result.Y * result2.Z - result.Z * result2.Y;
				vec2.Y = result.Z * result2.X - result.X * result2.Z;
				vec2.Z = result.X * result2.Y - result.Y * result2.X;
				float num3 = vec2.X * vec2.X + vec2.Y * vec2.Y + vec2.Z * vec2.Z;
				if (num3 > num2)
				{
					num2 = num3;
					norm = vec2;
				}
			}
			if (num2 <= 0f)
			{
				norm = Vec3.Zero;
				num = Vec3.LongAxis(ref result);
				norm[num] = 1f;
			}
		}

		private void CheckOrientation()
		{
			float num = 0f;
			for (MeshUtils.Face next = _mesh._fHead._next; next != _mesh._fHead; next = next._next)
			{
				MeshUtils.Edge edge = next._anEdge;
				if (edge._winding > 0)
				{
					do
					{
						num += (edge._Org._s - edge._Dst._s) * (edge._Org._t + edge._Dst._t);
						edge = edge._Lnext;
					}
					while (edge != next._anEdge);
				}
			}
			if (num < 0f)
			{
				for (MeshUtils.Vertex next2 = _mesh._vHead._next; next2 != _mesh._vHead; next2 = next2._next)
				{
					next2._t = 0f - next2._t;
				}
				Vec3.Neg(ref _tUnit);
			}
		}

		private void ProjectPolygon()
		{
			Vec3 norm = _normal;
			bool flag = false;
			if (norm.X == 0f && norm.Y == 0f && norm.Z == 0f)
			{
				ComputeNormal(ref norm);
				flag = true;
			}
			int num = Vec3.LongAxis(ref norm);
			_sUnit[num] = 0f;
			_sUnit[(num + 1) % 3] = SUnitX;
			_sUnit[(num + 2) % 3] = SUnitY;
			_tUnit[num] = 0f;
			_tUnit[(num + 1) % 3] = ((!(norm[num] > 0f)) ? SUnitY : (0f - SUnitY));
			_tUnit[(num + 2) % 3] = ((!(norm[num] > 0f)) ? (0f - SUnitX) : SUnitX);
			for (MeshUtils.Vertex next = _mesh._vHead._next; next != _mesh._vHead; next = next._next)
			{
				Vec3.Dot(ref next._coords, ref _sUnit, out next._s);
				Vec3.Dot(ref next._coords, ref _tUnit, out next._t);
			}
			if (flag)
			{
				CheckOrientation();
			}
			bool flag2 = true;
			for (MeshUtils.Vertex next2 = _mesh._vHead._next; next2 != _mesh._vHead; next2 = next2._next)
			{
				if (flag2)
				{
					_bminX = (_bmaxX = next2._s);
					_bminY = (_bmaxY = next2._t);
					flag2 = false;
				}
				else
				{
					if (next2._s < _bminX)
					{
						_bminX = next2._s;
					}
					if (next2._s > _bmaxX)
					{
						_bmaxX = next2._s;
					}
					if (next2._t < _bminY)
					{
						_bminY = next2._t;
					}
					if (next2._t > _bmaxY)
					{
						_bmaxY = next2._t;
					}
				}
			}
		}

		private void TessellateMonoRegion(MeshUtils.Face face)
		{
			MeshUtils.Edge edge = face._anEdge;
			while (Geom.VertLeq(edge._Dst, edge._Org))
			{
				edge = edge._Lprev;
			}
			while (Geom.VertLeq(edge._Org, edge._Dst))
			{
				edge = edge._Lnext;
			}
			MeshUtils.Edge edge2 = edge._Lprev;
			while (edge._Lnext != edge2)
			{
				if (Geom.VertLeq(edge._Dst, edge2._Org))
				{
					while (edge2._Lnext != edge && (Geom.EdgeGoesLeft(edge2._Lnext) || Geom.EdgeSign(edge2._Org, edge2._Dst, edge2._Lnext._Dst) <= 0f))
					{
						edge2 = _mesh.Connect(edge2._Lnext, edge2)._Sym;
					}
					edge2 = edge2._Lprev;
				}
				else
				{
					while (edge2._Lnext != edge && (Geom.EdgeGoesRight(edge._Lprev) || Geom.EdgeSign(edge._Dst, edge._Org, edge._Lprev._Org) >= 0f))
					{
						edge = _mesh.Connect(edge, edge._Lprev)._Sym;
					}
					edge = edge._Lnext;
				}
			}
			while (edge2._Lnext._Lnext != edge)
			{
				edge2 = _mesh.Connect(edge2._Lnext, edge2)._Sym;
			}
		}

		private void TessellateInterior()
		{
			MeshUtils.Face face = _mesh._fHead._next;
			while (face != _mesh._fHead)
			{
				MeshUtils.Face next = face._next;
				if (face._inside)
				{
					TessellateMonoRegion(face);
				}
				face = next;
			}
		}

		private void DiscardExterior()
		{
			MeshUtils.Face face = _mesh._fHead._next;
			while (face != _mesh._fHead)
			{
				MeshUtils.Face next = face._next;
				if (!face._inside)
				{
					_mesh.ZapFace(face);
				}
				face = next;
			}
		}

		private void SetWindingNumber(int value, bool keepOnlyBoundary)
		{
			MeshUtils.Edge edge = _mesh._eHead._next;
			while (edge != _mesh._eHead)
			{
				MeshUtils.Edge next = edge._next;
				if (edge._Rface._inside != edge._Lface._inside)
				{
					edge._winding = ((!edge._Lface._inside) ? (-value) : value);
				}
				else if (!keepOnlyBoundary)
				{
					edge._winding = 0;
				}
				else
				{
					_mesh.Delete(edge);
				}
				edge = next;
			}
		}

		private int GetNeighbourFace(MeshUtils.Edge edge)
		{
			if (edge._Rface == null)
			{
				return -1;
			}
			if (!edge._Rface._inside)
			{
				return -1;
			}
			return edge._Rface._n;
		}

		private void OutputPolymesh(ElementType elementType, int polySize)
		{
			int num = 0;
			int num2 = 0;
			if (polySize < 3)
			{
				polySize = 3;
			}
			if (polySize > 3)
			{
				_mesh.MergeConvexFaces(polySize);
			}
			for (MeshUtils.Vertex next = _mesh._vHead._next; next != _mesh._vHead; next = next._next)
			{
				next._n = -1;
			}
			for (MeshUtils.Face next2 = _mesh._fHead._next; next2 != _mesh._fHead; next2 = next2._next)
			{
				next2._n = -1;
				if (next2._inside)
				{
					MeshUtils.Edge edge = next2._anEdge;
					int num3 = 0;
					do
					{
						MeshUtils.Vertex next = edge._Org;
						if (next._n == -1)
						{
							next._n = num2;
							num2++;
						}
						num3++;
						edge = edge._Lnext;
					}
					while (edge != next2._anEdge);
					next2._n = num;
					num++;
				}
			}
			_elementCount = num;
			if (elementType == ElementType.ConnectedPolygons)
			{
				num *= 2;
			}
			_elements = new int[num * polySize];
			_vertexCount = num2;
			_vertices = new ContourVertex[_vertexCount];
			for (MeshUtils.Vertex next = _mesh._vHead._next; next != _mesh._vHead; next = next._next)
			{
				if (next._n != -1)
				{
					int n = next._n;
					_vertices[next._n].Position = next._coords;
					_vertices[next._n].Data = next._data;
				}
			}
			int num4 = 0;
			for (MeshUtils.Face next2 = _mesh._fHead._next; next2 != _mesh._fHead; next2 = next2._next)
			{
				if (next2._inside)
				{
					MeshUtils.Edge edge = next2._anEdge;
					int num3 = 0;
					do
					{
						MeshUtils.Vertex next = edge._Org;
						_elements[num4++] = next._n;
						num3++;
						edge = edge._Lnext;
					}
					while (edge != next2._anEdge);
					for (int i = num3; i < polySize; i++)
					{
						_elements[num4++] = -1;
					}
					if (elementType == ElementType.ConnectedPolygons)
					{
						edge = next2._anEdge;
						do
						{
							_elements[num4++] = GetNeighbourFace(edge);
							edge = edge._Lnext;
						}
						while (edge != next2._anEdge);
						for (int i = num3; i < polySize; i++)
						{
							_elements[num4++] = -1;
						}
					}
				}
			}
		}

		private void OutputContours()
		{
			int num = 0;
			int num2 = 0;
			_vertexCount = 0;
			_elementCount = 0;
			for (MeshUtils.Face next = _mesh._fHead._next; next != _mesh._fHead; next = next._next)
			{
				if (next._inside)
				{
					MeshUtils.Edge edge2;
					MeshUtils.Edge edge = (edge2 = next._anEdge);
					do
					{
						_vertexCount++;
						edge2 = edge2._Lnext;
					}
					while (edge2 != edge);
					_elementCount++;
				}
			}
			_elements = new int[_elementCount * 2];
			_vertices = new ContourVertex[_vertexCount];
			int num3 = 0;
			int num4 = 0;
			num = 0;
			for (MeshUtils.Face next = _mesh._fHead._next; next != _mesh._fHead; next = next._next)
			{
				if (next._inside)
				{
					num2 = 0;
					MeshUtils.Edge edge2;
					MeshUtils.Edge edge = (edge2 = next._anEdge);
					do
					{
						_vertices[num3++].Position = edge2._Org._coords;
						_vertices[num3++].Data = edge2._Org._data;
						num2++;
						edge2 = edge2._Lnext;
					}
					while (edge2 != edge);
					_elements[num4++] = num;
					_elements[num4++] = num2;
					num += num2;
				}
			}
		}

		private float SignedArea(ContourVertex[] vertices)
		{
			float num = 0f;
			for (int i = 0; i < vertices.Length; i++)
			{
				ContourVertex contourVertex = vertices[i];
				ContourVertex contourVertex2 = vertices[(i + 1) % vertices.Length];
				num += contourVertex.Position.X * contourVertex2.Position.Y;
				num -= contourVertex.Position.Y * contourVertex2.Position.X;
			}
			return num * 0.5f;
		}

		public void AddContour(ContourVertex[] vertices)
		{
			AddContour(vertices, ContourOrientation.Original);
		}

		public void AddContour(ContourVertex[] vertices, ContourOrientation forceOrientation)
		{
			if (_mesh == null)
			{
				_mesh = new Mesh();
			}
			bool flag = false;
			if (forceOrientation != ContourOrientation.Original)
			{
				float num = SignedArea(vertices);
				flag = (forceOrientation == ContourOrientation.Clockwise && num < 0f) || (forceOrientation == ContourOrientation.CounterClockwise && num > 0f);
			}
			MeshUtils.Edge edge = null;
			for (int i = 0; i < vertices.Length; i++)
			{
				if (edge == null)
				{
					edge = _mesh.MakeEdge();
					_mesh.Splice(edge, edge._Sym);
				}
				else
				{
					_mesh.SplitEdge(edge);
					edge = edge._Lnext;
				}
				int num2 = ((!flag) ? i : (vertices.Length - 1 - i));
				edge._Org._coords = vertices[num2].Position;
				edge._Org._data = vertices[num2].Data;
				edge._winding = 1;
				edge._Sym._winding = -1;
			}
		}

		public void Tessellate(WindingRule windingRule, ElementType elementType, int polySize)
		{
			Tessellate(windingRule, elementType, polySize, null);
		}

		public void Tessellate(WindingRule windingRule, ElementType elementType, int polySize, CombineCallback combineCallback)
		{
			_vertices = null;
			_elements = null;
			_windingRule = windingRule;
			_combineCallback = combineCallback;
			if (_mesh != null)
			{
				ProjectPolygon();
				ComputeInterior();
				if (elementType == ElementType.BoundaryContours)
				{
					SetWindingNumber(1, true);
				}
				else
				{
					TessellateInterior();
				}
				_mesh.Check();
				if (elementType == ElementType.BoundaryContours)
				{
					OutputContours();
				}
				else
				{
					OutputPolymesh(elementType, polySize);
				}
				_mesh = null;
			}
		}
	}
}
