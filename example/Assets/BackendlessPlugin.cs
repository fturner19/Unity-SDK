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

#define ENABLE_PUSH_PLUGIN

using UnityEngine;
using BackendlessAPI;

public class BackendlessPlugin : MonoBehaviour
{
#if ENABLE_PUSH_PLUGIN
#if UNITY_ANDROID
  private static AndroidJavaObject activity = null;
#elif UNITY_IPHONE
  [System.Runtime.InteropServices.DllImport("__Internal")]
  extern static public void setListenerGameObject(string listenerName);
  [System.Runtime.InteropServices.DllImport("__Internal")]
  extern static public void registerForRemoteNotifications();
  [System.Runtime.InteropServices.DllImport("__Internal")]
  extern static public void unregisterForRemoteNotifications();
#endif
#endif

  public enum SERVER
  {
    BACKENDLESS,
    GMO_MBAAS
  };

  [SerializeField]
  private SERVER Server;
  [SerializeField]
  private string applicationId;
  [SerializeField]
  private string RestSecretKey;
  [SerializeField]
  private string version;

  void Awake()
  {
    DontDestroyOnLoad(this);

    if (Server == SERVER.GMO_MBAAS)
      Backendless.setUrl("https://api.gmo-mbaas.com");
    else
      Backendless.setUrl("https://api.backendless.com");

    Backendless.InitApp(applicationId, RestSecretKey, version);
#if ENABLE_PUSH_PLUGIN
    Backendless.Messaging.SetUnityRegisterDevice(UnityRegisterDevice, UnityUnregisterDevice);
#if UNITY_ANDROID
  AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
  activity = jc.GetStatic<AndroidJavaObject>("currentActivity");
  activity.Call("setUnityGameObject", this.gameObject.name);
#elif UNITY_IPHONE
  setListenerGameObject(this.gameObject.name);
#endif
#endif
  }

#if ENABLE_PUSH_PLUGIN
  public static void UnityRegisterDevice(string GCMSenderID, long timestamp)
  {
#if UNITY_ANDROID
    activity.Call("registerDevice", GCMSenderID, timestamp);
#elif UNITY_IPHONE
    registerForRemoteNotifications();
#endif
  }

  public static void UnityUnregisterDevice()
  {
#if UNITY_ANDROID
    activity.Call("unregisterDevice");
#elif UNITY_IPHONE
    unregisterForRemoteNotifications();
#endif
  }

  void setDeviceToken(string deviceToken)
  {
    Backendless.Messaging.DeviceRegistration.DeviceToken = deviceToken;
  }

  void setDeviceId(string deviceId)
  {
    Backendless.Messaging.DeviceRegistration.DeviceId = deviceId;
  }

  void setOs(string os)
  {
    Backendless.Messaging.DeviceRegistration.Os = os;
  }

  void setOsVersion(string osVersion)
  {
    Backendless.Messaging.DeviceRegistration.OsVersion = osVersion;
  }

  void setExpiration(long expiration)
  {
    Backendless.Messaging.DeviceRegistration.Timestamp = expiration;
  }

  void registerDeviceOnServer(string dummy)
  {
    Debug.Log("registerDeviceOnServer()");
    Backendless.Messaging.RegisterDeviceOnServer();
  }

  void unregisterDeviceOnServer(string dummy)
  {
    Debug.Log("unregisterDeviceOnServer()");
    Backendless.Messaging.UnregisterDeviceOnServer();
  }

  public void onDidFailToRegisterForRemoteNotificationsWithError(string error)
  {
    Debug.LogError("onDidFailToRegisterForRemoteNotificationsWithError() error=" + error);
  }

  void onPushMessage(string message)
  {
    Debug.Log("onPushMessage() message=" + message);
    PushMessages.onPushMessage(message);
  }
#endif
}
