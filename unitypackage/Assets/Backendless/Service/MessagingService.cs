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
using System.Collections.Generic;
using System.Globalization;
using BackendlessAPI.Async;
using BackendlessAPI.Engine;
using BackendlessAPI.Exception;
using BackendlessAPI.Messaging;
using Subscription = BackendlessAPI.Messaging.Subscription;
using BackendlessAPI.LitJson;

namespace BackendlessAPI.Service
{
  public class MessagingService
  {
    private static string DEFAULT_CHANNEL_NAME = "default";

    private static Messaging.DeviceRegistration _deviceRegistration;
    private static AsyncCallback<string> _deviceRegisterCallback = null;
    private static AsyncCallback<bool> _deviceUnregisterCallback = null;

    public delegate void UnityRegisterDevice(string GCMSenderID, long timestamp);
    public delegate void UnityUnregisterDevice();
    private UnityRegisterDevice _unityRegisterDevice;
    private UnityUnregisterDevice _unityUnregisterDevice;

    public void SetUnityRegisterDevice(UnityRegisterDevice unityRegisterDevice, UnityUnregisterDevice unityUnregisterDevice)
    {
      _unityRegisterDevice = unityRegisterDevice;
      _unityUnregisterDevice = unityUnregisterDevice;
    }

    public MessagingService()
    {
      _deviceRegistration = new DeviceRegistration();
    }

    public DeviceRegistration DeviceRegistration
    {
      get { return _deviceRegistration; }
    }

    public void RegisterDevice(string GCMSenderID)
    {
      RegisterDevice(GCMSenderID, (AsyncCallback<string>)null);
    }

    public void RegisterDevice(string GCMSenderID, AsyncCallback<string> callback)
    {
      RegisterDevice(GCMSenderID, DEFAULT_CHANNEL_NAME, callback);
    }

    public void RegisterDevice(string GCMSenderID, string channel)
    {
      RegisterDevice(GCMSenderID, channel, (AsyncCallback<string>)null);
    }

    public void RegisterDevice(string GCMSenderID, string channel, AsyncCallback<string> callback)
    {
      if (string.IsNullOrEmpty(channel))
        throw new ArgumentNullException(ExceptionMessage.NULL_CHANNEL_NAME);

      RegisterDevice(GCMSenderID, new List<string> { channel }, callback);
    }

    public void RegisterDevice(string GCMSenderID, List<string> channels)
    {
      RegisterDevice(GCMSenderID, channels, (AsyncCallback<string>)null);
    }

    public void RegisterDevice(string GCMSenderID, List<string> channels, AsyncCallback<string> callback)
    {
      RegisterDevice(GCMSenderID, channels, null, callback);
    }

    public void RegisterDevice(string GCMSenderID, DateTime expiration)
    {
      RegisterDevice(GCMSenderID, expiration, (AsyncCallback<string>)null);
    }

    public void RegisterDevice(string GCMSenderID, DateTime expiration, AsyncCallback<string> callback)
    {
      RegisterDevice(GCMSenderID, null, expiration, callback);
    }

    public void RegisterDevice(string GCMSenderID, List<string> channels, DateTime? expiration)
    {
      RegisterDevice(GCMSenderID, channels, expiration, (AsyncCallback<string>)null);
    }

    public void RegisterDevice(string GCMSenderID, List<string> channels, DateTime? expiration, AsyncCallback<string> callback)
    {
      if (channels == null)
        throw new ArgumentNullException(ExceptionMessage.NULL_CHANNEL_NAME);

      if (channels.Count == 0)
        _deviceRegistration.AddChannel(DEFAULT_CHANNEL_NAME);

      foreach (string channel in channels)
      {
        checkChannelName(channel);
        _deviceRegistration.AddChannel(channel);
      }

      _deviceRegistration.Expiration = expiration;

      _deviceRegisterCallback = callback;

      long timestamp;
      if (_deviceRegistration.Timestamp == null)
        timestamp = 0;
      else
        timestamp = (long)_deviceRegistration.Timestamp;
      _unityRegisterDevice(GCMSenderID, timestamp);
    }

