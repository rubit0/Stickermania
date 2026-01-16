using SA.CrossPlatform.UI;
using SA.iOS.UIKit;
using UnityEngine;

public class UM_AppFeaturesExample : MonoBehaviour
{
   private void Start()
   {
      iOSSetup();
   }

   private void iOSSetup()
   {
      //External URL Calls
      var url = ISN_UIApplication.ApplicationDelegate.GetLaunchURL();
      if(!string.IsNullOrEmpty(url)) {
         UM_DialogsUtility.ShowMessage("GetLaunchURL", "App is launched via external url");
      }
      
      ISN_UIApplication.ApplicationDelegate.OpenURL.AddListener((string openUrl) => {
         UM_DialogsUtility.ShowMessage("OpenURL", "URL request received: " + openUrl);
      });
   }  
}
