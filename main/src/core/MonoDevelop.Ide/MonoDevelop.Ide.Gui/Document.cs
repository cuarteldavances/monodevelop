//
// Document.cs
//
// Author:
//   Lluis Sanchez Gual
//
// Copyright (C) 2005 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//


using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Gtk;

using MonoDevelop.Core;
using MonoDevelop.Core.Execution;
using MonoDevelop.Components;
using MonoDevelop.Projects;
using MonoDevelop.Projects.Text;
using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.Ide.Gui.Dialogs;
using MonoDevelop.Projects.Dom;
using MonoDevelop.Projects.Dom.Parser;
using MonoDevelop.Ide.Tasks;
using Mono.Addins;
using MonoDevelop.Ide.Extensions;

namespace MonoDevelop.Ide.Gui
{
	public class Document
	{
		internal object MemoryProbe = Counters.DocumentsInMemory.CreateMemoryProbe ();
		
		IWorkbenchWindow window;
		TextEditorExtension editorExtension;
		bool editorChecked;
		TextEditor textEditor;
		bool closed;
		
		bool parsing;
		const int ParseDelay = 600;

		public IWorkbenchWindow Window {
			get { return window; }
		}
		
		internal DateTime LastTimeActive {
			get;
			set;
		}
		
		public TextEditorExtension EditorExtension {
			get { return this.editorExtension; }
		}
 		
		public object GetContent (Type type)
		{
			//check whether the ViewContent can return the type directly
			object ret = Window.ActiveViewContent.GetContent (type);
			if (ret != null)
				return ret;
			
			//check the primary viewcontent
			//not sure if this is the right thing to do, but things depend on this behaviour
			if (Window.ViewContent != Window.ActiveViewContent) {
				ret = Window.ViewContent.GetContent (type);
				if (ret != null)
					return ret;
			}
			
			//no, so look through the TexteditorExtensions as well
			TextEditorExtension nextExtension = editorExtension;
			while (nextExtension != null) {
				if (type.IsAssignableFrom (nextExtension.GetType ()))
					return nextExtension;
				nextExtension = nextExtension.Next as TextEditorExtension;
			}
			return null;
		}
		
		public T GetContent <T>()
		{
			return (T) GetContent (typeof(T));
		}
		
		public Document (IWorkbenchWindow window)
		{
			Counters.OpenDocuments++;
			LastTimeActive = DateTime.Now;
			this.window = window;
			window.Closed += OnClosed;
			window.ActiveViewContentChanged += OnActiveViewContentChanged;
			if (IdeApp.Workspace != null)
				IdeApp.Workspace.ItemRemovedFromSolution += OnEntryRemoved;
		}

		public FilePath FileName {
			get {
				if (!Window.ViewContent.IsFile)
					return null;
				return Window.ViewContent.IsUntitled ? Window.ViewContent.UntitledName : Window.ViewContent.ContentName;
			}
		}

		public bool IsFile {
			get { return Window.ViewContent.IsFile; }
		}
		
		public bool IsDirty {
			get { return Window.ViewContent.ContentName == null || Window.ViewContent.IsDirty; }
			set { Window.ViewContent.IsDirty = value; }
		}
		
		public bool HasProject {
			get { return Window.ViewContent.Project != null; }
		}
		
		public Project Project {
			get { return Window.ViewContent.Project; }
		}
		
		ProjectDom fileDom;
		public ProjectDom Dom {
			get {
				ProjectDom result = ProjectDomService.GetProjectDom (Project);
				if (result != null)
					return result;
				
				if (fileDom == null) 
					fileDom = ProjectDomService.GetFileDom (FileName);
				return fileDom ?? ProjectDom.Empty;
			}
		}
		
		public string PathRelativeToProject {
			get { return Window.ViewContent.PathRelativeToProject; }
		}
		
		public void Select ()
		{
			window.SelectWindow ();
		}
		
		public IBaseViewContent ActiveView {
			get { return window.ActiveViewContent; }
		}
		
		public IViewContent PrimaryView {
			get { return window.ViewContent; }
		}
		
		public string Name {
			get {
				IViewContent view = Window.ViewContent;
				return view.IsUntitled ? view.UntitledName : view.ContentName;
			}
		}
		
		public TextEditor TextEditor {
			get {
				if (!editorChecked) {
					editorChecked = true;
					textEditor = TextEditor.GetTextEditor (Window.ViewContent);
				}
				return textEditor;
			}
		}
		
		Mono.TextEditor.TextEditorData data = null;
		public Mono.TextEditor.TextEditorData TextEditorData {
			get {
				if (data == null) {
					Mono.TextEditor.ITextEditorDataProvider view = GetContent <Mono.TextEditor.ITextEditorDataProvider> ();
					if (view != null) {
						data = view.GetTextEditorData ();
						data.Document.Tag = this;
					}
				}
				return data;
			}
		}
		
		
		public bool IsViewOnly {
			get { return Window.ViewContent.IsViewOnly; }
		}
		
