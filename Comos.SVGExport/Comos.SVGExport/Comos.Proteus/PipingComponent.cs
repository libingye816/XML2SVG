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
	public class PipingComponent : PlantItem
	{
		private object[] items1Field;

		private Items1ChoiceType[] items1ElementNameField;

		private string standardField;

		private string standardURIField;

		[XmlChoiceIdentifier("Items1ElementName")]
		[XmlElement("Component", typeof(Comos.Proteus.Component))]
		[XmlElement("ConnectionType", typeof(Comos.Proteus.String))]
		[XmlElement("FabricationCategory", typeof(FabricationCategory))]
		[XmlElement("InsideDiameter", typeof(LengthDouble))]
		[XmlElement("LargeDiameter", typeof(LengthDouble))]
		[XmlElement("NominalDiameter", typeof(LengthDouble))]
		[XmlElement("OperatorType", typeof(Comos.Proteus.String))]
		[XmlElement("OutsideDiameter", typeof(LengthDouble))]
		[XmlElement("PipingComponent", typeof(PipingComponent))]
		[XmlElement("Rating", typeof(Comos.Proteus.String))]
		[XmlElement("SmallDiameter", typeof(LengthDouble))]
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
		public Items1ChoiceType[] Items1ElementName
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

		[XmlAttribute]
		public string Standard
		{
			get
			{
				return this.standardField;
			}
			set
			{
				this.standardField = value;
			}
		}

		[XmlAttribute(DataType="anyURI")]
		public string StandardURI
		{
			get
			{
				return this.standardURIField;
			}
			set
			{
				this.standardURIField = value;
			}
		}

		public PipingComponent()
		{
		}
	}
}