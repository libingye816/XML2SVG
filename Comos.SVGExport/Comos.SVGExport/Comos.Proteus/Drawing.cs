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
	public class Drawing
	{
		private Comos.Proteus.Presentation presentationField;

		private Comos.Proteus.Extent extentField;

		private object[] itemsField;

		private string nameField;

		private string typeField;

		private string revisionField;

		private string revisionURIField;

		private string titleField;

		private string sizeField;

		private string sizeURIField;

		private DrawingOrientation orientationField;

		private bool orientationFieldSpecified;

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

		[XmlElement("Component", typeof(Comos.Proteus.Component))]
		[XmlElement("Curve", typeof(Curve))]
		[XmlElement("DrawingBorder", typeof(DrawingBorder))]
		[XmlElement("GenericAttributes", typeof(GenericAttributes))]
		[XmlElement("InsulationSymbol", typeof(InsulationSymbol))]
		[XmlElement("Label", typeof(Label))]
		[XmlElement("PipeFlowArrow", typeof(PipeFlowArrow))]
		[XmlElement("PipeSlopeSymbol", typeof(PipeSlopeSymbol))]
		[XmlElement("PropertyBreak", typeof(PropertyBreak))]
		[XmlElement("ScopeBubble", typeof(ScopeBubble))]
		[XmlElement("Surface", typeof(Surface))]
		[XmlElement("Symbol", typeof(Symbol))]
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

		[XmlAttribute]
		public DrawingOrientation Orientation
		{
			get
			{
				return this.orientationField;
			}
			set
			{
				this.orientationField = value;
			}
		}

		[XmlIgnore]
		public bool OrientationSpecified
		{
			get
			{
				return this.orientationFieldSpecified;
			}
			set
			{
				this.orientationFieldSpecified = value;
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
		public string Revision
		{
			get
			{
				return this.revisionField;
			}
			set
			{
				this.revisionField = value;
			}
		}

		[XmlAttribute(DataType="anyURI")]
		public string RevisionURI
		{
			get
			{
				return this.revisionURIField;
			}
			set
			{
				this.revisionURIField = value;
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

		[XmlAttribute]
		public string Title
		{
			get
			{
				return this.titleField;
			}
			set
			{
				this.titleField = value;
			}
		}

		[XmlAttribute]
		public string Type
		{
			get
			{
				return this.typeField;
			}
			set
			{
				this.typeField = value;
			}
		}

		public Drawing()
		{
			this.typeField = "PID";
		}
	}
}