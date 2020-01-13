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
	public class BsplineSurface : Surface
	{
		private object[] itemsField;

		private ItemsChoiceType1[] itemsElementNameField;

		private string degreeUField;

		private string degreeVField;

		private BsplineSurfaceSurfaceType surfaceTypeField;

		private BsplineSurfaceSurfaceForm surfaceFormField;

		private bool surfaceFormFieldSpecified;

		private string numControlPointsUField;

		private string numControlPointsVField;

		private string numKnotsUField;

		private string numKnotsVField;

		private bool closedUField;

		private bool closedUFieldSpecified;

		private bool closedVField;

		private bool closedVFieldSpecified;

		private bool selfIntersectField;

		private bool selfIntersectFieldSpecified;

		[XmlAttribute]
		public bool ClosedU
		{
			get
			{
				return this.closedUField;
			}
			set
			{
				this.closedUField = value;
			}
		}

		[XmlIgnore]
		public bool ClosedUSpecified
		{
			get
			{
				return this.closedUFieldSpecified;
			}
			set
			{
				this.closedUFieldSpecified = value;
			}
		}

		[XmlAttribute]
		public bool ClosedV
		{
			get
			{
				return this.closedVField;
			}
			set
			{
				this.closedVField = value;
			}
		}

		[XmlIgnore]
		public bool ClosedVSpecified
		{
			get
			{
				return this.closedVFieldSpecified;
			}
			set
			{
				this.closedVFieldSpecified = value;
			}
		}

		[XmlAttribute(DataType="integer")]
		public string DegreeU
		{
			get
			{
				return this.degreeUField;
			}
			set
			{
				this.degreeUField = value;
			}
		}

		[XmlAttribute(DataType="integer")]
		public string DegreeV
		{
			get
			{
				return this.degreeVField;
			}
			set
			{
				this.degreeVField = value;
			}
		}

		[XmlChoiceIdentifier("ItemsElementName")]
		[XmlElement("ControlPointArray", typeof(ControlPointArray), Form=XmlSchemaForm.Unqualified)]
		[XmlElement("Extent", typeof(Extent))]
		[XmlElement("GenericAttributes", typeof(GenericAttributes))]
		[XmlElement("KnotMultiplicitiesU", typeof(KnotMultiplicities), Form=XmlSchemaForm.Unqualified)]
		[XmlElement("KnotMultiplicitiesV", typeof(KnotMultiplicities), Form=XmlSchemaForm.Unqualified)]
		[XmlElement("KnotsU", typeof(Knots), Form=XmlSchemaForm.Unqualified)]
		[XmlElement("KnotsV", typeof(Knots), Form=XmlSchemaForm.Unqualified)]
		[XmlElement("Presentation", typeof(Presentation))]
		[XmlElement("Weights", typeof(Weights), Form=XmlSchemaForm.Unqualified)]
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

		[XmlElement("ItemsElementName")]
		[XmlIgnore]
		public ItemsChoiceType1[] ItemsElementName
		{
			get
			{
				return this.itemsElementNameField;
			}
			set
			{
				this.itemsElementNameField = value;
			}
		}

		[XmlAttribute(DataType="integer")]
		public string NumControlPointsU
		{
			get
			{
				return this.numControlPointsUField;
			}
			set
			{
				this.numControlPointsUField = value;
			}
		}

		[XmlAttribute(DataType="integer")]
		public string NumControlPointsV
		{
			get
			{
				return this.numControlPointsVField;
			}
			set
			{
				this.numControlPointsVField = value;
			}
		}

		[XmlAttribute(DataType="integer")]
		public string NumKnotsU
		{
			get
			{
				return this.numKnotsUField;
			}
			set
			{
				this.numKnotsUField = value;
			}
		}

		[XmlAttribute(DataType="integer")]
		public string NumKnotsV
		{
			get
			{
				return this.numKnotsVField;
			}
			set
			{
				this.numKnotsVField = value;
			}
		}

		[XmlAttribute]
		public bool SelfIntersect
		{
			get
			{
				return this.selfIntersectField;
			}
			set
			{
				this.selfIntersectField = value;
			}
		}

		[XmlIgnore]
		public bool SelfIntersectSpecified
		{
			get
			{
				return this.selfIntersectFieldSpecified;
			}
			set
			{
				this.selfIntersectFieldSpecified = value;
			}
		}

		[XmlAttribute]
		public BsplineSurfaceSurfaceForm SurfaceForm
		{
			get
			{
				return this.surfaceFormField;
			}
			set
			{
				this.surfaceFormField = value;
			}
		}

		[XmlIgnore]
		public bool SurfaceFormSpecified
		{
			get
			{
				return this.surfaceFormFieldSpecified;
			}
			set
			{
				this.surfaceFormFieldSpecified = value;
			}
		}

		[XmlAttribute]
		public BsplineSurfaceSurfaceType SurfaceType
		{
			get
			{
				return this.surfaceTypeField;
			}
			set
			{
				this.surfaceTypeField = value;
			}
		}

		public BsplineSurface()
		{
		}
	}
}