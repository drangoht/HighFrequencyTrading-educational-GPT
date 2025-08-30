// Risk manager
// - Tracks current position
// - Enforces max position limit (both long and short)
// - Thread-safe with Interlocked

using System.Threading;

public class RiskManager {
    private readonly int _max; // maximum absolute position allowed
    private int _pos;          // current position

    public RiskManager(int maxPos) { _max = maxPos; }

    // Can we buy more? (not exceeding max position)
    public bool CanBuy(int qty) => (_pos + qty) <= _max;

    // Can we sell more? (not exceeding negative max position)
    public bool CanSell(int qty) => (_pos - qty) >= -_max;

    // Called when an order is filled
    public void OnFill(int delta) => Interlocked.Add(ref _pos, delta);

    public int Position => _pos;
}
