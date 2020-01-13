using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Comos.Proteus
{
	[GeneratedCode("xsd", "4.6.1055.0")]
	[Serializable]
	[XmlType(IncludeInSchema=false)]
	public enum ItemsChoiceType1
	{
		ControlPointArray,
		Extent,
		GenericAttributes,
		KnotMultiplicitiesU,
		KnotMultiplicitiesV,
		KnotsU,
		KnotsV,
		Presentation,
		Weights
	}
}