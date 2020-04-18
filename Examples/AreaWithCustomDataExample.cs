using GTA;
using GTA.Math;
using GTA.Native;
using AreaLib;

namespace AreaLibraryExamples
{
    public class AreaWithCustomDataExample : Script
    {
        // https://gtaforums.com/topic/820813-displaying-help-text/
        public void DisplayHelpTextThisFrame(string text)
        {
            Function.Call(Hash._SET_TEXT_COMPONENT_FORMAT, "STRING");
            Function.Call(Hash._ADD_TEXT_COMPONENT_STRING, text);
            Function.Call(Hash._0x238FFE5C7B0498A6, 0, 0, 1, -1);
        }

        public AreaWithCustomDataExample()
        {
            // Create an area
            Sphere beerArea = new Sphere(new Vector3(-1111.98f, 2693.92f, 18.55f), 1.0f);
            beerArea.SetData("iTimesVisited", 0);

            // Set up events
            beerArea.PlayerEnter += (AreaBase area) =>
            {
                // Get current value
                area.GetData<int>("iTimesVisited", out var timesVisited);

                // Update
                area.SetData("iTimesVisited", timesVisited + 1);
            };

            beerArea.DataChange += (AreaBase area, string key, object oldValue, object newValue) =>
            {
                // Debug message
                UI.Notify($"Data changed ({key}): {oldValue} -> {newValue}");

                // Player might be craving some beer
                // You can do this inside PlayerEnter as well with GetData, this is for example purposes
                if (key == "iTimesVisited" && (int)newValue >= 3)
                {
                    UI.Notify($"You visited this area {newValue} times, you sure you don't want some beer?");
                }
            };

            // Start tracking of the area
            AreaLibrary.Track(beerArea);
        }
    }
}
