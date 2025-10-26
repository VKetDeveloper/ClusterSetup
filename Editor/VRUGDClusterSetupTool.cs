using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;

[InitializeOnLoad]
public static class VRUGDClusterSetupTool
{
    private const string SetupKey = "VRUGDClusterSetupDone";

    static VRUGDClusterSetupTool()
    {
        // Unity起動後に一度だけ実行
        EditorApplication.update += OnEditorStartup;
    }

    private static void OnEditorStartup()
    {
        EditorApplication.update -= OnEditorStartup;

        if (!EditorPrefs.GetBool(SetupKey, false))
        {
            bool result = EditorUtility.DisplayDialog(
                "VRUGD Cluster セットアップ",
                "VRUGD Cluster 開発環境をセットアップしますか？\n" +
                "Scoped Registries / 必要パッケージ / Linear 設定を自動で行います。",
                "はい（セットアップ）", "いいえ");

            if (result)
            {
                RunSetupWithProgress();
                EditorPrefs.SetBool(SetupKey, true);
                EditorUtility.DisplayDialog("完了", "VRUGD Cluster のセットアップが完了しました！", "OK");
            }
            else
            {
                Debug.Log("VRUGD Cluster セットアップをスキップしました。");
            }
        }
    }

    [MenuItem("VRUGD/Cluster SetupTools/セットアップを実行")]
    public static void RunSetupWithProgress()
    {
        EditorUtility.DisplayProgressBar("VRUGD Cluster セットアップ中", "初期化しています...", 0.0f);

        try
        {
            string manifestPath = Path.Combine(Application.dataPath, "../Packages/manifest.json");
            if (!File.Exists(manifestPath))
            {
                EditorUtility.ClearProgressBar();
                Debug.LogError("manifest.json が見つかりません。");
                return;
            }

            string json = File.ReadAllText(manifestPath);
            var jObj = JObject.Parse(json);
            var deps = jObj["dependencies"] as JObject;
            var scopedRegs = jObj["scopedRegistries"] as JArray ?? new JArray();
            jObj["scopedRegistries"] = scopedRegs;

            float step = 0f;

            // 1️⃣ Newtonsoft.Json
            EditorUtility.DisplayProgressBar("VRUGD Cluster セットアップ中", "Newtonsoft.Json を追加しています...", step += 0.25f);
            if (deps["com.unity.nuget.newtonsoft-json"] == null)
            {
                deps["com.unity.nuget.newtonsoft-json"] = "3.2.1";
                Debug.Log("📦 Newtonsoft.Json を追加しました。");
            }

            // 2️⃣ Scoped Registry
            EditorUtility.DisplayProgressBar("VRUGD Cluster セットアップ中", "Scoped Registry を追加しています...", step += 0.25f);
            bool hasRegistry = scopedRegs.Any(r => r["name"]?.ToString() == "Cluster");
            if (!hasRegistry)
            {
                var newReg = new JObject
                {
                    ["name"] = "Cluster",
                    ["url"] = "https://registry.npmjs.com",
                    ["scopes"] = new JArray("mu.cluster.cluster-creator-kit")
                };
                scopedRegs.Add(newReg);
                Debug.Log("✅ Scoped Registry を追加しました。");
            }

            // 3️⃣ mu.cluster.cluster-creator-kit
            EditorUtility.DisplayProgressBar("VRUGD Cluster セットアップ中", "Cluster Creator Kit を追加しています...", step += 0.25f);
            if (deps["mu.cluster.cluster-creator-kit"] == null)
            {
                deps["mu.cluster.cluster-creator-kit"] = "latest";
                Debug.Log("✅ mu.cluster.cluster-creator-kit を dependencies に追加しました。");
            }

            // manifest.json 保存
            File.WriteAllText(manifestPath, jObj.ToString());
            AssetDatabase.Refresh();

            // 4️⃣ Color Space 設定
            EditorUtility.DisplayProgressBar("VRUGD Cluster セットアップ中", "Color Space を Linear に変更しています...", step += 0.25f);
            if (PlayerSettings.colorSpace != ColorSpace.Linear)
            {
                PlayerSettings.colorSpace = ColorSpace.Linear;
                Debug.Log("✅ Color Space を Linear に設定しました。");
            }
            else
            {
                Debug.Log("ℹ️ すでに Color Space は Linear です。");
            }

            Debug.Log("🎉 VRUGD Cluster セットアップ完了！");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("VRUGD Cluster セットアップ中にエラーが発生しました: " + ex.Message);
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }
}
