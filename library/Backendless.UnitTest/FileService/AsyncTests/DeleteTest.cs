/*
Copyright 2015 Backendless Corp. All Rights Reserved.
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
using BackendlessAPI.Async;
using BackendlessAPI.File;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BackendlessAPI.Test.FileService.AsyncTests
{
  [TestClass]
  public class DeleteTest : TestsFrame
  {
    //public static string URL = "https://api.backendless.com/";
    public static string URL = "https://api.gmo-mbaas.com/";

    [TestMethod]
    public void TestDeleteSingleFile()
    {
      RunAndAwait( () =>
        {
          FileStream fileToUpload = CreateRandomFile();
          string path = GetRandomPath();

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

                                            Backendless.Files.Remove( path,
                                                                      new ResponseCallback<object>( this )
                                                                        {
                                                                          ResponseHandler = response => CountDown()
                                                                        } );
                                          }
                                      } );
        } );
    }

    [TestMethod]
    public void TestDeleteNonExistingFile()
    {
      RunAndAwait(
        () =>
        Backendless.Files.Remove( "foobarfoo",
                                  new AsyncCallback<object>(
                                    response => Assert.Fail( "Server didn't send an exception" ),
                                    fault => CheckErrorCode( 6000, fault ) ) ) );
    }

    [TestMethod]
    public void TestDeleteEmptyDirectory()
    {
      RunAndAwait( () =>
        {
          var fileToUpload = CreateRandomFile();
          var path = GetRandomPath();
          var dirName = "somedir";

          Backendless.Files.Upload( fileToUpload, dirName + "/" + path,
                                    new ResponseCallback<BackendlessFile>( this )
                                      {
                                        ResponseHandler = backendlessFile =>
                                          {
                                            Assert.IsNotNull( backendlessFile, "Server returned a null" );
                                            Assert.IsNotNull( backendlessFile.FileURL, "Server returned a null url" );
                                            Assert.AreEqual(
                                              URL + Defaults.TEST_APP_ID.ToLower() + "/" +
                                              Defaults.TEST_VERSION.ToLower() + "/files/" + dirName + "/" + path, backendlessFile.FileURL,
                                              "Server returned wrong url " + backendlessFile.FileURL );

                                            Backendless.Files.RemoveDirectory( dirName,
                                                                               new ResponseCallback<object>( this )
                                                                                 {
                                                                                   ResponseHandler = response => CountDown()
                                                                                 } );
                                          }
                                      } );
        } );
    }

    [TestMethod]
    public void TestDeleteNonExistingDirectory()
    {
      RunAndAwait(
        () =>
        Backendless.Files.RemoveDirectory( "foobarfoodir",
                                           new AsyncCallback<object>(
                                             response => Assert.Fail( "Server didn't throw an expected exception" ),
                                             fault => CheckErrorCode( 6000, fault ) ) ) );
    }

    [TestMethod]
    public void TestDeleteDirectoryWithFiles()
    {
      RunAndAwait( () =>
        {
          var fileToUpload = CreateRandomFile();
          string path = GetRandomPath();
          string dirName = "somedir";

          Backendless.Files.Upload( fileToUpload, dirName + "/" + path,
                                    new ResponseCallback<BackendlessFile>( this )
                                      {
                                        ResponseHandler = backendlessFile =>
                                          {
                                            Assert.IsNotNull( backendlessFile, "Server returned a null" );
                                            Assert.IsNotNull( backendlessFile.FileURL, "Server returned a null url" );
                                            Assert.AreEqual(
                                              URL + Defaults.TEST_APP_ID.ToLower() + "/" +
                                              Defaults.TEST_VERSION.ToLower() + "/files/" + dirName + "/" + path, backendlessFile.FileURL,
                                              "Server returned wrong url " + backendlessFile.FileURL );

                                            Backendless.Files.RemoveDirectory( dirName,
                                                                               new ResponseCallback<object>( this )
                                                                                 {
                                                                                   ResponseHandler = response => CountDown()
                                                                                 } );
                                          }
                                      } );
        } );
    }
  }
}