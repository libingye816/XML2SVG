using Comos.CSMEnum;
using Comos.Global;
using Comos.Global.CExtensions;
using Comos.Intern.DevTools;
using Comos.Piping.Generic;
using Comos.Proteus;
using Comos.ReportAnalysis;
using Comos.SolidBaseUtilities;
using Comos.WSP.Interfaces.Utilities.ObjectCreation;
using Comos.WSP.Interfaces.Utilities.ObjectCreation.Creators;
using Comos.WSP.PfdMod;
using Comos.WSP.RoDevicePfd.Main;
using Comos.WSP.RoUtilities;
using Comos.WSP.RoUtilities.Common.Shared;
using Comos.XMpLant;
using Comos3D;
//using ComosImportInterface;
using ComosLib;
using ComosVBInterface;
using ComosWSPInterface;
using Iso15926SemanticVerification;
using Iso15926SemanticVerification.ResultLog;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32;
using Plt;
using ProgressLogger2;
using REPORTLib;
using stdole;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Xml;
using System.Xml.Schema;
using System.Diagnostics;
using System.Text;

namespace Comos.SVGExport
{
    [GuidAttribute("DB37B63B-C805-49F9-A6FC-BC90E99F8B98")]
    [ComVisible(true)]
    public interface ISvgExport
    {
        [DispId(1)]
        int GetVersion();
        [DispId(2)]
        void Export(Document doc, string ExportFileName, string INIFileName, string ExportVersion = "3.3.3");
        //void Export(Document doc, IXReport XRep, string ExportFileName, string INIFileName, string ExportVersion = "3.3.3");


        [DispId(3)]
        bool Unattended { get; set; }

        [DispId(4)]
        string EvaluateObject(object obj);
    }

    [GuidAttribute("58DEB25D-5637-49CB-9F0A-FD9A98325FD3")]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [ComDefaultInterface(typeof(ISvgExport))]
    public class SvgExport : ISvgExport
    {
        private static I3DMath M3D;

        private double PI = SvgExport.M3D.PI();

        private double dblFactorDTPmm = 0.352777777777778;

        private ArrayList m_StockNumbers = new ArrayList();

        private int m_xmp = 1;

        private int m_xmc = 1;

        private double m_ComosDocumentSizeY;

        private Document m_rDoc;

        private bool m_fUnattended;

        private bool m_MessageBoxAlreadyDisplayed;

        private string m_strSaveFileName = "";

        private IPfdMod m_PfdMod;

        private IXReport m_xRep;

        private IComosDWorkset m_Workset;

        private ReportAnalyzer m_ReportExport = new ReportAnalyzer();

        private ArrayList m_arlLineTypeMapping = new ArrayList();

        private XmlDocument m_xmlDoc;

        private XmlElement m_Root;

        private XmlElement m_Catalogue;

        private XmlElement m_DrawingBorder;

        private XmlElement m_Drawing;

        private IDictionary<string, string> m_dicIncludeInBranchUIDs = new Dictionary<string, string>();

        private ArrayList m_arlSignalLines = new ArrayList();

        private ArrayList m_arlReorderElements = new ArrayList();

        private XMpLantExportLogging m_Logging;

        private enmXMpLantVersion m_xvExportVersion = enmXMpLantVersion.XMpLantVersion_3_3_3;

        private string m_SymbolType = "";

        private IComosDStandardTable m_stUnitMapping;

        private RunningNumber m_ShapeCatalogueNumber = new RunningNumber(0);

        private XmlElement m_xeProcessInstrumentationFunctionGraphicalDummy;

        private bool m_fUseOldLineTypeDefault;

        private Dictionary<string, IComosDDevice> m_dictCollectedNozzles = new Dictionary<string, IComosDDevice>();

        private readonly Dictionary<string, string> _dictProteusPlantItems = new Dictionary<string, string>()
        {
            { "ActuatingFunction", string.Empty },
            { "ActuatingSystem", string.Empty },
            { "ActuatingSystemComponent", string.Empty },
            { "Component", string.Empty },
            { "Equipment", string.Empty },
            { "InformationFlow", string.Empty },
            { "InstrumentationLoopFunction", string.Empty },
            { "InstrumentComponent", string.Empty },
            { "InstrumentConnection", string.Empty },
            { "InstrumentLoop", string.Empty },
            { "Nozzle", string.Empty },
            { "PipingComponent", string.Empty },
            { "PipingNetworkSegment", string.Empty },
            { "PipingNetworkSystem", string.Empty },
            { "PlantArea", string.Empty },
            { "PlantItem", string.Empty },
            { "ProcessInstrument", string.Empty },
            { "ProcessInstrumentationFunction", string.Empty },
            { "ProcessSignalGeneratingFunction", string.Empty },
            { "ProcessSignalGeneratingSystem", string.Empty },
            { "ProcessSignalGeneratingSystemComponent", string.Empty },
            { "Project", string.Empty }
        };

        private readonly Dictionary<string, string> _dictProteusAnnotationItems = new Dictionary<string, string>()
        {
            { "InsulationSymbol", string.Empty },
            { "PipeConnectorSymbol", string.Empty },
            { "PipeFlowArrow", string.Empty },
            { "PipeSlopeSymbol", string.Empty },
            { "PropertyBreak", string.Empty },
            { "ScopeBubble", string.Empty },
            { "SignalConnectorSymbol", string.Empty },
            { "Symbol", string.Empty }
        };

        private const string COMOS_GLOBALS_XMPLANT_EXPORT_UNATTENDED = "XMpLantExportUnattended";

        private const string COMOS_GLOBALS_XMPLANT_EXPORT_VERSION_3_2_0 = "XMpLantExportVersion_3_2_0";

        private const double COLOR_CHANNEL_MAXIMUM = 255;

        private const double COLOR_WHITE = 16777215;

        private const float COMOS_TEXT_ROTATION_MODE_MIN = 91f;

        private const float COMOS_TEXT_ROTATION_MODE_MAX = 271f;

        private static string COMOS_CONNECTOR_STANDARD_INPUT
        {
            get
            {
                return AppGlobal.GetDBStr(DBKey.I1__DI1);
            }
        }

        private static string COMOS_CONNECTOR_STANDARD_OUTPUT
        {
            get
            {
                return AppGlobal.GetDBStr(DBKey.O1__DO1);
            }
        }

        private static string COMOS_SYSTEM_BEHAVIOR_MULTI_FLOW_DIR
        {
            get
            {
                return AppGlobal.GetDBStr(DBKey.BehaviorMultiFlowDir__Y00A04491);
            }
        }

        private static string COMOS_SYSTEM_CHAPTER_NAME
        {
            get
            {
                return AppGlobal.GetDBStr(DBKey.SYS__Y00T00001);
            }
        }

        private static string COMOS_SYSTEM_FROM_OBJ
        {
            get
            {
                return AppGlobal.GetDBStr(DBKey.FromObj__Y00A02607);
            }
        }

        private static string COMOS_SYSTEM_TO_OBJ
        {
            get
            {
                return AppGlobal.GetDBStr(DBKey.ToObj__Y00A02577);
            }
        }

        private static string COMOS_TECHNICAL_DATA_CHAPTER_NAME
        {
            get
            {
                return AppGlobal.GetDBStr(DBKey.PI030__Y00T00003);
            }
        }

        private static string COMOS_TECHNICAL_DATA_NOMINAL_WIDTH
        {
            get
            {
                return AppGlobal.GetDBStr(DBKey.PIA008__Y00A00744);
            }
        }

        private static string COMOS_XMPLANT_CHAPTER_NAME
        {
            get
            {
                return AppGlobal.GetDBStr(DBKey.XMpLant__Y00T00277);
            }
        }

        private static string COMOS_XMPLANT_COMOS_NESTED_NAME
        {
            get
            {
                return AppGlobal.GetDBStr(DBKey.ComosNestedName__Y00A01139);
            }
        }

        private static string COMOS_XMPLANT_COMOS_NESTED_NAMES
        {
            get
            {
                return AppGlobal.GetDBStr(DBKey.ComosNestedNames__Y00A01139);
            }
        }

        private static string COMOS_XMPLANT_COMPONENT_CLASS
        {
            get
            {
                return AppGlobal.GetDBStr(DBKey.ComponentClass__Y00A02635);
            }
        }

        private static string COMOS_XMPLANT_COMPONENT_CLASS_URI
        {
            get
            {
                return AppGlobal.GetDBStr(DBKey.ComponentClassURI__Y00A01084);
            }
        }

        private static string COMOS_XMPLANT_COMPONENT_NAME
        {
            get
            {
                return AppGlobal.GetDBStr(DBKey.ComponentName__Y00A02636);
            }
        }

        private static string COMOS_XMPLANT_COMPONENT_TYPE
        {
            get
            {
                return AppGlobal.GetDBStr(DBKey.ComponentType__Y00A02637);
            }
        }

        private static string COMOS_XMPLANT_DOCUMENT_NAME
        {
            get
            {
                return AppGlobal.GetDBStr(DBKey.DocumentName__Y00A01154);
            }
        }

        private static string COMOS_XMPLANT_EXPORT_BORDER
        {
            get
            {
                return AppGlobal.GetDBStr(DBKey.ExportBorder__Y00A00734);
            }
        }

        private static string COMOS_XMPLANT_EXPORT_LISTED_ATTRIBUTES
        {
            get
            {
                return AppGlobal.GetDBStr(DBKey.ExportListedAttributes__Y00A02626);
            }
        }

        private static string COMOS_XMPLANT_FIXED_XMPLANT_ATTRIBUTES
        {
            get
            {
                return AppGlobal.GetDBStr(DBKey.FixedXMpLantAttributes__Y00A01109);
            }
        }

        private static string COMOS_XMPLANT_GENERIC_ATTRIBUTES_LIST
        {
            get
            {
                return AppGlobal.GetDBStr(DBKey.GenericAttributesList__Y00A00042);
            }
        }

        private static string COMOS_XMPLANT_ISO_MAPPING_TABLE
        {
            get
            {
                return AppGlobal.GetDBStr(DBKey.Y00A05664__Y00A05664);
            }
        }

        private static string COMOS_XMPLANT_PERSISTENT_ID
        {
            get
            {
                return AppGlobal.GetDBStr(DBKey.PersistentID__Y00A01524);
            }
        }

        private static string COMOS_XMPLANT_PLANT_ITEM_NODE_NAME
        {
            get
            {
                return AppGlobal.GetDBStr(DBKey.PlantItemNodeName__Y00A02645);
            }
        }

        private static string COMOS_XMPLANT_REVISION
        {
            get
            {
                return AppGlobal.GetDBStr(DBKey.Revision__Y00A00825);
            }
        }

        private static string COMOS_XMPLANT_SPECIFICATION
        {
            get
            {
                return AppGlobal.GetDBStr(DBKey.Specification__Y00A01078);
            }
        }

        private static string COMOS_XMPLANT_STATUS
        {
            get
            {
                return AppGlobal.GetDBStr(DBKey.Status__Y00A01088);
            }
        }

        private static string COMOS_XMPLANT_STOCK_NUMBER
        {
            get
            {
                return AppGlobal.GetDBStr(DBKey.StockNumber__Y00A00941);
            }
        }

        private static string COMOS_XMPLANT_TAG
        {
            get
            {
                return AppGlobal.GetDBStr(DBKey.Tag__Y00A02652);
            }
        }

        [Obsolete("ADNOJO6, F37349, 28.06.2013: TagName doesn't exist in Comos DB!")]
        private static string COMOS_XMPLANT_TAG_NAME
        {
            get
            {
                return AppGlobal.GetDBStr(DBKey.TagName__Y00A02652);
            }
        }

        private static string COMOS_XMPLANT_XMPLANT_ATTRIBUTE_NAME
        {
            get
            {
                return AppGlobal.GetDBStr(DBKey.XMpLantAttributeName__Y00A04280);
            }
        }

        bool ISvgExport.Unattended
        {
            get
            {
                return this.m_fUnattended;
            }
            set
            {
                this.m_fUnattended = value;
            }
        }

        public LogFile EngineeringAdapterLogFile
        {
            get;
            set;
        }

        public string FileName
        {
            get
            {
                return this.m_strSaveFileName;
            }
            set
            {
                this.m_strSaveFileName = value;
            }
        }

        public HierarchicalTask ParentTask
        {
            get;
            set;
        }

        public string ValidationFilesPath { get; set; } = string.Empty;

        public enmXMpLantVersion XMpLantVersion
        {
            get
            {
                return this.m_xvExportVersion;
            }
            set
            {
                this.m_xvExportVersion = value;
            }
        }

        // Use type object for script calls.
        public string EvaluateObject(object obj)
        {
            Debugger.Launch();
            try
            {
                return EvaluateObject(obj as IComosBaseObject);
            }
            catch { }

            return "";
        }

        // Use safe type for internal processing.
        internal string EvaluateObject(IComosBaseObject obj)
        {
            if (obj == null)
                return "";

            string res = obj.Name;

            if (!System.String.IsNullOrEmpty(obj.Description))
                res += " " + obj.Description;

            return res;
        }

        static SvgExport()
        {
            SvgExport.M3D = ObjectCreation.WspObjectCreator.Math3D.Create();
        }

        public SvgExport()
        {
            this.m_ReportExport.FoundReportLine += new ReportAnalyzer.FoundReportLineHandler(this.ReportExport_FoundReportLine);
            this.m_ReportExport.FoundReportArc += new ReportAnalyzer.FoundReportArcHandler(this.ReportExport_FoundReportArc);
            this.m_ReportExport.FoundReportText += new ReportAnalyzer.FoundReportTextHandler(this.ReportExport_FoundReportText);
        }

