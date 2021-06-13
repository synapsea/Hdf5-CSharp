using System.Collections.Generic;
using HDF5CSharp.DataTypes;

namespace HDF5CSharp.Interfaces
{
    public interface IHdf5AcquisitionFile
    {
        Hdf5Patient Patient { get; set; }
        Hdf5Recording Recording { get; set; }
        Hdf5Channel[] Channels { get; set; }
        List<Hdf5Event> EventList { get; }
        Hdf5Events Events { get; }
    }
}
