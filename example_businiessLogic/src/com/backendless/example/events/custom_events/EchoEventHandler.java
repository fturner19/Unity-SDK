package com.backendless.example.events.custom_events;

import com.backendless.Backendless;
import com.backendless.servercode.RunnerContext;
import com.backendless.servercode.annotation.Asset;
import com.backendless.servercode.annotation.Async;
import com.backendless.servercode.annotation.BackendlessEvent;
import com.backendless.servercode.annotation.BackendlessGrantAccess;
import com.backendless.servercode.annotation.BackendlessRejectAccess;
import com.backendless.servercode.logging.Logger;

import java.util.Collections;
import java.util.HashMap;
import java.util.Map;
import java.util.Map.Entry;

/**
 * EchoEventHandler handles custom event "echo". This is accomplished with the
 * BackendlessEvent( "echo" ) annotation. The event can be raised by either the
 * client-side or the server-side code (in other event handlers or timers). The
 * name of the class is not significant, it can be changed, since the event
 * handler is associated with the event only through the annotation.
 */
@BackendlessEvent("echo")
public class EchoEventHandler extends
		com.backendless.servercode.extension.CustomEventHandler {
	
	@Override
	public Map handleEvent(RunnerContext context, Map eventArgs) {
		System.out.println("eventArgs=" + eventArgs);
		return eventArgs;
	}

}