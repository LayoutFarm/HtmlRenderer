//
// Program.cs
//
// Author: Jeffrey Stedfast <jeff@xamarin.com>
//
// Copyright (c) 2015 Xamarin Inc. (www.xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using System;
using System.IO;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace CodeGenerator {
	class GraphNode
	{
		public readonly List<GraphNode> Children = new List<GraphNode> ();
		public readonly string Name;
		public readonly int State;
		public readonly char Char;
		public string Value;

		public GraphNode (GraphNode parent, int state, char c)
		{
			State = state;
			Char = c;

			if (parent != null) {
				parent.Children.Add (this);
				Name = parent.Name + c;
			} else {
				Name = "&";
			}
		}
	}

	class InnerSwitchLabel
	{
		public readonly int CurrentState;
		public readonly int NextState;
		public readonly string Comment;

		public InnerSwitchLabel (int current, int next, string comment)
		{
			CurrentState = current;
			NextState = next;
			Comment = comment;
		}
	}

	class Program
	{
		static readonly SortedDictionary<char, SortedDictionary<int, InnerSwitchLabel>> OuterSwitchLabels = new SortedDictionary<char, SortedDictionary<int, InnerSwitchLabel>> ();
		static readonly SortedDictionary<int, GraphNode> FinalStates = new SortedDictionary<int, GraphNode> ();
		static readonly GraphNode Root = new GraphNode (null, 0, '\0');

		public static void Main (string[] args)
		{
			int maxEntityLength = 0;
			int state = 0;

			using (var json = new JsonTextReader (new StreamReader ("HtmlEntities.json"))) {
				while (json.Read ()) {
					string name, value;

					if (json.TokenType == JsonToken.StartObject)
						continue;

					if (json.TokenType != JsonToken.PropertyName)
						break;

					name = (string) json.Value;

					// trim leading '&' and trailing ';'
					name = name.TrimStart ('&').TrimEnd (';');

					if (!json.Read () || json.TokenType != JsonToken.StartObject)
						break;

					// read to the "codepoints" property
					if (!json.Read () || json.TokenType != JsonToken.PropertyName)
						break;

					// skip the array of integers...
					if (!json.Read () || json.TokenType != JsonToken.StartArray)
						break;

					while (json.Read ()) {
						if (json.TokenType == JsonToken.EndArray)
							break;
					}

					// the property should be "characters" - this is what we want
					if (!json.Read () || json.TokenType != JsonToken.PropertyName)
						break;

					value = json.ReadAsString ();

					var node = Root;

					for (int i = 0; i < name.Length; i++) {
						bool found = false;

						for (int j = 0; j < node.Children.Count; j++) {
							if (node.Children[j].Char == name[i]) {
								node = node.Children[j];
								found = true;
								break;
							}
						}

						if (!found) {
							node = new GraphNode (node, ++state, name[i]);
							continue;
						}
					}

					if (node.Value == null) {
						FinalStates.Add (node.State, node);
						node.Value = value;
					}

					maxEntityLength = Math.Max (maxEntityLength, name.Length);

					if (!json.Read () || json.TokenType != JsonToken.EndObject)
						break;
				}
			}

			using (var output = new StreamWriter ("HtmlEntityDecoder.g.cs")) {
				output.WriteLine ("// WARNING: This file is auto-generated. DO NOT EDIT!");
				output.WriteLine ();
				output.WriteLine ("namespace HtmlKit {");
				output.WriteLine ("\tpublic partial class HtmlEntityDecoder {");
				output.WriteLine ("\t\tconst int MaxEntityLength = {0};", maxEntityLength);
				output.WriteLine ();
				GeneratePushNamedEntityMethod (output);
				output.WriteLine ();
				GenerateGetNamedEntityValueMethod (output);
				output.WriteLine ("\t}");
				output.WriteLine ("}");
			}
		}

		static void GenerateSwitchLabels (GraphNode node)
		{
			foreach (var child in node.Children) {
				SortedDictionary<int, InnerSwitchLabel> states;
				InnerSwitchLabel inner;

				if (!OuterSwitchLabels.TryGetValue (child.Char, out states))
					OuterSwitchLabels[child.Char] = states = new SortedDictionary<int, InnerSwitchLabel> ();

				if (!states.TryGetValue (node.State, out inner))
					states[node.State] = new InnerSwitchLabel (node.State, child.State, string.Format ("{0} -> {1}", node.Name, child.Name));

				GenerateSwitchLabels (child);
			}
		}

		static void GeneratePushNamedEntityMethod (TextWriter output)
		{
			GenerateSwitchLabels (Root);

			output.WriteLine ("\t\tbool PushNamedEntity (char c)");
			output.WriteLine ("\t\t{");
			output.WriteLine ("\t\t\tswitch (c) {");
			foreach (var outer in OuterSwitchLabels) {
				output.WriteLine ("\t\t\tcase '{0}':", outer.Key);
				output.WriteLine ("\t\t\t\tswitch (state) {");
				foreach (var state in outer.Value)
					output.WriteLine ("\t\t\t\tcase {0}: state = {1}; break; // {2}", state.Value.CurrentState, state.Value.NextState, state.Value.Comment);
				output.WriteLine ("\t\t\t\tdefault: return false;");
				output.WriteLine ("\t\t\t\t}");
				output.WriteLine ("\t\t\t\tbreak;");
			}
			output.WriteLine ("\t\t\tdefault: return false;");
			output.WriteLine ("\t\t\t}"); // end switch (state)
			output.WriteLine ();
			output.WriteLine ("\t\t\tpushed[index++] = c;");
			output.WriteLine ();
			output.WriteLine ("\t\t\treturn true;");
			output.WriteLine ("\t\t}");
		}

		static void GenerateGetNamedEntityValueMethod (TextWriter output)
		{
			output.WriteLine ("\t\tstring GetNamedEntityValue ()");
			output.WriteLine ("\t\t{");
			output.WriteLine ("\t\t\tswitch (state) {");
			foreach (var kvp in FinalStates) {
				var state = kvp.Value;

				output.Write ("\t\t\tcase {0}: return \"", state.State);
				for (int i = 0; i < state.Value.Length; i++)
					output.Write ("\\u{0:X4}", (int) state.Value[i]);
				output.WriteLine ("\"; // {0}", state.Name);
			}
			output.WriteLine ("\t\t\tdefault: return new string (pushed, 0, index);");
			output.WriteLine ("\t\t\t}");
			output.WriteLine ("\t\t}");
		}
	}
}
