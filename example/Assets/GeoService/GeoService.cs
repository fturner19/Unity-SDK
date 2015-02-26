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
using BackendlessAPI.Data;
using BackendlessAPI.Geo;
using System.Collections.Generic;

public class GeoService : MonoBehaviour
{
  [SerializeField]
  private GameObject mWaiting;
  private Rect mPopupWindowRect;
  private bool mIsGetPointsFinish = false;
  private bool mIsGetPointsSuccess = false;
  private string mResultMessage = "";
  private double mLatitude = 33.099657;
  private double mLongitude = -96.825244;
  private float mRadius = 0;
  private string mPoints = "";

  void OnGUI()
  {
    int left = MainMenu.MARGIN;
    int top = MainMenu.MARGIN;
    int width = Screen.width - (MainMenu.MARGIN * 2);
    int height = ((Screen.height - MainMenu.MARGIN) / 8) - MainMenu.MARGIN;

    if (mIsGetPointsFinish)
    {
      mWaiting.SetActive(false);
      if (mIsGetPointsSuccess)
        mPopupWindowRect = GUI.ModalWindow(0, new Rect(MainMenu.MARGIN * 3, MainMenu.MARGIN * 3, Screen.width - ((MainMenu.MARGIN * 3) * 2), Screen.height - ((MainMenu.MARGIN * 3) * 2)), DoPopupWindow, mResultMessage);
      else
      {
        mIsGetPointsFinish = false;
        mIsGetPointsSuccess = false;
      }
    }
    else
    {
      GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
      titleStyle.alignment = TextAnchor.MiddleCenter;
      GUIStyle inputTitleStyle = new GUIStyle(GUI.skin.label);
      inputTitleStyle.fontSize = (int)(GUI.skin.label.fontSize / 2.0f);
      GUI.Label(new Rect(left, top, width, height), "Geo Service", titleStyle);
      top += MainMenu.MARGIN + height;
      GUI.Box(new Rect(left, top, width, (MainMenu.MARGIN + height) * 3), "");
      GUI.Label(new Rect(left, top, width, (MainMenu.MARGIN + height) * 3), mPoints, inputTitleStyle);
      top += (MainMenu.MARGIN + height) * 3;
      inputTitleStyle.alignment = TextAnchor.MiddleCenter;
      GUI.Label(new Rect(left, top, width, height / 2), "latutude : " + mLatitude + ", longitude : " + mLongitude, inputTitleStyle);
      top += height / 2;
      GUI.Label(new Rect(left, top, width, height / 2), "radius (km) : " + (int)mRadius, inputTitleStyle);
      top += height / 2;
      mRadius = GUI.HorizontalSlider(new Rect(left, top, width, height / 2), mRadius, 0.0f, 40000.0f);
      top += height / 2;
      if (GUI.Button(new Rect(left, top, width, height), "GetPoints"))
        GetPoints();
      top += MainMenu.MARGIN + height;
      if (GUI.Button(new Rect(left, top, width, height), "Back"))
        Application.LoadLevel("MainMenu");
    }
  }

  void GetPoints()
  {
    mWaiting.SetActive(true);

    List<string> categories = new List<string>();
    categories.Add("geoservice_sample");
    BackendlessGeoQuery query = new BackendlessGeoQuery(mLatitude, mLongitude, mRadius, Units.KILOMETERS, categories);

    AsyncCallback<BackendlessCollection<GeoPoint>> callback = new AsyncCallback<BackendlessCollection<GeoPoint>>(
      collection =>
      {
        mPoints = "";
        foreach (GeoPoint point in collection.GetCurrentPage())
        {
          mPoints += "City:" + point.Metadata["city"] + ", Lat:" + point.Latitude + ", Lon:" + point.Longitude + "\n";
        }
        mIsGetPointsFinish = true;
        mIsGetPointsSuccess = false;
      },
      fault =>
      {
        mResultMessage = "Error\n\nCode = " + fault.FaultCode + "\nmessage = " + fault.Message;
        mIsGetPointsFinish = true;
        mIsGetPointsSuccess = true;
      });

    Backendless.Geo.GetPoints(query, callback);
  }

  void DoPopupWindow(int windowID)
  {
    float height = GUI.skin.label.fontSize * 3.0f;
    float top = mPopupWindowRect.height - height - MainMenu.MARGIN;
    Rect rect = new Rect(mPopupWindowRect.xMin - MainMenu.MARGIN, top, mPopupWindowRect.width - (mPopupWindowRect.xMin - MainMenu.MARGIN) * 2, height);

    if (GUI.Button(rect, "OK"))
    {
        mIsGetPointsFinish = false;
        mIsGetPointsSuccess = false;
    }
  }
}
