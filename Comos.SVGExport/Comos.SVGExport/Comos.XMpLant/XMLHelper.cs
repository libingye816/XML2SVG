using Comos.Global;
using System;
using System.Collections;
using System.Globalization;
using System.Xml;

namespace Comos.XMpLant
{
	internal class XMLHelper
	{
		public XMLHelper()
		{
		}

		internal static bool ChildElementExists(XmlElement xeParent, string strChildElementName)
		{
			bool count;
			try
			{
				count = xeParent.GetElementsByTagName(strChildElementName).Count > 0;
			}
			catch (Exception exception)
			{
				AppGlobal.StdErrorHandler(exception);
				return false;
			}
			return count;
		}

		internal static XmlElement CreateElementPipeConnectorSymbol(XmlDocument xdMainDoc, string strID, string strComponentClass = "PipeConnectorSymbol", string strComponentClassUri = "", string strComponentName = "", string strComponentType = "", string strRevision = "", string strRevisionUri = "", string strStatus = "", string strStatusUri = "")
		{
			XmlElement xmlElement;
			try
			{
				XmlElement xmlElement1 = xdMainDoc.CreateElement("PipeConnectorSymbol");
				xmlElement1.SetAttribute("ID", strID);
				xmlElement1.SetAttribute("ComponentClass", strComponentClass);
				if (!string.IsNullOrEmpty(strComponentClassUri))
				{
					xmlElement1.SetAttribute("ComponentClassURI", strComponentClassUri);
				}
				if (!string.IsNullOrEmpty(strComponentName))
				{
					xmlElement1.SetAttribute("ComponentName", strComponentName);
				}
				if (!string.IsNullOrEmpty(strComponentType))
				{
					xmlElement1.SetAttribute("ComponentType", strComponentType);
				}
				if (!string.IsNullOrEmpty(strRevision))
				{
					xmlElement1.SetAttribute("Revision", strRevision);
				}
				if (!string.IsNullOrEmpty(strRevisionUri))
				{
					xmlElement1.SetAttribute("Status", strRevisionUri);
				}
				if (!string.IsNullOrEmpty(strStatus))
				{
					xmlElement1.SetAttribute("Status", strStatus);
				}
				if (!string.IsNullOrEmpty(strStatusUri))
				{
					xmlElement1.SetAttribute("Status", strStatusUri);
				}
				xmlElement = xmlElement1;
			}
			catch (Exception exception)
			{
				AppGlobal.StdErrorHandler(exception);
				return null;
			}
			return xmlElement;
		}

		internal static bool CreateElementProcessInstrumentationFunctionGraphicalDummy(XmlDocument xdMainDoc, GetAttributeId newId, ref XmlElement xe, string strComponentClass = "OrphanInformationFlowCollectorDummy", string strComponentClassUri = "", string strComponentName = "", string strComponentType = "", string strRevision = "", string strRevisionUri = "", string strStatus = "", string strStatusUri = "")
		{
			bool flag;
			try
			{
				if (xe == null)
				{
					xe = xdMainDoc.CreateElement("ProcessInstrumentationFunction");
					XmlElement documentElement = xdMainDoc.DocumentElement;
					if (documentElement != null)
					{
						documentElement.AppendChild(xe);
					}
					else
					{
					}
				}
				xe.SetAttribute("ID", newId(false));
				xe.SetAttribute("ComponentClass", strComponentClass);
				if (!string.IsNullOrEmpty(strComponentClassUri))
				{
					xe.SetAttribute("ComponentClassURI", strComponentClassUri);
				}
				if (!string.IsNullOrEmpty(strComponentName))
				{
					xe.SetAttribute("ComponentName", strComponentName);
				}
				if (!string.IsNullOrEmpty(strComponentType))
				{
					xe.SetAttribute("ComponentType", strComponentType);
				}
				if (!string.IsNullOrEmpty(strRevision))
				{
					xe.SetAttribute("Revision", strRevision);
				}
				if (!string.IsNullOrEmpty(strRevisionUri))
				{
					xe.SetAttribute("Status", strRevisionUri);
				}
				if (!string.IsNullOrEmpty(strStatus))
				{
					xe.SetAttribute("Status", strStatus);
				}
				if (!string.IsNullOrEmpty(strStatusUri))
				{
					xe.SetAttribute("Status", strStatusUri);
				}
				flag = true;
			}
			catch (Exception exception)
			{
				AppGlobal.StdErrorHandler(exception);
				return false;
			}
			return flag;
		}