    public void RegisterDeviceOnServer()
    {
      AsyncCallback<string> callback = _deviceRegisterCallback;

      AsyncCallback<Dictionary<string, object>> responder = new AsyncCallback<Dictionary<string, object>>(response =>
      {
        if (callback != null)
          callback.ResponseHandler.Invoke(GetRegisterDeviceOnServerResult(response));
      }, fault =>
      {
        if (callback != null)
          callback.ErrorHandler.Invoke(fault);
        else
          throw new BackendlessException(fault);
      });

      Invoker.InvokeAsync<Dictionary<string, object>>(Invoker.Api.MESSAGINGSERVICE_REGISTERDEVICEONSERVER, new Object[] { GetRegisterDeviceOnServerRequestData(_deviceRegistration) }, responder);
    }

    public List<DeviceRegistration> GetRegistrations()
    {
      return Invoker.InvokeSync<List<DeviceRegistration>>(Invoker.Api.MESSAGINGSERVICE_GETREGISTRATION, new Object[] { null, GetDeviceId() });
    }

    public void GetRegistrations(AsyncCallback<List<DeviceRegistration>> callback)
    {
      Invoker.InvokeAsync<List<DeviceRegistration>>(Invoker.Api.MESSAGINGSERVICE_GETREGISTRATION, new Object[] { null, GetDeviceId() }, callback);
    }

    public void UnregisterDevice(AsyncCallback<bool> callback)
    {
      _deviceUnregisterCallback = callback;

      _unityUnregisterDevice();
    }

    public void UnregisterDeviceOnServer()
    {
      AsyncCallback<bool> callback = _deviceUnregisterCallback;

      AsyncCallback<Dictionary<string, object>> responder = new AsyncCallback<Dictionary<string, object>>(response =>
      {
        if (callback != null)
          callback.ResponseHandler.Invoke(GetUnregisterDeviceOnServerResult(response));
      }, fault =>
      {
        if (callback != null)
          callback.ErrorHandler.Invoke(fault);
        else
          throw new BackendlessException(fault);
      });

      Invoker.InvokeAsync<Dictionary<string, object>>(Invoker.Api.MESSAGINGSERVICE_UNREGISTERDEVICEONSERVER, new Object[] { null, _deviceRegistration.DeviceId }, responder);
    }


    #region PUBLISH SYNC (DEFAULT CHANNEL)

    public Messaging.MessageStatus Publish(object message)
    {
      return Publish(message, DEFAULT_CHANNEL_NAME);
    }

    public Messaging.MessageStatus Publish(object message, Messaging.PublishOptions publishOptions)
    {
      return Publish(message, DEFAULT_CHANNEL_NAME, publishOptions);
    }

    public Messaging.MessageStatus Publish(object message, Messaging.DeliveryOptions deliveryOptions)
    {
      return Publish(message, DEFAULT_CHANNEL_NAME, null, deliveryOptions);
    }

    public Messaging.MessageStatus Publish(object message, Messaging.PublishOptions publishOptions,
                                     Messaging.DeliveryOptions deliveryOptions)
    {
      return Publish(message, DEFAULT_CHANNEL_NAME, publishOptions, deliveryOptions);
    }

    #endregion

    #region PUBLISH ASYNC (DEFAULT CHANNEL)

    public void Publish(object message, AsyncCallback<MessageStatus> callback)
    {
      Publish(message, DEFAULT_CHANNEL_NAME, callback);
    }

    public void Publish(object message, Messaging.PublishOptions publishOptions, AsyncCallback<MessageStatus> callback)
    {
      Publish(message, DEFAULT_CHANNEL_NAME, publishOptions, callback);
    }

    public void Publish(object message, Messaging.DeliveryOptions deliveryOptions, AsyncCallback<MessageStatus> callback)
    {
      Publish(message, DEFAULT_CHANNEL_NAME, null, deliveryOptions, callback);
    }

    public void Publish(object message, Messaging.PublishOptions publishOptions, Messaging.DeliveryOptions deliveryOptions,
                         AsyncCallback<MessageStatus> callback)
    {
      Publish(message, DEFAULT_CHANNEL_NAME, publishOptions, deliveryOptions, callback);
    }

    #endregion

    #region PUBLISH SYNC

    public Messaging.MessageStatus Publish(object message, string channelName)
    {
      return PublishSync(message, channelName, null, null);
    }

    public Messaging.MessageStatus Publish(object message, string channelName, Messaging.PublishOptions publishOptions)
    {
      return PublishSync(message, channelName, publishOptions, null);
    }

