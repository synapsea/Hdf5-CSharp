using HDF.PInvoke;
using HDF5CSharp.DataTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace HDF5CSharp
{
    public partial class Hdf5
    {

        public static T ReadObject<T>(long groupId, T readValue, string groupName)
        {
            if (readValue == null)
            {
                throw new ArgumentNullException(nameof(readValue));
            }

            Type tyObject = readValue.GetType();
            //foreach (Attribute attr in Attribute.GetCustomAttributes(tyObject))
            //{
            //    if (attr is Hdf5GroupName)
            //        groupName = (attr as Hdf5GroupName).Name;
            //    if (attr is Hdf5SaveAttribute)
            //    {
            //        Hdf5SaveAttribute atLeg = attr as Hdf5SaveAttribute;
            //        if (atLeg.SaveKind == Hdf5Save.DoNotSave)
            //            return readValue;
            //    }
            //}
            bool isGroupName = !string.IsNullOrWhiteSpace(groupName);
            if (isGroupName)
            {
                groupId = H5G.open(groupId, Hdf5Utils.NormalizedName(groupName));
            }

            ReadFields(tyObject, readValue, groupId);
            ReadProperties(tyObject, readValue, groupId);

            if (isGroupName)
            {
                CloseGroup(groupId);
            }

            return readValue;
        }

        public static T ReadObject<T>(long groupId, string groupName) where T : new()
        {
            T readValue = new T();
            return ReadObject(groupId, readValue, groupName);
        }

        private static void ReadFields(Type tyObject, object readValue, long groupId)
        {
            FieldInfo[] miMembers = tyObject.GetFields(BindingFlags.DeclaredOnly |
                                                       /*BindingFlags.NonPublic |*/ BindingFlags.Public |
                                                       BindingFlags.Instance);

            foreach (FieldInfo info in miMembers)
            {
                bool nextInfo = false;
                string alternativeName = string.Empty;
                foreach (Attribute attr in Attribute.GetCustomAttributes(info))
                {
                    if (attr is Hdf5EntryNameAttribute nameAttribute)
                    {
                        alternativeName = nameAttribute.Name;
                    }

                    if (attr is Hdf5SaveAttribute attribute)
                    {
                        Hdf5Save kind = attribute.SaveKind;
                        nextInfo = (kind == Hdf5Save.DoNotSave);
                    }
                    else
                    {
                        nextInfo = false;
                    }
                }

                if (nextInfo)
                {
                    continue;
                }

                Type ty = info.FieldType;
                TypeCode code = Type.GetTypeCode(ty);

                string name = info.Name;
                Hdf5Utils.LogDebug?.Invoke($"groupname: {tyObject.Name}; field name: {name}");
                bool success;
                Array values;

                if (ty.IsArray)
                {
                    var elType = ty.GetElementType();
                    TypeCode elCode = Type.GetTypeCode(elType);

                    if (elCode != TypeCode.Object)
                    {
                        (success, values) = dsetRW.ReadArray(elType, groupId, name, alternativeName);
                    }
                    else
                    {
                        values = CallByReflection<Array>(nameof(ReadCompounds), elType,
                            new object[] {groupId, name, alternativeName});
                        success = true;
                    }

                    if (success)
                    {
                        info.SetValue(readValue, values);
                    }
                }

                else if (ty.IsGenericType && ty.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var elType = Hdf5Utils.GetEnumerableType(ty);
                    TypeCode elCode = Type.GetTypeCode(elType);
                    if (elCode != TypeCode.Object)
                    {
                        (success, values) = dsetRW.ReadArray(elType, groupId, name, alternativeName);
                        if (success)
                        {
                            Type genericClass = typeof(List<>);
                            // MakeGenericType is badly named
                            Type constructedClass = genericClass.MakeGenericType(elType);

                            IList created = (IList) Activator.CreateInstance(constructedClass);
                            foreach (var o in values)
                            {
                                created.Add(o);

                            }

                            info.SetValue(readValue, created);
                        }
                    }
                    else
                    {
                        var result = CallByReflection<object>(nameof(ReadCompounds), elType,
                            new object[] {groupId, name, alternativeName});
                        info.SetValue(readValue, result);

                    }




                }
                else if (primitiveTypes.Contains(code) || ty == typeof(TimeSpan))
                {
                    (success, values) = dsetRW.ReadArray(ty, groupId, name, alternativeName);
                    // get first value depending on rank of the matrix
                    int[] first = new int[values.Rank].Select(f => 0).ToArray();
                    if (success)
                    {
                        info.SetValue(readValue, values.GetValue(first));
                    }
                }
                else
                {
                    object value = info.GetValue(readValue);
                    if (value != null)
                    {
                        ReadObject(groupId, value, name);
                    }
                }
            }
        }

        private static void ReadProperties(Type tyObject, object readValue, long groupId)
        {
            PropertyInfo[] miMembers = tyObject.GetProperties( /*BindingFlags.DeclaredOnly |*/
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo info in miMembers)
            {
                bool nextInfo = false;
                string alternativeName = string.Empty;
                foreach (Attribute attr in Attribute.GetCustomAttributes(info))
                {
                    if (attr is Hdf5SaveAttribute hdf5SaveAttribute)
                    {
                        Hdf5Save kind = hdf5SaveAttribute.SaveKind;
                        nextInfo = (kind == Hdf5Save.DoNotSave);
                    }

                    if (attr is Hdf5EntryNameAttribute hdf5EntryNameAttribute)
                    {
                        alternativeName = hdf5EntryNameAttribute.Name;
                    }
                }

                if (nextInfo)
                {
                    continue;
                }

                Type ty = info.PropertyType;
                TypeCode code = Type.GetTypeCode(ty);
                string name = info.Name;

                bool success;
                Array values;
                if (ty.IsArray)
                {
                    var elType = ty.GetElementType();
                    TypeCode elCode = Type.GetTypeCode(elType);

                    if (elCode != TypeCode.Object || ty == typeof(TimeSpan[]))
                    {
                        (success, values) = dsetRW.ReadArray(elType, groupId, name, alternativeName);
                        if (success)
                        {
                            info.SetValue(readValue, values);
                        }
                    }
                    else
                    {
                        var obj = CallByReflection<IEnumerable>(nameof(ReadCompounds), elType,
                            new object[] {groupId, name, alternativeName});
                        var objArr = (obj).Cast<object>().ToArray();
                        values = Array.CreateInstance(elType, objArr.Length);
                        Array.Copy(objArr, values, objArr.Length);
                        info.SetValue(readValue, values);
                    }

                }
                else if (ty.IsGenericType && ty.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var elType = Hdf5Utils.GetEnumerableType(ty);
                    TypeCode elCode = Type.GetTypeCode(elType);
                    if (elCode != TypeCode.Object)
                    {
                        (success, values) = dsetRW.ReadArray(elType, groupId, name, alternativeName);
                        if (success)
                        {
                            Type genericClass = typeof(List<>);
                            // MakeGenericType is badly named
                            Type constructedClass = genericClass.MakeGenericType(elType);

                            IList created = (IList) Activator.CreateInstance(constructedClass);
                            foreach (var o in values)
                            {
                                created.Add(o);

                            }

                            info.SetValue(readValue, created);
                        }

                    }
                    else
                    {
                        var result = CallByReflection<object>(nameof(ReadCompounds), elType,
                            new object[] {groupId, name, alternativeName});
                        info.SetValue(readValue, result);
                    }
                }





                else if (primitiveTypes.Contains(code) || ty == typeof(TimeSpan))
                {
                    (success, values) = dsetRW.ReadArray(ty, groupId, name, alternativeName);
                    if (success && values.Length > 0)
                    {
                        int[] first = new int[values.Rank].Select(f => 0).ToArray();
                        if (info.CanWrite)
                        {
                            info.SetValue(readValue, values.GetValue(first));
                        }
                        else
                        {
                            Hdf5Utils.LogMessage($"property {info.Name} is read only. cannot set value",
                                Hdf5LogLevel.Warning);
                        }
                    }
                }
                else
                {
                    object value = info.GetValue(readValue, null);
                    if (value != null)
                    {
                        value = ReadObject(groupId, value, name);
                        info.SetValue(readValue, value);
                    }
                }
            }
        }

        public static List<Hdf5Element> ReadTreeFileStructure(string fileName)
        {
            return ReadFileStructure(fileName).tree;
        }

        public static List<Hdf5Element> ReadFlatFileStructure(string fileName)
        {
            return ReadFileStructure(fileName).flat;
        }

        internal static (List<Hdf5Element> tree, List<Hdf5Element> flat) ReadFileStructure(string fileName)
        {
            var attributes = new List<Hdf5AttributeElement>();
            var elements = new List<Hdf5Element>();
            var structure = new List<Hdf5Element>();
            if (!File.Exists(fileName))
            {
                Hdf5Utils.LogError?.Invoke($"File {fileName} does not exist");
                return (structure, elements);
            }

            long fileId = H5F.open(fileName, H5F.ACC_RDONLY);
            if (fileId < 0)
            {
                Hdf5Utils.LogError?.Invoke($"Could not open file {fileName}");
                return (structure, elements);
            }

            try
            {
                StringBuilder filePath = new StringBuilder(260);
                H5F.get_name(fileId, filePath, new IntPtr(260));
                ulong idx = 0;
                bool reEnableErrors = Settings.ErrorLoggingEnable;

                Settings.EnableErrorReporting(false);
                H5L.iterate(fileId, H5.index_t.NAME, H5.iter_order_t.INC, ref idx, Callback,
                    Marshal.StringToHGlobalAnsi("/"));
                Settings.EnableErrorReporting(reEnableErrors);


            }
            catch (Exception e)
            {
                Hdf5Utils.LogError?.Invoke($"Error during reading file structure of {fileName}. Error:{e}");
            }
            finally
            {
                if (fileId > 0)
                {
                    H5F.close(fileId);
                }
            }

            int Callback(long elementId, IntPtr intPtrName, ref H5L.info_t info, IntPtr intPtrUserData)
            {
                ulong idx2 = 0;
                long groupId = -1;
                long datasetId = -1;
                H5O.type_t objectType;
                var name = Marshal.PtrToStringAnsi(intPtrName);
                var userData = Marshal.PtrToStringAnsi(intPtrUserData);
                var fullName = CombinePath(userData, name);
                Hdf5ElementType elementType = Hdf5ElementType.Unknown;
                // this is necessary, since H5Oget_info_by_name is slow because it wants verbose object header data 
                // and H5G_loc_info is not directly accessible
                // only chance is to modify source code (H5Oget_info_by_name)
                groupId = (H5L.exists(elementId, name) >= 0) ? H5G.open(elementId, name) : -1L;
                if (H5I.is_valid(groupId) > 0)
                {

                    objectType = H5O.type_t.GROUP;
                    elementType = Hdf5ElementType.Group;
                    ulong attId = 0;
                    var index = H5A.iterate(groupId, H5.index_t.NAME, H5.iter_order_t.INC, ref attId, AttributeCallback,
                        Marshal.StringToHGlobalAnsi("/"));
                }
                else
                {
                    datasetId = H5D.open(elementId, name);
                    if ((H5I.is_valid(datasetId) > 0))
                    {
                        objectType = H5O.type_t.DATASET;
                        elementType = Hdf5ElementType.Dataset;
                        ulong attId = 0;
                        var index = H5A.iterate(groupId, H5.index_t.NAME, H5.iter_order_t.INC, ref attId,
                            AttributeCallback, Marshal.StringToHGlobalAnsi("/"));
                    }
                    else
                    {
                        objectType = H5O.type_t.UNKNOWN;
                        elementType = Hdf5ElementType.Group;
                    }
                }


                var parent = elements.FirstOrDefault(e =>
                {
                    var index = fullName.LastIndexOf("/", StringComparison.Ordinal);
                    var partial = fullName.Substring(0, index);
                    return partial.Equals(e.Name);

                });

                if (parent == null)
                {
                    var element = new Hdf5Element(fullName, elementType, null, attributes, elementId, false);
                    attributes.Clear();
                    structure.Add(element);
                    elements.Add(element);
                }
                else
                {
                    var element = new Hdf5Element(fullName, elementType, parent, attributes, elementId, false);
                    attributes.Clear();
                    parent.AddChild(element);
                    elements.Add(element);
                }

                if (objectType == H5O.type_t.GROUP)
                {
                    H5L.iterate(groupId, H5.index_t.NAME, H5.iter_order_t.INC, ref idx2, Callback,
                        Marshal.StringToHGlobalAnsi(fullName));
                }

                // clean up
                if (H5I.is_valid(groupId) > 0)
                {
                    H5G.close(groupId);
                }

                if (H5I.is_valid(datasetId) > 0)
                {
                    H5D.close(datasetId);
                }

                return 0;
            }

            int AttributeCallback(long location_id, IntPtr attr_name, ref H5A.info_t ainfo, IntPtr op_data)
            {
                var name = Marshal.PtrToStringAnsi(attr_name);

                var att = ReadStringAttributes(location_id, name, String.Empty);
                if (att.success && att.items.Any())
                {
                    attributes.Add(new Hdf5AttributeElement(name, att.items.First()));
                }


                //var typeId = H5A.get_type(location_id);
                //H5T.class_t classType = H5T.get_class(location_id);
                //var cset = H5T.get_cset(location_id);
                //var padd = H5T.get_strpad(location_id);
                return 0;

            }

            return (structure, elements);
        }


        private static string CombinePath(string path1, string path2)
        {
            return $"{path1}/{path2}".Replace("///", "/").Replace("//", "/");
        }

        public static TabularData<T> Read2DTable<T>(long groupId, string datasetName) where T : new()
        {
            TabularData<T> table = new TabularData<T>();

            Type ty = typeof(T[,]);
            bool success;
            Array values;
            if (ty.IsArray)
            {
                var elType = ty.GetElementType();
                TypeCode elCode = Type.GetTypeCode(elType);

                if (elCode != TypeCode.Object || ty == typeof(TimeSpan[]))
                {
                    (success, values) = dsetRW.ReadArray(elType, groupId, datasetName, "");
                    if (success)
                    {
                        table.Data = (T[,]) values;
                    }
                }
                else
                {
                    var obj = CallByReflection<IEnumerable>(nameof(ReadCompounds), elType,
                        new object[] {groupId, datasetName, ""});
                    var objArr = (obj).Cast<object>().ToArray();
                    values = Array.CreateInstance(elType, objArr.Length);
                    Array.Copy(objArr, values, objArr.Length);
                    table.Data = (T[,]) values;
                }

            }

            return table;
        }
    }
}
