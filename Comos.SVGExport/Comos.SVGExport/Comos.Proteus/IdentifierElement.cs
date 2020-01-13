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
	public class IdentifierElement
	{
		private string nameField;

		private string valueField;

		private string itemIDField;

		private string dependantAttributeField;

		private IdentifierElementDependantAttributeContents dependantAttributeContentsField;

		[XmlAttribute]
		public string DependantAttribute
		{
			get
			{
				return this.dependantAttributeField;
			}
			set
			{
				this.dependantAttributeField = value;
			}
		}

		[DefaultValue(IdentifierElementDependantAttributeContents.ValueAndUnits)]
		[XmlAttribute]
		public IdentifierElementDependantAttributeContents DependantAttributeContents
		{
			get
			{
				return this.dependantAttributeContentsField;
			}
			set
			{
				this.dependantAttributeContentsField = value;
			}
		}

		[XmlAttribute(DataType="IDREF")]
		public string ItemID
		{
			get
			{
				return this.itemIDField;
			}
			set
			{
				this.itemIDField = value;
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

		public IdentifierElement()
		{
			this.dependantAttributeContentsField = IdentifierElementDependantAttributeContents.ValueAndUnits;
		}
	}
}