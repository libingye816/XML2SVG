using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Comos.Proteus
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("xsd", "4.6.1055.0")]
	[Serializable]
	[XmlRoot(Namespace="", IsNullable=false)]
	[XmlType(AnonymousType=true)]
	public class PlantModel
	{
		private Comos.Proteus.PlantInformation plantInformationField;

		private Comos.Proteus.Project projectField;

		private PlantModelRDLService[] rDLServiceField;

		private Comos.Proteus.Extent extentField;

		private object[] itemsField;

		public Comos.Proteus.Extent Extent
		{
			get
			{
				return this.extentField;
			}
			set
			{
				this.extentField = value;
			}
		}

		[XmlElement("ActuatingSystem", typeof(ActuatingSystem))]
		[XmlElement("Curve", typeof(Curve))]
		[XmlElement("Drawing", typeof(Drawing))]
		[XmlElement("Equipment", typeof(Equipment))]
		[XmlElement("InformationFlow", typeof(InformationFlow))]
		[XmlElement("InstrumentationLoopFunction", typeof(InstrumentationLoopFunction))]
		[XmlElement("InstrumentComponent", typeof(InstrumentComponent))]
		[XmlElement("InstrumentLoop", typeof(InstrumentLoop))]
		[XmlElement("PipingNetworkSystem", typeof(PipingNetworkSystem))]
		[XmlElement("PlantArea", typeof(PlantArea))]
		[XmlElement("Presentation", typeof(Presentation))]
		[XmlElement("ProcessInstrument", typeof(ProcessInstrument))]
		[XmlElement("ProcessInstrumentationFunction", typeof(ProcessInstrumentationFunction))]
		[XmlElement("ProcessSignalGeneratingSystem", typeof(ProcessSignalGeneratingSystem))]
		[XmlElement("ShapeCatalogue", typeof(ShapeCatalogue))]
		[XmlElement("SignalConnectorSymbol", typeof(SignalConnectorSymbol))]
		[XmlElement("Surface", typeof(Surface))]
		[XmlElement("Text", typeof(Text))]
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

		public Comos.Proteus.PlantInformation PlantInformation
		{
			get
			{
				return this.plantInformationField;
			}
			set
			{
				this.plantInformationField = value;
			}
		}

		public Comos.Proteus.Project Project
		{
			get
			{
				return this.projectField;
			}
			set
			{
				this.projectField = value;
			}
		}

		[XmlElement("RDLService", Form=XmlSchemaForm.Unqualified)]
		public PlantModelRDLService[] RDLService
		{
			get
			{
				return this.rDLServiceField;
			}
			set
			{
				this.rDLServiceField = value;
			}
		}

		public PlantModel()
		{
		}
	}
}