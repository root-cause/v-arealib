using System;
using System.Collections.Generic;
using System.Linq;
using GTA;
using GTA.Math;

namespace AreaLib
{
    /// <summary>
    /// Generic area class that you can inherit to make custom areas/shapes.
    /// </summary>
    public abstract class AreaBase
    {
        #region Properties
        internal HashSet<int> TrackedEntities { get; } = new HashSet<int>();
        internal HashSet<int> TrackedInside { get; } = new HashSet<int>();
        internal Dictionary<string, object> Data { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Whether the player ped should be ignored by this area. If ignored, this area will not call <see cref="PlayerEnter"/> and <see cref="PlayerLeave"/> events.
        /// </summary>
        public bool IgnorePlayer { get; set; } = false;

        /// <summary>
        /// Whether the player ped is inside this area.
        /// </summary>
        public bool IsPlayerInside { get; internal set; } = false;
        #endregion

        #region Events
        /// <summary>
        /// Called when the player ped enters this area.
        /// </summary>
        public event PlayerEvent PlayerEnter;

        /// <summary>
        /// Called when the player ped leaves this area.
        /// </summary>
        public event PlayerEvent PlayerLeave;

        /// <summary>
        /// Called when a tracked entity enters this area.
        /// </summary>
        public event EntityEvent TrackedEntityEnter;

        /// <summary>
        /// Called when a tracked entity leaves this area.
        /// </summary>
        public event EntityEvent TrackedEntityLeave;

        /// <summary>
        /// Called when data of this area is updated by <see cref="SetData{T}(string, T)"/>.
        /// </summary>
        public event DataChangeEvent DataChange;
        #endregion

        #region Methods
        /// <summary>
        /// Returns whether the specified position is inside this area.
        /// </summary>
        /// <param name="position">The position to check.</param>
        public abstract bool IsPointInside(Vector3 position);
        #endregion

        #region Invoker methods
        internal void InvokePlayerEnter()
        {
            PlayerEnter?.Invoke(this);
        }

        internal void InvokePlayerLeave()
        {
            PlayerLeave?.Invoke(this);
        }

        internal void InvokeEntityEnter(int entityHandle)
        {
            TrackedEntityEnter?.Invoke(this, entityHandle);
        }

        internal void InvokeEntityLeave(int entityHandle)
        {
            TrackedEntityLeave?.Invoke(this, entityHandle);
        }

        internal void InvokeDataChange(string key, object oldValue, object newValue)
        {
            DataChange?.Invoke(this, key, oldValue, newValue);
        }
        #endregion

        #region Tracked entity methods
        /// <summary>
        /// Starts tracking of the specified entity for this area.
        /// </summary>
        /// <param name="entity">The entity to track.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is null.</exception>
        public void TrackEntity(Entity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            // Prevent player ped from being tracked with TrackEntity
            if (entity.Handle == Game.Player.Character.Handle)
            {
                return;
            }

            TrackedEntities.Add(entity.Handle);
        }

        /// <summary>
        /// Returns whether the specified entity is being tracked by this area.
        /// </summary>
        /// <param name="entity">The entity to check.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is null.</exception>
        public bool IsEntityTracked(Entity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return TrackedEntities.Contains(entity.Handle);
        }

        /// <summary>
        /// Returns a read only collection of all tracked entity handles of this area.
        /// </summary>
        public IReadOnlyCollection<int> GetAllTrackedEntities()
        {
            return TrackedEntities.ToList().AsReadOnly();
        }

        /// <summary>
        /// Returns a read only collection of tracked entity handles that are inside this area.
        /// </summary>
        public IReadOnlyCollection<int> GetTrackedEntitiesInside()
        {
            return TrackedInside.ToList().AsReadOnly();
        }

        /// <summary>
        /// Stops tracking of the specified entity for this area.
        /// </summary>
        /// <param name="entity">The entity to stop tracking of.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is null.</exception>
        public void UntrackEntity(Entity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            TrackedEntities.Remove(entity.Handle);
            TrackedInside.Remove(entity.Handle);
        }
        #endregion

        #region Data methods
        /// <summary>
        /// Returns custom data of this area.
        /// </summary>
        /// <typeparam name="T">Data type.</typeparam>
        /// <param name="key">Data key. (Case sensitive)</param>
        /// <param name="result"></param>
        /// <returns>Returns true if retrieving data was successful, false otherwise.</returns>
        public bool GetData<T>(string key, out T result)
        {
            if (!Data.TryGetValue(key, out var value) || !(value is T data))
            {
                result = default;
                return false;
            }

            result = data;
            return true;
        }

        /// <summary>
        /// Sets custom data for this area.
        /// </summary>
        /// <typeparam name="T">Data type.</typeparam>
        /// <param name="key">Data key. (Case sensitive)</param>
        /// <param name="value">Data value.</param>
        public void SetData<T>(string key, T value)
        {
            bool existingKey = Data.TryGetValue(key, out var oldValue);
            Data[key] = value;

            if (existingKey)
            {
                InvokeDataChange(key, oldValue, value);
            }
        }

        /// <summary>
        /// Returns whether custom data collection of this area contains the specified data key.
        /// </summary>
        /// <param name="key">Data key. (Case sensitive)</param>
        public bool HasData(string key)
        {
            return Data.ContainsKey(key);
        }

        /// <summary>
        /// Returns a read only collection of all custom data keys of this area.
        /// </summary>
        public IReadOnlyCollection<string> GetDataKeys()
        {
            return Data.Keys.ToList().AsReadOnly();
        }

        /// <summary>
        /// Removes the specified data key from this area.
        /// </summary>
        /// <param name="key">Data key. (Case sensitive)</param>
        public void RemoveData(string key)
        {
            Data.Remove(key);
        }

        /// <summary>
        /// Removes all custom data from this area.
        /// </summary>
        public void RemoveAllData()
        {
            Data.Clear();
        }
        #endregion
    }
}
