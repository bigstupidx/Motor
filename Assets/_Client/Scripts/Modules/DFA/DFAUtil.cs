//
// DFA.cs
//
// Author:
// [tanjie]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using System;
using System.Collections.Generic;
using System.Text;

namespace GameClient {
	public class DFAUtil {
		private static int a = 0;
		private static bool isInit = false;
		private static char replaySymbol = '*';
		private static Node rootNode = new Node ('R');
		private static List<string> word = new List<string> ();

		public static void Init (string text) {
			string[] separator = null;
			separator = new string[] { ",", "\n", "\r" };
			createTree (text.Split (separator, StringSplitOptions.RemoveEmptyEntries));
			isInit = true;
		}

		private static void createTree (string[] arr) {
			foreach (string str in arr) {
				char[] cs = str.Trim ().ToLower ().ToCharArray ();
				if (cs.Length > 0) {
					insertNode (rootNode, cs, 0);
				}
			}
		}

		public static string FilterWords (string input, out bool isLimit) {
			isLimit = false;
			if (!isInit) {
				return input;
			}
			a = 0;
			word.Clear ();
			char[] chArray = input.ToCharArray ();
			Node rootNode = DFAUtil.rootNode;
			StringBuilder builder = new StringBuilder ();
			while (a < chArray.Length) {
				char c = chArray [a];
				if (char.IsUpper (c)) {
					c = c.ToString ().ToLower () [0];
				}
				rootNode = findNode (rootNode, c);
				if (rootNode == null) {
					rootNode = DFAUtil.rootNode;
					word.Clear ();
					rootNode = findNode (rootNode, c);
					if (rootNode == null) {
						builder.Append (chArray [a]);
						rootNode = DFAUtil.rootNode;
					} else {
						word.Add (chArray [0].ToString ());
						builder.Append (chArray [a]);
					}
				} else if (rootNode.flag == 1) {
					word.Add (chArray [a].ToString ());
					builder.Append (chArray [a]);
					StringBuilder builder2 = new StringBuilder ();
					builder2.Append (replaySymbol);
					builder.Remove (builder.Length - word.Count, word.Count);
					builder.Append (builder2.ToString ());
					word.Clear ();
					rootNode = DFAUtil.rootNode;
					isLimit = true;
				} else {
					word.Add (chArray [a].ToString ());
					builder.Append (chArray [a]);
				}
				a++;
			}
			return builder.ToString ();
		}

		private static Node findNode (Node node, char c) {
			List<Node> nodes = node.nodes;
			foreach (Node node3 in nodes) {
				if (node3.c == c) {
					return node3;
				}
			}
			return null;
		}

		private static void insertNode (Node node, char[] cs, int index) {
			Node item = findNode (node, cs [index]);
			if (item == null) {
				item = new Node (cs [index]);
				node.nodes.Add (item);
			}
			if (index == (cs.Length - 1)) {
				item.flag = 1;
			}
			index++;
			if (index < cs.Length) {
				insertNode (item, cs, index);
			}
		}

		public class Node {
			public char c;
			public int flag;
			public List<DFAUtil.Node> nodes;

			public Node (char c) {
				this.nodes = new List<DFAUtil.Node> ();
				this.c = c;
				this.flag = 0;
			}

			public Node (char c, int flag) {
				this.nodes = new List<DFAUtil.Node> ();
				this.c = c;
				this.flag = flag;
			}
		}
	}

}