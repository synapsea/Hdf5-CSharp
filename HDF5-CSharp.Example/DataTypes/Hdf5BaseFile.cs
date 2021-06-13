using HDF5CSharp.DataTypes;
using Microsoft.Extensions.Logging;
using System;

namespace HDF5CSharp.Example.DataTypes
{
    [Serializable]
    public abstract class Hdf5BaseFile
    {
        [Hdf5Save(Hdf5Save.DoNotSave)] private long FileId { get; }
        [Hdf5Save(Hdf5Save.DoNotSave)] protected long GroupRoot { get; }
        [Hdf5Save(Hdf5Save.DoNotSave)] protected string GroupName { get; }
        [Hdf5Save(Hdf5Save.DoNotSave)] protected long GroupId { get; }
        [Hdf5Save(Hdf5Save.DoNotSave)] protected bool Disposed { get; set; }
        [Hdf5Save(Hdf5Save.DoNotSave)] protected ILogger Logger { get; }
        protected Hdf5BaseFile(in long fileId, in long groupRoot, in string groupName, ILogger logger)
        {
            FileId = fileId;
            GroupRoot = groupRoot;
            GroupName = groupName;
            Logger = logger;
            GroupId = Hdf5.CreateOrOpenGroup(groupRoot, GroupName);

        }
        protected Hdf5BaseFile()
        {

        }

        public void FlushData()
        {
            if (Disposed)
            {
                return;
            }

            try
            {
                Hdf5.WriteObject(GroupRoot, this, GroupName);
                Hdf5.Flush(GroupId, HDF.PInvoke.H5F.scope_t.LOCAL);
            }
            catch (Exception e)
            {
                Logger?.LogError(e, $"Error FlushData: {e.Message}. Type: {GetType()}");
            }
        }
        public void FlushDataAndCloseObject()
        {
            try
            {
                if (Disposed)
                {
                    return;
                }

                FlushData();
                Hdf5.CloseGroup(GroupId);
            }
            catch (Exception e)
            {
                Logger?.LogError(e, $"Error FlushDataAndCloseObject: {e.Message}. Type: {GetType()}");
            }
        }
    }
}
