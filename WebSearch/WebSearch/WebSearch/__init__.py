"""
The flask application package.
"""
from flask_cors import CORS
from flask import Flask
from flask_pymongo import PyMongo
app = Flask(__name__,static_folder='static')
CORS(app,supports_credentials=True)
app.config["MONGO_URI"] = "mongodb://localhost:27017/testdb"
mongo = PyMongo(app)
import WebSearch.views
