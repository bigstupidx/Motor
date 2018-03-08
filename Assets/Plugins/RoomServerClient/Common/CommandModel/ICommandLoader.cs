// Author:
// [LongTianhong]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using System;
using System.Collections.Generic;

namespace CommandModel {

	public interface ICommandLoader<TCommand> where TCommand : ICommand {

		Dictionary<object, Type> LoadCommand();
	}
}