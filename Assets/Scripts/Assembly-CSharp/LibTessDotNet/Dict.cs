namespace LibTessDotNet
{
	public class Dict<TValue> where TValue : class
	{
		public class Node
		{
			internal TValue _key;

			internal Node _prev;

			internal Node _next;

			public TValue Key
			{
				get
				{
					return _key;
				}
			}

			public Node Prev
			{
				get
				{
					return _prev;
				}
			}

			public Node Next
			{
				get
				{
					return _next;
				}
			}
		}

		public delegate bool LessOrEqual(TValue lhs, TValue rhs);

		private LessOrEqual _leq;

		private Node _head;

		public Dict(LessOrEqual leq)
		{
			_leq = leq;
			_head = new Node();
			_head._prev = _head;
			_head._next = _head;
		}

		public Node Insert(TValue key)
		{
			return InsertBefore(_head, key);
		}

		public Node InsertBefore(Node node, TValue key)
		{
			do
			{
				node = node._prev;
			}
			while (node._key != null && !_leq(node._key, key));
			Node node2 = new Node();
			node2._key = key;
			Node node3 = node2;
			node3._next = node._next;
			node._next._prev = node3;
			node3._prev = node;
			node._next = node3;
			return node3;
		}

		public Node Find(TValue key)
		{
			Node node = _head;
			do
			{
				node = node._next;
			}
			while (node._key != null && !_leq(key, node._key));
			return node;
		}

		public Node Min()
		{
			return _head._next;
		}

		public void Remove(Node node)
		{
			node._next._prev = node._prev;
			node._prev._next = node._next;
		}
	}
}
