using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Vuforia;

/// <summary>Builds the lab scene once the Vuforia package is available.</summary>
public static class BuildSolarLabScene // Unity 6 Editor scene builder
{
    private const string PrefabFolder = "Assets/Solar System/Prefabs/";
    private const string MarkerPath = "Assets/Solar System/ImageTargets/SolarSystem.jpg";
    private const string AlienMarkerPath = "Assets/Solar System/ImageTargets/AlienMarker.png";
    private const string ScenePath = "Assets/Scenes/SampleScene.unity";
    private static readonly string[] Planets = { "Sun", "Mercury", "Earth", "Venus", "Mars", "Jupiter", "Saturn", "Uranus", "Neptune" };

    [MenuItem("Solar Lab/Build AR Solar System Scene")]
    public static void Build()
    {
        if (!System.IO.File.Exists("Packages/com.ptc.vuforia.engine-11.4.4.tgz"))
        {
            EditorUtility.DisplayDialog("Vuforia pendiente", "Primero importa el paquete Vuforia Engine 11.4.4 y vuelve a ejecutar esta opción.", "OK");
            return;
        }

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var camera = new GameObject("ARCamera");
        camera.tag = "MainCamera";
        var arCamera = camera.AddComponent<Camera>();
        // Vuforia draws the live camera image behind the 3D scene. A skybox
        // here covers that video background and makes the app look untracked.
        arCamera.clearFlags = CameraClearFlags.SolidColor;
        arCamera.backgroundColor = new Color(0, 0, 0, 0);
        camera.AddComponent<AudioListener>();
        camera.AddComponent<VuforiaBehaviour>();
        camera.AddComponent<DefaultInitializationErrorHandler>();

        var modelRoot = new GameObject("SolarSystemModels").transform;
        foreach (var planet in Planets)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabFolder + planet + ".prefab");
            if (prefab == null) continue;
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, modelRoot);
            instance.name = planet;
            instance.transform.localPosition = new Vector3(0, 0.08f, 0);
            instance.SetActive(planet == "Earth");
        }
        var nav = modelRoot.gameObject.AddComponent<global::MultiTargetVuforia>();
        var serialized = new SerializedObject(nav);
        serialized.FindProperty("modelRoot").objectReferenceValue = modelRoot;
        serialized.FindProperty("startingPlanetIndex").intValue = 2;
        serialized.ApplyModifiedPropertiesWithoutUndo();

        var targetCreator = camera.AddComponent<SolarInstantTarget>();
        var targetSerialized = new SerializedObject(targetCreator);
        targetSerialized.FindProperty("image").objectReferenceValue = AssetDatabase.LoadAssetAtPath<Texture2D>(MarkerPath);
        targetSerialized.FindProperty("companionImage").objectReferenceValue = AssetDatabase.LoadAssetAtPath<Texture2D>(AlienMarkerPath);
        targetSerialized.FindProperty("models").objectReferenceValue = modelRoot;
        targetSerialized.ApplyModifiedPropertiesWithoutUndo();

        var canvasObject = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));

        MakeButton(canvas.transform, nav, "Previo", "Previo", new Vector2(0.22f, 0.12f));
        MakeButton(canvas.transform, nav, "Siguiente", "Siguiente", new Vector2(0.78f, 0.12f));
        EditorSceneManager.SaveScene(scene, ScenePath);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        EditorUtility.DisplayDialog("Escena lista", "Se creó la escena AR. Falta asignar una clave de licencia Vuforia en Vuforia Configuration.", "OK");
        Selection.activeGameObject = camera;
    }

    [MenuItem("Solar Lab/Build Android APK (Solar + Alien markers)")]
    public static void BuildMenuCompanionApk()
    {
        var outputDirectory = System.IO.Path.GetFullPath(System.IO.Path.Combine(Application.dataPath, "../../outputs"));
        System.IO.Directory.CreateDirectory(outputDirectory);
        var options = new BuildPlayerOptions
        {
            scenes = new[] { ScenePath },
            locationPathName = System.IO.Path.Combine(outputDirectory, "SolarSystem_MenuCompanion.apk"),
            target = BuildTarget.Android,
            options = BuildOptions.None
        };
        var report = BuildPipeline.BuildPlayer(options);
        Debug.Log($"Android build result: {report.summary.result}; APK: {options.locationPathName}");
        if (report.summary.result != BuildResult.Succeeded)
            throw new System.Exception("La compilación Android no terminó correctamente: " + report.summary.result);
    }

    private static void MakeButton(Transform parent, global::MultiTargetVuforia nav, string name, string label, Vector2 anchor)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(UnityEngine.UI.Image), typeof(Button));
        go.transform.SetParent(parent, false);
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = anchor;
        rect.sizeDelta = new Vector2(220, 76);
        var image = go.GetComponent<UnityEngine.UI.Image>();
        image.color = new Color(0.08f, 0.3f, 0.58f, 0.9f);
        var button = go.GetComponent<Button>();
        button.onClick.AddListener(label == "Siguiente" ? nav.Siguiente : nav.Previo);
        var textGo = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        textGo.transform.SetParent(go.transform, false);
        var textRect = textGo.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero; textRect.anchorMax = Vector2.one;
        textRect.offsetMin = textRect.offsetMax = Vector2.zero;
        var text = textGo.GetComponent<Text>();
        text.text = label; text.alignment = TextAnchor.MiddleCenter; text.color = Color.white;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); text.fontSize = 28;
    }
}
