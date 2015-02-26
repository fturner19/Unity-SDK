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
using BackendlessAPI.Messaging;
using System.Collections.Generic;

public class PushMessages : MonoBehaviour
{
  [SerializeField]
  private GameObject mWaiting;
  private Rect mPopupWindowRect;
  private bool mIsSendMessageFinish = false;
  private bool mIsSendMessageSuccess = false;
  private bool mIsUnregisterFinish = false;
  private bool mIsUnregisterSuccess = false;
  private string mResultMessage = "";
  private string mSendMessage = "";
  private bool mForceNotify = true;
  private static List<string> mReceiveMessageList = new List<string>();

  void OnGUI()
  {
    int left = MainMenu.MARGIN;
    int top = MainMenu.MARGIN;
    int width = Screen.width - (MainMenu.MARGIN * 2);
    int height = ((Screen.height - MainMenu.MARGIN) / 10) - MainMenu.MARGIN;

    if (mIsSendMessageFinish)
    {
      mWaiting.SetActive(false);
      if(mIsSendMessageSuccess)
        mIsSendMessageFinish = false;
      else
        mPopupWindowRect = GUI.ModalWindow(0, new Rect(MainMenu.MARGIN * 3, MainMenu.MARGIN * 3, Screen.width - ((MainMenu.MARGIN * 3) * 2), Screen.height - ((MainMenu.MARGIN * 3) * 2)), DoPopupWindow, mResultMessage);
    }
    else if (mIsUnregisterFinish)
    {
      mWaiting.SetActive(false);
      if(mIsUnregisterSuccess)
        Application.LoadLevel("RegisterDevice");
      else
        mPopupWindowRect = GUI.ModalWindow(1, new Rect(MainMenu.MARGIN * 3, MainMenu.MARGIN * 3, Screen.width - ((MainMenu.MARGIN * 3) * 2), Screen.height - ((MainMenu.MARGIN * 3) * 2)), DoPopupWindow, mResultMessage);
    }
    else
    {
      GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
      titleStyle.alignment = TextAnchor.MiddleCenter;
      GUIStyle inputTitleStyle = new GUIStyle(GUI.skin.label);
      inputTitleStyle.fontSize = (int)(GUI.skin.label.fontSize / 2.0f);
      GUI.Label(new Rect(left, top, width, height), "Push Messages", titleStyle);
      top += MainMenu.MARGIN + height;
      GUI.Label(new Rect(left, top, width, (height / 2) * 3), "The device is now registered. Use the button below to broadcast a message or login to the Backendless Console, select Messaging and send a message from there.", inputTitleStyle);
      top += MainMenu.MARGIN + (height / 2) * 3;
      GUI.Label(new Rect(left, top, width, height / 2), "Message:", inputTitleStyle);
      top += height / 2;
      GUIStyle toggleStyle = new GUIStyle(GUI.skin.toggle);
      toggleStyle.fontSize = inputTitleStyle.fontSize;
      mForceNotify = GUI.Toggle(new Rect(left, top, width, height / 2), mForceNotify, "Include notification center update", toggleStyle);
      top += height / 2;
      mSendMessage = GUI.TextField(new Rect(left, top, width, height), mSendMessage);
      top += MainMenu.MARGIN + height;
      if (GUI.Button(new Rect(left, top, width, height), "Send"))
        SendMessage();
      top += MainMenu.MARGIN + height;
      GUI.Label(new Rect(left, top, width, height / 2), "Received Messages:", inputTitleStyle);
      top += height / 2;
      GUI.Box(new Rect(left, top, width, (MainMenu.MARGIN + height) * 3), "");
      string mReceiveMessage = "";
      lock (mReceiveMessageList)
      {
        foreach (string message in mReceiveMessageList)
          mReceiveMessage += message + "\n";
      }
      GUI.Label(new Rect(left, top, width, (MainMenu.MARGIN + height) * 3), mReceiveMessage, inputTitleStyle);
      top += (MainMenu.MARGIN + height) * 3 + MainMenu.MARGIN;
      if (GUI.Button(new Rect(left, top, width, height), "Unregister Device"))
        Unregister();
    }
  }

  public static void onPushMessage(string message)
  {
    lock (mReceiveMessageList)
    {
      if (mReceiveMessageList.Count > 5)
        mReceiveMessageList.RemoveAt(0);
      mReceiveMessageList.Add(message);
    }
  }

  void SendMessage()
  {
    mWaiting.SetActive(true);
    
    PublishOptions publishOptions = new PublishOptions();
    if (mForceNotify == true)
    {
      publishOptions.AddHeader(PublishOptions.ANDROID_TICKER_TEXT_TAG, mSendMessage);
      publishOptions.AddHeader(PublishOptions.ANDROID_CONTENT_TITLE_TAG, "Backendless example");
      publishOptions.AddHeader(PublishOptions.ANDROID_CONTENT_TEXT_TAG, mSendMessage);
    }
    DeliveryOptions deliveryOptions = new DeliveryOptions();
    deliveryOptions.PushBroadcast = DeliveryOptions.ALL;

    AsyncCallback<MessageStatus> callback = new AsyncCallback<MessageStatus>(
      status =>
      {
        mIsSendMessageFinish = true;
        mIsSendMessageSuccess = true;
      },
      fault =>
      {
        mResultMessage = "Error\n\nCode = " + fault.FaultCode + "\nMessage = " + fault.Message;
        mIsSendMessageFinish = true;
        mIsSendMessageSuccess = false;
      });

    Backendless.Messaging.Publish(mSendMessage, publishOptions, deliveryOptions, callback);
  }

  void Unregister()
  {
    mWaiting.SetActive(true);

    AsyncCallback<bool> callback = new AsyncCallback<bool>(
      result =>
      {
        mIsUnregisterFinish = true;
        mIsUnregisterSuccess = true;
      },
      fault =>
      {
        mResultMessage = "Error\n\nCode = " + fault.FaultCode + "\nMessage = " + fault.Message;
        mIsUnregisterFinish = true;
        mIsUnregisterSuccess = false;
      });
    Backendless.Messaging.UnregisterDevice(callback);
  }

  void DoPopupWindow(int windowID)
  {
    float height = GUI.skin.label.fontSize * 3.0f;
    float top = mPopupWindowRect.height - height - MainMenu.MARGIN;
    Rect rect = new Rect(mPopupWindowRect.xMin - MainMenu.MARGIN, top, mPopupWindowRect.width - (mPopupWindowRect.xMin - MainMenu.MARGIN) * 2, height);

    if (GUI.Button(rect, "OK"))
    {
      if (windowID == 0)
      {
        mIsSendMessageFinish = false;
        mIsSendMessageSuccess = false;
      }
      else if (windowID == 1)
      {
        mIsUnregisterFinish = false;
        mIsUnregisterSuccess = false;
      }
    }
  }
}
