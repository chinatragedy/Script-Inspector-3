# Script-Inspector-3_lua_extension
对Unity插件[Script Inspector 3](https://assetstore.unity.com/packages/tools/visual-scripting/script-inspector-3-3535)的扩展，添加了Lua的规则，支持Lua语言的高亮。（还有待完善...）
## Lua脚本高亮（HighLight）
1. 在Unity Assets Store获取[Script Inspector 3](https://assetstore.unity.com/packages/tools/visual-scripting/script-inspector-3-3535)插件，导入项目中，使用教程见插件使用说明。
2. 将仓库目录中的LuaParser.cs复制到插件目录下的.../Editor/ScriptInspector3/Scripts文件夹中。
3. 在.../Editor/ScriptInspector3/Scripts/FGParser.cs中找到下面的代码块，注册LuaParser。
![最后一行注册LuaParser](https://i.loli.net/2020/11/19/BWAnGzUvHImjgbr.png)

## 护眼主题
1. 把仓库里Themes文件夹下的cs文件拷贝到插件目录...Assets/_ThirdParty/Editor/ScriptInspector3/Themes中下面。

## 增加字体
1. 将要添加的字体包拷贝到...Editor/ScriptInspector3/EditorResources/Fonts文件夹中。
2. 在.../Editor/ScriptInspector3/Scripts/FGTextEditor.cs中找到下面的代码块，添加新字体包的名称。
![图中添加了JetBrainsMono字体](https://i.loli.net/2020/11/19/pQCWRkoFmJYwLTu.png)
