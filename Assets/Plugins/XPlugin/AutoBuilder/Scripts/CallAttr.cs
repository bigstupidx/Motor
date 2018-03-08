
using System;
using System.Collections.Generic;
using System.Reflection;

namespace XPlugin.AutoBuilder {

	public class CallAttr {

		public static void CallAllAttrMethodByOrder(Type type, params object[] parameters) {
			var list = FindAllMethod(type);
			list.Sort((x, y) => { return x.attr.Order - y.attr.Order; });
			foreach (var attrMethoed in list) {
				attrMethoed.method.Invoke(null, parameters);
			}
		}

		private static List<OrderAttrMethod> FindAllMethod(Type type) {
			var types = GetAllSubTypesInScripts();
			var ret = new List<OrderAttrMethod>();
			foreach (var mono in types) {
				MethodInfo[] methods = mono.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (MethodInfo method in methods) {
					if (method.IsDefined(type, false)) {
						OrderableAttr attr = (OrderableAttr)(method.GetCustomAttributes(type, true)[0]);
						ret.Add(new OrderAttrMethod(attr, method));
					}
				}
			}
			return ret;
		}

		private static Type[] GetAllSubTypesInScripts() {
			var result = new List<Type>();
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (var assembly in assemblies) {
				// this skips all but the Unity-scripted assemblies. You could remove this to search all assemblies in project
				if (!assembly.FullName.StartsWith("Assembly-")) {
					continue;
				}
				Type[] types = assembly.GetTypes();
				result.AddRange(types);
			}
			return result.ToArray();
		}
	}
}
