using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using KSP.UI.Screens;


namespace PART_ORGANIZER {


public static class Lib
{
  // get a texture
  public static Texture2D GetTexture(string url)
  {
    return GameDatabase.Instance.GetTexture(url, false);
  }


  // get a config node from the config system
  public static ConfigNode ParseConfig(string path)
  {
    return GameDatabase.Instance.GetConfigNode(path) ?? new ConfigNode();
  }


  // get a set of config nodes from the config system
  public static ConfigNode[] ParseConfigs(string path)
  {
    return GameDatabase.Instance.GetConfigNodes(path);
  }


  // get a value from config node
  public static T ConfigValue<T>(ConfigNode cfg, string key, T def_value)
  {
    try
    {
      return cfg.HasValue(key) ? (T)Convert.ChangeType(cfg.GetValue(key), typeof(T)) : def_value;
    }
    catch(Exception e)
    {
      Log("error while trying to parse '" + key + "' from " + cfg.name + " (" + e.Message + ")");
      return def_value;
    }
  }


  // write a message to the log
  public static void Log(string msg)
  {
    MonoBehaviour.print("[PartOrganizer] " + msg);
  }
}


public class Category
{
  public Category(ConfigNode node)
  {
    this.name           = Lib.ConfigValue(node, "name",           string.Empty);
    this.tag            = Lib.ConfigValue(node, "tag",            string.Empty);
    this.icon           = Lib.ConfigValue(node, "icon",           string.Empty);
    this.tooltip        = Lib.ConfigValue(node, "tooltip",        string.Empty);
  }

  public string name;               // unique name for the category
  public string tag;                // used to identify parts that belong to this category
  public string icon;               // url to icon used for the category
  public string tooltip;            // tooltip shown when hovering the icon
}


[KSPAddon(KSPAddon.Startup.MainMenu, true)]
public sealed class PartOrganizer : MonoBehaviour
{
  PartOrganizer()
  {
    // keep it alive
    DontDestroyOnLoad(this);
  }


  public void Start()
  {
    // log version
    Lib.Log("version " + Assembly.GetExecutingAssembly().GetName().Version);

    // parse config
    ConfigNode[] category_nodes = Lib.ParseConfigs("EditorCategory");
    foreach(ConfigNode category_node in category_nodes)
    {
      // parse category config
      Category cat = new Category(category_node);

      // ignore categories without name
      if (cat.name.Length == 0)
      {
        Lib.Log("ignoring unnamed category");
        continue;
      }

      // ignore categories without a tag
      if (cat.tag.Length == 0)
      {
        Lib.Log("category '" + cat.name + "' has no tag");
        continue;
      }

      // ignore categories without an icon, or that refer to a non-existent icon
      if (cat.icon.Length == 0 || Lib.GetTexture(cat.icon) == null)
      {
        Lib.Log("category '" + cat.name + "' doesn't speficy an icon, or the icon don't exist");
        continue;
      }

      // ignore categories for which no part exist
      if (PartLoader.LoadedPartsList.Find(k => k.tags.IndexOf(cat.tag, StringComparison.Ordinal) >= 0) == null)
      {
        Lib.Log("no parts for category '" + cat.name + "'");
        continue;
      }

      // finally, store the category
      categories.Add(cat);
    }

    // add callback on editor toolbar
    GameEvents.onGUIEditorToolbarReady.Add(this.setup);
  }


  void setup()
  {
    foreach(Category cat in categories)
    {
      // get textures
      Texture2D normal = Lib.GetTexture(cat.icon);
      Texture2D selected = Lib.GetTexture(cat.icon + "_selected");
      selected = (selected == null ? normal : selected);

      // create selectable icon
      var icon = new RUI.Icons.Selectable.Icon(cat.name, normal, selected);

      // get main category
      PartCategorizer.Category main = PartCategorizer.Instance.filters.Find(k => k.button.categoryName == "Filter by Function");

      // add custom subcategory
      PartCategorizer.AddCustomSubcategoryFilter(main, cat.tooltip, icon, k => k.tags.IndexOf(cat.tag, StringComparison.Ordinal) >= 0);
    }
  }


  List<Category> categories = new List<Category>();   // the set of custom categories

}



} // PART_ORGANIZER