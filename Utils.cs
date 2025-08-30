// Utility functions
// Example: pinning threads to specific CPU cores (important for HFT latency)
// This example works only on Windows (using kernel32.dll).
// On Linux, use sched_setaffinity via P/Invoke instead.

using System;
using System.Runtime.InteropServices;

public static class Utils {
    [DllImport("kernel32.dll")]
    static extern IntPtr GetCurrentThread();

    [DllImport("kernel32.dll")]
    static extern UIntPtr SetThreadAffinityMask(IntPtr hThread, UIntPtr dwThreadAffinityMask);

    // Pin current thread to a specific CPU core
    public static void PinThread(int cpuIndex) {
        var mask = (UIntPtr)(1u << cpuIndex);
        SetThreadAffinityMask(GetCurrentThread(), mask);
    }
}
