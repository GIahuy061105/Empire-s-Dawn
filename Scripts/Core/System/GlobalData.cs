using Godot;
using System.Collections.Generic;

namespace DemoRPGGame
{
	// BỎ từ khóa 'static' ở class, nhưng GIỮ 'static' cho các biến/hàm
	// Thêm 'partial' và kế thừa 'Node' để Godot cho phép làm AutoLoad
	public partial class GlobalData : Node
	{
		// 1. Danh sách Hero (Dùng static để SaveManager gọi trực tiếp được)
		public static List<Character> MyHeroes = new List<Character>();

		// 2. Kho đồ (Inventory)
		public static Dictionary<MaterialType, int> Inventory = new Dictionary<MaterialType, int>();
		public static List<Equipment> GearInventory = new List<Equipment>();

		// 3. Hàm khởi tạo dữ liệu ban đầu (New Game)
		public static void InitGame()
		{
			MyHeroes.Clear();
			Inventory.Clear();
			GearInventory.Clear();

			// Tặng sẵn 4 Hero 
			MyHeroes.Add(new Warrior("Arthur"));
			MyHeroes.Add(new Mage("Merlin"));
			MyHeroes.Add(new Saint("Anna"));
			MyHeroes.Add(new Debuffer("Zed"));
			
			// Tặng ít vàng khởi nghiệp
			AddItem(MaterialType.Gold, 10000);
			
			GD.Print("--- [GlobalData] Khởi tạo hành trình mới thành công! ---");
		}

		// --- CÁC HÀM QUẢN LÝ KHO ĐỒ  ---
		public static void AddItem(MaterialType type, int amount)
		{
			if (!Inventory.ContainsKey(type)) Inventory[type] = 0;
			Inventory[type] += amount;
		}

		public static int GetItemCount(MaterialType type) => Inventory.ContainsKey(type) ? Inventory[type] : 0;

		public static bool ConsumeItem(MaterialType type, int amount)
		{
			if (GetItemCount(type) >= amount)
			{
				Inventory[type] -= amount;
				return true;
			}
			return false;
		}
	}
}
