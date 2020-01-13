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
	public class CenterLine
	{
		private Comos.Proteus.Presentation presentationField;

		private Comos.Proteus.Extent extentField;

		private Comos.Proteus.Coordinate[] coordinateField;

		private Comos.Proteus.PersistentID[] persistentIDField;

		private Comos.Proteus.GenericAttributes[] genericAttributesField;

		private Comos.Proteus.Symbol symbolField;

		private string idField;

		private string numPointsField;

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

		[XmlAttribute(DataType="ID")]
		public string ID
		{
			get
			{
				return this.idField;
			}
			set
			{
				this.idField = value;
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

		[XmlElement("PersistentID")]
		public Comos.Proteus.PersistentID[] PersistentID
		{
			get
			{
				return this.persistentIDField;
			}
			set
			{
				this.persistentIDField = value;
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

		public Comos.Proteus.Symbol Symbol
		{
			get
			{
				return this.symbolField;
			}
			set
			{
				this.symbolField = value;
			}
		}

		public CenterLine()
		{
		}
	}
}