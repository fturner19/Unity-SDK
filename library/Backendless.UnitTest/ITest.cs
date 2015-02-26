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

using BackendlessAPI.Exception;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BackendlessAPI.Test
{
  public abstract class ITest
  {
    public virtual void CheckErrorCode( string expectedCode, BackendlessFault resultFault )
    {
      СheckStringExpectation( expectedCode, resultFault.FaultCode );
    }

    public virtual void CheckErrorCode(int expectedCode, BackendlessFault resultFault)
    {
      CheckCodeExpectation( expectedCode, resultFault.FaultCode, resultFault.Message );
    }

    public virtual void CheckErrorCode( string expectedCode, System.Exception resultException )
    {
      СheckStringExpectation( expectedCode, resultException );
    }

    public virtual void CheckErrorCode( int expectedCode, System.Exception resultException )
    {
      if( resultException is BackendlessException )
        CheckCodeExpectation( expectedCode, ((BackendlessException) resultException).FaultCode, resultException.Message );
      else
        СheckStringExpectation( expectedCode.ToString(), resultException );
    }

    private void СheckStringExpectation( string expectedMessage, System.Exception actualMessage )
    {
      Assert.IsTrue( actualMessage.Message.Contains( expectedMessage ),
                     "Server returned a wrong error code. \n" + "Expected: " + expectedMessage + "\n" + "Got: " +
                     actualMessage.Message );
    }

    private void СheckStringExpectation( string expectedMessage, string actualMessage )
    {
      Assert.AreEqual( expectedMessage, actualMessage,
                       "Server returned a wrong error code. \n" + "Expected: " + expectedMessage + "\n" + "Got: " +
                       actualMessage );
    }

    private void CheckCodeExpectation( int expectedCode, string actualCode, string message )
    {
      Assert.AreEqual( expectedCode.ToString(), actualCode,
                       "Server returned a wrong error code. \n" + "Expected: " + expectedCode + "\n" + "Got: { " +
                       actualCode + "\n" + message + " }" );
    }
  }
}