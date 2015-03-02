/*
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
using System.Threading;
using BackendlessAPI.Async;
using BackendlessAPI.Exception;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BackendlessAPI.Test
{
  public abstract class IAsyncTest : ITest
  {
    public BackendlessFault testFault;
    public CountdownEvent testLatch;

    public void SetLatch()
    {
      SetLatch( 1 );
    }

    public void SetLatch( int count )
    {
      testLatch = new CountdownEvent( count );
      testFault = null;
    }

    public void CountDown()
    {
      testLatch.Signal();
    }

    public void FailCountDownWith( string message )
    {
      testFault = new BackendlessFault( message );
      testLatch.Signal();
    }

    public void FailCountDownWith( System.Exception e )
    {
      testFault = new BackendlessFault( e.Message );
      testLatch.Signal();
    }

    public void FailCountDownWith( BackendlessFault backendlessFault )
    {
      testFault = backendlessFault;
      testLatch.Signal();
    }

    public void UnlatchWith( System.Exception e )
    {
      UnlatchWith( new BackendlessFault( e.Message ) );
    }

    public void UnlatchWith( BackendlessFault backendlessFault )
    {
      testFault = backendlessFault;

      for( int i = 0; i < testLatch.CurrentCount; i++ )
        testLatch.Signal();
    }

    public void RunAndAwait( Action runnable )
    {
      if( testLatch == null || testLatch.CurrentCount == 0 )
        SetLatch();

      runnable.Invoke();
      testLatch.Wait();

      if( testFault != null )
        Assert.Fail( testFault.ToString() );
    }

    public override void CheckErrorCode( string expectedCode, BackendlessFault resultFault )
    {
      СheckStringExpectation( expectedCode, resultFault.Message );
      CountDown();
    }

    public override void CheckErrorCode( int expectedCode, BackendlessFault resultFault )
    {
      base.CheckErrorCode( expectedCode, resultFault );
      CountDown();
    }

    public override void CheckErrorCode( string expectedCode, System.Exception resultException )
    {
      base.CheckErrorCode( expectedCode, resultException );
      CountDown();
    }

    public override void CheckErrorCode( int expectedCode, System.Exception resultException )
    {
      base.CheckErrorCode( expectedCode, resultException );
      CountDown();
    }

    private void СheckStringExpectation(string expectedMessage, string actualMessage)
    {
      Assert.IsTrue(actualMessage.Contains(expectedMessage),
                     "Server returned a wrong error code. \n" + "Expected: " + expectedMessage + "\n" + "Got: " +
                     actualMessage);
    }

    protected class ResponseCallback<TE> : AsyncCallback<TE>
    {
      public ResponseCallback( IAsyncTest context ) : base( response => context.CountDown(), context.FailCountDownWith )
      {
      }
    }
  }
}