		public void Reload ()
		{
			ICustomXmlSerializer memento = null;
			IMementoCapable mc = GetContent<IMementoCapable> ();
			if (mc != null) {
				memento = mc.Memento;
			}
			window.ViewContent.Load (window.ViewContent.ContentName);
			if (memento != null) {
				mc.Memento = memento;
			}
		}
		
		public virtual void Save ()
		{
			if (Window.ViewContent.IsViewOnly || !Window.ViewContent.IsDirty)
				return;

			if (!Window.ViewContent.IsFile) {
				Window.ViewContent.Save ();
				return;
			}
			
			if (Window.ViewContent.ContentName == null) {
				SaveAs ();
			} else {
				if (!FileService.RequestFileEdit (Window.ViewContent.ContentName))
					MessageService.ShowMessage (GettextCatalog.GetString ("The file could not be saved. Write permission has not been granted."));
				
				FileAttributes attr = FileAttributes.ReadOnly | FileAttributes.Directory | FileAttributes.Offline | FileAttributes.System;

				if (!File.Exists (Window.ViewContent.ContentName) || (File.GetAttributes(window.ViewContent.ContentName) & attr) != 0) {
					SaveAs ();
				} else {						
					string fileName = Window.ViewContent.ContentName;
					// save backup first						
					if((bool) PropertyService.Get ("SharpDevelop.CreateBackupCopy", false)) {
						Window.ViewContent.Save (fileName + "~");
						FileService.NotifyFileChanged (fileName);
					}
					Window.ViewContent.Save (fileName);
					FileService.NotifyFileChanged (fileName);
					OnSaved (EventArgs.Empty);
				}
			}
		}
		
		public void SaveAs ()
		{
			SaveAs (null);
		}
		
		public void SaveAs (string filename)
		{
			if (Window.ViewContent.IsViewOnly || !Window.ViewContent.IsFile)
				return;
			
			string encoding = null;
			
			IEncodedTextContent tbuffer = GetContent <IEncodedTextContent> ();
			if (tbuffer != null) {
				encoding = tbuffer.SourceEncoding;
				if (encoding == null)
					encoding = TextEncoding.DefaultEncoding;
			}
				
			if (filename == null) {
				var dlg = new OpenFileDialog (GettextCatalog.GetString ("Save as..."), FileChooserAction.Save) {
					TransientFor = IdeApp.Workbench.RootWindow,
					Encoding = encoding,
					ShowEncodingSelector = (tbuffer != null),
				};
				
				if (Window.ViewContent.IsUntitled)
					dlg.InitialFileName = Window.ViewContent.UntitledName;
				else {
					dlg.CurrentFolder = Path.GetDirectoryName (Window.ViewContent.ContentName);
					dlg.InitialFileName = Path.GetFileName (Window.ViewContent.ContentName);
				}
				
				if (!dlg.Run ())
					return;
				
				filename = dlg.SelectedFile;
				encoding = dlg.Encoding;
			}
		
			if (!FileService.IsValidPath (filename)) {
				MessageService.ShowMessage (GettextCatalog.GetString ("File name {0} is invalid", filename));
				return;
			}
			// detect preexisting file
			if(File.Exists(filename)){
				if (!MessageService.Confirm (GettextCatalog.GetString ("File {0} already exists. Overwrite?", filename), AlertButton.OverwriteFile))
					return;
			}
			
			// save backup first
			if((bool) PropertyService.Get ("SharpDevelop.CreateBackupCopy", false)) {
				if (tbuffer != null && encoding != null)
					tbuffer.Save (filename + "~", encoding);
				else
					Window.ViewContent.Save (filename + "~");
			}
			
			// do actual save
			if (tbuffer != null && encoding != null)
				tbuffer.Save (filename, encoding);
			else
				Window.ViewContent.Save (filename);

			FileService.NotifyFileChanged (filename);
			IdeApp.Workbench.RecentOpen.AddLastFile (filename, null);
			
			OnSaved (EventArgs.Empty);
		}
		
		public virtual bool IsBuildTarget
		{
			get
			{
				if (this.IsViewOnly)
					return false;
				if (Window.ViewContent.ContentName != null)
					return Services.ProjectService.CanCreateSingleFileProject(Window.ViewContent.ContentName);
				
				return false;
			}
		}
		
		public virtual IAsyncOperation Build ()
		{
			return IdeApp.ProjectOperations.BuildFile (Window.ViewContent.ContentName);
		}
		
		public virtual IAsyncOperation Rebuild ()
		{
			return Build ();
		}
		
