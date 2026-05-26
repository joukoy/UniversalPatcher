using J2534DotNet;

// Antus: Just a stub to satisfy build dependencies.
// @joukoy feel free to remove or replace this file.

namespace UniversalPatcher
{
    public class UPX_OBD : Elm327Device
    {
        public new const string DeviceType = "UPX-OBD";

        public UPX_OBD(IPort port) : base(port)
        {
            this.LogDeviceType = DataLogger.LoggingDevType.UPX_OBD;
        }

        public override string GetDeviceType()
        {
            return DeviceType;
        }
    }
}
