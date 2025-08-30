// Strategy logic
// - Consumes ticks from the ring buffer
// - Implements a naive strategy: always tries to buy 1 then sell 1 (ping-pong)
// - Consults risk manager before trading
// - Updates counters for analysis

using System;
using System.Threading;

public class Strategy {
    private readonly SpscRing<Tick> _ring;
    private readonly RiskManager _risk;

    // Metrics
    public int TickCount { get; private set; }
    public int FillCount { get; private set; }

    public Strategy(SpscRing<Tick> ring, RiskManager risk) {
        _ring = ring;
        _risk = risk;
    }

    // Main loop: runs in its own thread
    public void Run(Func<bool> runFlag) {
        while (runFlag()) {
            if (_ring.TryDequeue(out var tick)) {
                TickCount++;

                // Compute mid price
                var mid = (tick.Bid + tick.Ask) * 0.5;

                // Example: simple "always trade" strategy
                // Try to buy 1 unit
                if (_risk.CanBuy(1)) {
                    _risk.OnFill(+1);
                    FillCount++;
                }

                // Try to sell 1 unit
                if (_risk.CanSell(1)) {
                    _risk.OnFill(-1);
                    FillCount++;
                }
            } else {
                // Nothing in queue, spin a little
                Thread.SpinWait(1);
            }
        }
    }
}
