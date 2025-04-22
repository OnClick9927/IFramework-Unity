public class PanelNames
{
	public const string PanelOne = "Assets/Project/Examples/UI/PanelOne.prefab";
	public const string PanelTwo = "Assets/Project/Examples/UI/PanelTwo.prefab";
	public const string PanelTwo_1_ = "Assets/Project/Examples/UI/Panel##Two 1 @.prefab";
	public static System.Collections.Generic.Dictionary<string, System.Type> map = new System.Collections.Generic.Dictionary<string, System.Type>()
	{
		{PanelOne,typeof(IFramework.PanelOneView)},
		{PanelTwo,typeof(IFramework.PanelTwoView)},
		{PanelTwo_1_,typeof(IFramework.PanelTwo_1_View)},
	};
}
