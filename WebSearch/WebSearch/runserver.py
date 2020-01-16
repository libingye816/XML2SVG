"""
This script runs the WebSearch application using a development server.
"""

from os import environ
from WebSearch import app

if __name__ == '__main__':
    #HOST = environ.get('SERVER_HOST', 'localhost')
    #try:
    #    PORT = int(environ.get('SERVER_PORT', '8080'))
    #except ValueError:
    #    PORT = 5555
    app.run(host='localhost',port=8080,debug=True)
