using Godot;
using DemoRPGGame;

public partial class MainMenu : Control
{
	[Export] public Control settingsOverlay;
	public override void _Ready()
	{
		settingsOverlay.Hide();
		//Kết nối thanh trượt âm thanh
		var slider = settingsOverlay.GetNode<HSlider>("PanelContainer/VBoxContainer/HSlider");
		slider.Value = -10; 
		AudioServer.SetBusVolumeDb(0, -10);
		slider.ValueChanged += (value) => {
			float volumedb = (float)value;
			AudioServer.SetBusVolumeDb(0, volumedb);
			AudioServer.SetBusMute(0, value <= -39);
		};
	}
	public void _on_btn_setting_pressed()
	{
		settingsOverlay.Show();
		settingsOverlay.Modulate = new Color(1, 1, 1, 0); 
		var tween = CreateTween();
		tween.TweenProperty(settingsOverlay, "modulate", new Color(1, 1, 1, 1), 0.2f);
	}
	public void _on_btn_continue_pressed()
	{
		GD.Print(">>> Đang kiểm tra dữ liệu để vào game...");
		var loadedData = SaveManager.LoadGame();

		if (loadedData != null)
		{
			GlobalData.Instance.CurrentPlayer = loadedData;
			GD.Print(">>> Tải save thành công. Tiếp tục hành trình.");
		}
		else
		{
			GD.Print(">>> Không tìm thấy save. Khởi tạo tài khoản mới.");
		}
		GetTree().ChangeSceneToFile("res://Scenes/Lobby.tscn");
	}

	public void _on_btn_exit_pressed()
	{
		GetTree().Quit();
	}
	public void _on_btn_close_pressed()
	{
		settingsOverlay.Hide();
	}
}
