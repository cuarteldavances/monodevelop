// SourceEditorDisplayBinding.cs
//
// Author:
//   Mike Krüger <mkrueger@novell.com>
//
// Copyright (c) 2008 Novell, Inc (http://www.novell.com)
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

using System;
using System.IO;
using MonoDevelop.Core.Gui;
using MonoDevelop.Ide.Codons;
using MonoDevelop.Ide.Gui;

namespace MonoDevelop.SourceEditor
{
	public class SourceEditorDisplayBinding : IDisplayBinding
	{
		static SourceEditorDisplayBinding ()
		{
			SourceEditorOptions.Init ();
		}
		
		string IDisplayBinding.DisplayName {
			get {
				return "Source Code Editor";
			}
		}
		
		bool IDisplayBinding.CanCreateContentForFile (string fileName)
		{
			return false;
		}
		MonoDevelop.Ide.Gui.IViewContent IDisplayBinding.CreateContentForFile (string fileName)
		{
			return new SourceEditorView ();
		}

		bool IDisplayBinding.CanCreateContentForMimeType (string mimetype)
		{
			if (String.IsNullOrEmpty (mimetype))
				return false;
			if (mimetype.StartsWith ("text"))
				return true;
			switch (mimetype) {
			case "application/x-python":
			case "application/x-config":
			case "application/x-aspx":
			case "application/x-ascx":
			case "application/x-web-config":
				return true;
			}
			// If gedit can open the file, this editor also can do it
			foreach (DesktopApplication app in IdeApp.Services.PlatformService.GetAllApplications (mimetype))
				if (app.Command == "gedit")
					return true;
			return true;
		}

		MonoDevelop.Ide.Gui.IViewContent IDisplayBinding.CreateContentForMimeType (string mimeType, System.IO.Stream content)
		{
			SourceEditorView result = new SourceEditorView ();
			result.Document.MimeType = mimeType;
			using (StreamReader reader = new StreamReader (content)) {
				result.Document.Text = reader.ReadToEnd ();
			}
			return result;
		}
	}
}
