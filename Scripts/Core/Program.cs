using Godot;
using DemoRPGGame;
using DemoRPGGame.GameSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DemoRPGGame
{
	class Program
	{
		// --- MAIN: CỔNG VÀO GAME ---
		static void Main(string[] args)
		{
			Console.OutputEncoding = Encoding.UTF8;

			while (true)
			{
				Console.Clear();
				Console.ForegroundColor = ConsoleColor.Cyan;
				GD.Print("╔══════════════════════════════════════════╗");
				GD.Print("║        ⚔️  GAME DEMO  ⚔️        ║");
				GD.Print("╚══════════════════════════════════════════╝");
				Console.ResetColor();
				GD.Print("1. 🔥 CHƠI MỚI (NEW GAME)");
				GD.Print("2. 📂 TIẾP TỤC (LOAD GAME)");
				GD.Print("3. 🚪 THOÁT");
				Console.Write(">> Chọn: ");

				string choice = Console.ReadLine();
				if (choice == "1")
				{
					GlobalData.InitGame(); // Tạo mới hoàn toàn
					break; // Vào Lobby
				}
				else if (choice == "2")
				{
					if (SaveManager.LoadGame()) // Load thành công
					{
						break; // Vào Lobby
					}
					// Load thất bại thì lặp lại menu
				}
				else if (choice == "3") System.Environment.Exit(0);
			}

			// 2. Vòng lặp Sảnh Chờ
			while (true)
			{
				ShowLobby();
			}
		}

		// ==================================================================================
		// PHẦN 1: GIAO DIỆN SẢNH CHỜ (LOBBY)
		// ==================================================================================

		static void ShowLobby()
		{
			Console.Clear();
			Console.ForegroundColor = ConsoleColor.Yellow;
			GD.Print("╔════════════════════════════════════════════════════╗");
			GD.Print("║                 🏰 SẢNH CHỜ (PUB)                  ║");
			GD.Print("╚════════════════════════════════════════════════════╝");
			Console.ResetColor();

			GD.Print($"💰 Vàng: {GlobalData.GetItemCount(MaterialType.Gold):N0}");
			GD.Print("\n--- MENU CHÍNH ---");
			GD.Print("1. 🦸 QUẢN LÝ HERO (Mặc đồ, Thức tỉnh...)");
			GD.Print("2. 🎒 TÚI NGUYÊN LIỆU");
			GD.Print("3. ⚔️ BẢN ĐỒ CHIẾN ĐẤU (Đi farm đồ)");
			GD.Print("4. 💾 LƯU GAME (SAVE)");
			GD.Print("5. 🚪 THOÁT GAME");

			Console.Write("\n>> Chọn chức năng: ");
			switch (Console.ReadLine())
			{
				case "1": MenuHero(); break;
				case "2": MenuBag(); break;
				case "3": MenuBattleMap(); break;
				case "4": SaveManager.SaveGame(); break;
				case "5": System.Environment.Exit(0); break;
			}
		}

		static void MenuHero()
		{
			while (true)
			{
				Console.Clear();
				GD.Print("=== 🦸 QUẢN LÝ ĐỘI HÌNH ===");
				for (int i = 0; i < GlobalData.MyHeroes.Count; i++)
				{
					var c = GlobalData.MyHeroes[i];
					GD.Print($"{i + 1}. {c.Name} {GetAwakenStars(c)} | HP: {c.CurrentHP}/{c.SheetHP}");
				}
				GD.Print("0. Quay lại");
				Console.Write(">> Chọn Hero: ");

				if (int.TryParse(Console.ReadLine(), out int idx))
				{
					if (idx == 0) return;
					if (idx > 0 && idx <= GlobalData.MyHeroes.Count)
					{
						ShowHeroDetail(GlobalData.MyHeroes[idx - 1]);
					}
				}
			}
		}

		static void ShowHeroDetail(Character h)
		{
			while (true)
			{
				Console.Clear();
				Console.ForegroundColor = ConsoleColor.Cyan;
				GD.Print($"--- {h.Name.ToUpper()} ---");
				Console.ResetColor();
				GD.Print($"Cấp sao: {GetAwakenStars(h)} (+{h.AwakenLevel})");
				GD.Print($"HP : {h.SheetHP}  | ATK: {h.SheetATK}");
				GD.Print($"DEF: {h.SheetDEF}  | SPD: {h.SheetSPD}");
				GD.Print($"CRIT: {h.SheetCritRate * 100:F0}% | CDMG: {h.SheetCritDamage * 100:F0}%");
				GD.Print($"EFF : {h.SheetEff * 100:F0}%   | RES : {h.SheetRes * 100:F0}%");

				GD.Print("\n--- MENU ---");
				GD.Print("1. 🛡️ QUẢN LÝ TRANG BỊ (Mặc/Tháo)");
				GD.Print("2. 🌟 THỨC TỈNH (Nâng sao)");
				GD.Print("0. Quay lại");
				Console.Write(">> Chọn: ");

				string choice = Console.ReadLine();
				if (choice == "0") return;
				if (choice == "1") ManageGearUI(h); // Mới thêm
				if (choice == "2") PerformAwaken(h);
			}
		}

		// --- UI MẶC ĐỒ (QUAN TRỌNG CHO VIỆC FARM XONG MẶC) ---
		static void ManageGearUI(Character h)
		{
			while (true)
			{
				Console.Clear();
				GD.Print($"=== TRANG BỊ CỦA {h.Name.ToUpper()} ===");

				// Liệt kê 6 slot
				foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
				{
					string status = h.EquippedGear.ContainsKey(slot)
						? $"[{h.EquippedGear[slot].Rarity}] {h.EquippedGear[slot].Name} ({h.EquippedGear[slot].Set})"
						: "(Trống)";
					GD.Print($"{(int)slot + 1}. {slot}: {status}");
				}

				GD.Print("\nChọn Slot để thay đổi (1-6) hoặc 0 để quay lại:");
				if (int.TryParse(Console.ReadLine(), out int slotIdx))
				{
					if (slotIdx == 0) return;
					if (slotIdx >= 1 && slotIdx <= 6)
					{
						EquipmentSlot selectedSlot = (EquipmentSlot)(slotIdx - 1);
						ShowInventoryForSlot(h, selectedSlot);
					}
				}
			}
		}

		static void ShowInventoryForSlot(Character h, EquipmentSlot slot)
		{
			// Lọc đồ trong kho khớp với Slot đang chọn
			var availableGear = GlobalData.GearInventory.FindAll(i => i.Slot == slot);

			Console.Clear();
			GD.Print($"--- KHO ĐỒ: {slot} ---");

			if (h.EquippedGear.ContainsKey(slot)) GD.Print("1. [THÁO ĐỒ RA]");
			else GD.Print("1. (Không làm gì)");

			for (int i = 0; i < availableGear.Count; i++)
			{
				var item = availableGear[i];
				GD.Print($"{i + 2}. [{item.Rarity}] {item.Name} | Set: {item.Set} | Main: {item.MainStats.Value}");
			}

			Console.Write(">> Chọn món: ");
			if (int.TryParse(Console.ReadLine(), out int choice))
			{
				// Tháo đồ
				if (choice == 1 && h.EquippedGear.ContainsKey(slot))
				{
					var oldItem = h.EquippedGear[slot];
					GlobalData.GearInventory.Add(oldItem); // Trả về kho
					h.UnequipItem(slot);
					GD.Print("Đã tháo đồ!");
				}
				// Mặc đồ mới
				else if (choice >= 2 && choice - 2 < availableGear.Count)
				{
					var newItem = availableGear[choice - 2];

					if (h.EquippedGear.ContainsKey(slot)) // Nếu đang mặc thì tháo ra trước
						GlobalData.GearInventory.Add(h.EquippedGear[slot]);

					h.EquipItem(newItem); // Mặc vào
					GlobalData.GearInventory.Remove(newItem); // Xóa khỏi kho
					GD.Print("Đã trang bị!");
				}
			}
		}

		static void PerformAwaken(Character h)
		{
			Console.Clear();
			GD.Print($"✨ THỨC TỈNH: {h.Name} (Lên {h.AwakenLevel + 1} Sao)");

			if (h.AwakenLevel >= 6) { GD.Print("⚠️ Đã Max Cấp!"); Console.ReadLine(); return; }

			var reqs = h.GetAwakenRequirements();
			bool isEnough = true;

			GD.Print("\n🔻 YÊU CẦU:");
			foreach (var r in reqs)
			{
				int have = GlobalData.GetItemCount(r.Key);
				Console.Write($"- {r.Key}: {r.Value} ");
				if (have >= r.Value) { Console.ForegroundColor = ConsoleColor.Green; GD.Print($"(✔ {have})"); }
				else { Console.ForegroundColor = ConsoleColor.Red; GD.Print($"(❌ {have})"); isEnough = false; }
				Console.ResetColor();
			}

			if (!isEnough) { GD.Print("\n❌ Không đủ đồ!"); Console.ReadLine(); return; }

			Console.Write("\n>> Xác nhận nâng cấp (y/n)? ");
			if (Console.ReadLine() == "y")
			{
				foreach (var r in reqs) GlobalData.ConsumeItem(r.Key, r.Value);
				h.ApplyAwakenStats();
				GD.Print("\n🎉 THÀNH CÔNG!");
				Console.ReadLine();
			}
		}

		static void MenuBag()
		{
			Console.Clear();
			GD.Print("=== 🎒 TÚI NGUYÊN LIỆU ===");
			foreach (var item in GlobalData.Inventory)
			{
				if (item.Value > 0) GD.Print($"- {item.Key}: {item.Value}");
			}
			GD.Print("\n(Ấn phím bất kỳ để quay lại)");
			Console.ReadLine();
		}

		// ==================================================================================
		// PHẦN 2: BẢN ĐỒ & CHIẾN ĐẤU (QUAN TRỌNG: DROP SYSTEM)
		// ==================================================================================

		static void MenuBattleMap()
		{
			Console.Clear();
			GD.Print("=== ⚔️ BẢN ĐỒ CHIẾN ĐẤU ===");
			GD.Print("1. Cánh Đồng Slime (Tập sự) -> Drop: SET CÔNG / MÁU (Dễ rớt)");
			GD.Print("2. Trại Goblin     (Dễ)     -> Drop: SET CÔNG / THỦ");
			GD.Print("3. Pháo Đài Orc    (TB)     -> Drop: SET MÁU / KHÁNG");
			GD.Print("4. Hang Rồng       (Boss)   -> Drop: SET TỐC / CRIT / EFF");
			GD.Print("0. Quay lại");
			Console.Write(">> Chọn ải: ");

			List<List<Monster>> waves = null;
			List<EquipmentSet> dropSets = new List<EquipmentSet>();
			bool isTutorial = false;

			switch (Console.ReadLine())
			{
				case "1": // Ải Slime: Dành cho người mới
					waves = new List<List<Monster>> {
						new List<Monster> { new Slime(), new Slime() },
						new List<Monster> { new Slime(), new Slime(), new Slime() }
					};
					dropSets.Add(EquipmentSet.Attack_Set);
					dropSets.Add(EquipmentSet.Health_Set);
					isTutorial = true; // Bật cờ Tutorial để tăng tỉ lệ rớt
					break;
				case "2":
					waves = new List<List<Monster>> {
						new List<Monster> { new Goblin(), new Slime() },
						new List<Monster> { new Goblin(), new Goblin() }
					};
					dropSets.Add(EquipmentSet.Attack_Set);
					dropSets.Add(EquipmentSet.Defense_Set);
					break;
				case "3":
					waves = new List<List<Monster>> {
						new List<Monster> { new Orc(), new Goblin() },
						new List<Monster> { new Orc(), new Orc() }
					};
					dropSets.Add(EquipmentSet.Health_Set);
					dropSets.Add(EquipmentSet.Resistance_Set);
					break;
				case "4": // Rồng
					waves = new List<List<Monster>> {
						new List<Monster> { new Orc(), new Orc() },
						new List<Monster> { new Dragon() }
					};
					dropSets.Add(EquipmentSet.Speed_Set);
					dropSets.Add(EquipmentSet.Critical_Set);
					dropSets.Add(EquipmentSet.Effectiveness_Set);
					break;
				case "0": return;
			}

			if (waves != null) StartBattle(waves, dropSets, isTutorial);
		}

		// HÀM BATTLE CHÍNH: Xử lý Wave và Rớt đồ
		static void StartBattle(List<List<Monster>> waves, List<EquipmentSet> possibleDropSets, bool isTutorial = false)
		{
			// Hồi máu về SheetHP (Máu max thực tế)
			foreach (var h in GlobalData.MyHeroes) h.CurrentHP = h.SheetHP;

			for (int i = 0; i < waves.Count; i++)
			{
				Console.Clear();
				GD.Print($"\n🌊 WAVE {i + 1}/{waves.Count}");
				Thread.Sleep(1000);

				List<Character> allFighters = new List<Character>();
				allFighters.AddRange(GlobalData.MyHeroes);
				allFighters.AddRange(waves[i]);

				bool victory = ProcessBattle(allFighters);

				if (!victory)
				{
					GD.Print("\n💀 THẤT BẠI! Về thành dưỡng sức...");
					Console.ReadLine();
					return;
				}

				// --- HỆ THỐNG RỚT ĐỒ (DROP SYSTEM) ---
				GD.Print("\n📦 --- KẾT QUẢ & CHIẾN LỢI PHẨM ---");

				// 1. Rớt Trang Bị (Gear)
				// Nếu là Tutorial: 100% rớt. Nếu thường: 40% rớt.
				int dropChance = isTutorial ? 100 : 40;

				if (Random.Shared.Next(100) < dropChance && possibleDropSets.Count > 0)
				{
					// Chọn ngẫu nhiên Set
					var set = possibleDropSets[Random.Shared.Next(possibleDropSets.Count)];

					// Chọn ngẫu nhiên Slot (Nếu Tutorial ưu tiên Vũ khí/Áo để có stat cơ bản)
					EquipmentSlot slot;
					if (isTutorial) slot = Random.Shared.Next(2) == 0 ? EquipmentSlot.Weapon : EquipmentSlot.Armor;
					else slot = (EquipmentSlot)Random.Shared.Next(6);

					// Tạo đồ & Thêm vào kho
					Equipment dropItem = EquipmentGenerator.GenerateItem(slot);
					dropItem.Set = set; // Gán Set đúng với ải
					GlobalData.GearInventory.Add(dropItem);

					Console.ForegroundColor = ConsoleColor.Magenta;
					GD.Print($"   🎁 [GEAR] Bạn nhận được: {dropItem.Name}");
					GD.Print($"      (Set: {dropItem.Set} | Main: {dropItem.MainStats} {dropItem.MainStats.Value})");
					Console.ResetColor();
				}
				else
				{
					Console.ForegroundColor = ConsoleColor.DarkGray;
					GD.Print("   (Không có trang bị rớt ra...)");
					Console.ResetColor();
				}

				// 2. Rớt Nguyên Liệu (Cơ bản)
				foreach (var m in waves[i])
				{
					if (m is Slime) GlobalData.AddItem(MaterialType.SlimeJelly, 1);
					if (m is Goblin) GlobalData.AddItem(MaterialType.GoblinDagger, 1);
					if (m is Orc) GlobalData.AddItem(MaterialType.OrcBadge, 1);
					if (m is Dragon) GlobalData.AddItem(MaterialType.DragonScale, 1);
					GlobalData.AddItem(MaterialType.Gold, 100);
				}
				GD.Print("   -> Đã nhặt vàng và nguyên liệu.");

				// Hồi 30% máu
				foreach (var h in GlobalData.MyHeroes)
					if (h.CurrentHP > 0) CombatSystem.ApplyHeal(h, (int)(h.SheetHP * 0.3f));

				GD.Print("🌿 Đội hình hồi phục 30% HP.");
				Console.ReadLine();
			}
			GD.Print("🏆 HOÀN THÀNH ẢI!");
			Console.ReadLine();
		}

		static bool ProcessBattle(List<Character> allFighters)
		{
			int round = 1;
			while (true)
			{
				var heroes = allFighters.FindAll(c => !(c is Monster) && c.CurrentHP > 0);
				var monsters = allFighters.FindAll(c => c is Monster && c.CurrentHP > 0);

				if (monsters.Count == 0) return true;
				if (heroes.Count == 0) return false;

				GD.Print($"\n--- VÒNG {round} ---");
				// Sắp xếp theo TỐC ĐỘ THỰC TẾ (CurrentSPD)
				allFighters.Sort((a, b) => b.CurrentSPD.CompareTo(a.CurrentSPD));

				Console.Write("Turn: ");
				foreach (var f in allFighters) if (f.CurrentHP > 0) Console.Write($"{f.Name} > ");
				GD.Print();

				foreach (var actor in allFighters)
				{
					if (allFighters.Count(c => !(c is Monster) && c.CurrentHP > 0) == 0) return false;
					if (allFighters.Count(c => c is Monster && c.CurrentHP > 0) == 0) return true;

					if (actor.CurrentHP <= 0) continue;

					GD.Print($"\n➤ Lượt của: {actor.Name}");
					bool canAct = actor.OnturnStart();

					if (actor.CurrentHP <= 0) { GD.Print("💀 Gục ngã do DoT."); continue; }
					if (!canAct) { GD.Print("🚫 Bị Choáng/Ngủ, mất lượt."); Thread.Sleep(800); actor.OnturnEnd(); continue; }

					if (actor is Monster monster)
					{
						PerformBossTurn(monster, allFighters);
					}
					else
					{
						PlayerTurn(actor, allFighters);
					}

					actor.OnturnEnd();
					Thread.Sleep(500);
				}
				round++;
			}
		}

		// ==================================================================================
		// PHẦN 3: CÁC HÀM HỖ TRỢ
		// ==================================================================================

		static void PlayerTurn(Character hero, List<Character> allFighters)
		{
			hero.DrawUI();
			var monsters = allFighters.FindAll(c => c is Monster && c.CurrentHP > 0);

			if (monsters.Count == 0) return;

			GD.Print("1. Tấn công thường");

			if (hero is Warrior w) { GD.Print($"2. Chiến Hống {GetCD(w.S_BattleCry)}"); GD.Print($"3. Xoay Kiếm {GetCD(w.S_Whirlwind)}"); }
			else if (hero is Mage m) { GD.Print($"2. Tụ Pháp {GetCD(m.S_ManaCharge)}"); GD.Print($"3. Thiên Thạch {GetCD(m.S_MeteorRain)}"); }
			else if (hero is Saint s) { GD.Print($"2. Ban Phước {GetCD(s.S_Blessing)}"); GD.Print($"3. Đại Hồi Phục {GetCD(s.S_GrandHeal)}"); }
			else if (hero is Debuffer d) { GD.Print($"2. Lời Nguyền {GetCD(d.S_CursedTouch)}"); GD.Print($"3. Ám Sát {GetCD(d.S_Assassinate)}"); }

			Console.Write(">> Chọn: ");
			string choice = Console.ReadLine();
			switch (choice)
			{
				case "1":
					hero.Attack(ChooseTarget(monsters));
					break;
				case "2":
					if (hero is Warrior w2) w2.BattleCry();
					else if (hero is Mage m2) m2.ManaCharge();
					else if (hero is Debuffer d2) d2.CursedTouch(ChooseTarget(monsters));
					else if (hero is Saint s2) { var a = ChooseAlly(allFighters); if (a != null) s2.Blessing(a); }
					break;
				case "3":
					if (hero is Warrior w3) w3.Whirlwind(allFighters);
					else if (hero is Mage m3) m3.CastMeteor(allFighters);
					else if (hero is Debuffer d3) d3.Assassinate(ChooseTarget(monsters));
					else if (hero is Saint s3) { var a = ChooseAlly(allFighters); if (a != null) s3.Heal(a); }
					break;
				default: GD.Print("Mất lượt!"); break;
			}
		}

		static void PerformBossTurn(Monster monster, List<Character> allFighters)
		{
			GD.Print($"\n👿 {monster.Name} đang suy tính...");
			Thread.Sleep(500);
			monster.ExecuteTurn(allFighters);
		}

		static Character ChooseTarget(List<Character> enemies)
		{
			GD.Print("   🔻 Chọn mục tiêu:");
			for (int i = 0; i < enemies.Count; i++)
				GD.Print($"      [{i + 1}] {enemies[i].Name} (HP: {enemies[i].CurrentHP})");

			Console.Write("   >> Nhập số: ");
			if (int.TryParse(Console.ReadLine(), out int idx) && idx > 0 && idx <= enemies.Count)
				return enemies[idx - 1];
			return enemies[0];
		}

		static Character ChooseAlly(List<Character> allFighters)
		{
			var allies = allFighters.FindAll(c => !(c is Monster) && c.CurrentHP > 0);
			GD.Print("   💚 Chọn đồng đội:");
			for (int i = 0; i < allies.Count; i++)
				GD.Print($"      [{i + 1}] {allies[i].Name} ({allies[i].CurrentHP} HP)");

			Console.Write("   >> Nhập số: ");
			if (int.TryParse(Console.ReadLine(), out int idx) && idx > 0 && idx <= allies.Count)
				return allies[idx - 1];
			return allies[0];
		}

		static string GetCD(Skill s) => s.IsReady() ? "[Sẵn]" : $"[{s.CurrentCD}]";

		static string GetAwakenStars(Character h)
		{
			string stars = "";
			for (int i = 0; i < 6; i++) stars += (i < h.AwakenLevel) ? "★" : "☆";
			return stars;
		}
	}
}
