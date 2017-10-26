using UnityEngine;
using UnityEditor;
using System.Collections;

namespace RWDTools
{
	public class RWDStyles : MonoBehaviour
	{
		static Texture2D disc;
		public static Texture2D Disc {
			get { if (disc == null) { disc = Resources.Load<Texture2D>("Disc"); }return disc; }
			set { disc = value; }
		}

		static Texture2D quadGrad;
		public static Texture2D QuadGrad
		{
			get { if (quadGrad == null) { quadGrad = Resources.Load<Texture2D>("QuadGrad"); } return quadGrad; }
			set { quadGrad = value; }
		}

		static GUIStyle infoBox;
		public static GUIStyle InfoBox
		{
			get
			{
				if (infoBox == null)
				{
					try
					{
						infoBox = new GUIStyle(GUI.skin.GetStyle("Box"));
						infoBox.normal.background = GetBorderedTexture(new Color(0.2f,0.2f,0.2f,0.7f), new Color(0,0,0, 0.7f));
						infoBox.border = new RectOffset(1, 1, 1, 1);
						infoBox.padding = new RectOffset(5, 5, 5, 5);
						infoBox.normal.textColor = new Color(0.7f, 0.7f, 0.7f, 1);
						infoBox.alignment = TextAnchor.MiddleCenter;
						infoBox.fontSize = 9;

					}
					catch
					{
						infoBox = GetErrorGUIStyle();
					}
				}
				return infoBox;
			}
		}

		static GUIStyle midBox;
		public static GUIStyle MidBox
		{
			get
			{
				if (midBox == null)
				{
					try
					{
						midBox = new GUIStyle(GUI.skin.GetStyle("Box"));
						midBox.normal.background = GetBorderedTexture(new Color(0.26f, 0.26f, 0.26f), new Color(0.15f, 0.15f, 0.15f, 1));
						midBox.border = new RectOffset(1, 1, 1, 1);
						midBox.padding = new RectOffset(5,5,5,5);
						midBox.normal.textColor = new Color(0.7f, 0.7f, 0.7f, 1);
						midBox.alignment = TextAnchor.MiddleCenter;
						midBox.fontSize = 10;

					}
					catch
					{
						midBox = GetErrorGUIStyle();
					}
				}
				return midBox;
			}
		}

		private static GUIStyle foldout = null;
		public static GUIStyle Foldout
		{
			get
			{
				if (foldout == null)
				{
					try
					{
						foldout = new GUIStyle("Foldout");
					}
					catch { foldout = GetErrorGUIStyle(); }

				}
				return foldout;
			}
		}

		private static GUIStyle m_buttonTight = null;
		public static GUIStyle ButtonTight
		{
			get
			{
				if (m_buttonTight == null)
				{
					try
					{
						m_buttonTight = new GUIStyle(Button);
						m_buttonTight.margin = new RectOffset(0, 0, 0, 0);
						m_buttonTight.padding = new RectOffset(0, 0, 0, 0);
						m_buttonTight.fixedHeight = 0;
						m_buttonTight.fixedWidth = 0;
						m_buttonTight.overflow = new RectOffset(2, 2, 2, 2);
					}
					catch { m_buttonTight = GetErrorGUIStyle(); }

				}
				return m_buttonTight;
			}
		}

		private static GUIStyle m_Button = null;
		public static GUIStyle Button
		{
			get
			{
				if (m_Button == null)
				{
					try
					{
						m_Button = new GUIStyle(GUIStyle.none);

						m_Button.padding = new RectOffset(1, 1, 0, 1);
						m_Button.margin = new RectOffset(10, 10, 0, 0);
						m_Button.contentOffset = Vector2.zero;
						m_Button.fixedHeight = 24;

						m_Button.fontSize = 10;

                        m_Button.normal.textColor = EditorGUIUtility.isProSkin ? new Color(0.8f,0.8f,0.8f) : new Color(0.35f, 0.35f, 0.35f);
						m_Button.hover.textColor = EditorGUIUtility.isProSkin ? Color.white : new Color(0.2f,0.2f,0.2f);
						m_Button.active.textColor = EditorGUIUtility.isProSkin ? Color.gray : new Color(0.2f, 0.2f, 0.2f);
						m_Button.focused.textColor = Color.gray;

						m_Button.alignment = TextAnchor.MiddleCenter;

						m_Button.normal.background = EditorGUIUtility.isProSkin ? Resources.Load<Texture2D>("ToolbarButton_Idle") : Resources.Load<Texture2D>("ToolbarButton_Idle_Alt"); 
						m_Button.hover.background = EditorGUIUtility.isProSkin ? Resources.Load<Texture2D>("ToolbarButton_Hovered") : Resources.Load<Texture2D>("ToolbarButton_Hovered_Alt");
						m_Button.active.background = EditorGUIUtility.isProSkin ? Resources.Load<Texture2D>("ToolbarButton_Active") : Resources.Load<Texture2D>("ToolbarButton_Active_Alt");
						m_Button.focused.background = m_Button.active.background; 

						m_Button.border = new RectOffset(3, 3, 3, 3);
					}
					catch { m_Button = GetErrorGUIStyle(); }

				}
				return m_Button;
			}
		}

