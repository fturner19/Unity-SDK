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

using System;
using BackendlessAPI.Persistence;

namespace BackendlessAPI.Test.PersistenceService.AsyncEntities.BaseEntities
{
  public abstract class CreatedEntityAsync : BackendlessEntity
  {
    protected bool Equals( CreatedEntityAsync other )
    {
      return base.Equals( other ) && Created.Equals( other.Created );
    }

    public override bool Equals( object obj )
    {
      if( ReferenceEquals( null, obj ) )
        return false;
      if( ReferenceEquals( this, obj ) )
        return true;
      if( obj.GetType() != this.GetType() )
        return false;
      return Equals( (CreatedEntityAsync) obj );
    }

    public override int GetHashCode()
    {
      return Created.GetHashCode();
    }
  }
}