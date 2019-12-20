import json
import xmltodict

xml_file=open("TEST.XML",'r',encoding='UTF-8')
xml_str=xml_file.read()
json_dict=xmltodict.parse(xml_str)
json.dump(json_dict,open("TEST_OUT.json",'w'),indent=4)
