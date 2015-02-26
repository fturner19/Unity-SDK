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

using System.IO;
using Backendless.Test;
using BackendlessAPI.File;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BackendlessAPI.Test.FileService.AsyncTests
{
  [TestClass]
  public class UploadTests : TestsFrame
  {
    //public static string URL = "https://api.backendless.com/";
    public static string URL = "https://api.gmo-mbaas.com/";

    [TestMethod]
    public void TestUploadSingleFile()
    {
      RunAndAwait( () =>
        {
          var fileToUpload = CreateRandomFile();
          var path = GetRandomPath() + "/" + GetRandomPath();

          Backendless.Files.Upload( fileToUpload, path,
                                    new ResponseCallback<BackendlessFile>( this )
                                      {
                                        ResponseHandler = backendlessFile =>
                                          {
                                            Assert.IsNotNull( backendlessFile, "Server returned a null" );
                                            Assert.IsNotNull( backendlessFile.FileURL, "Server returned a null url" );
                                            Assert.AreEqual(
                                              URL + Defaults.TEST_APP_ID.ToLower() + "/" +
                                              Defaults.TEST_VERSION.ToLower() + "/files/" + path,
                                              backendlessFile.FileURL,
                                              "Server returned wrong url " + backendlessFile.FileURL );

                                            CountDown();
                                          }
                                      } );
        } );
    }

    [TestMethod]
    public void TestUploadInvalidPath()
    {
      RunAndAwait( () =>
        {
          var fileToUpload = CreateRandomFile();
          var path = "9!@%^&*(){}[]/?|`~";

          Backendless.Files.Upload( fileToUpload, path,
                                    new ResponseCallback<BackendlessFile>( this )
                                      {
                                          ErrorHandler = fault =>
                                            CountDown()
                                      } );
        } );
    }
  }
}