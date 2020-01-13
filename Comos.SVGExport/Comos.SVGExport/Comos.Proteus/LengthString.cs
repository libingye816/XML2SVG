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
	[XmlRoot("ScheduleThickness", Namespace="", IsNullable=false)]
	public class LengthString
	{
		private string valueField;

		private LengthUnitsType unitsField;

		private bool unitsFieldSpecified;

		private string valueURIField;

		private string unitsURIField;

		[XmlAttribute]
		public LengthUnitsType Units
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

		[XmlIgnore]
		public bool UnitsSpecified
		{
			get
			{
				return this.unitsFieldSpecified;
			}
			set
			{
				this.unitsFieldSpecified = value;
			}
		}

		[XmlAttribute(DataType="anyURI")]
		public string UnitsURI
		{
			get
			{
				return this.unitsURIField;
			}
			set
			{
				this.unitsURIField = value;
			}
		}

		[XmlAttribute]
		public string Value
		{
			get
			{
				return this.valueField;
			}
			set
			{
				this.valueField = value;
			}
		}

		[XmlAttribute(DataType="anyURI")]
		public string ValueURI
		{
			get
			{
				return this.valueURIField;
			}
			set
			{
				this.valueURIField = value;
			}
		}

		public LengthString()
		{
		}
	}
}