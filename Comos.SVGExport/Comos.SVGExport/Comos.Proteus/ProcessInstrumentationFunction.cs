using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Comos.Proteus
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("xsd", "4.6.1055.0")]
	[Serializable]
	[XmlRoot(Namespace="", IsNullable=false)]
	[XmlType(AnonymousType=true)]
	public class ProcessInstrumentationFunction : PlantItem
	{
		private PlantItem[] items1Field;

		[XmlElement("ActuatingFunction", typeof(ActuatingFunction))]
		[XmlElement("InformationFlow", typeof(InformationFlow))]
		[XmlElement("ProcessSignalGeneratingFunction", typeof(ProcessSignalGeneratingFunction))]
		public PlantItem[] Items1
		{
			get
			{
				return this.items1Field;
			}
			set
			{
				this.items1Field = value;
			}
		}

		public ProcessInstrumentationFunction()
		{
		}
	}
}