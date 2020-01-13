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
	public class InstrumentComponent : PlantItem
	{
		private object[] items1Field;

		private Items1ChoiceType3[] items1ElementNameField;

		[XmlChoiceIdentifier("Items1ElementName")]
		[XmlElement("Component", typeof(Comos.Proteus.Component))]
		[XmlElement("InsideDiameter", typeof(LengthDouble))]
		[XmlElement("InstrumentComponent", typeof(InstrumentComponent))]
		[XmlElement("NominalDiameter", typeof(LengthDouble))]
		[XmlElement("OperatorType", typeof(Comos.Proteus.String))]
		[XmlElement("OutsideDiameter", typeof(LengthDouble))]
		[XmlElement("WallThickness", typeof(LengthString))]
		public object[] Items1
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

		[XmlElement("Items1ElementName")]
		[XmlIgnore]
		public Items1ChoiceType3[] Items1ElementName
		{
			get
			{
				return this.items1ElementNameField;
			}
			set
			{
				this.items1ElementNameField = value;
			}
		}

		public InstrumentComponent()
		{
		}
	}
}