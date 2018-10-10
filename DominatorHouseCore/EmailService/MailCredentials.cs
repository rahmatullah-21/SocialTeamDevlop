using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.Utility;
using OpenPop.Mime;
using OpenPop.Pop3;
using ProtoBuf;

namespace DominatorHouseCore.EmailService
{
    [ProtoContract]
    public class MailCredentials : BindableBase
    {
        private string _hostname;
        private int _port;
        private string _username;
        private string _password;

        [ProtoMember(1)]
        public string Hostname
        {
            get { return _hostname; }
            set
            {
                if( _hostname == value)return;
                SetProperty(ref _hostname, value);
            }
        }

        [ProtoMember(2)]
        public int Port
        {
            get { return _port; }
            set
            {
                if (_port == value)return;
                SetProperty(ref _port, value);
            }
        }

        [ProtoMember(3)]
        public string Username
        {
            get { return _username; }
            set
            {
                if (_username == value) return;
                SetProperty(ref _username, value);
            }
        }

        [ProtoMember(4)]
        public string Password
        {
            get { return _password; }
            set
            {
                if (_password == value) return;
                SetProperty(ref _password, value);
            }
        }
    }

    public class IncomingData
    {
        public string From { get; set; }
        public string Subject { get; set; }
        public string Date { get; set; }
        public string Message { get; set; }
    }

    public class EmailClient
    {

        public static List<IncomingData> FetchAllMessages(MailCredentials mailCredentials, bool sslRequired)
        {
            // The client disconnects from the server when being disposed
            using (Pop3Client client = new Pop3Client())
            {
                int messageCount = ConnectAndGetMessageCount(mailCredentials, sslRequired, client);

                // We want to download all messages
                List<IncomingData> allMessages = new List<IncomingData>(messageCount);

                // Messages are numbered in the interval: [1, messageCount]
                // Ergo: message numbers are 1-based.
                // Most servers give the latest message the highest number
                for (int i = messageCount; i > 0; i--)
                {
                    var a = client.GetMessage(i);
                    var mailData = new IncomingData
                    {
                        From = a.Headers.From.HasValidMailAddress
                            ? a.Headers.From.MailAddress.Address
                            : a.Headers.From.Raw,
                        Date = a.Headers.Date,
                        Subject = a.Headers.Subject,
                        Message = a.MessagePart.ContentDescription
                    };

                    allMessages.Add(mailData);
                    // var mailMessage = a.MessagePart.GetBodyAsText();
                    // var messagePart = a.MessagePart.ContentDescription;
                    //string result = System.Text.Encoding.UTF8.GetString(a.RawMessage);
                    //var b = a.Headers.From;
                    //var c = a.Headers.Keywords;
                    //var d = a.Headers.Date;
                    //var e = a.Headers.Subject;
                }

                // Now return the fetched messages
                return allMessages;
            }
        }

        public static List<IncomingData> FetchLastXMailsFromSender(MailCredentials mailCredentials, bool sslRequired,
            string senderEmail, int requiredEmailsCount)
        {
            using (Pop3Client client = new Pop3Client())
            {
                int messageCount = ConnectAndGetMessageCount(mailCredentials, sslRequired, client);
                List<IncomingData> allMessages = new List<IncomingData>(messageCount);
                for (int i = messageCount; i > 0; i--)
                {
                    if (requiredEmailsCount == 0) break;
                    var a = client.GetMessage(i);
                    if (!a.Headers.From.Raw.Contains(senderEmail)) continue;
                    var mailData = new IncomingData
                    {
                        From = a.Headers.From.HasValidMailAddress
                            ? a.Headers.From.MailAddress.Address
                            : a.Headers.From.Raw,
                        Date = a.Headers.Date,
                        Subject = a.Headers.Subject,
                        Message = a.MessagePart.ContentDescription
                    };
                    allMessages.Add(mailData);
                    requiredEmailsCount--;
                }
                return allMessages;
            }

        }

        public static List<IncomingData> FetchLastXMails(MailCredentials mailCredentials, bool sslRequired, int requiredEmailsCount)
        {
            using (Pop3Client client = new Pop3Client())
            {
                int messageCount = ConnectAndGetMessageCount(mailCredentials, sslRequired, client);
                List<IncomingData> allMessages = new List<IncomingData>(messageCount);
                for (int i = messageCount; i > 0; i--)
                {
                    if (requiredEmailsCount == 0) break;
                    var a = client.GetMessage(i);
                    var mailData = new IncomingData
                    {
                        From = a.Headers.From.HasValidMailAddress
                            ? a.Headers.From.MailAddress.Address
                            : a.Headers.From.Raw,
                        Date = a.Headers.Date,
                        Subject = a.Headers.Subject,
                        Message = a.MessagePart.ContentDescription
                    };
                    allMessages.Add(mailData);
                    requiredEmailsCount--;
                }
                return allMessages;
            }

        }

        public static IncomingData FetchLastMailFromSender(MailCredentials mailCredentials, bool sslRequired,
            string senderEmail)
        {
            using (Pop3Client client = new Pop3Client())
            {
                int messageCount = ConnectAndGetMessageCount(mailCredentials, sslRequired, client);
                for (int i = messageCount; i > 0; i--)
                {
                    var a = client.GetMessage(i);
                    var mailData = new IncomingData();
                    if (!a.Headers.From.Raw.Contains(senderEmail)) continue;
                    mailData.From = senderEmail;
                    mailData.Date = a.Headers.Date;
                    mailData.Subject = a.Headers.Subject;
                    mailData.Message = a.MessagePart.ContentDescription;
                    return mailData;
                }
            }
            return null;
        }
        private static int ConnectAndGetMessageCount(MailCredentials mailCredentials, bool sslRequired, Pop3Client client)
        {
            ConnectToMailServer(mailCredentials, sslRequired, client);

            // Get the number of messages in the inbox
            int messageCount = client.GetMessageCount();
            return messageCount;
        }

        private static void ConnectToMailServer(MailCredentials mailCredentials, bool sslRequired, Pop3Client client)
        {
            // Connect to the server
            client.Connect(mailCredentials.Hostname, mailCredentials.Port, sslRequired);

            // Authenticate ourselves towards the server
            client.Authenticate(mailCredentials.Username, mailCredentials.Password, AuthenticationMethod.UsernameAndPassword);
        }
    }

}
