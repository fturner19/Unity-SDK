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
using BackendlessAPI.Test.PersistenceService.AsyncEntities.BaseEntities;

namespace BackendlessAPI.Test.PersistenceService.AsyncEntities.PrimitiveEntities
{
  public class BooleanEntityAsync: CreatedEntityAsync 
  {
    public bool BooleanField { get; set; }

    protected bool Equals( BooleanEntityAsync other )
    {
      return base.Equals( other ) && BooleanField.Equals( other.BooleanField );
    }

    public override bool Equals( object obj )
    {
      if( ReferenceEquals( null, obj ) )
        return false;
      if( ReferenceEquals( this, obj ) )
        return true;
      if( obj.GetType() != this.GetType() )
        return false;
      return Equals( (BooleanEntityAsync) obj );
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return (base.GetHashCode()*397) ^ BooleanField.GetHashCode();
      }
    }
  }
}
