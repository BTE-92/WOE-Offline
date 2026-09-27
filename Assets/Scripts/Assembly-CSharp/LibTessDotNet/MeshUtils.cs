namespace LibTessDotNet
{
	internal static class MeshUtils
	{
		public class Vertex
		{
			internal Vertex _prev;

			internal Vertex _next;

			internal Edge _anEdge;

			internal Vec3 _coords;

			internal float _s;

			internal float _t;

			internal PQHandle _pqHandle;

			internal int _n;

			internal object _data;
		}

		public class Face
		{
			internal Face _prev;

			internal Face _next;

			internal Edge _anEdge;

			internal Face _trail;

			internal int _n;

			internal bool _marked;

			internal bool _inside;

			internal int VertsCount
			{
				get
				{
					int num = 0;
					Edge edge = _anEdge;
					do
					{
						num++;
						edge = edge._Lnext;
					}
					while (edge != _anEdge);
					return num;
				}
			}
		}

		public struct EdgePair
		{
			internal Edge _e;

			internal Edge _eSym;

			public static EdgePair Create()
			{
				EdgePair edgePair = default(EdgePair);
				edgePair._e = new Edge();
				edgePair._e._pair = edgePair;
				edgePair._eSym = new Edge();
				edgePair._eSym._pair = edgePair;
				return edgePair;
			}
		}

		public class Edge
		{
			internal EdgePair _pair;

			internal Edge _next;

			internal Edge _Sym;

			internal Edge _Onext;

			internal Edge _Lnext;

			internal Vertex _Org;

			internal Face _Lface;

			internal Tess.ActiveRegion _activeRegion;

			internal int _winding;

			internal Face _Rface
			{
				get
				{
					return _Sym._Lface;
				}
				set
				{
					_Sym._Lface = value;
				}
			}

			internal Vertex _Dst
			{
				get
				{
					return _Sym._Org;
				}
				set
				{
					_Sym._Org = value;
				}
			}

			internal Edge _Oprev
			{
				get
				{
					return _Sym._Lnext;
				}
				set
				{
					_Sym._Lnext = value;
				}
			}

			internal Edge _Lprev
			{
				get
				{
					return _Onext._Sym;
				}
				set
				{
					_Onext._Sym = value;
				}
			}

			internal Edge _Dprev
			{
				get
				{
					return _Lnext._Sym;
				}
				set
				{
					_Lnext._Sym = value;
				}
			}

			internal Edge _Rprev
			{
				get
				{
					return _Sym._Onext;
				}
				set
				{
					_Sym._Onext = value;
				}
			}

			internal Edge _Dnext
			{
				get
				{
					return _Rprev._Sym;
				}
				set
				{
					_Rprev._Sym = value;
				}
			}

			internal Edge _Rnext
			{
				get
				{
					return _Oprev._Sym;
				}
				set
				{
					_Oprev._Sym = value;
				}
			}

			internal Edge()
			{
			}

			internal static void EnsureFirst(ref Edge e)
			{
				if (e == e._pair._eSym)
				{
					e = e._Sym;
				}
			}
		}

		public const int Undef = -1;

		public static Edge MakeEdge(Edge eNext)
		{
			EdgePair edgePair = EdgePair.Create();
			Edge e = edgePair._e;
			Edge eSym = edgePair._eSym;
			Edge.EnsureFirst(ref eNext);
			(eSym._next = eNext._Sym._next)._Sym._next = e;
			e._next = eNext;
			eNext._Sym._next = eSym;
			e._Sym = eSym;
			e._Onext = e;
			e._Lnext = eSym;
			e._Org = null;
			e._Lface = null;
			e._winding = 0;
			e._activeRegion = null;
			eSym._Sym = e;
			eSym._Onext = eSym;
			eSym._Lnext = e;
			eSym._Org = null;
			eSym._Lface = null;
			eSym._winding = 0;
			eSym._activeRegion = null;
			return e;
		}

		public static void Splice(Edge a, Edge b)
		{
			Edge onext = a._Onext;
			Edge onext2 = b._Onext;
			onext._Sym._Lnext = b;
			onext2._Sym._Lnext = a;
			a._Onext = onext2;
			b._Onext = onext;
		}

		public static void MakeVertex(Vertex vNew, Edge eOrig, Vertex vNext)
		{
			(vNew._prev = vNext._prev)._next = vNew;
			vNew._next = vNext;
			vNext._prev = vNew;
			vNew._anEdge = eOrig;
			Edge edge = eOrig;
			do
			{
				edge._Org = vNew;
				edge = edge._Onext;
			}
			while (edge != eOrig);
		}

		public static void MakeFace(Face fNew, Edge eOrig, Face fNext)
		{
			(fNew._prev = fNext._prev)._next = fNew;
			fNew._next = fNext;
			fNext._prev = fNew;
			fNew._anEdge = eOrig;
			fNew._trail = null;
			fNew._marked = false;
			fNew._inside = fNext._inside;
			Edge edge = eOrig;
			do
			{
				edge._Lface = fNew;
				edge = edge._Lnext;
			}
			while (edge != eOrig);
		}

		public static void KillEdge(Edge eDel)
		{
			Edge.EnsureFirst(ref eDel);
			Edge next = eDel._next;
			Edge next2 = eDel._Sym._next;
			next._Sym._next = next2;
			next2._Sym._next = next;
		}

		public static void KillVertex(Vertex vDel, Vertex newOrg)
		{
			Edge anEdge = vDel._anEdge;
			Edge edge = anEdge;
			do
			{
				edge._Org = newOrg;
				edge = edge._Onext;
			}
			while (edge != anEdge);
			Vertex prev = vDel._prev;
			Vertex next = vDel._next;
			next._prev = prev;
			prev._next = next;
		}

		public static void KillFace(Face fDel, Face newLFace)
		{
			Edge anEdge = fDel._anEdge;
			Edge edge = anEdge;
			do
			{
				edge._Lface = newLFace;
				edge = edge._Lnext;
			}
			while (edge != anEdge);
			Face prev = fDel._prev;
			Face next = fDel._next;
			next._prev = prev;
			prev._next = next;
		}
	}
}
