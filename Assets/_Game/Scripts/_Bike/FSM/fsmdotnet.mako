# coding: utf-8
<%!
from textwrap import wrap
import re
%><%
def parseCondition(c):
	def replacer(s):
		try:
			return '(events == Event.%s)' % (capitalCase(machine.event(s.group()).name),)
		except KeyError:
			return '%s()' % machine.input(s.group()).name
			
	return re.sub(r'\b(\w+)\b', replacer, c);

def formatAction(a,obj):
	if a.type == 'output':
		return 'if (%s != null) %s(obj);' % (a.name,a.name)
	else:
		return 'processEvent(%s);' % (capitalCase(a.name),)

%>
//#define FSMDEBUG
using UnityEngine;
using System.Collections;

//THIS FILE IS CREATED BY FSM EDITOR, MODIFIED BY PANWEIGUO
//COPYRIGHTS @ 2013 SIX DYNASTIES
//WWW.SIXDYNASTIES.COM WWW.MYCSOFT.NET

public class ${machine.name} {
	
public	enum State {
    % for s in machine.states[:-1]:
    	${capitalCase(s.name)},
    % endfor
    	${capitalCase(machine.states[-1].name)}
    };

public enum Event {
    % for e in machine.events[:-1]:
    	% for l in wrap(e.comment, 80):
    	//  ${l}
    	% endfor
    	${capitalCase(e.name)},
    % endfor
    	% for l in wrap(machine.events[-1].comment, 80):
    	//  ${l}
    	% endfor
    	${capitalCase(machine.events[-1].name)}
    };
	
	
public ${machine.name}() {
	_currentState = State.${machine.initialState};
	_lastState = _currentState;
}

#region delegate function
    % for i in machine.inputs:
        %if i.comment:
            %for l in wrap(i.comment, 80):
        //  ${l}
            %endfor
        %endif
     public delegate void _${i.name}(object obj);
    % endfor


    % for i in machine.inputs:
        %if i.comment:
            %for l in wrap(i.comment, 80):
        //  ${l}
            %endfor
        %endif
    public event _${i.name} ${i.name};
    % endfor


    % for o in machine.outputs:
        %if o.comment:
    		% for l in wrap(o.comment, 80):
        //  ${l}
    		% endfor
        % endif
       public delegate void _${o.name}(object obj);
    % endfor
	
    % for o in machine.outputs:
        %if o.comment:
    		% for l in wrap(o.comment, 80):
        //  ${l}
    		% endfor
        % endif
       public event _${o.name} ${o.name};
    % endfor

#endregion


	//get the current status name
    public string stateName(State state) {
		switch (state) {
		% for s in machine.states:
			case State.${capitalCase(s.name)}:
				return "${capitalCase(s.name)}";
		% endfor
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

    public void processEvent(Event events, object obj=null) {
			switch (_currentState) {
		% for s in machine.states:
			case State.${capitalCase(s.name)}:
			% for t in s.transitions:
				// ${t.name}
				if (${parseCondition(t.condition)}) {
				% if t.destination:
					% for a in s.exitingActions:
		#if FSMDEBUG
						Debug.Log("- ${machine.name}::${a.name}()");
		#endif
					${formatAction(a,obj)}
					% endfor
				% endif
				% for a in t.actions:
		#if FSMDEBUG
			        Debug.Log("- ${machine.name}::${a.name}()");
		#endif
					${formatAction(a,obj)}
				% endfor
				% if t.destination:
		#if FSMDEBUG
					Debug.Log("- ${machine.name}::${capitalCase(s.name)} -> ${t.destination.name}: ${t.name}");
		#endif
                    _lastState = _currentState;
					_currentState = State.${t.destination.name};
					% for a in t.destination.incomeActions:
		#if FSMDEBUG
					Debug.Log("- ${a.type} ${machine.name}::${a.name}()");
		#endif
					${formatAction(a,obj)}
					% endfor
				% endif
					break;
				}
			% endfor
				break;
		% endfor
			}
	}	
};
