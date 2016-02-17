/// <summary>
/// Event Manager
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NashTools
{
	public static class EventManager
	{
		// General delegate for events
		public delegate void OnEvent(Component sender, object param = null);  	

		// Dictionary of Events and Listeners
		private static Dictionary<int, List<OnEvent>> Listeners = new Dictionary<int, List<OnEvent>>();



		public static void Init()
		{
			Listeners = new Dictionary<int, List<OnEvent>>();
		}



		public static void AddListener(int eventIndex, OnEvent listener)
		{
			// Temp listeners list
			List<OnEvent> tempListenersList = null;

			// If there's already a list for this event type, copy it to the temp ListenList (the out parameter)
			if(Listeners.TryGetValue(eventIndex, out tempListenersList))
			{
				// add this listener to it
				tempListenersList.Add(listener);
				return;
			}
			
			//Otherwise, we'll need a new list and add this as the first listener.  
			tempListenersList = new List<OnEvent>();
			tempListenersList.Add(listener);

			// Add the temp list back to the main one. 
			Listeners.Add(eventIndex, tempListenersList);
		}




		public static void PostNotification(int eventIndex, Component sender, object param = null)
		{
			// Temp list of listeners
			List<OnEvent> tempListenersList = null;
			
			// If there are no listeners, get out, there's no one to notify.  If there are, put them in the temp ListenList (the out parameter)
			if(!Listeners.TryGetValue(eventIndex, out tempListenersList))
				return;
			
			// Notify listeners of this event
			for(int i = 0; i < tempListenersList.Count; i++)
			{
				//If listener is not null, then call delegate
				if(!tempListenersList[i].Equals(null))  
					tempListenersList[i](sender, param);
			}
		}



		public static void RemoveEvent(int eventIndex)
		{
			Listeners.Remove(eventIndex);
		}



		public static void Cleanup()
		{
			// Temp dictionary of event types and listeners
			Dictionary<int, List<OnEvent>> tempListenersDictionary = new Dictionary<int, List<OnEvent>>();

			foreach(KeyValuePair<int, List<OnEvent>> listenersList in Listeners)
			{
				//Cycle through all listeners in list, remove null objects
				for(int i = listenersList.Value.Count - 1; i >= 0; i--)
				{
					if(listenersList.Value[i].Equals(null))
						listenersList.Value.RemoveAt(i);
				}
				
				//If items remain in list for this event type, then add to temp dictionary
				if(listenersList.Value.Count > 0)
					tempListenersDictionary.Add (listenersList.Key, listenersList.Value);
			}
			
			// Replace the main dictionary with this one
			Listeners = tempListenersDictionary;
		}
	}
}