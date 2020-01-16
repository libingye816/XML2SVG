import json
import xmltodict
import svgwrite
import os
import math
import pymongo

SCALE = 1.0
PATH = "output3.svg"
TEST = "TEST3.XML"
MAX_HEIGHT = 297
#Keys = ["Equipment", "PipingNetworkSystem", "ProcessInstrumentationFunction"]
key_ignore = ["PlantInformation","Extent","Drawing","ShapeCatalogue"]
myclient=pymongo.MongoClient("mongodb://localhost:27017")
mydb = myclient["testdb"]
attribute_col=mydb["attributes"]
name_col=mydb["names"]
source_col=mydb["source"]
svg_col=mydb["svgfile"]
if name_col.count_documents({})==0:
    name_col.insert_one({"ComosProperties":["SystemUID"]})

print("mongodb connected!")


def transform(pos, ref,ysign,scale):
    xscale=float(scale["@X"])
    yscale=float(scale["@Y"])
    cosine = float(ref[0]) / math.sqrt(float(ref[0]) ** 2 + float(ref[1]) ** 2)
    sine = float(ref[1]) / math.sqrt(float(ref[0]) ** 2 + float(ref[1]) ** 2)
    return [xscale*float(pos[0]) * cosine - yscale*float(pos[1]) * ysign * sine, xscale*float(pos[0]) * sine + yscale*float(pos[1]) * ysign * cosine]


def line(prop, pos=None, scale=None):
    if pos is None:
        pos = {"Location": {"x": 0., "y": 0.}, "Reference": {"x": 1., "y": 0.}}
    if scale is None:
        scale={"@X":1.0,"@Y":1.0}
    ysign=1
    if "Axis" in pos:
        ysign = int(pos["Axis"]["@Z"])
    line_ref = list(pos["Location"].values())[:2]
    line_start = transform(list(prop["Coordinate"][0].values())[:2], list(pos["Reference"].values()),ysign,scale)
    line_end = transform(list(prop["Coordinate"][1].values())[:2], list(pos["Reference"].values()),ysign,scale)
    line_start[0] = float(line_start[0]) + float(line_ref[0])
    line_start[1] = MAX_HEIGHT - (float(line_start[1]) + float(line_ref[1]))
    line_end[0] = float(line_end[0]) + float(line_ref[0])
    line_end[1] = MAX_HEIGHT - (float(line_end[1]) + float(line_ref[1]))
    svg_entity = svgwrite.Drawing().line(start=tuple(line_start), end=tuple(line_end),
                                         stroke="rgb({},{},{})".format(int(float(prop["Presentation"]["@R"]) * 255),int(float(prop["Presentation"]["@G"]) * 255),int(float(prop["Presentation"]["@B"]) * 255)),
                                         stroke_width=float(prop["Presentation"]["@LineWeight"]) / SCALE)
    #color:65536?
    #if scale is not None:
    #    xscale = float(scale["@X"])
    #    yscale = float(scale["@Y"])
    #    if xscale == 1.0 and yscale == 1.0:
    #        return svg_entity
    #    svg_entity.scale(xscale, yscale)
    #    svg_entity.translate(-(xscale - 1) / xscale * float(line_ref[0]),
    #                         -(yscale - 1) / yscale * (MAX_HEIGHT - float(line_ref[1])))
    return svg_entity


