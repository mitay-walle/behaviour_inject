#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

namespace BehaviourInject.Internal
{
#if UNITY_EDITOR
	[CanEditMultipleObjects]
	[CustomEditor(typeof(Injector))]
	public class ChooseContextProperyDrawer : Editor
	{
		private Settings _settings;
		private bool _isDropped;

		private SerializedProperty _choosenContext;
		private SerializedProperty _useHierarchy;

		void OnEnable()
		{
			_settings = Settings.Load();
			_choosenContext = serializedObject.FindProperty("_contextIndex");
			;
			_useHierarchy = serializedObject.FindProperty("_useHierarchy");
			;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			bool useHierarchy = _useHierarchy.boolValue;
			_useHierarchy.boolValue = EditorGUILayout.Toggle("Use hierarchy", useHierarchy);

			if (!useHierarchy)
			{
				string[] contextNames = _settings.ContextNames;
				int contextIndex = _choosenContext.intValue;
				if (contextIndex >= contextNames.Length)
				{
					contextIndex = 0;
					_isDropped = true;
				}

				int index = EditorGUILayout.IntPopup("Context", contextIndex, contextNames, _settings.GetOptionValues());
				_choosenContext.intValue = index;

				if (_isDropped)
				{
					EditorGUILayout.HelpBox("Context index exceeded. Returned to " + contextNames[0], MessageType.Warning);
				}
			}

			if (targets.Length == 1)
			{
				if (target is Injector injector)
				{
					if (injector.GetComponents<Component>()[1] != injector)
					{
						EditorGUILayout.HelpBox("Should be first in components order!", MessageType.Warning);
						if (GUILayout.Button("Make first"))
						{
							var gameObject = injector.gameObject;
							int safetyCounter = 100;
							while (safetyCounter > 0 || gameObject.GetComponents<Component>()[1] != injector)
							{
								safetyCounter--;
								UnityEditorInternal.ComponentUtility.MoveComponentUp(injector);
							}
						}
					}
				}
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
#endif
}
