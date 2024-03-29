/*
Copyright 2015 Acrodea, Inc. All Rights Reserved.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

package com.backendless.push;

import com.unity3d.player.UnityPlayerActivity;

import android.os.Bundle;

public class BackendlessUnityPlayerActivity extends UnityPlayerActivity {

	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		Messaging.setContext(this);
	}
	
	public void registerDevice(String GCMSenderID, long expiration) {
		Messaging.registerDevice(GCMSenderID, expiration);
	}
	
	public void setUnityGameObject(String gameObject) {
		Messaging.setUnityGameObject(gameObject);
	}
	
	public void unregisterDevice() {
		Messaging.unregisterDevice();
	}
}
