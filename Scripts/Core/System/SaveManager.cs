using Godot;
using System.Text.Json;

namespace DemoRPGGame
{
	public static class SaveManager
	{
		private static string SavePath = "user://save_game.json";

		public static void SaveGame(PlayerData data)
		{
			string jsonString = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
			using (var file = Godot.FileAccess.Open(SavePath, Godot.FileAccess.ModeFlags.Write))
			{
				file.StoreString(jsonString);
			}
			GD.Print(">>> Đã tự động lưu game!");
		}

		public static PlayerData LoadGame()
		{
			if (!Godot.FileAccess.FileExists(SavePath))
			{
				PlayerData newData = new PlayerData();
				newData.SetDefault();
				SaveGame(newData);
				return newData;
			}

			using (var file = Godot.FileAccess.Open(SavePath, Godot.FileAccess.ModeFlags.Read))
			{
				return JsonSerializer.Deserialize<PlayerData>(file.GetAsText());
			}
		}

		public static PlayerData ResetAccount()
		{
			PlayerData resetData = new PlayerData();
			resetData.SetDefault();
			SaveGame(resetData);
			return resetData;
		}
	}
}