def circle(prop, pos=None, scale=None):
    if pos is None:
        pos = {"Location": {"x": 0., "y": 0.}, "Reference": {"x": 1., "y": 0.}}
    if scale is None:
        scale={"@X":1.0,"@Y":1.0}
    xscale=float(scale["@X"])
    yscale=float(scale["@Y"])
    ysign = 1
    if "Axis" in pos:
        ysign = int(pos["Axis"]["@Z"])
    circle_ref = list(pos["Location"].values())[:2]
    #circle_center = rotate(list(prop["Position"]["Location"].values())[:2], list(pos["Reference"].values()),ysign)
    circle_radius = float(prop["@Radius"])
    circle_center=list(prop["Position"]["Location"].values())[:2]
    circle_center[0] = float(circle_center[0])
    circle_center[1] = - (ysign*float(circle_center[1]))
    filled = "none"
    if "@Filled" in prop and prop["@Filled"] == "Solid":
        filled = "rgb({},{},{})".format(int(float(prop["Presentation"]["@R"]) * 255),int(float(prop["Presentation"]["@G"]) * 255),int(float(prop["Presentation"]["@B"]) * 255))
    svg_entity = svgwrite.Drawing().circle(center=tuple(circle_center), r=circle_radius, stroke="rgb({},{},{})".format(int(float(prop["Presentation"]["@R"]) * 255),int(float(prop["Presentation"]["@G"]) * 255),int(float(prop["Presentation"]["@B"]) * 255)), fill=filled,
                                           stroke_width=float(prop["Presentation"]["@LineWeight"]) / SCALE)
    # svg_entity.scale(SCALE,-SCALE)
    if xscale!=1.0 or yscale!=1.0:
        svg_entity.scale(xscale, yscale)
        #svg_entity.translate(-(xscale - 1) / xscale * float(circle_ref[0]),
        #                     -(yscale - 1) / yscale * (MAX_HEIGHT - float(circle_ref[1])))
    cosine = float(pos["Reference"]["@X"]) / math.sqrt(float(pos["Reference"]["@X"]) ** 2 + float(pos["Reference"]["@Y"]) ** 2)
    sine = float(pos["Reference"]["@Y"]) / math.sqrt(float(pos["Reference"]["@X"]) ** 2 + float(pos["Reference"]["@Y"]) ** 2)
    rotateAngle = math.degrees(math.asin(sine))
    if sine >= 0 and cosine < 0:
        rotateAngle = 180.0 - rotateAngle
    if sine < 0 and cosine < 0:
        rotateAngle = 540.0 - rotateAngle
    if rotateAngle!=0:
        svg_entity.rotate(360-rotateAngle)
    xMove=float(circle_ref[0])/xscale
    yMove=(MAX_HEIGHT-float(circle_ref[1]))/yscale
    svg_entity.translate(-yMove*sine+xMove*cosine,yMove*cosine+xMove*sine)
    return svg_entity


