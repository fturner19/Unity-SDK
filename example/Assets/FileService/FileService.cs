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
using BackendlessAPI.File;
using System.IO;

public class FileService : MonoBehaviour
{
  [SerializeField]
  private GameObject mWaiting;
  private string mFileName = "backendless.png";
  private Rect mPopupWindowRect;
  private bool mIsUploadFinish = false;
  private bool mIsRemoveFinish = false;
  private string mResultMessage = "";

  void OnGUI()
  {
    int left = MainMenu.MARGIN;
    int top = MainMenu.MARGIN;
    int width = Screen.width - (MainMenu.MARGIN * 2);
    int height = ((Screen.height - MainMenu.MARGIN) / 6) - MainMenu.MARGIN;

    if (mIsUploadFinish)
    {
      mWaiting.SetActive(false);
      mPopupWindowRect = GUI.ModalWindow(0, new Rect(MainMenu.MARGIN * 3, MainMenu.MARGIN * 3, Screen.width - ((MainMenu.MARGIN * 3) * 2), Screen.height - ((MainMenu.MARGIN * 3) * 2)), DoPopupWindow, mResultMessage);
    }
    else if (mIsRemoveFinish)
    {
      mWaiting.SetActive(false);
      mPopupWindowRect = GUI.ModalWindow(1, new Rect(MainMenu.MARGIN * 3, MainMenu.MARGIN * 3, Screen.width - ((MainMenu.MARGIN * 3) * 2), Screen.height - ((MainMenu.MARGIN * 3) * 2)), DoPopupWindow, mResultMessage);
    }
    else
    {
      GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
      titleStyle.alignment = TextAnchor.MiddleCenter;
      GUIStyle inputTitleStyle = new GUIStyle(GUI.skin.label);
      inputTitleStyle.fontSize = (int)(GUI.skin.label.fontSize / 2.0f);
      GUI.Label(new Rect(left, top, width, height), "File Service", titleStyle);
      top += MainMenu.MARGIN + height;
      GUI.Label(new Rect(left, top, width, height / 2), "File Name :", inputTitleStyle);
      top += height / 2;
      mFileName = GUI.TextField(new Rect(left, top, width, height), mFileName);
      top += MainMenu.MARGIN + height;
      if (GUI.Button(new Rect(left, top, width, height), "Upload"))
        Upload();
      top += MainMenu.MARGIN + height;
      if (GUI.Button(new Rect(left, top, width, height), "Remove"))
        Remove();
      top += MainMenu.MARGIN + height;
      if (GUI.Button(new Rect(left, top, width, height), "Back"))
        Application.LoadLevel("MainMenu");
    }
  }

  void Upload()
  {
    mWaiting.SetActive(true);

    TextAsset asset = Resources.Load("backendless_png") as TextAsset;
    Stream stream = new MemoryStream(asset.bytes);

    AsyncCallback<BackendlessFile> callback = new AsyncCallback<BackendlessFile>(
      file =>
      {
        mResultMessage = "Success\n\n\"" + mFileName + "\" uploaded.";
        mIsUploadFinish = true;
      },
      fault =>
      {
        mResultMessage = "Error\n\nCode = " + fault.FaultCode + "\nMessage = " + fault.Message;
        mIsUploadFinish = true;
      });
    Backendless.Files.Upload(stream, mFileName, callback);
  }

  void Remove()
  {
    mWaiting.SetActive(true);

    AsyncCallback<object> callback = new AsyncCallback<object>(
      o =>
      {
        mResultMessage = "Success\n\n\"" + mFileName + "\" removed.";
        mIsRemoveFinish = true;
      },
      fault =>
      {
        mResultMessage = "Error\n\nCode = " + fault.FaultCode + "\nMessage = " + fault.Message;
        mIsRemoveFinish = true;
      });
    Backendless.Files.Remove(mFileName, callback);
  }

  void DoPopupWindow(int windowID)
  {
    float height = GUI.skin.label.fontSize * 3.0f;
    float top = mPopupWindowRect.height - height - MainMenu.MARGIN;
    Rect rect = new Rect(mPopupWindowRect.xMin - MainMenu.MARGIN, top, mPopupWindowRect.width - (mPopupWindowRect.xMin - MainMenu.MARGIN) * 2, height);

    if (GUI.Button(rect, "OK"))
    {
      if (windowID == 0)
        mIsUploadFinish = false;
      else if (windowID == 1)
        mIsRemoveFinish = false;
    }
  }
}
