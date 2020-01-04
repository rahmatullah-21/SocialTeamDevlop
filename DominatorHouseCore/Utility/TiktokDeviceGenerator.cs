using ProtoBuf;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouseCore.Utility
{
    public class TiktokDeviceGenerator
    {
        public TiktokDeviceGenerator()
        {
            GenerateDetails();
        }

        [ProtoMember(1)]
        public string DeviceType { get; private set; }

        [ProtoMember(2)]
        public string SystemRegion { get; private set; }

        [ProtoMember(3)]
        public string DeviceId { get; set; }

        [ProtoMember(4)]
        public string Region { get; private set; }

        [ProtoMember(5)]
        public string Brand { get; set; }

        [ProtoMember(6)]
        public string Dpi { get; set; }

        [ProtoMember(7)]
        public string Resolution { get; set; }
        [ProtoMember(8)]
        public string Useragent { get; set; }
        [ProtoMember(9)]
        public string AndroidVersion { get; set; }
        [ProtoMember(10)]
        public string Model { get; set; }
        [ProtoMember(11)]
        public string Device { get; set; }
        [ProtoMember(12)]
        public string IId { get; set; }
        public void GenerateDetails()
        {
            var splitDeviceDetails = GetRandomDevice().Split(':');

            var splitAndroidDetails = splitDeviceDetails[0].Split(';');
            AndroidVersion = splitAndroidDetails[2].Split(' ')[2];
            SystemRegion = splitAndroidDetails[3].Split('_')[1];
            Model = splitAndroidDetails[4];
            Device = splitAndroidDetails[5].Split('/')[1];
            Useragent = splitDeviceDetails[0];
            DeviceType = splitDeviceDetails[1];
            Resolution = splitDeviceDetails[2];
            Dpi = splitDeviceDetails[3];
            Brand = splitDeviceDetails[4];
            DeviceId = GetRandomDeviceId(); //splitDeviceDetails[1];
            IId = GetIId(DeviceId);
        }

        /// <summary>
        /// GetRandomDevice method is used to get the random device details from the array of list
        /// </summary>
        /// <returns>Return the any one from list of devices</returns>
        private static string GetRandomDevice()
        {
            // retrun any one from device items
            return new[]
            {
               "com.zhiliaoapp.musically / 2018110931(Linux; U; Android 8.1.0; en_US; Moto G(5) Plus; Build/OPS28.85 - 17 - 6 - 2; Cronet / 58.0.2991.0):Moto+G+%285%29+Plus:1080*1776:480:motorola",
               "com.zhiliaoapp.musically / 2018110931(Linux; U; Android 8.0.0; en_US; ASUS_Z012DB; Build/OPR1.170623.026; Cronet / 58.0.2991.0):ASUS_Z012DB:1080*1920:480:asus",
               "com.zhiliaoapp.musically / 2018110931(Linux; U; Android 5.1.1; en_US; vivo V3Max; Build/LMY47V; Cronet / 58.0.2991.0):vivo+V3Max:1080*1920:480:vivo",
               //"com.zhiliaoapp.musically / 2018110931(Linux; U; Android 4.1.2; en_GB; SM - N910G; Build / JZO54K; Cronet / 58.0.2991.0):6760618510928741893:SM-N910G:480*800:240:samsung",
               "com.zhiliaoapp.musically / 2018110931(Linux; U; Android 5.1; en_US; A1601; Build/LMY47I; Cronet / 58.0.2991.0):A1601:720*1280:320:OPPO",
               "com.zhiliaoapp.musically / 2018110931(Linux; U; Android 6.0; en_GB; CPH1609; Build/MRA58K; Cronet / 58.0.2991.0):CPH1609:1080*1920:480:OPPO",
               "com.zhiliaoapp.musically / 2018110931(Linux; U; Android 6.0; en_IN; Lenovo A7020a48; Build/MRA58K; Cronet / 58.0.2991.0):Lenovo+A7020a48:1080*1920:480:Lenovo",
               "com.zhiliaoapp.musically / 2018110931(Linux; U; Android 6.0; en_US; vivo 1601; Build/MRA58K; Cronet / 58.0.2991.0):vivo+1601:720*1280:320:vivo",
               "com.zhiliaoapp.musically / 2018110931(Linux; U; Android 6.0.1; en_US; Redmi 3S; Build/MMB29M; Cronet / 58.0.2991.0):Redmi+3S:720*1280:320:Xiaomi",
               "com.zhiliaoapp.musically/2018110931 (Linux; U; Android 6.0; en_IN; Lenovo A7020a48; Build/MRA58K; Cronet/58.0.2991.0):Lenovo+A7020a48:1080*1920:480:Lenovo",
               "com.zhiliaoapp.musically/2018110931 (Linux; U; Android 8.0.0; en_IN; G3416; Build/48.1.A.2.122; Cronet/58.0.2991.0):G3416:1080*1776:480:Sony",
               "com.zhiliaoapp.musically/2018110931 (Linux; U; Android 8.0.0; en_IN; ASUS_Z012DB; Build/OPR1.170623.026; Cronet/58.0.2991.0):ASUS_Z012DB:1080*1920:480:asus"
            }.GetRandomItem();
        }

        private static Dictionary<string, string> DeviceIds = new Dictionary<string, string>
        {
            {"6585385698721908230", "6757918322598004486"},
            {"6720473090419623429", "6720473796165863174"},
            {"6630751755615569414", "6761387620859512582"},
            {"6777588069886608901", "6777588267183310598"},
            {"6746793713215129093", "6777592082356111109"},
            {"6760704845549831686", "6777595420233058053"},
            {"6760586893559285254", "6777601720819418886"},
            {"6760592492963972614", "6777604894855333638"},
            {"6729395505317283334", "6777606434089699078"},
            {"6746787552278595077", "6777607007597709062"},
            {"6774702208153601541", "6777609850875070214"},
            {"6760594866655315461", "6777630106111969029"},
            {"6760579683085846022", "6777630829042239237"},
            {"6724207280285419014", "6777631288230299398"},
        };

        public static string GetRandomDeviceId()
        {
            return DeviceIds.Keys.ToArray()[RandomUtilties.ObjRandom.Next(0, DeviceIds.Count)];
        }

        public static string GetIId(string deviceId)
        {
            return DeviceIds[deviceId];
        }
    }
}
