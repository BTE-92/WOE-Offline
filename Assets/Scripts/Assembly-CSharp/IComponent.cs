using System.Runtime.Serialization;

public interface IComponent : IPoolable, ISerializable
{
	bool m_active { get; set; }

	bool m_wasActive { get; set; }

	int m_identifier { get; set; }

	Entity p_entity { get; set; }

	ComponentType m_componentType { get; set; }
}
