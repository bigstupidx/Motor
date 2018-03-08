using System;
using System.Collections.Generic;
using Logging;
using System.Reflection;

namespace CommandModel.AssemblyCommandLoader {

	public class AssemblyCommandLoader<TCommand> : ICommandLoader<TCommand> where TCommand : ICommand {

		public ILog Log;

		public AssemblyCommandLoader() {
			Log = LogFactory.ForContext(this.GetType());
		}

		public Dictionary<object, Type> LoadCommand() {
			Type targetType = typeof(TCommand);
			Dictionary<object, Type> ret = new Dictionary<object, Type>();
			IEnumerable<Assembly> assemblies;
#if COREFX
			assemblies = AppDomain.CurrentDomain.GetAssemblies();
#else
			assemblies = new Assembly[] { GetType().Assembly };//在Unity中只在当前程序集寻找 TODO 应该使用不同的接口实现
#endif
			foreach (var assembly in assemblies) {
				var types = assembly.GetTypes();
				foreach (var type in types) {
#if COREFX
					var typeInfo = type.GetTypeInfo();
					if (typeInfo.IsAbstract) {
						continue;
					}
#else
					if (type.IsAbstract) {
						continue;
					}
#endif
					if (!targetType.IsAssignableFrom(type)) {
						continue;
					}
					var command = (TCommand)Activator.CreateInstance(type);
					if (ret.ContainsKey(command.Key)) {
						this.Log.Warn("Found duplicate command key : " + command.Key + " ,Will skip this");
					} else {
						Log.DebugFormat("find command:" + command.Key);
						ret.Add(command.Key, type);
					}
				}
			}
			return ret;
		}

	}

}