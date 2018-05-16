# RandomJS
This is a concept implementation of a proof-of-work (PoW) algorithm proposal for Monero (but it's usable for any PoW cryptocurrency). Credits to @hyc for the original idea to use random javascript execution to achieve ASIC resistance.

### Key features
* __ASIC resistant__. This is important for a decentralized cryptocurrency and allows anyone with an ordinary computer to participate in securing the network. The algorithm internally uses the Google V8 Javascript engine, which is a large software package consisting of almost 2 million lines of code. Full hardware implementation would require an enormous investment.
* __Asymmetrical__. Finding a single solution takes roughly 1 second¹ on a modern CPU core, while a valid solution can be verified in a few milliseconds¹. This is beneficial for mining pools by reducing their hardware requirements.
* __DoS resistant__. The algorithm stores an intermediate value in the block header, allowing quick verification whether the PoW meets the difficulty target. This requires just two Blake2b hash calculations (roughly 500 nanoseconds on a modern CPU). This is beneficial both for mining pools and network nodes in case someone wanted to flood them with invalid blocks.

¹ These performance numbers are not final as the javascript generation algorithm is still in development.

## Algorithm description

### Cryptographic hash function
The primary general-purpose hash function used by RandomJS is Blake2b with output size of 256 bits. This hash function was chosen for 3 primary reasons:

1. __Security.__ Security margin of Blake2b is comparable to the SHA-3 hash function.
1. __Speed.__ Blake2b was specifically designed to be fast in software, especially on modern 64-bit processors, where it's around three times faster than SHA-3.
1. __Built-in keyed mode.__ RandomJS requires both a plain hash function and a keyed hash function.

In the description below, `Blake2b(X)` and `Blake2b(K,X)` refer to the plain and keyed variant of Blake2b, respectively (both with 256-bit output length). `X[0]` refers to the left-most octet of a binary string `X`.

### Mining algorithm
1. Get a block header `H`.
1. Calculate `K = Blake2b(H)`.
1. Generate a random Javascript program `P` using `K` as the seed.
1. Calculate `A = Blake2b(K, P)`.
1. Execute program `P` and capture its output `Q`.
1. Calculate `B = Blake2b(K, Q)`.
1. If `A[0] != B[0]`, go back to step 1.
1. Set `B[0] = 0`.
1. Calculate `R = (A XOR B)`.
1. Calculate `PoW = Blake2b(K, R)`.
1. If `PoW` doesn't meet the difficulty target, go back to step 1.
1. Submit `H, R` as the result to be included in the block.

Finding and verifying a solution takes on average 769 Blake2b hash calculations, 256 random javascript program generations and 256 javascript executions.

### Verification algorithm
Input: `H, R` from the received block.
1. Calculate `K = Blake2b(H)`.
1. Calculate `PoW = Blake2b(K, R)`.
1. If `PoW` doesn't meet the difficulty target, _discard the block_.
1. Generate a random Javascript program `P` using `K` as the seed.
1. Calculate `A = Blake2b(K, P)`.
1. If `A[0] != R[0]`, _discard the block_.
1. Set `A[0] = 0`.
1. Execute program `P` and capture its output `Q`.
1. Calculate `B = Blake2b(K, Q)`.
1. If `R != (A XOR B)`, _discard the block_.

Verifying a valid solution requires 4 Blake2b hash calculations, one random program generation and one javascript execution.

In case of an DoS attack attempt, just 2 Blake2b hash calculations are required to discard the block. Otherwise an attacker needs to calculate substantial number of Blake2b hashes (equal to the current block difficulty) to force the verifying party to run the relatively costly (~few milliseconds) javascript generation and execution procedure.

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
1. Mono 4.0+ ([via package manager](http://www.mono-project.com/download/stable/#download-lin)). For recent Ubuntu and Debian distros, install using `sudo apt-get install mono-devel`.
1. NodeJS ([via package manager](https://nodejs.org/en/download/package-manager/)). For recent Ubuntu and Debian distros, install using `curl -sL https://deb.nodesource.com/setup_8.x | sudo -E bash -` and `sudo apt-get install -y nodejs`

#### Instructions
1. Build with `make`.
1. Run the javascript sandbox: `node sandbox.js`.
1. Run `mono Tevador.RandomJS.exe` or `mono Tevador.RandomJS.Crypto.exe` in a separate terminal window.
