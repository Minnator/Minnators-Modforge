namespace Editor.DataClasses.GameDataClasses;

public class Culture(string name)
{
   public List<KeyValuePair<string, string>> CountryModifiers = [];
   public List<KeyValuePair<string, string>> ProvinceModifiers = [];
   public string Name = name;
   public string[] MaleNames = [];
   public string[] FemaleNames = [];
   public string[] DynastyNames = [];
   public List<Tag> Primaries = [];
   public Color Color = Color.Empty;
   public string CultureGroup = string.Empty;

   public int MaleNamesCount => MaleNames.Length;
   public int FemaleNamesCount => FemaleNames.Length;
   public int DynastyNamesCount => DynastyNames.Length;
   public int TotalNameCount => MaleNamesCount + FemaleNamesCount + DynastyNamesCount;

   public List<Province> GetProvinces()
   {
      List<Province> provinces = [];
      foreach (var prov in Globals.LandProvinces)
      {
         if (prov.Culture == Name)
            provinces.Add(prov);
      }

      return provinces;
   }

   public override string ToString()
   {
      return Name;
   }
}

public class CultureGroup(string name)
{
   public List<KeyValuePair<string, string>> CountryModifiers = [];
   public List<KeyValuePair<string, string>> ProvinceModifiers = [];
   public List<Culture> Cultures = [];
   public string Name = name;
   public string Gfx = string.Empty;
   public string SecondGfx = string.Empty;
   public Color Color = Color.Empty;
   public string[] MaleNames = [];
   public string[] FemaleNames = [];
   public string[] DynastyNames = [];

   public int MaleNamesCount => MaleNames.Length;
   public int FemaleNamesCount => FemaleNames.Length;
   public int DynastyNamesCount => DynastyNames.Length;
   public int TotalNameCount => MaleNamesCount + FemaleNamesCount + DynastyNamesCount;

   public List<Province> GetProvinces()
   {
      List<Province> provinces = [];
      foreach (var prov in Globals.LandProvinces)
      {
         if (Globals.Cultures.TryGetValue(prov.Culture, out var culture))
            if (Globals.CultureGroups.TryGetValue(culture.CultureGroup, out var cultureGroup))
               if (cultureGroup.Name == Name)
                  provinces.Add(prov);
      }

      return provinces;
   }

   public bool HasCultureWithName(string name)    
   {
      return Cultures.Any(culture => culture.Name == name);
   }
   public override string ToString()
   {
      return Name;
   }
}