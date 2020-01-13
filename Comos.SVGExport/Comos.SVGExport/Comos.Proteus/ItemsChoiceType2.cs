using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Comos.Proteus
{
	[GeneratedCode("xsd", "4.6.1055.0")]
	[Serializable]
	[XmlType(IncludeInSchema=false)]
	public enum ItemsChoiceType2
	{
		ConnectionType,
		EndPreparation,
		GasketGap,
		GenericAttributes,
		InsideDiameter,
		NominalDiameter,
		OutsideDiameter,
		PersistentID,
		Position,
		Rating,
		ScheduleThickness,
		WeldType
	}
}