using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ilsFramework.Core.Editor
{
    public static class UIViewGenerator
    {
        public static Dictionary<string, Type> NeedBindComponents = new Dictionary<string, Type>()
        {
            ["btn"] = typeof(Button),
            ["trans"] = typeof(RectTransform),
            ["img"] = typeof(Image),
            ["text"] = typeof(TextMeshProUGUI),
            ["input"]= typeof(TMP_InputField),
            ["scroll"]= typeof(ScrollRect),
            ["canv"]= typeof(Canvas),
            ["go"]= typeof(GameObject),
        };
        [MenuItem("Assets/生成UIView文件", true,priority = -1)]
        public static bool ValidateGenerateUIView()
        {
            return Selection.activeGameObject;
        }
        [MenuItem("Assets/生成UIView文件", false,priority = -1)]
        public static void GenerateUIView()
        {
            if (PrefabUtility.GetPrefabAssetType(Selection.activeGameObject) == PrefabAssetType.Regular)
            {
                foreach (var binderTuple in GetAllExtraBinder())
                {
                    NeedBindComponents[binderTuple.Item1] = binderTuple.Item2;
                }
                
                string className = Selection.activeGameObject.name;
                string folder = Config.GetConfigInEditor<UIConfig>().UIViewCSharpFileFolderPath;
                string _namespace = Config.GetConfigInEditor<UIConfig>().UIViewCSharpNamespace;
                string scriptPath = $"{folder}/{className}.cs";
                List<ComponentInfo> componentInfos = new List<ComponentInfo>();
                HashSet<string> needNamespace = new HashSet<string>();
                GetRequireGenerateFieldInfoInTransform(Selection.activeGameObject.transform, "", ref componentInfos,ref needNamespace);

                string code = null;
                // 使用Roslyn构建语法树
                if (File.Exists(scriptPath))
                {
                    code = UpdateCodeGeneratePath(scriptPath,className,componentInfos, needNamespace);
                }
                else
                {
                    code = GenerateClassCode(AssetDatabase.GetAssetPath(Selection.activeGameObject),_namespace,className,componentInfos, needNamespace);
                }


                // 生成代码字符串
                scriptPath.LogSelf();
                // 确保目录存在
                string directory = Path.GetDirectoryName(scriptPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // 写入文件
                File.WriteAllText(scriptPath, code);

                // 刷新AssetDatabase
                AssetDatabase.Refresh();
            }
        }

        private static List<(string, Type)> GetAllExtraBinder()
        {
            var result = new List<(string, Type)>();
            var assemblieNames = new HashSet<string>() { "Framework", "Game" };
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where((assembly) => assemblieNames.Contains(assembly.GetName().Name));
            List<Type> types = new List<Type>();
            foreach (var assembly in assemblies)
            {
                types.AddRange(assembly.GetTypes());
            }

            foreach (var type in types)
            {
                if (type.GetCustomAttribute(typeof(UIBinder)) is UIBinder binder)
                {
                    result.Add((binder.key, type));
                }
            }
            return result;
        }
        public static void GetRequireGenerateFieldInfoInTransform(Transform transform, string path, ref List<ComponentInfo> componentInfos,ref HashSet<string> needNameSpace)
        {

            string currentPath = string.IsNullOrEmpty(path) ? transform.name : $"{path}/{transform.name}";
            if (transform.parent == null)
            {
                currentPath = "";
            }
            // 检查是否有组件标记 [xx]
            int finalEndIndex = -1;
            List<ComponentInfo> componentInfoBuffer = new List<ComponentInfo>();
            if (transform.name.Contains("["))
            {
                int startIndex = transform.name.IndexOf("[");
                int endIndex = transform.name.IndexOf("]");
                finalEndIndex = endIndex;
                if (startIndex < endIndex && endIndex > 0)
                {
                    string typeName = transform.name.Substring(startIndex + 1, endIndex - startIndex - 1).ToLower();
                    foreach (var s in typeName.Split(','))
                    {
                        if (NeedBindComponents.TryGetValue(s, out var type))
                        {
                            componentInfoBuffer.Add(new ComponentInfo(type, currentPath, s + "_"));
                            needNameSpace.Add(type.Namespace);
                        }
                    }
                }
            }

            string fieldName = transform.name.Substring(finalEndIndex + 1).Trim();
            componentInfoBuffer.ForEach((info) => info.FieldName = info.FieldName + fieldName);
            componentInfos.AddRange(componentInfoBuffer);

            // 递归分析子物体
            for (int i = 0; i < transform.childCount; i++)
            {
                GetRequireGenerateFieldInfoInTransform(transform.GetChild(i), currentPath, ref componentInfos,ref needNameSpace);
            }
        }
        

 
        private static string GenerateClassCode(string prefabPath,string _namespace,string name,List<ComponentInfo> components,HashSet<string> needNameSpace)
        {
            StringBuilder sb = new StringBuilder();
        
            // 添加命名空间和using语句
            sb.AppendLine("using ilsFramework.Core;");
            sb.AppendLine();
            
            sb.AppendLine($"namespace {_namespace}");
            sb.AppendLine("{");
            
            sb.Append(GetGenerateCode(name, components, needNameSpace));
            
            sb.AppendLine();
            sb.AppendLine($"    public partial class {name} : UIView");
            sb.AppendLine("    {");
            
            sb.AppendLine();
            sb.AppendLine("        public override EUILayer UILayer { get; }");
            sb.AppendLine();
            
            sb.AppendLine();
            sb.AppendLine("        public override string PackageName { get; }");
            sb.AppendLine();
        
       
            sb.AppendLine();
            sb.AppendLine($"         public override string UIPath =>\"{prefabPath}\";");
            sb.AppendLine();
       
            sb.AppendLine();
            sb.AppendLine($"         public override int LayerOffest {{ get; }}");
            sb.AppendLine();
        
            sb.AppendLine("        public override void ScriptedOnLoad()");
            sb.AppendLine("        {");
            sb.AppendLine();
            sb.AppendLine("        }");
            sb.AppendLine();
            
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        public static string UpdateCodeGeneratePath(string filePath,string name,List<ComponentInfo> components,HashSet<string> needNameSpace)
        {
            string oldCode = File.ReadAllText(filePath);
            
            //提取除了自动生成以外的所有部分
            int startIndex = oldCode.IndexOf("#region AutoGenerate");
            int endIndex = oldCode.IndexOf("#endregion");
            string previousCode = oldCode.Substring(0, startIndex);
            string postCode = oldCode.Substring(endIndex + "#endregion".Length);
            //生成代码
            
            StringBuilder sb = new StringBuilder();
            sb.Append(previousCode);
            sb.Append(GetGenerateCode(name, components, needNameSpace));
            sb.Append(postCode);
            return sb.ToString();
        }

        public static string GetGenerateCode(string name, List<ComponentInfo> components, HashSet<string> needNameSpace)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("#region AutoGenerate");
            foreach (var nameSpace in needNameSpace)
            {
                sb.AppendLine($"    using {nameSpace};");
            }
            sb.AppendLine();

        
            // 开始类定义
            sb.AppendLine($"    public partial class {name} : UIView");
            sb.AppendLine("    {");
        
            // 添加字段定义
            foreach (var component in components)
            {
                sb.AppendLine($"        public {component.ComponentType} {component.FieldName};");
            }
            sb.AppendLine();
        
            // 添加OnLoad方法
            sb.AppendLine("        public override void AutoGenerateOnLoad()");
            sb.AppendLine("        {");
            sb.AppendLine("            base.AutoGenerateOnLoad();");
            sb.AppendLine();
        
            foreach (var component in components)
            {
                sb.AppendLine($"            {component.FieldName} = UIPanelObject.transform.Find(\"{component.Path}\").GetComponent<{component.ComponentType}>();");
            }
        
            sb.AppendLine("        }");
            sb.AppendLine();
            
            // 结束类定义
            sb.AppendLine("    }");
            sb.AppendLine("#endregion");
            return sb.ToString();
        }
    }
    


    public class ComponentInfo
    {
        public ComponentInfo(Type type, string path,string fieldName)
        {
            ComponentType = type;
            Path = path;
            FieldName = fieldName;
        }
        
        public Type ComponentType { get; set; }
        public string FieldName { get; set; }
        public string Path { get; set; }

        public override string ToString()
        {
            return $"{FieldName}:{Path}:{ComponentType.Name}";
        }
    }
}