def text(prop,type="drawing"):
    # text_insert = [float(x) for x in
    # prop["Position"]["Location"].values()[:2]]
    # text_height = dxf_entity.dxf.height * 1.4 # hotfix - 1.4 to fit svg and
    # dvg
    if type == "label":
        assert prop["@Justification"] == "CenterCenter" or prop["@TextAngle"]=="0"
        xCoord = float(list(prop["Position"]["Location"].values())[0])-float(prop["@Width"])/2
        yCoord = float(list(prop["Position"]["Location"].values())[1])-float(prop["@Height"])/2
        angle = float(prop["@TextAngle"])
        while angle < 0:
            angle+=360
        while angle >= 360:
            angle-=360
        if angle > 90 and angle <= 270:
            angle-=180
        svg_entity = svgwrite.Drawing().text(prop["@String"], x=[xCoord], y=[MAX_HEIGHT - yCoord],
                                                 font_family=prop["@Font"], font_size=float(prop["@Height"]) * SCALE,
                                                 transform="rotate({} {},{})".format(-angle, prop["Position"]["Location"]["@X"],
                                                                                                           MAX_HEIGHT - float(prop["Position"]["Location"]["@Y"])),fill="rgb({},{},{})".format(int(float(prop["Presentation"]["@R"]) * 255),int(float(prop["Presentation"]["@G"]) * 255),int(float(prop["Presentation"]["@B"]) * 255)))
        
    elif type == "drawing":
        #"LeftTop"
        xCoord = float(list(prop["Position"]["Location"].values())[0])
        yCoord = float(list(prop["Position"]["Location"].values())[1]) - float(prop["@Height"])
        if prop["@Justification"] == "CenterTop":
            xCoord-=float(prop["@Width"]) / 2
        elif prop["@Justification"] == "CenterBottom":
            xCoord-=float(prop["@Width"]) / 2
            yCoord+=float(prop["@Height"])
        elif prop["@Justification"] == "LeftBottom":
            yCoord+=float(prop["@Height"])
        elif prop["@Justification"] == "RightBottom":
            xCoord-=float(prop["@Width"])
            yCoord+=float(prop["@Height"])
        elif prop["@Justification"] == "CenterCenter":
            xCoord -= float(prop["@Width"]) / 2
            yCoord += float(prop["@Height"]) / 2
        elif prop["@Justification"] == "RightTop":
            xCoord -= float(prop["@Width"])
        elif prop["@Justification"] == "RightCenter":
            xCoord -= float(prop["@Width"])
            yCoord += float(prop["@Height"]) / 2
        elif prop["@Justification"] == "LeftCenter":
            yCoord += float(prop["@Height"]) / 2
        if prop["@TextAngle"] == "0":
            svg_entity = svgwrite.Drawing().text(prop["@String"], x=[xCoord], y=[MAX_HEIGHT - yCoord],
                                                 font_family=prop["@Font"], font_size=float(prop["@Height"]) * SCALE, fill="rgb({},{},{})".format(int(float(prop["Presentation"]["@R"]) * 255),int(float(prop["Presentation"]["@G"]) * 255),int(float(prop["Presentation"]["@B"]) * 255)))
        elif int(float(prop["@TextAngle"])) % 360 == 270:
             #"CenterCenter"
            xMove = 0.
            yMove = 0.
            if prop["@Justification"] == "LeftTop":
                yMove = 2 * float(prop["@Height"])
            elif prop["@Justification"] == "RightTop":
                xMove = float(prop["@Width"])
            elif prop["@Justification"] == "CenterTop":
                yMove = float(prop["@Height"])
                xMove = float(prop["@Width"]) / 2
            elif prop["@Justification"] == "LeftBottom":
                xMove = -float(prop["@Width"])
            elif prop["@Justification"] == "RightBottom":
                yMove = -2 * float(prop["@Height"])
            elif prop["@Justification"] == "CenterBottom":
                yMove = -float(prop["@Height"])
                xMove = -float(prop["@Width"]) / 2
            elif prop["@Justification"] == "LeftCenter":
                xMove = -float(prop["@Width"]) / 2
                yMove = float(prop["@Height"])
            elif prop["@Justification"] == "RightCenter":
                xMove = float(prop["@Width"]) / 2
                yMove = -float(prop["@Height"])

            svg_entity = svgwrite.Drawing().text(prop["@String"], x=[xCoord], y=[MAX_HEIGHT - (yCoord)],
                                                     font_family=prop["@Font"], font_size=float(prop["@Height"]) * SCALE,
                                                     transform="rotate({} {},{}) translate({},{})".format(-float(prop["@TextAngle"]), float(prop["Position"]["Location"]["@X"]),
                                                                                                               MAX_HEIGHT - (float(prop["Position"]["Location"]["@Y"])),xMove,-yMove),fill="rgb({},{},{})".format(int(float(prop["Presentation"]["@R"]) * 255),int(float(prop["Presentation"]["@G"]) * 255),int(float(prop["Presentation"]["@B"]) * 255)))

        else:
            #"CenterCenter"
            xMove = 0.
            yMove = 0.
            if prop["@Justification"] == "LeftTop" or prop["@Justification"] == "RightTop" or prop["@Justification"] == "CenterTop":
                yMove+=float(prop["@Height"])
            elif prop["@Justification"] == "LeftBottom" or prop["@Justification"] == "RightBottom" or prop["@Justification"] == "CenterBottom":
                yMove-=float(prop["@Height"])

            svg_entity = svgwrite.Drawing().text(prop["@String"], x=[xCoord], y=[MAX_HEIGHT - (yCoord)],
                                                     font_family=prop["@Font"], font_size=float(prop["@Height"]) * SCALE,
                                                     transform="rotate({} {},{}) translate({},{})".format(-float(prop["@TextAngle"]), float(prop["Position"]["Location"]["@X"]),
                                                                                                               MAX_HEIGHT - (float(prop["Position"]["Location"]["@Y"])),xMove,-yMove),fill="rgb({},{},{})".format(int(float(prop["Presentation"]["@R"]) * 255),int(float(prop["Presentation"]["@G"]) * 255),int(float(prop["Presentation"]["@B"]) * 255)))

            # svg_entity.translate(text_insert[0]*(SCALE),
            # -text_insert[1]*(SCALE))
    return svg_entity


