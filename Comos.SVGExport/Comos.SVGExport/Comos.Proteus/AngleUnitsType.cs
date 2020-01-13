using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Comos.Proteus
{
	[GeneratedCode("xsd", "4.6.1055.0")]
	[Serializable]
	public enum AngleUnitsType
	{
		rad,
		deg,
		Radian,
		[XmlEnum("Degree-angle")]
		Degreeangle,
		CentesimalSecond,
		CentesimalMinute,
		[XmlEnum("Second-angle")]
		Secondangle,
		[XmlEnum("Minute-angle")]
		Minuteangle,
		Microradian,
		Milliradian,
		Kiloradian,
		Megaradian,
		Gigaradian,
		Mil_6400Radian,
		Cycle,
		Iso2041Cycle,
		DecimalDegree
	}
}