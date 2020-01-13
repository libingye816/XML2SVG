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
	public class Ellipse : Curve
	{
		private Comos.Proteus.Presentation presentationField;

		private Comos.Proteus.Extent extentField;

		private Comos.Proteus.Position positionField;

		private Comos.Proteus.GenericAttributes[] genericAttributesField;

		private double primaryAxisField;

		private double secondaryAxisField;

		private EllipseFilled filledField;

		private bool filledFieldSpecified;

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
		public EllipseFilled Filled
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

		public Comos.Proteus.Position Position
		{
			get
			{
				return this.positionField;
			}
			set
			{
				this.positionField = value;
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
		public double PrimaryAxis
		{
			get
			{
				return this.primaryAxisField;
			}
			set
			{
				this.primaryAxisField = value;
			}
		}

		[XmlAttribute]
		public double SecondaryAxis
		{
			get
			{
				return this.secondaryAxisField;
			}
			set
			{
				this.secondaryAxisField = value;
			}
		}

		public Ellipse()
		{
		}
	}
}