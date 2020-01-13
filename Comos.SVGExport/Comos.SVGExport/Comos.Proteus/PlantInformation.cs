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
	public class PlantInformation
	{
		private Comos.Proteus.UnitsOfMeasure unitsOfMeasureField;

		private string schemaVersionField;

		private string originatingSystemField;

		private string modelNameField;

		private DateTime dateField;

		private string projectNameField;

		private string projectCodeField;

		private string projectDescriptionField;

		private string companyNameField;

		private DateTime timeField;

		private string is3DField;

		private LengthUnitsType unitsField;

		private string disciplineField;

		[XmlAttribute]
		public string CompanyName
		{
			get
			{
				return this.companyNameField;
			}
			set
			{
				this.companyNameField = value;
			}
		}

		[XmlAttribute(DataType="date")]
		public DateTime Date
		{
			get
			{
				return this.dateField;
			}
			set
			{
				this.dateField = value;
			}
		}

		[XmlAttribute]
		public string Discipline
		{
			get
			{
				return this.disciplineField;
			}
			set
			{
				this.disciplineField = value;
			}
		}

		[XmlAttribute(DataType="NMTOKEN")]
		public string Is3D
		{
			get
			{
				return this.is3DField;
			}
			set
			{
				this.is3DField = value;
			}
		}

		[XmlAttribute]
		public string ModelName
		{
			get
			{
				return this.modelNameField;
			}
			set
			{
				this.modelNameField = value;
			}
		}

		[XmlAttribute]
		public string OriginatingSystem
		{
			get
			{
				return this.originatingSystemField;
			}
			set
			{
				this.originatingSystemField = value;
			}
		}

		[XmlAttribute]
		public string ProjectCode
		{
			get
			{
				return this.projectCodeField;
			}
			set
			{
				this.projectCodeField = value;
			}
		}

		[XmlAttribute]
		public string ProjectDescription
		{
			get
			{
				return this.projectDescriptionField;
			}
			set
			{
				this.projectDescriptionField = value;
			}
		}

		[XmlAttribute]
		public string ProjectName
		{
			get
			{
				return this.projectNameField;
			}
			set
			{
				this.projectNameField = value;
			}
		}

		[XmlAttribute]
		public string SchemaVersion
		{
			get
			{
				return this.schemaVersionField;
			}
			set
			{
				this.schemaVersionField = value;
			}
		}

		[XmlAttribute(DataType="time")]
		public DateTime Time
		{
			get
			{
				return this.timeField;
			}
			set
			{
				this.timeField = value;
			}
		}

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

		public Comos.Proteus.UnitsOfMeasure UnitsOfMeasure
		{
			get
			{
				return this.unitsOfMeasureField;
			}
			set
			{
				this.unitsOfMeasureField = value;
			}
		}

		public PlantInformation()
		{
			this.schemaVersionField = "4.0.1";
			this.is3DField = "no";
			this.disciplineField = "PID";
		}
	}
}