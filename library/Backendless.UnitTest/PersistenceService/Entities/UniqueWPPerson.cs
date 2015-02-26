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

using System;

namespace BackendlessAPI.Test.PersistenceService.Entities
{
  public class UniqueWPPerson : WPPerson
  {
    public DateTime Birthday { get; set; }

    protected bool Equals( UniqueWPPerson other )
    {
      return Birthday.Equals( other.Birthday );
    }

    public override bool Equals( object obj )
    {
      if( ReferenceEquals( null, obj ) )
        return false;
      if( ReferenceEquals( this, obj ) )
        return true;
      if( obj.GetType() != this.GetType() )
        return false;
      return Equals( (UniqueWPPerson) obj );
    }

    public override int GetHashCode()
    {
      return Birthday.GetHashCode();
    }
  }
}