using Godot;
using System.Collections.Generic;
using DemoRPGGame;

public partial class Lobby : Control
{
	[Export] public PackedScene HeroCardPrefab; // Kéo file HeroCard.tscn vào đây ở Inspector
	
	public override void _Ready()
	{
		DisplayHeroes();
	}

	public void DisplayHeroes()
	{
		var container = GetNode<Container>("ScrollContainer/HeroList");
		
		// Xóa các thẻ cũ nếu có
		foreach (Node child in container.GetChildren()) child.QueueFree();

		// Duyệt qua danh sách 4 Hero trong GlobalData
		foreach (var hero in GlobalData.MyHeroes)
		{
			// Tạo một thẻ mới từ bản thiết kế mẫu
			var card = HeroCardPrefab.Instantiate();
			GD.Print("Loại Node vừa tạo ra là: " + card.GetType().Name);
			container.AddChild(card);

			// Cập nhật thông tin lên thẻ
			card.GetNode<Label>("VBoxContainer/NameLabel").Text = hero.Name;
			card.GetNode<Label>("VBoxContainer/ClassLabel").Text = hero.GetType().Name;
			
			var hpBar = card.GetNode<ProgressBar>("VBoxContainer/HPBar");
			hpBar.MaxValue = hero.SheetHP;
			hpBar.Value = hero.CurrentHP;
		}
	}

	public void _on_btn_battle_pressed()
	{
		// Chuyển sang Scene bàn cờ 10x10
		GetTree().ChangeSceneToFile("res://Scenes/BattleScene.tscn");
	}
}
