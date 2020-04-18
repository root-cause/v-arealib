using System;
using GTA;
using GTA.Math;
using GTA.Native;
using AreaLib;

namespace AreaLibraryExamples
{
    public class AreaExample : Script
    {
        // https://gtaforums.com/topic/820813-displaying-help-text/
        public void DisplayHelpTextThisFrame(string text)
        {
            Function.Call(Hash._SET_TEXT_COMPONENT_FORMAT, "STRING");
            Function.Call(Hash._ADD_TEXT_COMPONENT_STRING, text);
            Function.Call(Hash._0x238FFE5C7B0498A6, 0, 0, 1, -1);
        }

        public AreaExample()
        {
            // Create an area
            Sphere doorHintArea = new Sphere(new Vector3(-1112.11f, 2689.73f, 18.59f), 1.0f);

            // Set up events
            doorHintArea.PlayerEnter += (AreaBase area) =>
            {
                Tick += ShowDoorHint;
            };

            doorHintArea.PlayerLeave += (AreaBase area) =>
            {
                Tick -= ShowDoorHint;
            };

            // Start tracking of the area
            AreaLibrary.Track(doorHintArea);
        }

        public void ShowDoorHint(object sender, EventArgs e)
        {
            DisplayHelpTextThisFrame("You can buy weapons and body armor from here.");
        }
    }
}
