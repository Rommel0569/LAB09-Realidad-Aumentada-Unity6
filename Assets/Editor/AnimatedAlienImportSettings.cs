using UnityEditor;
using UnityEngine;

/// <summary>Imports the bundled Animated Alien's FBX animation clips as Legacy clips.</summary>
public sealed class AnimatedAlienImportSettings : AssetPostprocessor
{
    private const string ModelPath = "Assets/Resources/AnimatedAlien.fbx";

    [InitializeOnLoadMethod]
    private static void ConfigureExistingModel()
    {
        EditorApplication.delayCall += () =>
        {
            var importer = AssetImporter.GetAtPath(ModelPath) as ModelImporter;
            if (importer == null) return;
            if (importer.animationType != ModelImporterAnimationType.Legacy || !importer.importAnimation)
            {
                importer.animationType = ModelImporterAnimationType.Legacy;
                importer.importAnimation = true;
                importer.SaveAndReimport();
            }
        };
    }

    private void OnPreprocessModel()
    {
        if (assetPath != ModelPath) return;
        var importer = (ModelImporter)assetImporter;
        importer.animationType = ModelImporterAnimationType.Legacy;
        importer.importAnimation = true;
        importer.materialImportMode = ModelImporterMaterialImportMode.ImportStandard;
    }
}
