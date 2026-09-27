using System.Collections.Generic;

public interface IAssembledClass
{
	GraphElement m_graphElement { get; set; }

	List<Entity> m_assembledEntities { get; set; }

	void Destroy();
}
