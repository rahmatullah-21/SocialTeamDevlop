using System.ComponentModel;
using ProtoBuf;
using System.Text;

namespace DominatorHouseCore.Utility
{
    [Localizable(false)]
    [ProtoContract]
    public class DeviceGenerator
    {
        public DeviceGenerator()
        {
            GenerateDetails();
        }

        [ProtoMember(6)]
        public string AndroidRelease { get; private set; }

        [ProtoMember(7)]
        public string AndroidVersion { get; private set; }

        [ProtoMember(1)]
        public string Device { get; private set; }

        [ProtoMember(2)]
        public string DeviceId { get;  set; }

        [ProtoMember(3)]
        public string Manufacturer { get; private set; }

        [ProtoMember(4)]
        public string Model { get; private set; }

        [ProtoMember(5)]
        public string PhoneId { get; private set; }

        public string Useragent =>
            string.Format(ConstantVariable.UseragentCommonFormat, (object)ConstantVariable.IgVersion, (object)AndroidVersion, (object)AndroidRelease, (object)Dpi, (object)Resolution, (object)ManufacturerBrand, (object)Model, (object)Device, (object)Cpu, (object)ConstantVariable.UseragentLocale+ "; 125398471");

        //"Instagram 10.33.0 Android ({1}/{2}; {3}; {4}; {5}; {6}; {7}; {8}; {9})"
        //"Instagram 6.21.2 Android 23/6.0.1; 640dpi; 1440x2560; ZTE; ZTE A2017U; ailsa_ii; qcom;en_US";
        //Instagram 37.0.0.5.97 Android (23/6.0.1; 480dpi; 1080x1920; Samsung;a8hplte;SM-A800IZ; qcom; en_US)

        [ProtoMember(8)]
        private string Brand { get; set; }

        [ProtoMember(9)]
        private string Cpu { get; set; }

        [ProtoMember(10)]
        private string Dpi { get; set; }

