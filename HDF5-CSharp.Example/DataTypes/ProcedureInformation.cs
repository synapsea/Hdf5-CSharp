using HDF5CSharp.DataTypes;
using Microsoft.Extensions.Logging;
using System;

namespace HDF5CSharp.Example.DataTypes
{
    [Hdf5Attributes(new[] { "General information of the procedure" })]
    [Hdf5GroupName("procedure_information")]
    public class ProcedureInformation : Hdf5BaseFile, IEquatable<ProcedureInformation>
    {
        [Hdf5EntryName("procedure_directory")]
        public string ProcedureDirectory { get; set; }
        [Hdf5("the type of procedure")]
        [Hdf5EntryName("procedure_type")]
        public string ProcedureType { get; set; }
        [Hdf5EntryName("start_datetime")]
        public DateTime StartDateTime { get; set; }
        [Hdf5EntryName("end_datetime")]
        public DateTime EndDateTime { get; set; }
        [Hdf5EntryName("procedure_id")]
        public string ProcedureID { get; set; }

        public ProcedureInformation(long fileId, long groupRoot, ILogger logger) : base(fileId, groupRoot, "procedure_information", logger)
        {
            ProcedureDirectory = string.Empty;
            ProcedureType = string.Empty;
        }

        public ProcedureInformation()
        {

        }

        public bool Equals(ProcedureInformation other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return ProcedureDirectory == other.ProcedureDirectory && ProcedureType == other.ProcedureType &&
                   StartDateTime.EqualsUpToMilliseconds(other.StartDateTime) && EndDateTime.EqualsUpToMilliseconds(other.EndDateTime) &&
                   ProcedureID == other.ProcedureID;

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

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((ProcedureInformation)obj);
        }

        public override int GetHashCode()
        {
            return (ProcedureDirectory.GetHashCode() * 397) ^ (ProcedureType.GetHashCode() * 397) ^
                   (StartDateTime.GetHashCode() * 397) ^ (EndDateTime.GetHashCode() * 397) ^
                   (ProcedureID.GetHashCode() * 397);
        }
    }
}
