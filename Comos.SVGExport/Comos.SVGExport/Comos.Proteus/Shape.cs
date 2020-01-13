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
	public class Shape : Curve
	{
		private Comos.Proteus.Presentation presentationField;

		private Comos.Proteus.Extent extentField;

		private Comos.Proteus.Coordinate[] coordinateField;

		private Comos.Proteus.GenericAttributes[] genericAttributesField;

		private string numPointsField;

		private ShapeFilled filledField;

		private bool filledFieldSpecified;

		[XmlElement("Coordinate")]
		public Comos.Proteus.Coordinate[] Coordinate
		{
			get
			{
				return this.coordinateField;
			}
			set
			{
				this.coordinateField = value;
			}
		}

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

		[XmlAttribute]
		public ShapeFilled Filled
		{
			get
			{
				return this.filledField;
			}
			set
			{
				this.filledField = value;
			}
		}

		[XmlIgnore]
		public bool FilledSpecified
		{
			get
			{
				return this.filledFieldSpecified;
			}
			set
			{
				this.filledFieldSpecified = value;
			}
		}

		[XmlElement("GenericAttributes")]
		public Comos.Proteus.GenericAttributes[] GenericAttributes
		{
			get
			{
				return this.genericAttributesField;
			}
			set
			{
				this.genericAttributesField = value;
			}
		}

		[XmlAttribute(DataType="nonNegativeInteger")]
		public string NumPoints
		{
			get
			{
				return this.numPointsField;
			}
			set
			{
				this.numPointsField = value;
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

		public Shape()
		{
		}
	}
}