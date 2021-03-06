// 
// Change.cs
//  
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
// Copyright (c) 2009 Novell, Inc (http://www.novell.com)
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
using System.Collections.Generic;
using MonoDevelop.Core;
using MonoDevelop.Projects.Text;
using MonoDevelop.Projects.CodeGeneration;
using Mono.TextEditor;
using MonoDevelop.Projects;
using MonoDevelop.Ide;


namespace MonoDevelop.Refactoring
{
	public abstract class Change
	{
		public string Description {
			get;
			set;
		}
		
		public Change ()
		{
		}
		
		public abstract void PerformChange (IProgressMonitor monitor, RefactorerContext rctx);
	}
	
	public class TextReplaceChange : Change
	{
		public string FileName {
			get;
			set;
		}
		
		public int Offset {
			get;
			set;
		}
		
		public bool MoveCaretToReplace {
			get;
			set;
		}
		
		int removedChars;
		public int RemovedChars {
			get { 
				return removedChars; 
			}
			set {
				if (value < 0)
					throw new ArgumentOutOfRangeException ("RemovedChars", "needs to be >= 0");
				removedChars = value; 
			}
		}
		
		public string InsertedText {
			get;
			set;
		}
		
		static List<TextEditorData> textEditorDatas = new List<TextEditorData> ();
		public static void FinishRefactoringOperation ()
		{
			foreach (TextEditorData data in textEditorDatas) {
				data.Document.EndAtomicUndo ();
				data.Document.CommitUpdateAll ();
			}
			textEditorDatas.Clear ();
		}
		
		internal static TextEditorData GetTextEditorData (string fileName)
		{
			if (IdeApp.Workbench == null)
				return null;
			foreach (var doc in IdeApp.Workbench.Documents) {
				if (doc.FileName == fileName) {
					TextEditorData result = doc.TextEditorData;
					if (result != null) {
						textEditorDatas.Add (result);
						result.Document.BeginAtomicUndo ();
						return result;
					}
				}
			}
			return null;
		}
		protected virtual TextEditorData TextEditorData {
			get {
				return GetTextEditorData (FileName);
			}
		}
		public override void PerformChange (IProgressMonitor monitor, RefactorerContext rctx)
		{
			if (rctx == null)
				throw new InvalidOperationException ("Refactory context not available.");
			
			TextEditorData textEditorData = this.TextEditorData;
			if (textEditorData == null) {
				IEditableTextFile file = rctx.GetFile (FileName);
				if (file != null) {
					if (RemovedChars > 0)
						file.DeleteText (Offset, RemovedChars);
					if (!string.IsNullOrEmpty (InsertedText))
						file.InsertText (Offset, InsertedText);
					rctx.Save ();
				}
			} else if (textEditorData != null) {
				int charsInserted = textEditorData.Replace (Offset, RemovedChars, InsertedText);
				if (MoveCaretToReplace)
					textEditorData.Caret.Offset = Offset + charsInserted;
			}
		}
		
		public override string ToString ()
		{
			return string.Format ("[TextReplaceChange: FileName={0}, Offset={1}, RemovedChars={2}, InsertedText={3}]", FileName, Offset, RemovedChars, InsertedText);
		}
	}
	
	public class CreateFileChange : Change
	{
		public string FileName {
			get;
			set;
		}
		
		public string Content {
			get;
			set;
		}
		
		public CreateFileChange (string fileName, string content)
		{
			this.FileName = fileName;
			this.Content = content;
			this.Description = string.Format (GettextCatalog.GetString ("Create file '{0}'"), Path.GetFileName (fileName));
		}
		
		public override void PerformChange (IProgressMonitor monitor, RefactorerContext rctx)
		{
			File.WriteAllText (FileName, Content);
			rctx.ParserContext.Project.AddFile (FileName);
			IdeApp.ProjectOperations.Save (rctx.ParserContext.Project);
		}
	}
	
	public class OpenFileChange : Change
	{
		public string FileName {
			get;
			set;
		}
		
		public OpenFileChange (string fileName)
		{
			this.FileName = fileName;
			this.Description = string.Format (GettextCatalog.GetString ("Open file '{0}'"), Path.GetFileName (fileName));
		}
		
		public override void PerformChange (IProgressMonitor monitor, RefactorerContext rctx)
		{
			IdeApp.Workbench.OpenDocument (FileName);
		}
	}
	
	public class RenameFileChange : Change
	{
		public string OldName {
			get;
			set;
		}
		
		public string NewName {
			get;
			set;
		}
		
		public RenameFileChange (string oldName, string newName)
		{
			this.OldName = oldName;
			this.NewName = newName;
			this.Description = string.Format (GettextCatalog.GetString ("Rename file '{0}' to '{1}'"), Path.GetFileName (oldName), Path.GetFileName (newName));
		}
		
		public override void PerformChange (IProgressMonitor monitor, RefactorerContext rctx)
		{
			FileService.RenameFile (OldName, NewName);
			
			if (rctx.ParserContext.Project != null)
				IdeApp.ProjectOperations.Save (rctx.ParserContext.Project);
		}
	}
	
	public class SaveProjectChange : Change
	{
		public Project Project {
			get;
			set;
		}
		
		public SaveProjectChange (Project project)
		{
			this.Project = project;
			this.Description = string.Format (GettextCatalog.GetString ("Save project {0}"), project.Name);
		}
		
		public override void PerformChange (IProgressMonitor monitor, RefactorerContext rctx)
		{
			Console.WriteLine ("SAVE !!!!");
			IdeApp.ProjectOperations.Save (this.Project);
		}

	}
}
