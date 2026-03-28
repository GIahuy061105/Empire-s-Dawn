using Godot;
using System.Collections.Generic;

namespace DemoRPGGame
{
	public partial class GlobalData : Node
	{
		public static GlobalData Instance { get; private set; }
		public PlayerData CurrentPlayer { get; set; }

		public override void _Ready()
		{
			Instance = this;
			LoadGameFromDisk();
		}

		public void LoadGameFromDisk()
		{
			CurrentPlayer = SaveManager.LoadGame();
			GD.Print("--- [GlobalData] Dữ liệu người chơi đã sẵn sàng! ---");
		}
	}
}