		internal static void CreateExtentByCoordinateChilds(XmlElement xeParent, CreateElementExtentDelegate ceed)
		{
			string str = string.Concat(".//", "Coordinate");
			double num = double.MaxValue;
			double num1 = double.MaxValue;
			double num2 = double.MinValue;
			double num3 = double.MinValue;
			foreach (object obj in xeParent.SelectNodes(str))
			{
				num = Math.Min(XMLHelper.ValueFromString(((XmlElement)obj).Attributes["X"].Value), num);
				num1 = Math.Min(XMLHelper.ValueFromString(((XmlElement)obj).Attributes["Y"].Value), num1);
				num2 = Math.Max(XMLHelper.ValueFromString(((XmlElement)obj).Attributes["X"].Value), num2);
				num3 = Math.Max(XMLHelper.ValueFromString(((XmlElement)obj).Attributes["Y"].Value), num3);
			}
			ceed(xeParent, num, num1, num2, num3);
		}

		internal static XmlElement GetConnectionPointsElement(XmlElement parentNode, XmlDocument xdMainDoc)
		{
			XmlElement xmlElement;
			if (parentNode == null || xdMainDoc == null)
			{
				return null;
			}
			try
			{
				XmlElement xmlElement1 = null;
				xmlElement1 = (parentNode.GetElementsByTagName("ConnectionPoints").Count != 0 ? (XmlElement)parentNode.GetElementsByTagName("ConnectionPoints").Item(0) : xdMainDoc.CreateElement("ConnectionPoints"));
				xmlElement = xmlElement1;
			}
			catch (Exception exception)
			{
				AppGlobal.StdErrorHandler(exception);
				return null;
			}
			return xmlElement;
		}

		internal static int GetConnectorNumberByName(XmlElement xeElement, string strConnectorName)
		{
			int num;
			try
			{
				if (xeElement["ConnectionPoints"] != null && xeElement["ConnectionPoints"]["GenericAttributes"] != null)
				{
					foreach (XmlElement childNode in xeElement["ConnectionPoints"]["GenericAttributes"].ChildNodes)
					{
						if (childNode.GetAttribute("Value") != strConnectorName)
						{
							continue;
						}
						int num1 = -1;
						int.TryParse(childNode.GetAttribute("Name").Substring("NodeName".Length), out num1);
						num = num1;
						return num;
					}
				}
				num = -1;
			}
			catch (Exception exception)
			{
				AppGlobal.StdErrorHandler(exception);
				num = -1;
			}
			return num;
		}

		internal static XmlElement GetElementByUID(enmXMpLantVersion xvExportVersion, XmlElement xeStartElement, string strSystemUID, bool fSearchGlobally = true)
		{
			XmlElement xmlElement;
			try
			{
				string str = "//";
				string str1 = "";
				if (xeStartElement != null)
				{
					if (!fSearchGlobally)
					{
						str = "./";
					}
					str1 = string.Concat(new string[] { str, "PersistentID", "[@", "Identifier", " = '", strSystemUID, "']/.." });
					XmlNodeList xmlNodeLists = xeStartElement.SelectNodes(str1);
					if (xmlNodeLists.Count != 1)
					{
						str1 = string.Concat(new string[] { str, "GenericAttributes", "/", "GenericAttribute", "[@", "Name", " = '", "SystemUID", "' and ", "@", "Value", " = '", strSystemUID, "']/../.." });
						if (xvExportVersion == enmXMpLantVersion.XMpLantVersion_3_2_0)
						{
							str1 = string.Concat(new string[] { str, "GenericAttributes", "/", "SystemUID", "[@", "Value", " = '", strSystemUID, "']/../.." });
						}
						xmlNodeLists = xeStartElement.SelectNodes(str1);
						if (xmlNodeLists.Count != 1)
						{
							xmlElement = null;
						}
						else
						{
							xmlElement = (XmlElement)xmlNodeLists.Item(0);
						}
					}
					else
					{
						xmlElement = (XmlElement)xmlNodeLists.Item(0);
					}
				}
				else
				{
					xmlElement = null;
				}
			}
			catch (Exception exception)
			{
				AppGlobal.StdErrorHandler(exception);
				xmlElement = null;
			}
			return xmlElement;
		}

