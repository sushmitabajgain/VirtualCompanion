using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public static class XRReady
{
    // A static list to hold references to XR display subsystems (e.g., VR display drivers)
    static readonly List<XRDisplaySubsystem> _displays = new();

    // Method to check if any XR display subsystem is currently running
    public static bool Running()
    {
        // Clear the list to remove old references
        _displays.Clear();

        // Populate the list with all available XR display subsystems
        SubsystemManager.GetSubsystems(_displays);

        // Return true if any XR display subsystem is running, false otherwise
        return _displays.Exists(d => d.running);
    }
}
