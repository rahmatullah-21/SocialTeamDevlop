using SQLite.CodeFirst;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.DatabaseHandler.LdTables.Account
{
    public class Connections
    {
        [Key]
        [Autoincrement]
        [Index]
        [Column(Order = 1)]
        public int Id { get; set; }

        /// <summary>
        /// Describes if Detailed UserInfo has been stored atlest once or not
        /// </summary>
        [Column(Order = 2)]
        public string IsDetailedUserInfoStored
        { get; set; }

        /// <summary>
        /// Describes if Detailed UserInfo Is Visible 
        /// </summary>
        [Column(Order = 3)]
        public string IsDetailedUserInfoVisible
        { get; set; }

        /// <summary>
        /// Contains FullName Of the Connection
        /// </summary>
        [Column(Order = 4)]
        public string FullName
        { get; set; }

        /// <summary>
        /// Contains ProfileUrl Of the Connection
        /// </summary>
        [Column(Order = 5)]
        [Unique]
        public string ProfileUrl
        { get; set; }

        /// <summary>
        /// Describes if Connection Has Anonymous Profile Picture or not
        /// </summary>
        [Column(Order = 6)]
        public string HasAnonymousProfilePicture { get; set; }

        /// <summary>
        /// Contains Profile Picture Url Of the Connection
        /// </summary>
        [Column(Order = 7)]
        public string ProfilePicUrl { get; set; }

        /// <summary>
        /// Describe Connection Type If FirstDegree,SecondDegree Or ThirdPlusDegree
        /// </summary>
        [Column(Order = 8)]
        public ConnectionType ConnectionType
        { get; set; }

        /// <summary>
        /// Contains Connected TimeStamp with this Account
        /// </summary>
        [Column(Order = 9)]
        public Int64 ConnectedTimeStamp { get; set; }

        /// <summary>
        /// Contains Occupation Of the Connection
        /// </summary>
        [Column(Order = 10)]
        public string Occupation { get; set; }


        /// <summary>
        /// Contains CompanyName Of the Connection
        /// </summary>
        [Column(Order = 11)]
        public string CompanyName { get; set; }

        /// <summary>
        /// Contains Detailed Info int JasonString Of the Connection
        /// </summary>
        [Column(Order = 12)]
        public string DetailedUserInfo { get; set; }

        /// <summary>
        /// Contains Interaction Time Stamp
        /// </summary>
        [Column(Order = 13)]
        public int InteractionTimeStamp { get; set; }
    }


    public enum ConnectionType : int
    {
        FirstDegree = 1,
        SeondDegree = 2,
        ThirdPlusDegree = 3
    }
}
