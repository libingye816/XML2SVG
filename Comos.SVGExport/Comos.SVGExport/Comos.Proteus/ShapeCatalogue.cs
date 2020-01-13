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
	public class ShapeCatalogue
	{
		private object[] itemsField;

		private string nameField;

		private string versionField;

		private string unitsField;

		private DateTime dateField;

		private bool dateFieldSpecified;

		[XmlAttribute(DataType="date")]
		public DateTime Date
		{
			get
			{
				return this.dateField;
			}
			set
			{
				this.dateField = value;
			}
		}

		[XmlIgnore]
		public bool DateSpecified
		{
			get
			{
				return this.dateFieldSpecified;
			}
			set
			{
				this.dateFieldSpecified = value;
			}
		}

		[XmlElement("ActuatingFunction", typeof(ActuatingFunction))]
		[XmlElement("ActuatingSystem", typeof(ActuatingSystem))]
		[XmlElement("ActuatingSystemComponent", typeof(ActuatingSystemComponent))]
		[XmlElement("Component", typeof(Comos.Proteus.Component))]
		[XmlElement("Equipment", typeof(Equipment))]
		[XmlElement("InstrumentationLoopFunction", typeof(InstrumentationLoopFunction))]
		[XmlElement("InstrumentComponent", typeof(InstrumentComponent))]
		[XmlElement("InsulationSymbol", typeof(InsulationSymbol))]
		[XmlElement("Label", typeof(Label))]
		[XmlElement("Nozzle", typeof(Nozzle))]
		[XmlElement("PipeConnectorSymbol", typeof(PipeConnectorSymbol))]
		[XmlElement("PipeFlowArrow", typeof(PipeFlowArrow))]
		[XmlElement("PipeSlopeSymbol", typeof(PipeSlopeSymbol))]
		[XmlElement("PipingComponent", typeof(PipingComponent))]
		[XmlElement("ProcessInstrument", typeof(ProcessInstrument))]
		[XmlElement("ProcessInstrumentationFunction", typeof(ProcessInstrumentationFunction))]
		[XmlElement("ProcessSignalGeneratingFunction", typeof(ProcessSignalGeneratingFunction))]
		[XmlElement("ProcessSignalGeneratingSystem", typeof(ProcessSignalGeneratingSystem))]
		[XmlElement("ProcessSignalGeneratingSystemComponent", typeof(ProcessSignalGeneratingSystemComponent))]
		[XmlElement("PropertyBreak", typeof(PropertyBreak))]
		[XmlElement("ScopeBubble", typeof(ScopeBubble))]
		[XmlElement("SignalConnectorSymbol", typeof(SignalConnectorSymbol))]
		[XmlElement("Symbol", typeof(Symbol))]
		public object[] Items
		{
			get
			{
				return this.itemsField;
			}
			set
			{
				this.itemsField = value;
			}
		}

		[XmlAttribute]
		public string Name
		{
			get
			{
				return this.nameField;
			}
			set
			{
				this.nameField = value;
			}
		}

		[XmlAttribute]
		public string Units
		{
			get
			{
				return this.unitsField;
			}
			set
			{
				this.unitsField = value;
			}
		}

		[XmlAttribute]
		public string Version
		{
			get
			{
				return this.versionField;
			}
			set
			{
				this.versionField = value;
			}
		}

		public ShapeCatalogue()
		{
		}
	}
}