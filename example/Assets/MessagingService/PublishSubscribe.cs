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

public class PublishSubscribe : MonoBehaviour
{
  [SerializeField]
  private GameObject mWaiting;
  private Rect mPopupWindowRect;
  private bool mIsPublishFinish = false;
  private bool mIsPublishSuccess = false;
  private string mResultMessage = "";
  private string mName = "my name";
  private string mChannel = "default";
  private string mPublishMessage = "";
  private Subscription mSubscription = null;
  private static List<string> mReceiveMessageList = new List<string>();

  void Start()
  {
    Subscribe();
  }

  void OnDestroy()
  {
    if (mSubscription != null)
      mSubscription.CancelSubscription();
  }

  void OnApplicationPause(bool pauseStatus)
  {
    if (mSubscription != null)
    {
      if (pauseStatus)
        mSubscription.PauseSubscription();
      else
        mSubscription.ResumeSubscription();
    }
  }

  void OnGUI()
  {
    int left = MainMenu.MARGIN;
    int top = MainMenu.MARGIN;
    int width = Screen.width - (MainMenu.MARGIN * 2);
    int height = ((Screen.height - MainMenu.MARGIN) / 11) - MainMenu.MARGIN;

    if (mIsPublishFinish)
    {
      mWaiting.SetActive(false);
      if (mIsPublishSuccess)
        mIsPublishFinish = false;
      else
        mPopupWindowRect = GUI.ModalWindow(0, new Rect(MainMenu.MARGIN * 3, MainMenu.MARGIN * 3, Screen.width - ((MainMenu.MARGIN * 3) * 2), Screen.height - ((MainMenu.MARGIN * 3) * 2)), DoPopupWindow, mResultMessage);
    }
#if false
    else if (mIsUnregisterFinish)
    {
      mWaiting.SetActive(false);
      mPopupWindowRect = GUI.ModalWindow(1, new Rect(MainMenu.MARGIN * 3, MainMenu.MARGIN * 3, Screen.width - ((MainMenu.MARGIN * 3) * 2), Screen.height - ((MainMenu.MARGIN * 3) * 2)), DoPopupWindow, mResultMessage);
    }
#endif
    else
    {
      GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
      titleStyle.alignment = TextAnchor.MiddleCenter;
      GUIStyle inputTitleStyle = new GUIStyle(GUI.skin.label);
      inputTitleStyle.fontSize = (int)(GUI.skin.label.fontSize / 2.0f);
      GUI.Label(new Rect(left, top, width, height), "Publish / Subscribe", titleStyle);
      top += MainMenu.MARGIN + height;
      GUI.Label(new Rect(left, top, width, height / 2), "Subscribe:", inputTitleStyle);
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
      GUI.Label(new Rect(left, top, width, height / 2), "Name:", inputTitleStyle);
      top += height / 2;
      mName = GUI.TextField(new Rect(left, top, width, height), mName);
      top += MainMenu.MARGIN + height;
      GUI.Label(new Rect(left, top, width, height / 2), "Channel:", inputTitleStyle);
      top += height / 2;
      mChannel = GUI.TextField(new Rect(left, top, width, height), mChannel);
      top += MainMenu.MARGIN + height;
      GUI.Label(new Rect(left, top, width, height / 2), "Message:", inputTitleStyle);
      top += height / 2;
      mPublishMessage = GUI.TextField(new Rect(left, top, width, height), mPublishMessage);
      top += MainMenu.MARGIN + height;
      if (GUI.Button(new Rect(left, top, width, height), "Publish"))
        Publish();
      top += MainMenu.MARGIN + height;
      if (GUI.Button(new Rect(left, top, width, height), "Back"))
        Application.LoadLevel("MessagingService");
    }
  }

  void Publish()
  {
    mWaiting.SetActive(true);

    PublishOptions publishOptions = new PublishOptions(mName);

    AsyncCallback<MessageStatus> callback = new AsyncCallback<MessageStatus>(
      status =>
      {
        mIsPublishFinish = true;
        mIsPublishSuccess = true;
      },
      fault =>
      {
        mResultMessage = "Error\n\nCode = " + fault.FaultCode + "\nMessage = " + fault.Message;
        mIsPublishFinish = true;
        mIsPublishSuccess = false;
      });

    Backendless.Messaging.Publish(mPublishMessage, mChannel, publishOptions, callback);
  }

  void Subscribe()
  {
    AsyncCallback<List<Message>> callback = new AsyncCallback<List<Message>>(
      messages =>
      {
        foreach (Message message in messages)
        {
          lock (mReceiveMessageList)
          {
            if (mReceiveMessageList.Count > 5)
              mReceiveMessageList.RemoveAt(0);
            mReceiveMessageList.Add(message.PublisherId + ": " + message.Data);
          }
        }
      },
      fault =>
      {
      });

    AsyncCallback<Subscription> subscriptionCallback = new AsyncCallback<Subscription>(
      subscription =>
      {
        mSubscription = subscription;
      },
      fault =>
      {
      });

    Backendless.Messaging.Subscribe(mChannel, callback, subscriptionCallback);
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
        mPublishMessage = "";
        mIsPublishFinish = false;
        mIsPublishSuccess = false;
      }
    }
  }
}
