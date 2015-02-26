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

public class UserServiceLogin : MonoBehaviour
{
  [SerializeField]
  private GameObject mWaiting;
  private Rect mPopupWindowRect;
  private bool mIsLogoutFinish = false;
  private bool mIsLogoutSuccess = false;
  private string mResultMessage = "";

  void OnGUI()
  {
    int left = MainMenu.MARGIN;
    int top = MainMenu.MARGIN;
    int width = Screen.width - (MainMenu.MARGIN * 2);
    int height = ((Screen.height - MainMenu.MARGIN) / 6) - MainMenu.MARGIN;

    if (mIsLogoutFinish)
    {
      mWaiting.SetActive(false);
      mPopupWindowRect = GUI.ModalWindow(0, new Rect(MainMenu.MARGIN * 3, MainMenu.MARGIN * 3, Screen.width - ((MainMenu.MARGIN * 3) * 2), Screen.height - ((MainMenu.MARGIN * 3) * 2)), DoPopupWindow, mResultMessage);
    }
    else
    {
      GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
      titleStyle.alignment = TextAnchor.MiddleCenter;
      GUIStyle inputTitleStyle = new GUIStyle(GUI.skin.label);
      inputTitleStyle.fontSize = (int)(GUI.skin.label.fontSize / 2.0f);
      GUI.Label(new Rect(left, top, width, height), "User Service - Logged In", titleStyle);
      top += MainMenu.MARGIN + height;
      GUI.Label(new Rect(left, top, width, height * 4), "You are now successfully logged in.\nThis concludes the example.\nUse the button below to logout.", titleStyle);
      top += (MainMenu.MARGIN + height) * 4;
      if (GUI.Button(new Rect(left, top, width, height), "Logout"))
        Logout();
    }
  }

  void Logout()
  {
    mWaiting.SetActive(true);

    AsyncCallback<object> callback = new AsyncCallback<object>(
      o =>
      {
        mResultMessage = "Success\n\nLogout!";
        mIsLogoutFinish = true;
        mIsLogoutSuccess = true;
      },
      fault =>
      {
        mResultMessage = "Error\n\nCode = " + fault.FaultCode + "\nMessage = " + fault.Message;
        mIsLogoutFinish = true;
        mIsLogoutSuccess = false;
      });
    Backendless.UserService.Logout(callback);
  }

  void DoPopupWindow(int windowID)
  {
    float height = GUI.skin.label.fontSize * 3.0f;
    float top = mPopupWindowRect.height - height - MainMenu.MARGIN;
    Rect rect = new Rect(mPopupWindowRect.xMin - MainMenu.MARGIN, top, mPopupWindowRect.width - (mPopupWindowRect.xMin - MainMenu.MARGIN) * 2, height);

    if (GUI.Button(rect, "OK"))
    {
      if (mIsLogoutSuccess)
        Application.LoadLevel("UserService");
      mIsLogoutFinish = false;
    }
  }
}
