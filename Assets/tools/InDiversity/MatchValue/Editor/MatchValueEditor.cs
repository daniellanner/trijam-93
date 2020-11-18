using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

[CustomEditor(typeof(MatchValue))]
public class MatchValueEditor : Editor
{
	SerializedProperty lookAtPoint;

	private List<string> _compname = new List<string>();
	private List<Type> _comptype = new List<Type>();

	private int _compSelection = 0;

	private List<string> _propname = new List<string>();
	private int _propSelection = 0;

	void OnEnable()
	{
		lookAtPoint = serializedObject.FindProperty("_toTrack");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		QueryComponents();
		_compSelection = EditorGUILayout.Popup("Component", _compSelection, _compname.ToArray());

		QueryProperties();
		_propSelection = EditorGUILayout.Popup("Property", _propSelection, _propname.ToArray());

		EditorGUILayout.PropertyField(lookAtPoint);
		serializedObject.ApplyModifiedProperties();
	}

	private void QueryComponents()
	{
		var go = ((MatchValue)target).gameObject;

		_compname.Clear();
		_comptype.Clear();
		var comps = go.GetComponents<Component>();

		comps.ToList().ForEach(it => { _comptype.Add(it.GetType()); _compname.Add(it.GetType().Name); });
	}

	private void QueryProperties()
	{
		var type = _comptype[_compSelection];

		_propname.Clear();
		type.GetProperties().ToList().ForEach(it => _propname.Add(it.Name));
	}
}
