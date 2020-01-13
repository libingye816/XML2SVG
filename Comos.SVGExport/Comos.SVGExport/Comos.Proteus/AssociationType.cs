using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Comos.Proteus
{
	[GeneratedCode("xsd", "4.6.1055.0")]
	[Serializable]
	[XmlType(AnonymousType=true)]
	public enum AssociationType
	{
		[XmlEnum("is about")]
		isabout,
		[XmlEnum("is a subject of")]
		isasubjectof,
		[XmlEnum("is associated with")]
		isassociatedwith,
		[XmlEnum("is a boundary of")]
		isaboundaryof,
		[XmlEnum("refers to")]
		refersto,
		[XmlEnum("is referenced by")]
		isreferencedby,
		[XmlEnum("is referenced in")]
		isreferencedin,
		describes,
		[XmlEnum("is described in")]
		isdescribedin,
		[XmlEnum("indirectly describes")]
		indirectlydescribes,
		[XmlEnum("is indirectly described in")]
		isindirectlydescribedin,
		defines,
		[XmlEnum("is defined in")]
		isdefinedin,
		[XmlEnum("is defined by")]
		isdefinedby,
		contains,
		[XmlEnum("is contained in")]
		iscontainedin,
		[XmlEnum("indirectly defines")]
		indirectlydefines,
		[XmlEnum("is indirectly defined in")]
		isindirectlydefinedin,
		[XmlEnum("is connected to")]
		isconnectedto,
		[XmlEnum("is logically connected to")]
		islogicallyconnectedto,
		[XmlEnum("has logical start")]
		haslogicalstart,
		[XmlEnum("is logical start of")]
		islogicalstartof,
		[XmlEnum("has logical end")]
		haslogicalend,
		[XmlEnum("is logical end of")]
		islogicalendof,
		[XmlEnum("is involved with role in")]
		isinvolvedwithrolein,
		[XmlEnum("is an activity with role involving")]
		isanactivitywithroleinvolving,
		[XmlEnum("is fulfilled by")]
		isfulfilledby,
		fulfills,
		[XmlEnum("is a part of")]
		isapartof,
		[XmlEnum("is an assembly including")]
		isanassemblyincluding,
		[XmlEnum("is a component of")]
		isacomponentof,
		[XmlEnum("is a composition including")]
		isacompositionincluding,
		[XmlEnum("is an element of")]
		isanelementof,
		[XmlEnum("is a collection including")]
		isacollectionincluding,
		[XmlEnum("is identified by")]
		isidentifiedby,
		[XmlEnum("is an identifier for")]
		isanidentifierfor,
		[XmlEnum("is a template including")]
		isatemplateincluding,
		[XmlEnum("is a component of template")]
		isacomponentoftemplate,
		[XmlEnum("is a template that refers to")]
		isatemplatethatrefersto,
		[XmlEnum("is a reference in template")]
		isareferenceintemplate,
		[XmlEnum("is classified as")]
		isclassifiedas,
		[XmlEnum("is incidentally classified as")]
		isincidentallyclassifiedas,
		[XmlEnum("has dataset")]
		hasdataset,
		[XmlEnum("is a dataset of")]
		isadatasetof,
		[XmlEnum("is a comment referring to")]
		isacommentreferringto,
		[XmlEnum("is referenced in comment")]
		isreferencedincomment,
		[XmlEnum("has document")]
		hasdocument,
		[XmlEnum("is a document for")]
		isadocumentfor,
		[XmlEnum("is the location of")]
		isthelocationof,
		[XmlEnum("is located in")]
		islocatedin,
		[XmlEnum("is upstream of")]
		isupstreamof,
		[XmlEnum("is downstream of")]
		isdownstreamof,
		[XmlEnum("is the responsibility of")]
		istheresponsibilityof
	}
}