def polyline(prop, pos=None, scale=None, type="polyline"):
    if pos is None:
        pos = {"Location": {"x": 0., "y": 0.}, "Reference": {"x": 1., "y": 0.}}
    if scale is None:
        scale={"@X":1.0,"@Y":1.0}
    ysign = 1
    if "Axis" in pos:
        ysign = int(pos["Axis"]["@Z"])
    assert int(prop["@NumPoints"]) == len(prop["Coordinate"])
    poly_ref = list(pos["Location"].values())[:2]
    point_list = [list(x.values())[:2] for x in prop["Coordinate"]]
    point_list = [transform(x, list(pos["Reference"].values()),ysign,scale) for x in point_list]
    point_list = [(x[0] + float(poly_ref[0]),
                   MAX_HEIGHT - (x[1] + float(poly_ref[1]))) for x in point_list]
    if type == "polyline":
        svg_entity = svgwrite.Drawing().polyline(points=point_list, stroke="rgb({},{},{})".format(int(float(prop["Presentation"]["@R"]) * 255),int(float(prop["Presentation"]["@G"]) * 255),int(float(prop["Presentation"]["@B"]) * 255)), fill='none',
                                                 stroke_width=float(prop["Presentation"]["@LineWeight"]) / SCALE)
        # svg_entity.scale(SCALE, -SCALE)
        #if scale is not None:
        #    xscale = float(scale["@X"])
        #    yscale = float(scale["@Y"])
        #    if xscale == 1.0 and yscale == 1.0:
        #        return svg_entity
        #    svg_entity.scale(xscale, yscale)
        #    svg_entity.translate(-(xscale - 1) / xscale * float(poly_ref[0]),
        #                         -(yscale - 1) / yscale * (MAX_HEIGHT - float(poly_ref[1])))
    elif type == "flow":
        svg_entity = svgwrite.Drawing().g()
        if prop["Presentation"]["@LineType"] == "2":
            svg_entity.add(svgwrite.Drawing().polyline(points=point_list, stroke="rgb({},{},{})".format(int(float(prop["Presentation"]["@R"]) * 255),int(float(prop["Presentation"]["@G"]) * 255),int(float(prop["Presentation"]["@B"]) * 255)), fill='none',
                                                       stroke_width=float(prop["Presentation"]["@LineWeight"]) / SCALE,
                                                       stroke_dasharray="2,2"))
        else:
            svg_entity.add(svgwrite.Drawing().polyline(points=point_list, stroke="rgb({},{},{})".format(int(float(prop["Presentation"]["@R"]) * 255),int(float(prop["Presentation"]["@G"]) * 255),int(float(prop["Presentation"]["@B"]) * 255)), fill='none',
                                                       stroke_width=float(prop["Presentation"]["@LineWeight"]) / SCALE))
        start = list(point_list[0])
        start_next = point_list[1]
        r = 4.0
        start[0] = (start[0] * r + start_next[0]) / (r + 1)
        start[1] = (start[1] * r + start_next[1]) / (r + 1)
        end = point_list[-1]
        end_prev = point_list[-2]
        if start[0] == start_next[0]:
            svg_entity.add(svgwrite.Drawing().line(start=(start[0] - 0.6667, start[1]), end=(start[0], start[1] + 2 * (start_next[1] - start[1]) / abs(start_next[1] - start[1])), stroke="blue",
                                                   stroke_width=float(prop["Presentation"]["@LineWeight"]) / SCALE))
            svg_entity.add(svgwrite.Drawing().line(start=(start[0] + 0.6667, start[1]), end=(start[0], start[1] + 2 * (start_next[1] - start[1]) / abs(start_next[1] - start[1])), stroke="blue",
                                                   stroke_width=float(prop["Presentation"]["@LineWeight"]) / SCALE))
        elif start[1] == start_next[1]:
            svg_entity.add(svgwrite.Drawing().line(start=(start[0], start[1] - 0.6667), end=(start[0] + 2 * (start_next[0] - start[0]) / abs(start_next[0] - start[0]), start[1]), stroke="blue",
                                                   stroke_width=float(prop["Presentation"]["@LineWeight"]) / SCALE))
            svg_entity.add(svgwrite.Drawing().line(start=(start[0], start[1] + 0.6667), end=(start[0] + 2 * (start_next[0] - start[0]) / abs(start_next[0] - start[0]), start[1]), stroke="blue",
                                                   stroke_width=float(prop["Presentation"]["@LineWeight"]) / SCALE))
        if end[0] == end_prev[0]:
            svg_entity.add(svgwrite.Drawing().line(start=tuple(end), end=(end[0] + 0.6667, end[1] + 2 * (end_prev[1] - end[1]) / abs(end_prev[1] - end[1])), stroke="black",
                                                   stroke_width=float(prop["Presentation"]["@LineWeight"]) / SCALE))
            svg_entity.add(svgwrite.Drawing().line(start=tuple(end), end=(end[0] - 0.6667, end[1] + 2 * (end_prev[1] - end[1]) / abs(end_prev[1] - end[1])), stroke="black",
                                                   stroke_width=float(prop["Presentation"]["@LineWeight"]) / SCALE))
        elif start[1] == start_next[1]:
            svg_entity.add(svgwrite.Drawing().line(start=tuple(end), end=(end[0] + 2 * (end_prev[0] - end[0]) / abs(end_prev[0] - end[0]), end[1] + 0.6667), stroke="black",
                                                   stroke_width=float(prop["Presentation"]["@LineWeight"]) / SCALE))
            svg_entity.add(svgwrite.Drawing().line(start=tuple(end), end=(end[0] + 2 * (end_prev[0] - end[0]) / abs(end_prev[0] - end[0]), end[1] - 0.6667), stroke="black",
                                                   stroke_width=float(prop["Presentation"]["@LineWeight"]) / SCALE))
    return svg_entity


