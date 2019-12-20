import json
import xmltodict
import svgwrite
import os

def line():

def circle():



def main():
    xml_file=open("TEST.XML",'r',encoding='UTF-8')
    xml_str=xml_file.read()
    json_dict=xmltodict.parse(xml_str)


if __name__=="__main__":
    main()