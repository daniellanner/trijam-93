using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace io.daniellanner.indiversity
{
	public class TimelineManager : MonoBehaviour
	{
		private struct TimelineData
		{
			public float TimeFactor { get; set; }

			public static TimelineData Default => new TimelineData
			{
				TimeFactor = 1f
			};
		}

		private List<string> _activeTimelineIDs = new List<string>();

		private Dictionary<string, List<MonoBehaviourInTimeline>> _allTimelineInstances
			= new Dictionary<string, List<MonoBehaviourInTimeline>>();
		private Dictionary<string, TimelineData> _allTimelineData
			= new Dictionary<string, TimelineData>();
		
		private List<MonoBehaviourInTimeline> _activeTimelineInstances
			= new List<MonoBehaviourInTimeline>();
		private List<TimelineData> _activeTimelineData
			= new List<TimelineData>();
		
		private bool AddToActiveTimeline(string id)
		{
			if (_activeTimelineIDs.Contains(id))
			{
				return false;
			}

			_activeTimelineIDs.Add(id);
			return true;
		}

		private void BuildActiveTimelines()
		{
			_activeTimelineInstances.Clear();
			_activeTimelineData.Clear();

			foreach (var id in _activeTimelineIDs)
			{
				// add timeline instances
				_activeTimelineInstances.AddRange(_allTimelineInstances[id]);

				// add data copies for each timeline entry
				int count = _allTimelineInstances[id].Count;
				var data = _allTimelineData[id];

				for(int i = 0; i < count; i++)
				{
					_activeTimelineData.Add(data);
				}
			}
		}

		public bool SetTimeFactorOfTimeline(string id, float factor)
		{
			if(!_allTimelineData.ContainsKey(id))
			{
				return false;
			}

			var data = _allTimelineData[id];
			data.TimeFactor = factor;
			_allTimelineData[id] = data;

			return true;
		}

		public void DeactiveTimeline(string id)
		{
			bool removed = _activeTimelineIDs.Remove(id);

			if (removed && _allTimelineInstances.ContainsKey(id))
			{
				_allTimelineInstances[id].ForEach(it => it.ExitTimeline());
			}
		}

		public void ActivateTimeline(string id)
		{
			bool notActive = AddToActiveTimeline(id);

			if (notActive && _allTimelineInstances.ContainsKey(id))
			{
				_allTimelineInstances[id].ForEach(it => it.EnterTimeline());
			}
		}

		private void OnEnable()
		{
			var obj = FindObjectsOfType<MonoBehaviour>().OfType<MonoBehaviourInTimeline>();

			string id;
			bool active;

			foreach (var it in obj)
			{
				it.Timelines = this;

				var desc = (TimelineDescription)System.Attribute.GetCustomAttribute(it.GetType(), typeof(TimelineDescription));

				if (desc == null)
				{
					Debug.LogWarning($"{it.GetType()} does not define a TimelineDescription and will be executed constantly. To change this please add a TimelineDescription attribute.");

					id = "default";
					active = true;
				}
				else
				{
					id = desc.ID;
					active = desc.PlayOnAwake;
				}

				if (active)
				{
					AddToActiveTimeline(id);
				}

				if (!_allTimelineInstances.ContainsKey(id))
				{
					_allTimelineInstances.Add(id, new List<MonoBehaviourInTimeline>());
					_allTimelineData.Add(id, TimelineData.Default);
				}

				_allTimelineInstances[id].Add(it);
			}

			BuildActiveTimelines();
		}

		private void Start()
		{
			foreach (var it in _activeTimelineInstances)
			{
				it.EnterTimeline();
			}
		}

		private void Update()
		{
			// Cleanup remove or adds
			BuildActiveTimelines();

			if (_activeTimelineInstances.Count != _activeTimelineData.Count)
			{
				Debug.LogError("Timeline Error: Active timeline and active timeline data count mismatch.");
			}

			int length = Mathf.Min(_activeTimelineInstances.Count, _activeTimelineData.Count);

			for (int i = 0; i < length; i++)
			{
				MonoBehaviourInTimeline timeline = _activeTimelineInstances[i];
				TimelineData data = _activeTimelineData[i];

				timeline.UpdateTimeline(Time.deltaTime * data.TimeFactor);
			}
		}
	}
}