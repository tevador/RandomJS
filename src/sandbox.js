/*
    (c) 2018 tevador <tevador@gmail.com>

    This file is part of Tevador.RandomJS.

    Tevador.RandomJS is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Tevador.RandomJS is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Tevador.RandomJS.  If not, see<http://www.gnu.org/licenses/>.
*/

const vm = require('vm');
const http = require('http');

const port = 18111;
const maxScriptLength = 1024 * 1024; //1 MiB
const vmOptions = { 
    displayErrors: true,
    timeout: 1000
};

http.createServer(function (request, response) {
    if(request.method === "POST") {
        let source = '';
        request.on('data', function(data) {
            source += data;
            if(source.length > maxScriptLength) {
                response.writeHead(413, 'Request Entity Too Large', {'Content-Type': 'text/plain'});
                response.end('Request Entity Too Large');
            }
        });
        request.on('end', function() {
            console.log('Request script length: ' + source.length);
            const sandbox = { 
                console: {
                    output: '',
                    log: function(x) {
                        this.output += x + '\n';
                    }
                }
            };
            vm.createContext(sandbox);
            let status = 200;
            try{
                vm.runInContext(source, sandbox, vmOptions);
            } catch (error) {
                sandbox.console.output += error;
                status = 500;
            }
            response.writeHead(status, {'Content-Type': 'text/plain'});
            response.end(sandbox.console.output);
        });
    } else {
        response.writeHead(405, 'Method Not Supported', {'Content-Type': 'text/plain'});
        response.end('Method Not Supported');
    }
  }).listen(port, 'localhost', function() {
      console.log("server listening on localhost:" + port);
  });