"""
The flask application package.
"""

from flask import Flask
app = Flask(__name__)
app.config["MONGO_URI"] = "mongodb://localhost:27017/testdb"
mongo = PyMongo(app)
import WebSearch.views
