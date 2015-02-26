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

public class MainMenu : MonoBehaviour
{
  public static int MARGIN = 20;
  void OnGUI()
  {
    int left = MARGIN;
    int top = MARGIN;
    int width = Screen.width - (MARGIN * 2);
    int height = ((Screen.height - MARGIN) / 8) - MARGIN;

    GUI.skin.window.fontSize = GUI.skin.label.fontSize = GUI.skin.textField.fontSize = GUI.skin.button.fontSize = (int)(height / 3.0f);
    GUI.skin.window.alignment = TextAnchor.MiddleCenter;
    GUI.skin.window.wordWrap = true;

    GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
    titleStyle.alignment = TextAnchor.MiddleCenter;
    GUI.Label(new Rect(left, top, width, height), "Backendless Unity3d SDK", titleStyle);
    top += MARGIN + height;
    if (GUI.Button(new Rect(left, top, width, height), "User Service"))
      Application.LoadLevel("UserService");
    top += MARGIN + height;
    if (GUI.Button(new Rect(left, top, width, height), "Data Service"))
      Application.LoadLevel("DataService");
    top += MARGIN + height;
    if (GUI.Button(new Rect(left, top, width, height), "File Service"))
      Application.LoadLevel("FileService");
    top += MARGIN + height;
    if (GUI.Button(new Rect(left, top, width, height), "Messaging Service"))
      Application.LoadLevel("MessagingService");
    top += MARGIN + height;
    if (GUI.Button(new Rect(left, top, width, height), "Geo Service"))
      Application.LoadLevel("GeoService");
    top += MARGIN + height;
    if (GUI.Button(new Rect(left, top, width, height), "Business Logic"))
      Application.LoadLevel("BusinessLogic");
    top += MARGIN + height;
    if (GUI.Button(new Rect(left, top, width, height), "Exit"))
      Application.Quit();
  }
}
