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
	public class Equipment : PlantItem
	{
		private object[] items1Field;

		private Items1ChoiceType1[] items1ElementNameField;

		private string processAreaField;

		private string purposeField;

		[XmlChoiceIdentifier("Items1ElementName")]
		[XmlElement("Component", typeof(Comos.Proteus.Component))]
		[XmlElement("Discipline", typeof(Discipline))]
		[XmlElement("Equipment", typeof(Equipment))]
		[XmlElement("LowerLimitDesignPressure", typeof(PressureDouble))]
		[XmlElement("LowerLimitDesignTemperature", typeof(TemperatureDouble))]
		[XmlElement("MaximumDesignPressure", typeof(PressureDouble))]
		[XmlElement("MaximumDesignTemperature", typeof(TemperatureDouble))]
		[XmlElement("MinimumDesignPressure", typeof(PressureDouble))]
		[XmlElement("MinimumDesignTemperature", typeof(TemperatureDouble))]
		[XmlElement("Nozzle", typeof(Nozzle))]
		[XmlElement("UpperLimitDesignPressure", typeof(PressureDouble))]
		[XmlElement("UpperLimitDesignTemperature", typeof(TemperatureDouble))]
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
		public Items1ChoiceType1[] Items1ElementName
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
		public string ProcessArea
		{
			get
			{
				return this.processAreaField;
			}
			set
			{
				this.processAreaField = value;
			}
		}

		[XmlAttribute]
		public string Purpose
		{
			get
			{
				return this.purposeField;
			}
			set
			{
				this.purposeField = value;
			}
		}

		public Equipment()
		{
		}
	}
}