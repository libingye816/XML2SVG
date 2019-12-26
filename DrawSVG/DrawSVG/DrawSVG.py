import json
import xmltodict
import svgwrite
import os
import math

SCALE=1.0
PATH="output.svg"
MAX_HEIGHT=297
def line(prop,pos=None):
    if pos is None:
        pos={"Location":{"x":0.,"y":0.}}
    line_ref=list(pos["Location"].values())[:2]
    line_start = list(prop["Coordinate"][0].values())[:2]
    line_end = list(prop["Coordinate"][1].values())[:2]
    line_start[0]=float(line_start[0])+float(line_ref[0])
    line_start[1]=MAX_HEIGHT-(float(line_start[1])+float(line_ref[1]))
    line_end[0]=float(line_end[0])+float(line_ref[0])
    line_end[1]=MAX_HEIGHT-(float(line_end[1])+float(line_ref[1]))
    svg_entity = svgwrite.Drawing().line(start=tuple(line_start), end=tuple(line_end), stroke = "black", stroke_width = 0.5/SCALE )
    #svg_entity.scale(SCALE,-SCALE)
    return svg_entity
def circle(prop,pos=None):
    if pos is None:
        pos={"Location":{"x":0.,"y":0.}}
    circle_ref=list(pos["Location"].values())[:2]
    circle_center = list(prop["Position"]["Location"].values())[:2]
    circle_radius = float(prop["@Radius"])
    circle_center[0]=float(circle_center[0])+float(circle_ref[0])
    circle_center[1]=MAX_HEIGHT-(float(circle_center[1])+float(circle_ref[1]))
    svg_entity = svgwrite.Drawing().circle(center=tuple(circle_center), r=circle_radius, stroke = "black", fill="none", stroke_width = 0.5/SCALE )
    #svg_entity.scale(SCALE,-SCALE)
    return svg_entity
def text(prop):
    #text_insert = [float(x) for x in prop["Position"]["Location"].values()[:2]]
    #text_height = dxf_entity.dxf.height * 1.4 # hotfix - 1.4 to fit svg and dvg
    svg_entity = svgwrite.Drawing().text(prop["@String"], x=[float(list(prop["Position"]["Location"].values())[0])-float(prop["@Width"])/2],y=[MAX_HEIGHT-float(list(prop["Position"]["Location"].values())[1])+float(prop["@Height"])/2],font_family=prop["@Font"],font_size = float(prop["@Height"])*SCALE)
    #svg_entity.translate(text_insert[0]*(SCALE), -text_insert[1]*(SCALE))
    return svg_entity
def polyline(prop,pos=None):
    if pos is None:
        pos={"Location":{"x":0.,"y":0.}}
    point_list = [(float(list(x.values())[0])+float(list(pos["Location"].values())[0]), MAX_HEIGHT-(float(list(x.values())[1])+float(list(pos["Location"].values())[1]))) for x in prop["Coordinate"]]
    svg_entity = svgwrite.Drawing().polyline(points=point_list, stroke='black', fill='none', stroke_width=0.5/SCALE)
    #svg_entity.scale(SCALE, -SCALE)
    return svg_entity
def trimmedcurve(prop,pos=None):
    if pos is None:
        pos={"Location":{"x":0.,"y":0.}}
    startAngle=float(prop["@StartAngle"])
    endAngle=float(prop["@EndAngle"])
    assert "Circle" in prop
    circle_R=float(prop["Circle"]["@Radius"])
    circle_center=list(prop["Circle"]["Position"]["Location"].values())[:2]
    circle_ref=list(pos["Location"].values())[:2]
    circle_center[0]=float(circle_center[0])+float(circle_ref[0])
    circle_center[1]=float(circle_center[1])+float(circle_ref[1])
    if endAngle-startAngle>=360:
        circle_center[1]=MAX_HEIGHT-circle_center[1]
        return svgwrite.Drawing().circle(center=tuple(circle_center), r=circle_R, stroke = "black", fill="none", stroke_width = 0.5/SCALE )
    isLarge=False
    if endAngle-startAngle>180: 
        isLarge=True
    
    startPoint=[0.,0.]
    startPoint[0]=circle_center[0]+math.cos(startAngle*math.pi/180.0)*circle_R
    startPoint[1]=MAX_HEIGHT-(circle_center[1]+math.sin(startAngle*math.pi/180.0)*circle_R)
    endPoint=[0.,0.]
    endPoint[0]=circle_center[0]+math.cos(endAngle*math.pi/180.0)*circle_R
    endPoint[1]=MAX_HEIGHT-(circle_center[1]+math.sin(endAngle*math.pi/180.0)*circle_R)
    command=["M",tuple(startPoint)]
    svg_entity=svgwrite.Drawing().path(d=command,stroke = "black", fill="none", stroke_width = 0.5/SCALE)
    svg_entity.push_arc(target=tuple(endPoint),rotation=0,r=circle_R,large_arc=isLarge,angle_dir="-",absolute=True)
    return svg_entity
