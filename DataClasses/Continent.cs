using System.Collections.Generic;

namespace Editor.DataClasses;

public class Continent (string name, List<int> provinces)
{
    public string Name { get; } = name;
    public List<int> Provinces { get; set; } = provinces;

    public override bool Equals(object? obj)
    {
        if (obj is Continent other)
            return Name == other.Name;
        return false;
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}