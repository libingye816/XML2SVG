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
	public class BsplineCurve : Curve
	{
		private object[] itemsField;

		private ItemsChoiceType[] itemsElementNameField;

		private string degreeField;

		private BsplineCurveCurveType curveTypeField;

		private BsplineCurveCurveForm curveFormField;

		private bool curveFormFieldSpecified;

		private string numControlPointsField;

		private string numKnotsField;

		private bool closedField;

		private bool closedFieldSpecified;

		private bool selfIntersectField;

		private bool selfIntersectFieldSpecified;

		[XmlAttribute]
		public bool Closed
		{
			get
			{
				return this.closedField;
			}
			set
			{
				this.closedField = value;
			}
		}

		[XmlIgnore]
		public bool ClosedSpecified
		{
			get
			{
				return this.closedFieldSpecified;
			}
			set
			{
				this.closedFieldSpecified = value;
			}
		}

		[XmlAttribute]
		public BsplineCurveCurveForm CurveForm
		{
			get
			{
				return this.curveFormField;
			}
			set
			{
				this.curveFormField = value;
			}
		}

		[XmlIgnore]
		public bool CurveFormSpecified
		{
			get
			{
				return this.curveFormFieldSpecified;
			}
			set
			{
				this.curveFormFieldSpecified = value;
			}
		}

		[XmlAttribute]
		public BsplineCurveCurveType CurveType
		{
			get
			{
				return this.curveTypeField;
			}
			set
			{
				this.curveTypeField = value;
			}
		}

		[XmlAttribute(DataType="integer")]
		public string Degree
		{
			get
			{
				return this.degreeField;
			}
			set
			{
				this.degreeField = value;
			}
		}

		[XmlChoiceIdentifier("ItemsElementName")]
		[XmlElement("ControlPoints", typeof(ControlPoints), Form=XmlSchemaForm.Unqualified)]
		[XmlElement("Extent", typeof(Extent))]
		[XmlElement("GenericAttributes", typeof(GenericAttributes))]
		[XmlElement("KnotMultiplicities", typeof(KnotMultiplicities), Form=XmlSchemaForm.Unqualified)]
		[XmlElement("Knots", typeof(Knots), Form=XmlSchemaForm.Unqualified)]
		[XmlElement("Presentation", typeof(Presentation))]
		[XmlElement("WeightsData", typeof(WeightsData), Form=XmlSchemaForm.Unqualified)]
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
		public ItemsChoiceType[] ItemsElementName
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
		public string NumControlPoints
		{
			get
			{
				return this.numControlPointsField;
			}
			set
			{
				this.numControlPointsField = value;
			}
		}

		[XmlAttribute(DataType="integer")]
		public string NumKnots
		{
			get
			{
				return this.numKnotsField;
			}
			set
			{
				this.numKnotsField = value;
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

		public BsplineCurve()
		{
		}
	}
}