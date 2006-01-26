// this file was autogenerated by a tool.
using System;
using System.Collections;
using System.Text;

namespace ICSharpCode.SharpRefactory.Parser
{
	public class Keywords
	{
		static readonly string[] keywordList = {
			"abstract",
			"as",
			"base",
			"bool",
			"break",
			"byte",
			"case",
			"catch",
			"char",
			"checked",
			"class",
			"const",
			"continue",
			"decimal",
			"default",
			"delegate",
			"do",
			"double",
			"else",
			"enum",
			"event",
			"explicit",
			"extern",
			"false",
			"finally",
			"fixed",
			"float",
			"for",
			"foreach",
			"goto",
			"if",
			"implicit",
			"in",
			"int",
			"interface",
			"internal",
			"is",
			"lock",
			"long",
			"namespace",
			"new",
			"null",
			"object",
			"operator",
			"out",
			"override",
			"params",
			"private",
			"protected",
			"public",
			"readonly",
			"ref",
			"return",
			"sbyte",
			"sealed",
			"short",
			"sizeof",
			"stackalloc",
			"static",
			"string",
			"struct",
			"switch",
			"this",
			"throw",
			"true",
			"try",
			"typeof",
			"uint",
			"ulong",
			"unchecked",
			"unsafe",
			"ushort",
			"using",
			"virtual",
			"void",
			"volatile",
			"while"
		};
		
		static Hashtable keywords = new Hashtable();
		
		static Keywords()
		{
			for (int i = 0; i < keywordList.Length; ++i) {
				keywords.Add(keywordList[i], i + Tokens.Abstract);
			}
		}
		
		public static bool IsKeyword(string identifier)
		{
			return keywords[identifier] != null;
		}
		
		public static int GetToken(string keyword)
		{
			object k = keywords[keyword];
			if (k == null) return -1;
			return (int)k;
		}
	}
}
