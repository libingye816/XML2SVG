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
	public class GenericAttribute
	{
		private string nameField;

		private string valueField;

		private string defaultValueField;

		private string unitsField;

		private GenericAttributeFormat formatField;

		private bool formatFieldSpecified;

		private string attributeURIField;

		private string valueURIField;

		private string unitsURIField;

		[XmlAttribute(DataType="anyURI")]
		public string AttributeURI
		{
			get
			{
				return this.attributeURIField;
			}
			set
			{
				this.attributeURIField = value;
			}
		}

		[XmlAttribute]
		public string DefaultValue
		{
			get
			{
				return this.defaultValueField;
			}
			set
			{
				this.defaultValueField = value;
			}
		}

		[XmlAttribute]
		public GenericAttributeFormat Format
		{
			get
			{
				return this.formatField;
			}
			set
			{
				this.formatField = value;
			}
		}

		[XmlIgnore]
		public bool FormatSpecified
		{
			get
			{
				return this.formatFieldSpecified;
			}
			set
			{
				this.formatFieldSpecified = value;
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

		public GenericAttribute()
		{
		}
	}
}