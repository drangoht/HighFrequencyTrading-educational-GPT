// Entry point of the program
// This file wires everything together: the market feed simulator, the strategy, and the risk manager.
// It demonstrates how an HFT engine skeleton may be orchestrated with threads and a lock-free queue.

using System;
using System.Threading;

class Program {
    static void Main() {
        Console.WriteLine("HFT C# Starter — demo running for 2s...");

        // Create a Single-Producer-Single-Consumer ring buffer to carry ticks (market data) from feed to strategy
        var ring = new SpscRing<Tick>(1 << 14); // capacity: 16384

        // Risk manager prevents us from exceeding max position
        var risk = new RiskManager(10);

        // Strategy consumes ticks, decides trades, and sends orders to the risk manager
        var strat = new Strategy(ring, risk);

        // Market feed simulator generates fake ticks (bid/ask) and pushes them into the ring buffer
        var feed = new MarketFeedSimulator(ring);

        // Shared boolean flag to stop threads gracefully
        var run = true;

        // Thread running the feed
        var feedThread = new Thread(() => feed.Run(() => run));

        // Thread running the strategy
        var stratThread = new Thread(() => strat.Run(() => run));

        // Start both threads
        feedThread.Start();
        stratThread.Start();

        // Let them run for ~2 seconds
        Thread.Sleep(2000);
        run = false;

        // Wait for threads to stop
        feedThread.Join();
        stratThread.Join();

        // Print results
        Console.WriteLine($"Ticks processed: {strat.TickCount}");
        Console.WriteLine($"Fills: {strat.FillCount}");
        Console.WriteLine($"Final position: {risk.Position}");
    }
}
