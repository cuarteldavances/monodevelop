// DebuggingService.cs - Debugging service frontend for MonoDebugger
//
//  Author: Mike Kestner <mkesner@ximian.com>
//
// Copyright (c) 2004 Novell, Inc.

using System;
using System.Collections;
using System.Collections.Generic;

using MonoDevelop.Core;
using Mono.Addins;
using MonoDevelop.Core.Execution;
using MonoDevelop.Core.Gui;

using MonoDevelop.Ide.Gui;

using Mono.Debugging.Client;
using Mono.Debugging.Backend;

/*
 * Some places we should be doing some error handling we used to toss
 * exceptions, now we error out silently, this needs a real solution.
 */

namespace MonoDevelop.Ide.Debugging
{

	public class DebuggingService
	{
		const string FactoriesPath = "/Mono/Debugging/DebuggerFactories";
		
		BreakpointStore breakpoints = new BreakpointStore ();
		
		IConsole console;
		DebugExecutionHandlerFactory executionHandlerFactory;
		
		DebuggerSession session;
		Backtrace currentBacktrace;
		int currentFrame;

		public event EventHandler PausedEvent;
		public event EventHandler ResumedEvent;
		public event EventHandler StoppedEvent;
		
		public event EventHandler CallStackChanged;
		public event EventHandler CurrentFrameChanged;
		public event EventHandler ExecutionLocationChanged;

		internal DebuggingService()
		{
			executionHandlerFactory = new DebugExecutionHandlerFactory ();
		}

		public IExecutionHandlerFactory GetExecutionHandlerFactory ()
		{
			return executionHandlerFactory;
		}
		
		public DebuggerSession DebuggerSession {
			get { return session; }
		}
		
		public BreakpointStore Breakpoints {
			get { return breakpoints; }
		}
		
		void Cleanup ()
		{
			if (!IsDebugging)
				return;

			if (StoppedEvent != null)
				StoppedEvent (null, new EventArgs ());

			if (session != null) {
				session.Dispose ();
				session = null;
			}
			
			if (console != null) {
				console.Dispose ();
				console = null;
			}
			
			NotifyLocationChanged ();
		}

		public bool IsDebugging {
			get {
				return session != null;
			}
		}

		public bool IsRunning {
			get {
				return IsDebugging && session.IsRunning;
			}
		}

		void target_output (bool is_stderr, string line)
		{
			Console.WriteLine (line);
			console.Out.Write (line);
		}

		void KillApplication (object obj)
		{
			Cleanup ();
		}

		public void Pause ()
		{
			session.Stop ();
		}

		public void Resume ()
		{
			session.Continue ();
			NotifyLocationChanged ();
		}

		public IProcessAsyncOperation Run (string file, IConsole console)
		{
			DebugExecutionHandler h = new DebugExecutionHandler (null);
			return h.Execute (file, null, null, null, console);
		}
		
		public IAsyncOperation AttachToProcess (IDebuggerEngine debugger, ProcessInfo proc)
		{
			session = debugger.CreateSession ();
			IProgressMonitor monitor = IdeApp.Workbench.ProgressMonitors.GetRunProgressMonitor ();
			console = monitor as IConsole;
			SetupSession ();
			session.TargetExited += delegate {
				monitor.Dispose ();
			};
			session.AttachToProcess (proc);
			return monitor.AsyncOperation;
		}
		
		internal void InternalRun (string platform, DebuggerStartInfo startInfo, IConsole console)
		{
			this.console = console;
			
			if (platform != null)
				session = CreateDebugSessionForPlatform (platform);
			else
				session = CreateDebugSessionForFile (startInfo.Command);
			
			SetupSession ();
			session.Run (startInfo);
		}
		
		void SetupSession ()
		{
			session.Breakpoints = breakpoints;
			session.TargetEvent += OnTargetEvent;
			session.TargetStarted += OnStarted;
			session.OutputWriter = delegate (bool iserr, string text) {
				if (iserr)
					console.Error.Write (text);
				else
					console.Out.Write (text);
			};

			console.CancelRequested += new EventHandler (OnCancelRequested);
			NotifyLocationChanged ();
		}
		
		void OnStarted (object s, EventArgs a)
		{
			currentBacktrace = null;
			Gtk.Application.Invoke (delegate {
				NotifyCallStackChanged ();
				NotifyCurrentFrameChanged ();
				NotifyLocationChanged ();
				if (ResumedEvent != null)
					ResumedEvent (null, a);
			});
		}
		
		void OnTargetEvent (object sender, TargetEventArgs args)
		{
			try {
				Console.WriteLine ("OnTargetEvent, type - {0}", args.Type);
				if (args.Type != TargetEventType.TargetExited) {
					SetCurrentBacktrace (args.Backtrace);
				}
				
				switch (args.Type) {
					case TargetEventType.TargetExited:
					case TargetEventType.TargetSignaled:
						KillApplication (null);
						break;
					case TargetEventType.TargetStopped:
					case TargetEventType.TargetRunning:
					case TargetEventType.TargetHitBreakpoint:
					case TargetEventType.TargetInterrupted:
					case TargetEventType.UnhandledException:
						NotifyPaused ();
						break;
					default:
						break;
				}
			} catch (Exception e) {
				Console.WriteLine ("OnTargetEvent, {0}", e.ToString ());
			}

		}

