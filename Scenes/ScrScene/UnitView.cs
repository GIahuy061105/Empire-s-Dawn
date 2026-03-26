using Godot;
using DemoRPGGame; // Gọi namespace từ logic của bạn

public partial class UnitView : Node2D
{
	// Đây là biến kết nối với bộ não C# của bạn
	public Character Logic; 

	public override void _Ready()
	{
		if (Logic != null)
		{
			GetNode<Label>("NameTag").Text = Logic.Name;
			UpdateUI();
		}
	}

	public void UpdateUI()
	{
		var hpBar = GetNode<ProgressBar>("HPBar");
		hpBar.MaxValue = Logic.SheetHP;
		hpBar.Value = Logic.CurrentHP;
	}
}
