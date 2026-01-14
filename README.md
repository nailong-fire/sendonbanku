## 项目简介

《sendonbanku》是一款基于 Unity 制作的 2D 项目，采用 URP 渲染管线，包含地图探索与卡牌战斗场景。本说明主要阐述如何在unity引擎中游玩本项目。

本项目没有使用任何模板，游戏中人物背景，人物形象为组员余星晨手绘，其余图片为组员集体AI生成。项目代码没有使用任何初始模板，为本组成员从零搭建。

本项目在github上有备份，链接为[https://github.com/nailong-fire/sendonbanku](https://github.com/nailong-fire/sendonbanku)

## 环境需求

- Unity 版本：2022.3.62f2c1（LTS），请在 Unity Hub 中安装对应版本。
- 渲染管线：Universal Render Pipeline（URP），相关配置位于 [Assets/UniversalRenderPipelineGlobalSettings.asset](Assets/UniversalRenderPipelineGlobalSettings.asset) 与 [Assets/Settings/UniversalRP.asset](Assets/Settings/UniversalRP.asset)。
- 推荐：安装 TextMesh Pro（项目已包含资源）、2D Tilemap Editor（用于地图编辑）。

## 导入步骤（Unity Hub）

1) 打开 Unity Hub，选择 “Open”/“Add”。
2) 指向项目根目录 `sendonbanku`（本文件所在文件夹）。
3) 选择 Unity 2022.3.62f2c1 打开；首次导入会自动解析包与资源，等待进度完成(约1-2min)。
4) 若提示升级 API，保持默认（不建议更改）。

## 游玩方式

**[Assets/Scenes/start.unity](Assets/Scenes/start.unity)（标题/入口）**

在unity hub打开sendonbanku项目后，于project界面根据上午文件地址找到对应的“start”Scene文件，双击打开后点击位于窗口正中上方的开始按钮即可开始游玩。

## 运行入口与场景

- 启动场景： [Assets/Scenes/start.unity](Assets/Scenes/start.unity)（标题/入口）。
- 地图探索： [Assets/Scenes/map.unity](Assets/Scenes/map.unity) 与 [Assets/Scenes/worldmap.unity](Assets/Scenes/worldmap.unity)。
- 卡牌战斗： [Assets/Scenes/cardbattle.unity](Assets/Scenes/cardbattle.unity)。
- 调试示例： [Assets/Scenes/test.unity](Assets/Scenes/test.unity)。

在 Unity 中双击场景后点击播放即可体验。若需要直接构建，确保构建列表已包含上述核心场景（File > Build Settings）。

```
这里主要展示我们的不同的Scene的详细搭建结构，正常游玩请见上节
```

## 资源与目录速览

- 角色与动画： [Assets/Animation](Assets/Animation) 及 [Assets/Player](Assets/Player)。
- 卡牌数据与预制体： [Assets/Cards](Assets/Cards) 与 [Assets/Prefabs/cardprefab.prefab](Assets/Prefabs/cardprefab.prefab)。
- 场景与地图资源： [Assets/Scenes](Assets/Scenes) 与 [Assets/Pictures/maps](Assets/Pictures/maps)。
- UI 与字体： [Assets/Scripts/UI](Assets/Scripts/UI) 与 TextMesh Pro 资源位于 [Assets/TextMesh Pro](Assets/TextMesh Pro)。
- 脚本分层：位于 [Assets/Scripts](Assets/Scripts)，按功能拆分 `game`、`map`、`card`、`dialog` 等子目录。

## 构建与播放设置

- 平台：默认 PC, Mac & Linux Standalone，可在 Build Settings 中切换目标平台。
- 分辨率/质量：质量设置位于 Project Settings > Quality；URP 设置见 [Assets/Settings/Renderer2D.asset](Assets/Settings/Renderer2D.asset)。
- 音频：音轨在 [Assets/Audio](Assets/Audio)，播放脚本位于 [Assets/Scripts/Audio](Assets/Scripts/Audio)。

## 常见问题

- 材质或光照异常：确认项目使用 URP 模板，若材质丢失可右键材质选择 “Reimport”。
- 文本缺字：确保 TextMesh Pro 导入完成；必要时在 Window > TextMeshPro > Font Asset Creator 重新生成字体资产。
- 脚本报错：确认使用 2022.3.62f2c1；若有包缺失，可在 Package Manager 中点击 “Resolve” 或 “Add from disk” 对应 `Packages/manifest.json`。

## 团队交付

- 提交内容：完整 `sendonbanku` 项目文件夹（含 Assets、Packages、ProjectSettings）。
- 审阅方式：按上述导入步骤在 Unity 打开，运行 start 场景即可验证核心流程。
