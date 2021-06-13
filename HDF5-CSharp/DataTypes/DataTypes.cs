using HDF.PInvoke;
using System.Collections.Generic;
using System.Linq;

namespace HDF5CSharp.DataTypes
{
    public enum Hdf5ElementType
    {
        Unknown = 0,
        Group,
        Dataset,
        Attribute
    }

    public class Hdf5AttributeElement
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public Hdf5AttributeElement(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
    public abstract class Hdf5ElementBase
    {
        public string Name { get; set; }
        public Hdf5ElementType Type { get; set; }
        public bool IsLazyLoading { get; }

        public Hdf5ElementBase Parent { get; set; }
        public long Id { get; set; }
        protected bool HasLoadedOnce { get; set; }
        public abstract string GetPath();
        public abstract string GetDisplayName();
        public abstract IEnumerable<Hdf5ElementBase> GetChildren();
        protected abstract long GetId(long fileId);
        protected abstract void CloseId(long id);

        public Hdf5ElementBase(string name, Hdf5ElementType type, Hdf5ElementBase parent, long id, bool isLazyLoading)
        {
            Name = name;
            Type = type;
            Parent = parent;
            IsLazyLoading = isLazyLoading;
            Id = id;
        }

        public override string ToString() => $"{nameof(Name)}: {Name} ({Type}) ID:{Id}";
    }

    public class Hdf5Element : Hdf5ElementBase
    {
        private List<Hdf5Element> Children { get; }
        private List<Hdf5AttributeElement> Attributes { get; }
        public Hdf5Element(string name, Hdf5ElementType type, Hdf5ElementBase parent, List<Hdf5AttributeElement> attributes, long id, bool isLazyLoading) : base(name, type, parent, id, isLazyLoading)
        {
            Children = new List<Hdf5Element>();
            Attributes = attributes.ToList();
        }

        public bool HasChildren => Children.Any();

        public override string GetPath()
        {
            return Name;
        }

        public override string GetDisplayName()
        {
            return Name;
        }

        public override IEnumerable<Hdf5ElementBase> GetChildren()
        {
            return Children.ToList();
        }

        protected override long GetId(long fileId)
        {
            return H5G.open(fileId, GetPath());
        }

        protected override void CloseId(long id)
        {
            if (H5I.is_valid(id) > 0)
            {
                H5G.close(id);
            }
        }

        public void AddChild(Hdf5Element child)
        {
            Children.Add(child);
        }

     
        public Hdf5Element GetChildWithName(string childName)
        {
            var child = Children.FirstOrDefault(c => c.Name == childName);
            if (child != null)
            {
                return child;
            }

            var subChildren = Children.Where(c => c.HasChildren).Select(c => c.GetChildWithName(childName));
            return subChildren.FirstOrDefault();
        }
    }
}