        private string ManufacturerBrand
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Brand))
                    return Manufacturer;
                return string.Format("{0}/{1}", Manufacturer, Brand);
            }
        }

        [ProtoMember(11)]
        private string Resolution { get; set; }
        [ProtoMember(12)]
        public string AdId { get; set; }

        [ProtoMember(13)]
        public string Guid { get; set; }

        public void GenerateDetails()
        {
            var splitDeviceDetails = GetRandomDevice().Split(';');
            var splitAndroidDetails = splitDeviceDetails[0].Split('/');
            AndroidVersion = splitAndroidDetails[0];
            AndroidRelease = splitAndroidDetails[1];

            Dpi = splitDeviceDetails[1];
            Resolution = splitDeviceDetails[2];

            var splitManufacture = splitDeviceDetails[3].Split('/');
            Manufacturer = splitManufacture[0];
            if (splitManufacture.Length == 2)
                Brand = splitManufacture[1];

            Model = splitDeviceDetails[4];
            Device = splitDeviceDetails[5];
            Cpu = splitDeviceDetails[6];
            PhoneId = Utilities.GetGuid(); ;
            DeviceId = Utilities.GetMobileDeviceId();
            AdId = Utilities.GetGuid();
            Guid = Utilities.GetGuid();
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
                "23/6.0.1; 480dpi; 1080x1920; Samsung; GT - I5800L; GT - I5800L; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;GT-N7000B;GT-N7000B; qcom; en_US",
                "26/8.0.0; 480dpi; 1080x1920; Samsung;GT-P7300B;GT-P7300B; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;GT-P7320T;GT-P7320T; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;GT-P7500M;GT-P7500M; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;GT-P7500R;GT-P7500R; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;GT-P7500V;GT-P7500V; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;GT-S5698;GT-S5698; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;GT-S5820;GT-S5820; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;GT-S5830V;GT-S5830V; qcom; en_US",
                "26/8.0.0; 480dpi; 1080x1920; Samsung;SCH-I339;SCH-I339; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;SCH-I405U;SCH-I405U; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;SCH-I509U;SCH-I509U; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;SCH-I519;SCH-I519; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;SCH-I559;SCH-i559; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;SCH-I639;SCH-I639; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;SCH-I659;SCH-I659; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;SCH-I699;SCH-I699; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;SCH-I779;SCH-I779; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;SCH-I919U;SCH-I919U; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;SCH-W899;SCH-W899; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;SCH-W999;SCH-W999; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;SCH-i809;SCH-i809; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;SCH-i919;SCH-i919; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;SGH-I987;SGH-I987; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;SGH_T939;Behold II; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;SHV-E150S;SHV-E150S; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;SHW-M115S;SHW-M115S; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;SHW-M135K;SHW-M135K; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;SHW-M135L;SHW-M135L; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;SHW-M340D;SHW-M340D; qcom; en_US",
                "26/8.0.0; 480dpi; 1080x1920; Samsung;SHW-M340K;SHW-M340K; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;SHW-M460D;SHW-M460D; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;amazingtrf;SGH-S730G; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;aruba3gchn;GT-I8262D; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;espressorfcmcc;GT-P3108; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;gd1ltektt;EK-KC120K; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;godivaltevzw;SCH-I425; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;ironcmcc;GT-B9388; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;m0grandectc;SCH-W2013; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;m0grandectc;SCH-W9913; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;p4noterfskt;SHW-M480S; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;stunnerltespr;SPH-L500; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;superiorchn;GT-I9260; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;a7xltechn;SM-A710XZ; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;GT-B9120;GT-B9120; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;SCH-R880;SCH-R880; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;SCH-R720;SCH-R720; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;amazingtrf;SGH-S730M; qcom; en_US",
                "26/8.0.0; 480dpi; 1080x1920; Samsung;baffinltelgt;SHV-E270L; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;SGH-I927;SAMSUNG-SGH-I927; qcom; en_US",
                "23/6.0.1; 480dpi; 1080x1920; Samsung;SGH-I927;SGH-I927; qcom; en_US",
                "24 / 7.0; 640dpi; 1080x1920; OnePlus; ONEPLUS A3010; OnePlus3T; qcom; en_US",
                "23/6.0.1; 640dpi; 1440x2392; LGE/lge; RS988; h1; h1; en_US",
                "24/7.0; 640dpi; 1440x2560; HUAWEI; LON-L29; HWLON; hi3660; en_US",
                "23/6.0.1; 640dpi; 1440x2560; ZTE; ZTE A2017U; ailsa_ii; qcom; en_US",
                "23/6.0.1; 640dpi; 1440x2560; samsung; SM-G935F; hero2lte; samsungexynos8890; en_US",
                "23/6.0.1; 640dpi; 1440x2560; samsung; SM-G930F; herolte; samsungexynos8890; en_US",
                "23/6.0; 640dpi; 1920x1152; Yota; RN2; RN2; trident; en_US",
                "23/6.0.1; 640dpi; 1440x2560; samsung; SM-G935F; hero2lte; samsungexynos8890; en_US",
            }.GetRandomItem();
        }


        /// <summary>
        /// Generate a unique id for each device
        /// </summary>
        /// <returns></returns>
        //public static string GenerateGuid()
        //{
        //    var rand = new System.Random();
        //    return string.Format("{0}{1}-{2}-{3}-{4}-{5}{6}{7}",
        //        rand.Next(0, 65535).ToString("x4"),
        //        rand.Next(0, 65535).ToString("x4"),
        //        rand.Next(0, 65535).ToString("x4"),
        //        rand.Next(16384, 20479).ToString("x4"),
        //        rand.Next(32768, 49151).ToString("x4"),
        //        rand.Next(0, 65535).ToString("x4"),
        //        rand.Next(0, 65535).ToString("x4"),
        //        rand.Next(0, 65535).ToString("x4"));
        //}


        /// <summary>
        /// Generate Signature
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>

        public static string GenerateSignature(string data)
        {
            string secret = Constants.IG_SIG_KEY;
            var secretBytes = Encoding.UTF8.GetBytes(secret);
            var dataBytes = Encoding.UTF8.GetBytes(data);
            string signature = "";

            using (var hmac = new System.Security.Cryptography.HMACSHA256(secretBytes))
            {
                var hash = hmac.ComputeHash(dataBytes);
                for (int i = 0; i < hash.Length; i++)
                {
                    signature += hash[i].ToString("x2");
                }
                //    signature = Convert.ToBase64String(hash);
            }
            return signature;
        }

    }
}
