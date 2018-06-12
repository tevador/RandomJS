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
const os = require('os');
const http = require('http');
const cluster = require('cluster');
const commandLineArgs = require('command-line-args');

const optionDefinitions = [
    { name: 'complexity', alias: 'c', type: Boolean },
    { name: 'maxlen', alias: 'm', type: Number },
    { name: 'port', alis: 'p', type: Number },
    { name: 'threads', alis: 't', type: Number  },
    { name: 'timeout', alias: 'o', type: Number },
  ];

const options = commandLineArgs(optionDefinitions);

if(cluster.isMaster) {
    console.log('[MASTER] online');
    console.log('[MASTER] options: ' + JSON.stringify(options));
    cluster.schedulingPolicy = cluster.SCHED_RR;
    for (let i = 0; i < (options.threads || os.cpus().length); ++i) {
        cluster.fork();
    }

    cluster.on('exit', (worker, code, signal) => {
        console.log("[MASTER] worker W%s exited with signal %s", worker.id, signal);
        cluster.fork();
    });

    cluster.on('online', (worker) => {
        console.log("[MASTER] Worker %s is online", worker.id);
    });

    if(options.complexity)
        console.log('[MASTER] Program complexity measurement is enabled.');
} else {

    const port = options.port || 18111;
    const maxScriptLength = options.maxlen || 10 * 1024 * 1024; //10 MiB
    const vmOptions = {
        displayErrors: true,
        timeout: options.timeout || 10000
    };

    function nanotime() {
        let hrtime = process.hrtime();
        return hrtime[0] + hrtime[1] * 1e-9;
    }

    let complexityEnabled = options.complexity;
    const escomplex = complexityEnabled ? require('typhonjs-escomplex') : null;

    http.createServer(function (request, response) {
        let headers = { 'Content-Type': 'text/plain' };
        if(request.method === "POST") {
            let source = '';
            request.on('data', function(data) {
                source += data;
                if(source.length > maxScriptLength) {
                    response.writeHead(413, 'Request Entity Too Large', headers);
                    response.end('Request Entity Too Large');
                }
            });
            request.on('end', function() {
                let output = '';
                const sandbox = {
                    print: function(x) {
                        output += x + '\n';
                    }
                };
                vm.createContext(sandbox);
                let success = true;
                let startTime = nanotime();
                try{
                    vm.runInContext(source, sandbox, vmOptions);
                } catch (error) {
                    sandbox.print('----------ERROR-----------');
                    sandbox.print(error);
                    console.log(error);
                    success = false;
                }
                let endTime = nanotime();
                let executionTime = endTime - startTime;
                console.log('Request script length: ' + source.length + ', Execution time: ' + executionTime);
                headers['X-Execution-Time'] = executionTime;
                headers['X-Success'] = success;
                if(complexityEnabled) {
                    let report = escomplex.analyzeModule(source, { loadDefaultPlugins: false });
                    headers['X-Complexity-Cyclomatic'] = report.methodAggregate.cyclomatic;
                    headers['X-Complexity-Halstead'] = report.methodAggregate.halstead.difficulty;
                    headers['X-Logical-Lines'] = report.methodAggregate.sloc.logical;
                }
                response.writeHead(200, headers);
                response.end(output);
            });
        } else {
            response.writeHead(405, 'Method Not Supported', headers);
            response.end('Method Not Supported');
        }
    }).listen(port, 'localhost', function() {
        console.log("[WORKER] Server listening on localhost:" + port);
    });
}