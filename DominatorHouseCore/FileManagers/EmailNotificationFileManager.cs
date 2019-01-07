using System;
using System.IO;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;
using CommonServiceLocator;

namespace DominatorHouseCore.FileManagers
{
   public class EmailNotificationFileManager
    {
        public static bool SaveEmailNotification<T>(T emailNotification) where T : class
        {
            try
            {
                using (var stream = File.Create(ServiceLocator.Current.GetInstance<IConstantVariable>().GetOtherEmailNotificationFile()))
                {
                    Serializer.Serialize(stream, emailNotification);
                    return true;
                }

            }
            catch (Exception ex)
            {
               
                ex.DebugLog();
                return false;
            }
           
        }
        public static EmailNotificationsModel GetEmailNotifications()
        {
            try
            {
                using (var stream = File.OpenRead(ServiceLocator.Current.GetInstance<IConstantVariable>().GetOtherEmailNotificationFile()))
                {
                    return Serializer.Deserialize<EmailNotificationsModel>(stream);
                }
            }
            catch (Exception ex)
            {
                 ex.DebugLog();
            }
            return null;
        }
    }
}
