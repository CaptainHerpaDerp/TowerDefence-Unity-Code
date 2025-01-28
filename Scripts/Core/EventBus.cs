using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Accessed by other classes to subscribe to and publish events
    /// </summary>
    public class EventBus : MonoBehaviour
    {
        public static EventBus Instance { get; private set; }

        private Dictionary<string, Action<object>> eventHandlers = new Dictionary<string, Action<object>>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogError("More than one EventBus instance in scene!");
                Destroy(this);
                return;
            }
        }

        /// <summary>
        /// Listen for an event with the given name, the passed handler will be called when the event is published
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="handler"></param>
        public void Subscribe(string eventName, Action<object> handler)
        {
            if (!eventHandlers.ContainsKey(eventName))
            {
                eventHandlers[eventName] = null;
            }
            eventHandlers[eventName] += handler;
        }

        /// <summary>
        /// Subscribe to an event without passing any data
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="handler"></param>
        public void Subscribe(string eventName, Action handler)
        {
            if (!eventHandlers.ContainsKey(eventName))
            {
                eventHandlers[eventName] = null;
            }

            eventHandlers[eventName] += (data) => handler();
        }


        /// <summary>
        /// Unsubscribe from an event with the given name
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="handler"></param>
        public void Unsubscribe(string eventName, Action<object> handler)
        {
            if (eventHandlers.ContainsKey(eventName))
            {
                eventHandlers[eventName] -= handler;
            }
        }

        /// <summary>
        /// Publish (invoke) an event with the given name, passing the given data to the event handlers
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="eventData"></param>
        public void Publish(string eventName, object eventData = null)
        {
            if (eventHandlers.ContainsKey(eventName))
            {
                eventHandlers[eventName]?.Invoke(eventData);
            }
        }
    }
}