		private static GUIStyle m_ToolbarButton = null;
		public static GUIStyle ToolbarButton
		{
			get
			{
				if (m_ToolbarButton == null)
				{
					try
					{
						m_ToolbarButton = new GUIStyle(GUI.skin.GetStyle("Button"));

						m_ToolbarButton.fixedHeight = 42;
						m_ToolbarButton.fixedWidth = 52;
						m_ToolbarButton.padding = new RectOffset(5, 5, 6, 4);
						m_ToolbarButton.margin = new RectOffset(2, 2, 2, 2);

						m_ToolbarButton.normal.background = null;
						m_ToolbarButton.hover.background = Resources.Load<Texture2D>("ToolbarButton_Hovered");
						m_ToolbarButton.active.background = Resources.Load<Texture2D>("ToolbarButton_Active");
						m_ToolbarButton.focused.background = m_ToolbarButton.active.background;

						m_ToolbarButton.onNormal.background = m_ToolbarButton.active.background;
						m_ToolbarButton.onHover.background = m_ToolbarButton.active.background;
						m_ToolbarButton.onActive.background = m_ToolbarButton.active.background;
						m_ToolbarButton.onFocused.background = m_ToolbarButton.active.background;
					}
					catch { m_ToolbarButton = GetErrorGUIStyle(); }

				}
				return m_ToolbarButton;
			}
		}

		static GUIStyle middleLeftLabel = null;
		public static GUIStyle MiddleLeftLabel
		{
			get
			{
				if (middleLeftLabel == null)
				{
					try
					{
						middleLeftLabel = new GUIStyle("Label");
						middleLeftLabel.alignment = TextAnchor.MiddleLeft;
						middleLeftLabel.stretchHeight = true;
					}
					catch { middleLeftLabel = GetErrorGUIStyle(); }
				}
				return middleLeftLabel;
			}
		}

		static GUIStyle smallFloatField = null;
		public static GUIStyle SmallFloatField
		{
			get
			{
				if (smallFloatField == null)
				{
					try
					{
						smallFloatField = new GUIStyle("TextField");
						smallFloatField.alignment = TextAnchor.MiddleLeft;
						smallFloatField.fixedWidth = 80;
						smallFloatField.stretchWidth = false;
						smallFloatField.margin = new RectOffset(10,3,10,3);
						smallFloatField.padding = new RectOffset(0,0,0,0);
						smallFloatField.fixedHeight = 0;
						middleLeftLabel.stretchHeight = true;
						smallFloatField.overflow = new RectOffset(5,3,3,3);
					}
					catch { smallFloatField = GetErrorGUIStyle(); }
				}
				return smallFloatField;
			}
		}

		static GUIStyle padlock = null;
		public static GUIStyle Padlock
		{
			get
			{
				if (padlock == null)
				{
					try
					{
						padlock = new GUIStyle("IN LockButton");
						padlock.margin = new RectOffset(2, 5, 2, 2);
						padlock.padding = new RectOffset(5, 5, 5, 5);

					}
					catch { padlock = GetErrorGUIStyle(); }
				}
				return padlock;
			}
		}

		static GUIStyle alignedToggle = null;
		public static GUIStyle AlignedToggle
		{
			get
			{
				if (alignedToggle == null)
				{
					try
					{
						alignedToggle = new GUIStyle("Toggle");
						alignedToggle.padding.top = 0;
						alignedToggle.overflow.top = -2;
					}
					catch { alignedToggle = GetErrorGUIStyle(); }
				}
				return alignedToggle;
			}
		}

		static GUIStyle smallBody = null;
		public static GUIStyle SmallBody
		{
			get
			{
				if (smallBody == null)
				{
					try
					{
						smallBody = new GUIStyle("Label");
						smallBody.fontSize = 9;
						smallBody.wordWrap = true;
						smallBody.normal.textColor = Color.gray;
					}
					catch { smallBody = GetErrorGUIStyle(); }
				}
				return smallBody;
			}
		}