    public Messaging.MessageStatus Publish(object message, string channelName, Messaging.DeliveryOptions deliveryOptions)
    {
      return PublishSync(message, channelName, null, deliveryOptions);
    }

    public Messaging.MessageStatus Publish(object message, string channelName, Messaging.PublishOptions publishOptions,
                                     Messaging.DeliveryOptions deliveryOptions)
    {
      return PublishSync(message, channelName, publishOptions, deliveryOptions);
    }

    private Messaging.MessageStatus PublishSync(object message, string channelName, Messaging.PublishOptions publishOptions,
                                          Messaging.DeliveryOptions deliveryOptions)
    {
      checkChannelName(channelName);

      if (message == null)
        throw new ArgumentNullException(ExceptionMessage.NULL_MESSAGE);

      return Invoker.InvokeSync<Messaging.MessageStatus>(Invoker.Api.MESSAGINGSERVICE_PUBLISH, new object[] { GetPublishRequestData(message, publishOptions, deliveryOptions), channelName });
    }

    #endregion

    #region PUBLISH ASYNC

    public void Publish(object message, string channelName, AsyncCallback<MessageStatus> callback)
    {
      Publish(message, channelName, null, null, callback);
    }

    public void Publish(object message, string channelName, Messaging.PublishOptions publishOptions,
                         AsyncCallback<MessageStatus> callback)
    {
      Publish(message, channelName, publishOptions, null, callback);
    }

    public void Publish(object message, string channelName, Messaging.DeliveryOptions deliveryOptions,
                         AsyncCallback<MessageStatus> callback)
    {
      Publish(message, channelName, null, deliveryOptions, callback);
    }

    public void Publish(object message, string channelName, Messaging.PublishOptions publishOptions,
                         Messaging.DeliveryOptions deliveryOptions, AsyncCallback<MessageStatus> callback)
    {
      checkChannelName(channelName);

      if (message == null)
        throw new ArgumentNullException(ExceptionMessage.NULL_MESSAGE);

      Invoker.InvokeAsync(Invoker.Api.MESSAGINGSERVICE_PUBLISH, new object[] { GetPublishRequestData(message, publishOptions, deliveryOptions), channelName }, callback);
    }

    #endregion

    public bool Cancel(string messageId)
    {
      if (string.IsNullOrEmpty(messageId))
        throw new ArgumentNullException(ExceptionMessage.NULL_MESSAGE_ID);

      Dictionary<string, object> response = Invoker.InvokeSync<Dictionary<string, object>>(Invoker.Api.MESSAGINGSERVICE_CANCEL, new Object[] { null, messageId });
      return GetCancelResult(response);
    }

    public void Cancel(string messageId, AsyncCallback<bool> callback)
    {
      if (string.IsNullOrEmpty(messageId))
        throw new ArgumentNullException(ExceptionMessage.NULL_MESSAGE_ID);

      AsyncCallback<Dictionary<string, object>> responder = new AsyncCallback<Dictionary<string, object>>(response =>
      {
        if (callback != null)
          callback.ResponseHandler.Invoke(GetCancelResult(response));
      }, fault =>
      {
        if (callback != null)
          callback.ErrorHandler.Invoke(fault);
      });

      Invoker.InvokeAsync<Dictionary<string, object>>(Invoker.Api.MESSAGINGSERVICE_CANCEL, new Object[] { null, messageId }, responder);
    }

    #region SUBSCRIBE SYNC (DEFAULT CHANNEL)

    public Messaging.Subscription Subscribe(AsyncCallback<List<Message>> callback)
    {
      return Subscribe(DEFAULT_CHANNEL_NAME, callback);
    }

    public Messaging.Subscription Subscribe(int pollingInterval, AsyncCallback<List<Message>> callback)
    {
      return Subscribe(DEFAULT_CHANNEL_NAME, pollingInterval, callback);
    }

    public Messaging.Subscription Subscribe(AsyncCallback<List<Message>> callback,
                                      Messaging.SubscriptionOptions subscriptionOptions)
    {
      return Subscribe(DEFAULT_CHANNEL_NAME, callback, subscriptionOptions);
    }

    public Messaging.Subscription Subscribe(int pollingInterval, AsyncCallback<List<Message>> callback,
                                      Messaging.SubscriptionOptions subscriptionOptions)
    {
      return Subscribe(DEFAULT_CHANNEL_NAME, pollingInterval, callback, subscriptionOptions);
    }

