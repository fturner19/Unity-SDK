﻿/*
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

namespace BackendlessAPI.Exception
{
  public class ExceptionMessage
  {
    public const string NUL_WEBBROWSER = "WebBrowser cannot be null";
    public const string ILLEGAL_ARGUMENT_EXCEPTION = "IllegalArgumentException";
    public const string SERVER_ERROR = "Server returned an error.";
    public const string CLIENT_ERROR = "Internal client exception.";

    public const string WRONG_MANIFEST = "Wrong dependencies at the manifest";
    public const string NOT_INITIALIZED = "Backendless application was not initialized";

    public const string NULL_USER = "User cannot be null or empty.";
    public const string NULL_ROLE_NAME = "Role cannot be null or empty";
    public const string NULL_PASSWORD = "User password cannot be null or empty.";
    public const string NULL_LOGIN = "User login cannot be null or empty.";

    public const string NULL_CONTEXT =
        "Context cannot be null. Use Backendless.initApp( Context context, String applicationId, String secretKey, String version ) for proper initialization.";

    public const string NULL_CATEGORY_NAME = "Category name cannot be null or empty.";
    public const string NULL_GEOPOINT = "Geopoint cannot be null.";
    public const string DEFAULT_CATEGORY_NAME = "cannot add or delete a default category name.";

    public const string NULL_ENTITY = "Entity cannot be null.";
    public const string NULL_ENTITY_NAME = "Entity name cannot be null or empty.";
    public const string NULL_ID = "Id cannot be null or empty.";

    public const string NULL_UNIT = "Unit type cannot be null or empty.";

    public const string NULL_CHANNEL_NAME = "Channel name cannot be null or empty.";
    public const string NULL_MESSAGE = "Message cannot be null. Use an empty string instead.";
    public const string NULL_MESSAGE_ID = "Message id cannot be null or empty.";
    public const string NULL_SUBSCRIPTION_ID = "Subscription id cannot be null or empty.";

    public const string NULL_FILE = "File cannot be null.";
    public const string NULL_PATH = "File path cannot be null or empty.";
    public const string NULL_BITMAP = "Bitmap cannot be null";
    public const string NULL_COMPRESS_FORMAT = "CompressFormat cannot be null";

    public const string NULL_IDENTITY = "Identity cannot be null or empty.";

    public const string NULL_APPLICATION_ID = "Application id cannot be null";
    public const string NULL_SECRET_KEY = "Secret key cannot be null";
    public const string NULL_VERSION = "Version cannot be null";
    public const string NULL_DEVICE_TOKEN = "Null device token received";

    public const string WRONG_RADIUS = "Wrong radius value.";

    public const string WRONG_SEARCH_RECTANGLE_QUERY =
        "Wrong rectangle search query. It should contain four points.";

    public const string WRONG_FILE = "cannot read the file.";

    public const string WRONG_LATITUDE_VALUE = "Latitude value should be between -90 and 90.";
    public const string WRONG_LONGITUDE_VALUE = "Longitude value should be between -180 and 180.";

    public const string WRONG_USER_ID = "User not logged in or wrong user id.";

    public const string WRONG_GEO_QUERY = "Could not understand Geo query options. Specify any.";

    public const string INCONSISTENT_GEO_QUERY =
        "Inconsistent geo query. Query should not contain both rectangle and radius search parameters.";

    public const string WRONG_OFFSET = "Offset cannot have a negative value.";
    public const string WRONG_PAGE_SIZE = "Pagesize cannot have a negative value.";

    public const string WRONG_SUBSCRIPTION_STATE = "cannot resume a subscription, which is not paused.";
    public const string WRONG_EXPIRATION_DATE = "Wrong expiration date";

    public const string WRONG_POLLING_INTERVAL = "Wrong polling interval";
    public const string DEVICE_NOT_REGISTERED = "Device is not registered.";

    public const string NOT_READABLE_FILE = "File is not readable.";
    public const string FILE_UPLOAD_ERROR = "Could not upload a file";

    public const string ENTITY_MISSING_DEFAULT_CONSTRUCTOR = "No default noargument constructor";
    public const string ENTITY_WRONG_OBJECT_ID_FIELD_TYPE = "Wrong type of the objectId field";
    public const string ENTITY_WRONG_CREATED_FIELD_TYPE = "Wrong type of the created field";
    public const string ENTITY_WRONG_UPDATED_FIELD_TYPE = "Wrong type of the updated field";
    public const string WRONG_ENTITY_TYPE = "Wrong entity type";

    public const string SERVICE_NOT_DECLARED =
        "com.backendless.AndroidService is not declared at the application manifest";

    public const string LOCAL_FILE_EXISTS = "Local file exists";
    public const string WRONG_REMOTE_PATH = "Remote path cannot be empty";

    public const string NO_DEVICEID_CAPABILITY =
        "In order to use Backendless SDK in WindowsPhone environment, application should have an 'ID_CAP_IDENTITY_DEVICE' capability";
    public const string NULL_GEO_QUERY = "Geo query should not be null";
    public const string INCONSISTENT_GEO_RELATIVE = "Geo query should contain relative metadata and a threshold for a relative search";
    public const string NULL_BODYPARTS = "BodyParts cannot be null";
    public const string NULL_SUBJECT = "Subject cannot be null";
    public const string NULL_RECIPIENTS = "Recipients cannot be empty";
    public const string NULL_ATTACHMENTS = "Attachments cannot be null";
  }
}