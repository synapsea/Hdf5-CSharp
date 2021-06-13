using HDF.PInvoke;
using HDF5CSharp.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace HDF5CSharp
{
    public static partial class Hdf5
    {
        private static Hdf5ReaderWriter attrRW = new Hdf5ReaderWriter(new Hdf5AttributeRW());
        public static Dictionary<string, List<string>> Attributes(Type type)
        {
            Dictionary<string, List<string>> attributes = new Dictionary<string, List<string>>();

            int attributeNum = 0;
            foreach (Attribute attr in Attribute.GetCustomAttributes(type))
            {
                switch (attr)
                {
                    case Hdf5EntryNameAttribute hdf5EntryNameAttribute:
                        attributes.Add($"attribute {attributeNum++}: {hdf5EntryNameAttribute.Name}", new List<string>() { hdf5EntryNameAttribute.Name });
                        break;
                    case Hdf5Attributes hdf5Attributes:
                        attributes.Add($"attribute {attributeNum++}", hdf5Attributes.Names.ToList());
                        break;
                    case Hdf5Attribute hdf5Attribute:
                        attributes.Add($"attribute {attributeNum++}", new List<string>() { hdf5Attribute.Name });
                        break;
                    case Hdf5KeyValuesAttributes hdf5KeyValuesAttribute:
                        attributes.Add(hdf5KeyValuesAttribute.Key, hdf5KeyValuesAttribute.Values.ToList());
                        break;
                }
            }
            return attributes;
        }
        public static Dictionary<string, List<string>> Attributes(PropertyInfo propertyInfo)
        {
            Dictionary<string, List<string>> attributes = new Dictionary<string, List<string>>();
            int attributeNum = 0;
            foreach (Attribute attr in Attribute.GetCustomAttributes(propertyInfo))
            {
                switch (attr)
                {
                    case Hdf5EntryNameAttribute hdf5EntryNameAttribute:
                        attributes.Add($"attribute {attributeNum++}: {hdf5EntryNameAttribute.Name}", new List<string>() { hdf5EntryNameAttribute.Name });
                        break;
                    case Hdf5Attributes hdf5Attributes:
                        attributes.Add($"attribute {attributeNum++}", hdf5Attributes.Names.ToList());
                        break;
                    case Hdf5Attribute hdf5Attribute:
                        attributes.Add($"attribute {attributeNum++}", new List<string>() { hdf5Attribute.Name });
                        break;
                    case Hdf5KeyValuesAttributes hdf5KeyValuesAttribute:
                        attributes.Add(hdf5KeyValuesAttribute.Key, hdf5KeyValuesAttribute.Values.ToList());
                        break;
                }
            }
            return attributes;
        }

        public static Dictionary<string, List<string>> Attributes(FieldInfo fieldInfo)
        {
            Dictionary<string, List<string>> attributes = new Dictionary<string, List<string>>();
            int attributeNum = 0;
            foreach (Attribute attr in Attribute.GetCustomAttributes(fieldInfo))
            {
                switch (attr)
                {
                    case Hdf5EntryNameAttribute hdf5EntryNameAttribute:
                        attributes.Add($"attribute {attributeNum++}: {hdf5EntryNameAttribute.Name}", new List<string>() { hdf5EntryNameAttribute.Name });
                        break;
                    case Hdf5Attributes hdf5Attributes:
                        attributes.Add($"attribute {attributeNum++}", hdf5Attributes.Names.ToList());
                        break;
                    case Hdf5Attribute hdf5Attribute:
                        attributes.Add($"attribute {attributeNum++}", new List<string>() { hdf5Attribute.Name });
                        break;
                    case Hdf5KeyValuesAttributes hdf5KeyValuesAttribute:
                        attributes.Add(hdf5KeyValuesAttribute.Key, hdf5KeyValuesAttribute.Values.ToList());
                        break;
                }
            }
            return attributes;
        }

        public static (bool success, Array result) ReadAttributes<T>(long groupId, string name)
        {
            return attrRW.ReadArray<T>(groupId, name, string.Empty);
        }
        public static bool AttributeExists(long groupId, string attributeName) => Hdf5Utils.ItemExists(groupId, attributeName, Hdf5ElementType.Attribute);
        public static T ReadAttribute<T>(long groupId, string name)
        {
            var attrs = attrRW.ReadArray<T>(groupId, name, string.Empty);
            if (!attrs.success)
            {
                Hdf5Utils.LogError?.Invoke($"{name} was not found");
                return default;
            }
            int[] first = new int[attrs.result.Rank].Select(f => 0).ToArray();
            T result = (T)attrs.result.GetValue(first);
            return result;
        }

        public static (bool success, IEnumerable<string> items) ReadStringAttributes(long groupId, string name, string alternativeName)
        {
            var nameToUse = Hdf5Utils.GetRealAttributeName(groupId, name, alternativeName);
            if (!nameToUse.valid)
            {
                Hdf5Utils.LogError?.Invoke($"Error reading {groupId}. Name:{name}. AlternativeName:{alternativeName}");
                return (false, Array.Empty<string>());
            }

            var datasetId = H5A.open(groupId, nameToUse.name);
            long typeId = H5A.get_type(datasetId);
            long spaceId = H5A.get_space(datasetId);
            long count = H5S.get_simple_extent_npoints(spaceId);
            H5S.close(spaceId);

            IntPtr[] rdata = new IntPtr[count];
            GCHandle hnd = GCHandle.Alloc(rdata, GCHandleType.Pinned);
            H5A.read(datasetId, typeId, hnd.AddrOfPinnedObject());

            var strs = new List<string>();
            for (int i = 0; i < rdata.Length; ++i)
            {
                if (rdata[i] == IntPtr.Zero)
                {
                    continue;
                }
                int len = 0;
                while (Marshal.ReadByte(rdata[i], len) != 0) { ++len; }
                byte[] buffer = new byte[len];
                Marshal.Copy(rdata[i], buffer, 0, buffer.Length);
                string s = Hdf5Utils.ReadStringBuffer(buffer);
                strs.Add(s);

                H5.free_memory(rdata[i]);
            }

            hnd.Free();
            H5T.close(typeId);
            H5A.close(datasetId);
            return (true, strs);
        }



        public static (bool success, Array result) ReadPrimitiveAttributes<T>(long groupId, string name, string alternativeName) //where T : struct
        {
            Type type = typeof(T);
            var datatype = GetDatatype(type);

            var attributeId = H5A.open(groupId, Hdf5Utils.NormalizedName(name));
            if (attributeId <= 0)
            {
                attributeId = H5A.open(groupId, Hdf5Utils.NormalizedName(alternativeName));
            }

            if (attributeId <= 0)
            {
                Hdf5Utils.LogError?.Invoke($"Error reading {groupId}. Name:{name}. AlternativeName:{alternativeName}");
                return (false, Array.Empty<T>());
            }
            var spaceId = H5A.get_space(attributeId);
            int rank = H5S.get_simple_extent_ndims(spaceId);
            ulong[] maxDims = new ulong[rank];
            ulong[] dims = new ulong[rank];
            long memId = H5S.get_simple_extent_dims(spaceId, dims, maxDims);
            long[] lengths = dims.Select(d => Convert.ToInt64(d)).ToArray();
            Array attributes = Array.CreateInstance(type, lengths);

            var typeId = H5A.get_type(attributeId);
            //var mem_type = H5T.copy(datatype);
            if (datatype == H5T.C_S1)
            {
                H5T.set_size(datatype, new IntPtr(2));
            }

            //var propId = H5A.get_create_plist(attributeId);
            //memId = H5S.create_simple(rank, dims, maxDims);
            GCHandle hnd = GCHandle.Alloc(attributes, GCHandleType.Pinned);
            H5A.read(attributeId, datatype, hnd.AddrOfPinnedObject());
            hnd.Free();
            H5T.close(typeId);
            H5A.close(attributeId);
            H5S.close(spaceId);
            return (true, attributes);
        }

        public static (int success, long attributeId) WriteStringAttribute(long groupId, string name, string val, string groupOrDatasetName)
        {
            return WriteStringAttributes(groupId, name, new[] { val }, groupOrDatasetName);
        }

        public static (int success, long CreatedId) WriteStringAttributes(long groupId, string name, IEnumerable<string> values, string groupOrDatasetName = null)
        {
            long tmpId = groupId;
            if (!string.IsNullOrWhiteSpace(groupOrDatasetName))
            {
                long datasetId = H5D.open(groupId, Hdf5Utils.NormalizedName(groupOrDatasetName));
                if (datasetId > 0)
                {
                    groupId = datasetId;
                }
            }
            else
            {

            }

            // create UTF-8 encoded attributes
            long datatype = H5T.create(H5T.class_t.STRING, H5T.VARIABLE);
            H5T.set_cset(datatype, Hdf5Utils.GetCharacterSet(Settings.CharacterSetType));
            H5T.set_strpad(datatype, Hdf5Utils.GetCharacterPadding(Settings.CharacterPaddingType));

            int strSz = values.Count();
            long spaceId = H5S.create_simple(1, new[] { (ulong)strSz }, null);
            string normalizedName = Hdf5Utils.NormalizedName(name);

            var attributeId = Hdf5Utils.GetAttributeId(groupId, normalizedName, datatype, spaceId);
            GCHandle[] hnds = new GCHandle[strSz];
            IntPtr[] wdata = new IntPtr[strSz];

            int cntr = 0;
            foreach (string str in values)
            {
                hnds[cntr] = GCHandle.Alloc(
                    Hdf5Utils.StringToByte(str),
                    GCHandleType.Pinned);
                wdata[cntr] = hnds[cntr].AddrOfPinnedObject();
                cntr++;
            }

            var hnd = GCHandle.Alloc(wdata, GCHandleType.Pinned);

            var result = H5A.write(attributeId, datatype, hnd.AddrOfPinnedObject());
            hnd.Free();

            for (int i = 0; i < strSz; ++i)
            {
                hnds[i].Free();
            }

            H5A.close(attributeId);
            H5S.close(spaceId);
            H5T.close(datatype);
            if (tmpId != groupId)
            {
                H5D.close(groupId);
            }
            return (result, attributeId);
        }

        public static (int success, long CreatedId) WriteAttribute<T>(long groupId, string name, T attribute) //where T : struct
        {
            return WriteAttributes<T>(groupId, name, new T[1] { attribute });
        }

        public static (int success, long CreatedId) WriteAttributes<T>(long groupId, string name, Array attributes) //
        {
            return attrRW.WriteArray(groupId, name, attributes, new Dictionary<string, List<string>>());
            //if (attributes.GetType().GetElementType() == typeof(string))
            //     WriteStringAttributes(groupId, name, attributes.Cast<string>(), attributeName);
            //else
            //    WritePrimitiveAttribute<T>(groupId, name, attributes, attributeName);
        }

        public static (int success, long CreatedgroupId) WritePrimitiveAttribute<T>(long groupId, string name, Array attributes) //where T : struct
        {
            var tmpId = groupId;
            int rank = attributes.Rank;
            ulong[] dims = Enumerable.Range(0, rank).Select(i => (ulong)attributes.GetLength(i)).ToArray();
            ulong[] maxDims = null;
            var spaceId = H5S.create_simple(rank, dims, maxDims);
            var datatype = GetDatatype(typeof(T));
            var typeId = H5T.copy(datatype);
            string nameToUse = Hdf5Utils.NormalizedName(name);
            //var attributeId = H5A.create(groupId, nameToUse, datatype, spaceId);
            var attributeId = Hdf5Utils.GetAttributeId(groupId, nameToUse, datatype, spaceId);
            GCHandle hnd = GCHandle.Alloc(attributes, GCHandleType.Pinned);
            var result = H5A.write(attributeId, datatype, hnd.AddrOfPinnedObject());
            hnd.Free();

            H5A.close(attributeId);
            H5S.close(spaceId);
            H5T.close(typeId);
            if (tmpId != groupId)
            {
                H5D.close(groupId);
            }
            return (result, attributeId);
        }
    }


}