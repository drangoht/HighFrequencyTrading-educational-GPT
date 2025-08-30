// Lock-free Single Producer Single Consumer (SPSC) ring buffer
// - One thread enqueues (producer) -> the feed
// - One thread dequeues (consumer) -> the strategy
// This avoids locks by design: we only have one producer and one consumer.

using System;
using System.Runtime.CompilerServices;
using System.Threading;

public class SpscRing<T> where T : class {
    private readonly T[] _buf;    // circular buffer
    private readonly int _mask;   // for fast modulo (capacity must be power of 2)
    private long _head = 0; // producer writes head
    private long _tail = 0; // consumer writes tail

    public SpscRing(int capacityPowerOfTwo) {
        if ((capacityPowerOfTwo & (capacityPowerOfTwo - 1)) != 0)
            throw new ArgumentException("capacity must be power of two");
        _buf = new T[capacityPowerOfTwo];
        _mask = capacityPowerOfTwo - 1;
    }

    // Producer pushes data
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryEnqueue(T item) {
        var h = _head;
        var next = h + 1;

        // Check if buffer is full: head - tail == size
        if (next - _tail > _buf.Length)
            return false; // full, producer must retry

        // Write item
        _buf[h & _mask] = item;

        // Publish new head using interlocked operation for memory barrier
        Interlocked.Exchange(ref _head, next);
        return true;
    }

    // Consumer pulls data
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryDequeue(out T item) {
        var t = _tail;

        // Empty if tail == head
        if (t == _head) {
            item = null!;
            return false;
        }

        // Read item
        item = _buf[t & _mask];

        // Free slot
        _buf[t & _mask] = null!;

        // Publish new tail using interlocked operation
        Interlocked.Exchange(ref _tail, t + 1);
        return true;
    }
}
