# RandomJS
This is a concept implementation of a proof-of-work (PoW) algorithm proposal for Monero (but it's usable for any PoW cryptocurrency). Credits to @hyc for the original idea to use random javascript execution to achieve ASIC resistance.

### Key features
* __ASIC resistant__. This is important for a decentralized cryptocurrency and allows anyone with an ordinary computer to participate in securing the network. The algorithm internally uses the Google V8 Javascript engine, which is a large software package consisting of almost 2 million lines of code. Full hardware implementation would require an enormous investment.
* __Asymmetrical__. Finding a single solution takes roughly 1 second on a modern CPU core, while a valid solution can be verified in a few milliseconds. This is beneficial for mining pools by reducing their hardware requirements.
* __DoS resistant__. The algorithm stores an intermediate value in the block header, allowing quick verification whether the PoW meets the difficulty target. This requires just two Blake2b hash calculations (roughly 500 nanoseconds on a modern CPU). This is beneficial both for mining pools and network nodes in case someone wanted to flood them with invalid blocks.

A more detailed description will follow.

## Build dependencies and instructions
The concept of random javascript generator and miner presented here is written in C#. The generated javascript code is run externally in a NodeJS sandbox.

The project has 3 main units:

* __Tevador.RandomJS.exe__ - generates a random javascript program to STDOUT. Optional parameter is a numerical seed (signed 32-bit integer).
* __Tevador.RandomJS.Crypto.exe__ - runs the miner for about 60 seconds and shows the mining statistics. Optional parameter is a Monero block header template (152 hex characters).
* __sandbox.js__ - NodeJS sandbox for executing javascript. The sandbox must be running to use Tevador.RandomJS.Crypto.exe.

### Windows
#### Dependencies
1. Visual studio 2017 ([official download](https://www.visualstudio.com/downloads/))
1. NodeJS ([official download](https://nodejs.org/en/download))

#### Instructions
1. Build the solution in Visual studio.
1. Run the javascript sandbox in the command prompt: `node sandbox.js`.
1. Run `Tevador.RandomJS.exe` or `Tevador.RandomJS.Crypto.exe` in a separate command prompt.

### Linux
#### Dependencies
1. Mono 4.0+ ([via package manager](http://www.mono-project.com/download/stable/#download-lin)). For recent Ubuntu and Debian distros, install using `sudo apt-get install mono-complete`.
1. NodeJS ([via package manager](https://nodejs.org/en/download/package-manager/)). For recent Ubuntu and Debian distros, install using `curl -sL https://deb.nodesource.com/setup_8.x | sudo -E bash -` and `sudo apt-get install -y nodejs`

#### Instructions
1. Build with `make`.
1. Run the javascript sandbox: `node sandbox.js`.
1. Run `mono Tevador.RandomJS.exe` or `mono Tevador.RandomJS.Crypto.exe` in a separate terminal window.