def trimmedcurve(prop, pos=None, scale=None):
    if pos is None:
        pos = {"Location": {"x": 0., "y": 0.}, "Reference": {"@X": 1., "@Y": 0.}}
    if scale is None:
        scale={"@X":1.0,"@Y":1.0}
    xscale=float(scale["@X"])
    yscale=float(scale["@Y"])
    ysign = 1
    if "Axis" in pos:
        ysign = int(pos["Axis"]["@Z"])
    cosine = float(pos["Reference"]["@X"]) / math.sqrt(float(pos["Reference"]["@X"]) ** 2 + float(pos["Reference"]["@Y"]) ** 2)
    sine = float(pos["Reference"]["@Y"]) / math.sqrt(float(pos["Reference"]["@X"]) ** 2 + float(pos["Reference"]["@Y"]) ** 2)
    rotateAngle = math.degrees(math.asin(sine))
    if sine >= 0 and cosine < 0:
        rotateAngle = 180.0 - rotateAngle
    if sine < 0 and cosine < 0:
        rotateAngle = 540.0 - rotateAngle
    startAngle = ysign * float(prop["@StartAngle"])
    endAngle = ysign * float(prop["@EndAngle"])
    
    assert "Circle" in prop
    circle_R = float(prop["Circle"]["@Radius"])
    #circle_center = rotate(list(prop["Circle"]["Position"]["Location"].values())[:2], list(pos["Reference"].values()),ysign)
    circle_ref = list(pos["Location"].values())[:2]
    circle_center=list(prop["Circle"]["Position"]["Location"].values())[:2]
    circle_center[0] = float(circle_center[0])
    circle_center[1] = float(circle_center[1])*ysign
    if abs(endAngle - startAngle) >= 360:
        circle_center[1] =  - circle_center[1]
        return svgwrite.Drawing().circle(center=tuple(circle_center), r=circle_R, stroke="rgb({},{},{})".format(int(float(prop["Presentation"]["@R"]) * 255),int(float(prop["Presentation"]["@G"]) * 255),int(float(prop["Presentation"]["@B"]) * 255)), fill="none",
                                         stroke_width=float(prop["Circle"]["Presentation"]["@LineWeight"]) / SCALE)
    isLarge = False
    if abs(endAngle - startAngle) > 180:
        isLarge = True

    startPoint = [0., 0.]
    startPoint[0] = circle_center[0] + math.cos(startAngle * math.pi / 180.0) * circle_R
    startPoint[1] = - (circle_center[1] + math.sin(startAngle * math.pi / 180.0) * circle_R)
    endPoint = [0., 0.]
    endPoint[0] = circle_center[0] + math.cos(endAngle * math.pi / 180.0) * circle_R
    endPoint[1] = - (circle_center[1] + math.sin(endAngle * math.pi / 180.0) * circle_R)
    command = ["M", tuple(startPoint)]
    svg_entity = svgwrite.Drawing().path(d=command, stroke="rgb({},{},{})".format(int(float(prop["Circle"]["Presentation"]["@R"]) * 255),int(float(prop["Circle"]["Presentation"]["@G"]) * 255),int(float(prop["Circle"]["Presentation"]["@B"]) * 255)), fill="none",
                                         stroke_width=float(prop["Circle"]["Presentation"]["@LineWeight"]) / SCALE)
    svg_entity.push_arc(target=tuple(endPoint), rotation=0, r=circle_R, large_arc=isLarge, angle_dir="-" if ysign > 0 else "+", absolute=True)
    #if scale is not None:
    #    xscale = float(scale["@X"])
    #    yscale = float(scale["@Y"])
    #    if xscale == 1.0 and yscale == 1.0:
    #        return svg_entity
    #    svg_entity.scale(xscale, yscale)
    #    svg_entity.translate(-(xscale - 1) / xscale * float(circle_ref[0]),
    #                         -(yscale - 1) / yscale * (MAX_HEIGHT - float(circle_ref[1])))
    if xscale!=1.0 or yscale!=1.0:
        svg_entity.scale(xscale, yscale)
    #if rotateAngle!=0:
    #    svg_entity.rotate(360-rotateAngle)
    xMove=float(circle_ref[0])/xscale
    yMove=(MAX_HEIGHT-float(circle_ref[1]))/yscale
    svg_entity.translate(-yMove*sine+xMove*cosine,yMove*cosine+xMove*sine)
    return svg_entity