    #endregion

    #region SUBSCRIBE SYNC

    public Messaging.Subscription Subscribe(string channelName, AsyncCallback<List<Message>> callback)
    {
      return Subscribe(channelName, 0, callback, new Messaging.SubscriptionOptions());
    }

    public Messaging.Subscription Subscribe(string channelName, int pollingInterval, AsyncCallback<List<Message>> callback)
    {
      return Subscribe(channelName, pollingInterval, callback, new Messaging.SubscriptionOptions());
    }

    public Messaging.Subscription Subscribe(string channelName, AsyncCallback<List<Message>> callback,
                                      Messaging.SubscriptionOptions subscriptionOptions)
    {
      return Subscribe(channelName, 0, callback, subscriptionOptions);
    }

    public Messaging.Subscription Subscribe(string channelName, int pollingInterval, AsyncCallback<List<Message>> callback,
                                      Messaging.SubscriptionOptions subscriptionOptions)
    {
      checkChannelName(channelName);

      if (pollingInterval < 0)
        throw new ArgumentException(ExceptionMessage.WRONG_POLLING_INTERVAL);

      Messaging.Subscription subscription = subscribeForPollingAccess(channelName, subscriptionOptions);

      subscription.ChannelName = channelName;

      if (pollingInterval != 0)
        subscription.PollingInterval = pollingInterval;

      subscription.OnSubscribe(callback);

      return subscription;
    }

    #endregion

    #region SUBSCRIBE ASYNC (DEFAULT CHANNEL)

    public void Subscribe(int pollingInterval, AsyncCallback<List<Message>> callback,
                           AsyncCallback<Subscription> subscriptionCallback)
    {
      Subscribe(DEFAULT_CHANNEL_NAME, pollingInterval, callback, null, subscriptionCallback);
    }

    public void Subscribe(AsyncCallback<List<Message>> callback, AsyncCallback<Subscription> subscriptionCallback)
    {
      Subscribe(DEFAULT_CHANNEL_NAME, 0, callback, null, subscriptionCallback);
    }

    public void Subscribe(AsyncCallback<List<Message>> callback, Messaging.SubscriptionOptions subscriptionOptions,
                           AsyncCallback<Subscription> subscriptionCallback)
    {
      Subscribe(DEFAULT_CHANNEL_NAME, 0, callback, subscriptionOptions, subscriptionCallback);
    }

    public void Subscribe(int pollingInterval, AsyncCallback<List<Message>> callback,
                           Messaging.SubscriptionOptions subscriptionOptions,
                           AsyncCallback<Subscription> subscriptionCallback)
    {
      Subscribe(DEFAULT_CHANNEL_NAME, pollingInterval, callback, subscriptionOptions, subscriptionCallback);
    }

    #endregion

    #region SUBSCRIBE ASYNC

    public void Subscribe(string channelName, AsyncCallback<List<Message>> callback,
                           AsyncCallback<Subscription> subscriptionCallback)
    {
      Subscribe(channelName, 0, callback, null, subscriptionCallback);
    }

    public void Subscribe(string channelName, int pollingInterval, AsyncCallback<List<Message>> callback,
                           AsyncCallback<Subscription> subscriptionCallback)
    {
      Subscribe(channelName, pollingInterval, callback, null, subscriptionCallback);
    }

    public void Subscribe(string channelName, AsyncCallback<List<Message>> callback,
                           Messaging.SubscriptionOptions subscriptionOptions,
                           AsyncCallback<Subscription> subscriptionCallback)
    {
      Subscribe(channelName, 0, callback, subscriptionOptions, subscriptionCallback);
    }

    public void Subscribe(string channelName, int pollingInterval, AsyncCallback<List<Message>> callback,
                           Messaging.SubscriptionOptions subscriptionOptions,
                           AsyncCallback<Subscription> subscriptionCallback)
    {
      checkChannelName(channelName);
      if (pollingInterval < 0)
        throw new ArgumentException(ExceptionMessage.WRONG_POLLING_INTERVAL);

      var responder = new AsyncCallback<Subscription>(r =>
        {
          Messaging.Subscription subscription = r;
          subscription.ChannelName = channelName;

          if (pollingInterval != 0)
            subscription.PollingInterval = pollingInterval;

          subscription.OnSubscribe(callback);

          if (subscriptionCallback != null)
            subscriptionCallback.ResponseHandler.Invoke(subscription);
        }, f =>
          {
            if (subscriptionCallback != null)
              subscriptionCallback.ErrorHandler.Invoke(f);
            else
              throw new BackendlessException(f);
          });
      subscribeForPollingAccess(channelName, subscriptionOptions, responder);
    }