		internal static int GetElementConnectorCount(XmlElement xeElement)
		{
			int count;
			if (xeElement == null)
			{
				return -1;
			}
			try
			{
				count = xeElement.GetElementsByTagName("Node").Count - 1;
			}
			catch (Exception exception)
			{
				AppGlobal.StdErrorHandler(exception);
				count = -1;
			}
			return count;
		}

		internal static double GetExtentValue(XmlElement xeParent, string elementMinMax, string attributeXYZ)
		{
			XmlNodeList xmlNodeLists = xeParent.SelectNodes(string.Concat(new string[] { "./", "Extent", "/", elementMinMax, "/@", attributeXYZ, "" }));
			if (xmlNodeLists != null && xmlNodeLists[0] != null)
			{
				double num = 0;
				if (double.TryParse(xmlNodeLists[0].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out num))
				{
					return num;
				}
			}
			return double.NaN;
		}

		internal static XmlElement GetGenericAttributeByName(string genericAttributeName, XmlElement plantItemNode)
		{
			XmlNodeList xmlNodeLists = plantItemNode.SelectNodes(string.Concat(new string[] { "./", "GenericAttributes", "/", "GenericAttribute", "[@", "Name", " = '", genericAttributeName, "']" }));
			if (xmlNodeLists.Count < 1)
			{
				return null;
			}
			return xmlNodeLists.Item(0) as XmlElement;
		}

		internal static XmlElement GetGenericAttributesSet(string strSetName, XmlElement plantItemNode)
		{
			try
			{
				if (!string.IsNullOrEmpty(strSetName))
				{
					XmlNodeList xmlNodeLists = plantItemNode.SelectNodes(string.Concat(new string[] { "./", "GenericAttributes", "[@", "Set", " = '", strSetName, "']" }));
					if (xmlNodeLists.Count > 0)
					{
						return xmlNodeLists.Item(0) as XmlElement;
					}
				}
			}
			catch (Exception exception)
			{
				AppGlobal.StdErrorHandler(exception);
			}
			return null;
		}

		internal static XmlElement GetParentNodePresentationElement(XmlElement xeParent)
		{
			XmlElement xmlElement;
			if (xeParent == null)
			{
				return null;
			}
			try
			{
				XmlElement firstChild = null;
				if (xeParent.FirstChild.Name != "Presentation")
				{
					foreach (XmlElement childNode in xeParent.ChildNodes)
					{
						if (childNode.Name != "Presentation")
						{
							continue;
						}
						firstChild = childNode;
					}
				}
				else
				{
					firstChild = (XmlElement)xeParent.FirstChild;
				}
				xmlElement = firstChild;
			}
			catch (Exception exception)
			{
				AppGlobal.StdErrorHandler(exception);
				xmlElement = null;
			}
			return xmlElement;
		}

		internal static string GetUIDOfElement(enmXMpLantVersion xvExportVersion, XmlElement XE)
		{
			string value;
			try
			{
				string str = string.Concat("./", "PersistentID");
				XmlNode xmlNodes = XE.SelectSingleNode(str);
				if (xmlNodes == null)
				{
					str = (xvExportVersion < enmXMpLantVersion.XMpLantVersion_3_3_3 ? string.Concat("./", "GenericAttributes", "/", "SystemUID") : string.Concat(new string[] { "./", "GenericAttributes", "/", "GenericAttribute", "[@", "Name", " = '", "SystemUID", "']" }));
					xmlNodes = XE.SelectSingleNode(str);
					value = (xmlNodes == null ? "" : xmlNodes.Attributes["Value"].Value);
				}
				else
				{
					value = xmlNodes.Attributes["Identifier"].Value;
				}
			}
			catch (Exception exception)
			{
				AppGlobal.StdErrorHandler(exception);
				value = "";
			}
			return value;
		}

		internal static string StringFromValue(double value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}

		internal static double ValueFromString(string value)
		{
			return Convert.ToDouble(value, CultureInfo.InvariantCulture);
		}
	}
}