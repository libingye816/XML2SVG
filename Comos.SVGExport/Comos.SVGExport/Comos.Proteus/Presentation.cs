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
	public class Presentation
	{
		private string layerField;

		private string colorField;

		private string lineTypeField;

		private string lineWeightField;

		private double rField;

		private bool rFieldSpecified;

		private double gField;

		private bool gFieldSpecified;

		private double bField;

		private bool bFieldSpecified;

		[XmlAttribute]
		public double B
		{
			get
			{
				return this.bField;
			}
			set
			{
				this.bField = value;
			}
		}

		[XmlIgnore]
		public bool BSpecified
		{
			get
			{
				return this.bFieldSpecified;
			}
			set
			{
				this.bFieldSpecified = value;
			}
		}

		[XmlAttribute]
		public string Color
		{
			get
			{
				return this.colorField;
			}
			set
			{
				this.colorField = value;
			}
		}

		[XmlAttribute]
		public double G
		{
			get
			{
				return this.gField;
			}
			set
			{
				this.gField = value;
			}
		}

		[XmlIgnore]
		public bool GSpecified
		{
			get
			{
				return this.gFieldSpecified;
			}
			set
			{
				this.gFieldSpecified = value;
			}
		}

		[XmlAttribute]
		public string Layer
		{
			get
			{
				return this.layerField;
			}
			set
			{
				this.layerField = value;
			}
		}

		[XmlAttribute]
		public string LineType
		{
			get
			{
				return this.lineTypeField;
			}
			set
			{
				this.lineTypeField = value;
			}
		}

		[XmlAttribute]
		public string LineWeight
		{
			get
			{
				return this.lineWeightField;
			}
			set
			{
				this.lineWeightField = value;
			}
		}

		[XmlAttribute]
		public double R
		{
			get
			{
				return this.rField;
			}
			set
			{
				this.rField = value;
			}
		}

		[XmlIgnore]
		public bool RSpecified
		{
			get
			{
				return this.rFieldSpecified;
			}
			set
			{
				this.rFieldSpecified = value;
			}
		}

		public Presentation()
		{
		}
	}
}