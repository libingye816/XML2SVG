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
	public class PipingNetworkSystem : PlantItem
	{
		private object[] items1Field;

		private Items1ChoiceType5[] items1ElementNameField;

		[XmlChoiceIdentifier("Items1ElementName")]
		[XmlElement("EndDiameter", typeof(LengthDouble))]
		[XmlElement("InsideDiameter", typeof(LengthDouble))]
		[XmlElement("LowerLimitDesignPressure", typeof(PressureDouble))]
		[XmlElement("LowerLimitDesignTemperature", typeof(TemperatureDouble))]
		[XmlElement("LowerLimitOperatingPressure", typeof(PressureDouble))]
		[XmlElement("LowerLimitOperatingTemperature", typeof(TemperatureDouble))]
		[XmlElement("MaximumDesignPressure", typeof(PressureDouble))]
		[XmlElement("MaximumDesignTemperature", typeof(TemperatureDouble))]
		[XmlElement("MaximumOperatingPressure", typeof(PressureDouble))]
		[XmlElement("MaximumOperatingTemperature", typeof(TemperatureDouble))]
		[XmlElement("MinimumDesignPressure", typeof(PressureDouble))]
		[XmlElement("MinimumDesignTemperature", typeof(TemperatureDouble))]
		[XmlElement("MinimumOperatingPressure", typeof(PressureDouble))]
		[XmlElement("MinimumOperatingTemperature", typeof(TemperatureDouble))]
		[XmlElement("NominalDesignPressure", typeof(PressureDouble))]
		[XmlElement("NominalDesignTemperature", typeof(TemperatureDouble))]
		[XmlElement("NominalDiameter", typeof(LengthDouble))]
		[XmlElement("NormalDesignPressure", typeof(PressureDouble))]
		[XmlElement("NormalDesignTemperature", typeof(TemperatureDouble))]
		[XmlElement("NormalOperatingPressure", typeof(PressureDouble))]
		[XmlElement("NormalOperatingTemperature", typeof(TemperatureDouble))]
		[XmlElement("OutsideDiameter", typeof(LengthDouble))]
		[XmlElement("PipingNetworkSegment", typeof(PipingNetworkSegment))]
		[XmlElement("PropertyBreak", typeof(PropertyBreak))]
		[XmlElement("StartDiameter", typeof(LengthDouble))]
		[XmlElement("TestPressure", typeof(PressureDouble))]
		[XmlElement("UpperLimitDesignPressure", typeof(PressureDouble))]
		[XmlElement("UpperLimitDesignTemperature", typeof(TemperatureDouble))]
		[XmlElement("UpperLimitOperatingPressure", typeof(PressureDouble))]
		[XmlElement("UpperLimitOperatingTemperature", typeof(TemperatureDouble))]
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
		public Items1ChoiceType5[] Items1ElementName
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

		public PipingNetworkSystem()
		{
		}
	}
}