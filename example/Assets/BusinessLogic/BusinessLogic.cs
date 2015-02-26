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
using System.Collections;
using System.Collections.Generic;

public class BusinessLogic : MonoBehaviour
{
  [SerializeField]
  private GameObject mWaiting;
  private string mSend = "hello world";
  private string mReceive = "";
  private Rect mPopupWindowRect;
  private bool mIsDispatchFinish = false;
  private bool mIsDispatchSuccess = false;
  private string mResultMessage = "";

  void OnGUI()
  {
    int left = MainMenu.MARGIN;
    int top = MainMenu.MARGIN;
    int width = Screen.width - (MainMenu.MARGIN * 2);
    int height = ((Screen.height - MainMenu.MARGIN) / 7) - MainMenu.MARGIN;

    if (mIsDispatchFinish)
    {
      mWaiting.SetActive(false);
      if (mIsDispatchSuccess)
      {
        mIsDispatchFinish = false;
        mIsDispatchSuccess = false;
      }
      else
        mPopupWindowRect = GUI.ModalWindow(0, new Rect(MainMenu.MARGIN * 3, MainMenu.MARGIN * 3, Screen.width - ((MainMenu.MARGIN * 3) * 2), Screen.height - ((MainMenu.MARGIN * 3) * 2)), DoPopupWindow, mResultMessage);
    }
    else
    {
      GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
      titleStyle.alignment = TextAnchor.MiddleCenter;
      GUIStyle inputTitleStyle = new GUIStyle(GUI.skin.label);
      inputTitleStyle.fontSize = (int)(GUI.skin.label.fontSize / 2.0f);
      GUI.Label(new Rect(left, top, width, height), "Business Logic", titleStyle);
      top += MainMenu.MARGIN + height;
      GUI.Label(new Rect(left, top, width, height / 2), "Send :", inputTitleStyle);
      top += height / 2;
      mSend = GUI.TextField(new Rect(left, top, width, height), mSend);
      top += MainMenu.MARGIN + height;
      GUI.Label(new Rect(left, top, width, height / 2), "Receive :", inputTitleStyle);
      top += height / 2;
      mReceive = GUI.TextField(new Rect(left, top, width, height), mReceive);
      top += MainMenu.MARGIN + height;
      if (GUI.Button(new Rect(left, top, width, height), "ECHO event dispatch"))
        Dispatch();
      top += MainMenu.MARGIN + height;
      if (GUI.Button(new Rect(left, top, width, height), "Back"))
        Application.LoadLevel("MainMenu");
    }
  }

  void Dispatch()
  {
    mWaiting.SetActive(true);

    AsyncCallback<IDictionary> callback = new AsyncCallback<IDictionary>(
      result =>
      {
        foreach (DictionaryEntry r in result)
        {
          mReceive = (string)r.Value;
        }
        mIsDispatchFinish = true;
        mIsDispatchSuccess = true;
      },
      fault =>
      {
        mResultMessage = "Error\n\nCode = " + fault.FaultCode + "\nMessage = " + fault.Message;
        mIsDispatchFinish = true;
        mIsDispatchSuccess = false;
      });
    Dictionary<string, object> eventArgs = new Dictionary<string, object>();
    eventArgs.Add("echoKey", mSend);
    Backendless.Events.Dispatch("echo", eventArgs, callback);
    // CodeRunner source
    /*
    @BackendlessEvent( "echo" )
    public class EchoEventHandler extends com.backendless.servercode.extension.CustomEventHandler
    {

      @Override
      public Map handleEvent( RunnerContext context, Map eventArgs )
      {
      System.out.println("eventArgs=" + eventArgs);
      return eventArgs; // echo
      }

    }
    */
  }

  void DoPopupWindow(int windowID)
  {
    float height = GUI.skin.label.fontSize * 3.0f;
    float top = mPopupWindowRect.height - height - MainMenu.MARGIN;
    Rect rect = new Rect(mPopupWindowRect.xMin - MainMenu.MARGIN, top, mPopupWindowRect.width - (mPopupWindowRect.xMin - MainMenu.MARGIN) * 2, height);

    if (GUI.Button(rect, "OK"))
    {
      mIsDispatchFinish = false;
      mIsDispatchSuccess = false;
    }
  }
}
