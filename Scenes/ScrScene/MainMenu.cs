using Godot;
using DemoRPGGame;
using DemoRPGGame.GameSystem;

public partial class MainMenu : Control
{
	// Kiểm tra tên hàm này phải khớp 100% với Signal trong Godot
	public void _on_btn_newgame_pressed()
	{
		GD.Print(">>> Nút Bắt đầu mới đã được nhấn!");
		
		// 1. Khởi tạo dữ liệu (Arthur, Merlin...)
		GlobalData.InitGame(); 
		GD.Print(">>> Đã khởi tạo GlobalData thành công.");

		// 2. Chuyển cảnh (Đảm bảo đường dẫn file Lobby.tscn chính xác)
		Error err = GetTree().ChangeSceneToFile("res://Scenes/Lobby.tscn");

		if (err == Error.Ok)
		{
			GD.Print(">>> Chuyển sang Lobby thành công!");
		}
		else
		{
			GD.PrintErr($">>> LỖI CHUYỂN CẢNH: {err}. Kiểm tra lại đường dẫn res://Scenes/Lobby.tscn");
		}
	}

	public void _on_btn_continue_pressed()
	{
		GD.Print(">>> Đang thử tải file save...");
		if (SaveManager.LoadGame())
		{
			GetTree().ChangeSceneToFile("res://Scenes/Lobby.tscn");
		}
	}

	public void _on_btn_exit_pressed()
	{
		GetTree().Quit();
	}
}
