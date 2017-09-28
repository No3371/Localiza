using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CustomEditorUtility {
#if UNITY_EDITOR
	public static void DrawRectOutline(Rect rect)
	{
		DrawRectOutline(rect, new Color(0, 0, 0, 0.4f));
	}

	public static void DrawRectOutline(Rect rect, Color color)
	{
		Handles.color = color;
		Handles.DrawPolyLine(new Vector2(rect.xMin, rect.yMin), new Vector2(rect.xMin, rect.yMax), new Vector2(rect.xMax, rect.yMax), new Vector2(rect.xMax, rect.yMin), new Vector2(rect.xMin, rect.yMin));
	}

	public static Rect ScaleRect(Rect rect, float multiplier = 1f, float offset = 0f )
	{
		return new Rect(
			rect.xMin - (rect.width * (multiplier - 1f)) / 2 - (offset / 2),
			rect.yMin - (rect.height * (multiplier - 1)) / 2 - (offset / 2),
			rect.width * multiplier + offset,
			rect.height * multiplier + offset
		);
	}

	public static Rect TransfromRectRatio(Rect rect, bool useWidthValue, Vector2 maxSize, Vector2 minSize, Vector2 PreferredSize)
	{
		float newWidth, newHeight;
		
		if (useWidthValue) {
			newWidth = PreferredSize.x;
			newWidth = Mathf.Clamp(newWidth, minSize.x, maxSize.x);
			newHeight = newWidth * PreferredSize.y / PreferredSize.x;
			if (newHeight > maxSize.y || newHeight < minSize.y) {
				newHeight = Mathf.Clamp(newHeight, minSize.y, maxSize.y);
				newWidth = newHeight * PreferredSize.x / PreferredSize.y;
			}
		}
		else {
			newHeight = PreferredSize.y;
			newHeight = Mathf.Clamp(newHeight, minSize.y, maxSize.y);
			newWidth = newHeight * PreferredSize.x / PreferredSize.y;
			if (newWidth > maxSize.x || newWidth < minSize.x) {
				newWidth = Mathf.Clamp(newWidth, minSize.x, maxSize.x);
				newHeight = newWidth * PreferredSize.y / PreferredSize.x;
			}
		}

		return new Rect(
			rect.center.x - newWidth/2,			
			rect.center.y - newHeight/2,			
			newWidth,
			newHeight
		);
	}

	public static void DrawTagListBar(List<string> tagList, ref string addingCache){
		GUILayout.BeginHorizontal(GUILayout.Height(24));
			GUILayout.Space(4);
			GUILayout.Label("Tags:", GUILayout.Width(40));
			for (int i = 0; i < tagList.Count; i++)
			{
				GUILayout.Label(tagList[i], GUILayout.ExpandWidth(false));
				DrawRectOutline(GUILayoutUtility.GetLastRect(), new Color(0.4f, 0.4f, 1f, 1f));
				if (GUILayout.Button("X", GUILayout.Width(16), GUILayout.Height(16)))
				{
					tagList.RemoveAt(i);
				}
				GUILayout.Space(4);
			}
			addingCache = GUILayout.TextField(addingCache, GUILayout.Width(64));
			if (GUILayout.Button("+", GUILayout.Width(16), GUILayout.Height(16)))
			{
				tagList.Add(addingCache);
				addingCache = string.Empty;
			}
		GUILayout.EndHorizontal();
		
	}

	public enum Directions {
		Top,
		Bottom,
		Left,
		Right
	}
	public static void DrawSideBorders (Rect rect) {
		DrawSideBorder(Directions.Top, rect, new Color(0f, 0f, 0f, 0.4f));
		DrawSideBorder(Directions.Bottom, rect, new Color(0f, 0f, 0f, 0.4f));
		DrawSideBorder(Directions.Left, rect, new Color(0f, 0f, 0f, 0.4f));
		DrawSideBorder(Directions.Right, rect, new Color(0f, 0f, 0f, 0.4f));
	}
	
	public static void DrawSideBorders (Rect rect, Color color) {
		DrawSideBorder(Directions.Top, rect, color);
		DrawSideBorder(Directions.Bottom, rect, color);
		DrawSideBorder(Directions.Left, rect, color);
		DrawSideBorder(Directions.Right, rect, color);
	}


	public static void DrawSideBorder (Directions dir, Rect rect) {
		if (rect == null) return;
		DrawSideBorder(dir, rect, new Color(0f, 0f, 0f, 0.4f));
	}

	public static void DrawSideBorder (Directions dir, Rect rect, Color color) {
		Handles.color = color;
		switch (dir) {
			case Directions.Top:
				Handles.DrawLine(new Vector2(rect.xMin, rect.yMin), new Vector2(rect.xMax, rect.yMin));
				break;
			case Directions.Bottom:
				Handles.DrawLine(new Vector2(rect.xMin, rect.yMax), new Vector2(rect.xMax, rect.yMax));
				break;
			case Directions.Left:
				Handles.DrawLine(new Vector2(rect.xMin, rect.yMin), new Vector2(rect.xMin, rect.yMax));
				break;
			case Directions.Right:
				Handles.DrawLine(new Vector2(rect.xMax, rect.yMin), new Vector2(rect.xMax, rect.yMax));
				break;				
		}

	}
	
	public static void DrawShadow (Rect rect, int radius = 6) {
		DrawShadow (Directions.Top, rect, new Color(0f, 0f, 0f, 0.42f), radius);
		DrawShadow (Directions.Bottom, rect, new Color(0f, 0f, 0f, 0.42f), radius);
		DrawShadow (Directions.Left, rect, new Color(0f, 0f, 0f, 0.42f), radius);
		DrawShadow (Directions.Right, rect, new Color(0f, 0f, 0f, 0.42f), radius);
	}


	public static void DrawShadow (Directions dir, Rect rect, int radius = 6) {
		DrawShadow (dir, rect, new Color(0f, 0f, 0f, 0.54f), radius);
	}

	public static void DrawShadow (Directions dir, Rect rect, Color color, int radius = 6) {
		Handles.color = color;
		float[] tempAlpha = new float[radius];
		for (int i = 0; i < radius; i++) {
			tempAlpha[i] = color.a/radius * (i + 1);
		}

		Vector2 start = new Vector2();
		Vector2 end = new Vector2();
		Vector2 modifier = new Vector2();
		switch (dir) {
			case Directions.Top:
				start = new Vector2(rect.xMin, rect.yMin);
				end = new Vector2(rect.xMax, rect.yMin);
				modifier = new Vector2(0, -1);
				break;
			case Directions.Bottom:
				start = new Vector2(rect.xMin, rect.yMax);
				end = new Vector2(rect.xMax, rect.yMax);
				modifier = new Vector2(0, 1);
				break;
			case Directions.Left:
				start = new Vector2(rect.xMin, rect.yMin);
				end = new Vector2(rect.xMin, rect.yMax);
				modifier = new Vector2(-1, 0);
				break;
			case Directions.Right:
				start = new Vector2(rect.xMax, rect.yMin);
				end = new Vector2(rect.xMax, rect.yMax);
				modifier = new Vector2(1, 0);
				break;				
		}

		for (int i = 0; i < radius; i++) {
			Handles.color = new Color(color.r, color.g, color.b, tempAlpha[radius - i - 1]);
			Handles.DrawLine(start + modifier*i, end + modifier*i);
		}

	}
	public static int GetLastControlId()
	{
		System.Reflection.FieldInfo getLastControlId = typeof (EditorGUIUtility).GetField("s_LastControlID", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
		if(getLastControlId != null)
			return (int)getLastControlId.GetValue(null);
		return 0;
	}
#endif
}