        private void _CollectMisleadPipingComponents()
        {
            try
            {
                XmlElement xmlElement = this.m_xmlDoc.CreateElement("PipingNetworkSystem");
                xmlElement.SetAttribute("ID", this.GetAttributeID(false));
                xmlElement.SetAttribute("ComponentClass", "OrphanPipingNetworkSystem");
                XmlElement xmlElement1 = this.m_xmlDoc.CreateElement("PipingNetworkSegment");
                xmlElement1.SetAttribute("ID", this.GetAttributeID(false));
                xmlElement1.SetAttribute("ComponentClass", "OrphanPipingNetworkSegment");
                xmlElement.AppendChild(xmlElement1);
                ArrayList arrayLists = new ArrayList();
                foreach (XmlElement mRoot in this.m_Root)
                {
                    if (mRoot.Name != "PipingComponent")
                    {
                        continue;
                    }
                    arrayLists.Add(mRoot);
                }
                if (arrayLists.Count > 0)
                {
                    foreach (XmlElement arrayList in arrayLists)
                    {
                        xmlElement1.AppendChild(arrayList);
                    }
                }
                if (xmlElement1.ChildNodes.Count > 0)
                {
                    this.m_Root.AppendChild(xmlElement);
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
        }

        private string _getComponentName(Item reportItem)
        {
            try
            {
                REPORTLib.Object obj = reportItem as REPORTLib.Object;
                REPORTLib.Object obj1 = obj;
                if (obj != null && obj1.XObj is IPfdRoDevice)
                {
                    PfdRoDeviceMain xObj = obj1.XObj as PfdRoDeviceMain;
                    bool flag = this.HasLocalSymbolScript(reportItem);
                    return string.Concat(xObj.GraphicalHashCode.ToString(), (flag ? "_" : ""), (flag ? this.m_ShapeCatalogueNumber.GetNextNumber().ToString() : ""));
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
            return string.Empty;
        }

        private void _getLineTypeMappingFromStandardTable()
        {
            try
            {
                this.m_arlLineTypeMapping.Clear();
                IComosDStandardTable objectOfType = ComosObjectReference.StandardTableForProteusLineTypeMpping.GetObjectOfType<IComosDStandardTable>();
                if (objectOfType != null)
                {
                    IComosDOwnCollection comosDOwnCollection = objectOfType.StandardValues();
                    int num = comosDOwnCollection.Count();
                    for (int i = 1; i <= num; i++)
                    {
                        IComosDStandardValue comosDStandardValue = comosDOwnCollection.Item(i) as IComosDStandardValue;
                        string str = comosDStandardValue.GetXValue(0).Trim();
                        string str1 = comosDStandardValue.GetXValue(1).Trim();
                        if (str != "" && str.ToUpper() != "X" && str1 != "")
                        {
                            SvgExport.m_stcLineTypeMappingItem mStcLineTypeMappingItem = new SvgExport.m_stcLineTypeMappingItem(str, str1);
                            this.m_arlLineTypeMapping.Add(mStcLineTypeMappingItem);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
        }

        private void _reorderPipingNozzles()
        {
            foreach (IComosDDevice value in this.m_dictCollectedNozzles.Values)
            {
                IComosDDevice comosDDevice = value.owner() as IComosDDevice;
                IComosDDevice comosDDevice1 = comosDDevice;
                if (comosDDevice == null || !this.IsComosBranch(comosDDevice1))
                {
                    continue;
                }
                XmlElement elementByUID = XMLHelper.GetElementByUID(this.m_xvExportVersion, this.m_Root, value.SystemUID(), true);
                XmlElement xmlElement = elementByUID;
                if (elementByUID == null)
                {
                    continue;
                }
                XmlElement elementByUID1 = XMLHelper.GetElementByUID(this.m_xvExportVersion, this.m_Root, comosDDevice1.SystemUID(), true);
                XmlElement xmlElement1 = elementByUID1;
                if (elementByUID1 == null)
                {
                    continue;
                }
                xmlElement1.InsertAfter(xmlElement, xmlElement1.LastChild);
            }
        }

        private bool _signalLinesConnected(object XObj)
        {
            IRoConnectors roConnector = (XObj as IConnectedRO).Connectors();
            if (roConnector.Count >= 2)
            {
                IRoConnector roConnector1 = roConnector.Item(1);
                IRoConnector roConnector2 = roConnector.Item(2);
                if (roConnector1 != null && roConnector1.ConnectedWith != null && roConnector2 != null && roConnector2.ConnectedWith != null)
                {
                    return true;
                }
            }
            return false;
        }

        private void AddElementsToReorderCollection(SvgExport.m_ReorderElements elements)
        {
            if (elements == null || elements.PreviousElement == null || elements.NextElement == null || elements.PreviousElement.ParentNode == null)
            {
                return;
            }
            if (elements.PreviousElement.ParentNode.Name == "PipingNetworkSegment" && (elements.NextElement.Name == "CenterLine" || elements.NextElement.Name == "Equipment" || elements.NextElement.Name == "PipingComponent" || elements.NextElement.Name == "ProcessInstrument" || elements.NextElement.Name == "Component"))
            {
                this.m_arlReorderElements.Add(elements);
            }
        }

        private void AddGenericAttributesAllComosSpecs(XmlElement parentNode, IComosBaseObject comosBaseObject, ArrayList specsToExport)
        {
            try
            {
                bool flag = specsToExport != null;
                if (!flag || specsToExport.Count != 0)
                {
                    IComosDCollection comosDCollection = null;
                    if (comosBaseObject is IComosDDevice)
                    {
                        comosDCollection = ((IComosDDevice)comosBaseObject).Specifications();
                    }
                    if (comosBaseObject is IComosDCDevice)
                    {
                        comosDCollection = ((IComosDCDevice)comosBaseObject).Specifications();
                    }
                    IComosDSpecification comosDSpecification = null;
                    IComosDSpecification comosDSpecification1 = null;
                    IComosDStandardTable isoMappingTable = this.GetIsoMappingTable(comosBaseObject);
                    string comosChapterName = "";
                    for (int i = 1; i <= comosDCollection.Count(); i++)
                    {
                        comosDSpecification = comosDCollection.Item(i) as IComosDSpecification;
                        comosChapterName = this.GetComosChapterName(comosDSpecification);
                        if (!string.IsNullOrEmpty(comosChapterName))
                        {
                            XmlElement xmlElement = this.CreateGenericAttributesSet(comosChapterName);
                            for (int j = 1; j <= comosDSpecification.Specifications().Count(); j++)
                            {
                                comosDSpecification1 = comosDSpecification.Specifications().Item(j);
                                if (!flag || specsToExport.Contains(comosDSpecification1.NestedName()))
                                {
                                    this.CreateGenericAttribute(comosDSpecification1, flag, isoMappingTable, xmlElement, false);
                                }
                            }
                            SvgExport.SetAttributeCount(xmlElement);
                            if (xmlElement.ChildNodes.Count > 0)
                            {
                                parentNode.AppendChild(xmlElement);
                            }
                        }
                    }
                    if (parentNode.Attributes["ID"] != null && parentNode.Attributes["ID"].Value == "XMP_1")
                    {
                        parentNode.SelectNodes("NZ");
                    }
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
        }

        private void AddGenericAttributesToPlantItem(XmlElement plantItemNode, IComosBaseObject comosBaseObject, bool readComosSpecs)
        {
            try
            {
                XmlElement xmlElement = this.CreateGenericAttributesNameDescription(comosBaseObject);
                if (xmlElement != null)
                {
                    plantItemNode.AppendChild(xmlElement);
                }
                this.AddGenericAttributesAllComosSpecs(plantItemNode, comosBaseObject, this.GetListedExportAttributes(comosBaseObject));
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
        }

        private string AlignTrans(string a, ref double x1, ref double y1, ref double x2, ref double y2)
        {
            string str;
            try
            {
                switch (a)
                {
                    case "LT":
                        {
                            x1 = 0;
                            y1 = -1;
                            x2 = 1;
                            y2 = 0;
                            str = "LeftTop";
                            break;
                        }
                    case "LC":
                        {
                            x1 = 0;
                            y1 = -0.5;
                            x2 = 1;
                            y2 = 0.5;
                            str = "LeftCenter";
                            break;
                        }
                    case "LB":
                        {
                            x1 = 0;
                            y1 = 0;
                            x2 = 1;
                            y2 = 1;
                            str = "LeftBottom";
                            break;
                        }
                    case "CT":
                        {
                            x1 = -0.5;
                            y1 = -1;
                            x2 = 0.5;
                            y2 = 0;
                            str = "CenterTop";
                            break;
                        }
                    case "CC":
                        {
                            x1 = -0.5;
                            y1 = -0.5;
                            x2 = 0.5;
                            y2 = 0.5;
                            str = "CenterCenter";
                            break;
                        }
                    case "CB":
                        {
                            x1 = -0.5;
                            y1 = 0;
                            x2 = 0.5;
                            y2 = 1;
                            str = "CenterBottom";
                            break;
                        }
                    case "RT":
                        {
                            x1 = -1;
                            y1 = -1;
                            x2 = 0;
                            y2 = 0;
                            str = "RightTop";
                            break;
                        }
                    case "RC":
                        {
                            x1 = -1;
                            y1 = -0.5;
                            x2 = 0;
                            y2 = 0.5;
                            str = "RightCenter";
                            break;
                        }
                    case "RB":
                        {
                            x1 = -1;
                            y1 = 0;
                            x2 = 0;
                            y2 = 1;
                            str = "RightBottom";
                            break;
                        }
                    default:
                        {
                            str = "LeftBottom";
                            break;
                        }
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                str = "";
            }
            return str;
        }

        private double Arctg(double x, double y)
        {
            double num;
            try
            {
                if (x > 0)
                {
                    num = Math.Atan(y / x);
                }
                else if (x != 0)
                {
                    num = Math.Atan(y / x) + this.PI;
                }
                else
                {
                    num = (y <= 0 ? -this.PI / 2 : this.PI / 2);
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                num = 0;
            }
            return num;
        }

        private int BGRtoRGB(int intBGR)
        {
            return (intBGR & 255) << 16 | intBGR & 65280 | (intBGR & 16711680) >> 16;
        }

        //void ISvgExport.Export(Document doc, IXReport XRep, string ExportFileName, string INIFileName, string ExportVersion)
        void ISvgExport.Export(Document doc, string ExportFileName, string INIFileName, string ExportVersion)
        {
            try
            {
                Debugger.Launch();
                this.m_rDoc = doc;
                //this.m_xRep = XRep;
                this.m_ComosDocumentSizeY = doc.SizeY / doc.DrawingScale;
                if (this.IsXMpLantLicenceAvailable())
                {
                    if (this.m_fUnattended)
                    {
                        this.m_strSaveFileName = ExportFileName;
                        this.m_xvExportVersion = this.SetUnattendedSchemaVersion(ExportVersion);
                    }
                    else
                    {
                        this.GetSaveFileName();
                    }
                    if (this.m_strSaveFileName.Trim() != "")
                    {
                        this.ConvertROtoXMPlant();
                    }
                }
                else if (!this.m_fUnattended || this.m_fUnattended && !this.m_MessageBoxAlreadyDisplayed)
                {
                    CMessageBox.Show(AppGlobal.ITX("~01366 Diese Funktion ist nicht im Lizenzumfang vorhanden."), AppGlobal.ITX("~0666d XMpLant Export"), MessageBoxButton.OK);
                    this.m_MessageBoxAlreadyDisplayed = true;
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
        }

        int ISvgExport.GetVersion()
        {

            return 3;
        }

        private bool ContainsEvaluableText(string strText)
        {
            bool flag;
            try
            {
                flag = (strText.Contains("%N") || strText.Contains("*V*P") ? true : false);
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                flag = false;
            }
            return flag;
        }

        private void ConvertROtoXMPlant()
        {
            object obj;
            try
            {
                try
                {
                    if (this.XMpLantExportVersion_3_2_0())
                    {
                        this.m_xvExportVersion = enmXMpLantVersion.XMpLantVersion_3_2_0;
                    }
                    object obj1 = ((IPLTVarstorage)AppGlobal.Workset.Globals()).ItemByKey("Proteus_UseOldLineTypeDefault");
                    if (obj1 is bool)
                    {
                        this.m_fUseOldLineTypeDefault = (bool)obj1;
                    }
                    if (this.m_dicIncludeInBranchUIDs.Count > 0)
                    {
                        this.m_dicIncludeInBranchUIDs.Clear();
                    }
                    if (!this.m_fUnattended)
                    {
                        Application.Current.MainWindow.IsEnabled = false;
                    }
                    IXDocCommon xDoc = (IXDocCommon)this.m_rDoc.XDoc;
                    if (xDoc.IWspXDoc.ApplicationType == ApplicationType.AT_PID || xDoc.IWspXDoc.ApplicationType == ApplicationType.AT_FEED)
                    {
                        this.m_SymbolType = ((IComosDDocument)this.m_rDoc.ComosDocument()).SymbolType();
                        this.m_Workset = ((IComosDDocument)this.m_rDoc.ComosDocument()).Workset();
                        this.ProjectGetParameters(((IComosDDocument)this.m_rDoc.ComosDocument()).Project());
                        if (this.m_arlLineTypeMapping.Count == 0)
                        {
                            this._getLineTypeMappingFromStandardTable();
                        }
                        if (this.m_arlLineTypeMapping.Count == 0)
                        {
                        }
                        Lib lib = AppGlobal.Workset.Lib() as Lib;
                        this.m_stUnitMapping = lib.GetStandardTable(AppGlobal.Workset, "Y40|M22|A10|Y40M22N00002");
                        Dictionary<ulong, Item> nums = this.DocItems2Dictionary();
                        this.m_xmlDoc = this.CreateDocumentBasis(this.m_strSaveFileName);
                        this.m_Root = this.m_xmlDoc.DocumentElement;
                        this.CreateElementPlantInformation();
                        this.CreateElementExtent(this.m_Root, 0, 0, this.m_rDoc.SizeX, this.m_rDoc.SizeY);
                        this.CreateNodeDrawing(this.m_Root, this.m_rDoc);
                        this.m_Catalogue = this.m_xmlDoc.CreateElement("ShapeCatalogue");
                        this.m_Catalogue.SetAttribute("Name", "ShapeCatalogue");
                        this.m_Root.AppendChild(this.m_Catalogue);
                        ArrayList allDocHatch = this.GetAllDocHatch(nums);
                        for (int i = 0; i < 2; i++)
                        {
                            int num = 1;
                            int count = nums.Count;
                            foreach (KeyValuePair<ulong, Item> keyValuePair in nums)
                            {
                                Item value = keyValuePair.Value;
                                if (value.SystemTypeName == "Object")
                                {
                                    REPORTLib.Object obj2 = value as REPORTLib.Object;
                                    REPORTLib.Object obj3 = obj2;
                                    obj = (obj2 == null || obj3.XObj == null ? LateBinding.LateGet(value, null, "XObj", null, null, null) : obj3.XObj);
                                    bool flag = obj is IPfdRoFlag;
                                    bool flag1 = obj is IRoConnPfd;
                                    bool flag2 = obj is IRoDevice;
                                    if (i == 1 & flag)
                                    {
                                        if (this.m_xvExportVersion != enmXMpLantVersion.XMpLantVersion_3_2_0)
                                        {
                                            XmlElement xmlElement = this.CreateLabel(obj, value);
                                            if (xmlElement == null)
                                            {
                                                continue;
                                            }
                                            this.m_Drawing.AppendChild(xmlElement);
                                        }
                                        else
                                        {
                                            this.CreateGraphicalElements((obj as IRoDeviceCommon).IBaseRoDevice.Container.GeoEngine(false) as ComosGeoEngine, value, this.m_xmlDoc, this.m_Drawing, false, obj as IGeometry);
                                        }
                                    }
                                    else if (!(i == 1 & flag1))
                                    {
                                        if (!((i != 0 || flag ? false : !flag1) & flag2))
                                        {
                                            continue;
                                        }
                                        this.SymbolToXMPlant(obj, value);
                                    }
                                    else
                                    {
                                        this.CreatePipeStru(obj, value);
                                    }
                                }
                                else if (value.SystemTypeName == "Line" && i == 0)
                                {
                                    REPORTLib.Line line = value as REPORTLib.Line;
                                    this.CreateLine(line.P1.x, line.P1.y, line.P2.x, line.P2.y, this.m_xmlDoc, this.m_Drawing, value, false, true, 0, null, null);
                                }
                                else if (value.SystemTypeName == "Arc" && i == 0)
                                {
                                    this.ReportArcToCircle(value, this.m_Drawing, allDocHatch);
                                }
                                else if (!(value.SystemTypeName == "Text") || i != 0)
                                {
                                    if (!(value.SystemTypeName == "ReportRect") || i != 0)
                                    {
                                        continue;
                                    }
                                    XmlElement xmlElement1 = this.CreateElementPolyLineFromRectangle(value);
                                    if (xmlElement1 == null)
                                    {
                                        continue;
                                    }
                                    this.m_Drawing.AppendChild(xmlElement1);
                                }
                                else
                                {
                                    this.CreateText(this.m_Drawing, value, null);
                                }
                            }
                        }
                        foreach (SvgExport.m_SignalLine mArlSignalLine in this.m_arlSignalLines)
                        {
                            this.CreateSignalLine(mArlSignalLine.XObj, mArlSignalLine.ItemInDoc);
                        }
                        this.ReorderPipingNetworkSegments();
                        foreach (SvgExport.m_ReorderElements mArlReorderElement in this.m_arlReorderElements)
                        {
                            if (mArlReorderElement.PreviousElement.ParentNode == null)
                            {
                                continue;
                            }
                            mArlReorderElement.PreviousElement.ParentNode.InsertAfter(mArlReorderElement.NextElement, mArlReorderElement.PreviousElement);
                        }
                        this._reorderPipingNozzles();
                        this._CollectMisleadPipingComponents();
                        XmlElement mXeProcessInstrumentationFunctionGraphicalDummy = this.m_xeProcessInstrumentationFunctionGraphicalDummy;
                        this.m_xmlDoc.Save(this.m_strSaveFileName);
                    }
                }
                catch (Exception exception)
                {
                    AppGlobal.StdErrorHandler(exception);
                }
            }
            finally
            {
                if (!this.m_fUnattended)
                {
                    Application.Current.MainWindow.IsEnabled = true;
                }
                this.m_xmlDoc = null;
            }
        }

        private void CreateAttributesForPlantItem(XmlElement nodePlantItem, object baseObject, enmXMpLantComponentType ComponentType, string mandatoryShapeCatalogueComponentName = "")
        {
            try
            {
                string attributeID = "";
                string label = "";
                string comosObjectAttributeValueByNestedName = "";
                string str = "";
                string comosObjectAttributeValueByNestedName1 = "";
                string str1 = "";
                string comosObjectAttributeValueByNestedName2 = "";
                string str2 = "";
                string comosObjectAttributeValueByNestedName3 = "";
                string str3 = "";
                string comosObjectAttributeValueByNestedName4 = "";
                string str4 = "";
                if (baseObject is IComosDCDevice)
                {
                    IComosDCDevice comosDCDevice = (IComosDCDevice)baseObject;
                    attributeID = this.GetAttributeID(true);
                    comosObjectAttributeValueByNestedName1 = comosDCDevice.SystemFullName();
                }
                else if (baseObject is IComosDDevice)
                {
                    IComosDDevice comosDDevice = (IComosDDevice)baseObject;
                    attributeID = this.GetAttributeID(false);
                    label = comosDDevice.Label;
                    if (nodePlantItem.Name == "PipingNetworkSegment")
                    {
                        comosObjectAttributeValueByNestedName = string.Concat("XMP_", comosDDevice.SystemUID());
                    }
                    if (comosDDevice.CDevice != null)
                    {
                        comosObjectAttributeValueByNestedName1 = comosDDevice.CDevice.SystemFullName();
                    }
                }
                else if (!(baseObject is IGraphicAttributes))
                {
                    attributeID = this.GetAttributeID(false);
                }
                else
                {
                    IGraphicAttributes graphicAttribute = (IGraphicAttributes)baseObject;
                    attributeID = this.GetAttributeID(false);
                    if (nodePlantItem.Name == "SignalLine")
                    {
                    }
                }
                label = this.getComosObjectAttributeValueByNestedName((IComosBaseObject)baseObject, SvgExport.COMOS_XMPLANT_CHAPTER_NAME, SvgExport.COMOS_XMPLANT_TAG);
                comosObjectAttributeValueByNestedName = this.getComosObjectAttributeValueByNestedName((IComosBaseObject)baseObject, SvgExport.COMOS_XMPLANT_CHAPTER_NAME, SvgExport.COMOS_XMPLANT_SPECIFICATION);
                comosObjectAttributeValueByNestedName1 = this.getComosObjectAttributeValueByNestedName((IComosBaseObject)baseObject, SvgExport.COMOS_XMPLANT_CHAPTER_NAME, SvgExport.COMOS_XMPLANT_STOCK_NUMBER);
                if (!(nodePlantItem.Name == "ProcessInstrumentationFunction") || this.m_xvExportVersion < enmXMpLantVersion.XMpLantVersion_4_0_1)
                {
                    str1 = this.getComosObjectAttributeValueByNestedName((IComosBaseObject)baseObject, SvgExport.COMOS_XMPLANT_CHAPTER_NAME, SvgExport.COMOS_XMPLANT_COMPONENT_CLASS);
                    comosObjectAttributeValueByNestedName2 = this.getComosObjectAttributeValueByNestedName((IComosBaseObject)baseObject, SvgExport.COMOS_XMPLANT_CHAPTER_NAME, SvgExport.COMOS_XMPLANT_COMPONENT_CLASS_URI);
                }
                else
                {
                    str1 = "ProcessInstrumentationFunction";
                    comosObjectAttributeValueByNestedName2 = "http://sandbox.dexpi.org/rdl/ProcessInstrumentationFunction";
                }
                str2 = (string.IsNullOrEmpty(mandatoryShapeCatalogueComponentName) ? this.getComosObjectAttributeValueByNestedName((IComosBaseObject)baseObject, SvgExport.COMOS_XMPLANT_CHAPTER_NAME, SvgExport.COMOS_XMPLANT_COMPONENT_NAME) : mandatoryShapeCatalogueComponentName);
                if (string.IsNullOrEmpty(str2))
                {
                    str2 = attributeID;
                }
                comosObjectAttributeValueByNestedName3 = this.getComosObjectAttributeValueByNestedName((IComosBaseObject)baseObject, SvgExport.COMOS_XMPLANT_CHAPTER_NAME, SvgExport.COMOS_XMPLANT_REVISION);
                comosObjectAttributeValueByNestedName4 = this.getComosObjectAttributeValueByNestedName((IComosBaseObject)baseObject, SvgExport.COMOS_XMPLANT_CHAPTER_NAME, SvgExport.COMOS_XMPLANT_STATUS);
                if (comosObjectAttributeValueByNestedName4 != "Current" && comosObjectAttributeValueByNestedName4 != "Deleted" && comosObjectAttributeValueByNestedName4 != "Modified" && comosObjectAttributeValueByNestedName4 != "New")
                {
                    comosObjectAttributeValueByNestedName4 = "Current";
                }
                nodePlantItem.SetAttribute("ID", attributeID);
                if (label.Trim() != "")
                {
                    nodePlantItem.SetAttribute((this.m_xvExportVersion >= enmXMpLantVersion.XMpLantVersion_3_3_3 ? "TagName" : "Tag"), label);
                }
                if (comosObjectAttributeValueByNestedName.Trim() != "")
                {
                    nodePlantItem.SetAttribute("Specification", comosObjectAttributeValueByNestedName);
                }
                if (str.Trim() != "" && this.m_xvExportVersion >= enmXMpLantVersion.XMpLantVersion_3_6_0)
                {
                    nodePlantItem.SetAttribute("SpecificationURI", str);
                }
                if (comosObjectAttributeValueByNestedName1.Trim() != "")
                {
                    nodePlantItem.SetAttribute("StockNumber", comosObjectAttributeValueByNestedName1);
                }
                if (str1.Trim() != "")
                {
                    nodePlantItem.SetAttribute("ComponentClass", str1);
                }
                if (comosObjectAttributeValueByNestedName2.Trim() != "" && this.m_xvExportVersion >= enmXMpLantVersion.XMpLantVersion_3_6_0)
                {
                    nodePlantItem.SetAttribute("ComponentClassURI", comosObjectAttributeValueByNestedName2);
                }
                if ((baseObject is IComosDCDevice || ComponentType == enmXMpLantComponentType.Explicit || ComponentType == enmXMpLantComponentType.Parametric || !string.IsNullOrEmpty(mandatoryShapeCatalogueComponentName)) && !string.IsNullOrEmpty(str2))
                {
                    nodePlantItem.SetAttribute("ComponentName", str2);
                }
                if (ComponentType == enmXMpLantComponentType.Normal)
                {
                    nodePlantItem.SetAttribute("ComponentType", "Normal");
                }
                else if ((int)ComponentType - (int)enmXMpLantComponentType.Explicit <= (int)enmXMpLantComponentType.NoComponentType)
                {
                    nodePlantItem.SetAttribute("ComponentType", "Explicit");
                }
                if (comosObjectAttributeValueByNestedName3.Trim() != "")
                {
                    nodePlantItem.SetAttribute("Revision", comosObjectAttributeValueByNestedName3);
                }
                if (str3.Trim() != "" && this.m_xvExportVersion >= enmXMpLantVersion.XMpLantVersion_3_6_0)
                {
                    nodePlantItem.SetAttribute("RevisionURI", str3);
                }
                if (comosObjectAttributeValueByNestedName4.Trim() != "")
                {
                    nodePlantItem.SetAttribute("Status", comosObjectAttributeValueByNestedName4);
                }
                if (str4.Trim() != "" && this.m_xvExportVersion >= enmXMpLantVersion.XMpLantVersion_3_6_0)
                {
                    nodePlantItem.SetAttribute("StatusURI", str4);
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
        }

        private void CreateCircle(double cx, double cy, XmlElement childNode, XmlDocument xmlDoc, double r, Item ItemInDoc, bool IsPlantItemInstance, NRect ur, bool gra, double theta, NDisplayElement De, bool Hatch, bool Solid, IGeometry igeo, bool isarc)
        {
            int lineTyp;
            float width;
            try
            {
                double itemInDoc = 0;
                double mComosDocumentSizeY = 0;
                XmlElement xmlElement = xmlDoc.CreateElement("Circle");
                childNode.AppendChild(xmlElement);
                xmlElement.SetAttribute("Radius", r.ToString().Replace(",", "."));
                if (Solid)
                {
                    xmlElement.SetAttribute("Filled", "Solid");
                }
                else if (Hatch)
                {
                    xmlElement.SetAttribute("Filled", "Hatch");
                }
                if (IsPlantItemInstance)
                {
                    string layer = De.Header.Layer;
                    lineTyp = De.Header.LineTyp;
                    string str = lineTyp.ToString();
                    width = De.Header.Width;
                    string str1 = width.ToString().Replace(",", ".");
                    lineTyp = De.Header.Color;
                    this.CreateElementPresentation(xmlDoc, xmlElement, layer, str, str1, lineTyp.ToString());
                    if (!isarc)
                    {
                        itemInDoc = cx + ItemInDoc.x1;
                        mComosDocumentSizeY = this.m_ComosDocumentSizeY + cy - ItemInDoc.y1;
                    }
                    else
                    {
                        double scaleValueX = igeo.ScaleValueX;
                        double scaleValueY = igeo.ScaleValueY;
                        itemInDoc = cx * scaleValueX;
                        mComosDocumentSizeY = cy * scaleValueY;
                        if (scaleValueX != 1 || scaleValueY != 1)
                        {
                            this.RotateCoord(ref itemInDoc, ref mComosDocumentSizeY, -theta);
                        }
                        itemInDoc += ItemInDoc.x1;
                        mComosDocumentSizeY = this.m_ComosDocumentSizeY + mComosDocumentSizeY - ItemInDoc.y1;
                    }
                    this.CreateElementExtent(xmlElement, itemInDoc - r, mComosDocumentSizeY - r, itemInDoc + r, mComosDocumentSizeY + r);
                    this.CreateElementPosition(xmlElement, itemInDoc, mComosDocumentSizeY, theta);
                }
                else if (!gra)
                {
                    string layer1 = De.Header.Layer;
                    lineTyp = De.Header.LineTyp;
                    string str2 = lineTyp.ToString();
                    width = De.Header.Width;
                    string str3 = width.ToString().Replace(",", ".");
                    lineTyp = De.Header.Color;
                    this.CreateElementPresentation(xmlDoc, xmlElement, layer1, str2, str3, lineTyp.ToString());
                    this.TransformCoord(-theta, 1 / igeo.ScaleValueX, 1 / igeo.ScaleValueY, igeo.Reflect, cx, cy, out itemInDoc, out mComosDocumentSizeY);
                    this.CreateElementExtent(xmlElement, itemInDoc - r, mComosDocumentSizeY - r, itemInDoc + r, mComosDocumentSizeY + r);
                    this.CreateElementPosition(xmlElement, itemInDoc, mComosDocumentSizeY, 0);
                }
                else
                {
                    string str4 = ItemInDoc.Layer.ToString();
                    string str5 = ItemInDoc.LineType.ToString();
                    double num = ItemInDoc.Width;
                    string str6 = num.ToString().Replace(",", ".");
                    lineTyp = ItemInDoc.Color;
                    this.CreateElementPresentation(xmlDoc, xmlElement, str4, str5, str6, lineTyp.ToString());
                    itemInDoc = cx;
                    mComosDocumentSizeY = this.m_ComosDocumentSizeY - cy;
                    this.CreateElementExtent(xmlElement, itemInDoc - r, mComosDocumentSizeY - r, itemInDoc + r, mComosDocumentSizeY + r);
                    this.CreateElementPosition(xmlElement, itemInDoc, mComosDocumentSizeY, theta);
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
        }

        private void CreateCircleArc(ComosGeoEngine GE, NArc arc, XmlElement childNode, XmlDocument xmlDoc, Item ItemInDoc, bool IsPlantItemInstance, double theta, NDisplayElement De, IGeometry igeo)
        {
            try
            {
                XmlElement xmlElement = xmlDoc.CreateElement("TrimmedCurve");
                childNode.AppendChild(xmlElement);
                double startAngle = arc.GetStartAngle();
                startAngle -= SvgExport.M3D.GRAD(theta);
                xmlElement.SetAttribute("StartAngle", startAngle.ToString().Replace(",", "."));
                double angle = startAngle + arc.GetAngle();
                xmlElement.SetAttribute("EndAngle", angle.ToString().Replace(",", "."));
                this.CreateCircle(arc.CP.x, arc.CP.y, xmlElement, xmlDoc, arc.Radius, ItemInDoc, IsPlantItemInstance, arc.Umreck, false, theta, De, false, false, igeo, true);
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
        }

        private void CreateConnection(XmlElement ParentNode, IComosDDevice comosDDevice)
        {
            int connectorNumberByName;
            try
            {
                if (comosDDevice != null)
                {
                    string attribute = "";
                    string str = "-1";
                    string attribute1 = "";
                    string str1 = "-1";
                    IComosDSpecification spec = this.GetSpec(comosDDevice.Specifications(), SvgExport.COMOS_SYSTEM_CHAPTER_NAME, SvgExport.COMOS_SYSTEM_FROM_OBJ);
                    IComosDSpecification comosDSpecification = this.GetSpec(comosDDevice.Specifications(), SvgExport.COMOS_SYSTEM_CHAPTER_NAME, SvgExport.COMOS_SYSTEM_TO_OBJ);
                    if (spec != null || comosDSpecification != null)
                    {
                        if (spec != null)
                        {
                            IComosDDevice comosDDevice1 = null;
                            IComosDConnector linkObject = spec.LinkObject as IComosDConnector;
                            comosDDevice1 = (linkObject == null ? spec.LinkObject as IComosDDevice : linkObject.owner() as IComosDDevice);
                            string str2 = "";
                            if (linkObject != null && linkObject.Name.Length == 3 && AppGlobal.Workset.IsNumeric(linkObject.Name) && linkObject.Layer == "R")
                            {
                                str2 = linkObject.SystemUID();
                            }
                            else if (comosDDevice1 != null)
                            {
                                str2 = comosDDevice1.SystemUID();
                            }
                            if (str2 != "")
                            {
                                XmlElement elementByUID = XMLHelper.GetElementByUID(this.m_xvExportVersion, this.m_xmlDoc.DocumentElement, str2, true);
                                if (elementByUID != null)
                                {
                                    attribute = elementByUID.GetAttribute("ID");
                                    int elementConnectorCount = XMLHelper.GetElementConnectorCount(elementByUID);
                                    if (elementConnectorCount == 1)
                                    {
                                        str = "1";
                                    }
                                    else if (elementConnectorCount > 1)
                                    {
                                        string connectorNameByFlow = this.GetConnectorNameByFlow(comosDDevice1, SvgExport.enmDeviceConnector.dcDeviceOutputConnector);
                                        connectorNumberByName = XMLHelper.GetConnectorNumberByName(elementByUID, connectorNameByFlow);
                                        str = connectorNumberByName.ToString();
                                    }
                                }
                            }
                        }
                        if (comosDSpecification != null)
                        {
                            IComosDDevice comosDDevice2 = null;
                            IComosDConnector comosDConnector = comosDSpecification.LinkObject as IComosDConnector;
                            comosDDevice2 = (comosDConnector == null ? comosDSpecification.LinkObject as IComosDDevice : comosDConnector.owner() as IComosDDevice);
                            string str3 = "";
                            if (comosDConnector != null && comosDConnector.Name.Length == 3 && AppGlobal.Workset.IsNumeric(comosDConnector.Name) && comosDConnector.Layer == "R")
                            {
                                str3 = comosDConnector.SystemUID();
                            }
                            else if (comosDDevice2 != null)
                            {
                                str3 = comosDDevice2.SystemUID();
                            }
                            if (str3 != "")
                            {
                                if (comosDDevice2.CDevice.DetailClass != "<")
                                {
                                    XmlElement xmlElement = XMLHelper.GetElementByUID(this.m_xvExportVersion, this.m_xmlDoc.DocumentElement, str3, true);
                                    if (xmlElement != null)
                                    {
                                        attribute1 = xmlElement.GetAttribute("ID");
                                        string connectorNameByFlow1 = this.GetConnectorNameByFlow(comosDDevice2, SvgExport.enmDeviceConnector.dcDeviceOutputConnector);
                                        connectorNumberByName = XMLHelper.GetConnectorNumberByName(xmlElement, connectorNameByFlow1);
                                        str1 = connectorNumberByName.ToString();
                                    }
                                }
                                else
                                {
                                    IComosDDevice comosDDevice3 = comosDDevice2.owner() as IComosDDevice;
                                    if (comosDDevice3 != null)
                                    {
                                        int num = 0;
                                        IComosDDevice comosDDevice4 = null;
                                        for (int i = 1; i <= comosDDevice3.Devices().Count(); i++)
                                        {
                                            IComosDDevice comosDDevice5 = comosDDevice3.Devices().Item(i) as IComosDDevice;
                                            if (comosDDevice5 != null && comosDDevice5.CDevice.DetailClass == "<")
                                            {
                                                num++;
                                                if (comosDDevice5 != comosDDevice2)
                                                {
                                                    comosDDevice4 = comosDDevice5;
                                                }
                                            }
                                        }
                                        if (num != 2)
                                        {
                                            attribute1 = XMLHelper.GetElementByUID(this.m_xvExportVersion, this.m_xmlDoc.DocumentElement, str3, true).GetAttribute("ID");
                                            str1 = "1";
                                        }
                                        else
                                        {
                                            str3 = comosDDevice4.SystemUID();
                                            attribute1 = XMLHelper.GetElementByUID(this.m_xvExportVersion, this.m_xmlDoc.DocumentElement, str3, true).GetAttribute("ID");
                                            str1 = "1";
                                        }
                                    }
                                }
                            }
                        }
                        XmlElement item = ParentNode["Connection"];
                        item = this.CreateElementConnection(item, attribute, int.Parse(str), attribute1, int.Parse(str1));
                        ParentNode.AppendChild(item);
                    }
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
        }

        private void CreateCreatePipeFlowArrowGraphics(ComosGeoEngine Ge, Item ItemInDoc, XmlElement xePipeFlowArrow, bool InCatalogue, IGeometry IGeo)
        {
            int displayElementCount = Ge.DisplayElementCount;
            this.GetAllHatch(Ge);
            double angle = IGeo.Angle;
            for (int i = 0; i <= displayElementCount - 1; i++)
            {
                NDisplayElement displayElement = Ge.DisplayElement[i];
                NGeoElement geometrie = displayElement.Geometrie;
                if (geometrie.Type == 1 && this.BGRtoRGB(displayElement.Header.Color) == 255 && displayElement.Header.Layer == "250")
                {
                    NLine nLine = geometrie as NLine;
                    if (nLine != null)
                    {
                        this.CreateLine(nLine.P1.x, nLine.P1.y, nLine.P2.x, nLine.P2.y, this.m_xmlDoc, xePipeFlowArrow, ItemInDoc, InCatalogue, false, angle, displayElement, IGeo);
                    }
                }
            }
        }

        private bool CreateDependantAttribute(Item reportItem, NTextElement GeTxt, out string DependantAttribute, out string ItemID)
        {
            string str;
            bool flag;
            bool flag1;
            DependantAttribute = "";
            ItemID = "";
            try
            {
                REPORTLib.Object obj = reportItem as REPORTLib.Object;
                if (obj != null)
                {
                    object xObj = obj.XObj;
                    if (xObj == null)
                    {
                        flag1 = false;
                    }
                    else if (xObj is IPfdRoFlag)
                    {
                        IRoDevice roDevice = xObj as IRoDevice;
                        if (roDevice != null)
                        {
                            IComosDDevice device = roDevice.Device;
                            if (device != null)
                            {
                                XmlElement elementByUID = XMLHelper.GetElementByUID(this.m_xvExportVersion, this.m_xmlDoc.DocumentElement, device.SystemUID(), true);
                                if (elementByUID == null || elementByUID.Attributes["ID"] == null)
                                {
                                    flag1 = false;
                                }
                                else
                                {
                                    ItemID = elementByUID.Attributes["ID"].Value;
                                    if (!string.IsNullOrEmpty(ItemID.Trim()))
                                    {
                                        IComosDSpecification comosDSpecification = device.spec(this.GetSpecNestedNameOfEvaluableText(GeTxt.txt));
                                        IComosDStandardTable isoMappingTable = this.GetIsoMappingTable(device);
                                        this.GetAttributeIsoNameAndUri(isoMappingTable, comosDSpecification.NestedName(), comosDSpecification.Description, SvgExport.enmXValueType.Normal, out DependantAttribute, out str, out flag);
                                        if (!string.IsNullOrEmpty(DependantAttribute.Trim()))
                                        {
                                            this.CreateNonExistingGenericAttribute(elementByUID, comosDSpecification, DependantAttribute, isoMappingTable);
                                            DependantAttribute = string.Concat("[", DependantAttribute, "]");
                                            return true;
                                        }
                                        else
                                        {
                                            ItemID = "";
                                            flag1 = false;
                                        }
                                    }
                                    else
                                    {
                                        flag1 = false;
                                    }
                                }
                            }
                            else
                            {
                                flag1 = false;
                            }
                        }
                        else
                        {
                            flag1 = false;
                        }
                    }
                    else
                    {
                        flag1 = false;
                    }
                }
                else
                {
                    flag1 = false;
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                flag1 = false;
            }
            return flag1;
        }

        private XmlDocument CreateDocumentBasis(string strFileName)
        {
            XmlDocument xmlDocument;
            try
            {
                XmlDocument xmlDocument1 = new XmlDocument();
                using (StringWriter stringWriter = new StringWriter())
                {
                    using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
                    {
                        xmlTextWriter.Formatting = Formatting.Indented;
                        xmlTextWriter.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                        xmlTextWriter.WriteStartElement("PlantModel");
                        xmlTextWriter.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
                        xmlTextWriter.WriteAttributeString("xsi", "noNamespaceSchemaLocation", null, this.GetVersionDependentSchemaFilename());
                    }
                    xmlDocument1.LoadXml(stringWriter.ToString());
                }
                xmlDocument = xmlDocument1;
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                xmlDocument = null;
            }
            return xmlDocument;
        }

        private void CreateDrawingPresentationExtent(Document RDDoc, XmlElement Drawing)
        {
            try
            {
                if (!RDDoc.BlackBackground)
                {
                    double num = 16777215;
                    this.CreateElementPresentation(this.m_xmlDoc, Drawing, "", "", "", num.ToString());
                }
                else
                {
                    this.CreateElementPresentation(this.m_xmlDoc, Drawing, "", "", "", "0");
                }
                this.CreateElementExtent(Drawing, 0, 0, RDDoc.SizeX, RDDoc.SizeY);
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
        }

        private XmlElement CreateElementAssociation(AssociationType atAssociationType, string itemId)
        {
            XmlElement xmlElement = this.m_xmlDoc.CreateElement("Association");
            xmlElement.SetAttribute("Type", atAssociationType.GetXmlEnumAttributeValueFromEnum<AssociationType>());
            xmlElement.SetAttribute("ItemID", itemId);
            return xmlElement;
        }

        private XmlElement CreateElementCenterLine(IConnectedRO cro, XmlElement Presentation, IComosDDevice dvcCenterLine)
        {
            XmlElement xmlElement;
            try
            {
                IRoConnector roConnector = cro.Connectors().Item(1);
                IWspPoints points = (cro as IRoConnCommon).IBaseRoConn.IConnectionLine.GetPoints();
                double num = roConnector.x;
                double num1 = roConnector.y;
                XmlElement xmlElement1 = this.m_xmlDoc.CreateElement("CenterLine");
                XmlElement xmlElement2 = (XmlElement)Presentation.Clone();
                xmlElement1.SetAttribute("NumPoints", points.Count.ToString());
                xmlElement1.AppendChild(xmlElement2);
                this.CreateElementExtent(xmlElement1, 0, 0, 0, 0);
                XmlElement item = xmlElement1["Extent"]["Min"];
                XmlElement item1 = xmlElement1["Extent"]["Max"];
                for (int i = 1; i <= points.Count; i++)
                {
                    object obj = i - 1;
                    double item2 = num + points.Item[obj].x;
                    double mComosDocumentSizeY = this.m_ComosDocumentSizeY - num1 + points.Item[obj].y;
                    if (i != 1)
                    {
                        item.SetAttribute("X", XMLHelper.StringFromValue(Math.Min(XMLHelper.ValueFromString(item.Attributes["X"].Value), item2)));
                        item.SetAttribute("Y", XMLHelper.StringFromValue(Math.Min(XMLHelper.ValueFromString(item.Attributes["Y"].Value), mComosDocumentSizeY)));
                        item1.SetAttribute("X", XMLHelper.StringFromValue(Math.Max(XMLHelper.ValueFromString(item1.Attributes["X"].Value), item2)));
                        item1.SetAttribute("Y", XMLHelper.StringFromValue(Math.Max(XMLHelper.ValueFromString(item1.Attributes["Y"].Value), mComosDocumentSizeY)));
                    }
                    else
                    {
                        item.SetAttribute("X", item2.ToString().Replace(",", "."));
                        item.SetAttribute("Y", mComosDocumentSizeY.ToString().Replace(",", "."));
                        item1.SetAttribute("X", item2.ToString().Replace(",", "."));
                        item1.SetAttribute("Y", mComosDocumentSizeY.ToString().Replace(",", "."));
                    }
                    xmlElement1.AppendChild(this.CreateElementCoordinate(item2, mComosDocumentSizeY, 0));
                }
                if (dvcCenterLine != null)
                {
                    this.AddGenericAttributesToPlantItem(xmlElement1, dvcCenterLine, false);
                }
                xmlElement = xmlElement1;
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                xmlElement = null;
            }
            return xmlElement;
        }

        private void CreateElementCenterLine(object XObj, XmlDocument xmlDoc, XmlNode root)
        {
            IConnectedRO xObj = XObj as IConnectedRO;
            xObj.Connectors().Item(1);
            XmlElement xmlElement = xmlDoc.CreateElement("CenterLine");
            root.AppendChild(xmlElement);
            IWspPoints points = (xObj as IRoConnCommon).IBaseRoConn.IConnectionLine.GetPoints();
            xmlElement.SetAttribute("NumPoints", points.Count.ToString());
            for (int i = 0; i < points.Count; i++)
            {
                object obj = i;
                IWspPoint item = points.Item[obj];
                xmlElement.AppendChild(this.CreateElementCoordinate(item.x, item.y, 0));
            }
        }

        private XmlElement CreateElementConnection(XmlElement Connection, string strFromId, int intFromNode, string strToId, int intToNode)
        {
            if (Connection == null)
            {
                Connection = this.m_xmlDoc.CreateElement("Connection");
            }
            Connection.SetAttribute("FromID", strFromId);
            if (intFromNode != -1)
            {
                Connection.SetAttribute("FromNode", intFromNode.ToString());
            }
            Connection.SetAttribute("ToID", strToId);
            if (intToNode != -1)
            {
                Connection.SetAttribute("ToNode", intToNode.ToString());
            }
            return Connection;
        }

        private void CreateElementConnectionPoints(XmlElement parentNode, Item ItemInDoc, IComosDDevice DDev, bool IsPlantItemInstance, ArrayList arlGeoTexts, double theta, IGeometry igeo)
        {
            try
            {
                double num = 0;
                double num1 = 0;
                double itemInDoc = 0;
                double mComosDocumentSizeY = 0;
                double itemInDoc1 = 0;
                double mComosDocumentSizeY1 = 0;
                int count = arlGeoTexts.Count + 1;
                int num2 = 0;
                XmlElement connectionPointsElement = XMLHelper.GetConnectionPointsElement(parentNode, this.m_xmlDoc);
                XmlElement xmlElement = this.m_xmlDoc.CreateElement("GenericAttributes");
                XmlElement xmlElement1 = this.m_xmlDoc.CreateElement("Extent");
                XmlElement xmlElement2 = this.m_xmlDoc.CreateElement("Node");
                if (this.m_xvExportVersion >= enmXMpLantVersion.XMpLantVersion_4_0_1)
                {
                    xmlElement2.SetAttribute("ID", this.GetAttributeID(!IsPlantItemInstance));
                }
                xmlElement.SetAttribute("Number", count.ToString().Replace(",", "."));
                connectionPointsElement.SetAttribute("NumPoints", count.ToString().Replace(",", "."));
                parentNode.AppendChild(connectionPointsElement);
                connectionPointsElement.AppendChild(xmlElement1);
                connectionPointsElement.AppendChild(xmlElement2);
                if (!IsPlantItemInstance)
                {
                    this.CreateElementPosition(xmlElement2, num, num1, 0);
                }
                else
                {
                    itemInDoc = ItemInDoc.x1;
                    itemInDoc1 = ItemInDoc.x1;
                    mComosDocumentSizeY = this.m_ComosDocumentSizeY - ItemInDoc.y1;
                    mComosDocumentSizeY1 = this.m_ComosDocumentSizeY - ItemInDoc.y1;
                    this.CreateElementPosition(xmlElement2, ItemInDoc.x1, (double)(this.m_ComosDocumentSizeY - ItemInDoc.y1), 0);
                }
                xmlElement2.SetAttribute("Name", "Origin");
                xmlElement.AppendChild(this.CreateElementGenericAttribute(string.Concat("NodeName", num2.ToString()), "Origin", "", "", "string", "", "", ""));
                num2++;
                foreach (NTextElement arlGeoText in arlGeoTexts)
                {
                    int num3 = arlGeoText.txt.LastIndexOf(".");
                    string str = arlGeoText.txt.Substring(num3 + 1);
                    str = str.Remove(str.LastIndexOf("%"));
                    xmlElement.AppendChild(this.CreateElementGenericAttribute(string.Concat("NodeName", num2.ToString()), str, "", "", "string", "", "", ""));
                    if (str.StartsWith(this.GetConnectorNameByFlow(DDev, SvgExport.enmDeviceConnector.dcDeviceInputConnector)))
                    {
                        connectionPointsElement.SetAttribute("FlowIn", num2.ToString());
                    }
                    if (str.StartsWith(this.GetConnectorNameByFlow(DDev, SvgExport.enmDeviceConnector.dcDeviceOutputConnector)))
                    {
                        connectionPointsElement.SetAttribute("FlowOut", num2.ToString());
                    }
                    double num4 = arlGeoText.point.x;
                    num = num4;
                    double num5 = num4;
                    double num6 = arlGeoText.point.y;
                    num1 = num6;
                    double num7 = num6;
                    this.TransformCoord(-theta, igeo.ScaleValueX, igeo.ScaleValueY, igeo.Reflect, num, num1, out num5, out num7);
                    if (!IsPlantItemInstance)
                    {
                        itemInDoc = Math.Min(itemInDoc, num5) / igeo.ScaleValueX;
                        mComosDocumentSizeY = Math.Min(mComosDocumentSizeY, num7) / igeo.ScaleValueY;
                        itemInDoc1 = Math.Max(itemInDoc1, num5) / igeo.ScaleValueX;
                        mComosDocumentSizeY1 = Math.Max(mComosDocumentSizeY1, num7) / igeo.ScaleValueY;
                    }
                    else
                    {
                        itemInDoc = Math.Min(itemInDoc, num);
                        mComosDocumentSizeY = Math.Min(mComosDocumentSizeY, num1);
                        itemInDoc1 = Math.Max(itemInDoc1, num);
                        mComosDocumentSizeY1 = Math.Max(mComosDocumentSizeY1, num1);
                    }
                    xmlElement2 = this.m_xmlDoc.CreateElement("Node");
                    if (this.m_xvExportVersion >= enmXMpLantVersion.XMpLantVersion_4_0_1)
                    {
                        xmlElement2.SetAttribute("ID", this.GetAttributeID(!IsPlantItemInstance));
                    }
                    xmlElement2.SetAttribute("Name", str);
                    connectionPointsElement.AppendChild(xmlElement2);
                    if (!IsPlantItemInstance)
                    {
                        this.CreateElementPosition(xmlElement2, num5, num7, 0);
                    }
                    else
                    {
                        this.CreateElementPosition(xmlElement2, num, num1, 0);
                    }
                    num2++;
                }
                this.CreateElementExtent(connectionPointsElement, itemInDoc, mComosDocumentSizeY, itemInDoc1, mComosDocumentSizeY1);
                connectionPointsElement.AppendChild(xmlElement);
                ArrayList specs = this.GetSpecs(DDev.Specifications(), SvgExport.COMOS_TECHNICAL_DATA_CHAPTER_NAME, SvgExport.COMOS_TECHNICAL_DATA_NOMINAL_WIDTH);
                if (specs.Count > 0)
                {
                    string str1 = ((IComosDSpecification)specs[0]).DisplayValue().Trim();
                    if (str1.Trim() != "")
                    {
                        XmlElement xmlElement3 = this.m_xmlDoc.CreateElement("NominalDiameter");
                        xmlElement3.SetAttribute("Value", str1);
                        XmlNodeList elementsByTagName = connectionPointsElement.GetElementsByTagName("Node");
                        for (int i = 1; i <= elementsByTagName.Count - 1; i++)
                        {
                            if (elementsByTagName[i]["NominalDiameter"] == null)
                            {
                                elementsByTagName[i].AppendChild(xmlElement3.Clone());
                            }
                        }
                        if (specs.Count > 1)
                        {
                            foreach (IComosDSpecification spec in specs)
                            {
                                IComosDLinkInfo comosDLinkInfo = spec.LinkInfo();
                                if (comosDLinkInfo == null)
                                {
                                    continue;
                                }
                                XmlNodeList xmlNodeLists = connectionPointsElement.SelectNodes(string.Concat(new string[] { "./", "Node", "[@", "Name", " = '", comosDLinkInfo.LinkName, "']" }));
                                if (xmlNodeLists.Count != 1 || xmlNodeLists[0]["NominalDiameter"] == null)
                                {
                                    continue;
                                }
                                str1 = spec.DisplayValue().Trim();
                                if (str1.Trim() == "")
                                {
                                    continue;
                                }
                                xmlNodeLists[0]["NominalDiameter"].SetAttribute("Value", str1);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
        }

        private void CreateElementConnectionPointsPipingNetworkBranch(XmlElement parentNode, IRoConnector roConn)
        {
            try
            {
                double num = 0;
                double mComosDocumentSizeY = 0;
                double num1 = 0;
                double mComosDocumentSizeY1 = 0;
                int num2 = 4;
                XmlElement connectionPointsElement = XMLHelper.GetConnectionPointsElement(parentNode, this.m_xmlDoc);
                XmlElement xmlElement = this.m_xmlDoc.CreateElement("GenericAttributes");
                XmlElement xmlElement1 = this.m_xmlDoc.CreateElement("Extent");
                XmlElement xmlElement2 = null;
                xmlElement.SetAttribute("Number", num2.ToString().Replace(",", "."));
                connectionPointsElement.SetAttribute("NumPoints", num2.ToString().Replace(",", "."));
                parentNode.AppendChild(connectionPointsElement);
                connectionPointsElement.AppendChild(xmlElement1);
                string[] strArrays = new string[] { "Origin", "I1", "O1", "O2" };
                for (int i = 0; i < num2; i++)
                {
                    xmlElement2 = this.m_xmlDoc.CreateElement("Node");
                    if (this.m_xvExportVersion >= enmXMpLantVersion.XMpLantVersion_4_0_1)
                    {
                        xmlElement2.SetAttribute("ID", this.GetAttributeID(false));
                    }
                    xmlElement2.SetAttribute("Name", strArrays[i]);
                    connectionPointsElement.AppendChild(xmlElement2);
                    this.CreateElementPosition(xmlElement2, roConn.x, (double)(this.m_ComosDocumentSizeY - roConn.y), 0);
                    xmlElement.AppendChild(this.CreateElementGenericAttribute(string.Concat("NodeName", i.ToString()), strArrays[i], "", "", "string", "", "", ""));
                }
                connectionPointsElement.SetAttribute("FlowIn", "1");
                connectionPointsElement.SetAttribute("FlowOut", "2");
                num = roConn.x;
                num1 = roConn.x;
                mComosDocumentSizeY = this.m_ComosDocumentSizeY - roConn.y;
                mComosDocumentSizeY1 = this.m_ComosDocumentSizeY - roConn.y;
                this.CreateElementExtent(connectionPointsElement, num, mComosDocumentSizeY, num1, mComosDocumentSizeY1);
                connectionPointsElement.AppendChild(xmlElement);
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
        }

        private XmlElement CreateElementCoordinate(double X, double Y, double Z)
        {
            XmlElement xmlElement;
            try
            {
                XmlElement xmlElement1 = this.m_xmlDoc.CreateElement("Coordinate");
                double num = Math.Round(X, 4);
                xmlElement1.SetAttribute("X", num.ToString().Replace(",", "."));
                num = Math.Round(Y, 4);
                xmlElement1.SetAttribute("Y", num.ToString().Replace(",", "."));
                num = Math.Round(Z, 4);
                xmlElement1.SetAttribute("Z", num.ToString().Replace(",", "."));
                xmlElement = xmlElement1;
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                xmlElement = null;
            }
            return xmlElement;
        }

        private void CreateElementExtent(XmlElement ParentNode, double x1, double y1, double x2, double y2)
        {
            try
            {
                XmlElement item = null;
                XmlElement xmlElement = null;
                XmlElement item1 = null;
                if (ParentNode.GetElementsByTagName("Extent").Count != 0)
                {
                    item = ParentNode["Extent"];
                    xmlElement = item["Min"];
                    item1 = item["Max"];
                }
                else
                {
                    item = this.m_xmlDoc.CreateElement("Extent");
                    ParentNode.AppendChild(item);
                }
                if (xmlElement == null)
                {
                    xmlElement = this.m_xmlDoc.CreateElement("Min");
                    item.AppendChild(xmlElement);
                }
                if (item1 == null)
                {
                    item1 = this.m_xmlDoc.CreateElement("Max");
                    item.AppendChild(item1);
                }
                double num = Math.Min(x1, x2);
                xmlElement.SetAttribute("X", num.ToString().Replace(",", "."));
                num = Math.Min(y1, y2);
                xmlElement.SetAttribute("Y", num.ToString().Replace(",", "."));
                xmlElement.SetAttribute("Z", "0");
                num = Math.Max(x1, x2);
                item1.SetAttribute("X", num.ToString().Replace(",", "."));
                num = Math.Max(y1, y2);
                item1.SetAttribute("Y", num.ToString().Replace(",", "."));
                item1.SetAttribute("Z", "0");
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
        }

        private XmlElement CreateElementGenericAttribute(string Name, string Value = "", string Units = "", string ComosUnits = "", string Format = "", string URI = "", string ValueURI = "", string UnitsURI = "")
        {
            XmlElement xmlElement;
            XmlElement xmlElement1 = null;
            try
            {
                if (!string.IsNullOrEmpty(Name))
                {
                    if (this.m_xvExportVersion != enmXMpLantVersion.XMpLantVersion_3_2_0)
                    {
                        xmlElement1 = this.m_xmlDoc.CreateElement("GenericAttribute");
                        xmlElement1.SetAttribute("Name", Name);
                    }
                    else
                    {
                        xmlElement1 = this.m_xmlDoc.CreateElement(Name);
                    }
                    xmlElement1.SetAttribute("Value", Value);
                    if (!string.IsNullOrEmpty(Units))
                    {
                        xmlElement1.SetAttribute("Units", Units);
                    }
                    if (!string.IsNullOrEmpty(Format))
                    {
                        xmlElement1.SetAttribute("Format", Format);
                    }
                    if (this.m_xvExportVersion <= enmXMpLantVersion.XMpLantVersion_3_3_3 && !string.IsNullOrEmpty(URI))
                    {
                        xmlElement1.SetAttribute("URI", URI);
                    }
                    if (this.m_xvExportVersion >= enmXMpLantVersion.XMpLantVersion_3_6_0)
                    {
                        if (!string.IsNullOrEmpty(URI))
                        {
                            xmlElement1.SetAttribute("AttributeURI", URI);
                        }
                        if (!string.IsNullOrEmpty(ValueURI))
                        {
                            xmlElement1.SetAttribute("ValueURI", ValueURI);
                        }
                        if (!string.IsNullOrEmpty(UnitsURI))
                        {
                            xmlElement1.SetAttribute("UnitsURI", UnitsURI);
                        }
                    }
                    if (this.m_xvExportVersion == enmXMpLantVersion.XMpLantVersion_3_2_0 && !string.IsNullOrEmpty(ComosUnits))
                    {
                        xmlElement1.SetAttribute("ComosUnits", ComosUnits);
                    }
                    xmlElement = xmlElement1;
                }
                else
                {
                    xmlElement = null;
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                return xmlElement1;
            }
            return xmlElement;
        }

        private XmlElement CreateElementInstrumentComponent(IComosDDevice DDev, Item ItemInDoc, object XObj)
        {
            XmlElement xmlElement;
            try
            {
                ComosGeoEngine comosGeoEngine = (XObj as IRoDeviceCommon).IBaseRoDevice.Container.GeoEngine(false) as ComosGeoEngine;
                string empty = string.Empty;
                this.SymbolIntoCatalog(DDev, comosGeoEngine, XObj, ItemInDoc, ref empty);
                XmlElement xmlElement1 = this.CreatePlantItem(DDev, "InstrumentComponent", false);
                this.CreateAttributesForPlantItem(xmlElement1, DDev, (this.HasLocalSymbolScript(ItemInDoc) ? enmXMpLantComponentType.Normal : enmXMpLantComponentType.Explicit), empty);
                this.SymbolToNode(comosGeoEngine, XObj, ItemInDoc, DDev, this.m_xmlDoc, xmlElement1, true);
                this.CreateXMpLantFixedAttributes(xmlElement1, DDev);
                xmlElement = xmlElement1;
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                xmlElement = null;
            }
            return xmlElement;
        }

        private XmlElement CreateElementLabel()
        {
            XmlElement xmlElement;
            try
            {
                XmlElement xmlElement1 = this.m_xmlDoc.CreateElement("Label");
                xmlElement1.SetAttribute("ID", this.GetAttributeID(false));
                xmlElement1.SetAttribute("ComponentClass", "Label");
                if (this.m_xvExportVersion >= enmXMpLantVersion.XMpLantVersion_3_6_0)
                {
                    xmlElement1.SetAttribute("ComponentClassURI", "SAGTBD");
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

        private XmlElement CreateElementPipeFlowArrow()
        {
            XmlElement xmlElement;
            try
            {
                XmlElement xmlElement1 = this.m_xmlDoc.CreateElement("PipeFlowArrow");
                xmlElement1.SetAttribute("ID", this.GetAttributeID(false));
                xmlElement1.SetAttribute("ComponentClass", "PipeFlowArrow");
                xmlElement = xmlElement1;
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                return null;
            }
            return xmlElement;
        }

        private XmlElement CreateElementPipingNetworkBranch()
        {
            XmlElement xmlElement;
            try
            {
                XmlElement xmlElement1 = this.m_xmlDoc.CreateElement("PipingComponent");
                xmlElement1.SetAttribute("ID", this.GetAttributeID(false));
                xmlElement1.SetAttribute("ComponentClass", "PipingNetworkBranch");
                if (this.m_xvExportVersion >= enmXMpLantVersion.XMpLantVersion_4_0_1)
                {
                    xmlElement1.SetAttribute("ComponentClass", "PipeTee");
                    xmlElement1.SetAttribute("ComponentClassURI", "http://data.posccaesar.org/rdl/RDS427724");
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

        private XmlElement CreateElementPipingNetworkSystem(XmlDocument xmlDoc, IConnectedRO cro, IComosDDevice DDevPipe, XmlElement xePNSegment)
        {
            XmlElement xmlElement;
            try
            {
                XmlElement xmlElement1 = null;
                if (DDevPipe != null)
                {
                    xmlElement1 = this.CreatePlantItem(DDevPipe, "PipingNetworkSystem", false);
                    this.CreateAttributesForPlantItem(xmlElement1, DDevPipe, enmXMpLantComponentType.NoComponentType, "");
                    this.CreateNodePersistentID(xmlElement1, DDevPipe);
                    this.AddGenericAttributesToPlantItem(xmlElement1, DDevPipe, false);
                    this.CreateElementCenterLine(cro, XMLHelper.GetParentNodePresentationElement(xePNSegment), DDevPipe);
                }
                else
                {
                    xmlElement1 = xmlDoc.CreateElement("PipingNetworkSystem");
                    this.CreateAttributesForPlantItem(xmlElement1, null, enmXMpLantComponentType.NoComponentType, "");
                    string str = (this.m_xvExportVersion == enmXMpLantVersion.XMpLantVersion_3_2_0 ? "Tag" : "TagName");
                    xmlElement1.SetAttribute(str, xePNSegment.GetAttribute(str));
                    xmlElement1.SetAttribute("StockNumber", "PipeDummy");
                }
                xmlElement = xmlElement1;
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                xmlElement = null;
            }
            return xmlElement;
        }

        private void CreateElementPlantInformation()
        {
            try
            {
                XmlElement xmlElement = this.m_xmlDoc.CreateElement("PlantInformation");
                xmlElement.SetAttribute("SchemaVersion", this.GetVersionString());
                xmlElement.SetAttribute("OriginatingSystem", "Comos");
                DateTime today = DateTime.Today;
                xmlElement.SetAttribute("Date", today.ToString("u").Substring(0, 10));
                today = DateTime.Now;
                xmlElement.SetAttribute("Time", today.ToString("HH:mm:ss"));
                xmlElement.SetAttribute("Is3D", "no");
                xmlElement.SetAttribute("Units", "mm");
                xmlElement.SetAttribute("Discipline", "PID");
                this.CreateElementUnitsOfMeasure(xmlElement);
                this.m_Root.AppendChild(xmlElement);
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
        }

        private XmlElement CreateElementPolyLineFromRectangle(Item ItemInDoc)
        {
            try
            {
                ReportRect itemInDoc = ItemInDoc as ReportRect;
                if (itemInDoc != null)
                {
                    XmlElement xmlElement = this.m_xmlDoc.CreateElement("PolyLine");
                    xmlElement.SetAttribute("NumPoints", "5");
                    this.m_Root.AppendChild(xmlElement);
                    XmlDocument mXmlDoc = this.m_xmlDoc;
                    string str = ItemInDoc.Layer.ToString();
                    string str1 = ItemInDoc.LineType.ToString();
                    double width = ItemInDoc.Width;
                    string str2 = width.ToString().Replace(",", ".");
                    int color = ItemInDoc.Color;
                    this.CreateElementPresentation(mXmlDoc, xmlElement, str, str1, str2, color.ToString());
                    double p1 = itemInDoc.P1.x;
                    double p2 = itemInDoc.P2.x;
                    double mComosDocumentSizeY = this.m_ComosDocumentSizeY - itemInDoc.P1.y;
                    double num = this.m_ComosDocumentSizeY - itemInDoc.P2.y;
                    this.CreateElementExtent(xmlElement, p1, mComosDocumentSizeY, p2, num);
                    xmlElement.AppendChild(this.CreateElementCoordinate(p1, mComosDocumentSizeY, 0));
                    xmlElement.AppendChild(this.CreateElementCoordinate(p2, mComosDocumentSizeY, 0));
                    xmlElement.AppendChild(this.CreateElementCoordinate(p2, num, 0));
                    xmlElement.AppendChild(this.CreateElementCoordinate(p1, num, 0));
                    xmlElement.AppendChild(this.CreateElementCoordinate(p1, mComosDocumentSizeY, 0));
                    return xmlElement;
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
            return null;
        }

        private XmlElement CreateElementPolyLineFromRectangle(Item ItemInDoc, NDisplayElement displayElement, IGeometry iGeo, double theta, bool IsPlantItemInstance)
        {
            try
            {
                NRectClass umreck = displayElement.Geometrie.Umreck as NRectClass;
                if (umreck != null)
                {
                    XmlElement xmlElement = this.m_xmlDoc.CreateElement("PolyLine");
                    xmlElement.SetAttribute("NumPoints", "5");
                    this.m_Root.AppendChild(xmlElement);
                    XmlDocument mXmlDoc = this.m_xmlDoc;
                    string layer = displayElement.Header.Layer;
                    int lineTyp = displayElement.Header.LineTyp;
                    string str = lineTyp.ToString();
                    float width = displayElement.Header.Width;
                    string str1 = width.ToString().Replace(",", ".");
                    lineTyp = displayElement.Header.Color;
                    this.CreateElementPresentation(mXmlDoc, xmlElement, layer, str, str1, lineTyp.ToString());
                    double leftTop = umreck.LeftTop.x;
                    double rightBottom = umreck.RightBottom.x;
                    double num = umreck.LeftTop.y;
                    double rightBottom1 = umreck.RightBottom.y;
                    double p1 = leftTop;
                    double mComosDocumentSizeY = num;
                    double p11 = rightBottom;
                    double mComosDocumentSizeY1 = rightBottom1;
                    if (LateBinding.LateGet(ItemInDoc, null, "XObj", null, null, null) is IPfdRoFlag)
                    {
                        this.TransformCoord(theta, 1 / iGeo.ScaleValueX, 1 / iGeo.ScaleValueY, iGeo.Reflect, leftTop, num, out p1, out mComosDocumentSizeY);
                        this.TransformCoord(theta, 1 / iGeo.ScaleValueX, 1 / iGeo.ScaleValueY, iGeo.Reflect, rightBottom, rightBottom1, out p11, out mComosDocumentSizeY1);
                        p1 += ItemInDoc.P1.x;
                        mComosDocumentSizeY = mComosDocumentSizeY + (this.m_ComosDocumentSizeY - ItemInDoc.P1.y);
                        p11 += ItemInDoc.P1.x;
                        mComosDocumentSizeY1 = mComosDocumentSizeY1 + (this.m_ComosDocumentSizeY - ItemInDoc.P1.y);
                    }
                    else if (!IsPlantItemInstance)
                    {
                        this.TransformCoord(-theta, 1 / iGeo.ScaleValueX, 1 / iGeo.ScaleValueY, iGeo.Reflect, leftTop, num, out p1, out mComosDocumentSizeY);
                        this.TransformCoord(-theta, 1 / iGeo.ScaleValueX, 1 / iGeo.ScaleValueY, iGeo.Reflect, rightBottom, rightBottom1, out p11, out mComosDocumentSizeY1);
                    }
                    this.CreateElementExtent(xmlElement, p1, mComosDocumentSizeY, p11, mComosDocumentSizeY1);
                    xmlElement.AppendChild(this.CreateElementCoordinate(p1, mComosDocumentSizeY, 0));
                    xmlElement.AppendChild(this.CreateElementCoordinate(p11, mComosDocumentSizeY, 0));
                    xmlElement.AppendChild(this.CreateElementCoordinate(p11, mComosDocumentSizeY1, 0));
                    xmlElement.AppendChild(this.CreateElementCoordinate(p1, mComosDocumentSizeY1, 0));
                    xmlElement.AppendChild(this.CreateElementCoordinate(p1, mComosDocumentSizeY, 0));
                    return xmlElement;
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
            return null;
        }

        private void CreateElementPosition(XmlElement parentNode, double x1, double y1, double theta)
        {
            this.CreateElementPosition(parentNode, x1, y1, theta, false);
        }

        private void CreateElementPosition(XmlElement parentNode, double x1, double y1, double theta, bool IsReflect)
        {
            try
            {
                XmlElement xmlElement = this.m_xmlDoc.CreateElement("Position");
                XmlElement xmlElement1 = this.m_xmlDoc.CreateElement("Location");
                XmlElement xmlElement2 = this.m_xmlDoc.CreateElement("Axis");
                XmlElement xmlElement3 = this.m_xmlDoc.CreateElement("Reference");
                parentNode.AppendChild(xmlElement);
                xmlElement.AppendChild(xmlElement1);
                xmlElement.AppendChild(xmlElement2);
                xmlElement.AppendChild(xmlElement3);
                double num = Math.Round(x1, 4);
                xmlElement1.SetAttribute("X", num.ToString().Replace(",", "."));
                num = Math.Round(y1, 4);
                xmlElement1.SetAttribute("Y", num.ToString().Replace(",", "."));
                xmlElement1.SetAttribute("Z", "0");
                xmlElement2.SetAttribute("X", "0");
                xmlElement2.SetAttribute("Y", "0");
                xmlElement2.SetAttribute("Z", "1");
                if (IsReflect)
                {
                    xmlElement2.SetAttribute("Z", "-1");
                    theta -= 3.14159265358979;
                }
                num = Math.Round(Math.Cos(theta), 4);
                xmlElement3.SetAttribute("X", num.ToString().Replace(",", "."));
                num = Math.Round(Math.Sin(theta), 4);
                xmlElement3.SetAttribute("Y", num.ToString().Replace(",", "."));
                xmlElement3.SetAttribute("Z", "0");
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
        }

        private void CreateElementPresentation(XmlDocument xmlDoc, XmlElement ParentNode, string strLayer, string strComosLineType, string strLineWeight, string strColor)
        {
            int num;
            try
            {
                strLineWeight = strLineWeight.Replace(",", ".");
                int.TryParse(strColor, out num);
                num = this.BGRtoRGB(num);
                XmlElement xmlElement = xmlDoc.CreateElement("Presentation");
                ParentNode.AppendChild(xmlElement);
                xmlElement.SetAttribute("Layer", strLayer);
                xmlElement.SetAttribute("Color", num.ToString());
                xmlElement.SetAttribute("LineType", this.TranslateLineType(strComosLineType));
                xmlElement.SetAttribute("LineWeight", strLineWeight);
                string str = "0";
                string str1 = "0";
                string str2 = "0";
                if (num > 0)
                {
                    Color color = Color.FromArgb(num);
                    double r = (double)color.R / 255;
                    str = r.ToString("0.000").Replace(",", ".");
                    r = (double)color.G / 255;
                    str1 = r.ToString("0.000").Replace(",", ".");
                    r = (double)color.B / 255;
                    str2 = r.ToString("0.000").Replace(",", ".");
                }
                xmlElement.SetAttribute("R", str);
                xmlElement.SetAttribute("G", str1);
                xmlElement.SetAttribute("B", str2);
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
        }

        private void CreateElementScale(XmlElement parentNode, double ScaleX, double ScaleY, double ScaleZ)
        {
            XmlElement xmlElement = this.m_xmlDoc.CreateElement("Scale");
            parentNode.AppendChild(xmlElement);
            double num = Math.Round(ScaleX, 4);
            xmlElement.SetAttribute("X", num.ToString().Replace(",", "."));
            num = Math.Round(ScaleY, 4);
            xmlElement.SetAttribute("Y", num.ToString().Replace(",", "."));
            num = Math.Round(ScaleZ, 4);
            xmlElement.SetAttribute("Z", num.ToString().Replace(",", "."));
        }

        private XmlElement CreateElementSignalLine(object XObj, XmlDocument xmlDoc)
        {
            XmlElement xmlElement;
            IConnectedRO xObj = XObj as IConnectedRO;
            IRoConnectors roConnector = xObj.Connectors();
            roConnector.Item(1);
            IGraphicAttributes graphicAttribute = XObj as IGraphicAttributes;
            xmlElement = (this.m_xvExportVersion < enmXMpLantVersion.XMpLantVersion_4_0_1 ? xmlDoc.CreateElement("SignalLine") : xmlDoc.CreateElement("InformationFlow"));
            if (((IRoConnPfd)xObj).ConnectionType == PfdConnectionType.PROCESS_LINE)
            {
                xmlElement.SetAttribute("ComponentClass", "MeasuringLineFunction");
                xmlElement.SetAttribute("ComponentClassURI", "http://sandbox.dexpi.org/rdl/MeasuringLineFunction");
            }
            if (((IRoConnPfd)xObj).ConnectionType == PfdConnectionType.ACTION_LINE)
            {
                xmlElement.SetAttribute("ComponentClass", "SignalLineFunction");
                xmlElement.SetAttribute("ComponentClassURI", "http://sandbox.dexpi.org/rdl/SignalLineFunction");
            }
            string str = graphicAttribute.LineType.ToString();
            string str1 = graphicAttribute.Layer.ToString();
            string str2 = graphicAttribute.Breadth.ToString();
            int color = graphicAttribute.Color;
            this.CreateElementPresentation(xmlDoc, xmlElement, str1, str, str2, color.ToString());
            this.CreateElementExtent(xmlElement, roConnector.Item(1).x, this.m_ComosDocumentSizeY - roConnector.Item(1).y, roConnector.Item(2).x, this.m_ComosDocumentSizeY - roConnector.Item(2).y);
            this.CreateElementPosition(xmlElement, 0, 0, 0);
            xmlElement.AppendChild(this.CreateElementCenterLine(xObj, XMLHelper.GetParentNodePresentationElement(xmlElement), null));
            return xmlElement;
        }

        private XmlElement CreateElementText(string strString, string strFont, string strJustification, string strWidth, string strHeight, string strTextAngle, string strSlantAngle, string strDependantAttribute, string strItemID)
        {
            XmlElement xmlElement;
            try
            {
                XmlElement xmlElement1 = this.m_xmlDoc.CreateElement("Text");
                xmlElement1.SetAttribute("Font", strFont);
                xmlElement1.SetAttribute("Width", strWidth);
                xmlElement1.SetAttribute("Height", strHeight);
                if (strString.Trim() != "")
                {
                    xmlElement1.SetAttribute("String", strString);
                }
                if (strJustification.Trim() != "")
                {
                    xmlElement1.SetAttribute("Justification", strJustification);
                }
                if (strFont.Trim() != "")
                {
                    xmlElement1.SetAttribute("TextAngle", strTextAngle);
                }
                if (strFont.Trim() != "")
                {
                    xmlElement1.SetAttribute("SlantAngle", strSlantAngle);
                }
                if (strDependantAttribute.Trim() != "")
                {
                    xmlElement1.SetAttribute("DependantAttribute", strDependantAttribute);
                }
                if (strItemID.Trim() != "")
                {
                    xmlElement1.SetAttribute("ItemID", strItemID);
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

        private void CreateElementUnitsOfMeasure(XmlElement xeParent)
        {
            try
            {
                XmlElement xmlElement = this.m_xmlDoc.CreateElement("UnitsOfMeasure");
                xmlElement.SetAttribute("Distance", "mm");
                if (this.m_stUnitMapping != null)
                {
                    for (int i = 1; i <= this.m_stUnitMapping.StandardValues().Count(); i++)
                    {
                        IComosDStandardValue comosDStandardValue = (IComosDStandardValue)this.m_stUnitMapping.StandardValues().Item(i);
                        string upper = comosDStandardValue.GetXValue(4).ToUpper();
                        string xValue = comosDStandardValue.GetXValue(1);
                        if (!string.IsNullOrEmpty(upper) && !string.IsNullOrEmpty(xValue))
                        {
                            if (upper == "Area".ToUpper())
                            {
                                xmlElement.SetAttribute("Area", xValue);
                            }
                            if (upper == "Angle".ToUpper())
                            {
                                xmlElement.SetAttribute("Angle", xValue);
                            }
                            if (upper == "Distance".ToUpper())
                            {
                                xmlElement.SetAttribute("Distance", xValue);
                            }
                            if (upper == "Temperature".ToUpper())
                            {
                                xmlElement.SetAttribute("Temperature", xValue);
                            }
                            if (upper == "Pressure".ToUpper())
                            {
                                xmlElement.SetAttribute("Pressure", xValue);
                            }
                            if (upper == "Volume".ToUpper())
                            {
                                xmlElement.SetAttribute("Volume", xValue);
                            }
                            if (upper == "Weight".ToUpper())
                            {
                                xmlElement.SetAttribute("Weight", xValue);
                            }
                        }
                    }
                    xeParent.AppendChild(xmlElement);
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
        }

        private void CreateGenericAttribute(IComosDSpecification specAttribute, bool fExportAttributesWithEmptyValues, IComosDStandardTable stdtIsoMappingTable, XmlElement xeGenericAttributesSet, bool PlacedAttribute = false)
        {
            string str;
            string str1;
            bool flag;
            string str2;
            string str3 = this.RemoveSymbol(specAttribute.Description).Replace(" ", "");
            if (string.IsNullOrEmpty(str3))
            {
                return;
            }
            string str4 = specAttribute.DisplayValue().Trim();
            string[] strArrays = specAttribute.XValueArray().Split(new char[] { '|' });
            for (int i = (int)Enum.GetValues(typeof(SvgExport.enmXValueType)).Cast<SvgExport.enmXValueType>().Min<SvgExport.enmXValueType>(); i <= (int)Enum.GetValues(typeof(SvgExport.enmXValueType)).Cast<SvgExport.enmXValueType>().Max<SvgExport.enmXValueType>(); i++)
            {
                SvgExport.enmXValueType _enmXValueType = (SvgExport.enmXValueType)i;
                string description = specAttribute.Description;
                string str5 = str3;
                if (_enmXValueType != SvgExport.enmXValueType.Normal)
                {
                    description = string.Concat(description, "_", _enmXValueType.GetHashCode());
                }
                if (_enmXValueType == SvgExport.enmXValueType.Normal && specAttribute.RangeType < 2)
                {
                    str = str4;
                }
                else if (_enmXValueType != SvgExport.enmXValueType.Min || specAttribute.RangeType <= 0)
                {
                    if (_enmXValueType != SvgExport.enmXValueType.Max || specAttribute.RangeType <= 0 || strArrays.GetUpperBound(0) < 1)
                    {
                        goto Label0;
                    }
                    str = strArrays[1].Trim();
                }
                else
                {
                    str = strArrays[0].Trim();
                }
                if (fExportAttributesWithEmptyValues || !string.IsNullOrEmpty(str))
                {
                    this.GetAttributeIsoNameAndUri(stdtIsoMappingTable, specAttribute.NestedName(), description, _enmXValueType, out str5, out str1, out flag);
                    if (!flag || PlacedAttribute)
                    {
                        string str6 = string.Concat(new string[] { "./", "GenericAttribute", "[starts-with(@", "Name", ", '", str5, "')]" });
                        if (this.m_xvExportVersion == enmXMpLantVersion.XMpLantVersion_3_2_0)
                        {
                            str6 = str5;
                        }
                        XmlNodeList xmlNodeLists = xeGenericAttributesSet.SelectNodes(str6);
                        if (xmlNodeLists != null && xmlNodeLists.Count > 0)
                        {
                            str5 = string.Concat(str5, xmlNodeLists.Count + 1);
                        }
                        string str7 = specAttribute.PhysUnitLabel().Trim();
                        this.GetAttributeUnitIsoNameAndUri(specAttribute.Unit, ref str7, out str2);
                        XmlElement xmlElement = this.CreateElementGenericAttribute(str5, str, str7, string.Concat(specAttribute.PhysUnitLabel(), " ", specAttribute.Unit).Trim(), this.GetGenericAttributeFormat(specAttribute.Type), str1, "", str2);
                        if (xmlElement != null)
                        {
                            xeGenericAttributesSet.AppendChild(xmlElement);
                        }
                    }
                }
            Label0: return;
            }
        }

        private XmlElement CreateGenericAttributesNameDescription(IComosBaseObject comosBaseObject)
        {
            XmlElement xmlElement;
            try
            {
                if (comosBaseObject == null)
                {
                    xmlElement = null;
                }
                else
                {
                    XmlElement xmlElement1 = this.m_xmlDoc.CreateElement("GenericAttributes");
                    xmlElement1.SetAttribute("Set", "ComosProperties");
                    xmlElement1.AppendChild(this.CreateElementGenericAttribute("SystemUID", comosBaseObject.SystemUID(), "", "", "string", "", "", ""));
                    xmlElement1.AppendChild(this.CreateElementGenericAttribute("Description", comosBaseObject.Description, "", "", "string", "", "", ""));
                    xmlElement1.AppendChild(this.CreateElementGenericAttribute("Name", comosBaseObject.Name, "", "", "string", "", "", ""));
                    xmlElement1.AppendChild(this.CreateElementGenericAttribute("FullName", comosBaseObject.FullName(), "", "", "string", "", "", ""));
                    xmlElement1.AppendChild(this.CreateElementGenericAttribute("Label", comosBaseObject.Label, "", "", "string", "", "", ""));
                    xmlElement1.AppendChild(this.CreateElementGenericAttribute("FullLabel", comosBaseObject.FullLabel(), "", "", "string", "", "", ""));
                    xmlElement1.AppendChild(this.CreateElementGenericAttribute("AliasFullLabel", comosBaseObject.AliasFullLabel(), "", "", "string", "", "", ""));
                    xmlElement1.AppendChild(this.CreateElementGenericAttribute("PathFullName", comosBaseObject.PathFullName(null), "", "", "string", "", "", ""));
                    if (comosBaseObject is IComosDCDevice)
                    {
                        IComosDCDevice comosDCDevice = (IComosDCDevice)comosBaseObject;
                        xmlElement1.AppendChild(this.CreateElementGenericAttribute("ComosClass", comosDCDevice.Class, "", "", "string", "", "", ""));
                        xmlElement1.AppendChild(this.CreateElementGenericAttribute("ComosDetailClass", comosDCDevice.DetailClass, "", "", "string", "", "", ""));
                        xmlElement1.AppendChild(this.CreateElementGenericAttribute("ComosRIClass", comosDCDevice.RIClass, "", "", "string", "", "", ""));
                    }
                    if (comosBaseObject is IComosDDevice)
                    {
                        IComosDDevice comosDDevice = (IComosDDevice)comosBaseObject;
                        xmlElement1.AppendChild(this.CreateElementGenericAttribute("ComosClass", comosDDevice.Class, "", "", "string", "", "", ""));
                        xmlElement1.AppendChild(this.CreateElementGenericAttribute("ComosDetailClass", comosDDevice.DetailClass, "", "", "string", "", "", ""));
                        xmlElement1.AppendChild(this.CreateElementGenericAttribute("ComosRIClass", comosDDevice.RIClass, "", "", "string", "", "", ""));
                    }
                    xmlElement1.SetAttribute("Number", xmlElement1.ChildNodes.Count.ToString());
                    xmlElement = xmlElement1;
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                xmlElement = null;
            }
            return xmlElement;
        }

        private XmlElement CreateGenericAttributesSet(string strChapterName)
        {
            XmlElement xmlElement = this.m_xmlDoc.CreateElement("GenericAttributes");
            xmlElement.SetAttribute("Set", strChapterName);
            return xmlElement;
        }

        private void CreateGraphicalElements(ComosGeoEngine Ge, Item ItemInDoc, XmlDocument xmlDoc, XmlElement childNode, bool IsPlantItemInstance, IGeometry IGeo)
        {
            int displayElementCount = Ge.DisplayElementCount;
            ArrayList allHatch = this.GetAllHatch(Ge);
            double theta = this.getTheta(IGeo);
            for (int i = 0; i <= displayElementCount - 1; i++)
            {
                NDisplayElement displayElement = Ge.DisplayElement[i];
                this.WriteGraphicalElements(displayElement, Ge, IGeo, xmlDoc, childNode, ItemInDoc, IsPlantItemInstance, theta, allHatch, null);
            }
        }

        private bool CreateInstrument(IComosDDevice DDev, Item ItemInDoc, object XObj)
        {
            bool flag;
            try
            {
                XmlElement elementInstrumentLoop = null;
                XmlElement xmlElement = null;
                bool flag1 = false;
                if (this.m_xvExportVersion == enmXMpLantVersion.XMpLantVersion_3_2_0)
                {
                    elementInstrumentLoop = this.GetElementInstrumentLoop(DDev, out flag1);
                    if (!flag1)
                    {
                        this.m_Root.AppendChild(elementInstrumentLoop);
                    }
                    xmlElement = this.CreateInstrumentElement(DDev, ItemInDoc, XObj);
                    if (elementInstrumentLoop == null)
                    {
                        this.m_Root.AppendChild(xmlElement);
                    }
                    else
                    {
                        elementInstrumentLoop.AppendChild(xmlElement);
                        if (elementInstrumentLoop.Attributes["StockNumber"] != null && xmlElement.Attributes["Tag"] != null && elementInstrumentLoop.Attributes["StockNumber"].Value == "InstrumentLoopDummy")
                        {
                            elementInstrumentLoop.SetAttribute("Tag", xmlElement.Attributes["Tag"].Value);
                        }
                    }
                }
                else if (this.m_xvExportVersion >= enmXMpLantVersion.XMpLantVersion_3_3_3 && this.m_xvExportVersion <= enmXMpLantVersion.XMpLantVersion_3_6_0)
                {
                    string comosObjectAttributeValueByNestedName = this.getComosObjectAttributeValueByNestedName(DDev, SvgExport.COMOS_XMPLANT_CHAPTER_NAME, SvgExport.COMOS_XMPLANT_PLANT_ITEM_NODE_NAME);
                    if (comosObjectAttributeValueByNestedName == "ProcessInstrument")
                    {
                        xmlElement = this.CreateInstrumentElement(DDev, ItemInDoc, XObj);
                        this.m_Root.AppendChild(xmlElement);
                    }
                    else if (comosObjectAttributeValueByNestedName == "InstrumentComponent")
                    {
                        xmlElement = this.CreateElementInstrumentComponent(DDev, ItemInDoc, XObj);
                        this.m_Root.AppendChild(xmlElement);
                    }
                }
                else if (this.m_xvExportVersion >= enmXMpLantVersion.XMpLantVersion_4_0_1)
                {
                    elementInstrumentLoop = this.GetElementInstrumentLoop(DDev, out flag1);
                    xmlElement = this.CreateInstrumentElement(DDev, ItemInDoc, XObj);
                    elementInstrumentLoop.AppendChild(this.CreateElementAssociation(AssociationType.isacollectionincluding, xmlElement.Attributes["ID"].Value));
                    xmlElement.AppendChild(this.CreateElementAssociation(AssociationType.isapartof, elementInstrumentLoop.Attributes["ID"].Value));
                    if (!flag1)
                    {
                        this.m_Root.AppendChild(elementInstrumentLoop);
                    }
                    this.m_Root.AppendChild(xmlElement);
                }
                flag = xmlElement != null;
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                return false;
            }
            return flag;
        }

        private void CreateInstrumentConnectionInNS(XmlElement xeForecomingCenterLine, IComosDConnector piObj1)
        {
            XmlElement xmlElement = this.m_xmlDoc.CreateElement("InstrumentConnection");
            xmlElement.SetAttribute("ID", this.GetAttributeID(false));
            xeForecomingCenterLine.ParentNode.InsertAfter(xmlElement, xeForecomingCenterLine);
            this.AddElementsToReorderCollection(new SvgExport.m_ReorderElements(xeForecomingCenterLine, xmlElement));
        }

        private XmlElement CreateInstrumentElement(IComosDDevice DDev, Item ItemInDoc, object XObj)
        {
            XmlElement xmlElement;
            XmlElement xmlElement1;
            try
            {
                ComosGeoEngine comosGeoEngine = (XObj as IRoDeviceCommon).IBaseRoDevice.Container.GeoEngine(false) as ComosGeoEngine;
                string empty = string.Empty;
                this.SymbolIntoCatalog(DDev, comosGeoEngine, XObj, ItemInDoc, ref empty);
                xmlElement = (this.m_xvExportVersion > enmXMpLantVersion.XMpLantVersion_3_6_0 ? this.CreatePlantItem(DDev, "ProcessInstrumentationFunction", false) : this.CreatePlantItem(DDev, "ProcessInstrument", false));
                this.CreateAttributesForPlantItem(xmlElement, DDev, (this.HasLocalSymbolScript(ItemInDoc) ? enmXMpLantComponentType.Normal : enmXMpLantComponentType.Explicit), empty);
                this.SymbolToNode(comosGeoEngine, XObj, ItemInDoc, DDev, this.m_xmlDoc, xmlElement, true);
                this.CreateXMpLantFixedAttributes(xmlElement, DDev);
                xmlElement1 = xmlElement;
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                return null;
            }
            return xmlElement1;
        }

        private XmlElement CreateLabel(object xObj, Item reportItem)
        {
            XmlElement xmlElement;
            try
            {
                XmlElement xmlElement1 = this.CreateElementLabel();
                if (xmlElement1 != null)
                {
                    IRoDeviceCommon roDeviceCommon = xObj as IRoDevice as IRoDeviceCommon;
                    roDeviceCommon.IBaseRoDevice.Container.EvalDone = 1;
                    ComosGeoEngine comosGeoEngine = roDeviceCommon.IBaseRoDevice.Container.GeoEngine(false) as ComosGeoEngine;
                    this.CreateElementExtent(xmlElement1, comosGeoEngine.Umreck.left + reportItem.x1, this.m_ComosDocumentSizeY + comosGeoEngine.Umreck.bottom - reportItem.y1, comosGeoEngine.Umreck.right + reportItem.x1, this.m_ComosDocumentSizeY + comosGeoEngine.Umreck.top - reportItem.y1);
                    this.CreateElementPosition(xmlElement1, XMLHelper.GetExtentValue(xmlElement1, "Min", "X"), XMLHelper.GetExtentValue(xmlElement1, "Min", "Y"), 0);
                    IGeometry geometry = xObj as IGeometry;
                    this.CreateGraphicalElements(comosGeoEngine, reportItem, this.m_xmlDoc, xmlElement1, true, geometry);
                    this.CreateTextElements(comosGeoEngine, reportItem, xmlElement1);
                    xmlElement = xmlElement1;
                }
                else
                {
                    xmlElement = null;
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                return null;
            }
            return xmlElement;
        }

        private void CreateLine(double P1x, double P1y, double P2x, double P2y, XmlDocument xmlDoc, XmlElement childNode, Item ItemInDoc, bool IsPlantItemInstance, bool gra, double theta, NDisplayElement De, IGeometry igeo)
        {
            double itemInDoc;
            double mComosDocumentSizeY;
            double p2x;
            double num;
            int lineTyp;
            float width;
            try
            {
                XmlElement xmlElement = xmlDoc.CreateElement("Line");
                childNode.AppendChild(xmlElement);
                if (IsPlantItemInstance)
                {
                    string layer = De.Header.Layer;
                    lineTyp = De.Header.LineTyp;
                    string str = lineTyp.ToString();
                    width = De.Header.Width;
                    string str1 = width.ToString().Replace(",", ".");
                    lineTyp = De.Header.Color;
                    this.CreateElementPresentation(xmlDoc, xmlElement, layer, str, str1, lineTyp.ToString());
                    itemInDoc = ItemInDoc.x1 + P1x;
                    mComosDocumentSizeY = this.m_ComosDocumentSizeY + P1y - ItemInDoc.y1;
                    p2x = ItemInDoc.x1 + P2x;
                    num = this.m_ComosDocumentSizeY + P2y - ItemInDoc.y1;
                    this.CreateElementExtent(xmlElement, itemInDoc, mComosDocumentSizeY, p2x, num);
                    xmlElement.AppendChild(this.CreateElementCoordinate(itemInDoc, mComosDocumentSizeY, 0));
                    xmlElement.AppendChild(this.CreateElementCoordinate(p2x, num, 0));
                }
                else if (!gra)
                {
                    string layer1 = De.Header.Layer;
                    lineTyp = De.Header.LineTyp;
                    string str2 = lineTyp.ToString();
                    width = De.Header.Width;
                    string str3 = width.ToString().Replace(",", ".");
                    lineTyp = De.Header.Color;
                    this.CreateElementPresentation(xmlDoc, xmlElement, layer1, str2, str3, lineTyp.ToString());
                    this.TransformCoord(-theta, 1 / igeo.ScaleValueX, 1 / igeo.ScaleValueY, igeo.Reflect, P1x, P1y, out itemInDoc, out mComosDocumentSizeY);
                    this.TransformCoord(-theta, 1 / igeo.ScaleValueX, 1 / igeo.ScaleValueY, igeo.Reflect, P2x, P2y, out p2x, out num);
                    this.CreateElementExtent(xmlElement, itemInDoc, mComosDocumentSizeY, p2x, num);
                    xmlElement.AppendChild(this.CreateElementCoordinate(itemInDoc, mComosDocumentSizeY, 0));
                    xmlElement.AppendChild(this.CreateElementCoordinate(p2x, num, 0));
                }
                else
                {
                    string str4 = ItemInDoc.Layer.ToString();
                    string str5 = ItemInDoc.LineType.ToString();
                    double width1 = ItemInDoc.Width;
                    string str6 = width1.ToString().Replace(",", ".");
                    lineTyp = ItemInDoc.Color;
                    this.CreateElementPresentation(xmlDoc, xmlElement, str4, str5, str6, lineTyp.ToString());
                    itemInDoc = P1x;
                    p2x = P2x;
                    mComosDocumentSizeY = this.m_ComosDocumentSizeY - P1y;
                    num = this.m_ComosDocumentSizeY - P2y;
                    this.CreateElementExtent(xmlElement, itemInDoc, mComosDocumentSizeY, p2x, num);
                    xmlElement.AppendChild(this.CreateElementCoordinate(itemInDoc, mComosDocumentSizeY, 0));
                    xmlElement.AppendChild(this.CreateElementCoordinate(p2x, num, 0));
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
        }

        private void CreateNodeDrawing(XmlElement ParentNode, Document RDDoc)
        {
            try
            {
                IComosDDocument comosDDocument = (IComosDDocument)RDDoc.ComosDocument();
                this.m_Drawing = this.m_xmlDoc.CreateElement("Drawing");
                ParentNode.AppendChild(this.m_Drawing);
                string comosObjectAttributeValueByNestedName = this.getComosObjectAttributeValueByNestedName(comosDDocument, SvgExport.COMOS_XMPLANT_CHAPTER_NAME, SvgExport.COMOS_XMPLANT_DOCUMENT_NAME);
                if (comosObjectAttributeValueByNestedName == string.Empty)
                {
                    comosObjectAttributeValueByNestedName = comosDDocument.NestedName();
                }
                this.m_Drawing.SetAttribute("Name", comosObjectAttributeValueByNestedName);
                this.m_Drawing.SetAttribute("Type", "PID");
                this.m_Drawing.SetAttribute("Title", comosDDocument.Description);
                this.CreateDrawingPresentationExtent(RDDoc, this.m_Drawing);
                if ((this.getComosObjectAttributeValueByNestedName(comosDDocument, SvgExport.COMOS_XMPLANT_CHAPTER_NAME, SvgExport.COMOS_XMPLANT_EXPORT_BORDER) == "1" ? true : false))
                {
                    this.CreateNodeDrawingBorder(this.m_Drawing, RDDoc);
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
        }

        private void CreateNodeDrawingBorder(XmlElement ParentNode, Document RDDoc)
        {
            try
            {
                this.m_DrawingBorder = this.m_xmlDoc.CreateElement("DrawingBorder");
                ParentNode.AppendChild(this.m_DrawingBorder);
                this.CreateDrawingPresentationExtent(RDDoc, this.m_DrawingBorder);
                this.m_ReportExport.ExportDocument(RDDoc.MasterDocument());
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
        }

        private void CreateNodePersistentID(XmlElement ParentNode, IComosBaseObject comosBaseObject)
        {
            try
            {
                IComosDSpecification comosDSpecification = comosBaseObject.spec(string.Concat(SvgExport.COMOS_XMPLANT_CHAPTER_NAME, ".", SvgExport.COMOS_XMPLANT_PERSISTENT_ID));
                string str = "";
                if (comosDSpecification != null)
                {
                    str = comosDSpecification.@value;
                }
                if (str.Trim() == "")
                {
                    str = comosBaseObject.SystemUID();
                }
                XmlElement xmlElement = this.m_xmlDoc.CreateElement("PersistentID");
                ParentNode.AppendChild(xmlElement);
                xmlElement.SetAttribute("Identifier", str);
                xmlElement.SetAttribute("Context", "Comos");
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
        }

        private void CreateNonExistingGenericAttribute(XmlElement xeParentObject, IComosDSpecification specAttribute, string strDependantAttributeName, IComosDStandardTable stdtIsoMappingTable)
        {
            if (xeParentObject == null || specAttribute == null || XMLHelper.GetGenericAttributeByName(strDependantAttributeName, xeParentObject) != null)
            {
                return;
            }
            IComosDSpecification comosDSpecification = specAttribute.owner() as IComosDSpecification;
            if (comosDSpecification == null)
            {
                return;
            }
            XmlElement genericAttributesSet = XMLHelper.GetGenericAttributesSet(this.GetComosChapterName(comosDSpecification), xeParentObject) ?? this.CreateGenericAttributesSet(this.GetComosChapterName(comosDSpecification));
            if (genericAttributesSet == null)
            {
                return;
            }
            this.CreateGenericAttribute(specAttribute, true, stdtIsoMappingTable, genericAttributesSet, true);
            SvgExport.SetAttributeCount(genericAttributesSet);
        }

        private void CreatePipeConnectorSymbolsForSegment(IComosDDevice dDevice, ComosGeoEngine Ge, Item ItemInDoc, object XObj)
        {
            try
            {
                if (dDevice != null)
                {
                    IComosDDocObj comosDDocObj = SvgExport.IntPipeLib.FindBeforeSegmentsPlacedOnOtherReport(dDevice);
                    IComosDDocObj comosDDocObj1 = SvgExport.IntPipeLib.FindAfterSegmentsPlacedOnOtherReport(dDevice);
                    if (comosDDocObj != null || comosDDocObj1 != null)
                    {
                        XmlElement xmlElement = XMLHelper.CreateElementPipeConnectorSymbol(this.m_xmlDoc, this.GetAttributeID(false), "PipeConnectorSymbol", "", "", "", "", "", "", "");
                        this.CreateNodePersistentID(xmlElement, dDevice);
                        this.CreateGraphicalElements(Ge, ItemInDoc, this.m_xmlDoc, xmlElement, true, XObj as IGeometry);
                        this.m_Drawing.AppendChild(xmlElement);
                    }
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
        }

        private void CreatePipeStru(object XObj, Item ItemInDoc)
        {
            try
            {
                IConnectedRO xObj = XObj as IConnectedRO;
                IComosDDevice device = (XObj as IRoDevice).Device;
                if (device != null)
                {
                    IComosDDevice comosDDevice = null;
                    string str = "";
                    XmlElement xmlElement = null;
                    XmlElement elementByUID = null;
                    bool flag = false;
                    bool flag1 = false;
                    if (this.IsComosSegment(device))
                    {
                        comosDDevice = device;
                        device = device.owner() as IComosDDevice;
                    }
                    if (this.IsComosBranch(device))
                    {
                        str = device.SystemUID();
                        XmlElement elementByUID1 = XMLHelper.GetElementByUID(this.m_xvExportVersion, this.m_xmlDoc.DocumentElement, str, true);
                        xmlElement = (XmlElement)elementByUID1;
                        flag = xmlElement != null;
                        if (elementByUID1 == null)
                        {
                            xmlElement = this.CreatePipingSeg(ItemInDoc, xObj, device, comosDDevice);
                        }
                        if (xmlElement != null)
                        {
                            ComosGeoEngine comosGeoEngine = (XObj as IRoDeviceCommon).IBaseRoDevice.Container.GeoEngine(false) as ComosGeoEngine;
                            if (this.m_xvExportVersion != enmXMpLantVersion.XMpLantVersion_3_2_0)
                            {
                                XmlElement xmlElement1 = this.CreateLabel(XObj, ItemInDoc);
                                if (xmlElement1 != null)
                                {
                                    xmlElement1.SetAttribute("ComponentName", "OffPageReference");
                                    this.m_Drawing.AppendChild(xmlElement1);
                                }
                            }
                            else
                            {
                                this.CreateGraphicalElements(comosGeoEngine, ItemInDoc, this.m_xmlDoc, this.m_Drawing, false, XObj as IGeometry);
                            }
                            XmlElement xmlElement2 = this.CreateElementPipeFlowArrow();
                            xmlElement2.AppendChild(this.m_xmlDoc.CreateElement("Extent"));
                            this.CreateCreatePipeFlowArrowGraphics(comosGeoEngine, ItemInDoc, xmlElement2, true, XObj as IGeometry);
                            XMLHelper.CreateExtentByCoordinateChilds(xmlElement2, new CreateElementExtentDelegate(this.CreateElementExtent));
                            xmlElement.AppendChild(xmlElement2);
                            XmlElement xmlElement3 = null;
                            if (comosDDevice == null)
                            {
                                comosDDevice = device;
                            }
                            xmlElement3 = this.CreateElementCenterLine(xObj, XMLHelper.GetParentNodePresentationElement(xmlElement), comosDDevice);
                            xmlElement.AppendChild(xmlElement3);
                            if (xmlElement3 != null)
                            {
                                foreach (XmlElement xmlElement4 in this.CreatePipingNetworkBranch(XObj))
                                {
                                    xmlElement.AppendChild(xmlElement4);
                                    this.AddElementsToReorderCollection(new SvgExport.m_ReorderElements(xmlElement3, xmlElement4));
                                    XmlElement connectedElement = this.GetConnectedElement(comosDDevice);
                                    if (connectedElement == null || !(connectedElement.Name == "PipingComponent"))
                                    {
                                        continue;
                                    }
                                    this.AddElementsToReorderCollection(new SvgExport.m_ReorderElements(xmlElement3, xmlElement4));
                                }
                            }
                        }
                        device = device.owner() as IComosDDevice;
                    }
                    if (!flag)
                    {
                        if (!this.IsComosPipe(device))
                        {
                            elementByUID = this.CreateElementPipingNetworkSystem(this.m_xmlDoc, xObj, null, xmlElement);
                            flag1 = true;
                        }
                        else
                        {
                            str = device.SystemUID();
                            elementByUID = (XmlElement)XMLHelper.GetElementByUID(this.m_xvExportVersion, this.m_xmlDoc.DocumentElement, str, true);
                            if (elementByUID == null)
                            {
                                elementByUID = this.CreateElementPipingNetworkSystem(this.m_xmlDoc, xObj, device, xmlElement);
                                flag1 = true;
                            }
                        }
                        if (elementByUID != null && xmlElement != null)
                        {
                            elementByUID.AppendChild(xmlElement);
                        }
                    }
                    if (flag1 && elementByUID != null)
                    {
                        this.m_Root.AppendChild(elementByUID);
                    }
                }
                else
                {
                    this.m_arlSignalLines.Add(new SvgExport.m_SignalLine(XObj, ItemInDoc));
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
        }

        private ArrayList CreatePipingNetworkBranch(object XObj)
        {
            ArrayList arrayLists = new ArrayList();
            try
            {
                IConnectedRO xObj = XObj as IConnectedRO;
                if (xObj != null)
                {
                    IRoConnectors roConnector = xObj.Connectors();
                    if (roConnector != null && roConnector.Count > 2)
                    {
                        for (int i = 1; i <= roConnector.Count; i++)
                        {
                            IRoConnector roConnector1 = roConnector.Item(i);
                            if (roConnector1 != null && roConnector1.IsDynamic)
                            {
                                XmlElement xmlElement = this.CreateElementPipingNetworkBranch();
                                IComosDConnector comosConnector = roConnector1.ComosConnector;
                                if (comosConnector != null)
                                {
                                    this.CreateNodePersistentID(xmlElement, comosConnector);
                                    this.CreateElementConnectionPointsPipingNetworkBranch(xmlElement, roConnector1);
                                }
                                arrayLists.Add(xmlElement);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
            return arrayLists;
        }

        private XmlElement CreatePipingSeg(Item ItemInDoc, IConnectedRO cro, IComosDDevice dvc, IComosDDevice ddevSegment)
        {
            XmlElement xmlElement;
            try
            {
                XmlElement xmlElement1 = this.CreatePlantItem(dvc, "PipingNetworkSegment", false);
                IRoConnectors roConnector = cro.Connectors();
                this.CreateAttributesForPlantItem(xmlElement1, dvc, enmXMpLantComponentType.NoComponentType, "");
                IGraphicAttributes graphicAttribute = cro as IGraphicAttributes;
                XmlDocument mXmlDoc = this.m_xmlDoc;
                string str = graphicAttribute.Layer.ToString();
                string str1 = graphicAttribute.LineType.ToString();
                string str2 = graphicAttribute.Breadth.ToString();
                int color = graphicAttribute.Color;
                this.CreateElementPresentation(mXmlDoc, xmlElement1, str, str1, str2, color.ToString());
                this.CreateElementExtent(xmlElement1, roConnector.Item(1).x, this.m_ComosDocumentSizeY - roConnector.Item(1).y, roConnector.Item(2).x, this.m_ComosDocumentSizeY - roConnector.Item(2).y);
                this.CreateNodePersistentID(xmlElement1, dvc);
                this.AddGenericAttributesToPlantItem(xmlElement1, dvc, true);
                xmlElement = xmlElement1;
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                xmlElement = null;
            }
            return xmlElement;
        }

        private XmlElement CreatePipingSegFromProcessInstrument(IRoDevice repDefSigLine, IComosDDevice dvcMainSeg, out string sPCSegmentID)
        {
            XmlElement xmlElement;
            sPCSegmentID = string.Empty;
            try
            {
                string attributeID = this.GetAttributeID(false);
                string str = this.GetAttributeID(false);
                string str1 = "";
                IRoConnectors roConnector = (repDefSigLine as IConnectedRO).Connectors();
                for (int i = 1; i <= roConnector.Count; i++)
                {
                    IRoConnector roConnector1 = roConnector.Item(i);
                    if (roConnector1.ConnectedWith != null && roConnector1.ConnectedWith.ComosOwner != null && roConnector1.ConnectedWith.ComosOwner is IComosDDevice)
                    {
                        IComosDDevice comosOwner = (IComosDDevice)roConnector1.ConnectedWith.ComosOwner;
                        if (this.IsComosInstrument(comosOwner))
                        {
                            str1 = comosOwner.SystemUID();
                            break;
                        }
                    }
                }
                XmlElement elementByUID = XMLHelper.GetElementByUID(this.m_xvExportVersion, this.m_xmlDoc.DocumentElement, str1, true);
                string str2 = dvcMainSeg.SystemUID();
                string value = "";
                foreach (XmlElement childNode in XMLHelper.GetElementByUID(this.m_xvExportVersion, this.m_xmlDoc.DocumentElement, str2, true).ChildNodes)
                {
                    if (childNode.Name != "InstrumentConnection")
                    {
                        continue;
                    }
                    value = childNode.Attributes["ID"].Value;
                    break;
                }
                string attributeID1 = this.GetAttributeID(false);
                XmlElement xmlElement1 = this.m_xmlDoc.CreateElement("PipingNetworkSegment");
                xmlElement1.SetAttribute("ID", attributeID1);
                xmlElement1.SetAttribute("ComponentClass", "InstrumentProcessConnection");
                XmlElement xmlElement2 = this.m_xmlDoc.CreateElement("Connection");
                xmlElement2.SetAttribute("FromID", elementByUID.Attributes["ID"].Value);
                xmlElement2.SetAttribute("FromNode", "1");
                xmlElement2.SetAttribute("ToID", value);
                xmlElement2.SetAttribute("ToNode", "1");
                xmlElement1.AppendChild(xmlElement2);
                this.CreateElementCenterLine(repDefSigLine, this.m_xmlDoc, xmlElement1);
                XmlElement xmlElement3 = this.m_xmlDoc.CreateElement("InstrumentLoop");
                xmlElement3.SetAttribute("ID", str);
                XmlElement xmlElement4 = this.m_xmlDoc.CreateElement("Association");
                xmlElement4.SetAttribute("Type", "is a part of");
                xmlElement4.SetAttribute("ItemID", str);
                elementByUID.InsertBefore(xmlElement4, elementByUID.FirstChild);
                XmlElement xmlElement5 = this.m_xmlDoc.CreateElement("Association");
                xmlElement5.SetAttribute("Type", "is a collection including");
                xmlElement5.SetAttribute("ItemID", elementByUID.Attributes["ID"].Value);
                xmlElement3.AppendChild(xmlElement5);
                XmlElement xmlElement6 = this.m_xmlDoc.CreateElement("CONTAINER");
                xmlElement6.AppendChild(xmlElement1);
                xmlElement6.AppendChild(xmlElement3);
                sPCSegmentID = attributeID;
                xmlElement = xmlElement6;
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                xmlElement = null;
            }
            return xmlElement;
        }

        private XmlElement CreatePlantItem(IComosBaseObject baseObj, string strXmlClass, bool ignoreXmlClassSpec = false)
        {
            XmlElement xmlElement;
            try
            {
                bool flag = false;
                if (!ignoreXmlClassSpec)
                {
                    string comosObjectXmlClassByNestedName = this.getComosObjectXmlClassByNestedName(baseObj, SvgExport.COMOS_XMPLANT_CHAPTER_NAME, SvgExport.COMOS_XMPLANT_PLANT_ITEM_NODE_NAME);
                    if (!string.IsNullOrEmpty(comosObjectXmlClassByNestedName))
                    {
                        strXmlClass = comosObjectXmlClassByNestedName;
                    }
                }
                if (this._dictProteusPlantItems.ContainsKey(strXmlClass) || this._dictProteusAnnotationItems.ContainsKey(strXmlClass))
                {
                    flag = true;
                }
                //if (!flag && this.m_Logging != null)
                //{
                //    string str = AppGlobal.ITX("~0768d Object has an unknown XML-Class according to internal XML structure in style of ISO 15926");
                //    this.m_Logging.Warning(string.Concat(new string[] { str, ": ", baseObj.SystemFullName(), " (", strXmlClass, ")" }), baseObj);
                //}
                xmlElement = this.m_xmlDoc.CreateElement(strXmlClass);
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                return null;
            }
            return xmlElement;
        }

        private void CreateSignalLine(object XObj, Item ItemInDoc)
        {
            string str;
            string str1;
            IRoConnectors roConnector;
            object comosOwner;
            object obj;
            try
            {
                if (this.m_xvExportVersion >= enmXMpLantVersion.XMpLantVersion_4_0_1)
                {
                    IConnectedRO xObj = XObj as IConnectedRO;
                    if (xObj != null)
                    {
                        roConnector = xObj.Connectors();
                    }
                    else
                    {
                        roConnector = null;
                    }
                    IRoConnectors roConnector1 = roConnector;
                    if (roConnector1 != null && roConnector1.Count >= 2)
                    {
                        IRoConnector roConnector2 = roConnector1.Item(1);
                        IRoConnector roConnector3 = roConnector1.Item(2);
                        if (roConnector2 != null)
                        {
                            IRoConnector connectedWith = roConnector2.ConnectedWith;
                            if (connectedWith != null)
                            {
                                comosOwner = connectedWith.ComosOwner;
                            }
                            else
                            {
                                comosOwner = null;
                            }
                        }
                        else
                        {
                            comosOwner = null;
                        }
                        if (comosOwner is IComosDDevice)
                        {
                            if (roConnector3 != null)
                            {
                                IRoConnector connectedWith1 = roConnector3.ConnectedWith;
                                if (connectedWith1 != null)
                                {
                                    obj = connectedWith1.ComosOwner;
                                }
                                else
                                {
                                    obj = null;
                                }
                            }
                            else
                            {
                                obj = null;
                            }
                            if (obj is IComosDDevice)
                            {
                                XmlElement xmlElement = this.CreateElementSignalLine(XObj, this.m_xmlDoc);
                                xmlElement.SetAttribute("ID", this.GetAttributeID(false));
                                IComosDDevice comosDDevice = (IComosDDevice)roConnector2.ConnectedWith.ComosOwner;
                                IComosDDevice comosOwner1 = (IComosDDevice)roConnector3.ConnectedWith.ComosOwner;
                                XmlElement elementByUID = XMLHelper.GetElementByUID(this.m_xvExportVersion, this.m_xmlDoc.DocumentElement, comosDDevice.SystemUID(), true);
                                XmlElement elementByUID1 = XMLHelper.GetElementByUID(this.m_xvExportVersion, this.m_xmlDoc.DocumentElement, comosOwner1.SystemUID(), true);
                                string empty = string.Empty;
                                string empty1 = string.Empty;
                                bool flag = false;
                                if (elementByUID != null)
                                {
                                    empty = (elementByUID.Attributes["ID"] != null ? elementByUID.Attributes["ID"].Value : string.Empty);
                                    xmlElement.AppendChild(this.CreateElementAssociation(AssociationType.haslogicalstart, empty));
                                    if (this.IsComosInstrument(comosDDevice))
                                    {
                                        elementByUID.AppendChild(xmlElement);
                                        flag = true;
                                    }
                                }
                                if (elementByUID1 != null)
                                {
                                    empty1 = (elementByUID1.Attributes["ID"] != null ? elementByUID1.Attributes["ID"].Value : string.Empty);
                                    xmlElement.AppendChild(this.CreateElementAssociation(AssociationType.haslogicalend, empty1));
                                    if (this.IsComosInstrument(comosOwner1))
                                    {
                                        elementByUID1.AppendChild(xmlElement);
                                        flag = true;
                                    }
                                }
                                XmlElement xmlElement1 = this.CreateElementConnection(null, empty, -1, empty1, -1);
                                xmlElement.AppendChild(xmlElement1);
                                if (!flag && XMLHelper.CreateElementProcessInstrumentationFunctionGraphicalDummy(this.m_xmlDoc, new GetAttributeId(this.GetAttributeID), ref this.m_xeProcessInstrumentationFunctionGraphicalDummy, "OrphanInformationFlowCollectorDummy", "", "", "", "", "", "", "") && this.m_xeProcessInstrumentationFunctionGraphicalDummy != null)
                                {
                                    this.m_xeProcessInstrumentationFunctionGraphicalDummy.AppendChild(xmlElement);
                                }
                            }
                        }
                    }
                }
                if (this.m_xvExportVersion >= enmXMpLantVersion.XMpLantVersion_3_3_3 && this.m_xvExportVersion <= enmXMpLantVersion.XMpLantVersion_3_6_0)
                {
                    if (!this._signalLinesConnected(XObj))
                    {
                        XmlElement xmlElement2 = this.CreateElementSignalLine(XObj, this.m_xmlDoc);
                        this.m_Root.AppendChild(xmlElement2);
                    }
                    if (!this.IsFunctionLine(XObj))
                    {
                        IConnectedRO connectedRO = XObj as IConnectedRO;
                        IRoConnectors roConnector4 = connectedRO.Connectors();
                        IRoConnector roConnector5 = roConnector4.Item(1);
                        IComosDDevice device = (XObj as IRoDevice).Device;
                        string str2 = "";
                        IComosDConnector comosConnector = null;
                        for (int i = 1; i <= roConnector4.Count; i++)
                        {
                            IRoConnector roConnector6 = roConnector4.Item(i);
                            if (roConnector6.ConnectedWith != null && roConnector6.ConnectedWith.ComosOwner != null && roConnector6.ConnectedWith.ComosOwner is IComosDDevice)
                            {
                                IComosDDevice comosDDevice1 = (IComosDDevice)roConnector6.ConnectedWith.ComosOwner;
                                if (this.IsComosSegment(comosDDevice1))
                                {
                                    str2 = comosDDevice1.SystemUID();
                                    comosConnector = roConnector6.ConnectedWith.ComosConnector;
                                    break;
                                }
                            }
                        }
                        XmlElement elementByUID2 = XMLHelper.GetElementByUID(this.m_xvExportVersion, this.m_xmlDoc.DocumentElement, str2, true);
                        if (elementByUID2 != null)
                        {
                            this.CreateInstrumentConnectionInNS(elementByUID2, comosConnector);
                            StringList<IComosDDevice> stringList = new StringList<IComosDDevice>()
                            {
                                Duplicates = OlDuplicates.DupIgnore,
                                Autosort = true
                            };
                            for (int j = 1; j <= roConnector4.Count; j++)
                            {
                                IRoConnector roConnector7 = roConnector4.Item(j);
                                if (roConnector7.ConnectedWith != null && roConnector7.ConnectedWith.ComosOwner != null)
                                {
                                    string str3 = (roConnector7.ConnectedWith.ComosOwner as IComosDDevice).SystemUID();
                                    stringList.AddObject(str3, roConnector7.ConnectedWith.ComosOwner as IComosDDevice);
                                }
                            }
                            bool flag1 = false;
                            bool flag2 = false;
                            int num = -1;
                            string empty2 = string.Empty;
                            do
                            {
                                if (flag1 || stringList.Count <= 0)
                                {
                                    break;
                                }
                                int num1 = 0;
                                while (num1 < stringList.Count)
                                {
                                    if (stringList[num1].Value == null)
                                    {
                                        num1++;
                                    }
                                    else
                                    {
                                        num = num1;
                                        break;
                                    }
                                }
                                if (num == -1)
                                {
                                    break;
                                }
                                empty2 = string.Empty;
                                if (!this.DDeviceHasBranch(stringList[num].Value, out empty2))
                                {
                                    IComosDDevice[] nextDDevices = this.GetNextDDevices(stringList[num].Value);
                                    if (nextDDevices != null)
                                    {
                                        IComosDDevice[] comosDDeviceArray = nextDDevices;
                                        for (int k = 0; k < (int)comosDDeviceArray.Length; k++)
                                        {
                                            IComosDDevice comosDDevice2 = comosDDeviceArray[k];
                                            stringList.AddObject(comosDDevice2.SystemUID(), comosDDevice2);
                                        }
                                    }
                                    IComosDDevice comosDDevice3 = null;
                                    stringList.setObject(num, ref comosDDevice3);
                                }
                                else
                                {
                                    flag1 = true;
                                    break;
                                }
                            }
                            while (num < stringList.Count - 1);
                            if (flag1 && num > -1)
                            {
                                IComosDDevice value = stringList[num].Value;
                                if (this.DDeviceHasBranchInXML(value, "PipingNetworkSegment", out str) && str != empty2)
                                {
                                    flag2 = true;
                                }
                                if (this.DDeviceHasBranchInXML((IComosDDevice)value.owner(), "PipingNetworkSystem", out str1))
                                {
                                    IRoConnector roConnector8 = roConnector4.Item(2);
                                    if (roConnector5.ConnectedWith != null && roConnector8.ConnectedWith != null)
                                    {
                                        IComosDDevice comosOwner2 = (IComosDDevice)roConnector5.ConnectedWith.ComosOwner;
                                        IComosDDevice comosOwner3 = (IComosDDevice)roConnector8.ConnectedWith.ComosOwner;
                                        string name = comosOwner2.Name;
                                        string name1 = comosOwner3.Name;
                                        REPORTLib.Object container = roConnector5.ConnectedWith.ReportObject.Container;
                                        REPORTLib.Object container1 = roConnector8.ConnectedWith.ReportObject.Container;
                                        string empty3 = string.Empty;
                                        string comosObjectAttributeValueByNestedName = this.getComosObjectAttributeValueByNestedName(comosOwner2, SvgExport.COMOS_XMPLANT_CHAPTER_NAME, SvgExport.COMOS_XMPLANT_PLANT_ITEM_NODE_NAME);
                                        string comosObjectAttributeValueByNestedName1 = this.getComosObjectAttributeValueByNestedName(comosOwner3, SvgExport.COMOS_XMPLANT_CHAPTER_NAME, SvgExport.COMOS_XMPLANT_PLANT_ITEM_NODE_NAME);
                                        XmlElement xmlElement3 = null;
                                        if (comosObjectAttributeValueByNestedName.CompareTo("InstrumentComponent") == 0 || comosObjectAttributeValueByNestedName.CompareTo("ProcessInstrument") == 0)
                                        {
                                            xmlElement3 = this.CreatePipingSeg(ItemInDoc, connectedRO, comosOwner2, comosOwner2);
                                        }
                                        else if (comosObjectAttributeValueByNestedName1.CompareTo("InstrumentComponent") == 0 || comosObjectAttributeValueByNestedName1.CompareTo("ProcessInstrument") == 0)
                                        {
                                            xmlElement3 = this.CreatePipingSegFromProcessInstrument(XObj as IRoDevice, (IComosDDevice)value.owner(), out empty3);
                                        }
                                        if (xmlElement3 != null)
                                        {
                                            if (xmlElement3.Name != "CONTAINER")
                                            {
                                                this.m_Root.AppendChild(xmlElement3);
                                            }
                                            else
                                            {
                                                for (int l = 0; l < xmlElement3.ChildNodes.Count; l++)
                                                {
                                                    this.m_Root.AppendChild(xmlElement3.ChildNodes[l].Clone());
                                                }
                                            }
                                        }
                                        if (empty3.Length > 0)
                                        {
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        IRoConnectors roConnector9 = (XObj as IConnectedRO).Connectors();
                        if (roConnector9.Count >= 2)
                        {
                            IRoConnector roConnector10 = roConnector9.Item(1);
                            IRoConnector roConnector11 = roConnector9.Item(2);
                            if (roConnector10 != null && roConnector10.ConnectedWith != null && roConnector10.ConnectedWith.ComosOwner != null && roConnector10.ConnectedWith.ComosOwner is IComosDDevice && roConnector11 != null && roConnector11.ConnectedWith != null && roConnector11.ConnectedWith.ComosOwner != null && roConnector11.ConnectedWith.ComosOwner is IComosDDevice)
                            {
                                IComosDDevice comosOwner4 = (IComosDDevice)roConnector10.ConnectedWith.ComosOwner;
                                IComosDDevice comosDDevice4 = (IComosDDevice)roConnector11.ConnectedWith.ComosOwner;
                                XmlElement elementByUID3 = XMLHelper.GetElementByUID(this.m_xvExportVersion, this.m_xmlDoc.DocumentElement, comosOwner4.SystemUID(), true);
                                XmlElement elementByUID4 = XMLHelper.GetElementByUID(this.m_xvExportVersion, this.m_xmlDoc.DocumentElement, comosDDevice4.SystemUID(), true);
                                if (elementByUID3 != null && elementByUID4 != null)
                                {
                                    XmlElement xmlElement4 = this.CreateElementSignalLine(XObj, this.m_xmlDoc);
                                    this.m_Root.AppendChild(xmlElement4);
                                    string str4 = (elementByUID3.Attributes["ID"] != null ? elementByUID3.Attributes["ID"].Value : "");
                                    XmlElement xmlElement5 = this.CreateElementConnection(null, str4, -1, (elementByUID4.Attributes["ID"] != null ? elementByUID4.Attributes["ID"].Value : ""), -1);
                                    xmlElement4.AppendChild(xmlElement5);
                                }
                            }
                        }
                    }
                }
                else if (this.m_xvExportVersion == enmXMpLantVersion.XMpLantVersion_3_2_0)
                {
                    XmlElement xmlElement6 = this.CreateElementSignalLine(XObj, this.m_xmlDoc);
                    this.m_Root.AppendChild(xmlElement6);
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
        }

        private void CreateText(XmlElement ParentNode, Item ItemInDoc, NTextElement GeTxt)
        {
            double drawRectWidth;
            try
            {
                double num = 0;
                double num1 = 0;
                double num2 = 1;
                double num3 = 1;
                string geTxt = "";
                string name = "";
                string str = "";
                string str1 = "";
                string str2 = "";
                string str3 = "0";
                string str4 = "0";
                string str5 = "";
                string str6 = "";
                IText itemInDoc = ItemInDoc as IText;
                if (GeTxt == null)
                {
                    geTxt = itemInDoc.text;
                    name = itemInDoc.Font.Name;
                    str = this.AlignTrans(itemInDoc.Align, ref num, ref num1, ref num2, ref num3);
                    drawRectWidth = itemInDoc.GetDrawRectWidth();
                    str1 = drawRectWidth.ToString().Replace(",", ".");
                    drawRectWidth = (double)((double)itemInDoc.Font.Size) * this.dblFactorDTPmm;
                    str2 = drawRectWidth.ToString().Replace(",", ".");
                    drawRectWidth = itemInDoc.Angle;
                    str3 = drawRectWidth.ToString().Replace(",", ".");
                    str4 = "0";
                }
                else
                {
                    if (!this.ContainsEvaluableText(GeTxt.txt))
                    {
                        geTxt = GeTxt.txt;
                    }
                    else
                    {
                        this.CreateDependantAttribute(ItemInDoc, GeTxt, out str5, out str6);
                        geTxt = GeTxt.EvaluatedText;
                    }
                    name = GeTxt.Font.Name;
                    str = this.SoftAlignTrans((double)GeTxt.JustX, (double)GeTxt.JustY, ref num, ref num1, ref num2, ref num3);
                    drawRectWidth = GeTxt.Umreck.XSize;
                    str1 = drawRectWidth.ToString().Replace(",", ".");
                    drawRectWidth = (double)GeTxt.Font.Height * this.dblFactorDTPmm;
                    str2 = drawRectWidth.ToString().Replace(",", ".");
                    float geoTextAngle = this.GetGeoTextAngle(GeTxt);
                    str3 = geoTextAngle.ToString().Replace(",", ".");
                    str4 = "0";
                }
                if (!string.IsNullOrEmpty(geTxt.Trim()) || !string.IsNullOrEmpty(str5.Trim()))
                {
                    XmlElement xmlElement = this.CreateElementText(geTxt, name, str, str1, str2, str3, str4, str5, str6);
                    ParentNode.AppendChild(xmlElement);
                    XmlDocument mXmlDoc = this.m_xmlDoc;
                    string str7 = ItemInDoc.Layer.ToString();
                    string str8 = ItemInDoc.LineType.ToString();
                    string str9 = ItemInDoc.Width.ToString();
                    int color = ItemInDoc.Color;
                    this.CreateElementPresentation(mXmlDoc, xmlElement, str7, str8, str9, color.ToString());
                    if (GeTxt == null)
                    {
                        this.CreateElementExtent(xmlElement, ItemInDoc.x1 + num * itemInDoc.GetDrawRectWidth(), this.m_ComosDocumentSizeY - (ItemInDoc.y1 + num1 * itemInDoc.GetDrawRectHeight()), ItemInDoc.x1 + num2 * itemInDoc.GetDrawRectWidth(), this.m_ComosDocumentSizeY - (ItemInDoc.y1 + num3 * itemInDoc.GetDrawRectHeight()));
                        this.CreateElementPosition(xmlElement, ItemInDoc.x1, this.m_ComosDocumentSizeY - ItemInDoc.y1, itemInDoc.Angle / 180 * this.PI);
                    }
                    else if (ItemInDoc != null)
                    {
                        this.CreateElementExtent(xmlElement, ItemInDoc.x1 + GeTxt.Umreck.left, this.m_ComosDocumentSizeY - ItemInDoc.y1 + GeTxt.Umreck.bottom, ItemInDoc.x1 + GeTxt.Umreck.right, this.m_ComosDocumentSizeY - ItemInDoc.y1 + GeTxt.Umreck.top);
                        this.CreateElementPosition(xmlElement, ItemInDoc.x1 + GeTxt.point.x, this.m_ComosDocumentSizeY - ItemInDoc.y1 + GeTxt.point.y, 0);
                    }
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
        }

        private void CreateTextElements(ComosGeoEngine Ge, Item ItemInDoc, XmlElement parentNode)
        {
            int displayElementCount = Ge.DisplayElementCount;
            this.GetAllHatch(Ge);
            ArrayList arrayLists = new ArrayList();
            for (int i = 0; i <= displayElementCount - 1; i++)
            {
                NDisplayElement displayElement = Ge.DisplayElement[i];
                if (displayElement.Geometrie.Type == 5)
                {
                    this.CreateText(parentNode, ItemInDoc, displayElement.Geometrie as NTextElement);
                }
            }
        }

        private void CreateXMpLantFixedAttributes(XmlElement parentNode, IComosBaseObject baseObject)
        {
            try
            {
                IComosDCollection comosDCollection = null;
                if (baseObject is IComosDCDevice)
                {
                    comosDCollection = ((IComosDCDevice)baseObject).Specifications();
                }
                if (baseObject is IComosDDevice)
                {
                    comosDCollection = ((IComosDDevice)baseObject).Specifications();
                }
                IComosDSpecification spec = this.GetSpec(comosDCollection, SvgExport.COMOS_XMPLANT_CHAPTER_NAME, SvgExport.COMOS_XMPLANT_FIXED_XMPLANT_ATTRIBUTES);
                if (spec != null)
                {
                    IComosDSpecification comosDSpecification = spec.Specifications().Item(SvgExport.COMOS_XMPLANT_XMPLANT_ATTRIBUTE_NAME) as IComosDSpecification;
                    IComosDSpecification comosDSpecification1 = spec.Specifications().Item(SvgExport.COMOS_XMPLANT_COMOS_NESTED_NAME) as IComosDSpecification;
                    if (comosDSpecification != null && comosDSpecification1 != null)
                    {
                        int num = comosDSpecification.MaxLinkedXValueCount();
                        for (int i = 0; i <= num - 1; i++)
                        {
                            string str = comosDSpecification.GetXValue(i).Trim();
                            string str1 = comosDSpecification1.GetXValue(i).Trim();
                            if (str != "" && str1 != "")
                            {
                                IComosDSpecification specByNestedName = this.GetSpecByNestedName(comosDCollection, str1);
                                if (specByNestedName != null)
                                {
                                    string str2 = specByNestedName.DisplayValue().Trim();
                                    XmlElement xmlElement = this.m_xmlDoc.CreateElement(str);
                                    xmlElement.InnerText = str2;
                                    parentNode.AppendChild(xmlElement);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
        }

        private bool DDeviceHasBranch(IComosDDevice aSearchDDev, out string branchSysUID)
        {
            bool flag = false;
            branchSysUID = string.Empty;
            if (aSearchDDev != null)
            {
                string comosObjectAttributeValueByNestedName = this.getComosObjectAttributeValueByNestedName(aSearchDDev, SvgExport.COMOS_XMPLANT_CHAPTER_NAME, SvgExport.COMOS_XMPLANT_PLANT_ITEM_NODE_NAME);
                if (comosObjectAttributeValueByNestedName.Length != 0)
                {
                    branchSysUID = aSearchDDev.SystemUID();
                }
                else
                {
                    if (aSearchDDev.owner() is IComosDDevice)
                    {
                        comosObjectAttributeValueByNestedName = this.getComosObjectAttributeValueByNestedName(aSearchDDev.owner() as IComosBaseObject, SvgExport.COMOS_XMPLANT_CHAPTER_NAME, SvgExport.COMOS_XMPLANT_PLANT_ITEM_NODE_NAME);
                    }
                    if (comosObjectAttributeValueByNestedName.Length != 0)
                    {
                        branchSysUID = ((IComosDDevice)aSearchDDev.owner()).SystemUID();
                    }
                    else
                    {
                        IComosDCDevice comosDCDevice = aSearchDDev.owner() as IComosDCDevice;
                        if (comosDCDevice != null)
                        {
                            comosObjectAttributeValueByNestedName = this.getComosObjectAttributeValueByNestedName(comosDCDevice, SvgExport.COMOS_XMPLANT_CHAPTER_NAME, SvgExport.COMOS_XMPLANT_PLANT_ITEM_NODE_NAME);
                        }
                    }
                }
                if (comosObjectAttributeValueByNestedName == "PipingNetworkSegment")
                {
                    flag = true;
                }
            }
            return flag;
        }

        private bool DDeviceHasBranchInXML(IComosDDevice aSearchDDev, string xmlElementName, out string branchSysUID)
        {
            bool flag = false;
            branchSysUID = string.Empty;
            string str = aSearchDDev.SystemUID();
            aSearchDDev.FullName();
            XmlElement elementByUID = XMLHelper.GetElementByUID(this.m_xvExportVersion, this.m_xmlDoc.DocumentElement, str, true);
            if (elementByUID != null)
            {
                XmlNode parentNode = elementByUID.ParentNode;
                if (parentNode != null && parentNode.Name == xmlElementName)
                {
                    flag = true;
                    int num = 0;
                    while (num < parentNode.ChildNodes.Count)
                    {
                        if (parentNode.ChildNodes.Item(num).Name != "PersistentID")
                        {
                            num++;
                        }
                        else
                        {
                            branchSysUID = parentNode.ChildNodes.Item(num).Attributes["Identifier"].Value;
                            break;
                        }
                    }
                }
            }
            return flag;
        }

        private bool DexpiVerify()
        {
            string str = "";
            string str1 = "";
            string str2 = "";
            string dexpiVerificationFilename = this.GetDexpiVerificationFilename();
            bool flag = true;
            if (!File.Exists(dexpiVerificationFilename))
            {
                this.m_Logging.Information(string.Concat(AppGlobal.ITX("~07401 No DEXPI information model file found"), ": ", dexpiVerificationFilename));
                flag = false;
            }
            else
            {
                Verificator verificator = new Verificator();
                verificator.VerifyFile(this.m_strSaveFileName, dexpiVerificationFilename);
                this.m_Logging.WaitMsg(AppGlobal.ITX("~073fd DEXPI verification"), verificator.resultLogger.Results.Count);
                int num = 1;
                this.m_Logging.Information(string.Concat(new string[] { "XML file: '", this.m_strSaveFileName, "'", Environment.NewLine, "DEXPI file: ", dexpiVerificationFilename, "'" }));
                foreach (ResultItem result in verificator.resultLogger.Results)
                {
                    int num1 = num;
                    num = num1 + 1;
                    this.m_Logging.WaitProgress = (long)num1;
                    if (result.Classification == ResultItem.Classifications.Info)
                    {
                        str = string.Concat(str, this.GenerateDexpiVerificationMessage(result), Environment.NewLine);
                    }
                    if (result.Classification == ResultItem.Classifications.Warning)
                    {
                        str1 = string.Concat(str1, this.GenerateDexpiVerificationMessage(result), Environment.NewLine);
                    }
                    if (result.Classification != ResultItem.Classifications.Error)
                    {
                        continue;
                    }
                    str2 = string.Concat(str2, this.GenerateDexpiVerificationMessage(result), Environment.NewLine);
                }
                if (!string.IsNullOrEmpty(str))
                {
                    this.m_Logging.Warning(str, null);
                }
                if (!string.IsNullOrEmpty(str1))
                {
                    this.m_Logging.Warning(str1, null);
                    flag = false;
                }
                if (!string.IsNullOrEmpty(str2))
                {
                    this.m_Logging.Error(str2);
                    flag = false;
                }
                this.m_Logging.Information(string.Concat(AppGlobal.ITX("~073fd DEXPI verification"), " ", AppGlobal.ITX("~03744 Done")));
            }
            if (flag)
            {
                this.m_Logging.Success(AppGlobal.ITX("~06044 Successful"));
            }
            this.m_Logging.StopCurrentTask();
            return flag;
        }

        private void DistanceFromLine(double cx, double cy, double ax, double ay, double bx, double by, ref double distanceSegment, ref double distanceLine)
        {
            double num = (bx - ax) * (bx - ax) + (by - ay) * (by - ay);
            double num1 = ((cx - ax) * (bx - ax) + (cy - ay) * (by - ay)) / num;
            double num2 = ((ay - cy) * (bx - ax) - (ax - cx) * (by - ay)) / num;
            distanceLine = Math.Abs(num2) * Math.Sqrt(num);
            if (num1 >= 0 && num1 <= 1)
            {
                distanceSegment = distanceLine;
                return;
            }
            double num3 = (cx - ax) * (cx - ax) + (cy - ay) * (cy - ay);
            double num4 = (cx - bx) * (cx - bx) + (cy - by) * (cy - by);
            if (num3 < num4)
            {
                distanceSegment = Math.Sqrt(num3);
                return;
            }
            distanceSegment = Math.Sqrt(num4);
        }

        private Dictionary<ulong, Item> DocItems2Dictionary()
        {
            Dictionary<ulong, Item> nums;
            try
            {
                Dictionary<ulong, Item> nums1 = new Dictionary<ulong, Item>();
                Items item = this.m_rDoc.Items();
                Item item1 = null;
                while (item.NextItem(out item1) != 0)
                {
                    nums1.Add(item1.LID, item1);
                }
                nums = nums1;
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                return null;
            }
            return nums;
        }

        ~SvgExport()
        {
            this.m_ReportExport.FoundReportLine -= new ReportAnalyzer.FoundReportLineHandler(this.ReportExport_FoundReportLine);
            this.m_ReportExport.FoundReportArc -= new ReportAnalyzer.FoundReportArcHandler(this.ReportExport_FoundReportArc);
            this.m_ReportExport.FoundReportText -= new ReportAnalyzer.FoundReportTextHandler(this.ReportExport_FoundReportText);
        }

        private Item FindReportLibItemForDevice(IComosDDevice DDev)
        {
            Item item;
            try
            {
                Item item1 = null;
                Items item2 = this.m_rDoc.Items();
                item2.FilterIIDs[0] = "{77DCABF2-7FBD-11D2-A8E1-00609775B2A2}";
                while (item2.NextItem(out item1) != 0)
                {
                    if ((LateBinding.LateGet(item1, null, "XObj", null, null, null) as IRoDevice).Device != DDev)
                    {
                        continue;
                    }
                    item = item1;
                    return item;
                }
                item = null;
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                item = null;
            }
            return item;
        }

        private string GenerateDexpiVerificationMessage(ResultItem ri)
        {
            return string.Concat(new object[] { "(", ri.LineNumber, ") ", ri.Message });
        }

        private string GenerateSchemaValidationMessage(ValidationEventArgs vea)
        {
            return string.Concat(new object[] { "(", vea.Exception.LineNumber, ",", vea.Exception.LinePosition, ") ", vea.Message });
        }

        private ArrayList GetAllDocHatch(Dictionary<ulong, Item> DocItems)
        {
            ArrayList arrayLists;
            try
            {
                ArrayList arrayLists1 = new ArrayList();
                foreach (KeyValuePair<ulong, Item> docItem in DocItems)
                {
                    Item value = docItem.Value;
                    if (value.SystemTypeName != "Path")
                    {
                        continue;
                    }
                    double[] itemHatch = this.GetItemHatch(value);
                    if (itemHatch == null)
                    {
                        continue;
                    }
                    arrayLists1.Add(itemHatch);
                }
                arrayLists = arrayLists1;
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                return new ArrayList();
            }
            return arrayLists;
        }

        private ArrayList GetAllDocHatch(Items DocItems)
        {
            Item item;
            ArrayList arrayLists;
            try
            {
                ArrayList arrayLists1 = new ArrayList();
                while (DocItems.NextItem(out item) != 0)
                {
                    if (item.SystemTypeName != "Path")
                    {
                        continue;
                    }
                    double[] itemHatch = this.GetItemHatch(item);
                    if (itemHatch == null)
                    {
                        continue;
                    }
                    arrayLists1.Add(itemHatch);
                }
                arrayLists = arrayLists1;
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                return new ArrayList();
            }
            return arrayLists;
        }

        private ArrayList GetAllHatch(ComosGeoEngine Ge)
        {
            ArrayList arrayLists;
            try
            {
                ArrayList arrayLists1 = new ArrayList();
                int displayElementCount = Ge.DisplayElementCount;
                for (int i = 0; i <= displayElementCount - 1; i++)
                {
                    NGeoElement geometrie = Ge.DisplayElement[i].Geometrie;
                    if (geometrie.Type == 6)
                    {
                        NPath nPath = geometrie as NPath;
                        for (int j = 0; j < nPath.ItemCount(); j++)
                        {
                            double[] umreck = new double[4];
                            NGeoElement nGeoElement = nPath.Item(j).Geometrie;
                            umreck[0] = nGeoElement.Umreck.left;
                            umreck[1] = nGeoElement.Umreck.bottom;
                            umreck[2] = nGeoElement.Umreck.right;
                            umreck[3] = nGeoElement.Umreck.top;
                            arrayLists1.Add(umreck);
                        }
                    }
                }
                arrayLists = arrayLists1;
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                arrayLists = null;
            }
            return arrayLists;
        }

        private string GetAttributeID(bool IsShapeCatalogueItem)
        {
            string str;
            try
            {
                string str1 = "";
                if (!IsShapeCatalogueItem)
                {
                    str1 = string.Concat("XMP_", this.m_xmp.ToString());
                    this.m_xmp++;
                }
                else
                {
                    str1 = string.Concat("XMC_", this.m_xmc.ToString());
                    this.m_xmc++;
                }
                str = str1;
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                str = "";
            }
            return str;
        }

        private void GetAttributeIsoNameAndUri(IComosDStandardTable stdTable, string strAttributeNestedName, string AttributeDescription, SvgExport.enmXValueType ValueType, out string IsoName, out string IsoUri, out bool IgnoreAttributeFlag)
        {
            IsoName = AttributeDescription;
            IsoUri = "";
            IgnoreAttributeFlag = false;
            try
            {
                if (stdTable != null)
                {
                    int num = 1;
                    while (num <= stdTable.StandardValues().Count())
                    {
                        IComosDStandardValue comosDStandardValue = (IComosDStandardValue)stdTable.StandardValues().Item(num);
                        string xValue = comosDStandardValue.GetXValue(0);
                        string str = comosDStandardValue.GetXValue(6);
                        string xValue1 = comosDStandardValue.GetXValue(1);
                        int num1 = -1;
                        if (!string.IsNullOrEmpty(xValue1))
                        {
                            int.TryParse(xValue1, out num1);
                        }
                        if (!(xValue == strAttributeNestedName) || num1 != ValueType.GetHashCode())
                        {
                            num++;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(str) && str == "1")
                            {
                                IgnoreAttributeFlag = true;
                            }
                            if (!string.IsNullOrEmpty(comosDStandardValue.GetXValue(3)))
                            {
                                IsoName = comosDStandardValue.GetXValue(3);
                            }
                            IsoUri = comosDStandardValue.GetXValue(5);
                            break;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
        }

        private void GetAttributeUnitIsoNameAndUri(string strUnitNestedName, ref string IsoName, out string IsoUri)
        {
            IsoUri = "";
            if (this.m_stUnitMapping == null || string.IsNullOrEmpty(strUnitNestedName))
            {
                return;
            }
            for (int i = 1; i <= this.m_stUnitMapping.StandardValues().Count(); i++)
            {
                IComosDStandardValue comosDStandardValue = (IComosDStandardValue)this.m_stUnitMapping.StandardValues().Item(i);
                if (strUnitNestedName == comosDStandardValue.GetXValue(2))
                {
                    IsoName = comosDStandardValue.GetXValue(1);
                    IsoUri = comosDStandardValue.GetXValue(3);
                    return;
                }
            }
        }

        private string GetComosChapterName(IComosDSpecification comosChapter)
        {
            return this.RemoveSymbol(comosChapter.Description).Replace(" ", "");
        }

        private string getComosObjectAttributeValueByNestedName(IComosBaseObject comosBaseObject, string chapterName, string attributeName)
        {
            string empty = string.Empty;
            if (comosBaseObject != null)
            {
                IComosDSpecification comosDSpecification = comosBaseObject.spec(string.Concat(chapterName, ".", attributeName));
                if (comosDSpecification != null)
                {
                    empty = comosDSpecification.DisplayValue();
                }
            }
            return empty;
        }

        private string getComosObjectXmlClassByNestedName(IComosBaseObject comosBaseObject, string chapterName, string attributeName)
        {
            IComosDSpecification comosDSpecification;
            string empty = string.Empty;
            if (comosBaseObject != null)
            {
                comosDSpecification = comosBaseObject.spec(string.Concat(chapterName, ".", attributeName));
            }
            else
            {
                comosDSpecification = null;
            }
            IComosDSpecification comosDSpecification1 = comosDSpecification;
            if (comosDSpecification1 != null)
            {
                empty = comosDSpecification1.GetXValueFromStandardValueDescription(comosDSpecification1.DisplayValue(), 0);
            }
            return empty;
        }

        private XmlElement GetConnectedElement(IComosDDevice DDevCenterLine)
        {
            try
            {
                bool flag = true;
                IComosDOwnCollection comosDOwnCollection = DDevCenterLine.OwnerCollection() as IComosDOwnCollection;
                int num = comosDOwnCollection.ItemIndex(DDevCenterLine) + 1;
                while (num <= comosDOwnCollection.Count())
                {
                    if (!this.IsComosSegment(comosDOwnCollection.Item(num) as IComosDDevice))
                    {
                        num++;
                    }
                    else
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    IComosDConnector connectorByFlow = this.GetConnectorByFlow(DDevCenterLine, SvgExport.enmDeviceConnector.dcDeviceOutputConnector);
                    if (connectorByFlow != null)
                    {
                        IComosDConnector comosDConnector = connectorByFlow.ConnectedWith();
                        if (comosDConnector != null)
                        {
                            IComosDDevice comosDDevice = comosDConnector.owner() as IComosDDevice;
                            if (comosDDevice != null)
                            {
                                string str = comosDDevice.SystemUID();
                                XmlElement elementByUID = XMLHelper.GetElementByUID(this.m_xvExportVersion, this.m_xmlDoc.DocumentElement, str, true);
                                if (elementByUID != null)
                                {
                                    return elementByUID;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
            return null;
        }

        private int GetConnectionPointsNodeCount(XmlElement xeParent)
        {
            int count;
            try
            {
                count = xeParent.GetElementsByTagName("Node").Count - 1;
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                count = -1;
            }
            return count;
        }

        private IComosDConnector GetConnectorByFlow(IComosDDevice DDev, SvgExport.enmDeviceConnector DeviceConnector)
        {
            IComosDConnector connectorByName;
            try
            {
                connectorByName = this.GetConnectorByName(DDev, this.GetConnectorNameByFlow(DDev, DeviceConnector));
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                connectorByName = null;
            }
            return connectorByName;
        }

        private IComosDConnector GetConnectorByName(IComosDDevice DDev, string ConName)
        {
            IComosDConnector comosDConnector;
            try
            {
                IComosDConnector comosDConnector1 = null;
                if (DDev != null)
                {
                    int num = 1;
                    while (num <= DDev.Connectors().Count())
                    {
                        comosDConnector1 = (IComosDConnector)DDev.Connectors().Item(num);
                        if (comosDConnector1.Name != ConName)
                        {
                            num++;
                        }
                        else
                        {
                            comosDConnector = comosDConnector1;
                            return comosDConnector;
                        }
                    }
                }
                comosDConnector = null;
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                comosDConnector = null;
            }
            return comosDConnector;
        }

        private string GetConnectorNameByFlow(IComosDDevice DDev, SvgExport.enmDeviceConnector DeviceConnector)
        {
            string name;
            try
            {
                if (DDev.Connectors() == null || DDev.Connectors().Count() != 1)
                {
                    string cOMOSCONNECTORSTANDARDINPUT = SvgExport.COMOS_CONNECTOR_STANDARD_INPUT;
                    string cOMOSCONNECTORSTANDARDOUTPUT = SvgExport.COMOS_CONNECTOR_STANDARD_OUTPUT;
                    IComosDSpecification spec = this.GetSpec(DDev.Specifications(), SvgExport.COMOS_SYSTEM_CHAPTER_NAME, SvgExport.COMOS_SYSTEM_BEHAVIOR_MULTI_FLOW_DIR);
                    if (spec != null)
                    {
                        if (spec.DisplayValue().Split(new char[] { '>' }).GetUpperBound(0) > 0)
                        {
                            cOMOSCONNECTORSTANDARDINPUT = spec.DisplayValue().Split(new char[] { '>' })[0];
                            cOMOSCONNECTORSTANDARDOUTPUT = spec.DisplayValue().Split(new char[] { '>' })[1];
                        }
                    }
                    if (DeviceConnector == SvgExport.enmDeviceConnector.dcDeviceInputConnector)
                    {
                        name = cOMOSCONNECTORSTANDARDINPUT;
                    }
                    else if (DeviceConnector == SvgExport.enmDeviceConnector.dcDeviceOutputConnector)
                    {
                        name = cOMOSCONNECTORSTANDARDOUTPUT;
                    }
                    else
                    {
                        name = null;
                    }
                }
                else
                {
                    name = (DDev.Connectors().Item(1) as IComosDConnector).Name;
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                name = null;
            }
            return name;
        }

        private ArrayList GetConnectors(ComosGeoEngine Ge, Item ItemInDoc, XmlElement parentNode, bool IsPlantItemInstance)
        {
            int displayElementCount = Ge.DisplayElementCount;
            this.GetAllHatch(Ge);
            ArrayList arrayLists = new ArrayList();
            for (int i = 0; i <= displayElementCount - 1; i++)
            {
                NDisplayElement displayElement = Ge.DisplayElement[i];
                if (displayElement.Geometrie.Type == 5)
                {
                    NTextElement geometrie = displayElement.Geometrie as NTextElement;
                    string str = geometrie.txt;
                    if (this.isConnector(displayElement.Header.Class))
                    {
                        arrayLists.Add(geometrie);
                    }
                    if (IsPlantItemInstance)
                    {
                        this.CreateText(parentNode, ItemInDoc, geometrie);
                    }
                }
            }
            return arrayLists;
        }

        private string GetDexpiVerificationFilename()
        {
            if (!string.IsNullOrEmpty(this.ValidationFilesPath) && Directory.Exists(this.ValidationFilesPath))
            {
                return string.Concat(this.ValidationFilesPath, "\\dexpi_information_model.n3");
            }
            return string.Concat(System.IO.Path.GetDirectoryName(this.m_strSaveFileName), "\\dexpi_information_model.n3");
        }

        private ArrayList GetEffectiveExportAttributes(IComosBaseObject comosBaseObject)
        {
            ArrayList arrayLists;
            try
            {
                ArrayList arrayLists1 = new ArrayList();
                IComosDCollection comosDCollection = null;
                if (comosBaseObject is IComosDDevice)
                {
                    comosDCollection = ((IComosDDevice)comosBaseObject).Specifications();
                }
                if (comosBaseObject is IComosDCDevice)
                {
                    comosDCollection = ((IComosDCDevice)comosBaseObject).Specifications();
                }
                if (comosDCollection != null)
                {
                    IComosDSpecification spec = this.GetSpec(comosDCollection, SvgExport.COMOS_XMPLANT_CHAPTER_NAME, SvgExport.COMOS_XMPLANT_EXPORT_LISTED_ATTRIBUTES);
                    if (spec == null || !(spec.@value == "1"))
                    {
                        IComosDSpecification comosDSpecification = null;
                        IComosDSpecification comosDSpecification1 = null;
                        for (int i = 1; i <= comosDCollection.Count(); i++)
                        {
                            comosDSpecification = comosDCollection.Item(i) as IComosDSpecification;
                            for (int j = 1; j <= comosDSpecification.Specifications().Count(); j++)
                            {
                                comosDSpecification1 = (IComosDSpecification)comosDSpecification.Specifications().Item(j);
                                if (comosDSpecification1.DisplayValue().Trim() != "")
                                {
                                    arrayLists1.Add(comosDSpecification1);
                                }
                            }
                        }
                    }
                    else
                    {
                        IComosDSpecification comosDSpecification2 = this.GetSpec(comosDCollection, SvgExport.COMOS_XMPLANT_CHAPTER_NAME, SvgExport.COMOS_XMPLANT_GENERIC_ATTRIBUTES_LIST).Specifications().Item(SvgExport.COMOS_XMPLANT_COMOS_NESTED_NAMES) as IComosDSpecification;
                        if (comosDSpecification2 != null)
                        {
                            int num = comosDSpecification2.MaxLinkedXValueCount();
                            for (int k = 0; k <= num - 1; k++)
                            {
                                IComosDSpecification specByNestedName = this.GetSpecByNestedName(comosDCollection, comosDSpecification2.GetXValue(k));
                                if (specByNestedName != null)
                                {
                                    arrayLists1.Add(specByNestedName);
                                }
                            }
                        }
                    }
                    arrayLists = arrayLists1;
                }
                else
                {
                    arrayLists = null;
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                arrayLists = null;
            }
            return arrayLists;
        }

        private XmlElement GetElementInstrumentLoop(IComosDDevice DDev, out bool ElementExists)
        {
            string str;
            XmlElement xmlElement;
            ElementExists = false;
            try
            {
                XmlElement elementByUID = null;
                IComosDDevice comosDDevice = DDev.owner() as IComosDDevice;
                str = (this.m_xvExportVersion > enmXMpLantVersion.XMpLantVersion_3_6_0 ? "InstrumentationLoopFunction" : "InstrumentLoop");
                if (comosDDevice == null || !(comosDDevice.Class == "P") || !(comosDDevice.DetailClass == ""))
                {
                    elementByUID = this.m_xmlDoc.CreateElement(str);
                    this.CreateAttributesForPlantItem(elementByUID, null, enmXMpLantComponentType.NoComponentType, "");
                    elementByUID.SetAttribute("StockNumber", "InstrumentLoopDummy");
                    xmlElement = elementByUID;
                }
                else
                {
                    string str1 = comosDDevice.SystemUID();
                    elementByUID = XMLHelper.GetElementByUID(this.m_xvExportVersion, this.m_xmlDoc.DocumentElement, str1, true);
                    if (elementByUID == null || !(elementByUID.Name == str))
                    {
                        elementByUID = this.CreatePlantItem(comosDDevice, str, false);
                        this.CreateAttributesForPlantItem(elementByUID, comosDDevice, enmXMpLantComponentType.NoComponentType, "");
                        this.CreateElementPresentation(this.m_xmlDoc, elementByUID, "0", "0", "0", "0");
                        this.CreateElementExtent(elementByUID, 0, 0, 0, 0);
                        if (this.m_xvExportVersion >= enmXMpLantVersion.XMpLantVersion_4_0_1)
                        {
                            this.CreateNodePersistentID(elementByUID, comosDDevice);
                            elementByUID.AppendChild(this.CreateGenericAttributesNameDescription(comosDDevice));
                            this.AddGenericAttributesAllComosSpecs(elementByUID, comosDDevice, null);
                        }
                        xmlElement = elementByUID;
                    }
                    else
                    {
                        ElementExists = true;
                        xmlElement = elementByUID;
                    }
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                xmlElement = null;
            }
            return xmlElement;
        }

        private string GetGenericAttributeFormat(string strSpecificationType)
        {
            string str;
            try
            {
                if (strSpecificationType == "N")
                {
                    str = "double";
                }
                else
                {
                    if (!(strSpecificationType == "C") && !(strSpecificationType == "A") && !(strSpecificationType == "D") && !(strSpecificationType == "S"))
                    {
                    }
                    str = "string";
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                return "";
            }
            return str;
        }

        private float GetGeoTextAngle(NTextElement GeTxt)
        {
            float single;
            try
            {
                float angle = GeTxt.Font.Angle;
                if (this.m_rDoc.TextRotationMode == 0)
                {
                    if (angle < 0f)
                    {
                        angle += 360f;
                    }
                    else if (angle > 91f && angle < 271f)
                    {
                        angle = GeTxt.Font.Angle + 180f;
                        if (angle >= 360f)
                        {
                            angle -= 360f;
                        }
                    }
                }
                single = angle;
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                single = 0f;
            }
            return single;
        }

        internal object getGlobals(object globals, string glName)
        {
            object obj = null;
            try
            {
                IPLTVarstorage pLTVarstorage = globals as IPLTVarstorage;
                if (pLTVarstorage != null)
                {
                    int num = 1;
                    int num1 = pLTVarstorage.Count();
                    while (num <= num1)
                    {
                        pLTVarstorage.ItemKey(num);
                        if (pLTVarstorage.ItemKey(num).ToUpper() == glName.ToUpper())
                        {
                            obj = pLTVarstorage.Item(num);
                        }
                        num++;
                    }
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
            return obj;
        }

        private IComosDStandardTable GetIsoMappingTable(IComosBaseObject comosBaseObject)
        {
            IComosDStandardTable linkObject = null;
            try
            {
                IComosDCollection comosDCollection = null;
                if (comosBaseObject is IComosDDevice)
                {
                    comosDCollection = ((IComosDDevice)comosBaseObject).Specifications();
                }
                if (comosBaseObject is IComosDCDevice)
                {
                    comosDCollection = ((IComosDCDevice)comosBaseObject).Specifications();
                }
                IComosDSpecification spec = this.GetSpec(comosDCollection, SvgExport.COMOS_XMPLANT_CHAPTER_NAME, SvgExport.COMOS_XMPLANT_ISO_MAPPING_TABLE);
                if (spec != null)
                {
                    linkObject = spec.LinkObject as IComosDStandardTable;
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
            return linkObject;
        }

        private double[] GetItemHatch(Item ItemInDoc)
        {
            try
            {
                if (ItemInDoc.SystemTypeName == "Path")
                {
                    return new double[] { ItemInDoc.x1, ItemInDoc.y1, ItemInDoc.x2, ItemInDoc.y2 };
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
            return null;
        }

        public ISvgExport GetIWspExport()
        {
            ISvgExport wspExport;
            try
            {
                wspExport = this;
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                wspExport = null;
            }
            return wspExport;
        }

        private ArrayList GetListedExportAttributes(IComosBaseObject comosBaseObject)
        {
            ArrayList arrayLists;
            ArrayList arrayLists1 = new ArrayList();
            try
            {
                IComosDCollection comosDCollection = null;
                if (comosBaseObject is IComosDDevice)
                {
                    comosDCollection = ((IComosDDevice)comosBaseObject).Specifications();
                }
                if (comosBaseObject is IComosDCDevice)
                {
                    comosDCollection = ((IComosDCDevice)comosBaseObject).Specifications();
                }
                if (comosDCollection != null)
                {
                    IComosDSpecification spec = this.GetSpec(comosDCollection, SvgExport.COMOS_XMPLANT_CHAPTER_NAME, SvgExport.COMOS_XMPLANT_EXPORT_LISTED_ATTRIBUTES);
                    if (spec == null)
                    {
                        arrayLists = null;
                    }
                    else if (spec.@value != "1")
                    {
                        arrayLists = null;
                    }
                    else
                    {
                        IComosDSpecification comosDSpecification = this.GetSpec(comosDCollection, SvgExport.COMOS_XMPLANT_CHAPTER_NAME, SvgExport.COMOS_XMPLANT_GENERIC_ATTRIBUTES_LIST).Specifications().Item(SvgExport.COMOS_XMPLANT_COMOS_NESTED_NAMES) as IComosDSpecification;
                        if (comosDSpecification != null)
                        {
                            int num = comosDSpecification.MaxLinkedXValueCount();
                            if (num != 0)
                            {
                                for (int i = 0; i <= num - 1; i++)
                                {
                                    string xValue = comosDSpecification.GetXValue(i);
                                    if (!string.IsNullOrEmpty(xValue.Trim()))
                                    {
                                        arrayLists1.Add(xValue);
                                    }
                                }
                            }
                            else
                            {
                                arrayLists = arrayLists1;
                                return arrayLists;
                            }
                        }
                        return arrayLists1;
                    }
                }
                else
                {
                    arrayLists = null;
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                arrayLists = null;
            }
            return arrayLists;
        }

        private IComosDDevice[] GetNextDDevices(IComosDDevice oStartDevice)
        {
            IComosDDevice[] array = null;
            if (oStartDevice != null)
            {
                IComosDCollection comosDCollection = oStartDevice.AllConnectors();
                if (comosDCollection != null)
                {
                    List<IComosDDevice> comosDDevices = new List<IComosDDevice>();
                    array = new IComosDDevice[comosDCollection.Count()];
                    for (int i = 0; i < comosDCollection.Count(); i++)
                    {
                        IComosDConnector comosDConnector = (IComosDConnector)comosDCollection.Item(i + 1);
                        if (comosDConnector != null)
                        {
                            IComosDConnector comosDConnector1 = comosDConnector.ConnectedWith();
                            if (comosDConnector1 != null)
                            {
                                comosDDevices.Add((IComosDDevice)comosDConnector1.owner());
                            }
                            comosDConnector1 = null;
                            comosDConnector = null;
                        }
                    }
                    array = comosDDevices.ToArray();
                }
            }
            return array;
        }

        private string GetNodeNameByCDeviceDetailClass(IComosDCDevice CDev)
        {
            string detailClass;
            try
            {
                detailClass = CDev.DetailClass;
                detailClass = (detailClass != "<" ? "Equipment" : "Nozzle");
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                detailClass = "";
            }
            return detailClass;
        }

        private IPfdMod GetPfdMod()
        {
            IPfdMod mPfdMod;
            try
            {
                if (this.m_PfdMod == null)
                {
                    this.m_PfdMod = new PfdMod();
                }
                mPfdMod = this.m_PfdMod;
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                return null;
            }
            return mPfdMod;
        }

        private int GetPIDConnectorCount(IComosDDevice DDev)
        {
            int num;
            try
            {
                int num1 = -1;
                if (DDev != null)
                {
                    num1 = 0;
                    for (int i = 1; i <= DDev.Connectors().Count(); i++)
                    {
                        IComosDConnector comosDConnector = (IComosDConnector)DDev.Connectors().Item(i);
                        if (comosDConnector.Name.StartsWith(AppGlobal.GetDBStr(DBKey.I__DI)) || comosDConnector.Name.StartsWith(AppGlobal.GetDBStr(DBKey.O__DO)))
                        {
                            num1++;
                        }
                    }
                }
                num = num1;
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                num = -1;
            }
            return num;
        }

        private int GetRealItemCount()
        {
            int num;
            try
            {
                Items item = this.m_rDoc.Items();
                Item item1 = null;
                int num1 = 0;
                while (item.NextItem(out item1) != 0)
                {
                    num1++;
                }
                num = num1;
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                return 0;
            }
            return num;
        }

        internal void GetSaveFileName()
        {
            try
            {
                string str = AppGlobal.ITX("~07417 XMpLant Files");
                SaveFileDialog saveFileDialog = new SaveFileDialog()
                {
                    FileName = "",
                    DefaultExt = ".xml",
                    Filter = string.Concat(str, " (.xml)|*.xml")
                };
                bool? nullable = saveFileDialog.ShowDialog();
                if (nullable.GetValueOrDefault() & nullable.HasValue)
                {
                    this.m_strSaveFileName = saveFileDialog.FileName;
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
        }

        private string GetSchemaFilename()
        {
            if (!string.IsNullOrEmpty(this.ValidationFilesPath) && Directory.Exists(this.ValidationFilesPath))
            {
                return string.Concat(this.ValidationFilesPath, "\\", this.GetVersionDependentSchemaFilename());
            }
            return string.Concat(System.IO.Path.GetDirectoryName(this.m_strSaveFileName), "\\", this.GetVersionDependentSchemaFilename());
        }

        private IComosDSpecification GetSpec(IComosDCollection comosColl, string strChapterName, string strSpecName)
        {
            IComosDSpecification comosDSpecification;
            try
            {
                IComosDSpecification comosDSpecification1 = null;
                IComosDSpecification comosDSpecification2 = comosColl.Item(strChapterName) as IComosDSpecification;
                if (comosDSpecification2 != null)
                {
                    comosDSpecification1 = (IComosDSpecification)comosDSpecification2.Specifications().Item(strSpecName);
                }
                comosDSpecification = comosDSpecification1;
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                comosDSpecification = null;
            }
            return comosDSpecification;
        }

        private IComosDSpecification GetSpecByNestedName(IComosDCollection comosColl, string strComosNestedName)
        {
            IComosDSpecification spec;
            try
            {
                spec = this.GetSpec(comosColl, strComosNestedName.Split(new char[] { '.' })[0], strComosNestedName.Split(new char[] { '.' })[1]);
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                spec = null;
            }
            return spec;
        }

        private string GetSpecNestedNameOfEvaluableText(string EvaluableText)
        {
            string str = "";
            string str1 = "comosspec(";
            if (EvaluableText.ToLower().Contains(str1))
            {
                str = EvaluableText.Substring(EvaluableText.ToLower().IndexOf(str1) + str1.Length);
                string[] strArrays = str.Split(new char[] { ',' });
                str = string.Concat(strArrays[0].Trim().Replace("'", ""), ".", strArrays[1].Trim().Replace("'", ""));
            }
            return str;
        }

        private ArrayList GetSpecs(IComosDCollection comosColl, string strChapterName, string strSpecName)
        {
            ArrayList arrayLists;
            try
            {
                ArrayList arrayLists1 = new ArrayList();
                if (comosColl != null)
                {
                    IComosDSpecification comosDSpecification = comosColl.Item(strChapterName) as IComosDSpecification;
                    if (comosDSpecification != null)
                    {
                        for (int i = 1; i <= comosDSpecification.Specifications().Count(); i++)
                        {
                            IComosDSpecification comosDSpecification1 = comosDSpecification.Specifications().Item(i) as IComosDSpecification;
                            if (comosDSpecification1 != null && comosDSpecification1.Name.StartsWith(strSpecName))
                            {
                                arrayLists1.Add(comosDSpecification1);
                            }
                        }
                    }
                }
                arrayLists = arrayLists1;
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                arrayLists = null;
            }
            return arrayLists;
        }

        private double getTheta(IGeometry iGeo)
        {
            return iGeo.Angle / 360 * (2 * this.PI);
        }

        private void GetTrans(NTrans trans, double Angle, double ScleX, double ScleY, bool reflect, double x, double y)
        {
            double num = (double)((reflect ? -1 : 1));
            double num1 = num * (Math.Cos(Angle) * ScleX);
            double num2 = Math.Sin(Angle) * ScleY * -1;
            double num3 = num * (Math.Sin(Angle) * ScleX);
            double num4 = Math.Cos(Angle) * ScleY;
            trans.Assign3(num1, num2, num3, num4, x, y);
        }

        private string GetVersionDependentSchemaFilename()
        {
            if (this.m_xvExportVersion == enmXMpLantVersion.XMpLantVersion_4_0_1)
            {
                return "XMpLantPIDProfileSchema_4.0.1.xsd";
            }
            if (this.m_xvExportVersion == enmXMpLantVersion.XMpLantVersion_3_6_0)
            {
                return "XMpLantProfileSchema_3.6.0.xsd";
            }
            if (this.m_xvExportVersion == enmXMpLantVersion.XMpLantVersion_3_3_3)
            {
                return "XMpLantSchema_3.3.3.xsd";
            }
            if (this.m_xvExportVersion == enmXMpLantVersion.XMpLantVersion_3_2_0)
            {
                return "XMpLantProfileSchema_3.2.0.xsd";
            }
            return "";
        }

        private string GetVersionString()
        {
            if (this.m_xvExportVersion == enmXMpLantVersion.XMpLantVersion_4_0_1)
            {
                return "4.0.1";
            }
            if (this.m_xvExportVersion == enmXMpLantVersion.XMpLantVersion_3_6_0)
            {
                return "3.6.0";
            }
            if (this.m_xvExportVersion == enmXMpLantVersion.XMpLantVersion_3_3_3)
            {
                return "3.3.3";
            }
            if (this.m_xvExportVersion == enmXMpLantVersion.XMpLantVersion_3_2_0)
            {
                return "3.2.0";
            }
            return "";
        }

        private bool HasHatch(NGeoElement NGeo, ReportArc arc, ArrayList list)
        {
            bool flag;
            try
            {
                for (int i = 0; i < list.Count; i++)
                {
                    double[] item = new double[4];
                    item = (double[])list[i];
                    if (NGeo != null)
                    {
                        double num = Math.Abs(item[0] - NGeo.Umreck.left);
                        double num1 = Math.Abs(item[1] - NGeo.Umreck.bottom);
                        double num2 = Math.Abs(item[2] - NGeo.Umreck.right);
                        double num3 = Math.Abs(item[3] - NGeo.Umreck.top);
                        if (num < 0.01 && num1 < 0.01 && num2 < 0.01 && num3 < 0.01)
                        {
                            flag = true;
                            return flag;
                        }
                    }
                    else if (arc != null && item[0] <= arc.x1 && item[1] <= arc.y1 && item[2] >= arc.x2 && item[3] >= arc.y2)
                    {
                        flag = true;
                        return flag;
                    }
                }
                flag = false;
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                flag = false;
            }
            return flag;
        }

        private bool HasLocalSymbolScript(Item reportItem)
        {
            REPORTLib.Object obj = reportItem as REPORTLib.Object;
            if (obj != null)
            {
                IPfdRoDevice xObj = obj.XObj as IPfdRoDevice;
                if (xObj != null)
                {
                    ISymbolScript symbolScript = xObj as ISymbolScript;
                    if (symbolScript != null)
                    {
                        return symbolScript.IsSymbolScriptLocal;
                    }
                }
            }
            return true;
        }

        private bool IsComosBranch(IComosDDevice dDev)
        {
            bool flag;
            try
            {
                flag = GlobalUtilities.GU.GU_IsPipeBranch(dDev);
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                flag = false;
            }
            return flag;
        }

        private bool IsComosInstrument(IComosDDevice dDev)
        {
            try
            {
                IComosDSpecification specByNestedName = this.GetSpecByNestedName(dDev.Specifications(), string.Concat(SvgExport.COMOS_XMPLANT_CHAPTER_NAME, ".", SvgExport.COMOS_XMPLANT_PLANT_ITEM_NODE_NAME));
                if (specByNestedName != null && specByNestedName.DisplayValue().Trim() == "ProcessInstrumentationFunction")
                {
                    return true;
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
            return false;
        }

        private bool IsComosPipe(IComosDDevice dDev)
        {
            bool flag;
            try
            {
                flag = GlobalUtilities.GU.GU_IsPipeStream(dDev);
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                flag = false;
            }
            return flag;
        }

        private bool IsComosSegment(IComosDDevice dDev)
        {
            bool flag;
            try
            {
                flag = GlobalUtilities.GU.GU_IsPipeSegment(dDev);
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                flag = false;
            }
            return flag;
        }

        private bool isConnector(string HeaderClass)
        {
            bool flag;
            try
            {
                flag = this.GetPfdMod().isConnectorClass(HeaderClass);
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                return false;
            }
            return flag;
        }

        private bool IsFunctionLine(object XObj)
        {
            bool flag;
            try
            {
                IRoConnPfd xObj = XObj as IRoConnPfd;
                flag = (xObj == null ? false : xObj.ConnectionType == PfdConnectionType.ACTION_LINE);
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                return false;
            }
            return flag;
        }

        private bool IsOnLine(PointD endPoint1, PointD endPoint2, PointD checkPoint)
        {
            double num = 0;
            double num1 = 0;
            this.DistanceFromLine(checkPoint.x, checkPoint.y, endPoint1.x, endPoint1.y, endPoint2.x, endPoint2.y, ref num, ref num1);
            if (num < 1E-07)
            {
                return true;
            }
            return false;
        }

        private bool IsXMpLantLicenceAvailable()
        {
            if (AppGlobal.Workset.ModuleLicense(63) == null && AppGlobal.Workset.ModuleLicense(9) == null)
            {
                return false;
            }
            return true;
        }

        private void ProjectGetLineTypeMapping(IComosDProject comosProject)
        {
            try
            {
                this.m_arlLineTypeMapping.Clear();
                IComosDSpecification comosDSpecification = (IComosDSpecification)this.m_Workset.POptions().GetType().InvokeMember("GetOption", BindingFlags.InvokeMethod, null, this.m_Workset.POptions(), new object[] { 239 });
                IComosDSpecification comosDSpecification1 = (IComosDSpecification)this.m_Workset.POptions().GetType().InvokeMember("GetOption", BindingFlags.InvokeMethod, null, this.m_Workset.POptions(), new object[] { 240 });
                if (comosDSpecification != null && comosDSpecification1 != null)
                {
                    int num = comosDSpecification.MaxLinkedXValueCount();
                    for (int i = 0; i <= num - 1; i++)
                    {
                        string str = comosDSpecification.GetXValue(i).Trim();
                        string str1 = comosDSpecification1.GetXValue(i).Trim();
                        if (str != "" && str1 != "")
                        {
                            SvgExport.m_stcLineTypeMappingItem mStcLineTypeMappingItem = new SvgExport.m_stcLineTypeMappingItem(str, str1);
                            this.m_arlLineTypeMapping.Add(mStcLineTypeMappingItem);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
        }

        private void ProjectGetParameters(IComosDProject comosProject)
        {
            try
            {
                this.ProjectGetLineTypeMapping(comosProject);
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
        }

        private void RemoveEmptyConnectionElements(XmlElement root)
        {
            try
            {
                ArrayList arrayLists = new ArrayList();
                foreach (XmlElement elementsByTagName in root.GetElementsByTagName("Connection"))
                {
                    if (!(elementsByTagName.GetAttribute("FromID") == "") || !(elementsByTagName.GetAttribute("FromNode") == "-1") || !(elementsByTagName.GetAttribute("ToID") == "") || !(elementsByTagName.GetAttribute("ToNode") == "-1"))
                    {
                        continue;
                    }
                    arrayLists.Add(elementsByTagName);
                }
                foreach (XmlElement arrayList in arrayLists)
                {
                    arrayList.ParentNode.RemoveChild(arrayList);
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
        }

        private string RemoveSymbol(string s)
        {
            string str;
            try
            {
                if (s != "")
                {
                    string str1 = s;
                    if (char.IsNumber(s, 0))
                    {
                        string str2 = s.Substring(0, 1);
                        switch (str2)
                        {
                            case "0":
                                {
                                    string str3 = s.Remove(0, 1).Insert(0, "Zero");
                                    str1 = str3;
                                    str = str3;
                                    return str;
                                }
                            case "1":
                                {
                                    string str4 = s.Remove(0, 1).Insert(0, "One");
                                    str1 = str4;
                                    str = str4;
                                    return str;
                                }
                            case "2":
                                {
                                    string str5 = s.Remove(0, 1).Insert(0, "Two");
                                    str1 = str5;
                                    str = str5;
                                    return str;
                                }
                            case "3":
                                {
                                    string str6 = s.Remove(0, 1).Insert(0, "Three");
                                    str1 = str6;
                                    str = str6;
                                    return str;
                                }
                            case "4":
                                {
                                    string str7 = s.Remove(0, 1).Insert(0, "Four");
                                    str1 = str7;
                                    str = str7;
                                    return str;
                                }
                            case "5":
                                {
                                    string str8 = s.Remove(0, 1).Insert(0, "Five");
                                    str1 = str8;
                                    str = str8;
                                    return str;
                                }
                            case "6":
                                {
                                    string str9 = s.Remove(0, 1).Insert(0, "Six");
                                    str1 = str9;
                                    str = str9;
                                    return str;
                                }
                            case "7":
                                {
                                    string str10 = s.Remove(0, 1).Insert(0, "Seven");
                                    str1 = str10;
                                    str = str10;
                                    return str;
                                }
                            case "8":
                                {
                                    string str11 = s.Remove(0, 1).Insert(0, "Eight");
                                    str1 = str11;
                                    str = str11;
                                    return str;
                                }
                            case "9":
                                {
                                    string str12 = s.Remove(0, 1).Insert(0, "Nine");
                                    str1 = str12;
                                    str = str12;
                                    return str;
                                }
                        }
                    }
                    for (int i = str1.Length - 1; i >= 0; i--)
                    {
                        if (!char.IsLetter(str1, i) && !char.IsNumber(str1, i))
                        {
                            str1 = str1.Remove(i, 1);
                        }
                    }
                    str = str1;
                }
                else
                {
                    str = s;
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                str = "";
            }
            return str;
        }

        private void ReorderCenterLines(XmlElement xeSegment)
        {
            try
            {
                if (xeSegment.SelectNodes("./CenterLine") != null && xeSegment.SelectNodes("./CenterLine").Count > 1)
                {
                    string uIDOfElement = XMLHelper.GetUIDOfElement(this.m_xvExportVersion, xeSegment);
                    if (uIDOfElement != "")
                    {
                        IComosDDevice comosDDevice = this.m_Workset.LoadObjectByType(8, uIDOfElement) as IComosDDevice;
                        if (comosDDevice != null)
                        {
                            ArrayList arrayLists = new ArrayList();
                            ArrayList arrayLists1 = new ArrayList();
                            IComosDCollection comosDCollection = comosDDevice.Devices();
                            if (comosDCollection.Count() <= 0)
                            {
                                foreach (XmlElement childNode in xeSegment.ChildNodes)
                                {
                                    if (childNode.Name != "CenterLine")
                                    {
                                        continue;
                                    }
                                    arrayLists1.Add(childNode);
                                }
                            }
                            else
                            {
                                for (int i = 1; i <= comosDCollection.Count(); i++)
                                {
                                    IComosDDevice comosDDevice1 = comosDCollection.Item(i) as IComosDDevice;
                                    if (this.IsComosSegment(comosDDevice1))
                                    {
                                        arrayLists.Add(comosDDevice1.SystemUID());
                                    }
                                }
                                foreach (string arrayList in arrayLists)
                                {
                                    XmlElement elementByUID = XMLHelper.GetElementByUID(this.m_xvExportVersion, this.m_xmlDoc.DocumentElement, arrayList, true);
                                    if (elementByUID == null)
                                    {
                                        continue;
                                    }
                                    if (elementByUID.Name == "CenterLine")
                                    {
                                        arrayLists1.Add(elementByUID);
                                    }
                                    if (elementByUID.NextSibling == null || !(elementByUID.NextSibling.Name == "InstrumentConnection"))
                                    {
                                        continue;
                                    }
                                    arrayLists1.Add(elementByUID.NextSibling);
                                }
                            }
                            if (arrayLists1.Count > 0)
                            {
                                foreach (XmlElement xmlElement in arrayLists1)
                                {
                                    try
                                    {
                                        xeSegment.RemoveChild(xmlElement);
                                    }
                                    catch
                                    {
                                    }
                                }
                                foreach (XmlElement arrayList1 in arrayLists1)
                                {
                                    xeSegment.AppendChild(arrayList1);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
        }

        private void ReorderPipingNetworkSegments()
        {
            try
            {
                XmlElement documentElement = this.m_xmlDoc.DocumentElement;
                foreach (XmlElement elementsByTagName in documentElement.GetElementsByTagName("PipingNetworkSegment"))
                {
                    IComosDDevice comosDDevice = null;
                    string uIDOfElement = XMLHelper.GetUIDOfElement(this.m_xvExportVersion, elementsByTagName);
                    if (uIDOfElement.Length != 0)
                    {
                        comosDDevice = this.m_Workset.LoadObjectByType(8, uIDOfElement) as IComosDDevice;
                        this.CreateConnection(elementsByTagName, comosDDevice);
                    }
                    this.ReorderCenterLines(elementsByTagName);
                    this.RepositionComponents(documentElement, elementsByTagName);
                    this.RemoveEmptyConnectionElements(documentElement);
                    if (comosDDevice == null)
                    {
                        continue;
                    }
                    this.CreateXMpLantFixedAttributes(elementsByTagName, comosDDevice);
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
        }

        private void ReportArcToCircle(Item ReportItem, XmlElement ParentNode, ArrayList hatchs)
        {
            try
            {
                ReportArc reportItem = ReportItem as ReportArc;
                double num = Math.Sqrt(Math.Pow(reportItem.x1 - reportItem.x2, 2) + Math.Pow(reportItem.y1 - reportItem.y2, 2));
                XmlElement xmlElement = this.m_xmlDoc.CreateElement("TrimmedCurve");
                bool fill = reportItem.Fill != 0;
                ParentNode.AppendChild(xmlElement);
                if (reportItem.Angle != 360)
                {
                    double startAngle = reportItem.StartAngle + reportItem.Angle;
                    double startAngle1 = reportItem.StartAngle;
                    xmlElement.SetAttribute("StartAngle", startAngle1.ToString().Replace(",", "."));
                    xmlElement.SetAttribute("EndAngle", startAngle.ToString().Replace(",", "."));
                }
                else
                {
                    xmlElement.SetAttribute("StartAngle", "0");
                    xmlElement.SetAttribute("EndAngle", "360");
                }
                this.CreateCircle(reportItem.x1, reportItem.y1, xmlElement, this.m_xmlDoc, num, ReportItem, false, null, true, 0, null, this.HasHatch(null, reportItem, hatchs), fill, null, true);
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
        }

        private void ReportExport_FoundReportArc(object sender, FoundReportItemEventArgs e)
        {
        }

        private void ReportExport_FoundReportLine(object sender, FoundReportItemEventArgs e)
        {
            try
            {
                this.CreateLine(e.DocItem.P1.x, e.DocItem.P1.y, e.DocItem.P2.x, e.DocItem.P2.y, this.m_xmlDoc, this.m_DrawingBorder, e.DocItem, false, true, e.DocItem.ReportDocument().DrawingScale, null, null);
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
        }

        private void ReportExport_FoundReportText(object sender, FoundReportItemEventArgs e)
        {
            try
            {
                this.CreateText(this.m_DrawingBorder, e.DocItem, null);
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
        }

        private void RepositionComponents(XmlElement root, XmlElement xeSegment)
        {
            try
            {
                XmlNodeList elementsByTagName = xeSegment.GetElementsByTagName("CenterLine");
                if (elementsByTagName.Count >= 1)
                {
                    foreach (XmlElement xmlElement in elementsByTagName)
                    {
                        string uIDOfElement = XMLHelper.GetUIDOfElement(this.m_xvExportVersion, xmlElement);
                        IComosDDevice comosDDevice = (IComosDDevice)this.m_Workset.LoadObjectByType(8, uIDOfElement);
                        if (comosDDevice == null)
                        {
                            continue;
                        }
                        IComosDConnector connectorByFlow = this.GetConnectorByFlow(comosDDevice, SvgExport.enmDeviceConnector.dcDeviceOutputConnector);
                        IComosDConnector comosDConnector = connectorByFlow.ConnectedWith();
                        if (comosDConnector == null || comosDConnector.owner() == null)
                        {
                            continue;
                        }
                        IComosDDevice comosDDevice1 = connectorByFlow.ConnectedWith().owner() as IComosDDevice;
                        if (comosDDevice1 == null || this.IsComosSegment(comosDDevice1))
                        {
                            continue;
                        }
                        XmlElement elementByUID = XMLHelper.GetElementByUID(this.m_xvExportVersion, this.m_xmlDoc.DocumentElement, comosDDevice1.SystemUID(), true);
                        this.AddElementsToReorderCollection(new SvgExport.m_ReorderElements(xmlElement, elementByUID));
                    }
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
        }

        private DateTime RetrieveLinkerTimestamp()
        {
            string location = Assembly.GetCallingAssembly().Location;
            byte[] numArray = new byte[2048];
            Stream fileStream = null;
            try
            {
                fileStream = new FileStream(location, FileMode.Open, FileAccess.Read);
                fileStream.Read(numArray, 0, 2048);
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
            int num = BitConverter.ToInt32(numArray, 60);
            int num1 = BitConverter.ToInt32(numArray, num + 8);
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0);
            dateTime = dateTime.AddSeconds((double)num1);
            TimeSpan utcOffset = TimeZone.CurrentTimeZone.GetUtcOffset(dateTime);
            dateTime = dateTime.AddHours((double)utcOffset.Hours);
            return dateTime;
        }

        private void RotateCoord(ref double x, ref double y, double theta)
        {
            try
            {
                double num = Math.Sqrt(x * x + y * y);
                double num1 = this.Arctg(x, y);
                x = num * Math.Cos(num1 - theta);
                y = num * Math.Sin(num1 - theta);
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
        }

        private bool SchemaValidation()
        {
            string empty = string.Empty;
            string str = string.Empty;
            bool flag = true;
            XmlSchemaValidation xmlSchemaValidation = new XmlSchemaValidation()
            {
                XmlFileName = this.m_strSaveFileName,
                XmlSchemaFileName = this.GetSchemaFilename()
            };
            if (!File.Exists(xmlSchemaValidation.XmlSchemaFileName))
            {
                this.m_Logging.Information(string.Concat(AppGlobal.ITX("~073ff The referenced schema file does not exist"), ": ", xmlSchemaValidation.XmlSchemaFileName));
                flag = false;
            }
            else
            {
                xmlSchemaValidation.ValidateFile();
                this.m_Logging.WaitMsg(AppGlobal.ITX("~073fc Schema validation"), xmlSchemaValidation.ValidationEvents.Count);
                int num = 1;
                this.m_Logging.Information(string.Concat(new string[] { "Xml file: '", xmlSchemaValidation.XmlFileName, "'", Environment.NewLine, "Schema file: '", xmlSchemaValidation.XmlSchemaFileName, "'" }));
                foreach (ValidationEventArgs validationEvent in xmlSchemaValidation.ValidationEvents)
                {
                    int num1 = num;
                    num = num1 + 1;
                    this.m_Logging.WaitProgress = (long)num1;
                    if (validationEvent.Severity == XmlSeverityType.Warning)
                    {
                        empty = string.Concat(empty, this.GenerateSchemaValidationMessage(validationEvent), Environment.NewLine);
                    }
                    if (validationEvent.Severity != XmlSeverityType.Error)
                    {
                        continue;
                    }
                    str = string.Concat(str, this.GenerateSchemaValidationMessage(validationEvent), Environment.NewLine);
                }
                if (!string.IsNullOrEmpty(empty))
                {
                    this.m_Logging.Warning(empty, null);
                    flag = false;
                }
                if (!string.IsNullOrEmpty(str))
                {
                    this.m_Logging.Error(str);
                    flag = false;
                }
                this.m_Logging.Information(string.Concat(AppGlobal.ITX("~073fc Schema validation"), " ", AppGlobal.ITX("~03744 Done")));
            }
            if (flag)
            {
                this.m_Logging.Success(AppGlobal.ITX("~06044 Successful"));
            }
            this.m_Logging.StopCurrentTask();
            return flag;
        }

        private static void SetAttributeCount(XmlElement xeGenericAttributesSet)
        {
            xeGenericAttributesSet.SetAttribute("Number", xeGenericAttributesSet.ChildNodes.Count.ToString());
        }

        public void SetExportVerion_3_2_0()
        {
            this.m_xvExportVersion = enmXMpLantVersion.XMpLantVersion_3_2_0;
        }

        public void SetExportVerion_3_3_3()
        {
            this.m_xvExportVersion = enmXMpLantVersion.XMpLantVersion_3_3_3;
        }

        public void SetExportVerion_3_6_0()
        {
            this.m_xvExportVersion = enmXMpLantVersion.XMpLantVersion_3_6_0;
        }

        private enmXMpLantVersion SetUnattendedSchemaVersion(string ExportVersion)
        {
            if (ExportVersion == "3.3.3")
            {
                return enmXMpLantVersion.XMpLantVersion_3_3_3;
            }
            if (ExportVersion == "3.6.0")
            {
                return enmXMpLantVersion.XMpLantVersion_3_6_0;
            }
            if (ExportVersion == "4.0.1")
            {
                return enmXMpLantVersion.XMpLantVersion_4_0_1;
            }
            return this.m_xvExportVersion;
        }

        private string SoftAlignTrans(double x, double y, ref double x1, ref double y1, ref double x2, ref double y2)
        {
            string str;
            try
            {
                str = string.Concat(x.ToString(), y.ToString());
                switch (str)
                {
                    case "02":
                        {
                            x1 = 0;
                            y1 = -1;
                            x2 = 1;
                            y2 = 0;
                            str = "LeftTop";
                            break;
                        }
                    case "01":
                        {
                            x1 = 0;
                            y1 = -0.5;
                            x2 = 1;
                            y2 = 0.5;
                            str = "LeftCenter";
                            break;
                        }
                    case "00":
                        {
                            x1 = 0;
                            y1 = 0;
                            x2 = 1;
                            y2 = 1;
                            str = "LeftBottom";
                            break;
                        }
                    case "12":
                        {
                            x1 = -0.5;
                            y1 = -1;
                            x2 = 0.5;
                            y2 = 0;
                            str = "CenterTop";
                            break;
                        }
                    case "11":
                        {
                            x1 = -0.5;
                            y1 = -0.5;
                            x2 = 0.5;
                            y2 = 0.5;
                            str = "CenterCenter";
                            break;
                        }
                    case "10":
                        {
                            x1 = -0.5;
                            y1 = 0;
                            x2 = 0.5;
                            y2 = 1;
                            str = "CenterBottom";
                            break;
                        }
                    case "22":
                        {
                            x1 = -1;
                            y1 = -1;
                            x2 = 0;
                            y2 = 0;
                            str = "RightTop";
                            break;
                        }
                    case "21":
                        {
                            x1 = -1;
                            y1 = -0.5;
                            x2 = 0;
                            y2 = 0.5;
                            str = "RightCenter";
                            break;
                        }
                    case "20":
                        {
                            x1 = -1;
                            y1 = 0;
                            x2 = 0;
                            y2 = 1;
                            str = "RightBottom";
                            break;
                        }
                    default:
                        {
                            str = "LeftBottom";
                            break;
                        }
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                str = "";
            }
            return str;
        }

        private void SymbolIntoCatalog(IComosDDevice DDev, ComosGeoEngine Ge, object XObj, Item ItemInDoc, ref string MandatoryShapeCatalogueComponentName)
        {
            try
            {
                if (DDev.CDevice != null)
                {
                    string str = this._getComponentName(ItemInDoc);
                    MandatoryShapeCatalogueComponentName = str;
                    if (!this.m_StockNumbers.Contains(str))
                    {
                        if (string.IsNullOrEmpty(str))
                        {
                            str = this.m_ShapeCatalogueNumber.GetNextNumber().ToString();
                            MandatoryShapeCatalogueComponentName = str;
                        }
                        XmlElement xmlElement = this.CreatePlantItem(DDev.CDevice, this.GetNodeNameByCDeviceDetailClass(DDev.CDevice), false);
                        this.CreateAttributesForPlantItem(xmlElement, DDev.CDevice, enmXMpLantComponentType.NoComponentType, MandatoryShapeCatalogueComponentName);
                        this.m_StockNumbers.Add(str);
                        this.m_Catalogue.AppendChild(xmlElement);
                        this.SymbolToNode(Ge, XObj, ItemInDoc, DDev, this.m_xmlDoc, xmlElement, false);
                        this.CreateXMpLantFixedAttributes(xmlElement, DDev.CDevice);
                    }
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
        }

        private void SymbolToNode(ComosGeoEngine Ge, object XObj, Item ItemInDoc, IComosDDevice DDev, XmlDocument xmlDoc, XmlElement parentNode, bool IsPlantItemInstance)
        {
            try
            {
                IGeometry xObj = XObj as IGeometry;
                double scaleValueX = xObj.ScaleValueX;
                double scaleValueY = xObj.ScaleValueY;
                double num = SvgExport.M3D.RAD(xObj.Angle);
                string str = ItemInDoc.Layer.ToString();
                string str1 = ItemInDoc.LineType.ToString();
                string str2 = ItemInDoc.Width.ToString();
                int color = ItemInDoc.Color;
                this.CreateElementPresentation(xmlDoc, parentNode, str, str1, str2, color.ToString());
                if (!IsPlantItemInstance)
                {
                    this.CreateElementExtent(parentNode, Ge.Umreck.left / scaleValueX, Ge.Umreck.bottom / scaleValueY, Ge.Umreck.right / scaleValueX, Ge.Umreck.top / scaleValueY);
                    this.CreateElementPosition(parentNode, 0, 0, 0);
                    this.AddGenericAttributesToPlantItem(parentNode, ((IRoDevice)XObj).Device.CDevice, true);
                    this.CreateGraphicalElements(Ge, ItemInDoc, xmlDoc, parentNode, IsPlantItemInstance, xObj);
                }
                else
                {
                    this.CreateElementExtent(parentNode, Ge.Umreck.left + ItemInDoc.x1, this.m_ComosDocumentSizeY + Ge.Umreck.bottom - ItemInDoc.y1, Ge.Umreck.right + ItemInDoc.x1, this.m_ComosDocumentSizeY + Ge.Umreck.top - ItemInDoc.y1);
                    this.CreateElementPosition(parentNode, ItemInDoc.x1, (double)(this.m_ComosDocumentSizeY - ItemInDoc.y1), num, xObj.Reflect);
                    this.CreateElementScale(parentNode, scaleValueX, scaleValueY, 1);
                    this.CreateNodePersistentID(parentNode, ((IRoDevice)XObj).Device);
                    this.AddGenericAttributesToPlantItem(parentNode, ((IRoDevice)XObj).Device, true);
                    if (parentNode.Attributes["ComponentType"].Value != "Explicit")
                    {
                        this.CreateGraphicalElements(Ge, ItemInDoc, xmlDoc, parentNode, IsPlantItemInstance, xObj);
                    }
                }
                ArrayList connectors = this.GetConnectors(Ge, ItemInDoc, parentNode, IsPlantItemInstance);
                this.CreateElementConnectionPoints(parentNode, ItemInDoc, DDev, IsPlantItemInstance, connectors, num, xObj);
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
        }

        private void SymbolToXMPlant(object XObj, Item ItemInDoc)
        {
            try
            {
                if (!(XObj is IPfdRoFlag) && !(XObj is IRoConnPfd))
                {
                    IRoDevice xObj = XObj as IRoDevice;
                    IComosDDevice device = xObj.Device;
                    if (device != null)
                    {
                        if (!this.IsComosInstrument(device) || !this.CreateInstrument(device, ItemInDoc, XObj))
                        {
                            bool flag = this.HasLocalSymbolScript(ItemInDoc);
                            IRoDeviceCommon roDeviceCommon = xObj as IRoDeviceCommon;
                            roDeviceCommon.IBaseRoDevice.Container.EvalDone = 1;
                            ComosGeoEngine comosGeoEngine = roDeviceCommon.IBaseRoDevice.Container.GeoEngine(false) as ComosGeoEngine;
                            XmlElement elementByUID = XMLHelper.GetElementByUID(this.m_xvExportVersion, this.m_xmlDoc.DocumentElement, device.SystemUID(), true);
                            if (elementByUID != null)
                            {
                                XmlElement parentNode = null;
                                if (!(elementByUID.Name == "CenterLine") || !(elementByUID.ParentNode.Name == "PipingNetworkSegment"))
                                {
                                    parentNode = (elementByUID.Name != "PipingNetworkSegment" ? elementByUID : elementByUID);
                                }
                                else
                                {
                                    parentNode = (XmlElement)elementByUID.ParentNode;
                                }
                                if (parentNode != null)
                                {
                                    IGeometry geometry = XObj as IGeometry;
                                    this.CreateGraphicalElements(comosGeoEngine, ItemInDoc, this.m_xmlDoc, parentNode, false, geometry);
                                }
                            }
                            else
                            {
                                IComosDDevice comosDDevice = device.owner() as IComosDDevice;
                                string str = "";
                                if (comosDDevice != null)
                                {
                                    str = comosDDevice.SystemUID();
                                }
                                if (device.CDevice != null)
                                {
                                    string str1 = "Equipment";
                                    if (device.CDevice.DetailClass == "<")
                                    {
                                        str1 = "Nozzle";
                                        if (comosDDevice != null && this.IsComosBranch(comosDDevice))
                                        {
                                            this.m_dictCollectedNozzles.Add(device.SystemUID(), device);
                                            str1 = "PipingComponent";
                                            elementByUID = this.CreatePlantItem(device, str1, true);
                                        }
                                    }
                                    if (elementByUID == null)
                                    {
                                        elementByUID = this.CreatePlantItem(device, str1, false);
                                    }
                                    string empty = string.Empty;
                                    if (!flag || this.m_xvExportVersion == enmXMpLantVersion.XMpLantVersion_4_0_1)
                                    {
                                        this.SymbolIntoCatalog(device, comosGeoEngine, XObj, ItemInDoc, ref empty);
                                    }
                                    this.CreateAttributesForPlantItem(elementByUID, device, (flag ? enmXMpLantComponentType.Normal : enmXMpLantComponentType.Explicit), empty);
                                    XmlNode xmlNodes = null;
                                    xmlNodes = XMLHelper.GetElementByUID(this.m_xvExportVersion, this.m_xmlDoc.DocumentElement, str, true);
                                    if (xmlNodes == null)
                                    {
                                        this.m_Root.AppendChild(elementByUID);
                                    }
                                    else
                                    {
                                        xmlNodes.AppendChild(elementByUID);
                                    }
                                    this.SymbolToNode(comosGeoEngine, XObj, ItemInDoc, device, this.m_xmlDoc, elementByUID, true);
                                    this.CreateXMpLantFixedAttributes(elementByUID, device);
                                }
                                else
                                {
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
            }
        }

        private void TransformCoord(double theta, double scaleValueX, double scaleValueY, bool reflect, double coordX, double coordY, out double rotatedCoordX, out double rotatedCoordY)
        {
            NTrans nTransClass = new NTransClass();
            NCoordClass nCoordClass = new NCoordClass();
            ((INCoord)nCoordClass).x = coordX;
            ((INCoord)nCoordClass).y = coordY;
            NCoord nCoord = nCoordClass;
            this.GetTrans(nTransClass, theta, 1, 1, false, 0, 0);
            nTransClass.Transform(nCoord);
            this.GetTrans(nTransClass, 0, scaleValueX, scaleValueY, false, 0, 0);
            nTransClass.Transform(nCoord);
            this.GetTrans(nTransClass, 0, 1, 1, reflect, 0, 0);
            nTransClass.Transform(nCoord);
            rotatedCoordX = nCoord.x;
            rotatedCoordY = nCoord.y;
        }

        private string TranslateLineType(string strComosLineType)
        {
            string xMpLantLineType;
            try
            {
                if (this.m_arlLineTypeMapping.Count > 0)
                {
                    foreach (SvgExport.m_stcLineTypeMappingItem mArlLineTypeMapping in this.m_arlLineTypeMapping)
                    {
                        if (mArlLineTypeMapping.ComosLineType != strComosLineType)
                        {
                            continue;
                        }
                        xMpLantLineType = mArlLineTypeMapping.XMpLantLineType;
                        return xMpLantLineType;
                    }
                }
                xMpLantLineType = (this.m_fUseOldLineTypeDefault ? "0" : strComosLineType);
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                xMpLantLineType = (this.m_fUseOldLineTypeDefault ? "0" : strComosLineType);
            }
            return xMpLantLineType;
        }

        private void VerifyIso15926File()
        {
            if (this.m_Logging != null)
            {
                bool flag = true;
                this.m_Logging.StartNewCategory("Verifying XML File");
                bool flag1 = this.SchemaValidation();
                flag = this.DexpiVerify();
                this.m_Logging.StopCurrentCategory();
                this.m_Logging.StopCurrentTask();
            }
        }

        private void WriteGraphicalElements(NDisplayElement De, ComosGeoEngine Ge, IGeometry IGeo, XmlDocument xmlDoc, XmlElement childNode, Item ItemInDoc, bool IsPlantItemInstance, double theta, ArrayList hatches, NPath workingGeoPath)
        {
            bool flag;
            NGeoElement geometrie = De.Geometrie;
            bool flag1 = false;
            switch (geometrie.Type)
            {
                case 1:
                    {
                        NLine nLine = geometrie as NLine;
                        this.CreateLine(nLine.P1.x, nLine.P1.y, nLine.P2.x, nLine.P2.y, xmlDoc, childNode, ItemInDoc, IsPlantItemInstance, false, theta, De, IGeo);
                        return;
                    }
                case 2:
                    {
                        NCircle nCircle = geometrie as NCircle;
                        double radius = nCircle.Radius;
                        if ((radius != 1 ? false : workingGeoPath == null))
                        {
                            return;
                        }
                        if (nCircle.Fill != 0)
                        {
                            flag = true;
                        }
                        else
                        {
                            flag = (workingGeoPath != null ? workingGeoPath.FillMode == enumPathFillType.pftSolidFill : false);
                        }
                        bool flag2 = flag;
                        flag1 = (workingGeoPath == null ? false : this.HasHatch(geometrie, null, hatches));
                        this.CreateCircle(nCircle.CP.x, nCircle.CP.y, childNode, xmlDoc, radius, ItemInDoc, IsPlantItemInstance, nCircle.Umreck, false, theta, De, flag1, flag2, IGeo, false);
                        return;
                    }
                case 3:
                    {
                        NArc nArc = geometrie as NArc;
                        Convert.ToBoolean(nArc.Fill);
                        if (workingGeoPath != null)
                        {
                            enumPathFillType fillMode = workingGeoPath.FillMode;
                        }
                        this.CreateCircleArc(Ge, nArc, childNode, xmlDoc, ItemInDoc, IsPlantItemInstance, theta, De, IGeo);
                        return;
                    }
                case 4:
                case 5:
                case 7:
                    {
                        return;
                    }
                case 6:
                    {
                        NPath nPath = geometrie as NPath;
                        int num = nPath.ItemCount();
                        for (int i = 0; i < num; i++)
                        {
                            NDisplayElement nDisplayElement = nPath.Item(i);
                            NGeoElement nGeoElement = nDisplayElement.Geometrie;
                            this.WriteGraphicalElements(nDisplayElement, Ge, IGeo, xmlDoc, childNode, ItemInDoc, IsPlantItemInstance, theta, hatches, nPath);
                        }
                        return;
                    }
                case 8:
                    {
                        XmlElement xmlElement = this.CreateElementPolyLineFromRectangle(ItemInDoc, De, IGeo, theta, IsPlantItemInstance);
                        if (xmlElement == null)
                        {
                            return;
                        }
                        childNode.AppendChild(xmlElement);
                        return;
                    }
                default:
                    {
                        return;
                    }
            }
        }

        private bool XMpLantExportVersion_3_2_0()
        {
            bool flag;
            try
            {
                object obj = (this.m_rDoc.ComosDocument() as IComosDDocument).Workset().Globals();
                obj.GetType();
                object obj1 = null;
                IPLTVarstorage pLTVarstorage = obj as IPLTVarstorage;
                if (pLTVarstorage != null)
                {
                    obj1 = pLTVarstorage.ItemByKey("XMpLantExportVersion_3_2_0");
                }
                bool? nullable = (bool?)(obj1 as bool?);
                flag = (!nullable.HasValue ? false : nullable.Value);
            }
            catch (Exception exception)
            {
                AppGlobal.StdErrorHandler(exception);
                flag = false;
            }
            return flag;
        }

        private enum enmDeviceConnector
        {
            dcDeviceInputConnector = 1,
            dcDeviceOutputConnector = 2
        }

        public enum enmXValueType
        {
            Normal = -1,
            Min = 0,
            Max = 1
        }

        internal static class IntPipeLib
        {
            internal static IComosDDocObj FindAfterSegmentsPlacedOnOtherReport(IComosDDevice CurrentSegment)
            {
                if (CurrentSegment == null)
                {
                    return null;
                }
                IComosDOwnCollection comosDOwnCollection = CurrentSegment.OwnerCollection() as IComosDOwnCollection;
                int num = comosDOwnCollection.ItemIndex(CurrentSegment);
                IComosDDocObj firstDocObjOfDevice = SvgExport.IntPipeLib.GetFirstDocObjOfDevice(CurrentSegment);
                for (int i = num + 1; i <= comosDOwnCollection.Count(); i++)
                {
                    IComosDDevice comosDDevice = comosDOwnCollection.Item(i) as IComosDDevice;
                    if (SvgExport.IntPipeLib.IsComosSegment(comosDDevice))
                    {
                        IComosDCollection comosDCollection = comosDDevice.BackPointerPlacedDocObjs();
                        for (int j = 1; j <= comosDCollection.Count(); j++)
                        {
                            IComosDDocObj comosDDocObj = comosDCollection.Item(j) as IComosDDocObj;
                            if (!(comosDDocObj.owner() as IComosBaseObject).IsObjectEqual(firstDocObjOfDevice.owner() as IComosBaseObject))
                            {
                                return comosDDocObj;
                            }
                        }
                    }
                }
                return null;
            }

            internal static IComosDDocObj FindBeforeSegmentsPlacedOnOtherReport(IComosDDevice CurrentSegment)
            {
                IComosDOwnCollection comosDOwnCollection = CurrentSegment.OwnerCollection() as IComosDOwnCollection;
                int num = comosDOwnCollection.ItemIndex(CurrentSegment);
                IComosDDocObj firstDocObjOfDevice = SvgExport.IntPipeLib.GetFirstDocObjOfDevice(CurrentSegment);
                for (int i = num - 1; i >= 1; i--)
                {
                    IComosDDevice comosDDevice = comosDOwnCollection.Item(i) as IComosDDevice;
                    if (SvgExport.IntPipeLib.IsComosSegment(comosDDevice))
                    {
                        IComosDCollection comosDCollection = comosDDevice.BackPointerPlacedDocObjs();
                        for (int j = 1; j <= comosDCollection.Count(); j++)
                        {
                            IComosDDocObj comosDDocObj = comosDCollection.Item(j) as IComosDDocObj;
                            if (!(comosDDocObj.owner() as IComosBaseObject).IsObjectEqual(firstDocObjOfDevice.owner() as IComosBaseObject))
                            {
                                return comosDDocObj;
                            }
                        }
                    }
                }
                return null;
            }

            internal static IComosDDocObj GetFirstDocObjOfDevice(IComosDDevice Dev)
            {
                IComosDCollection comosDCollection = Dev.BackPointerPlacedDocObjs();
                if (comosDCollection.Count() == 0)
                {
                    return null;
                }
                return comosDCollection.Item(1) as IComosDDocObj;
            }

            internal static bool IsComosSegment(IComosDDevice dDev)
            {
                bool flag;
                try
                {
                    flag = (dDev == null || !(dDev.Class == "E") || !(dDev.DetailClass == "Q") ? false : true);
                }
                catch (Exception exception)
                {
                    AppGlobal.StdErrorHandler(exception);
                    flag = false;
                }
                return flag;
            }
        }

        private class m_ReorderElements
        {
            public XmlElement NextElement
            {
                get;
                set;
            }

            public XmlElement PreviousElement
            {
                get;
                set;
            }

            public m_ReorderElements(XmlElement _PreviousElement, XmlElement _NextElement)
            {
                this.PreviousElement = _PreviousElement;
                this.NextElement = _NextElement;
            }
        }

        private class m_SignalLine
        {
            public Item ItemInDoc
            {
                get;
                set;
            }

            public object XObj
            {
                get;
                set;
            }

            public m_SignalLine(object _XObj, Item _ItemInDoc)
            {
                this.XObj = _XObj;
                this.ItemInDoc = _ItemInDoc;
            }
        }

        private struct m_stcLineTypeMappingItem
        {
            public string ComosLineType;

            public string XMpLantLineType;

            public m_stcLineTypeMappingItem(string ComosLineType, string XMpLantLineType)
            {
                this.ComosLineType = ComosLineType;
                this.XMpLantLineType = XMpLantLineType;
            }
        }
    }
}
