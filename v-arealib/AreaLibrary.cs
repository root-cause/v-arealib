using System;
using System.Collections.Generic;
using System.Linq;
using GTA;
using GTA.Math;
using GTA.Native;

namespace AreaLib
{
    #region Delegates
    public delegate void PlayerEvent(AreaBase area);
    public delegate void EntityEvent(AreaBase area, int entityHandle);
    public delegate void DataChangeEvent(AreaBase area, string key, object oldValue, object newValue);
    #endregion

    public class AreaLibrary : Script
    {
        private static HashSet<AreaBase> _areas = new HashSet<AreaBase>();
        private Dictionary<int, Vector3> _entityPosCache = new Dictionary<int, Vector3>();

        #region Public methods
        /// <summary>
        /// Starts tracking of the specified area.
        /// </summary>
        /// <param name="area">The area to track.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="area"/> is null.</exception>
        public static void Track(AreaBase area)
        {
            if (area == null)
            {
                throw new ArgumentNullException(nameof(area));
            }

            _areas.Add(area);
        }

        /// <summary>
        /// Returns whether the specified area is being tracked.
        /// </summary>
        /// <param name="area">The area to check.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="area"/> is null.</exception>
        public static bool IsTracked(AreaBase area)
        {
            if (area == null)
            {
                throw new ArgumentNullException(nameof(area));
            }

            return _areas.Contains(area);
        }

        /// <summary>
        /// Returns a read only collection of all tracked areas.
        /// </summary>
        public static IReadOnlyCollection<AreaBase> GetAreas()
        {
            return _areas.ToList().AsReadOnly();
        }

        /// <summary>
        /// Stops tracking of the specified area.
        /// </summary>
        /// <param name="area">The area to stop tracking of.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="area"/> is null.</exception>
        public static void Untrack(AreaBase area)
        {
            if (area == null)
            {
                throw new ArgumentNullException(nameof(area));
            }

            _areas.Remove(area);
        }
        #endregion

        public AreaLibrary()
        {
            Interval = 200;

            Tick += AreaLib_Tick;
            Aborted += AreaLib_Aborted;
        }

        private void AreaLib_Tick(object sender, EventArgs e)
        {
            Vector3 playerPosition = Game.Player.Character.Position;

            foreach (AreaBase area in _areas.ToList())
            {
                // Handle player
                if (!area.IgnorePlayer)
                {
                    bool check = area.IsPointInside(playerPosition);

                    if (check && !area.IsPlayerInside)
                    {
                        area.IsPlayerInside = true;
                        area.InvokePlayerEnter();
                    }
                    else if (!check && area.IsPlayerInside)
                    {
                        area.IsPlayerInside = false;
                        area.InvokePlayerLeave();
                    }
                }

                // Handle tracked entities
                if (area.TrackedEntities.Count > 0)
                {
                    area.TrackedEntities.RemoveWhere(handle => !Function.Call<bool>(Hash.DOES_ENTITY_EXIST, handle));
                    area.TrackedInside.IntersectWith(area.TrackedEntities);

                    foreach (int handle in area.TrackedEntities.ToList())
                    {
                        if (!_entityPosCache.ContainsKey(handle))
                        {
                            _entityPosCache[handle] = Function.Call<Vector3>(Hash.GET_ENTITY_COORDS, handle, false);
                        }

                        bool check = area.IsPointInside(_entityPosCache[handle]);
                        if (check && !area.TrackedInside.Contains(handle))
                        {
                            area.TrackedInside.Add(handle);
                            area.InvokeEntityEnter(handle);
                        }
                        else if (!check && area.TrackedInside.Contains(handle))
                        {
                            area.TrackedInside.Remove(handle);
                            area.InvokeEntityLeave(handle);
                        }
                    }
                }
            }

            _entityPosCache.Clear();
        }

        private void AreaLib_Aborted(object sender, EventArgs e)
        {
            _areas.Clear();
            _entityPosCache.Clear();

            _areas = null;
            _entityPosCache = null;
        }
    }
}
