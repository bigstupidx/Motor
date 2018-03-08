
//#define FSMDEBUG
using UnityEngine;
using System.Collections;

//THIS FILE IS CREATED BY FSM EDITOR, MODIFIED BY PANWEIGUO
//COPYRIGHTS @ 2013 SIX DYNASTIES
//WWW.SIXDYNASTIES.COM WWW.MYCSOFT.NET

public class BikeFSM {

	public enum State {
		Attack,
		Back,
		Crash,
		Ride,
		Stand
	};

	public enum Event {
		Attack,
		Attack_finish,
		Back,
		Crash,
		Reset,
		Ride,
		Stand
	};


	public BikeFSM() {
		_currentState = State.Stand;
		_lastState = _currentState;
	}

	#region delegate function




	public delegate void _OnAttackIn(object obj);
	public delegate void _OnAttackOut(object obj);
	public delegate void _OnBackIn(object obj);
	public delegate void _OnBackOut(object obj);
	public delegate void _OnCrashIn(object obj);
	public delegate void _OnCrashOut(object obj);
	public delegate void _OnRideIn(object obj);
	public delegate void _OnRideOut(object obj);
	public delegate void _OnStandIn(object obj);
	public delegate void _OnStandOut(object obj);

	public event _OnAttackIn OnAttackIn;
	public event _OnAttackOut OnAttackOut;
	public event _OnBackIn OnBackIn;
	public event _OnBackOut OnBackOut;
	public event _OnCrashIn OnCrashIn;
	public event _OnCrashOut OnCrashOut;
	public event _OnRideIn OnRideIn;
	public event _OnRideOut OnRideOut;
	public event _OnStandIn OnStandIn;
	public event _OnStandOut OnStandOut;

	#endregion


	//get the current status name
	public string stateName(State state) {
		switch (state) {
			case State.Attack:
				return "Attack";
			case State.Back:
				return "Back";
			case State.Crash:
				return "Crash";
			case State.Ride:
				return "Ride";
			case State.Stand:
				return "Stand";
		}
		return "Unknown";
	}

	public State currentState() {
		return _currentState;
	}

	public State lastState() {
		return _lastState;
	}

	private State _currentState;
	private State _lastState;
	private State _backState; // after a short status change, the fsm will be set to this status.


	public State getBackState() {
		return _backState;
	}

	public State setBackState(State stat) {
		_backState = stat;
		return stat;
	}

