using UnityEditor;
using UnityEngine;
using System.Collections;

[InitializeOnLoad]
public static class PixelPerfectGameWindow
{
	private const int TabHeight = 22;
    private const int HeightFix = -5;

	public static Vector2 Size;
	public static Vector2 Position;

	private static KeyCode QuitKeyCode;
	private static bool PPGWEnabled;
	
	static bool quitting = false;
	
	static PixelPerfectGameWindow()
	{
		UpdateEditorPrefs();

		EditorApplication.playmodeStateChanged -= CheckPlayModeState;
		EditorApplication.playmodeStateChanged += CheckPlayModeState;

        EditorApplication.update -= CheckExitKey;
        EditorApplication.update += CheckExitKey;
	}
	
	static void CheckExitKey()
	{
		if (EditorApplication.isPlaying && PPGWEnabled)
		{
			if (Input.GetKey(QuitKeyCode) && !quitting)
			{
				quitting = true;
				EditorApplication.isPlaying = false;
			}
		}
	}
	
	static void CheckPlayModeState()
	{
		if (PPGWEnabled)
		{
			if (EditorApplication.isPlayingOrWillChangePlaymode)
			{
				SaveGameViewSettings();
			}
			else
			{
				CloseGameWindow();
				quitting = false;
			}
			
			if (EditorApplication.isPlaying)
			{
				FullScreenGameWindow();
			}
		}
	}
	
	static EditorWindow GetMainGameView(){
		EditorApplication.ExecuteMenuItem("Window/Game");
		System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
		System.Reflection.MethodInfo GetMainGameView = T.GetMethod("GetMainGameView",System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
		System.Object Res = GetMainGameView.Invoke(null,null);
		return (EditorWindow)Res;
	}
	
	static void UpdateEditorPrefs()
	{
		Size = new Vector2(EditorPrefs.GetInt("PPGW_GameSizeX"),EditorPrefs.GetInt("PPGW_GameSizeY"));
		Position = new Vector2(EditorPrefs.GetInt("PPGW_GamePosX"),EditorPrefs.GetInt("PPGW_GamePosY"));
		SetQuitKey(EditorPrefs.GetInt("PPGW_QuitKeyIndex"));
		PPGWEnabled = EditorPrefs.GetBool("PPGW_Enabled");
	}
	
	static void SetQuitKey(int index)
	{
		switch (index)
		{
		 case 0:
		 QuitKeyCode = KeyCode.Escape;
		 break;
		 case 1:
		 QuitKeyCode = KeyCode.F1;
		 break;
		 case 2:
		 QuitKeyCode = KeyCode.End;
		 break;
		 case 3:
		 QuitKeyCode = KeyCode.KeypadMinus;
		 break;
		}
	}	
	
	static void FullScreenGameWindow(){
		
		UpdateEditorPrefs ();
		SetGameWindow (Size, Position);
		
	}

	public static void SetGameWindow(Vector2 size, Vector2 position)
	{
		EditorWindow gameView = GetMainGameView();

		Rect newPos = new Rect(position.x, position.y - TabHeight, size.x, size.y + HeightFix + TabHeight);
		
		gameView.position = newPos;
		gameView.minSize = new Vector2(size.x, size.y + HeightFix + TabHeight);
		gameView.maxSize = gameView.minSize;
		gameView.position = newPos;	
	}
	
	static void SaveGameViewSettings()
	{
		System.Type T = System.Type.GetType("UnityEditor.WindowLayout,UnityEditor");
		System.Reflection.MethodInfo SaveLayoutMethod = T.GetMethod("SaveWindowLayout", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
		SaveLayoutMethod.Invoke(null,new object[] { "PPGW_PreviousLayout" });
	}
	
	static void CloseGameWindow()
	{
		System.Type T = System.Type.GetType("UnityEditor.WindowLayout,UnityEditor");
		System.Reflection.MethodInfo SaveLayoutMethod = T.GetMethod("LoadWindowLayout");
		SaveLayoutMethod.Invoke(null,new object[] { "PPGW_PreviousLayout", false }); 
	} 
}
