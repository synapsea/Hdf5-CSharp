using HDF.PInvoke;
using HDF5CSharp.DataTypes;
using System.Collections.Generic;

namespace HDF5CSharp
{
    public static partial class Hdf5
    {
        public static bool GroupExists(long groupId, string groupName) => Hdf5Utils.ItemExists(groupId, groupName, Hdf5ElementType.Group);

        public static int CloseGroup(long groupId)
        {
            return H5G.close(groupId);
        }

        public static long CreateOrOpenGroup(long fileOrGroupId, string groupName)
        {

            return (Hdf5Utils.ItemExists(fileOrGroupId, groupName, Hdf5ElementType.Group))
                ? H5G.open(fileOrGroupId, Hdf5Utils.NormalizedName(groupName))
                : H5G.create(fileOrGroupId, Hdf5Utils.NormalizedName(groupName));
        }




        /// <summary>
        /// creates a structure of groups at once
        /// </summary>
        /// <param name="groupOrFileId"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public static long CreateGroupRecursively(long groupOrFileId, string groupName)
        {
            IEnumerable<string> grps = groupName.Split('/');
            long gid = groupOrFileId;
            groupName = "";
            foreach (var name in grps)
            {
                groupName = string.Concat(groupName, "/", name);
                gid = CreateOrOpenGroup(gid, groupName);
            }
            return gid;
        }

        public static ulong NumberOfAttributes(int groupId, string groupName)
        {
            H5O.info_t info = new H5O.info_t();
            var gid = H5O.get_info(groupId, ref info);
            return info.num_attrs;
        }

        public static H5G.info_t GroupInfo(long groupId)
        {
            H5G.info_t info = new H5G.info_t();
            var gid = H5G.get_info(groupId, ref info);
            return info;
        }
    }
}
