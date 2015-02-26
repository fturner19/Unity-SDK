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

using BackendlessAPI.Test.PersistenceService.Entities.BaseEntities;

namespace BackendlessAPI.Test.PersistenceService.Entities.UpdateEntities
{
  public class BaseUpdateEntity : UpdatedEntity
  {
    public int Age { get; set; }
    public string Name { get; set; }

    protected bool Equals( BaseUpdateEntity other )
    {
      return Age == other.Age && string.Equals( Name, other.Name ) && string.Equals( ObjectId, other.ObjectId ) && string.Equals( Created, other.Created );
    }

    public override bool Equals( object obj )
    {
      if( ReferenceEquals( null, obj ) )
        return false;
      if( ReferenceEquals( this, obj ) )
        return true;
      if( obj.GetType() != this.GetType() )
        return false;
      return Equals( (BaseUpdateEntity) obj );
    }

    public override int GetHashCode()
    {
      unchecked
      {
        int hashCode = base.GetHashCode();
        hashCode = (hashCode*397) ^ Age;
        hashCode = (hashCode*397) ^ (Name != null ? Name.GetHashCode() : 0);
        return hashCode;
      }
    }
  }
}