    #endregion

    public List<Message> PollMessages(string channelName, string subscriptionId)
    {
      checkChannelName(channelName);

      if (string.IsNullOrEmpty(subscriptionId))
        throw new ArgumentNullException(ExceptionMessage.NULL_SUBSCRIPTION_ID);

      try
      {
        Dictionary<string, List<Message>> response = Invoker.InvokeSync<Dictionary<string, List<Message>>>(Invoker.Api.MESSAGINGSERVICE_POLLMESSAGES,
                                                     new object[]
                                                     {
                                                       null, channelName,
                                                       subscriptionId
                                                     });
        if (response != null && response.ContainsKey("messages"))
          return (List<Message>)response["messages"];
        else
          return new List<Message>();
      }
      catch (BackendlessException)
      {
        return new List<Message>();
      }
    }

    public void PollMessages(string channelName, string subscriptionId, AsyncCallback<List<Message>> callback)
    {
      checkChannelName(channelName);

      if (string.IsNullOrEmpty(subscriptionId))
        throw new ArgumentNullException(ExceptionMessage.NULL_SUBSCRIPTION_ID);

      AsyncCallback<Dictionary<string, List<Message>>> responder = new AsyncCallback<Dictionary<string, List<Message>>>(
        response =>
        {
          List<Message> result = null;
          if (response != null && response.ContainsKey("messages"))
            result = (List<Message>)response["messages"];
          else
            result = new List<Message>();

          if (callback != null)
            callback.ResponseHandler.Invoke(result);
        }, fault =>
      {
        if (callback != null)
          callback.ErrorHandler.Invoke(fault);
      });

      Invoker.InvokeAsync<Dictionary<string, List<Message>>>(Invoker.Api.MESSAGINGSERVICE_POLLMESSAGES, new object[] { null, channelName, subscriptionId }, responder);
    }

    #region SEND EMAIL
    public void SendTextEmail(String subject, String messageBody, List<String> recipients)
    {
      SendEmail(subject, new BodyParts(messageBody, null), recipients, new List<String>());
    }

    public void SendTextEmail(String subject, String messageBody, String recipient)
    {
      SendEmail(subject, new BodyParts(messageBody, null), new List<String>() { recipient }, new List<String>());
    }

    public void SendHTMLEmail(String subject, String messageBody, List<String> recipients)
    {
      SendEmail(subject, new BodyParts(null, messageBody), recipients, new List<String>());
    }

    public void SendHTMLEmail(String subject, String messageBody, String recipient)
    {
      SendEmail(subject, new BodyParts(null, messageBody), new List<String>() { recipient }, new List<String>());
    }

    public void SendEmail(String subject, BodyParts bodyParts, String recipient, List<String> attachments)
    {
      SendEmail(subject, bodyParts, new List<String>() { recipient }, attachments);
    }

    public void SendEmail(String subject, BodyParts bodyParts, String recipient)
    {
      SendEmail(subject, bodyParts, new List<String>() { recipient }, new List<String>());
    }

    public void SendEmail(String subject, BodyParts bodyParts, List<String> recipients, List<String> attachments)
    {
      if (subject == null)
        throw new ArgumentNullException(ExceptionMessage.NULL_SUBJECT);

      if (bodyParts == null)
        throw new ArgumentNullException(ExceptionMessage.NULL_BODYPARTS);

      if (recipients == null || recipients.Count == 0)
        throw new ArgumentNullException(ExceptionMessage.NULL_RECIPIENTS);

      if (attachments == null)
        throw new ArgumentNullException(ExceptionMessage.NULL_ATTACHMENTS);

      Dictionary<string, object> requestData = new Dictionary<string, object>();
      requestData.Add("subject", subject);
      requestData.Add("bodyparts", bodyParts);
      if (recipients.Count > 0)
        requestData.Add("to", recipients);
      if (attachments.Count > 0)
        requestData.Add("attachment", attachments);

      Invoker.InvokeSync<object>(Invoker.Api.MESSAGINGSERVICE_SENDEMAIL, new Object[] { requestData });
    }

