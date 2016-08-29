# PartOrganizer
This simple mod allow the creation of custom part subcategories in the KSP Editor.

###EditorCategory
The mod parse all EditorCategory entries it can find inside the .cfg files in GameData, and use that information to create custom subcategories.

| PROPERTY      | DESCRIPTION                                                | EXAMPLE
| ------------- |------------------------------------------------------------|---
| name          | Unique name for the category                               | Electrical
| tag           | Tag used to identify parts that belongs to this category   | _electrical
| icon          | Url to the icon                                            | MyMod/icons/my_icon
| tooltip       | Optional tooltip shown when hovering the icon              | Electrical

###Icons
An icon is required, and it need to be 32x32 pixels in size. The url to the icon is the path inside GameData, without file extension. If another image is present with the same url but ending with '_selected', that image will be used when the category is selected.

###Tag
The tag system is exploited to identify parts that belong to a category. In the example above, any parts that contain the string '_electrical' in its 'tags' field will be included in the Electrical category.

###Issues
To avoid showing a part in a custom category and in the stock category at the same time, the part 'category' field can be set to 'none'. This reduce the clutter, but has the drawback that the part can't be filtered by tag anymore in the stock search system.

###Included categories
Only a category is provided, as an example: Electrical. It include solar panels, batteries, lights, rtg and fuel cells.

###Tutorial
```
// define the category
EditorCategory
{
  name = Electrical
  tag = _electrical
  icon = PartOrganizer/Electrical/electrical
  tooltip = Electrical
}


// solar panels
@PART[solarPanels1,solarPanels2,solarPanels3,solarPanels4,solarPanels5,largeSolarPanel]
{
  @category = none
  @tags ^= :$: _electrical:
}


// batteries
@PART[batteryPack,batteryBankMini,batteryBank,batteryBankLarge,ksp_r_largeBatteryPack]
{
  @category = none
  @tags ^= :$: _electrical:
}


// rtg and fuel cells
@PART[rtg,FuelCell,FuelCellArray]
{
  @category = none
  @tags ^= :$: _electrical:
}


// lights
@PART[spotLight1,spotLight2]
{
  @category = none
  @tags ^= :$: _electrical:
}
```