func_dict={"Line":line,"Circle":circle,"text":text,"PolyLine":polyline,"TrimmedCurve":trimmedcurve}
def Node(circle_center,circle_ref):
    circle_center[0]=float(circle_center[0])+float(circle_ref[0])
    circle_center[1]=MAX_HEIGHT-(float(circle_center[1])+float(circle_ref[1]))
    svg_entity = svgwrite.Drawing().circle(center=tuple(circle_center), r=1.5, stroke = "black", fill="none", stroke_width = 0.5/SCALE )
    #svg_entity.scale(SCALE,-SCALE)
    return svg_entity
def component(prop,shape):
    svg_entity=svgwrite.Drawing().g()
    svg_drawing=svgwrite.Drawing().g()
    for key in shape.keys():
        if key in func_dict:
            if (type(shape[key]).__name__=='OrderedDict'):
                svg_drawing.add(func_dict[key](shape[key],prop["Position"]))
            else:
                for i in range(len(shape[key])):
                    svg_drawing.add(func_dict[key](shape[key][i],prop["Position"]))
    connection=prop["ConnectionPoints"]
    assert int(connection["@NumPoints"])==len(connection["Node"])
    assert connection["Node"][0]["@Name"]=="Origin"
    origin=list(connection["Node"][0]["Position"]["Location"].values())[:2]
    for node in list(connection["Node"])[1:]:
        svg_drawing.add(Node(list(node["Position"]["Location"].values())[:2],origin))
    svg_entity.add(svg_drawing)
    svg_text=svgwrite.Drawing().g()
    svg_text.add(text(prop["Text"]))
    svg_entity.add(svg_text)
    return svg_entity


def drawing(dic):
    svg=svgwrite.Drawing(size=("420mm","297mm"),viewBox=("0 0 420 297"))
    if "Drawing" in dic["PlantModel"]:
        drawing=dic["PlantModel"]["Drawing"]
        svg_entity=svgwrite.Drawing().g()
        for key in drawing.keys():
            if key in func_dict:
                if (type(drawing[key]).__name__=='OrderedDict'):
                    svg_entity.add(func_dict[key](drawing[key]))
                else:
                    for i in range(len(drawing[key])):
                        svg_entity.add(func_dict[key](drawing[key][i]))
        svg.add(svg_entity)
    if "ShapeCatalogue" in dic["PlantModel"]:
        shapeCat=dic["PlantModel"]["ShapeCatalogue"]
        svg_entity=svgwrite.Drawing().g()
        for key in shapeCat.keys():
            if key=="@Name": 
                continue
            component_list=dic["PlantModel"][key]
            if (type(component_list).__name__=='OrderedDict'):
                component_list=[component_list]
            shape_list=shapeCat[key]
            if (type(shape_list).__name__=='OrderedDict'):
                shape_list=[shape_list]
            for element in component_list:
                for shape in shape_list:
                    if element["@ComponentName"]==shape["@ComponentName"]:
                        svg_entity.add(component(element,shape))
                        break
        svg.add(svg_entity)
    svg.saveas(PATH)
def main():
    xml_file=open("TEST.XML",'r',encoding='UTF-8')
    xml_str=xml_file.read()
    json_dict=xmltodict.parse(xml_str)
    drawing(json_dict)

if __name__=="__main__":
    main()