using System.Collections.Generic;
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
        public string DeviceId { get; private set; }

        [ProtoMember(3)]
        public string Manufacturer { get; private set; }

        [ProtoMember(4)]
        public string Model { get; private set; }

        [ProtoMember(5)]
        public string PhoneId { get; private set; }

        public string Useragent =>
            string.Format(ConstantVariable.UseragentCommonFormat, (object)ConstantVariable.IgVersion, (object)this.AndroidVersion, (object)this.AndroidRelease, (object)this.Dpi, (object)this.Resolution, (object)this.ManufacturerBrand, (object)this.Model, (object)this.Device, (object)this.Cpu, (object)ConstantVariable.UseragentLocale);

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
                if (string.IsNullOrWhiteSpace(this.Brand))
                    return this.Manufacturer;
                return string.Format("{0}/{1}", (object)this.Manufacturer, (object)this.Brand);
            }
        }

        [ProtoMember(11)]
        private string Resolution { get; set; }


        public void GenerateDetails()
        {
            var splitDeviceDetails = DeviceGenerator.GetRandomDevice().Split(';');
            var splitAndroidDetails = splitDeviceDetails[0].Split('/');
            this.AndroidVersion = splitAndroidDetails[0];
            this.AndroidRelease = splitAndroidDetails[1];

            this.Dpi = splitDeviceDetails[1];
            this.Resolution = splitDeviceDetails[2];

            var splitManufacture = splitDeviceDetails[3].Split('/');
            this.Manufacturer = splitManufacture[0];
            if (splitManufacture.Length == 2)
                this.Brand = splitManufacture[1];

            this.Model = splitDeviceDetails[4];
            this.Device = splitDeviceDetails[5];
            this.Cpu = splitDeviceDetails[6];
            this.PhoneId = Utilities.GetGuid(true); ;
            this.DeviceId = Utilities.GetMobileDeviceId(this.PhoneId);
       
        }

        /// <summary>
        /// GetRandomDevice method is used to get the random device details from the array of list
        /// </summary>
        /// <returns>Return the any one from list of devices</returns>
        private static string GetRandomDevice()
        {

            // retrun any one from device items
            return ((IList<string>)new string[6]
            {

                //23/6.0.1; 640dpi; 1440x2560; ZTE; ZTE A2017U; ailsa_ii; qcom;en_US
                /*Format : AndroidVersion/AndroidRelease; dpi; Resolutions; Manufacturer/Brand; Model; Device; CPU  */
                "24/7.0;380dpi;1080x1920;OnePlus;ONEPLUS A3010;OnePlus3T;qcom",
                "23/6.0.1;640dpi;1440x2392;LGE/lge;RS988;h1;h1",
                "24/7.0;640dpi;1440x2560;HUAWEI;LON-L29;HWLON;hi3660",
                "23/6.0.1;640dpi;1440x2560;ZTE;ZTE A2017U;ailsa_ii;qcom",
                "23/6.0.1;640dpi;1440x2560;samsung;SM-G935F;hero2lte;samsungexynos8890",
                "23/6.0.1;640dpi;1440x2560;samsung;SM-G930F;herolte;samsungexynos8890"
            }).GetRandomItem();
        }


        /// <summary>
        /// Generate a unique id for each device
        /// </summary>
        /// <returns></returns>
        public static string GenerateGuid()
        {
            var rand = new System.Random();
            return string.Format("{0}{1}-{2}-{3}-{4}-{5}{6}{7}",
                rand.Next(0, 65535).ToString("x4"),
                rand.Next(0, 65535).ToString("x4"),
                rand.Next(0, 65535).ToString("x4"),
                rand.Next(16384, 20479).ToString("x4"),
                rand.Next(32768, 49151).ToString("x4"),
                rand.Next(0, 65535).ToString("x4"),
                rand.Next(0, 65535).ToString("x4"),
                rand.Next(0, 65535).ToString("x4"));
        }


        /// <summary>
        /// Generate Signature
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>

        public static string GenerateSignature(string data)
        {//0443b39a54b05f064a4917a3d1da4d6524a3fb0878eacabf1424515051674daa (new)
            //25eace5393646842f0d0c3fb2ac7d3cfa15c052436ee86b5406a8433f54d24a5 (old)
            string secret = "25eace5393646842f0d0c3fb2ac7d3cfa15c052436ee86b5406a8433f54d24a5";
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