def Node(circle_center, circle_ref):
    circle_center[0] = float(circle_center[0]) + float(circle_ref[0])
    circle_center[1] = MAX_HEIGHT - (float(circle_center[1]) + float(circle_ref[1]))
    svg_entity = svgwrite.Drawing().circle(center=tuple(circle_center), r=1.0, stroke="black", fill="none",
                                           stroke_width=0.1 / SCALE)
    # svg_entity.scale(SCALE,-SCALE)
    return svg_entity


def elementDraw(prop, shape):
    svg_entity = svgwrite.Drawing().g(id=prop["PersistentID"]["@Identifier"] + "_info")
    svg_drawing = svgwrite.Drawing().g(id=prop["PersistentID"]["@Identifier"] + "_shape")
    for key in shape.keys():
        if key in func_dict:
            if (type(shape[key]).__name__ == 'OrderedDict'):
                svg_drawing.add(func_dict[key](shape[key], prop["Position"], prop["Scale"]))
            else:
                for i in range(len(shape[key])):
                    svg_drawing.add(func_dict[key](shape[key][i], prop["Position"], prop["Scale"]))
    connection = prop["ConnectionPoints"]
    node_list = connection["Node"]
    if (type(connection["Node"]).__name__ == 'OrderedDict'):
        node_list = [node_list]
    assert int(connection["@NumPoints"]) == len(node_list)
    assert node_list[0]["@Name"] == "Origin"
    origin = list(node_list[0]["Position"]["Location"].values())[:2]
    for node in list(node_list)[1:]:
        svg_drawing.add(Node(list(node["Position"]["Location"].values())[:2], origin))
    svg_entity.add(svg_drawing)
    if "Text" in prop:
        svg_text = svgwrite.Drawing().g(id=prop["PersistentID"]["@Identifier"] + "_text")
        if (type(prop["Text"]).__name__ == 'OrderedDict'):
            svg_text.add(text(prop["Text"]))
        else:
            for label in list(prop["Text"]):
                svg_text.add(text(label,type="label"))
        svg_entity.add(svg_text)
    if "InformationFlow" in prop:
        if (type(prop["InformationFlow"]).__name__ == 'OrderedDict'):
            svg_entity.add(polyline(prop["InformationFlow"]["CenterLine"], type="flow"))
        else:
            for flow in list(prop["InformationFlow"]):
                svg_entity.add(polyline(flow["CenterLine"], type="flow"))

    component_attribute={"ID":prop["PersistentID"]["@Identifier"],"svgfile":PATH,"jsonfile":TEST}
    attributes = prop["GenericAttributes"]
    if (type(attributes).__name__ == 'OrderedDict'):
        attributes = [attributes]
    for attribute_set in attributes:
        name_list=list(name_col.find_one()[attribute_set["@Set"]]) if attribute_set["@Set"] in name_col.find_one() else []
        flag=False
        attribute_list=attribute_set["GenericAttribute"]
        if (type(attribute_list).__name__ == 'OrderedDict'):
            attribute_list = [attribute_list]
        for attribute in attribute_list:
            component_attribute[attribute["@Name"].replace('.','_').replace('$','%')]=str(attribute["@Value"])
            if attribute["@Name"] not in name_list:
                name_list.append(attribute["@Name"])
                flag=True
        if flag:
            name_col.update_one({},{"$set": { attribute_set["@Set"]: sorted(name_list) }})
    attribute_col.insert_one(component_attribute)
    print("component uploaded")
    return svg_entity


