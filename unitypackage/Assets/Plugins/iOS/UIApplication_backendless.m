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

#import <objc/runtime.h>
#import "UIApplication_backendless.h"

char * listenerGameObject = 0;
void setDeviceId()
{
    NSString* uuidString = [[[UIDevice currentDevice] identifierForVendor] UUIDString];
    UnitySendMessage(listenerGameObject, "setDeviceId", [uuidString UTF8String]);
}

void setListenerGameObject(char * listenerName)
{
    free(listenerGameObject);
    listenerGameObject = 0;
    unsigned long len = strlen(listenerName);
    listenerGameObject = malloc(len+1);
    strcpy(listenerGameObject, listenerName);
    
    setDeviceId();
}

void registerForRemoteNotifications()
{
    if ([[UIApplication sharedApplication] respondsToSelector:@selector(registerUserNotificationSettings:)])
    {
        // iOS 8 Notifications
        [[UIApplication sharedApplication] registerUserNotificationSettings:[UIUserNotificationSettings settingsForTypes:(UIUserNotificationTypeSound | UIUserNotificationTypeAlert | UIUserNotificationTypeBadge) categories:nil]];
        
        [[UIApplication sharedApplication] registerForRemoteNotifications];
    }
    else
    {
        // iOS < 8 Notifications
        [[UIApplication sharedApplication] registerForRemoteNotificationTypes:(UIRemoteNotificationType)(UIRemoteNotificationTypeBadge | UIRemoteNotificationTypeSound | UIRemoteNotificationTypeAlert)];
    }
}

void unregisterForRemoteNotifications()
{
    [[UIApplication sharedApplication] unregisterForRemoteNotifications];

    UnitySendMessage(listenerGameObject, "unregisterDeviceOnServer", "");
}

@implementation UIApplication (backendless)
+ (void)load
{
    method_exchangeImplementations(class_getInstanceMethod(self, @selector(setDelegate:)), class_getInstanceMethod(self, @selector(setBackendlessDelegate:)));
}

BOOL backendlessRunTimeDidFinishLaunching(id self, SEL _cmd, id application, id launchOptions)
{
    [[UIApplication sharedApplication] setApplicationIconBadgeNumber:0];
    
    BOOL result = YES;
    if ([self respondsToSelector:@selector(application:backendlessDidFinishLaunchingWithOptions:)]) {
        result = (BOOL) [self application:application backendlessDidFinishLaunchingWithOptions:launchOptions];
    } else {
        [self applicationDidFinishLaunching:application];
        result = YES;
    }
    return result;
}

void backendlessRunTimeDidRegisterForRemoteNotificationsWithDeviceToken(id self, SEL _cmd, id application, id devToken)
{
    if ([self respondsToSelector:@selector(application:backendlessDidRegisterForRemoteNotificationsWithDeviceToken:)]) {
        [self application:application backendlessDidRegisterForRemoteNotificationsWithDeviceToken:devToken];
    }
    const unsigned *tokenBytes = [devToken bytes];
    NSString *hexToken = [NSString stringWithFormat:@"%08x%08x%08x%08x%08x%08x%08x%08x",
                          ntohl(tokenBytes[0]), ntohl(tokenBytes[1]), ntohl(tokenBytes[2]),
                          ntohl(tokenBytes[3]), ntohl(tokenBytes[4]), ntohl(tokenBytes[5]),
                          ntohl(tokenBytes[6]), ntohl(tokenBytes[7])];
    UnitySendMessage(listenerGameObject, "setDeviceToken", [hexToken UTF8String]);

    NSString* os = @"IOS";
    UnitySendMessage(listenerGameObject, "setOs", [os UTF8String]);

    NSString* version = [[UIDevice currentDevice] systemVersion];
    UnitySendMessage(listenerGameObject, "setOsVersion", [version UTF8String]);

    UnitySendMessage(listenerGameObject, "registerDeviceOnServer", "");
}

void backendlessRunTimeDidFailToRegisterForRemoteNotificationsWithError(id self, SEL _cmd, id application, id error)
{
    if ([self respondsToSelector:@selector(application:backendlessDidFailToRegisterForRemoteNotificationsWithError:)]) {
        [self application:application backendlessDidFailToRegisterForRemoteNotificationsWithError:error];
    }
    NSString *errorString = [error description];
    const char * str = [errorString UTF8String];
    UnitySendMessage(listenerGameObject, "onDidFailToRegisterForRemoteNotificationsWithError", str);
}

void backendlessRunTimeDidReceiveRemoteNotification(id self, SEL _cmd, id application, id userInfo)
{
    if ([self respondsToSelector:@selector(application:backendlessDidReceiveRemoteNotification:)]) {
        [self application:application backendlessDidReceiveRemoteNotification:userInfo];
    }
    
    UnitySendMessage(listenerGameObject, "onPushMessage", [[[userInfo objectForKey:@"aps"] objectForKey:@"alert"] UTF8String]);
}

void backendlessRunTimeApplicationWillEnterForeground(id self, SEL _cmd, id application)
{
    [[UIApplication sharedApplication] setApplicationIconBadgeNumber:0];
}

static void exchangeMethodImplementations(Class class, SEL oldMethod, SEL newMethod, IMP impl, const char * signature)
{
    Method method = nil;
    method = class_getInstanceMethod(class, oldMethod);
    if (method) {
        class_addMethod(class, newMethod, impl, signature);
        method_exchangeImplementations(class_getInstanceMethod(class, oldMethod), class_getInstanceMethod(class, newMethod));
    } else {
        class_addMethod(class, oldMethod, impl, signature);
    }
}

- (void) setBackendlessDelegate:(id<UIApplicationDelegate>)delegate
{
    static Class delegateClass = nil;
    
    if(delegateClass == [delegate class]) {
        [self setBackendlessDelegate:delegate];
        return;
    }
    
    delegateClass = [delegate class];
    
    exchangeMethodImplementations(delegateClass, @selector(application:didFinishLaunchingWithOptions:),
                                  @selector(application:backendlessDidFinishLaunchingWithOptions:), (IMP)backendlessRunTimeDidFinishLaunching, "v@:::");
    
    
    exchangeMethodImplementations(delegateClass, @selector(application:didRegisterForRemoteNotificationsWithDeviceToken:),
                                  @selector(application:backendlessDidRegisterForRemoteNotificationsWithDeviceToken:), (IMP)backendlessRunTimeDidRegisterForRemoteNotificationsWithDeviceToken, "v@:::");
    
    exchangeMethodImplementations(delegateClass, @selector(application:didFailToRegisterForRemoteNotificationsWithError:),
                                  @selector(application:backendlessDidFailToRegisterForRemoteNotificationsWithError:), (IMP)backendlessRunTimeDidFailToRegisterForRemoteNotificationsWithError, "v@:::");
    
    exchangeMethodImplementations(delegateClass, @selector(application:didReceiveRemoteNotification:),
                                  @selector(application:backendlessDidReceiveRemoteNotification:), (IMP)backendlessRunTimeDidReceiveRemoteNotification, "v@:::");
    
    exchangeMethodImplementations(delegateClass, @selector(applicationWillEnterForeground:),
                                  @selector(backendlessApplicationWillEnterForeground:), (IMP)backendlessRunTimeApplicationWillEnterForeground, "v@::");
    
    [self setBackendlessDelegate:delegate];
}

@end
