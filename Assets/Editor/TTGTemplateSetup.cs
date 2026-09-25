using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using TripleTapGames.Foundation;

public static class TTGTemplateSetup
{
    public static void Create()
    {
        PlayerSettings.companyName = "Triple Tap Games";
        PlayerSettings.productName = "TTG Unity Template";
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.tripletapgames.template");
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, "com.tripletapgames.template");
        Directory.CreateDirectory("Assets/Scenes");
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Main.unity");
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene("Assets/Scenes/Main.unity", true) };
        Directory.CreateDirectory("Assets/TTGGenerated/Resources/TTG");
        AssetDatabase.Refresh();
        const string root = "Assets/TTGGenerated/Resources/TTG/";
        if (!File.Exists(root + "TTGProjectConfig.asset"))
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<TTGProjectConfig>(), root + "TTGProjectConfig.asset");
        if (!File.Exists(root + "TTAdsConfig.asset"))
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<TTAdsConfig>(), root + "TTAdsConfig.asset");
        AssetDatabase.SaveAssets();
        const string defines = "TTG_FACEBOOK;TTG_GAMEANALYTICS;TTG_SINGULAR;TTG_IAP";
        foreach (var group in new[] { BuildTargetGroup.Standalone, BuildTargetGroup.Android, BuildTargetGroup.iOS })
            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, defines);
        Debug.Log("TTG template created: SDKs installed, services disabled until configured.");
    }
}
