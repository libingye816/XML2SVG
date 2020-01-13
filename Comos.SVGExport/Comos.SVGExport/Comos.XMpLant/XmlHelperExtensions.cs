using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace Comos.XMpLant
{
	internal static class XmlHelperExtensions
	{
		public static string GetXmlEnumAttributeValueFromEnum<TEnum>(this TEnum value)
		where TEnum : struct, IConvertible
		{
			Type type = typeof(TEnum);
			if (!type.IsEnum)
			{
				return null;
			}
			MemberInfo memberInfo = type.GetMember(value.ToString()).FirstOrDefault<MemberInfo>();
			if (memberInfo == null)
			{
				return null;
			}
			XmlEnumAttribute xmlEnumAttribute = memberInfo.GetCustomAttributes(false).OfType<XmlEnumAttribute>().FirstOrDefault<XmlEnumAttribute>();
			if (xmlEnumAttribute == null)
			{
				return null;
			}
			return xmlEnumAttribute.Name;
		}
	}
}