	public void processEvent(Event events, object obj = null) {
		switch (_currentState) {
			case State.Attack:
				// attack_finish
				if ((events == Event.Attack_finish)) {
#if FSMDEBUG
						Debug.Log("- BikeFSM::OnAttackOut()");
#endif
					if (OnAttackOut != null) OnAttackOut(obj);
#if FSMDEBUG
					Debug.Log("- BikeFSM::Attack -> Ride: attack_finish");
#endif
					_lastState = _currentState;
					_currentState = State.Ride;
#if FSMDEBUG
					Debug.Log("- output BikeFSM::OnRideIn()");
#endif
					if (OnRideIn != null) OnRideIn(obj);
					break;
				}
				// crash
				if ((events == Event.Crash)) {
#if FSMDEBUG
						Debug.Log("- BikeFSM::OnAttackOut()");
#endif
					if (OnAttackOut != null) OnAttackOut(obj);
#if FSMDEBUG
					Debug.Log("- BikeFSM::Attack -> Crash: crash");
#endif
					_lastState = _currentState;
					_currentState = State.Crash;
#if FSMDEBUG
					Debug.Log("- output BikeFSM::OnCrashIn()");
#endif
					if (OnCrashIn != null) OnCrashIn(obj);
					break;
				}
				break;
			case State.Back:
				// stand
				if ((events == Event.Stand)) {
#if FSMDEBUG
						Debug.Log("- BikeFSM::OnBackOut()");
#endif
					if (OnBackOut != null) OnBackOut(obj);
#if FSMDEBUG
					Debug.Log("- BikeFSM::Back -> Stand: stand");
#endif
					_lastState = _currentState;
					_currentState = State.Stand;
#if FSMDEBUG
					Debug.Log("- output BikeFSM::OnStandIn()");
#endif
					if (OnStandIn != null) OnStandIn(obj);
					break;
				}
				// crash
				if ((events == Event.Crash)) {
#if FSMDEBUG
						Debug.Log("- BikeFSM::OnBackOut()");
#endif
					if (OnBackOut != null) OnBackOut(obj);
#if FSMDEBUG
					Debug.Log("- BikeFSM::Back -> Crash: crash");
#endif
					_lastState = _currentState;
					_currentState = State.Crash;
#if FSMDEBUG
					Debug.Log("- output BikeFSM::OnCrashIn()");
#endif
					if (OnCrashIn != null) OnCrashIn(obj);
					break;
				}
				break;
			case State.Crash:
				// reset
				if ((events == Event.Reset)) {
#if FSMDEBUG
						Debug.Log("- BikeFSM::OnCrashOut()");
#endif
					if (OnCrashOut != null) OnCrashOut(obj);
#if FSMDEBUG
					Debug.Log("- BikeFSM::Crash -> Stand: reset");
#endif
					_lastState = _currentState;
					_currentState = State.Stand;
#if FSMDEBUG
					Debug.Log("- output BikeFSM::OnStandIn()");
#endif
					if (OnStandIn != null) OnStandIn(obj);
					break;
				}
				break;
			case State.Ride:
				// stand
				if ((events == Event.Stand)) {
#if FSMDEBUG
						Debug.Log("- BikeFSM::OnRideOut()");
#endif
					if (OnRideOut != null) OnRideOut(obj);
#if FSMDEBUG
					Debug.Log("- BikeFSM::Ride -> Stand: stand");
#endif
					_lastState = _currentState;
					_currentState = State.Stand;
#if FSMDEBUG
					Debug.Log("- output BikeFSM::OnStandIn()");
#endif
					if (OnStandIn != null) OnStandIn(obj);
					break;
				}
				// attack
				if ((events == Event.Attack)) {
#if FSMDEBUG
						Debug.Log("- BikeFSM::OnRideOut()");
#endif
					if (OnRideOut != null) OnRideOut(obj);
#if FSMDEBUG
					Debug.Log("- BikeFSM::Ride -> Attack: attack");
#endif
					_lastState = _currentState;
					_currentState = State.Attack;
#if FSMDEBUG
					Debug.Log("- output BikeFSM::OnAttackIn()");
#endif
					if (OnAttackIn != null) OnAttackIn(obj);
					break;
				}
				// crash
				if ((events == Event.Crash)) {
#if FSMDEBUG
						Debug.Log("- BikeFSM::OnRideOut()");
#endif
					if (OnRideOut != null) OnRideOut(obj);
#if FSMDEBUG
					Debug.Log("- BikeFSM::Ride -> Crash: crash");
#endif
					_lastState = _currentState;
					_currentState = State.Crash;
#if FSMDEBUG
					Debug.Log("- output BikeFSM::OnCrashIn()");
#endif
					if (OnCrashIn != null) OnCrashIn(obj);
					break;
				}
				break;
			case State.Stand:
				// ride
				if ((events == Event.Ride)) {
#if FSMDEBUG
						Debug.Log("- BikeFSM::OnStandOut()");
#endif
					if (OnStandOut != null) OnStandOut(obj);
#if FSMDEBUG
					Debug.Log("- BikeFSM::Stand -> Ride: ride");
#endif
					_lastState = _currentState;
					_currentState = State.Ride;
#if FSMDEBUG
					Debug.Log("- output BikeFSM::OnRideIn()");
#endif
					if (OnRideIn != null) OnRideIn(obj);
					break;
				}
				// back
				if ((events == Event.Back)) {
#if FSMDEBUG
						Debug.Log("- BikeFSM::OnStandOut()");
#endif
					if (OnStandOut != null) OnStandOut(obj);
#if FSMDEBUG
					Debug.Log("- BikeFSM::Stand -> Back: back");
#endif
					_lastState = _currentState;
					_currentState = State.Back;
#if FSMDEBUG
					Debug.Log("- output BikeFSM::OnBackIn()");
#endif
					if (OnBackIn != null) OnBackIn(obj);
					break;
				}
				// crash
				if ((events == Event.Crash)) {
#if FSMDEBUG
						Debug.Log("- BikeFSM::OnStandOut()");
#endif
					if (OnStandOut != null) OnStandOut(obj);
#if FSMDEBUG
					Debug.Log("- BikeFSM::Stand -> Crash: crash");
#endif
					_lastState = _currentState;
					_currentState = State.Crash;
#if FSMDEBUG
					Debug.Log("- output BikeFSM::OnCrashIn()");
#endif
					if (OnCrashIn != null) OnCrashIn(obj);
					break;
				}
				break;
		}
	}
};
