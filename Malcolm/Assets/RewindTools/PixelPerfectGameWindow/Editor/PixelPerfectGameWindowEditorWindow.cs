using UnityEditor;
using UnityEngine;
using System.Collections;

namespace RWDTools
{
	public class PixelPerfectGameWindowEditorWindow : EditorWindow
	{
		[MenuItem ("Window/Pixel Perfect Game Window")]
		static void OpenPopup() {

			PPGWWindow = (PixelPerfectGameWindowEditorWindow) (EditorWindow.GetWindow(typeof(PixelPerfectGameWindowEditorWindow)));

			Vector2 minSize = new Vector2(300, 228);

			PPGWWindow.minSize = minSize;
			PPGWWindow.maxSize = minSize;

#if UNITY_5_3_OR_NEWER
			PPGWWindow.titleContent = new GUIContent("Pixel Perfect Game Window Settings");
#else
			PPGWWindow.title = "Pixel Perfect Game Window Settings";
#endif
			PPGWWindow.ShowPopup();
		}

		static PixelPerfectGameWindowEditorWindow PPGWWindow;

		private static Vector2 gameSize;
		static Vector2 GameSize {
			get { return gameSize; }
			set 
			{ 
				gameSize = value; 
				EditorPrefs.SetInt("PPGW_GameSizeX",Mathf.RoundToInt(gameSize.x));
				EditorPrefs.SetInt("PPGW_GameSizeY",Mathf.RoundToInt(gameSize.y));
			}
		}
	
		private static Vector2 gamePosition;
		static Vector2 GamePosition {
			get { return gamePosition; }
			set 
			{ 
				gamePosition = value; 
				EditorPrefs.SetInt("PPGW_GamePosX",Mathf.RoundToInt(gamePosition.x));
				EditorPrefs.SetInt("PPGW_GamePosY",Mathf.RoundToInt(gamePosition.y));
			}
		}
	
		private static int quitKeyIndex;
		static int QuitKeyIndex {
			get { return quitKeyIndex; }
			set 
			{ 
				quitKeyIndex = value; 
				EditorPrefs.SetInt("PPGW_QuitKeyIndex",quitKeyIndex);
			}
		}

		private static int presetIndex;
		static int PresetIndex {
			get { return presetIndex; }
			set 
			{ 
				presetIndex = value; 
				EditorPrefs.SetInt("PPGW_PresetIndex",presetIndex);
			}
		}
	
		public static string[] ViableQuitCodes = new string[] { "Escape" , "F1" , "End" , "Keypad Minus"  };
		KeyCode QuitKeycode;
	
		private static bool ppgwEnabled = false;
		static bool PPGWEnabled {
			get { return ppgwEnabled; }
			set 
			{ 
				ppgwEnabled = value; 
				EditorPrefs.SetBool("PPGW_Enabled",ppgwEnabled);
			}
		}

		void OnEnable()
		{
			LoadEditorPrefs();
		
			if (GameSize == Vector2.zero)
			{
				GameSize = new Vector2(Screen.currentResolution.width,Screen.currentResolution.height);
			}		
		}
	
		void LoadEditorPrefs()
		{
			gameSize = new Vector2(EditorPrefs.GetInt("PPGW_GameSizeX"),EditorPrefs.GetInt("PPGW_GameSizeY"));
			gamePosition = new Vector2(EditorPrefs.GetInt("PPGW_GamePosX"),EditorPrefs.GetInt("PPGW_GamePosY"));
			quitKeyIndex = EditorPrefs.GetInt("PPGW_QuitKeyIndex");
			ppgwEnabled = EditorPrefs.GetBool("PPGW_Enabled");
			presetIndex = EditorPrefs.GetInt("PPGW_PresetIndex");
		}

		void LoadPreset(int index)
		{
			int width = EditorPrefs.GetInt ("PPGW_Preset_" + index + "_Width");
			int height = EditorPrefs.GetInt ("PPGW_Preset_" + index + "_Height");
			int posX = EditorPrefs.GetInt ("PPGW_Preset_" + index + "_PosX");
			int posY = EditorPrefs.GetInt ("PPGW_Preset_" + index + "_PosY");

			GameSize = new Vector2 (width, height);
			GamePosition = new Vector2 (posX, posY);
		}

