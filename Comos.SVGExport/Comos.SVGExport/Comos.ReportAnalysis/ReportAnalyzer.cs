using Comos.Global;
using REPORTLib;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Comos.ReportAnalysis
{
	internal class ReportAnalyzer
	{
		public ReportAnalyzer()
		{
		}

		public void ExportDocument(Document comosDocument)
		{
			this.m_ExportDocument(comosDocument);
		}

		~ReportAnalyzer()
		{
		}

		private void ItemExport_Arc(Item rdItem)
		{
			try
			{
				this.RaiseFoundReportArc(new FoundReportItemEventArgs(rdItem));
			}
			catch (Exception exception)
			{
				AppGlobal.StdErrorHandler(exception);
			}
		}

		private void ItemExport_Line(Line rdLine)
		{
			try
			{
				this.RaiseFoundReportLine(new FoundReportItemEventArgs((Item)rdLine));
			}
			catch (Exception exception)
			{
				AppGlobal.StdErrorHandler(exception);
			}
		}

		private void ItemExport_Text(Item rdItem)
		{
			try
			{
				this.RaiseFoundReportText(new FoundReportItemEventArgs(rdItem));
			}
			catch (Exception exception)
			{
				AppGlobal.StdErrorHandler(exception);
			}
		}

		private void m_ExportDocument(Document comosDocument)
		{
			// 
			// Current member / type: System.Void Comos.ReportAnalysis.ReportAnalyzer::m_ExportDocument(REPORTLib.Document)
			// File path: C:\Program Files (x86)\COMOS\1032\Current\Bin\COMOS.XMpLantExport2.DLL
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Void m_ExportDocument(REPORTLib.Document)
			// 
			// 未将对象引用设置到对象的实例。
			//    在 ..() 位置 C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Cil\InstructionBlock.cs:行号 167
			//    在 ..([] ) 位置 C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Cil\InstructionBlock.cs:行号 129
			//    在 ..( ) 位置 C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Cil\InstructionBlock.cs:行号 162
			//    在 ..(Int32 ) 位置 C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RemoveCompilerOptimizationsStep.cs:行号 382
			//    在 ..() 位置 C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RemoveCompilerOptimizationsStep.cs:行号 76
			//    在 ..(DecompilationContext ,  ) 位置 C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RemoveCompilerOptimizationsStep.cs:行号 55
			//    在 ..(MethodBody ,  , ILanguage ) 位置 C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:行号 88
			//    在 ..(MethodBody , ILanguage ) 位置 C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:行号 70
			//    在 Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) 位置 C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:行号 95
			//    在 Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) 位置 C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:行号 58
			//    在 ..(ILanguage , MethodDefinition ,  ) 位置 C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:行号 117
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		protected virtual void RaiseFoundReportArc(FoundReportItemEventArgs eaFRI)
		{
			this.FoundReportArc(this, eaFRI);
		}

		protected virtual void RaiseFoundReportLine(FoundReportItemEventArgs eaFRI)
		{
			this.FoundReportLine(this, eaFRI);
		}

		protected virtual void RaiseFoundReportText(FoundReportItemEventArgs eaFRI)
		{
			this.FoundReportText(this, eaFRI);
		}

		public event ReportAnalyzer.FoundReportArcHandler FoundReportArc;

		public event ReportAnalyzer.FoundReportLineHandler FoundReportLine;

		public event ReportAnalyzer.FoundReportTextHandler FoundReportText;

		public delegate void FoundReportArcHandler(object sender, FoundReportItemEventArgs e);

		public delegate void FoundReportLineHandler(object sender, FoundReportItemEventArgs e);

		public delegate void FoundReportTextHandler(object sender, FoundReportItemEventArgs e);
	}
}