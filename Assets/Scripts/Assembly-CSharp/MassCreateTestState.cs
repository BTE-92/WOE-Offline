using UnityEngine;

public class MassCreateTestState : BasicState
{
	public SpriteSheet m_spriteSheet;

	private int ticker;

	public SpriteC createSprite(Vector2 pos, Color col, string tag)
	{
		float num = 10f;
		Entity entity = EntityManager.AddEntity(tag);
		TransformC transformC = TransformS.AddComponent(entity, "sprite");
		SpriteC spriteC = SpriteS.AddComponent(transformC, new Frame(0f, 0f, 5f, 5f), m_spriteSheet);
		SpriteS.SetDimensions(spriteC, num * 1f, num * 1f);
		SpriteS.SetColor(spriteC, col);
		TransformS.SetPosition(transformC, pos);
		return spriteC;
	}

	public override void Enter(IStatedObject _parent)
	{
		m_spriteSheet = FrameworkTestScene.m_spriteSheet;
		ticker = 0;
		TextS.ChangeText(FrameworkTestScene.m_titleTXC, "Mass create & destroy");
	}

	public override void Execute()
	{
		ticker++;
		if (ticker % 2 == 0)
		{
			for (int i = 0; i < 100; i++)
			{
				Vector2 pos = new Vector2(Random.Range(-50, 50), Random.Range(-50, 50));
				createSprite(pos, new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0.5f, 1f)), "SmallStack");
			}
		}
		else if (ticker % 2 == 1)
		{
			EntityManager.RemoveEntitiesByTag("SmallStack");
		}
		if (ticker % 200 == 0)
		{
			for (int j = 0; j < 500; j++)
			{
				Vector2 pos2 = new Vector2(Random.Range(-200, 200), Random.Range(-200, 200));
				createSprite(pos2, new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0.5f, 1f)), "BigStack");
			}
		}
		else if (ticker % 200 == 100)
		{
			EntityManager.RemoveEntitiesByTag("BigStack");
		}
	}

	public override void Exit()
	{
		EntityManager.RemoveAllEntities();
	}
}
