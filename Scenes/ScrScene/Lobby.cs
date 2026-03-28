using Godot;
using System;
using System.Collections.Generic;
using DemoRPGGame;
public partial class Lobby : Control
{
	[ExportGroup("Top Bar")]
	[Export] public Label LblAccountName;
	[Export] public Label LblAccountLevel;
	[Export] public Label LblGold;
	[Export] public Label LblDiamond;
	[ExportGroup("Hero Formation")]
	[Export] public HeroSlotUI[] HeroSlots; 

	[ExportGroup("Overlays")]
	[Export] public Control SettingOverlay;

	public override void _Ready()
	{
		RefreshLobbyUI();
		SettingOverlay.Hide();
	}

	public void RefreshLobbyUI()
	{
		var player = GlobalData.Instance.CurrentPlayer;
		LblGold.Text = player.Gold.ToString("N0");      // Hiện: 500,000
		LblDiamond.Text = player.Diamond.ToString("N0"); // Hiện: 1,200

		// --- Cập nhật 4 Thẻ Hero ---
		for (int i = 0; i < HeroSlots.Length; i++)
		{
			if (i < player.OwnedCardIds.Count)
			{
				string heroId = player.OwnedCardIds[i];
				var texture = GD.Load<Texture2D>($"res://Assets/Heroes/{heroId}_Avatar.png");
				HeroSlots[i].SetupCard(heroId, texture, 1); 
			}
			else
			{
				HeroSlots[i].SetEmpty();
			}
		}
	}
	public void _on_btn_setting_pressed()
	{
		SettingOverlay.Show();
	}
	public void _on_btn_close_pressed()
	{
		SettingOverlay.Hide();
	}
	public void _on_btn_hero_view_pressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/HeroView.tscn");
	}
	public void _on_btn_bag_view_pressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/BagView.tscn");
	}
	public void _on_btn_battle_pressed()
	{
		GD.Print(">>> Tiến vào chiến trường 10x10!");
		GetTree().ChangeSceneToFile("res://Scenes/BattleScene.tscn");
	}

	public void _on_btn_gacha_pressed()
	{
		GD.Print(">>> Mở màn hình triệu hồi!");
		GetTree().ChangeSceneToFile("res://Scenes/GachaScene.tscn");
	}
	
}
