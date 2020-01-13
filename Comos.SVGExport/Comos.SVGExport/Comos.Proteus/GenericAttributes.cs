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
	public class GenericAttributes
	{
		private GenericAttribute[] itemsField;

		private string[] textField;

		private string numberField;

		private string setField;

		[XmlElement("GenericAttribute")]
		public GenericAttribute[] Items
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

		[XmlAttribute(DataType="nonNegativeInteger")]
		public string Number
		{
			get
			{
				return this.numberField;
			}
			set
			{
				this.numberField = value;
			}
		}

		[XmlAttribute]
		public string Set
		{
			get
			{
				return this.setField;
			}
			set
			{
				this.setField = value;
			}
		}

		[XmlText]
		public string[] Text
		{
			get
			{
				return this.textField;
			}
			set
			{
				this.textField = value;
			}
		}

		public GenericAttributes()
		{
		}
	}
}