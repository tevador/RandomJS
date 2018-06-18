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

process.stdin.setEncoding('ascii');
process.stdout.setEncoding('ascii');

let __source = '';

function print(x) {
  process.stdout.write(x + "\n");
}

function runScript() {
  eval(__source);
  process.stdout.write("\0");
  __source = '';
}

process.stdin.on('readable', () => {
  const chunk = process.stdin.read();
  if (chunk !== null) {
    __source += chunk;
    if(__source.slice(-1) === "\0") {
      __source = __source.substr(0, __source.length - 1);
      runScript();
    }
  }
});

process.stdin.on('end', () => {
  runScript();
  process.exit();
});