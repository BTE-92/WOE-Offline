using UnityEngine;

public class SpriteSortingTestState : BasicState
{
	public SpriteSheet m_spriteSheet;

	private int ticker;

	private int dir;

	private int spriteNum;

	public SpriteC createSprite(Vector2 pos, Color col)
	{
		float num = 10f;
		Entity entity = EntityManager.AddEntity("spriteEnt");
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
		dir = 1;
		spriteNum = 0;
		TextS.ChangeText(FrameworkTestScene.m_titleTXC, "Sorting, Color & visibility");
	}

	public override void Execute()
	{
		ticker++;
		if (ticker % 2 == 0)
		{
			Vector2 pos = new Vector2(-100 + spriteNum * 4 - spriteNum / 50 * 50 * 4, 30 + spriteNum - spriteNum / 50 * 60);
			SpriteC s = createSprite(pos, new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0.5f, 1f)));
			SpriteS.SetSortValue(s, dir * spriteNum);
			SpriteS.SetVisibility(s, dir > 0);
			spriteNum++;
		}
		if (ticker % 60 == 0)
		{
			dir = -dir;
			for (int i = 0; i < m_spriteSheet.m_components.m_aliveCount; i++)
			{
				int num = m_spriteSheet.m_components.m_aliveIndices[i];
				SpriteC s2 = m_spriteSheet.m_components.m_array[num];
				SpriteS.SetSortValue(s2, dir * i);
			}
		}
	}

	public override void Exit()
	{
		EntityManager.RemoveAllEntities();
	}
}