		public virtual void Clean ()
		{
		}
		
		public IAsyncOperation Run ()
		{
			return Run (Runtime.ProcessService.DefaultExecutionHandler);
		}

		public virtual IAsyncOperation Run (IExecutionHandler handler)
		{
			return IdeApp.ProjectOperations.ExecuteFile (Window.ViewContent.ContentName, handler);
		}

		public virtual bool CanRun ()
		{
			return CanRun (Runtime.ProcessService.DefaultExecutionHandler);
		}
		
		public virtual bool CanRun (IExecutionHandler handler)
		{
			return IsBuildTarget && Window.ViewContent.ContentName != null && IdeApp.ProjectOperations.CanExecuteFile (Window.ViewContent.ContentName, handler);
		}
		
		public bool Close ()
		{
			return Window.CloseWindow (false, true, 0);
		}
		
		protected virtual void OnSaved (EventArgs args)
		{
			if (Saved != null)
				Saved (this, args);
		}
		
		void OnClosed (object s, EventArgs a)
		{
			closed = true;
			ClearTasks ();
			
			string currentParseFile = FileName;
			Project curentParseProject = Project;
			
			if (window is SdiWorkspaceWindow)
				((SdiWorkspaceWindow)window).DetachFromPathedDocument ();
			window.Closed -= OnClosed;
			window.ActiveViewContentChanged -= OnActiveViewContentChanged;
			if (IdeApp.Workspace != null)
				IdeApp.Workspace.ItemRemovedFromSolution -= OnEntryRemoved;
			
			try {
				OnClosed (a);
			} catch (Exception ex) {
				LoggingService.LogError ("Exception while calling OnClosed.", ex);
			}
			
			while (editorExtension != null) {
				try {
					editorExtension.Dispose ();
				} catch (Exception ex) {
					LoggingService.LogError ("Exception while disposing extension:" + editorExtension, ex);
				}
				editorExtension = editorExtension.Next as TextEditorExtension;
			}
			
			// Parse the file when the document is closed. In this way if the document
			// is closed without saving the changes, the saved compilation unit
			// information will be restored
			if (currentParseFile != null) {
				System.Threading.ThreadPool.QueueUserWorkItem (delegate {
					// Don't access Document properties from the thread
					ProjectDomService.Parse (curentParseProject, currentParseFile, DesktopService.GetMimeTypeForUri (currentParseFile));
				});
			}
			if (fileDom != null) {
				ProjectDomService.RemoveFileDom (FileName);
				fileDom = null;
			}
			
			Counters.OpenDocuments--;
		}
#region document tasks
		object lockObj = new object ();
		
		ParsedDocument lastErrorFreeParsedDocument;
		
		ParsedDocument parsedDocument;
		public ParsedDocument ParsedDocument {
			get {
				return parsedDocument;
			}
			set {
				// for unit testing purposes
				parsedDocument = value;
			}
		}
		
		public ICompilationUnit CompilationUnit {
			get {
				return parsedDocument != null ? parsedDocument.CompilationUnit : null;
			}
		}
		
		void ClearTasks ()
		{
			lock (lockObj) {
				TaskService.Errors.ClearByOwner (this);
			}
		}
		
//		void CompilationUnitUpdated (object sender, ParsedDocumentEventArgs args)
//		{
//			if (this.FileName == args.FileName) {
////				if (!args.Unit.HasErrors)
//				parsedDocument = args.ParsedDocument;
///* TODO: Implement better task update algorithm.
//
//				ClearTasks ();
//				lock (lockObj) {
//					foreach (Error error in args.Unit.Errors) {
//						tasks.Add (new Task (this.FileName, error.Message, error.Column, error.Line, error.ErrorType == ErrorType.Error ? TaskType.Error : TaskType.Warning, this.Project));
//					}
//					IdeApp.Services.TaskService.AddRange (tasks);
//				}*/
//			}
//		}
#endregion
		void OnActiveViewContentChanged (object s, EventArgs args)
		{
			OnViewChanged (args);
		}
		
		protected virtual void OnClosed (EventArgs args)
		{
			if (Closed != null)
				Closed (this, args);
		}
		
		protected virtual void OnViewChanged (EventArgs args)
		{
			if (ViewChanged != null)
				ViewChanged (this, args);
		}
		
