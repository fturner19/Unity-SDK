﻿/*
Copyright 2015 Backendless Corp. All Rights Reserved.

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

using System;
using System.Globalization;
using System.IO;
using System.Text;
using Backendless.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BackendlessAPI.Test.FileService.AsyncTests
{
  [TestClass]
  public class TestsFrame : IAsyncTest
  {
    protected readonly Random Random = new Random();

    public FileStream CreateRandomFile()
    {
      var baseText = (DateTime.Now.Ticks + Random.Next()).ToString( CultureInfo.InvariantCulture );
      var file = System.IO.File.Create( baseText );
      var text = new UTF8Encoding( true ).GetBytes( baseText );

      for( int i = 0; i < 100; i++ )
        file.Write( text, 0, text.Length );

      return file;
    }

    public string GetRandomPath()
    {
      return "path_" + (DateTime.Now.Ticks + Random.Next()).ToString( CultureInfo.InvariantCulture );
    }

    [TestInitialize]
    public void SetUp()
    {
      Backendless.InitApp( Defaults.TEST_APP_ID, Defaults.TEST_SECRET_KEY, Defaults.TEST_VERSION );
    }
  }
}