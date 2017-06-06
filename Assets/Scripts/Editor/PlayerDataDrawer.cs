using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(PlayerData))]
public class PlayerDataDrawer : PropertyDrawer {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		// Using BeginProperty / EndProperty on the parent property means that
		// prefab override logic works on the entire property.
		EditorGUI.BeginProperty(position, label, property);

		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

		PlayerData playerData = attribute as PlayerData;

		EditorGUI.TextField(position, (playerData == null ? "Null" :  playerData.ToString()));

		EditorGUI.EndProperty();
	}
}