    public void SendTextEmail(String subject, String messageBody, List<String> recipients, AsyncCallback<object> responder)
    {
      SendEmail(subject, new BodyParts(messageBody, null), recipients, new List<String>(), responder);
    }

    public void SendTextEmail(String subject, String messageBody, String recipient, AsyncCallback<object> responder)
    {
      SendEmail(subject, new BodyParts(messageBody, null), new List<String>() { recipient }, new List<String>(), responder);
    }

    public void SendHTMLEmail(String subject, String messageBody, List<String> recipients, AsyncCallback<object> responder)
    {
      SendEmail(subject, new BodyParts(null, messageBody), recipients, new List<String>(), responder);
    }

    public void SendHTMLEmail(String subject, String messageBody, String recipient, AsyncCallback<object> responder)
    {
      SendEmail(subject, new BodyParts(null, messageBody), new List<String>() { recipient }, new List<String>(), responder);
    }

    public void SendEmail(String subject, BodyParts bodyParts, String recipient, List<String> attachments, AsyncCallback<object> responder)
    {
      SendEmail(subject, bodyParts, new List<String>() { recipient }, attachments, responder);
    }

    public void SendEmail(String subject, BodyParts bodyParts, String recipient, AsyncCallback<object> responder)
    {
      SendEmail(subject, bodyParts, new List<String>() { recipient }, new List<String>(), responder);
    }

    public void SendEmail(String subject, BodyParts bodyParts, List<String> recipients, List<String> attachments,
                           AsyncCallback<object> responder)
    {
      if (subject == null)
        throw new ArgumentNullException(ExceptionMessage.NULL_SUBJECT);

      if (bodyParts == null)
        throw new ArgumentNullException(ExceptionMessage.NULL_BODYPARTS);

      if (recipients == null || recipients.Count == 0)
        throw new ArgumentNullException(ExceptionMessage.NULL_RECIPIENTS);

      if (attachments == null)
        throw new ArgumentNullException(ExceptionMessage.NULL_ATTACHMENTS);

      Dictionary<string, object> requestData = new Dictionary<string, object>();
      requestData.Add("subject", subject);
      requestData.Add("bodyparts", bodyParts);
      if (recipients.Count > 0)
        requestData.Add("to", recipients);
      if (attachments.Count > 0)
        requestData.Add("attachment", attachments);

      Invoker.InvokeAsync(Invoker.Api.MESSAGINGSERVICE_SENDEMAIL, new Object[] { requestData }, responder);
    }
    #endregion

    private Subscription subscribeForPollingAccess(string channelName, Messaging.SubscriptionOptions subscriptionOptions)
    {
      checkChannelName(channelName);

      if (subscriptionOptions == null)
        subscriptionOptions = new Messaging.SubscriptionOptions();

      return Invoker.InvokeSync<Subscription>(Invoker.Api.MESSAGINGSERVICE_SUBSCRIBE, new Object[] { GetSubscribeRequestData(subscriptionOptions), channelName });
    }

    private void subscribeForPollingAccess(string channelName, Messaging.SubscriptionOptions subscriptionOptions,
                                            AsyncCallback<Subscription> callback)
    {
      checkChannelName(channelName);

      if (subscriptionOptions == null)
        subscriptionOptions = new Messaging.SubscriptionOptions();

      Invoker.InvokeAsync(Invoker.Api.MESSAGINGSERVICE_SUBSCRIBE, new Object[] { GetSubscribeRequestData(subscriptionOptions), channelName }, callback);
    }

    private void checkChannelName(string channelName)
    {
      if (string.IsNullOrEmpty(channelName))
        throw new ArgumentNullException(ExceptionMessage.NULL_CHANNEL_NAME);
    }

