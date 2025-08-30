# HFT C# Starter (Commented, Educational)

This is a highly commented educational starter project illustrating **High Frequency Trading (HFT)** concepts in C#.

## Features
- Lock-free Single Producer Single Consumer (SPSC) ring buffer
- Market feed simulator generating fake bid/ask ticks
- Simple naive strategy (buy then sell)
- Risk manager enforcing max position limits
- Optional thread pinning utility (Windows only)

## Build & Run

```bash
dotnet build
dotnet run
```

Output shows:
- Number of ticks processed
- Number of fills executed
- Final position

Usefull links:
- [LMAX Disruptor (concept)](https://github.com/LMAX-Exchange/disruptor)
- [Disruptor-net (C#)](https://github.com/disruptor-net/Disruptor-net)
- [joaoportela/CircularBuffer-CSharp](https://github.com/joaoportela/CircularBuffer-CSharp)
- [Introduction au HFT (Wikipedia FR)](https://fr.wikipedia.org/wiki/Trading_haute_fr%C3%A9quence)
## Disclaimer
This code is **for learning purposes only**. It is not suitable for real trading.
