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
	public class Text
	{
		private object[] itemsField;

		private string numLinesField;

		private string stringField;

		private string fontField;

		private TextJustification justificationField;

		private double widthField;

		private double heightField;

		private double textAngleField;

		private bool textAngleFieldSpecified;

		private double slantAngleField;

		private bool slantAngleFieldSpecified;

		private string itemIDField;

		private string setField;

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

		[XmlAttribute]
		public string Font
		{
			get
			{
				return this.fontField;
			}
			set
			{
				this.fontField = value;
			}
		}

		[XmlAttribute]
		public double Height
		{
			get
			{
				return this.heightField;
			}
			set
			{
				this.heightField = value;
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

		[XmlElement("Extent", typeof(Extent))]
		[XmlElement("GenericAttributes", typeof(GenericAttributes))]
		[XmlElement("Position", typeof(Comos.Proteus.Position))]
		[XmlElement("Presentation", typeof(Presentation))]
		[XmlElement("String", typeof(TextString), Form=XmlSchemaForm.Unqualified)]
		[XmlElement("TextStringFormatSpecification", typeof(TextStringFormatSpecification))]
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

		[DefaultValue(TextJustification.LeftBottom)]
		[XmlAttribute]
		public TextJustification Justification
		{
			get
			{
				return this.justificationField;
			}
			set
			{
				this.justificationField = value;
			}
		}

		[XmlAttribute(DataType="nonNegativeInteger")]
		public string NumLines
		{
			get
			{
				return this.numLinesField;
			}
			set
			{
				this.numLinesField = value;
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

		[XmlAttribute]
		public double SlantAngle
		{
			get
			{
				return this.slantAngleField;
			}
			set
			{
				this.slantAngleField = value;
			}
		}

		[XmlIgnore]
		public bool SlantAngleSpecified
		{
			get
			{
				return this.slantAngleFieldSpecified;
			}
			set
			{
				this.slantAngleFieldSpecified = value;
			}
		}

		[XmlAttribute]
		public string String
		{
			get
			{
				return this.stringField;
			}
			set
			{
				this.stringField = value;
			}
		}

		[XmlAttribute]
		public double TextAngle
		{
			get
			{
				return this.textAngleField;
			}
			set
			{
				this.textAngleField = value;
			}
		}

		[XmlIgnore]
		public bool TextAngleSpecified
		{
			get
			{
				return this.textAngleFieldSpecified;
			}
			set
			{
				this.textAngleFieldSpecified = value;
			}
		}

		[XmlAttribute]
		public double Width
		{
			get
			{
				return this.widthField;
			}
			set
			{
				this.widthField = value;
			}
		}

		public Text()
		{
			this.justificationField = TextJustification.LeftBottom;
			this.dependantAttributeContentsField = IdentifierElementDependantAttributeContents.ValueAndUnits;
		}
	}
}