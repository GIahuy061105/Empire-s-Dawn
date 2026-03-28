using Godot;
using System;

public partial class HeroSlotUI : Button
{
	[Export] public TextureRect avatarNode;
	[Export] public TextureRect plusIconNode;
	[Export] public Label nameLabel;
	[Export] public Label levelLabel;
	[Export] public Texture2D emptyFrame; 
	[Export] public Texture2D occupiedFrame;

	public override void _Ready()
	{
		SetEmpty();
	}
	public void SetupCard(string heroName, Texture2D avatarTex, int level)
	{
		plusIconNode.Hide();     
		avatarNode.Show();      
		avatarNode.Texture = avatarTex;
		
		nameLabel.Text = heroName;
		levelLabel.Text = $"Lv.{level}";
	}

	public void SetEmpty()
	{
		avatarNode.Hide();       
		plusIconNode.Show();    
		
		nameLabel.Text = "Trống";
		levelLabel.Text = "";
		
		// GetNode<TextureRect>("BgCard").Texture = emptyFrame;
	}
}
