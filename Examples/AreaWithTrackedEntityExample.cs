using System.Windows.Forms;
using GTA;
using GTA.Math;
using GTA.Native;
using AreaLib;

namespace AreaLibraryExamples
{
    public class AreaWithTrackedEntityExample : Script
    {
        public AreaWithTrackedEntityExample()
        {
            KeyUp += AreaWithTrackedEntityExample_KeyUp;
        }

        public void AreaWithTrackedEntityExample_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                // Get player's current vehicle
                Vehicle currentVehicle = Game.Player.Character.CurrentVehicle;
                if (currentVehicle == null)
                {
                    UI.Notify("You're not in a vehicle.");
                    return;
                }

                Sphere vehicleStopperArea = new Sphere(currentVehicle.GetOffsetInWorldCoords(new Vector3(0.0f, 5.0f, 0.0f)), 3.0f);

                // We don't need PlayerEnter/PlayerLeave events here
                vehicleStopperArea.IgnorePlayer = true;

                // Start tracking current vehicle
                vehicleStopperArea.TrackEntity(currentVehicle);

                // Tracked entities call TrackedEntityEnter/TrackedEntityLeave events
                vehicleStopperArea.TrackedEntityEnter += (AreaBase area, int entityHandle) => 
                {
                    UI.Notify($"Tracked entity entered the area, handle: {entityHandle}");

                    Function.Call(Hash._TASK_BRING_VEHICLE_TO_HALT, entityHandle, 3.0f, 1);
                };

                vehicleStopperArea.TrackedEntityLeave += (AreaBase area, int entityHandle) =>
                {
                    UI.Notify($"Tracked entity left the area, handle: {entityHandle}");

                    Function.Call(Hash.SET_VEHICLE_ALARM, entityHandle, true);
                    Function.Call(Hash.START_VEHICLE_ALARM, entityHandle);
                };

                // Start tracking of the area
                AreaLibrary.Track(vehicleStopperArea);
            }
        }
    }
}
