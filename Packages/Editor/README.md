# VRUGD Cluster Setup Tools (Test)

Unity プロジェクトに Cluster Creator Kit の導入・設定を全自動で行うエディタ拡張です。

## 導入方法
- Scoped Registry の自動追加  
- mu.cluster.cluster-creator-kit の自動導入  
- Color Space を Linear に設定  
- 初回起動時にセットアップ確認ダイアログ表示  
- メニューから再セットアップ可能（VRUGD → Cluster SetupTools）

## 🚀 使い方

1. `Packages/manifest.json` に次を追加します：

   ```json
   "dependencies": {
     "com.vrugd.cluster.setup": "https://github.com/VKetDeveloper/ClusterSetup.git"
   }

