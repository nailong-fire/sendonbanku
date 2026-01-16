## 项目简介

《sendonbanku》是一款基于 Unity 制作的 2D 项目，采用 URP 渲染管线，包含地图探索与卡牌战斗场景。本说明主要阐述如何在unity引擎中游玩本项目。

本项目没有使用任何模板，游戏中所有图片为组员余星晨手绘，动画为组员使用unity animation组件完成。项目代码没有使用任何初始模板，为本组成员从零搭建。

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

**[Assets/Scenes/Initial.unity](Assets/Scenes/Initial.unity)（标题/入口）**

在unity hub打开sendonbanku项目后，于project界面根据上午文件地址找到对应的“Initial”Scene文件，双击打开后点击位于窗口正中上方的开始按钮即可开始游玩。

## 玩法简介

本游戏采用 **2D 俯视探索 + 卡牌对战** 的混合核心玩法，玩家需要在广阔的地图中探索剧情，并通过策略卡牌战斗击败强敌。

### 1. 探索模式 (Exploration)
- **移动控制**：使用键盘 **A / D** 或 **方向键** 控制角色在像素风格的 2D 地图上自由移动。
- **交互系统**：当接近 NPC 或可交互物体时，按下 **E 键** 即可进行对话、领取任务或触发剧情事件。
- **叙事体验**：游戏包含完整的对话系统与实时过场动画（Cutscenes），通过与关键角色（如村长等）互动推动故事走向。

### 2. 卡牌战斗 (Card Battle)
- **战斗类型**：回合制策略卡牌对战（Turn-based Card Game）。
- **战场策略**：
  - **站位机制**：战场分为 **前排** 与 **后排**，不同位置可能影响卡牌的攻击范围或受击优先级。
  - **卡牌属性**：每张卡牌拥有独立的 **生命值 (Health)**、**攻击力 (Power)**、**速度 (Speed)** 与 **费用 (Cost)**。
- **回合流程**：
  1.  **部署阶段**：玩家消耗资源将手牌中的**Faith**单位部署到战场上的战术位置，或点击牌面后点击弃牌区将牌弃置。完成操作后点击空格键将回合结束。
  2.  **战斗阶段**：所有单位根据 **速度 (Speed)** 属性决定行动顺序，自动执行攻击或技能。单位在血量清空后会自行消失
  3. **回合判定**：当一方的出牌区无牌时，角色的**Hope**会降低，**Hope**清零视为死亡，
  4.  **结算**：当一方满足胜利条件时战斗结束。

具体卡牌信息见 [卡牌全集.md](卡牌全集.md)

### 3. 游戏特色
- **手绘美术**：全游戏角色与场景均为组员手绘制作。
- **沉浸反馈**：包含丰富的音效（如环境音、打击音效）与视觉特效（卡牌受伤震动、变色反馈等）。

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
- 审阅方式：按上述导入步骤在 Unity 打开，运行Initial场景即可验证核心流程。