		internal void OnDocumentAttached ()
		{
			IExtensibleTextEditor editor = GetContent<IExtensibleTextEditor> ();
			if (editor == null)
				return;
			editor.TextChanged += OnDocumentChanged;
			//this.parsedDocument = MonoDevelop.Projects.Dom.Parser.ProjectDomService.Parse (Project, FileName, DesktopService.GetMimeTypeForUri (FileName), TextEditor.Text);
			OnDocumentChanged (this, null);

			// If the new document is a text editor, attach the extensions
			
			ExtensionNodeList extensions = AddinManager.GetExtensionNodes ("/MonoDevelop/Ide/TextEditorExtensions", typeof(TextEditorExtensionNode));
			
			editorExtension = null;
			TextEditorExtension last = null;
			
			foreach (TextEditorExtensionNode extNode in extensions) {
				if (!extNode.Supports (FileName))
					continue;
				TextEditorExtension ext = (TextEditorExtension) extNode.CreateInstance ();
				if (ext.ExtendsEditor (this, editor)) {
					if (editorExtension == null)
						editorExtension = ext;
					if (last != null)
						last.Next = ext;
					last = ext;
					ext.Initialize (this);
				}
			}
			if (editorExtension != null)
				last.Next = editor.AttachExtension (editorExtension);
			window.Document = this;
			
			if (window is SdiWorkspaceWindow)
				((SdiWorkspaceWindow)window).AttachToPathedDocument (GetContent<MonoDevelop.Ide.Gui.Content.IPathedDocument> ());
		}
		
		internal void SetProject (Project project)
		{
			IExtensibleTextEditor editor = GetContent<IExtensibleTextEditor> ();
			if (editor != null)
				editor.TextChanged -= OnDocumentChanged;
			while (editorExtension != null) {
				try {
					editorExtension.Dispose ();
				} catch (Exception ex) {
					LoggingService.LogError ("Exception while disposing extension:" + editorExtension, ex);
				}
				editorExtension = editorExtension.Next as TextEditorExtension;
			}
			editorExtension = null;
			ISupportsProjectReload pr = GetContent<ISupportsProjectReload> ();
			if (pr != null) {
				Window.ViewContent.Project = project;
				pr.Update (project);
			}
			OnDocumentAttached ();
		}
		
		/// <summary>
		/// This method can take some time to finish. It's not threaded
		/// </summary>
		/// <returns>
		/// A <see cref="ParsedDocument"/> that contains the current dom.
		/// </returns>
		public ParsedDocument UpdateParseDocument ()
		{
			parsing = true;
			try {
				string currentParseFile = FileName;
				string mime = DesktopService.GetMimeTypeForUri (currentParseFile);
				string currentParseText = TextEditor.Text;
					Project curentParseProject = Project;
				this.parsedDocument = ProjectDomService.Parse (curentParseProject, currentParseFile, mime, currentParseText);
				if (this.parsedDocument != null && !this.parsedDocument.HasErrors)
					this.lastErrorFreeParsedDocument = parsedDocument;
			} finally {
				parsing = false;
				OnDocumentParsed (EventArgs.Empty);
			}
			return this.parsedDocument;
		}
		
		void OnDocumentChanged (object o, EventArgs a)
		{
			// Don't directly parse the document because doing it at every key press is
			// very inefficient. Do it after a small delay instead, so several changes can
			// be parsed at the same time.
			
			if (parsing)
				return;

			parsing = true;
			string currentParseFile = FileName;
			string mime = DesktopService.GetMimeTypeForUri (currentParseFile);
			
			GLib.Timeout.Add (ParseDelay, delegate {
				if (closed)
					return false;
				parsing = false;
				string currentParseText = TextEditor.Text;
				Project curentParseProject = Project;
				System.Threading.ThreadPool.QueueUserWorkItem (delegate {
					// Don't access Document properties from the thread
					this.parsedDocument = ProjectDomService.Parse (curentParseProject, currentParseFile, mime, currentParseText);
					if (this.parsedDocument != null && !this.parsedDocument.HasErrors)
						this.lastErrorFreeParsedDocument = parsedDocument;
					DispatchService.GuiSyncDispatch (delegate {
						OnDocumentParsed (EventArgs.Empty);
					});
				});
				return false;
			});
		}
		
		internal object ExtendedCommandTargetChain {
			get { return editorExtension; }
		}

		public ParsedDocument LastErrorFreeParsedDocument {
			get {
				return lastErrorFreeParsedDocument;
			}
		}
		
		void OnEntryRemoved (object sender, SolutionItemEventArgs args)
		{
			if (args.SolutionItem == window.ViewContent.Project)
				window.ViewContent.Project = null;
		}
		
		protected virtual void OnDocumentParsed (EventArgs e)
		{
			EventHandler handler = this.DocumentParsed;
			if (handler != null)
				handler (this, e);
		}
		
		public event EventHandler Closed;
		public event EventHandler Saved;
		public event EventHandler ViewChanged;
		
		public event EventHandler DocumentParsed;
	}
	
	
	[Serializable]
	public sealed class DocumentEventArgs : EventArgs
	{
		public Document Document {
			get;
			set;
		}
		public DocumentEventArgs (Document document)
		{
			this.Document = document;
		}
	}
}

