using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;

public partial class CentralEventSystem : Node {
	public static Dictionary<string, List<Delegate>> eventDictionary = new Dictionary<string, List<Delegate>>();
	public static void RegisterEventHandler(string eventName, Delegate handler) {
		if(!IsHandlerValid(handler)){
			GD.PrintErr("You provided an event handler \"" + handler.GetMethodInfo().Name + "\" for eventName \"" + eventName + "\" with an invalid signature. Please ensure that your event handlers accept one parameter that is a child of EventArgs");
			return;
		}
		if(!eventDictionary.ContainsKey(eventName)) eventDictionary[eventName] = new List<Delegate>();
		eventDictionary[eventName].Add(handler);
	}

    public static void UnregisterEventHandler(string eventName, Delegate handler){
        if(eventDictionary.ContainsKey(eventName)) eventDictionary[eventName].Remove(handler);
    }

	public static void PublishEvent(string eventName, EventArgs eventArgs) {
		if(!eventDictionary.ContainsKey(eventName)) return;
		foreach(Delegate handler in eventDictionary[eventName]) handler.DynamicInvoke(eventArgs);
	}

	private static bool IsHandlerValid(Delegate handler) {
		ParameterInfo[] parameterInfos = handler.GetMethodInfo().GetParameters();
		if(parameterInfos.Length != 1 || !parameterInfos[0].ParameterType.IsSubclassOf(typeof(EventArgs))) return false;
		return true;
	}
}
