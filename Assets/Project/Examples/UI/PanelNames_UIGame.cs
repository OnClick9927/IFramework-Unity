public class PanelNames_UIGame
{
	public const string PanelOne = "Assets/Project/Examples/UI/PanelOne.prefab";
	public const string PanelTwo = "Assets/Project/Examples/UI/PanelTwo.prefab";
	public static System.Collections.Generic.Dictionary<string, System.Type> map = new System.Collections.Generic.Dictionary<string, System.Type>()
	{
		{PanelOne,typeof(IFramework.PanelOneView)},
		{PanelTwo,typeof(IFramework.PanelTwoView)},
	};
}
