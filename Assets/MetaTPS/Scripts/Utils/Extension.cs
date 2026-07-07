using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
public static class Extension
{
	public static void AddEvent(this Button btn, UnityAction action)
	{
		btn.onClick.RemoveAllListeners();
		btn.onClick.AddListener(action);
	}

	public static void AddValueChangedEvent(this TMP_InputField input, UnityAction<string> action)
	{
		input.onValueChanged.RemoveAllListeners();
		input.onValueChanged.AddListener(action);
	}

	public static void AddSelectEvent(this TMP_InputField input, UnityAction<string> action)
	{
		input.onSelect.RemoveAllListeners();
		input.onSelect.AddListener(action);
	}

	public static void AddDeselectEvent(this TMP_InputField input, UnityAction<string> action)
	{
		input.onDeselect.RemoveAllListeners();
		input.onDeselect.AddListener(action);
	}
}