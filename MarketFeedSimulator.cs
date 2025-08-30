// Market feed simulator
// - Generates artificial ticks (bid/ask prices around a mid price)
// - Pushes them into the ring buffer
// - Runs in its own thread

using System;
using System.Threading;

// A Tick represents a market data update (simplified)
public class Tick {
    public double Bid { get; set; }
    public double Ask { get; set; }
    public DateTime Ts { get; set; }
}

public class MarketFeedSimulator {
    private readonly SpscRing<Tick> _ring;
    private readonly Random _rng = new Random(42);
    private double _mid = 100.0; // starting price

    public MarketFeedSimulator(SpscRing<Tick> ring) {
        _ring = ring;
    }

    // Run until runFlag() returns false
    public void Run(Func<bool> runFlag) {
        while (runFlag()) {
            // Random walk for mid price
            _mid += (_rng.NextDouble() - 0.5) * 0.01;

            // Create tick with small bid/ask spread
            var tick = new Tick {
                Ts = DateTime.UtcNow,
                Bid = _mid - 0.005,
                Ask = _mid + 0.005
            };

            // Keep retrying if buffer is full
            while (!_ring.TryEnqueue(tick)) {
                Thread.SpinWait(1); // avoid busy waiting too hard
            }

            // Control frequency: ~50k ticks per second
            Thread.SpinWait(20);
        }
    }
}
