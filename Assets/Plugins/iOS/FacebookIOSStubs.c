// No-op replacements for the native Facebook iOS functions that IOSFacebook.cs imports with
// [DllImport("__Internal")]. Without them the Xcode link step fails with "Undefined symbols".
// Put this file in Assets/Plugins/iOS/ of the Unity project.

void iosInit(_Bool cookie, _Bool logging, _Bool status, _Bool frictionlessRequests, const char* urlSuffix) {}

void iosLogin(const char* scope) {}

void iosLogout(void) {}

void iosSetShareDialogMode(int mode) {}

void iosFeedRequest(int requestId, const char* toId, const char* link, const char* linkName, const char* linkCaption,
                    const char* linkDescription, const char* picture, const char* mediaSource, const char* actionName,
                    const char* actionLink, const char* reference) {}

void iosAppRequest(int requestId, const char* message, const char** to, int toLength, const char* filters,
                   const char** excludeIds, int excludeIdsLength, _Bool hasMaxRecipients, int maxRecipients,
                   const char* data, const char* title) {}

void iosFBSettingsPublishInstall(int requestId, const char* appId) {}

void iosFBAppEventsLogEvent(const char* logEvent, double valueToSum, int numParams, const char** paramKeys,
                            const char** paramVals) {}

void iosFBAppEventsLogPurchase(double logPurchase, const char* currency, int numParams, const char** paramKeys,
                               const char** paramVals) {}

void iosFBAppEventsSetLimitEventUsage(_Bool limitEventUsage) {}

void iosGetDeepLink(void) {}
