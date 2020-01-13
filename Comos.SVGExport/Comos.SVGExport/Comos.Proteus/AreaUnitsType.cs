using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Comos.Proteus
{
	[GeneratedCode("xsd", "4.6.1055.0")]
	[Serializable]
	public enum AreaUnitsType
	{
		[XmlEnum("sq mm")]
		sqmm,
		[XmlEnum("sq cm")]
		sqcm,
		[XmlEnum("sq dm")]
		sqdm,
		[XmlEnum("sq m")]
		sqm,
		[XmlEnum("sq km")]
		sqkm,
		[XmlEnum("sq in")]
		sqin,
		[XmlEnum("sq ft")]
		sqft,
		[XmlEnum("sq yd")]
		sqyd,
		MillimetreSquared,
		CentimetreSquared,
		DecimetreSquared,
		MetreSquared,
		KilometreSquared,
		InchSquared,
		FootSquared,
		YardSquared,
		MileSquared,
		Barn,
		MicrometreSquared,
		Are,
		Hectare,
		Acre,
		HundredFootSquared,
		UsSurveyMileSquared
	}
}