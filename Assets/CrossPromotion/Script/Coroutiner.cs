using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace crosspromotion
{
	public class Coroutiner {
		private static CoroutinerInstance cache = null;
		private static GameObject coroutineContainer = null;

		public static Coroutine StartCoroutine(IEnumerator iterationResult, bool canStopWhenClearScene = true) {
			//Create GameObject with MonoBehaviour to handle task.
			if (coroutineContainer == null) {
				coroutineContainer = new GameObject("Coroutiner");
				cache = coroutineContainer.AddComponent(typeof(CoroutinerInstance)) as CoroutinerInstance;
			}

			Action del = delegate { };
			return cache.ProcessWork(iterationResult, canStopWhenClearScene, del);
		}

		public static void StopCoroutine(IEnumerator iterationResult) {
			cache.StopCoroutine(iterationResult);
		}

		public static void StopAllCoroutine() {
			cache.StopAllCoroutine();
		}

		public static void PauseAllCoroutine() {
			if (cache != null) {
				CoroutinerInstance coroutinerInstance = cache;
				List<CoroutineController> c = coroutinerInstance.coroutineController;
				for (int i = 0; i < c.Count; i++) {
					if (c[i].state == CoroutineState.Running)
						c[i].Pause();
				}

			}
		}

		public static void ResumeAllCoroutine() {
			if (cache != null) {
				CoroutinerInstance coroutinerInstance = cache;
				List<CoroutineController> c = coroutinerInstance.coroutineController;
				for (int i = 0; i < c.Count; i++) {
					if (c[i].state == CoroutineState.Paused)
						c[i].Resume();
				}

			}
		}
	}

	public class CoroutinerInstance : MonoBehaviour {
		public List<CoroutineController> coroutineController = new List<CoroutineController>();
		void Awake() {
			DontDestroyOnLoad(this);
		}

		public Coroutine ProcessWork(IEnumerator routine, bool canStopWhenClearScene, Action onFinish) {
			if (routine == null) {
				throw new System.ArgumentNullException("routine");
			}
			CoroutineController c = new CoroutineController(routine, canStopWhenClearScene);
			c.onFinish += delegate (CoroutineController controller) {
				coroutineController.Remove(c);
				onFinish.Invoke();
			};
			coroutineController.Add(c);
			return StartCoroutine(c.Start());
		}

		public void StopAllCoroutine() {
			List<CoroutineController> removeAble = new List<CoroutineController>();
			foreach (CoroutineController controller in coroutineController) {
				if (controller.canStopWhenClearScene) {
					controller.Stop();
					removeAble.Add(controller);
				}
			}
			for (int i = 0; i < removeAble.Count; i++) {
				coroutineController.Remove(removeAble[i]);
			}
		}
	}

}