		void SavePreset(int index)
		{
			EditorPrefs.SetInt ("PPGW_Preset_" + index + "_Width", (int) GameSize.x);
			EditorPrefs.SetInt ("PPGW_Preset_" + index + "_Height", (int)  GameSize.y);
			EditorPrefs.SetInt ("PPGW_Preset_" + index + "_PosX", (int)  GamePosition.x);
			EditorPrefs.SetInt ("PPGW_Preset_" + index + "_PosY", (int)  GamePosition.y);
		}
	
		void OnGUI()
		{
			GUILayout.Label ("Pixel Perfect Game Window Settings", RWDStyles.Heading);

			GUILayout.BeginVertical(RWDStyles.Section);

			GUIContent PresetTitle = new GUIContent("Presets", "Presets \n\nSave and load presets here.");
			GUILayout.Label(PresetTitle, RWDStyles.SubHeading);

			GUILayout.BeginHorizontal();
			PresetIndex = EditorGUILayout.Popup(PresetIndex, new string[] { "Preset 1", "Preset 2", "Preset 3", "Preset 4", "Preset 5" });
			if (GUI.changed)
			{
				LoadPreset(PresetIndex);
			}
			if (GUILayout.Button("Save Preset"))
			{
				SavePreset(PresetIndex);
			}
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();

			GUILayout.BeginVertical (RWDStyles.Section);

			GUIContent PlayModeTitle = new GUIContent("Play Mode Settings", "Play Mode Settings \n\nThese settings are applied when you press the play button.");
			GUILayout.Label(PlayModeTitle, RWDStyles.SubHeading);

			PPGWEnabled = EditorGUILayout.Toggle("Play Mode", PPGWEnabled);
			QuitKeyIndex = EditorGUILayout.Popup("Exit Play Mode", QuitKeyIndex, ViableQuitCodes);

			GUILayout.EndVertical();

			GUILayout.BeginVertical (RWDStyles.Section);

			GUILayout.BeginHorizontal(GUIStyle.none);
			GUIContent SizeTitle = new GUIContent("Size", "Size \n\nThe size, in pixels, of your game window.");
			GUILayout.Label(SizeTitle, RWDStyles.SubHeading, GUILayout.Width(60));

			Vector2 newGameSize = EditorGUILayout.Vector2Field("",new Vector2((int)GameSize.x, (int)GameSize.y), GUILayout.Height(12));
			if ((newGameSize != GameSize))
			{
				GameSize = new Vector2((int) (newGameSize.x > 1 ? newGameSize.x : 1), (int) (newGameSize.y > 1 ? newGameSize.y : 1));
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(GUIStyle.none);

			GUIContent PositionTitle = new GUIContent("Position", "Position \n\nThe position, relative to the top left of your primary monitor, in pixels, of the top left of the game window.");
			GUILayout.Label(PositionTitle, RWDStyles.SubHeading, GUILayout.Width(60));

			Vector2 newGamePosition = EditorGUILayout.Vector2Field("", new Vector2((int)GamePosition.x, (int)GamePosition.y), GUILayout.Height(12));
			if ((newGamePosition != GamePosition))
			{
				GamePosition = new Vector2((int)newGamePosition.x, (int)newGamePosition.y);
			}
			GUILayout.EndHorizontal();
			GUILayout.EndVertical ();


			GUILayout.BeginHorizontal (RWDStyles.Section);
			GUIContent OverrideTitle = new GUIContent("Override Editor Position", "Override Editor Position \n\nThis button will set your game window to the specified position and size, whether in play more or not.");
			if (GUILayout.Button (OverrideTitle)) 
			{
				PixelPerfectGameWindow.SetGameWindow(GameSize,GamePosition);
				PPGWWindow.Focus();
			}
			GUILayout.EndHorizontal ();
		}

	}
}