		void NotifyPaused ()
		{
			Gtk.Application.Invoke (delegate {
				if (PausedEvent != null)
					PausedEvent (null, EventArgs.Empty);
				NotifyLocationChanged ();
			});
		}
		
		void NotifyLocationChanged ()
		{
			if (ExecutionLocationChanged != null)
				ExecutionLocationChanged (null, EventArgs.Empty);
		}
		
		void NotifyCurrentFrameChanged ()
		{
			if (CurrentFrameChanged != null)
				CurrentFrameChanged (this, EventArgs.Empty);
		}
		
		void NotifyCallStackChanged ()
		{
			if (CallStackChanged != null)
				CallStackChanged (this, EventArgs.Empty);
		}
		
		void OnCancelRequested (object sender, EventArgs args)
		{
			Stop ();
		}

		public void Stop ()
		{
			if (!IsDebugging)
				return;

			session.Exit ();
			Cleanup ();
		}

		public void StepInto ()
		{
			if (!IsDebugging)
				//throw new Exception ("Can't step without running debugger.");
				return;

			if (IsRunning)
				throw new Exception ("Can't step unless paused.");

			session.StepLine ();
			NotifyLocationChanged ();
		}

		public void StepOver ()
		{
			if (!IsDebugging)
				//throw new Exception ("Can't step without running debugger.");
				return;

			if (IsRunning)
				//throw new Exception ("Can't step unless paused.");
				return;

			session.NextLine ();
			NotifyLocationChanged ();
		}

		public void StepOut ()
		{
			if (!IsDebugging)
				return;

			if (IsRunning)
				return;

			session.Finish ();
			NotifyLocationChanged ();
		}

		public Backtrace CurrentCallStack {
			get { return currentBacktrace; }
		}

		public string CurrentFilename {
			get {
				StackFrame sf = CurrentFrame;
				if (sf != null)
					return sf.SourceLocation.Filename;
				else
					return null;
			}
		}

		public int CurrentLineNumber {
			get {
				StackFrame sf = CurrentFrame;
				if (sf != null)
					return sf.SourceLocation.Line;
				else
					return -1;
			}
		}

		public StackFrame CurrentFrame {
			get {
				if (currentBacktrace != null && currentFrame != -1)
					return currentBacktrace.GetFrame (currentFrame);
				else
					return null;
			}
		}
		
		public int CurrentFrameIndex {
			get {
				return currentFrame;
			}
			set {
				if (currentBacktrace != null && value < currentBacktrace.FrameCount) {
					currentFrame = value;
					Gtk.Application.Invoke (delegate {
						NotifyCurrentFrameChanged ();
					});
				}
				else
					currentFrame = -1;
			}
		}
		
		void SetCurrentBacktrace (Backtrace bt)
		{
			currentBacktrace = bt;
			if (currentBacktrace != null)
				currentFrame = 0;
			else
				currentFrame = -1;

			Gtk.Application.Invoke (delegate {
				NotifyCallStackChanged ();
				NotifyCurrentFrameChanged ();
			});
		}
		
		public bool CanDebugPlatform (string platform)
		{
			return GetFactoryForPlatform (platform) != null;
		}
		
		public bool CanDebugFile (string file)
		{
			return GetFactoryForFile (file) != null;
		}
		
		public DebuggerSession CreateDebugSessionForPlatform (string platform)
		{
			IDebuggerEngine factory = GetFactoryForPlatform (platform);
			if (factory != null) {
				DebuggerSession ds = factory.CreateSession ();
				ds.Initialize ();
				return ds;
			} else
				throw new InvalidOperationException ("Unsupported platform: " + platform);
		}
		
		public DebuggerSession CreateDebugSessionForFile (string file)
		{
			IDebuggerEngine factory = GetFactoryForFile (file);
			if (factory != null) {
				DebuggerSession ds = factory.CreateSession ();
				ds.Initialize ();
				return ds;
			} else
				throw new InvalidOperationException ("Unsupported file: " + file);
		}
		
		public IDebuggerEngine[] GetDebuggerEngines ()
		{
			return (IDebuggerEngine[]) AddinManager.GetExtensionObjects (FactoriesPath, typeof(IDebuggerEngine), true);
		}		
		
		IDebuggerEngine GetFactoryForPlatform (string platform)
		{
			foreach (TypeExtensionNode node in AddinManager.GetExtensionNodes (FactoriesPath)) {
				IDebuggerEngine factory = (IDebuggerEngine) node.GetInstance ();
				if (factory.CanDebugPlatform (platform))
					return factory;
			}
			return null;
		}
		
		IDebuggerEngine GetFactoryForFile (string file)
		{
			foreach (TypeExtensionNode node in AddinManager.GetExtensionNodes (FactoriesPath)) {
				IDebuggerEngine factory = (IDebuggerEngine) node.GetInstance ();
				if (factory.CanDebugFile (file))
					return factory;
			}
			return null;
		}
	}
}
