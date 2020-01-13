using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Comos.Proteus
{
	[GeneratedCode("xsd", "4.6.1055.0")]
	[Serializable]
	[XmlType(IncludeInSchema=false)]
	public enum Items1ChoiceType
	{
		Component,
		ConnectionType,
		FabricationCategory,
		InsideDiameter,
		LargeDiameter,
		NominalDiameter,
		OperatorType,
		OutsideDiameter,
		PipingComponent,
		Rating,
		SmallDiameter,
		WallThickness
	}
}