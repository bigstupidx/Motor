using System;
using System.Reflection;

namespace XPlugin.AutoBuilder {

	public class OrderAttrMethod {
		public MethodInfo method;
		public OrderableAttr attr;

		public OrderAttrMethod(OrderableAttr attr, MethodInfo method) {
			this.method = method;
			this.attr = attr;
		}
	}

	public class OrderableAttr : Attribute {
		public int Order;

		public OrderableAttr(int order) {
			this.Order = order;
		}
	}

	public class OnSingleBuildStartAttr : OrderableAttr {
		public OnSingleBuildStartAttr(int order = 0)
			: base(order) {
		}
	}

	public class OnSingleBuildFinishAttr : OrderableAttr {
		public OnSingleBuildFinishAttr(int order = 0)
			: base(order) {
		}
	}




}