def componentDraw(key, shapeCat, component):
    svg_entity = svgwrite.Drawing().g(id=component["PersistentID"]["@Identifier"])
    if key in shapeCat:
        shape_list = shapeCat[key]
        if (type(shape_list).__name__ == 'OrderedDict'):
            shape_list = [shape_list]
        for shape in shape_list:
            if component["@ComponentName"] == shape["@ComponentName"]:
                svg_entity.add(elementDraw(component, shape))
                break
    for subkey in list(component.keys()):
        subcomponent_list = component[subkey]
        if subkey[0] == '@':
            continue
        if (type(subcomponent_list).__name__ == 'OrderedDict'):
            subcomponent_list = [subcomponent_list]
        for subcomponent in subcomponent_list:
            if "@ComponentName" in subcomponent:
                svg_entity.add(componentDraw(subkey, shapeCat, subcomponent))
    return svg_entity


def systemDraw(shapeCat, component):
    svg_entity = svgwrite.Drawing().g()
    for subkey in list(component.keys()):
        subcomponent_list = component[subkey]
        if subkey[0] == '@':
            continue
        if (type(subcomponent_list).__name__ == 'OrderedDict'):
            subcomponent_list = [subcomponent_list]
        if subkey == "PipeFlowArrow":
            for arrow in subcomponent_list:
                assert len(arrow["Line"]) == 2
                svg_entity.add(line(list(arrow["Line"])[0]))
                svg_entity.add(line(list(arrow["Line"])[1]))
            continue
        if subkey == "InformationFlow":
            for flow in subcomponent_list:
                svg_entity.add(polyline(flow["CenterLine"],type="flow"))
            continue
        if subkey == "CenterLine":
            for centerLine in subcomponent_list:
                svg_entity.add(polyline(centerLine))
            continue
        for subcomponent in subcomponent_list:
            if "@ComponentName" in subcomponent:
                svg_entity.add(componentDraw(subkey, shapeCat, subcomponent))
            elif "@ComponentClass" in subcomponent:
                svg_entity.add(systemDraw(shapeCat,subcomponent))
    return svg_entity


