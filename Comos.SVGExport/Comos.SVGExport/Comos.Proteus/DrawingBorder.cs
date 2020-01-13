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
	public class DrawingBorder
	{
		private Comos.Proteus.Presentation presentationField;

		private Comos.Proteus.Extent extentField;

		private object[] itemsField;

		private string nameField;

		private string sizeField;

		private string sizeURIField;

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

		[XmlElement("Curve", typeof(Curve))]
		[XmlElement("GenericAttributes", typeof(GenericAttributes))]
		[XmlElement("Label", typeof(Label))]
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

		public Comos.Proteus.Presentation Presentation
		{
			get
			{
				return this.presentationField;
			}
			set
			{
				this.presentationField = value;
			}
		}

		[XmlAttribute]
		public string Size
		{
			get
			{
				return this.sizeField;
			}
			set
			{
				this.sizeField = value;
			}
		}

		[XmlAttribute(DataType="anyURI")]
		public string SizeURI
		{
			get
			{
				return this.sizeURIField;
			}
			set
			{
				this.sizeURIField = value;
			}
		}

		public DrawingBorder()
		{
		}
	}
}