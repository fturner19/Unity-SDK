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

using UnityEngine;
using BackendlessAPI;
using BackendlessAPI.Async;

public class RegisterDevice : MonoBehaviour
{
  [SerializeField]
  private GameObject mWaiting;
  private Rect mPopupWindowRect;
  private bool mIsRegisterFinish = false;
  private bool mIsRegisterSuccess = false;
  private string mResultMessage = "";

  void OnGUI()
  {
    int left = MainMenu.MARGIN;
    int top = MainMenu.MARGIN;
    int width = Screen.width - (MainMenu.MARGIN * 2);
    int height = ((Screen.height - MainMenu.MARGIN) / 8) - MainMenu.MARGIN;

    if (mIsRegisterFinish)
    {
      mWaiting.SetActive(false);
      if(mIsRegisterSuccess)
        Application.LoadLevel("PushMessages");
      else
        mPopupWindowRect = GUI.ModalWindow(0, new Rect(MainMenu.MARGIN * 3, MainMenu.MARGIN * 3, Screen.width - ((MainMenu.MARGIN * 3) * 2), Screen.height - ((MainMenu.MARGIN * 3) * 2)), DoPopupWindow, mResultMessage);
    }
    else
    {
      GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
      titleStyle.alignment = TextAnchor.MiddleCenter;
      GUIStyle inputTitleStyle = new GUIStyle(GUI.skin.label);
      inputTitleStyle.fontSize = (int)(GUI.skin.label.fontSize / 2.0f);
      GUI.Label(new Rect(left, top, width, height), "Push Messages", titleStyle);
      top += MainMenu.MARGIN + height;
      GUI.Label(new Rect(left, top, width, height * 5), "This example demonstrates push notifications to mobile devices.\nClick the button below to connect/register the device with Backendless to start receiving notifications.", titleStyle);
      top += (MainMenu.MARGIN + height) * 5;
      if (GUI.Button(new Rect(left, top, width, height), "Register Device"))
        Register();
      top += MainMenu.MARGIN + height;
      if (GUI.Button(new Rect(left, top, width, height), "Back"))
        Application.LoadLevel("MessagingService");
    }
  }

  void Register()
  {
    mWaiting.SetActive(true);

#if UNITY_ANDROID || UNITY_IPHONE
    AsyncCallback<string> callback = new AsyncCallback<string>(
      registrationId =>
      {
        mIsRegisterFinish = true;
        mIsRegisterSuccess = true;
      },
      fault =>
      {
        mResultMessage = "Error\n\nCode = " + fault.FaultCode + "\nMessage = " + fault.Message;
        mIsRegisterFinish = true;
        mIsRegisterSuccess = false;
      });
#endif

#if UNITY_ANDROID
    string GCMSenderID = "929309203937";
    Backendless.Messaging.RegisterDevice(GCMSenderID, callback);
#elif UNITY_IPHONE
    Backendless.Messaging.RegisterDevice("", callback);
#else
    mIsRegisterFinish = true;
    mIsRegisterSuccess = true;
#endif
  }

  void DoPopupWindow(int windowID)
  {
    float height = GUI.skin.label.fontSize * 3.0f;
    float top = mPopupWindowRect.height - height - MainMenu.MARGIN;
    Rect rect = new Rect(mPopupWindowRect.xMin - MainMenu.MARGIN, top, mPopupWindowRect.width - (mPopupWindowRect.xMin - MainMenu.MARGIN) * 2, height);

    if (GUI.Button(rect, "OK"))
      mIsRegisterFinish = false;
  }
}
