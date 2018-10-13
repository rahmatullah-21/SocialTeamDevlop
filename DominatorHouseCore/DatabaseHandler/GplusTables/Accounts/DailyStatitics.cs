using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SQLite;
using System;

namespace DominatorHouseCore.DatabaseHandler.GplusTables.Accounts
{
    public class DailyStatitics
    {
        [PrimaryKey]
        [AutoIncrement]
        [Indexed]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public DateTime Date
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public int Followers
        { get; set; }

        /// <summary>
        /// LinkedinGroups count of the DB owner when the statistics has got updated
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public int Followings
        { get; set; }

        /// <summary>
        /// Posts count of the DB owner when the statistics has got updated
        /// </summary>

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public int Communities
        { get; set; }

    }
}