def drawingDraw(drawing):
    svg_entity = svgwrite.Drawing().g()
    for key in drawing.keys():
        if key in func_dict:
            if (type(drawing[key]).__name__ == 'OrderedDict'):
                svg_entity.add(func_dict[key](drawing[key]))
            else:
                for i in range(len(drawing[key])):
                    svg_entity.add(func_dict[key](drawing[key][i]))
        if key == "Label":
            if (type(drawing[key]).__name__ == 'OrderedDict'):
                svg_entity.add(drawingDraw(drawing[key]))
            else:
                for i in range(len(drawing[key])):
                    svg_entity.add(drawingDraw(drawing[key][i]))

    return svg_entity


def draw(dic):
    svg = svgwrite.Drawing(size=(dic["PlantModel"]["Extent"]["Max"]["@X"] + dic["PlantModel"]["PlantInformation"]["@Units"], dic["PlantModel"]["Extent"]["Max"]["@Y"] + dic["PlantModel"]["PlantInformation"]["@Units"]),
        viewBox=("0 0 {} {}".format(dic["PlantModel"]["Extent"]["Max"]["@X"], dic["PlantModel"]["Extent"]["Max"]["@Y"])))
    global MAX_HEIGHT
    MAX_HEIGHT = float(dic["PlantModel"]["Extent"]["Max"]["@Y"])
    if "Drawing" in dic["PlantModel"]:
        drawing = dic["PlantModel"]["Drawing"]
        svg.add(drawingDraw(drawing))
    if "ShapeCatalogue" in dic["PlantModel"]:
        shapeCat = dic["PlantModel"]["ShapeCatalogue"]
        for key in dic["PlantModel"]:
            if key not in key_ignore:
                    component_list = dic["PlantModel"][key]
                    if (type(component_list).__name__ == 'OrderedDict'):
                        component_list = [component_list]
                    for component in component_list:
                        if "@ComponentName" in component:
                            svg.add(componentDraw(key, shapeCat, component))
                        elif "@ComponentClass" in component:
                            svg.add(systemDraw(shapeCat,component))
                        
    return svg


def main():
    global func_dict
    func_dict = {"Line": line, "Circle": circle, "text": text, "PolyLine": polyline, "TrimmedCurve": trimmedcurve,
                 "Text": text}
    xml_file = open(TEST, 'r', encoding='UTF-8')
    xml_str = xml_file.read()
    json_dict = xmltodict.parse(xml_str)
    svg=draw(json_dict)
    if svg_col.count_documents({"file":PATH})==0:
        svg_col.insert_one({"file":PATH,"svg":svg.tostring()})
    svg.saveas(PATH)
    if source_col.count_documents({"file":TEST})==0:
        source_col.insert_one({"file":TEST,"json":json_dict})
    

if __name__ == "__main__":
    main()
