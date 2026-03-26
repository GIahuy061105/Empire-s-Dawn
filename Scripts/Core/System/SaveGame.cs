using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.IO;
using System.Text.Json.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace DemoRPGGame.GameSystem
{
	public class SaveData
	{
		public int Gold  { get; set; }
		public List<InventoryItemSave> Inventory { get; set; } = new List<InventoryItemSave>();

		// Lưu kho trang bị
		public List<Equipment> GearInventory { get; set; } = new List<Equipment>();

		// Lưu danh sách Hero (Chi tiết từng con)
		public List<HeroSaveModel> Heroes { get; set; } = new List<HeroSaveModel>();
	}
	public class InventoryItemSave
	{
		public MaterialType MaterialType { get; set; }
		public int Amount  { get; set; }

	}
	public class HeroSaveModel
	{
		public string ClassType { get; set; }
		public string Name { get; set; }
		public int AwakenLevel { get; set; }
		public int CurrentHP { get; set; }
		// Lưu trang bị đang mặc
		public Dictionary<EquipmentSlot, Equipment> EquippedGear { get; set; } = new Dictionary<EquipmentSlot, Equipment>();
	}
	public static class SaveManager
	{
		// Sử dụng đường dẫn 'user://' của Godot để đảm bảo quyền ghi file
		private static string _filePath = ProjectSettings.GlobalizePath("user://savegame.json");

		public static void SaveGame()
		{
			SaveData data = new SaveData();
			data.Gold = GlobalData.GetItemCount(MaterialType.Gold);
			
			foreach (var item in GlobalData.Inventory)
			{
				if (item.Key != MaterialType.Gold)
					data.Inventory.Add(new InventoryItemSave { MaterialType = item.Key, Amount = item.Value });
			}

			data.GearInventory = GlobalData.GearInventory;

			foreach (var hero in GlobalData.MyHeroes)
			{
				HeroSaveModel model = new HeroSaveModel {
					Name = hero.Name,
					ClassType = hero.GetType().Name,
					AwakenLevel = hero.AwakenLevel,
					CurrentHP = hero.CurrentHP
				};

				foreach (var gear in hero.EquippedGear.Values)
				{
					model.EquippedGear.Add(gear.Slot, gear);
				}
				data.Heroes.Add(model);
			}

			var options = new JsonSerializerOptions { WriteIndented = true };
			options.Converters.Add(new JsonStringEnumConverter());

			string jsonString = JsonSerializer.Serialize(data, options);
			File.WriteAllText(_filePath, jsonString);

			GD.Print($"\n💾 ĐÃ LƯU GAME THÀNH CÔNG! (Đường dẫn: {_filePath})");
		}

		public static bool LoadGame()
		{
			if (!File.Exists(_filePath))
			{
				GD.PrintErr("⚠️ Chưa có file save nào!");
				return false;
			}

			try
			{
				string jsonString = File.ReadAllText(_filePath);
				var options = new JsonSerializerOptions();
				options.Converters.Add(new JsonStringEnumConverter());

				SaveData data = JsonSerializer.Deserialize<SaveData>(jsonString, options);

				GlobalData.MyHeroes.Clear();
				GlobalData.Inventory.Clear();
				GlobalData.GearInventory.Clear();

				GlobalData.Inventory[MaterialType.Gold] = data.Gold;
				foreach (var inv in data.Inventory)
				{
					GlobalData.Inventory[inv.MaterialType] = inv.Amount;
				}
				GlobalData.GearInventory = data.GearInventory;

				foreach (var model in data.Heroes)
				{
					Character hero = model.ClassType switch
					{
						"Warrior" => new Warrior(model.Name),
						"Mage" => new Mage(model.Name),
						"Saint" => new Saint(model.Name),
						"Debuffer" => new Debuffer(model.Name),
						_ => new Warrior(model.Name)
					};

					int targetAwaken = model.AwakenLevel;
					hero.AwakenLevel = 0; 
					for (int i = 0; i < targetAwaken; i++)
					{
						hero.ApplyAwakenStats(silent: true);
					}

					foreach (var gear in model.EquippedGear.Values)
					{
						hero.EquipItem(gear);
					}

					hero.CurrentHP = model.CurrentHP;
					GlobalData.MyHeroes.Add(hero);
				}

				GD.Print("\n📂 ĐÃ TẢI GAME THÀNH CÔNG!");
				return true;
			}
			catch (Exception ex)
			{
				GD.PrintErr($"Lỗi khi tải file save: {ex.Message}");
				return false;
			}
		}
	}
}