		static GUIStyle subHeading = null;
		public static GUIStyle SubHeading
		{
			get
			{
				if (subHeading == null)
				{
					try
					{
						subHeading = new GUIStyle("Label");
						subHeading.fontStyle = FontStyle.Bold;
					}
					catch { subHeading = GetErrorGUIStyle(); }
				}
				return subHeading;
			}
		}

		static GUIStyle heading = null;
		public static GUIStyle Heading
		{
			get
			{
				if (heading == null)
				{
					try
					{
						heading = new GUIStyle("OL Title");
						heading.padding = new RectOffset(10, 10, 10, 10);
						heading.margin = new RectOffset(4, 4, 4, 4);
						heading.fixedHeight = 0;
						heading.alignment = TextAnchor.MiddleCenter;
					}
					catch { heading = GetErrorGUIStyle(); }
				}
				return heading;
			}
		}

		static GUIStyle dragSection = null;
		public static GUIStyle DragSection
		{
			get
			{
				if (dragSection == null)
				{
					try
					{
						dragSection = new GUIStyle(GUIStyle.none);
						dragSection.alignment = TextAnchor.MiddleCenter;
						dragSection.normal.textColor = EditorGUIUtility.isProSkin ? new Color(0.6f, 0.6f, 0.6f, 1) : new Color(0.4f,0.4f,0.4f,1);
						dragSection.fontSize = 9;
						dragSection.normal.background = EditorGUIUtility.isProSkin ? GetBorderedTexture(new Color(0.3f, 0.3f, 0.3f), new Color(0.2f, 0.2f, 0.2f)) : GetBorderedTexture(new Color(0.7f, 0.7f, 0.7f), new Color(0.8f, 0.8f, 0.8f));
						dragSection.border = new RectOffset(1, 1, 1, 1);
					}
					catch { dragSection = GetErrorGUIStyle(); }
				}
				return dragSection;
			}
		}

		static GUIStyle section = null;
		public static GUIStyle Section
		{
			get
			{
				if (section == null)
				{
					try
					{
						section = new GUIStyle(GUI.skin.GetStyle("sv_iconselector_back"));
						section.padding = new RectOffset(4, 4, 4, 4);
						section.margin = new RectOffset(4, 4, 4, 4);
						section.stretchHeight = false;
					}
					catch { section = GetErrorGUIStyle(); }
				}
				return section;
			}
		}

		static Texture2D alphaPattern;
		public static Texture2D AlphaPattern
		{
			get {
			if (alphaPattern == null)
			{
					alphaPattern = new Texture2D(16, 16);
					alphaPattern.filterMode = FilterMode.Point;
					for (int x = 0; x < alphaPattern.width; x++)
					{
						for (int y = 0; y < alphaPattern.height; y++)
						{
							alphaPattern.SetPixel(x, y, Color.white);
							if (x > 7 ^ y > 7)
							{
								alphaPattern.SetPixel(x, y, new Color(0.8f, 0.8f, 0.8f));
							}
						}
					}
					alphaPattern.Apply();
			}
			return alphaPattern;
			}
			set{
				alphaPattern = value;
			}
		}

		static Texture2D GetFlatTexture(Color color)
		{
			Texture2D returnTex = new Texture2D(1, 1);
			returnTex.filterMode = FilterMode.Point;
			returnTex.SetPixel(0, 0, color);
			returnTex.Apply();
			returnTex.hideFlags = HideFlags.HideAndDontSave;
			return returnTex;
		}

		static Texture2D GetBorderedTexture(Color border, Color centre)
		{
			Texture2D returnTex = new Texture2D(3, 3);
			returnTex.filterMode = FilterMode.Point;
			for (int x = 0; x < returnTex.width; x++)
			{
				for (int y = 0; y < returnTex.height; y++)
				{
					returnTex.SetPixel(x, y, border);
					if (x == 1 && y == 1)
					{
						returnTex.SetPixel(x, y, centre);
					}
				}
			}
			returnTex.Apply();
			returnTex.hideFlags = HideFlags.HideAndDontSave;
			return returnTex;
		}


		public static GUISkin CurrentBuiltinSkin
		{
			get
			{
				return EditorGUIUtility.isProSkin ?
					EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene) :
						EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);
			}
		}

		public static bool IsUsingBuiltinSkin {
			get { return (GUI.skin == CurrentBuiltinSkin); }
		}

		private static GUIStyle GetErrorGUIStyle()
		{
			GUIStyle s = new GUIStyle();
			s.normal.textColor = Color.magenta;
			return s;
		}
	}
}
