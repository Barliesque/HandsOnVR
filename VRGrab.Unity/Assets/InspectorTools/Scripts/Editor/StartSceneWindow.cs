using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;


namespace Barliesque.InspectorTools.Editor
{
	///
	/// <summary>
	/// A handy dandy window that lets you choose which scene Unity begins with when the play button has been pressed.
	/// This allows you to work on any scene, and then test the app from the launcher scene without having to save and close
	/// the scene you're working on.
	/// </summary> 
	/// 
	/// AUTHOR: 
	/// - David Barlia
	/// 
	public class StartSceneWindow : EditorWindow
	{

		SceneAsset _startScene;
		SceneAsset _defaultScene;
		const string DEFAULT_SCENE_KEY = "StartSceneWindow/DefaultScenePath";
		const string START_SCENE_KEY = "StartSceneWindow/StartScenePath";
		bool _showDefault = true;


		[MenuItem("Tools/Start Scene", false, 1)]
		static void Open()
		{
			GetWindow<StartSceneWindow>();
		}


		void Awake()
		{
			var defaultScenePath = EditorPrefs.GetString(DEFAULT_SCENE_KEY);
			if (!string.IsNullOrEmpty(defaultScenePath)) {
				_defaultScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(defaultScenePath);
			}

			var startScenePath = EditorPrefs.GetString(START_SCENE_KEY);
			if (!string.IsNullOrEmpty(startScenePath)) {
				_startScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(startScenePath);
			}
		}


		void OnGUI()
		{
			titleContent = new GUIContent("Start Scene");
			EditorGUILayout.Space();
			EditorGUILayout.HelpBox("Select a scene to be loaded when pressing the Play button.  If set, all currently open scenes will be temporarily unloaded while in play mode, and this scene will be played instead.", MessageType.Info);

			EditorGUI.BeginChangeCheck();
			_startScene = (SceneAsset)EditorGUILayout.ObjectField(new GUIContent("Start Scene"), _startScene, typeof(SceneAsset), false);
			if (EditorGUI.EndChangeCheck()) {
				var startScenePath = AssetDatabase.GetAssetPath(_startScene);
				EditorPrefs.SetString(START_SCENE_KEY, startScenePath);
			}

			EditorGUILayout.BeginHorizontal();

			_showDefault = EditorTools.ToggleButton(_showDefault, "Set Default Scene");

			if (GUILayout.Button("Use Default Scene", GUI.skin.GetStyle("Button"))) {
				var defaultScenePath = EditorPrefs.GetString(DEFAULT_SCENE_KEY);
				_startScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(defaultScenePath);
				EditorPrefs.SetString(START_SCENE_KEY, defaultScenePath);
			}
			if (GUILayout.Button(new GUIContent("Clear", "Clear the start scene to use whatever scene(s) are currently open."))) {
				_startScene = null;
				EditorPrefs.SetString(START_SCENE_KEY, string.Empty);
			}
			EditorGUILayout.EndHorizontal();

			if (_showDefault) {
				EditorGUILayout.Space();
				EditorGUILayout.HelpBox("Specify a default scene to easily re-select it with the button below.", MessageType.Info);

				EditorGUI.BeginChangeCheck();
				_defaultScene = (SceneAsset)EditorGUILayout.ObjectField(new GUIContent("Default Scene"), _defaultScene, typeof(SceneAsset), false);
				if (EditorGUI.EndChangeCheck()) {
					var defaultScenePath = AssetDatabase.GetAssetPath(_defaultScene);
					EditorPrefs.SetString(DEFAULT_SCENE_KEY, defaultScenePath);
				}
			}
			
			EditorSceneManager.playModeStartScene = _startScene;
		}

	}

}