    private static Dictionary<string, object> GetPublishRequestData(object message, Messaging.PublishOptions publishOptions, Messaging.DeliveryOptions deliveryOptions)
    {
      Dictionary<string, object> requestData = new Dictionary<string, object>();
      requestData.Add("message", message);
      if (publishOptions != null)
      {
        if (string.IsNullOrEmpty(publishOptions.PublisherId) == false)
          requestData.Add("publisherId", publishOptions.PublisherId);
        if (publishOptions.Headers != null && publishOptions.Headers.Count > 0)
          requestData.Add("headers", publishOptions.Headers);
        if (string.IsNullOrEmpty(publishOptions.Subtopic) == false)
          requestData.Add("subtopic", publishOptions.Subtopic);
      }

      if (deliveryOptions != null)
      {
        if (string.IsNullOrEmpty(deliveryOptions.PushPolicy.ToString()) == false)
          requestData.Add("pushPolicy", deliveryOptions.PushPolicy.ToString());

        string pushBroadcastStr = "";
        if (deliveryOptions.PushBroadcast == DeliveryOptions.ALL)
          pushBroadcastStr = "ALL";
        else
        {
          if ((deliveryOptions.PushBroadcast & DeliveryOptions.IOS) > 0)
          {
            pushBroadcastStr = "IOS";
          }
          if ((deliveryOptions.PushBroadcast & DeliveryOptions.ANDROID) > 0)
          {
            if (string.IsNullOrEmpty(pushBroadcastStr) == false)
              pushBroadcastStr += "|";
            pushBroadcastStr = "ANDROID";
          }
          if ((deliveryOptions.PushBroadcast & DeliveryOptions.WP) > 0)
          {
            if (string.IsNullOrEmpty(pushBroadcastStr) == false)
              pushBroadcastStr += "|";
            pushBroadcastStr = "WP";
          }
        }
        requestData.Add("pushBroadcast", pushBroadcastStr);

        if (deliveryOptions.PushSinglecast != null && deliveryOptions.PushSinglecast.Count > 0)
          requestData.Add("pushSinglecast", deliveryOptions.PushSinglecast);

        if (deliveryOptions.PublishAt != null)
          requestData.Add("publishAt", deliveryOptions.PublishAt.Value.Ticks);

        if (deliveryOptions.RepeatEvery > 0)
          requestData.Add("repeatEvery", deliveryOptions.RepeatEvery);

        if (deliveryOptions.RepeatExpiresAt != null)
          requestData.Add("repeatExpiresAt", deliveryOptions.RepeatExpiresAt.Value.Ticks);
      }

      return requestData;
    }

    private static bool GetCancelResult(Dictionary<string, object> response)
    {
      bool cancelled = false;
      try
      {
        string status = (string)response["status"];
        if (status != null && status.Equals("CANCELLED"))
          cancelled = true;
      }
      catch (System.Exception)
      {
      }
      return cancelled;
    }

    private static Dictionary<string, object> GetSubscribeRequestData(Messaging.SubscriptionOptions subscriptionOptions)
    {
      Dictionary<string, object> requestData = new Dictionary<string, object>();
      if (string.IsNullOrEmpty(subscriptionOptions.SubscriberId) == false)
        requestData.Add("subscriberId", subscriptionOptions.SubscriberId);
      if (string.IsNullOrEmpty(subscriptionOptions.Subtopic) == false)
        requestData.Add("subtopic", subscriptionOptions.Subtopic);
      if (string.IsNullOrEmpty(subscriptionOptions.Selector) == false)
        requestData.Add("selector", subscriptionOptions.Selector);

      return requestData;
    }

    private static Dictionary<string, object> GetRegisterDeviceOnServerRequestData(DeviceRegistration deviceRegistration)
    {
      Dictionary<string, object> requestData = new Dictionary<string, object>();
      requestData.Add("deviceToken", deviceRegistration.DeviceToken);
      requestData.Add("deviceId", deviceRegistration.DeviceId);
      requestData.Add("os", deviceRegistration.Os);
      requestData.Add("osVersion", deviceRegistration.OsVersion);
      if (deviceRegistration.Channels != null)
        requestData.Add("channels", deviceRegistration.Channels);
      if (deviceRegistration.Expiration != null)
        requestData.Add("expiration", deviceRegistration.Timestamp);

      return requestData;
    }

    private static string GetRegisterDeviceOnServerResult(Dictionary<string, object> response)
    {
      string registrationId = null;
      try
      {
        registrationId = (string)response["registrationId"];
      }
      catch (System.Exception)
      {
      }
      return registrationId;
    }

    private static string GetDeviceId()
    {
      return _deviceRegistration.DeviceId;
    }

    private static bool GetUnregisterDeviceOnServerResult(Dictionary<string, object> response)
    {
      bool result = false;
      try
      {
        result = (bool)response["result"];
      }
      catch (System.Exception)
      {
      }
      return result;
    }
  }
}