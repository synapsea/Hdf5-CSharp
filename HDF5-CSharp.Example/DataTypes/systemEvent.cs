using HDF5CSharp.DataTypes;
using System;
using System.Runtime.InteropServices;

namespace HDF5CSharp.Example.DataTypes
{
    public enum SystemEventType
    {
        NewV2RModelReady = 0,
        NewMeshReady = 1,
        Saturation = 2,
        MissAlignment = 3,
        PAQConnectionError = 4,
        PAQConnectionOk = 5,
        PAQHardwareConnectionError = 6,
        PAQHardwareConnectionOk = 7,
        Dummy = 8,
        ECGCycleDescription = 9,
        SheathDetected = 10,
        LeakAnalysis = 11,
        NetworkAvailabilityOn,
        NetworkAvailabilityOff,
        ECGBodyLeadConnected,
        ECGBodyLeadDisconnected,
        FreeSpace
    }

    public class SystemEventModel : IEquatable<SystemEventModel>
    {
        public SystemEventType SystemEventType { get; set; }
        public long TimeStamp { get; set; }
        public string EventData { get; set; }

        public SystemEventModel()
        {
            EventData = string.Empty;
        }

        public SystemEventModel(SystemEventType systemEventType, long timeStamp, string eventData)
        {
            SystemEventType = systemEventType;
            TimeStamp = timeStamp;
            EventData = eventData;
        }

        public bool Equals(SystemEventModel other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return SystemEventType == other.SystemEventType && TimeStamp == other.TimeStamp && EventData == other.EventData;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((SystemEvent)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)SystemEventType;
                hashCode = (hashCode * 397) ^ TimeStamp.GetHashCode();
                hashCode = (hashCode * 397) ^ (EventData != null ? EventData.GetHashCode() : 0);
                return hashCode;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SystemEvent
    {
        [Hdf5EntryName("timestamp")] public long timestamp;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        [Hdf5EntryName("type")] public string type;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        [Hdf5EntryName("description")] public string description;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        [Hdf5EntryName("data")] public string data;
        public SystemEvent(long timestamp, string type, string description, string data)
        {
            this.timestamp = timestamp;
            this.type = type;
            this.description = description;
            this.data = data;
        }
    }
}
