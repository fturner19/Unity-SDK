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
using BackendlessAPI.Persistence;
using BackendlessAPI.Data;
using System.Collections.Generic;

public class DataService : MonoBehaviour
{
  [SerializeField]
  private GameObject mWaiting;
  private string mTodo = "";
  private Rect mPopupWindowRect;
  private bool mIsSaveFinish = false;
  private bool mIsRemoveFinish = false;
  private bool mIsFindFinish = false;
  private bool mIsFindSuccess = false;
  private string mResultMessage = "";
  private Vector2 mScrollPosition = Vector2.zero;
  private List<TodoList> mTodoLists = null;

  class TodoList : BackendlessEntity
  {
    public string Todo { get; set; }
  }

  void Start()
  {
    mTodoLists = new List<TodoList>();
    Find();
  }

  void OnGUI()
  {
    int left = MainMenu.MARGIN;
    int top = MainMenu.MARGIN;
    int width = Screen.width - (MainMenu.MARGIN * 2);
    int height = ((Screen.height - MainMenu.MARGIN) / 9) - MainMenu.MARGIN;

    if (mIsSaveFinish)
    {
      mWaiting.SetActive(false);
      mPopupWindowRect = GUI.ModalWindow(0, new Rect(MainMenu.MARGIN * 3, MainMenu.MARGIN * 3, Screen.width - ((MainMenu.MARGIN * 3) * 2), Screen.height - ((MainMenu.MARGIN * 3) * 2)), DoPopupWindow, mResultMessage);
    }
    else if (mIsRemoveFinish)
    {
      mWaiting.SetActive(false);
      mPopupWindowRect = GUI.ModalWindow(1, new Rect(MainMenu.MARGIN * 3, MainMenu.MARGIN * 3, Screen.width - ((MainMenu.MARGIN * 3) * 2), Screen.height - ((MainMenu.MARGIN * 3) * 2)), DoPopupWindow, mResultMessage);
    }
    else if (mIsFindFinish)
    {
      mWaiting.SetActive(false);
      if(mIsFindSuccess)
        mIsFindFinish = false;
      else
        mPopupWindowRect = GUI.ModalWindow(2, new Rect(MainMenu.MARGIN * 3, MainMenu.MARGIN * 3, Screen.width - ((MainMenu.MARGIN * 3) * 2), Screen.height - ((MainMenu.MARGIN * 3) * 2)), DoPopupWindow, mResultMessage);
    }
    else
    {
      GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
      titleStyle.alignment = TextAnchor.MiddleCenter;
      GUIStyle inputTitleStyle = new GUIStyle(GUI.skin.label);
      inputTitleStyle.fontSize = (int)(GUI.skin.label.fontSize / 2.0f);
      GUI.Label(new Rect(left, top, width, height), "Data Service", titleStyle);
      top += MainMenu.MARGIN + height;
      GUI.Label(new Rect(left, top, width, height / 2), "Todo List :", inputTitleStyle);
      top += height / 2;
      mScrollPosition = GUI.BeginScrollView(new Rect(left, top, width, (MainMenu.MARGIN + height) * 4), mScrollPosition, new Rect(0, 0, width - MainMenu.MARGIN*2, (MainMenu.MARGIN + height) * mTodoLists.Count));
      int todolistTop = 0;
      foreach(TodoList todolist in mTodoLists)
      {
        if (GUI.Button(new Rect(MainMenu.MARGIN, todolistTop, width - MainMenu.MARGIN*2, height), todolist.Todo))
          Remove(todolist);
        todolistTop += MainMenu.MARGIN + height;
      }
      GUI.EndScrollView();
      top += (MainMenu.MARGIN + height)*4;
      GUI.Label(new Rect(left, top, width, height / 2), "Todo :", inputTitleStyle);
      top += height / 2;
      mTodo = GUI.TextField(new Rect(left, top, width, height), mTodo);
      top += MainMenu.MARGIN + height;
      if (GUI.Button(new Rect(left, top, width, height), "Save"))
        Save();
      top += MainMenu.MARGIN + height;
      if (GUI.Button(new Rect(left, top, width, height), "Back"))
        Application.LoadLevel("MainMenu");
    }
  }

  void Save()
  {
    mWaiting.SetActive(true);

    TodoList todolist = new TodoList();
    todolist.Todo = mTodo;

    AsyncCallback<TodoList> callback = new AsyncCallback<TodoList>(
      savedTodoList =>
      {
        mResultMessage = "Success\n\n\"" + savedTodoList.Todo + "\" saved.";
        mIsSaveFinish = true;
      },
      fault =>
      {
        mResultMessage = "Error\n\nCode = " + fault.FaultCode + "\nMessage = " + fault.Message;
        mIsSaveFinish = true;
      });
    Backendless.Persistence.Of<TodoList>().Save(todolist, callback);
  }

  void Remove(TodoList todolist)
  {
    AsyncCallback<long> callback = new AsyncCallback<long>(
      deletionTime =>
      {
        mResultMessage = "Success\n\nremoved.";
        mIsRemoveFinish = true;
      },
      fault =>
      {
        mResultMessage = "Error\n\nCode = " + fault.FaultCode + "\nMessage = " + fault.Message;
        mIsRemoveFinish = true;
      });
    Backendless.Persistence.Of<TodoList>().Remove(todolist, callback);
  }

  void Find()
  {
    mWaiting.SetActive(true);

    AsyncCallback<BackendlessCollection<TodoList>> callback = new AsyncCallback<BackendlessCollection<TodoList>>(
      collection =>
      {
        mTodoLists = collection.GetCurrentPage();
        mIsFindFinish = true;
        mIsFindSuccess = true;
      },
      fault =>
      {
        if (fault.FaultCode.Equals("1009") == true)
        {
          mIsFindFinish = true;
          mIsFindSuccess = true;
        }
        else
        {
          mResultMessage = "Error\n\nCode = " + fault.FaultCode + "\nMessage = " + fault.Message;
          mIsFindFinish = true;
          mIsFindSuccess = false;
        }
      });
    Backendless.Persistence.Of<TodoList>().Find(new BackendlessDataQuery(), callback);
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
        mIsSaveFinish = false;
        mTodo = "";
        Find();
      }
      else if (windowID == 1)
      {
        mIsRemoveFinish = false;
        Find();
      }
      else if (windowID == 2)
        mIsFindFinish = false;
    }
  }
}
