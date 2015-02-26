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
using System;
using System.Collections.Generic;

public class MessagingService : MonoBehaviour
{
  void OnGUI()
  {
    int left = MainMenu.MARGIN;
    int top = MainMenu.MARGIN;
    int width = Screen.width - (MainMenu.MARGIN * 2);
    int height = ((Screen.height - MainMenu.MARGIN) / 5) - MainMenu.MARGIN;

    GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
    titleStyle.alignment = TextAnchor.MiddleCenter;
    GUIStyle inputTitleStyle = new GUIStyle(GUI.skin.label);
    inputTitleStyle.fontSize = (int)(GUI.skin.label.fontSize / 2.0f);
    GUI.Label(new Rect(left, top, width, height), "Messaging Service", titleStyle);
    top += MainMenu.MARGIN + height;
    if (GUI.Button(new Rect(left, top, width, height), "Publish / Subscribe"))
      Application.LoadLevel("PublishSubscribe");
    top += MainMenu.MARGIN + height;
    if (GUI.Button(new Rect(left, top, width, height), "Push Messages"))
      Application.LoadLevel("RegisterDevice");
    top += MainMenu.MARGIN + height;
    if (GUI.Button(new Rect(left, top, width, height), "Back"))
      Application.LoadLevel("MainMenu");
  }
}
