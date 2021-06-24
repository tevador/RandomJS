# Build dependencies and instructions

## C++ version

## Windows
#### Dependencies
1. Visual studio 2017 ([official download](https://www.visualstudio.com/downloads/))
1. Boost 1.67 ([installation and build instructions](https://www.boost.org/doc/libs/1_67_0/more/getting_started/windows.html))

#### Instructions
1. Clone repository https://github.com/tevador/moddable
1. Build the XS engine using the batch file `moddable\build\makefiles\win\build.bat`
1. Clone this repository.
1. Open and build the solution `RandomJS\src-cpp\randomjs.sln` in Visual studio.
1. Copy `moddable\build\bin\win\release\xst.exe` to the same directory as `randomjs.exe`. 

## Linux
#### Dependencies
1. GCC 5+ (tested with 6.3)
1. Boost 1.67 ([installation and build instructions](https://www.boost.org/doc/libs/1_67_0/more/getting_started/unix-variants.html))

#### Instructions
1. Clone repository https://github.com/tevador/moddable
1. Build the XS engine: `cd moddable/build/makefiles/lin && make`.
1. Clone this repository.
1. Build randomjs: `cd RandomJS/src-cpp && make`.
1. Copy `moddable/build/bin/lin/release/xst` to `RandomJS/src-cpp/bin`.

## C# version (testing only)

### Windows
#### Dependencies
1. Visual studio 2017 ([official download](https://www.visualstudio.com/downloads/))
1. NodeJS ([official download](https://nodejs.org/en/download))

#### Instructions
1. Build the solution in Visual studio.
1. Open the command prompt in the src directory.
1. Run `npm install`.
1. Run the javascript sandbox: `node sandbox.js`.
1. Run `Tevador.RandomJS.exe` or `Tevador.RandomJS.Miner.exe` in a separate command prompt.

### Linux
#### Dependencies
1. Mono 4.0+ ([via package manager](http://www.mono-project.com/download/stable/#download-lin)). For recent Ubuntu and Debian distros, install using `sudo apt-get install mono-devel`.
1. NodeJS ([via package manager](https://nodejs.org/en/download/package-manager/)). For recent Ubuntu and Debian distros, install using `curl -sL https://deb.nodesource.com/setup_8.x | sudo -E bash -` and `sudo apt-get install -y npm nodejs`

#### Instructions
1. Build with `make`.
1. Run `npm install`.
1. Run the javascript sandbox: `node sandbox.js`.
1. Run `mono Tevador.RandomJS.exe` or `mono Tevador.RandomJS.Miner.exe` in a separate terminal window.
