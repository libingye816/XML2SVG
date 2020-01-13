using Comos.Global;
using Comos.ProgressLoggerTemplates2;
using ComosVBInterface;
using ProgressLogger2;
using ProgressLogger2.Styles;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Comos.SVGExport
{
	internal class XMpLantExportLogging
	{
		private string m_Errtxt = "";

		private bool m_Unattended;

		private bool m_fHasDesiredTraceLevel;

		private LogFile m_LogFile;

		private LogFileViewer m_LogViewer;

		private Stack<HierarchicalTask> m_Categories;

		private Task m_CurrentTask;

		private ProgressView m_CurrentProgress;

		private bool NoLogging
		{
			get
			{
				if (!this.m_Unattended)
				{
					return false;
				}
				return this.m_LogFile == null;
			}
		}

		internal LogFile PL2LogFile
		{
			get
			{
				return this.m_LogFile;
			}
			set
			{
				this.m_LogFile = value;
			}
		}

		internal long WaitProgress
		{
			set
			{
				if (this.NoLogging)
				{
					return;
				}
				if (this.m_CurrentTask.ProgressValue * 100 / this.m_CurrentTask.ProgressMaxValue != (int)value * 100 / this.m_CurrentTask.ProgressMaxValue)
				{
					this.m_CurrentTask.ProgressValue = (int)value;
				}
			}
		}

		internal XMpLantExportLogging(IXReport rep, bool unattended, bool hastracelevel)
		{
			this.m_Errtxt = "";
			this.m_Unattended = unattended;
			this.m_fHasDesiredTraceLevel = hastracelevel;
			if (!unattended)
			{
				this.m_LogFile = Logger.CreateLogFile();
				StyleTemplate template = ComosStyle.getInstance().Template;
				this.m_LogViewer = new LogFileViewer(this.m_LogFile, template)
				{
					AllowSave = true,
					AutoScroll = true
				};
				ComosLogWindow.Show(this.m_LogViewer, AppGlobal.ITX("~0666d XMpLant Export"), 640, 480);
			}
			this.m_Categories = new Stack<HierarchicalTask>();
		}

		internal void Error(string err)
		{
			this.m_Errtxt = string.Concat(this.m_Errtxt, "\r\n", err);
			if (this.m_fHasDesiredTraceLevel && !this.NoLogging)
			{
				Logger.CreateError(this.GetActivePosition(), err);
			}
		}

		private Task GetActivePosition()
		{
			if (this.m_CurrentTask != null)
			{
				return this.m_CurrentTask;
			}
			if (this.m_Categories.Count <= 0)
			{
				return null;
			}
			return this.m_Categories.Peek();
		}

		internal void Information(string strMsg)
		{
			if (this.NoLogging)
			{
				return;
			}
			Logger.CreateHint(this.GetActivePosition(), strMsg);
		}

		internal void OutputErrorsToTraceSources()
		{
			if (this.m_Errtxt != "")
			{
				if (this.NoLogging)
				{
					AppGlobal.CTraceSources["AppGlobal"].TraceEvent(TraceEventType.Warning, 0, this.m_Errtxt);
					return;
				}
				if (this.m_fHasDesiredTraceLevel)
				{
					AppGlobal.CTraceSources["AppGlobal"].TraceEvent(TraceEventType.Warning, 0, this.m_Errtxt);
				}
			}
		}

		internal void PushCategory(HierarchicalTask pushTask)
		{
			if (pushTask != null)
			{
				Stack<HierarchicalTask> mCategories = this.m_Categories;
				if (mCategories == null)
				{
					return;
				}
				mCategories.Push(pushTask);
			}
		}

		internal void StartNewCategory(string catname)
		{
			if (this.NoLogging)
			{
				return;
			}
			HierarchicalTask hierarchicalTask = null;
			hierarchicalTask = (this.m_Categories.Count != 0 ? Logger.CreateHierarchicalTask(this.m_Categories.Peek(), catname, "", DateTime.Now) : Logger.CreateHierarchicalTask(this.m_LogFile, catname, "", DateTime.Now));
			this.m_Categories.Push(hierarchicalTask);
			hierarchicalTask.Start();
		}

		internal void StopCurrentCategory()
		{
			if (this.NoLogging)
			{
				return;
			}
			Task mCurrentTask = this.m_CurrentTask;
			if (mCurrentTask != null)
			{
				mCurrentTask.Stop();
			}
			else
			{
			}
			this.m_Categories.Pop().Stop();
		}

		internal void StopCurrentTask()
		{
			if (this.m_CurrentTask != null)
			{
				this.m_CurrentTask.Stop();
				this.m_CurrentTask = null;
			}
		}

		internal void Success(string strMsg)
		{
			if (this.NoLogging)
			{
				return;
			}
			Logger.CreateSuccess(this.GetActivePosition(), strMsg);
		}

		internal void WaitMsg(string sNewValue, int count)
		{
			if (this.NoLogging)
			{
				return;
			}
			if (this.m_CurrentTask != null)
			{
				if (this.m_CurrentProgress != null)
				{
					this.m_CurrentTask.ProgressValue = this.m_CurrentTask.ProgressMaxValue;
				}
				this.m_CurrentTask.Stop();
			}
			if (count <= 0)
			{
				this.m_CurrentProgress = null;
				this.m_CurrentTask = Logger.CreateTask(this.m_Categories.Peek(), sNewValue, "", DateTime.Now);
			}
			else
			{
				this.m_CurrentProgress = Logger.CreateProgressView(0, 0, count);
				this.m_CurrentTask = Logger.CreateTask(this.m_Categories.Peek(), sNewValue, "", DateTime.Now, this.m_CurrentProgress);
			}
			this.m_CurrentTask.Start();
		}

		internal void Warning(string strMsg, object ComosObject = null)
		{
			if (this.NoLogging)
			{
				AppGlobal.CTraceSources["AppGlobal"].TraceEvent(TraceEventType.Warning, 0, strMsg);
				return;
			}
			if (ComosObject == null)
			{
				Logger.CreateWarning(this.GetActivePosition(), strMsg);
				return;
			}
			Executer[] executerArray = new Executer[] { NavigationExecuter.Get(ComosObject) };
			Actions action = Logger.CreateAction(this.GetActivePosition().LogFile, executerArray);
			Logger.CreateWarning(this.GetActivePosition(), strMsg, action);
		}
	}
}