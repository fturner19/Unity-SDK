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

public class UserServiceRegister : MonoBehaviour
{
  [SerializeField]
  private GameObject mWaiting;
  private string mName = "";
  private string mEmail = "";
  private string mPassword = "";
  private string mVerifyPassword = "";
  private Rect mPopupWindowRect;
  private bool mIsRegisterFinish = false;
  private bool mIsRegisterSuccess = false;
  private string mResultMessage = "";

  void OnGUI()
  {
    int left = MainMenu.MARGIN;
    int top = MainMenu.MARGIN;
    int width = Screen.width - (MainMenu.MARGIN * 2);
    int height = ((Screen.height - MainMenu.MARGIN) / 9) - MainMenu.MARGIN;

    if (mIsRegisterFinish)
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
      GUI.Label(new Rect(left, top, width, height), "User Service - Register", titleStyle);
      top += MainMenu.MARGIN + height;
      GUI.Label(new Rect(left, top, width, height / 2), "Name :", inputTitleStyle);
      top += height / 2;
      mName = GUI.TextField(new Rect(left, top, width, height), mName);
      top += MainMenu.MARGIN + height;
      GUI.Label(new Rect(left, top, width, height / 2), "Password :", inputTitleStyle);
      top += height / 2;
      mPassword = GUI.PasswordField(new Rect(left, top, width, height), mPassword, '*');
      top += MainMenu.MARGIN + height;
      GUI.Label(new Rect(left, top, width, height / 2), "Verify Password :", inputTitleStyle);
      top += height / 2;
      mVerifyPassword = GUI.PasswordField(new Rect(left, top, width, height), mVerifyPassword, '*');
      top += MainMenu.MARGIN + height;
      GUI.Label(new Rect(left, top, width, height / 2), "Email :", inputTitleStyle);
      top += height / 2;
      mEmail = GUI.TextField(new Rect(left, top, width, height), mEmail);
      top += MainMenu.MARGIN + height;

      if (GUI.Button(new Rect(left, top, width, height), "Register"))
        Register();
      top += MainMenu.MARGIN + height;
      if (GUI.Button(new Rect(left, top, width, height), "Back"))
        Application.LoadLevel("UserService");
    }
  }

  void Register()
  {
    mWaiting.SetActive(true);

    BackendlessUser user = new BackendlessUser();
    user.Email = mEmail;
    user.Password = mPassword;
    user.AddProperty("name", mName);

    AsyncCallback<BackendlessUser> callback = new AsyncCallback<BackendlessUser>(
      savedUser =>
      {
        mResultMessage = "Success\n\nThank you for registering!\nYou can use your email to login.";
        mIsRegisterFinish = true;
        mIsRegisterSuccess = true;
      },
      fault =>
      {
        mResultMessage = "Error\n\nCode = " + fault.FaultCode + "\nMessage = " + fault.Message;
        mIsRegisterFinish = true;
        mIsRegisterSuccess = false;
      });
    Backendless.UserService.Register(user, callback);
  }

  void DoPopupWindow(int windowID)
  {
    float height = GUI.skin.label.fontSize * 3.0f;
    float top = mPopupWindowRect.height - height - MainMenu.MARGIN;
    Rect rect = new Rect(mPopupWindowRect.xMin - MainMenu.MARGIN, top, mPopupWindowRect.width - (mPopupWindowRect.xMin - MainMenu.MARGIN) * 2, height);

    if (GUI.Button(rect, "OK"))
    {
      if (mIsRegisterSuccess)
        Application.LoadLevel("UserService");
      mIsRegisterFinish = false;
    }
  }
}
