﻿using System.Collections;
using UnityEngine;

namespace crosspromotion
{
	public class CoroutineController {
		public delegate void OnFinish(CoroutineController coroutineController);

		public bool canStopWhenClearScene;
		public event OnFinish onFinish;

		private IEnumerator _routine;
		private Coroutine _coroutine;
		private CoroutineState _state;

		public CoroutineController(IEnumerator routine, bool canStopWhenClearScene) {
			_routine = routine;
			this.canStopWhenClearScene = canStopWhenClearScene;
			_state = CoroutineState.Ready;
		}

		public void StartCoroutine(MonoBehaviour monoBehaviour) {
			_coroutine = monoBehaviour.StartCoroutine(Start());
		}

		public IEnumerator Start() {
			if (_state != CoroutineState.Ready) {
				throw new System.InvalidOperationException("Unable to start coroutine in state: " + _state);
			}

			_state = CoroutineState.Running;
			do {
				//try
				//  {
				if (!_routine.MoveNext()) {
					_state = CoroutineState.Finished;
				}
				//}
				//catch (System.Exception ex)
				//{
				//    DLog.LogError("Exception in coroutine: " + ex.Message);
				//    _state = CoroutineState.Finished;
				//    break;
				//}

				yield return _routine.Current;
				while (_state == CoroutineState.Paused) {
					yield return null;
				}
			} while (_state == CoroutineState.Running);

			_state = CoroutineState.Finished;

			if (onFinish != null)
				onFinish(this);
		}

		public void Stop() {
			if (_state != CoroutineState.Running && _state != CoroutineState.Paused) {
				//throw new System.InvalidOperationException("Unable to stop coroutine in state: " + _state);
			}

			_state = CoroutineState.Finished;
		}

		public void Pause() {
			if (_state != CoroutineState.Running) {
				throw new System.InvalidOperationException("Unable to pause coroutine in state: " + _state);
			}

			_state = CoroutineState.Paused;
		}

		public void Resume() {
			if (_state != CoroutineState.Paused) {
				//throw new System.InvalidOperationException("Unable to resume coroutine in state: " + state);
			}

			_state = CoroutineState.Running;
		}

		public CoroutineState state {
			get { return _state; }
		}

		public Coroutine coroutine {
			get { return _coroutine; }
		}

		public IEnumerator routine {
			get { return _routine; }
		}
	}

}
