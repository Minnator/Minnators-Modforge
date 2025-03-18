using Editor.DataClasses.Saveables;
using Editor.ErrorHandling;
using Editor.Helper;

namespace Editor.DataClasses.GameDataClasses;

public class Culture(string name) : IStringify
{
   public List<KeyValuePair<string, string>> CountryModifiers = [];
   public List<KeyValuePair<string, string>> ProvinceModifiers = [];
   public string Name { get; set; } = name;
   public string[] MaleNames = [];
   public string[] FemaleNames = [];
   public string[] DynastyNames = [];
   public List<Tag> Primaries = [];
   public Color Color = Color.Empty;
   public CultureGroup CultureGroup = CultureGroup.Empty;

   public int MaleNamesCount => MaleNames.Length;
   public int FemaleNamesCount => FemaleNames.Length;
   public int DynastyNamesCount => DynastyNames.Length;
   public int TotalNameCount => MaleNamesCount + FemaleNamesCount + DynastyNamesCount;
   public static Culture Empty { get; } = new ("unknown") { Color = Color.DimGray };
   public List<Province> GetProvinces()
   {
      List<Province> provinces = [];
      foreach (var prov in Globals.LandProvinces)
      {
         if (prov.Culture == this)
            provinces.Add(prov);
      }

      return provinces;
   }

   public static IErrorHandle GeneralParse(string? str, out object result)
   {
      var handle = TryParse(str, out var culture);
      result = culture;
      return handle;       
   }

   public static IErrorHandle TryParse(string input, out Culture culture)
   {
      if (string.IsNullOrEmpty(input))
      {
         culture = Empty;
         return new ErrorObject(ErrorType.TypeConversionError, "Could not parse culture!", addToManager: false);
      }

      if (!Globals.Cultures.TryGetValue(input, out culture))
      {
         culture = Empty;
         return new ErrorObject(ErrorType.TODO_ERROR, $"Culture \"{input}\" is already defined!",
            addToManager: false);
      }

      return ErrorHandle.Success;
   }

   public override string ToString()
   {
      return Name;
   }

   public string Stringify() => Name;
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

   public static CultureGroup Empty { get; }= new ("unknown"){ Color = Color.DimGray };

   public List<Province> GetProvinces()
   {
      List<Province> provinces = [];
      foreach (var prov in Globals.LandProvinces)
      {
         if (prov.Culture.CultureGroup == this)
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