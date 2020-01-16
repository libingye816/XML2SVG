"""
Routes and views for the flask application.
"""

from datetime import datetime
from flask import render_template_string
from flask import url_for
from WebSearch import app
from flask import jsonify
baseURL="localhost:8080"

@app.route('/')
@app.route('/search/<key>/<value>',mehtods=['GET'])
def search(key,value):
    results=list(mongo.db.attributes.find({key:value},{"_id":0,"Name":1,"ID":1,"svgfile":1}))
    for result in results:
        result["url"]=baseURL+url_for('draw',name=result["svgfile"])
    return jsonify(results)

@app.route('/draw/<name>')
def draw(id):
    svgfile=list(mongo.db.svgfile.find({file:name}))
    assert len(svgfile)==1
    svgfile=svgfile[0]
    return render_template_string(
        svgfile["svg"]
    )

#@app.route('/about')
#def about():
#    """Renders the about page."""
#    return render_template(
#        'about.html',
#        title='About',
#        year=datetime.now().year,
#        message='Your application description page.'
#    )
