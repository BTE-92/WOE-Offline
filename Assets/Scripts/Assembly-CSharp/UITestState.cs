public class UITestState : BasicState
{
	public static int m_healthType = 1;

	public static string[] m_healthLabels = new string[3] { "No Health", "Hearths", "Health Bar" };

	public static string[] m_healthShortLabels = new string[3] { "None", "Hearths", "Bar" };

	public static Frame[] m_healthFrameLabels = new Frame[3]
	{
		new Frame(0f, 0f, 64f, 64f),
		new Frame(64f, 0f, 64f, 64f),
		new Frame(128f, 0f, 64f, 64f)
	};

	public static int m_healthAmount = 3;

	public static float m_speed = 0.5f;

	public static int m_controller = 0;

	public static string[] m_controllerLabels = new string[6] { "Player 1", "Player 2", "Player3", "Player4", "Computer", "Artificial Intelligence" };

	public static string[] m_controllerLabelsShort = new string[6] { "P1", "P2", "P3", "P4", "CPU", "AI" };

	public static bool m_CPU_hostile = true;

	public static string[] m_CPU_hostileLabels = new string[2] { "No", "Yes" };

	public static float m_CPU_skill = 0.35f;

	public static int m_PLAYER_controlMethod = 0;

	public static string[] m_controlMethodLabels = new string[5] { "Tilt", "1-Button", "2-Button", "Joystick & 1-Button", "Joystick & 2-Button" };

	public static string[] m_controlMethodLabelsShort = new string[5] { "Tilt", "1B", "2B", "J+1B", "J+2B" };

	public UIModel m_model;

	private SpriteSheet m_symbolSheet;

	public override void Enter(IStatedObject _parent)
	{
		TextS.ChangeText(FrameworkTestScene.m_titleTXC, "UI Components");
		m_model = new UIModel(this);
		UIManager.m_canvas = new UICanvas(null, "BaseCanvas", null, string.Empty);
		UIManager.m_canvas.SetMargins(0.01f);
		UIPanel uIPanel = new UIPanel(UIManager.m_canvas, "PropertyPanel", new UIComponent(null, false, string.Empty, null, null, string.Empty), new UIComponent(null, false, string.Empty, null, null, string.Empty));
		uIPanel.SetHeight(1f, RelativeTo.ParentHeight);
		uIPanel.SetWidth(0.382f, RelativeTo.ParentHeight);
		uIPanel.SetHorizontalAlign(1f);
		Debug.Log("propertyPanel " + uIPanel.m_TC.m_index);
		Debug.Log("propertyPanelContainer " + uIPanel.m_container.m_TC.m_index);
		m_symbolSheet = SpriteS.AddSpriteSheet(uIPanel.m_camera, ResourceManager.GetMaterial("Framework/SymbolMat"), 1f);
		UIHorizontalList uIHorizontalList = new UIHorizontalList(uIPanel, "HorizontalArea");
		Debug.Log("horizontal " + uIHorizontalList.m_TC.m_index);
		uIHorizontalList.SetHorizontalAlign(0f);
		uIHorizontalList.SetVerticalAlign(1f);
		UIVerticalList uIVerticalList = new UIVerticalList(uIHorizontalList, "VerticalArea");
		uIVerticalList.SetVerticalAlign(1f);
		uIVerticalList.SetMargins(0f, 0f, 0.04f, 0.04f, RelativeTo.ParentShortest);
		uIVerticalList.SetSpacing(0.04f, RelativeTo.ParentShortest);
		UIVerticalList parent = new UIVerticalList(uIVerticalList, "HealthArea");
		new UIListPropertyButton(parent, uIVerticalList, uIPanel, "Health", "Health (labels)", m_model, "m_healthType", m_healthLabels, m_healthShortLabels);
		new UIListPropertyButton(parent, uIVerticalList, uIPanel, "Health2", "Health (frames)", m_model, "m_healthType", m_healthLabels, m_symbolSheet, m_healthFrameLabels);
		new UIIntPropertyButton(parent, uIVerticalList, uIPanel, "HealthAmount", "Health Amount", m_model, "m_healthAmount", 1, 1000);
		parent = new UIVerticalList(uIVerticalList, "SpeedArea");
		new UIPercentagePropertyButton(parent, uIVerticalList, uIPanel, "Speed", "Speed percentage", m_model, "m_speed", -2f, 5f);
		new UIFloatPropertyButton(parent, uIVerticalList, uIPanel, "Speed2", "Speed float", m_model, "m_speed", -2f, 5f);
		new UIIntPropertyButton(parent, uIVerticalList, uIPanel, "Speed3", "Speed int", m_model, "m_speed", -2, 5);
		parent = new UIVerticalList(uIVerticalList, "PlayerArea");
		new UIListPropertyButton(parent, uIVerticalList, uIPanel, "Player", "Player", m_model, "m_controller", m_controllerLabels, m_controllerLabelsShort);
		new UIBoolPropertyButton(parent, uIVerticalList, uIPanel, "Hostile", "Hostile", m_model, "m_CPU_hostile", m_CPU_hostileLabels);
		new UIListPropertyButton(parent, uIVerticalList, uIPanel, "ControlMethod", "Control method", m_model, "m_PLAYER_controlMethod", m_controlMethodLabels, m_controlMethodLabelsShort);
		UIManager.m_canvas.Update();
	}

	public override void Execute()
	{
	}

	public override void Exit()
	{
		UIManager.DestroyUI();
		SpriteS.RemoveSpriteSheet(m_symbolSheet);
		m_symbolSheet = null;
		UIManager.m_canvas = null;
	}
}
