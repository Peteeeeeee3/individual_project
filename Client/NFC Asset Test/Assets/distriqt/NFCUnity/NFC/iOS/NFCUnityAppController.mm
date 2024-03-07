#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

#import "UnityAppController.h"

#define DTCoreApplicationContinueUserActivityNotification @"dtUnityApplicationContinueUserActivityNotification"

@interface NFCUnityAppController : UnityAppController

@end


@implementation NFCUnityAppController

// - (BOOL)application:(UIApplication *)app openURL:(NSURL *)url options:(NSDictionary<NSString *,id> *)options 
// {
// }


- (BOOL)  application: (UIApplication *)application 
 continueUserActivity: (nonnull NSUserActivity *)userActivity
   restorationHandler: (nonnull void (^)(NSArray<id<UIUserActivityRestoring>> * _Nullable))restorationHandler
{
    NSLog( @"DISTRIQT::NFCUnityAppController::continueUserActivity");
    
    NSDictionary* userInfo = [[NSDictionary alloc] initWithObjectsAndKeys:
							  userActivity, @"userActivity",
							  nil ];
	
	[[NSNotificationCenter defaultCenter] postNotificationName: @"dtUnityApplicationContinueUserActivityNotification"
														object: nil
													  userInfo: userInfo ];
    
    return YES;
}


@end


IMPL_APP_CONTROLLER_SUBCLASS(NFCUnityAppController)
